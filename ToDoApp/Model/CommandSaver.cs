using System.Collections.Generic;
using System.Data.SqlClient;
using ToDoApp.Model.Enums;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class CommandSaver
    {
        private Queue<(HttpRequestType, IEntity)> _commandQueue;
        
        public void AddCommand(HttpRequestType type, IEntity entity)
        {
            _commandQueue.Enqueue((type, entity));
        }

        public (HttpRequestType, IEntity) GetCommand()
        {
            return _commandQueue.Dequeue();
        }
    }
}