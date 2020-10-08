using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ToDoApp.Model;

namespace ToDoApp.ViewModel
{
    public class ToDoAppViewModel : INotifyPropertyChanged
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ObservableCollection<ToDoListViewModel> DefaultToDoLists { get; set; } =
            new ObservableCollection<ToDoListViewModel>();

        public ObservableCollection<ToDoListViewModel> ToDoLists { get; set; } =
            new ObservableCollection<ToDoListViewModel>();

        public BindingList<TaskViewModel> CurrentTasksList { get; set; } = new BindingList<TaskViewModel>();

        private TaskViewModel _currentTask;

        public TaskViewModel CurrentTask
        {
            get => _currentTask;
            set
            {
                _currentTask = value;
                OnPropertyChanged();
            }
        }

        private ToDoListViewModel _currentList;

        public ToDoListViewModel CurrentList
        {
            get => _currentList;
            set
            {
                _currentList = value;
                OnPropertyChanged();
            }
        }

        private bool _isTaskEditing;

        public bool IsTaskEditing
        {
            get => _isTaskEditing;
            set
            {
                _isTaskEditing = value;
                OnPropertyChanged();
                OnPropertyChanged("IsNotTaskEditing");
            }
        }

        public bool IsNotTaskEditing => !IsTaskEditing;
        public bool IsRepeatComboboxEnabled => CurrentTask.Date.HasValue;

        public ToDoAppViewModel()
        {
            CurrentTask = new TaskViewModel(new Task());
            CurrentList = new ToDoListViewModel(new ToDoList());
            OnPropertyChanged("IsRepeatComboboxEnabled");
            //CurrentList = DefaultToDoLists[0];
            // InitializeAllLists();
        }


        private void InitializeAllLists()
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
                           var newList = new ToDoList
                           {
                               Name = obj as string
                           };
                           _unitOfWork.ToDoLists.Add(newList);
                           ToDoLists.Add(new ToDoListViewModel(newList));
                           CurrentList = ToDoLists.Last();
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
                           _unitOfWork.ToDoLists.Update(choosenToDoList.ToDoList);
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
                           _unitOfWork.ToDoLists.Remove(choosenToDoList.ToDoList);
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
                               _unitOfWork.Tasks.Add(CurrentTask.Task);
                               CurrentTasksList.Insert(0, CurrentTask);
                               CurrentTask = new TaskViewModel(new Task
                               {
                                   ToDoList = CurrentList.ToDoList
                               });
                               OnPropertyChanged("IsRepeatComboboxEnabled");
                           }
                       ));
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
                           var choosenItem = obj as ListBoxItem;
                           var choosenTask = choosenItem.Content as TaskViewModel;
                           CurrentTask = choosenTask;
                           IsTaskEditing = true;
                           OnPropertyChanged("IsRepeatComboboxEnabled");
                       }));
            }
        }

        private RelayCommand _saveTaskCommand;

        public RelayCommand SaveTaskCommand
        {
            get
            {
                return _saveTaskCommand ??
                       (_saveTaskCommand = new RelayCommand(obj =>
                       {
                           _unitOfWork.Tasks.Update(CurrentTask.Task);
                           IsTaskEditing = false;
                           CurrentTask = new TaskViewModel(new Task());
                           OnPropertyChanged("IsRepeatComboboxEnabled");
                       }, obj => IsTaskEditing));
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
                           _unitOfWork.Tasks.Remove(choosenTask.Task);
                           CurrentTasksList.Remove(choosenTask);
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
                           OnPropertyChanged("IsRepeatComboboxEnabled");
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}