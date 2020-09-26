using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ToDoApp.Model
{
    public class ToDoList
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Task> Tasks { get; set; }

        public ToDoList(string name)
        {
            Name = name;
        }
    }
}