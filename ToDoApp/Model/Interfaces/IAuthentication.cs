using System;
using System.Threading.Tasks;
using ToDoApp.Model.Models;

namespace ToDoApp.Model.Interfaces
{
    public interface IAuthentication
    {
        User CurrentUser { get; }
        AccessToken CurrentToken { get; }
        Task<bool> Register(string username, string email, string password);
        Task<(bool isSuccess, AccessToken token)> Login(string username, string password);
        System.Threading.Tasks.Task Logout();
    }
}