using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Models;

namespace ToDoApp.Model.Interfaces
{
    public interface IStore
    {
        ApplicationContext DbContext { get; }
        Task<bool> Sync(User user);
    }
}