using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Model
{
    interface IRepository : IDisposable
    {
        IEnumerable<ToDoList> GetToDoLists(); 
        
        void CreateTask(Task item);
        void UpdateTask(Task item);
        void DeleteTask(int id);

        void CreateToDoList(ToDoList item);
        void UpdateToDoList(ToDoList item);
        void DeleteToDoList(int id);

        void Save();
    }
}
