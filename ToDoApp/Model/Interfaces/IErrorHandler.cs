using System;

namespace ToDoApp.Model.Interfaces
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}