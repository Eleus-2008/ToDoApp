using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Enums;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class SavingCommandDbSet<T> : IRepository<T> where T : class, IEntity
    {
        private readonly CommandSaver _commandSaver;
        private readonly DbSet<T> _repository;
        private readonly DbContext _context;

        public SavingCommandDbSet(CommandSaver commandSaver, DbSet<T> repository, DbContext context)
        {
            _commandSaver = commandSaver;
            _repository = repository;
            _context = context;
        }

        public List<T> GetAll()
        {
            return _repository.ToList();
        }

        public void Add(T item)
        {
            _repository.Add(item);
            _commandSaver.AddCommand(HttpRequestType.Put, item);
            _context.SaveChanges();
        }

        public void Update(T item)
        {
            _repository.Update(item);
            _commandSaver.AddCommand(HttpRequestType.Post, item);
            _context.SaveChanges();
        }

        public void Remove(T item)
        {
            _repository.Remove(item);
            _commandSaver.AddCommand(HttpRequestType.Delete, item);
            _context.SaveChanges();
        }
    }
}