using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ToDoApp.Model;

namespace ToDoApp.ViewModel
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        public ToDoList ToDoList { get; }

        public string Name
        {
            get => ToDoList.Name;
            set
            {
                ToDoList.Name = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<TaskViewModel> Tasks => ToDoList.Tasks.Select(task => new TaskViewModel(task));


        public ToDoListViewModel(ToDoList toDoList)
        {
            ToDoList = toDoList;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}