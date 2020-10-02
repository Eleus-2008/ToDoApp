using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class UnitOfWork
    {
        private readonly ApplicationContext _localContext = new ApplicationContext();
        private readonly ServerContext _serverContext;

        private readonly CommandSaver _commandSaver = new CommandSaver();

        public IRepository<Task> Tasks { get; private set; }
        public IRepository<ToDoList> ToDoLists { get; private set; }

        public UnitOfWork()
        {
            _serverContext = new ServerContext(_commandSaver);
            
            Tasks = new SavingCommandDbSet<Task>(_commandSaver, _localContext.Tasks, _localContext);
            ToDoLists = new SavingCommandDbSet<ToDoList>(_commandSaver, _localContext.ToDoLists, _localContext);
        }
    }
}