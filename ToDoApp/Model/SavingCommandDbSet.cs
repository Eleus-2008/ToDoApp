using System.Collections.Generic;
using System.Threading.Tasks;
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
            _repository.Load();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _repository.ToListAsync();
        }

        public async System.Threading.Tasks.Task AddAsync(T item)
        {
            await System.Threading.Tasks.Task.Run(() => _repository.Add(item));
            _commandSaver.AddCommand(HttpRequestType.Put, item);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task UpdateAsync(T item)
        {
            await System.Threading.Tasks.Task.Run(() => _repository.Update(item));
            _commandSaver.AddCommand(HttpRequestType.Post, item);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task RemoveAsync(T item)
        {
            await System.Threading.Tasks.Task.Run(() => _repository.Remove(item));
            _commandSaver.AddCommand(HttpRequestType.Delete, item);
            await _context.SaveChangesAsync();
        }
    }
}