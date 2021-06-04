using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ToDoApp.Model.DTO;
using ToDoApp.Model.Interfaces;
using ToDoApp.Model.Mapper;
using ToDoApp.Model.Models;

namespace ToDoApp.Model.Services
{
    public class Store : IStore
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public ApplicationContext DbContext { get; private set; } = new ApplicationContext();

        public Store(HttpClient httpClient)
        {
            _httpClient = httpClient;
            var config = new MapperConfiguration(expression => expression.AddProfile(new MappingProfile()));
            _mapper = config.CreateMapper();
        }

        public async Task<bool> Sync(User user)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", user.Token.Token);
                var updatingResponse =
                    await _httpClient.GetAsync("api/todolists/getupdatedtodolists?lastUpdateTime=" +
                                               JsonConvert.SerializeObject(user.LastSyncTime).Trim('"'));
                if (!updatingResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                var updatedListsDtos =
                    JsonConvert.DeserializeObject<IEnumerable<ToDoListDto>>(await updatingResponse.Content
                        .ReadAsStringAsync());

                if (updatedListsDtos == null)
                {
                    return false;
                }

                var updatedLists = _mapper.Map<IEnumerable<ToDoList>>(updatedListsDtos);
                foreach (var list in updatedLists)
                {
                    var listTasks = list.Tasks;
                    list.Tasks = null;

                    var currentList = await DbContext.ToDoLists.FindAsync(list.Id);
                    if (currentList != null)
                    {
                        if (list.IsDeleted)
                        {
                            DbContext.ToDoLists.Remove(currentList);
                        }
                        else
                        {
                            currentList.Name = list.Name;
                            currentList.IsUpdated = false;
                            DbContext.ToDoLists.Update(currentList);
                        }
                    }
                    else
                    {
                        list.User = user;
                        DbContext.ToDoLists.Add(list);
                    }

                    foreach (var task in listTasks)
                    {
                        var currentTask = await DbContext.Tasks.FindAsync(task.Id);
                        if (currentTask != null)
                        {
                            if (task.IsDeleted)
                            {
                                DbContext.Tasks.Remove(currentTask);
                            }
                            else
                            {
                                currentTask.Name = task.Name;
                                currentTask.IsDone = task.IsDone;
                                currentTask.Priority = task.Priority;
                                currentTask.Date = task.Date;
                                currentTask.TimeOfBeginning = task.TimeOfBeginning;
                                currentTask.TimeOfEnd = task.TimeOfEnd;
                                currentTask.RepeatingConditions = task.RepeatingConditions;
                                currentTask.IsUpdated = false;
                                DbContext.Tasks.Update(currentTask);
                            }
                        }
                        else
                        {
                            DbContext.Tasks.Add(task);
                        }
                    }
                }

                // загрузка обновленных локально сущностей на сервер
                var uploadingLists = DbContext.ToDoLists
                    .Where(list => list.User == user && (list.IsUpdated || list.IsDeleted || list.IsAdded))
                    .Include(list => list.Tasks).ToList();
                foreach (var list in uploadingLists)
                {
                    list.Tasks.RemoveAll(task => !(task.IsUpdated || task.IsDeleted || task.IsAdded));
                }

                var currentLists = uploadingLists.Where(list => list.IsDeleted).ToList();
                var uploadingListsDtos = _mapper.Map<IEnumerable<ToDoListDto>>(currentLists);
                DbContext.ToDoLists.RemoveRange(currentLists);
                uploadingLists.RemoveAll(list => list.IsDeleted);
                var json = JsonConvert.SerializeObject(uploadingListsDtos.Select(list => list.Id));
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var uploadingResponse = await _httpClient.PostAsync("api/todolists/deletetodolists", data);
                if (!uploadingResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                var currentTasks = uploadingLists.SelectMany(list =>
                    list.Tasks.Where(task => task.IsDeleted)).ToList();
                var uploadingTasksDtos = _mapper.Map<IEnumerable<ToDoListDto>>(currentTasks);
                DbContext.Tasks.RemoveRange(currentTasks);
                uploadingLists.ForEach(list => list.Tasks.RemoveAll(task => task.IsDeleted));
                json = JsonConvert.SerializeObject(uploadingTasksDtos.Select(task => task.Id));
                data = new StringContent(json, Encoding.UTF8, "application/json");
                uploadingResponse = await _httpClient.PostAsync("api/tasks/deletetasks", data);
                if (!uploadingResponse.IsSuccessStatusCode)
                {
                    return false;
                }
                
                currentLists = uploadingLists.Where(list => list.IsAdded || list.IsUpdated).ToList();
                uploadingListsDtos = _mapper.Map<IEnumerable<ToDoListDto>>(currentLists);
                currentLists.ForEach(list =>
                {
                    list.IsAdded = false;
                    list.IsUpdated = false;
                    DbContext.ToDoLists.Update(list);
                });
                currentLists.SelectMany(list => list.Tasks).ToList().ForEach(task =>
                {
                    task.IsAdded = false;
                    task.IsUpdated = false;
                    DbContext.Tasks.Update(task);
                });
                // uploadingLists.RemoveAll(list => list.IsAdded);
                json = JsonConvert.SerializeObject(uploadingListsDtos);
                data = new StringContent(json, Encoding.UTF8, "application/json");
                uploadingResponse = await _httpClient.PostAsync("api/todolists/addtodolists", data);
                if (!uploadingResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                currentTasks = uploadingLists.SelectMany(list =>
                    list.Tasks.Where(task => task.IsAdded)).ToList();
                uploadingTasksDtos = _mapper.Map<IEnumerable<ToDoListDto>>(currentTasks);
                currentTasks.ForEach(task =>
                {
                    task.IsAdded = false;
                    task.IsUpdated = false;
                    DbContext.Tasks.Update(task);
                });
                json = JsonConvert.SerializeObject(uploadingTasksDtos);
                data = new StringContent(json, Encoding.UTF8, "application/json");
                uploadingResponse = await _httpClient.PostAsync("api/tasks/addtasks", data);
                if (!uploadingResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                currentLists = uploadingLists.Where(list => list.IsUpdated).ToList();
                uploadingListsDtos = _mapper.Map<IEnumerable<ToDoListDto>>(currentLists);
                currentLists.ForEach(list =>
                {
                    list.IsUpdated = false;
                    DbContext.ToDoLists.Update(list);
                });
                currentLists.SelectMany(list => list.Tasks).ToList().ForEach(task =>
                {
                    task.IsUpdated = false;
                    DbContext.Tasks.Update(task);
                });
                uploadingLists.RemoveAll(list => list.IsUpdated);
                json = JsonConvert.SerializeObject(uploadingListsDtos);
                data = new StringContent(json, Encoding.UTF8, "application/json");
                uploadingResponse = await _httpClient.PostAsync("api/todolists/updatetodolists", data);
                if (!uploadingResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                currentTasks = uploadingLists.SelectMany(list =>
                    list.Tasks.Where(task => task.IsUpdated)).ToList();
                uploadingTasksDtos = _mapper.Map<IEnumerable<ToDoListDto>>(currentTasks);
                currentTasks.ForEach(task =>
                {
                    task.IsUpdated = false;
                    DbContext.Tasks.Update(task);
                });
                json = JsonConvert.SerializeObject(uploadingTasksDtos);
                data = new StringContent(json, Encoding.UTF8, "application/json");
                uploadingResponse = await _httpClient.PostAsync("api/tasks/updatetasks", data);
                if (!uploadingResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                user.LastSyncTime = DateTime.UtcNow;
                DbContext.Update(user);
                await DbContext.SaveChangesAsync();
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return true;
            }
            catch
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                await DbContext.DisposeAsync();
                DbContext = new ApplicationContext();
                return false;
            }
        }
    }
}