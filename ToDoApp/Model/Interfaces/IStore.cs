using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Model.Interfaces
{
    public interface IStore
    {
        ApplicationContext DbContext { get; }
        bool Sync(string token);
    }
}