using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ToDoApp.Model;

namespace ToDoApp.ViewModel
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        private ToDoList _toDoList;

        public ToDoList ToDoList => _toDoList;

        public string Name
        {
            get => _toDoList.Name;
            set
            {
                _toDoList.Name = value;
                OnPropertyChanged();
            }
        }

        public int Id => _toDoList.Id;

        public IEnumerable<TaskViewModel> Tasks => _toDoList.GetTasks.Select(task => new TaskViewModel(task));


        public ToDoListViewModel(ToDoList toDoList)
        {
            _toDoList = toDoList;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}