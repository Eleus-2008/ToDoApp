using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToDoApp.Model.DTO;
using ToDoApp.Model.Interfaces;
using ToDoApp.Model.Models;


namespace ToDoApp.Model.Services
{
    public class Authentication : IAuthentication
    {
        private readonly HttpClient _client;
        private readonly ApplicationContext _context;

        public User CurrentUser { get; private set; }
        public AccessToken CurrentToken { get; private set; }

        public Authentication(ApplicationContext context)
        {
            _context = context;
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
            var guest = context.Users.FirstOrDefault(user => user.Username == "guest");
            if (guest == null)
            {
                CurrentUser = new User
                {
                    Username = "guest"
                };
            }
            else
            {
                CurrentUser = guest;
            }

        }
        
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

        public async Task<(bool isSuccess, AccessToken token)> Login(string username,
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
                    var result = JsonConvert.DeserializeObject<AccessToken>(await response.Content.ReadAsStringAsync());
                    if (result != null)
                    {
                        var user = _context.Users.FirstOrDefault(u => u.Username == username.ToLower());
                        if (user == null)
                        {
                            CurrentUser = new User
                            {
                                Username = username
                            };
                            result.User = CurrentUser;
                        }
                        else
                        {
                            CurrentUser = user;
                        }
                        CurrentUser.Token = result;
                        CurrentToken = result;

                        _context.Tokens.Add(result);
                        await _context.SaveChangesAsync();
                        
                        return (true, result);
                    }

                    return (false, null);
                }
                else
                {
                    return (false, null);
                }
            }
            catch 
            {
                return (false, null);
            }
        }

        public async System.Threading.Tasks.Task Logout()
        {
            _context.Tokens.Remove(CurrentToken);
            await _context.SaveChangesAsync();
            
            CurrentUser = null;
            CurrentToken = null;
        }
    }
}