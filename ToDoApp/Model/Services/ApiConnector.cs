using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using ToDoApp.Model.DTO;
using ToDoApp.Model.Mapper;

namespace ToDoApp.Model.Services
{
    public class ApiConnector
    {
        private const string GET_TODOLISTS_URI = "todolists/getupdatedtodolists?lastUpdateTime=";
        private const string ADD_TODOLISTS_URI = "todolists/addtodolists";
        private const string ADD_TASKS_URI = "tasks/addtasks";
        private const string UPDATE_TODOLISTS_URI = "todolists/updatetodolists";
        private const string UPDATE_TASKS_URI = "tasks/updatetasks";
        private const string DELETE_TODOLISTS_URI = "todolists/deletetodolists";
        private const string DELETE_TASKS_URI = "tasks/deletetasks";

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public ApiConnector(HttpClient httpClient)
        {
            _httpClient = httpClient;
            var config = new MapperConfiguration(expression => expression.AddProfile(new MappingProfile()));
            _mapper = config.CreateMapper();
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void RemoveCurrentToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<(bool, IEnumerable<ToDoList>)> GetUpdatedToDoListsAsync(DateTime lastSyncDateTime)
        {
            var response =
                await _httpClient.GetAsync(GET_TODOLISTS_URI +
                                           JsonConvert.SerializeObject(lastSyncDateTime).Trim('"'));
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var listsDtos =
                JsonConvert.DeserializeObject<IEnumerable<ToDoListDto>>(await response.Content
                    .ReadAsStringAsync());

            if (listsDtos == null)
            {
                return default;
            }

            return (true, _mapper.Map<IEnumerable<ToDoList>>(listsDtos));
        }

        public async Task<bool> AddToDoListsAsync(IEnumerable<ToDoList> lists)
        {
            var listsDtos = _mapper.Map<IEnumerable<ToDoListDto>>(lists);
            return await SendUpdateRequestAsync(ADD_TODOLISTS_URI, listsDtos);
        }

        public async Task<bool> AddTasksAsync(IEnumerable<Task> tasks)
        {
            var tasksDtos = _mapper.Map<IEnumerable<ToDoListDto>>(tasks);
            return await SendUpdateRequestAsync(ADD_TASKS_URI, tasksDtos);
        }

        public async Task<bool> UpdateToDoListsAsync(IEnumerable<ToDoList> lists)
        {
            var listsDtos = _mapper.Map<IEnumerable<ToDoListDto>>(lists);
            return await SendUpdateRequestAsync(UPDATE_TODOLISTS_URI, listsDtos);
        }

        public async Task<bool> UpdateTasksAsync(IEnumerable<Task> tasks)
        {
            var tasksDtos = _mapper.Map<IEnumerable<ToDoListDto>>(tasks);
            return await SendUpdateRequestAsync(UPDATE_TASKS_URI, tasksDtos);
        }

        public async Task<bool> DeleteToDoListsAsync(IEnumerable<Guid> ids)
        {
            return await SendUpdateRequestAsync(DELETE_TODOLISTS_URI, ids);
        }

        public async Task<bool> DeleteTasksAsync(IEnumerable<Guid> ids)
        {
            return await SendUpdateRequestAsync(DELETE_TASKS_URI, ids);
        }

        private async Task<bool> SendUpdateRequestAsync(string uri, IEnumerable data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var uploadingResponse = await _httpClient.PostAsync(uri, content);
            return uploadingResponse.IsSuccessStatusCode;
        }
    }
}