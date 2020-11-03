using System.Windows.Input;

namespace ToDoApp.Model.Interfaces
{
    public interface IAsyncCommand<T> : ICommand
    {
        System.Threading.Tasks.Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }
}