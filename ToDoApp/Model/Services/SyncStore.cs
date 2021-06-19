using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Interfaces;
using ToDoApp.Model.Models;

namespace ToDoApp.Model.Services
{
    public class SyncStore : ISyncStore
    {
        private readonly ApiConnector _api;
        public ApplicationContext DbContext { get; private set; } = new ApplicationContext();

        public SyncStore(HttpClient httpClient)
        {
            _api = new ApiConnector(httpClient);
        }

        public async Task<bool> Sync(User user)
        {
            try
            {
                _api.SetToken(user.Token.Token);

                var (ok, updatedLists) = await _api.GetUpdatedToDoListsAsync(user.LastSyncTime);
                if (!ok)
                {
                    await FinishFailedSync();
                    return false;
                }
                UpdateLocalStorage(updatedLists, user);

                ok = await UploadLocalUpdates(user);
                if (!ok)
                {
                    await FinishFailedSync();
                    return false;
                }

                await FinishSuccessSync();
                return true;
            }
            catch
            {
                await FinishFailedSync();
                return false;
            }

            async System.Threading.Tasks.Task FinishSuccessSync()
            {
                user.LastSyncTime = DateTime.UtcNow;
                DbContext.Update(user);
                await DbContext.SaveChangesAsync();
                _api.RemoveCurrentToken();
            }
            
            async System.Threading.Tasks.Task FinishFailedSync()
            {
                _api.RemoveCurrentToken();
                await DbContext.DisposeAsync();
                DbContext = new ApplicationContext();
            }
        }

        private async void UpdateLocalStorage(IEnumerable<ToDoList> lists, User user)
        {
            foreach (var list in lists)
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
        }

        private async Task<bool> UploadLocalUpdates(User user)
        {
            var uploadingLists = DbContext.ToDoLists
                    .Where(list => list.User == user && (list.IsUpdated || list.IsDeleted || list.IsAdded))
                    .Include(list => list.Tasks).ToList();
                foreach (var list in uploadingLists)
                {
                    list.Tasks.RemoveAll(task => !(task.IsUpdated || task.IsDeleted || task.IsAdded));
                }

                var deletingLists = uploadingLists.Where(list => list.IsDeleted).ToList();
                var ok = await _api.DeleteToDoListsAsync(deletingLists.Select(list => list.Id));
                if (!ok)
                {
                    return false;
                }
                DbContext.ToDoLists.RemoveRange(deletingLists);
                uploadingLists.RemoveAll(list => list.IsDeleted);

                var deletingTasks = uploadingLists.SelectMany(list =>
                    list.Tasks.Where(task => task.IsDeleted)).ToList();
                ok = await _api.DeleteTasksAsync(deletingTasks.Select(task => task.Id));
                if (!ok)
                {
                    return false;
                }
                DbContext.Tasks.RemoveRange(deletingTasks);
                uploadingLists.ForEach(list => list.Tasks.RemoveAll(task => task.IsDeleted));

                var addingLists = uploadingLists.Where(list => list.IsAdded || list.IsUpdated).ToList();
                ok = await _api.AddToDoListsAsync(addingLists);
                if (!ok)
                {
                    return false;
                }
                addingLists.ForEach(list =>
                {
                    list.IsAdded = false;
                    list.IsUpdated = false;
                    DbContext.ToDoLists.Update(list);
                });
                addingLists.SelectMany(list => list.Tasks).ToList().ForEach(task =>
                {
                    task.IsAdded = false;
                    task.IsUpdated = false;
                    DbContext.Tasks.Update(task);
                });

                var addingTasks = uploadingLists.SelectMany(list =>
                    list.Tasks.Where(task => task.IsAdded)).ToList();
                ok = await _api.AddTasksAsync(addingTasks);
                if (!ok)
                {
                    return false;
                }
                addingTasks.ForEach(task =>
                {
                    task.IsAdded = false;
                    task.IsUpdated = false;
                    DbContext.Tasks.Update(task);
                });

                var updatingLists = uploadingLists.Where(list => list.IsUpdated).ToList();
                ok = await _api.UpdateToDoListsAsync(updatingLists);
                if (!ok)
                {
                    return false;
                }
                updatingLists.ForEach(list =>
                {
                    list.IsUpdated = false;
                    DbContext.ToDoLists.Update(list);
                });
                updatingLists.SelectMany(list => list.Tasks).ToList().ForEach(task =>
                {
                    task.IsUpdated = false;
                    DbContext.Tasks.Update(task);
                });
                uploadingLists.RemoveAll(list => list.IsUpdated);

                var updatingTasks = uploadingLists.SelectMany(list =>
                    list.Tasks.Where(task => task.IsUpdated)).ToList();
                ok = await _api.UpdateTasksAsync(updatingTasks);
                if (!ok)
                {
                    return false;
                }
                updatingTasks.ForEach(task =>
                {
                    task.IsUpdated = false;
                    DbContext.Tasks.Update(task);
                });

                return true;
        }
    }
}