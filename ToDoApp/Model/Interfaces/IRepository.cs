using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDoApp.Model.Interfaces
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync(); 
        
        System.Threading.Tasks.Task AddAsync(T item);
        System.Threading.Tasks.Task UpdateAsync(T item);
        System.Threading.Tasks.Task RemoveAsync(T item);
    }
}
