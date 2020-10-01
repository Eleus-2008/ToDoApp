using System.Collections.Generic;

namespace ToDoApp.Model.Interfaces
{
    public interface IRepository<T>
    {
        List<T> GetAll(); 
        
        void Add(T item);
        void Update(T item);
        void Remove(T item);
    }
}
