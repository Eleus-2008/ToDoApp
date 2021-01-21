using System;
using System.Threading.Tasks;

namespace ToDoApp.Model.Interfaces
{
    public interface IAuthentication
    {
        string CurrentUser { get; }
        string CurrentToken { get; }
        Task<bool> Register(string username, string email, string password);
        Task<(bool isSuccess, string token, DateTime expirationDate)> Login(string username, string password);
        void Logout();
    }
}