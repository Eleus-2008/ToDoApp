using System;
using System.Collections.Generic;
using ToDoAppWebService.DTO;

namespace ToDoApp.Model.DTO
{
    public class ToDoListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<TaskDto> Tasks { get; set; }
    }
}