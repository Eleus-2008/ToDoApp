using System;
using ToDoApp.Model;

namespace ToDoAppWebService.DTO
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDone { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? TimeOfBeginning { get; set; }
        public TimeSpan? TimeOfEnd { get; set; }
        public int Priority { get; set; }
        public RepeatingConditions RepeatingConditions { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public Guid ToDoListId { get; set; } 
    }
}