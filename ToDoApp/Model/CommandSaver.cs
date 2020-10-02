using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ToDoApp.Model.Enums;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class CommandSaver
    {
        private readonly Queue<(HttpRequestType, IEntity)> _commandQueue = new Queue<(HttpRequestType, IEntity)>();

        public void AddCommand(HttpRequestType type, IEntity entity)
        {
            _commandQueue.Enqueue((type, entity));
        }

        public (HttpRequestType, IEntity) GetCommand()
        {
            return _commandQueue.Peek();
        }

        public void DeleteCommand()
        {
            _commandQueue.Dequeue();
        }

        public IEnumerable<(HttpRequestType, IEntity)> GetAllCommands()
        {
            return _commandQueue.AsEnumerable();
        }

        public void DeleteAllCommands()
        {
            _commandQueue.Clear();
        }
    }
}