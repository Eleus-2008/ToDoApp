using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToDoApp.Model.DTO;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model.Services
{
    public class Authentication : IAuthentication
    {
        private readonly HttpClient _client;

        public Authentication()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
        }

        public string CurrentUser { get; private set; }
        public string CurrentToken { get; private set; }

        public async Task<bool> Register(string username, string email, string password)
        {
            var dto = new RegisterDto
            {
                Username = username,
                Email = email,
                Password = password
            };
            var json = JsonConvert.SerializeObject(dto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync("api/authentication/register", data);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool isSuccess, string token, DateTime expirationDate)> Login(string username,
            string password)
        {
            var dto = new LoginDto
            {
                Username = username,
                Password = password
            };
            var json = JsonConvert.SerializeObject(dto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _client.PostAsync("api/authentication/login", data);
                if (response.IsSuccessStatusCode)
                {
                    var type = new {token = "", expiration = new DateTime()};
                    var result =
                        JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), type);
                    if (result != null)
                    {
                        CurrentUser = username;
                        return (true, result.token, result.expiration);
                    }

                    return (false, null, new DateTime());
                }
                else
                {
                    return (false, null, new DateTime());
                }
            }
            catch 
            {
                return (false, null, new DateTime());
            }
        }

        public void Logout()
        {
            CurrentUser = null;
            CurrentToken = null;
        }
    }
}