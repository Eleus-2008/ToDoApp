using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Model.Interfaces
{
    public interface IDataService
    {
        ApplicationContext DbContext { get; }
        bool Sync(string token);
    }
}