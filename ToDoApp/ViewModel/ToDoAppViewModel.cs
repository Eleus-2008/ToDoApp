using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ToDoApp.Model;

namespace ToDoApp.ViewModel
{
    public class ToDoAppViewModel : INotifyPropertyChanged
    {
        private IRepository _repository;

        public ObservableCollection<ToDoListViewModel> DefaultToDoLists { get; set; } = new ObservableCollection<ToDoListViewModel>();
        public ObservableCollection<ToDoListViewModel> ToDoLists { get; set; } = new ObservableCollection<ToDoListViewModel>();

        public BindingList<TaskViewModel> TasksList { get; set; } = new BindingList<TaskViewModel>();

        public TaskViewModel CurrentTask = new TaskViewModel(new Task(""));

        public ToDoListViewModel CurrentList = new ToDoListViewModel(new ToDoList("default"));

        public bool IsTaskEditing { get; private set; }

        public ToDoAppViewModel()
        {
            //CurrentList = DefaultToDoLists[0];
            // UpdateAllLists();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateAllLists()
        {            
            throw new NotImplementedException();
        }

        private RelayCommand _addListCommand;
        public RelayCommand AddListCommand
        {
            get
            {
                return _addListCommand ??
                    (_addListCommand = new RelayCommand(obj =>
                    {
                        var listName = obj as string;
                        var newList = new ToDoList(listName);
                        //repository.CreateToDoList(newList);
                        ToDoLists.Add(new ToDoListViewModel(newList));
                    }));
            }
        }

        private RelayCommand _editListNameCommand;
        public RelayCommand EditListNameCommand
        {
            get
            {
                return _editListNameCommand ??
                    (_editListNameCommand = new RelayCommand(obj =>
                    {
                        var choosenItem = obj as ListBoxItem;
                        var choosenToDoList = choosenItem.Content as ToDoListViewModel;
                        //repository.UpdateToDoList();                        
                    }));
            }
        }

        private RelayCommand _deleteListCommand;
        public RelayCommand DeleteListCommand
        {
            get
            {
                return _deleteListCommand ??
                    (_deleteListCommand = new RelayCommand(obj =>
                    {
                        var choosenItem = obj as ListBoxItem;
                        var choosenToDoList = choosenItem.Content as ToDoListViewModel;
                        //repository.DeleteToDoList(choosenToDoList.Id);
                        ToDoLists.Remove(choosenToDoList);
                    }));
            }
        }

        private RelayCommand _addTaskCommand;
        public RelayCommand AddTaskCommand
        {
            get
            {
                return _addTaskCommand ??
                    (_addTaskCommand = new RelayCommand(obj =>
                    {
                        _repository.CreateTask(CurrentTask.Task);
                        TasksList.Add(CurrentTask);
                    },
                    obj => CurrentTask.Name.Trim() != ""));
            }
        }

        private RelayCommand _editTaskCommand;
        public RelayCommand EditTaskCommand
        {
            get
            {
                return _editTaskCommand ??
                    (_editTaskCommand = new RelayCommand(obj =>
                    {
                        if (obj is TaskViewModel task)
                        {
                            CurrentTask = task;
                            IsTaskEditing = true;
                        }
                    }));
            }
        }

        private RelayCommand _deleteTaskCommand;
        public RelayCommand DeleteTaskCommand
        {
            get
            {
                return _deleteTaskCommand ??
                    (_deleteTaskCommand = new RelayCommand(obj =>
                    {
                        var choosenItem = obj as ListBoxItem;
                        var choosenTask = choosenItem.Content as TaskViewModel;
                        //repository.DeleteTask(choosenTask.Id);
                        TasksList.Remove(choosenTask);
                        //когда будет БД, понять как из самого списка удалить
                    }));
            }
        }

        private RelayCommand _editTimeCommand;
        public RelayCommand EditTimeCommand
        {
            get
            {
                return _editTimeCommand ??
                    (_editTimeCommand = new RelayCommand(obj =>
                    {
                        var dateWindow = new DateWindow(new DateViewModel(CurrentTask));
                        dateWindow.ShowDialog();
                    }));
            }
        }

        private RelayCommand _editRepeatTimeCommand;
        public RelayCommand EditRepeatTimeCommand
        {
            get
            {
                return _editRepeatTimeCommand ??
                    (_editRepeatTimeCommand = new RelayCommand(obj =>
                    {
                        var repeatDateWindow = new RepeatDateWindow(new RepeatDateViewModel(CurrentTask));
                        repeatDateWindow.ShowDialog();
                    }));
            }
        }

    }
}