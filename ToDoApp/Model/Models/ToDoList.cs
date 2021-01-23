using System.Collections.Generic;
using ToDoApp.Model.Interfaces;
using ToDoApp.Model.Models;

namespace ToDoApp.Model
{
    public class ToDoList : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User User { get; set;  }

        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}