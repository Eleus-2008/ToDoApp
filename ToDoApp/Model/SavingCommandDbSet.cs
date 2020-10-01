using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Enums;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class SavingCommandDbSet<T> : IRepository<T> where T : class, IEntity
    {
        private CommandSaver _commandSaver;
        private DbSet<T> _repository;

        public SavingCommandDbSet(CommandSaver commandSaver, DbSet<T> repository)
        {
            _commandSaver = commandSaver;
            _repository = repository;
        }

        public List<T> GetAll()
        {
            return _repository.ToList();
        }

        public void Add(T item)
        {
            _repository.Add(item);
            _commandSaver.AddCommand(HttpRequestType.Put, item);
        }

        public void Update(T item)
        {
            _repository.Update(item);
            _commandSaver.AddCommand(HttpRequestType.Post, item);
        }

        public void Remove(T item)
        {
            _repository.Remove(item);
            _commandSaver.AddCommand(HttpRequestType.Delete, item);
        }
    }
}