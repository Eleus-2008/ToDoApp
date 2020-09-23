using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ToDoApp.Model
{
    public class ToDoList
    {
        private List<Task> _taskList = new List<Task>();
        
        public int Id { get; private set; }
        public string Name { get; set; }

        public IEnumerable<Task> GetTasks => _taskList;
        
        //временный конструктор
        public ToDoList(string name)
        {
            Name = name;
        }

        public ToDoList(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public ToDoList(int id, string name, IEnumerable<Task> tasks)
        {
            Id = id;
            Name = name;
            if (tasks != null)
            {
                _taskList.AddRange(tasks);
            }            
        }
    }
}