namespace ToDoApp.Model
{
    public class ServerContext
    {
        private readonly CommandSaver _commandSaver;

        public ServerContext(CommandSaver commandSaver)
        {
            _commandSaver = commandSaver;
        }
    }
}