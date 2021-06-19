using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Models;

namespace ToDoApp.Model.Interfaces
{
    public interface ISyncStore
    {
        ApplicationContext DbContext { get; }
        Task<bool> Sync(User user);
    }
}