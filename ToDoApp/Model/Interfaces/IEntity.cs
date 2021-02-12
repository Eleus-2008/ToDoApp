using System;

namespace ToDoApp.Model.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}