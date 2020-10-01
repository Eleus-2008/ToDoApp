using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class UnitOfWork
    {
        private ApplicationContext _localContext = new ApplicationContext();
        private ServerContext _serverContext = new ServerContext();

        private CommandSaver _commandSaver = new CommandSaver();

        public IRepository<Task> Tasks { get; private set; }
        public IRepository<ToDoList> ToDoLists { get; private set; }
    }
}