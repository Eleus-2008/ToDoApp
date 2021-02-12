using System;
using System.Collections.Generic;
using ToDoApp.Model.Interfaces;
using ToDoApp.Model.Models;

namespace ToDoApp.Model
{
    public class ToDoList : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public User User { get; set;  }
        
        public bool IsDeleted { get; set; }
        public bool IsUpdated { get; set; }

        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}