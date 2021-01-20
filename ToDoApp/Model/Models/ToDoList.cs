using System.Collections.Generic;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class ToDoList : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}