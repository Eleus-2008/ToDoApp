using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model;
using ToDoApp.Model.Enums;
using ToDoApp.Model.Interfaces;
using ToDoApp.Model.Models;
using ToDoApp.Model.Services;
using ToDoApp.View;

namespace ToDoApp.ViewModel
{
    public class ToDoAppViewModel : INotifyPropertyChanged
    {
        private readonly IStore _store;
        private readonly IAuthentication _authentication;
        private readonly HttpClient _httpClient;

        private ToDoListViewModel _unlistedTasksList;

        public ObservableCollection<ToDoListViewModel> DefaultToDoLists { get; set; } =
            new ObservableCollection<ToDoListViewModel>();


        private ObservableCollection<ToDoListViewModel> _toDoLists = new ObservableCollection<ToDoListViewModel>();

        public ObservableCollection<ToDoListViewModel> ToDoLists
        {
            get => _toDoLists;
            set
            {
                _toDoLists = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TaskViewModel> _currentTasksList;

        public ObservableCollection<TaskViewModel> CurrentTasksList
        {
            get => _currentTasksList;
            set
            {
                _currentTasksList = value;
                TasksView = new ListCollectionView(CurrentTasksList) {CustomSort = new TasksSorter()};
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();
        private int _selectedUserIndex;

        public int SelectedUserIndex
        {
            get => _selectedUserIndex;
            set
            {
                _selectedUserIndex = value;
                OnPropertyChanged();
                _authentication.CurrentUser = Users[_selectedUserIndex];
                InitializeToDoLists(Users[_selectedUserIndex]);
            }
        }

        private ListCollectionView _tasksView;

        public ListCollectionView TasksView
        {
            get => _tasksView;
            set
            {
                _tasksView = value;
                OnPropertyChanged();
            }
        }

        private TaskViewModel _currentTask;

        public TaskViewModel CurrentTask
        {
            get => _currentTask;
            set
            {
                _currentTask = value;
                OnPropertyChanged();
                OnPropertyChanged("IsRepeatComboboxEnabled");
                OnPropertyChanged("SelectedPriority");
                OnPropertyChanged("SelectedRepeatingCondition");
            }
        }

        private ToDoListViewModel _currentList;

        public ToDoListViewModel CurrentList
        {
            get => _currentList;
            set
            {
                if (value.Name == "Задачи")
                {
                    value.ToDoList.Tasks =
                        ToDoLists.SelectMany(list => list.Tasks.Select(task => task.Task))
                            .Union(_unlistedTasksList.ToDoList.Tasks).ToList();
                }

                if (value.Name == "Мой день")
                {
                    value.ToDoList.Tasks = ToDoLists.SelectMany(list => list.Tasks.Select(task => task.Task))
                        .Union(_unlistedTasksList.ToDoList.Tasks)
                        .Where(task =>
                        {
                            if (!task.Date.HasValue)
                            {
                                return true;
                            }

                            return task.Date == DateTime.Today;
                        }).ToList();
                }

                _currentList = value;
                CurrentTasksList = new ObservableCollection<TaskViewModel>(_currentList.Tasks.ToList());
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

        public int SelectedRepeatingCondition
        {
            get
            {
                switch (CurrentTask.TextRepeating)
                {
                    case "": return 0;
                    case "Ежедневно": return 1;
                    case "Рабочие дни": return 2;
                    case "Выходные": return 3;
                    case "Еженедельно": return 4;
                    case "Ежемесячно": return 5;
                    case "Ежегодно": return 6;
                    default: return 7;
                }
            }
            set
            {
                switch (value)
                {
                    case 0:
                        CurrentTask.RepeatingConditions = null;
                        break;
                    case 1:
                        CurrentTask.RepeatingConditions = new RepeatingConditions
                        {
                            Type = TypeOfRepeatTimeSpan.Day,
                            RepeatInterval = 1
                        };
                        break;
                    case 2:
                        CurrentTask.RepeatingConditions = new RepeatingConditions
                        {
                            Type = TypeOfRepeatTimeSpan.DayOfWeek,
                            RepeatInterval = 1,
                            RepeatingDaysOfWeek = new List<DayOfWeek>
                            {
                                DayOfWeek.Monday,
                                DayOfWeek.Tuesday,
                                DayOfWeek.Wednesday,
                                DayOfWeek.Thursday,
                                DayOfWeek.Friday
                            }
                        };
                        break;
                    case 3:
                        CurrentTask.RepeatingConditions = new RepeatingConditions
                        {
                            Type = TypeOfRepeatTimeSpan.DayOfWeek,
                            RepeatInterval = 1,
                            RepeatingDaysOfWeek = new List<DayOfWeek>
                            {
                                DayOfWeek.Saturday,
                                DayOfWeek.Sunday
                            }
                        };
                        break;
                    case 4:
                        CurrentTask.RepeatingConditions = new RepeatingConditions
                        {
                            Type = TypeOfRepeatTimeSpan.DayOfWeek,
                            RepeatInterval = 1,
                            RepeatingDaysOfWeek = new List<DayOfWeek>
                            {
                                CurrentTask.Date.Value.DayOfWeek
                            }
                        };
                        break;
                    case 5:
                        CurrentTask.RepeatingConditions = new RepeatingConditions
                        {
                            Type = TypeOfRepeatTimeSpan.Month,
                            RepeatInterval = 1
                        };
                        break;
                    case 6:
                        CurrentTask.RepeatingConditions = new RepeatingConditions
                        {
                            Type = TypeOfRepeatTimeSpan.Year,
                            RepeatInterval = 1
                        };
                        break;
                }

                OnPropertyChanged();
            }
        }

        public int SelectedPriority
        {
            get
            {
                switch (CurrentTask.TextPriority)
                {
                    case "Низкий": return 0;
                    case "Средний": return 1;
                    case "Высокий": return 2;
                    default: return 1;
                }
            }
            set
            {
                switch (value)
                {
                    case 0:
                        CurrentTask.Priority = 2;
                        break;
                    case 1:
                        CurrentTask.Priority = 5;
                        break;
                    case 2:
                        CurrentTask.Priority = 8;
                        break;
                }

                OnPropertyChanged();
            }
        }

        public ToDoAppViewModel()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
            _store = new Store();
            _authentication = new Authentication(_store.DbContext, _httpClient);


            CurrentTask = new TaskViewModel(new Task());

            foreach (var user in _store.DbContext.Users.Where(user => user.Token != null).Include(user => user.Token)
                .OrderByDescending(user => user.LastLogonTime))
            {
                Users.Add(user);
            }

            if (!Users.Any())
            {
                Users.Add(_store.DbContext.Users.FirstOrDefault(user => user.Username == "guest"));
            }

            SelectedUserIndex = Users.IndexOf(_authentication.CurrentUser);
        }

        private async System.Threading.Tasks.Task InitializeToDoLists(User user)
        {
            var toDoLists = _store.DbContext.ToDoLists
                .Where(list => list.User == user)
                .Include(list => list.Tasks)
                .Select(list => new ToDoListViewModel(list)).ToList();

            if (toDoLists.FirstOrDefault(list => list.Name == "Задачи без списка") == null)
            {
                var unlistedTasksList = new ToDoList
                {
                    Name = "Задачи без списка",
                    User = _authentication.CurrentUser
                };
                _unlistedTasksList = new ToDoListViewModel(unlistedTasksList);
                _store.DbContext.ToDoLists.Add(unlistedTasksList);
                await _store.DbContext.SaveChangesAsync();
            }
            else
            {
                _unlistedTasksList =
                    toDoLists.FirstOrDefault(list => list.Name == "Задачи без списка");
                toDoLists.Remove(toDoLists.Find(list => list.Name == "Задачи без списка"));
            }


            ToDoLists = new ObservableCollection<ToDoListViewModel>(toDoLists);

            if (!DefaultToDoLists.Any())
            {
                DefaultToDoLists.Add(new ToDoListViewModel(new ToDoList
                {
                    Name = "Задачи",
                    Tasks = toDoLists.SelectMany(x => x.ToDoList.Tasks).Union(_unlistedTasksList.ToDoList.Tasks)
                        .ToList()
                }));

                DefaultToDoLists.Add(new ToDoListViewModel(new ToDoList
                {
                    Name = "Мой день",
                    Tasks = toDoLists.SelectMany(x => x.ToDoList.Tasks).Union(_unlistedTasksList.ToDoList.Tasks).Where(
                        task =>
                        {
                            if (!task.Date.HasValue)
                            {
                                return true;
                            }

                            return task.Date == DateTime.Today;
                        }).ToList()
                }));
            }

            CurrentList = DefaultToDoLists[0];
            TasksView = new ListCollectionView(CurrentTasksList) {CustomSort = new TasksSorter()};
        }

        private RelayCommand _registerCommand;

        public RelayCommand RegisterCommand
        {
            get
            {
                return _registerCommand ??
                       (_registerCommand = new RelayCommand(obj =>
                       {
                           var viewModel = new RegisterViewModel(_authentication);
                           var window = new RegisterWindow(viewModel);
                           window.ShowDialog();
                       }));
            }
        }

        private AsyncRelayCommand<object> _loginCommand;

        public AsyncRelayCommand<object> LoginCommand
        {
            get
            {
                return _loginCommand ??
                       (_loginCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           var viewModel = new LoginViewModel(_authentication);
                           var window = new LoginWindow(viewModel);
                           window.ShowDialog();

                           if (_authentication.CurrentUser.Username != "guest" && _store.DbContext.ToDoLists.Any(list => list.User.Username == "guest"))
                           {
                               var guest = _store.DbContext.Users.FirstOrDefault(user => user.Username == "guest");
                               foreach (var list in _store.DbContext.ToDoLists.Where(list => list.User == guest))
                               {
                                   if (list.Name == "Задачи без списка")
                                   {
                                       var todolist = _store.DbContext.ToDoLists.FirstOrDefault(l =>
                                           l.User == _authentication.CurrentUser &&
                                           l.Name == "Задачи без списка");
                                       if (todolist == null)
                                       {
                                           todolist = new ToDoList
                                           {
                                               Name = "Задачи без списка",
                                               User = _authentication.CurrentUser
                                           };
                                       }
                                       foreach (var task in list.Tasks)
                                       {
                                           task.ToDoList = todolist;
                                       }
                                   }
                                   else
                                   {
                                       list.User = _authentication.CurrentUser;
                                   }

                                   await _store.DbContext.SaveChangesAsync();
                                   
                                   Users.Remove(guest);
                               }
                           }

                           if (!Users.Contains(_authentication.CurrentUser))
                           {
                               Users.Add(_authentication.CurrentUser);
                               SelectedUserIndex = Users.IndexOf(_authentication.CurrentUser);   
                           }
                       }));
            }
        }

        private AsyncRelayCommand<object> _logoutCommand;

        public AsyncRelayCommand<object> LogoutCommand
        {
            get
            {
                return _logoutCommand ??
                       (_logoutCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           Users.Remove(_authentication.CurrentUser);
                           await _authentication.Logout();
                           
                           if (!Users.Any())
                           {
                               Users.Add(_store.DbContext.Users.FirstOrDefault(user => user.Username == "guest"));
                           }

                           SelectedUserIndex = Users.IndexOf(_authentication.CurrentUser);
                       }));
            }
        }

        private AsyncRelayCommand<object> _addListCommand;

        public AsyncRelayCommand<object> AddListCommand
        {
            get
            {
                return _addListCommand ??
                       (_addListCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           var textBox = obj as TextBox;
                           var newList = new ToDoList
                           {
                               Name = textBox.Text,
                               User = _authentication.CurrentUser
                           };

                           ToDoLists.Add(new ToDoListViewModel(newList));
                           CurrentList = ToDoLists.Last();
                           textBox.Text = string.Empty;

                           _store.DbContext.ToDoLists.Add(newList);
                           await _store.DbContext.SaveChangesAsync();
                       }));
            }
        }

        private RelayCommand _chooseListCommand;

        public RelayCommand ChooseListCommand
        {
            get
            {
                return _chooseListCommand ??
                       (_chooseListCommand = new RelayCommand(obj =>
                       {
                           var chosenItem = obj as ListBoxItem;
                           var chosenToDoList = chosenItem.Content as ToDoListViewModel;
                           CurrentList = chosenToDoList;
                       }));
            }
        }

        private AsyncRelayCommand<object> _editListNameCommand;

        public AsyncRelayCommand<object> EditListNameCommand
        {
            get
            {
                return _editListNameCommand ??
                       (_editListNameCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           var chosenItem = obj as ListBoxItem;
                           var chosenToDoList = chosenItem.Content as ToDoListViewModel;

                           _store.DbContext.ToDoLists.Update(chosenToDoList.ToDoList);
                           await _store.DbContext.SaveChangesAsync();
                       }));
            }
        }

        private AsyncRelayCommand<object> _deleteListCommand;

        public AsyncRelayCommand<object> DeleteListCommand
        {
            get
            {
                return _deleteListCommand ??
                       (_deleteListCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           var chosenItem = obj as ListBoxItem;
                           var chosenToDoList = chosenItem.Content as ToDoListViewModel;

                           ToDoLists.Remove(chosenToDoList);
                           CurrentList = DefaultToDoLists[0];
                           CurrentTasksList = new ObservableCollection<TaskViewModel>(
                               CurrentTasksList.Where(x => x.Task.ToDoList != chosenToDoList.ToDoList));

                           foreach (var taskViewModel in chosenToDoList.Tasks)
                           {
                               _store.DbContext.Tasks.Remove(taskViewModel.Task);
                           }

                           _store.DbContext.ToDoLists.Remove(chosenToDoList.ToDoList);
                           await _store.DbContext.SaveChangesAsync();
                       }));
            }
        }

        private AsyncRelayCommand<object> _addTaskCommand;

        public AsyncRelayCommand<object> AddTaskCommand
        {
            get
            {
                return _addTaskCommand ??
                       (_addTaskCommand = new AsyncRelayCommand<object>(async obj =>
                           {
                               if (CurrentList == DefaultToDoLists.First(list => list.Name == "Задачи") ||
                                   CurrentList == DefaultToDoLists.First(list => list.Name == "Мой день"))
                               {
                                   CurrentTask.Task.ToDoList = _unlistedTasksList.ToDoList;
                               }
                               else
                               {
                                   CurrentTask.Task.ToDoList = CurrentList.ToDoList;
                               }

                               Application.Current.Dispatcher.Invoke(() => CurrentTasksList.Add(CurrentTask));

                               _store.DbContext.Tasks.Add(CurrentTask.Task);
                               await _store.DbContext.SaveChangesAsync();

                               CurrentTask = new TaskViewModel(new Task());

                               TasksView.Refresh();
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
                           var chosenItem = obj as ListBoxItem;
                           var chosenTask = chosenItem.Content as TaskViewModel;
                           CurrentTask = chosenTask;
                           IsTaskEditing = true;
                       }));
            }
        }

        private AsyncRelayCommand<object> _doTaskCommand;

        public AsyncRelayCommand<object> DoTaskCommand
        {
            get
            {
                return _doTaskCommand ??
                       (_doTaskCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           var chosenItem = obj as ListBoxItem;
                           var chosenTask = chosenItem.Content as TaskViewModel;
                           _store.DbContext.Tasks.Update(chosenTask.Task);
                           await _store.DbContext.SaveChangesAsync();

                           TasksView.Refresh();
                       }));
            }
        }

        private AsyncRelayCommand<object> _saveTaskCommand;

        public AsyncRelayCommand<object> SaveTaskCommand
        {
            get
            {
                return _saveTaskCommand ??
                       (_saveTaskCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           _store.DbContext.Tasks.Update(CurrentTask.Task);
                           await _store.DbContext.SaveChangesAsync();
                           IsTaskEditing = false;
                           CurrentTask = new TaskViewModel(new Task());

                           TasksView.Refresh();
                       }));
            }
        }

        private AsyncRelayCommand<object> _deleteTaskCommand;

        public AsyncRelayCommand<object> DeleteTaskCommand
        {
            get
            {
                return _deleteTaskCommand ??
                       (_deleteTaskCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           var chosenItem = obj as ListBoxItem;
                           var chosenTask = chosenItem.Content as TaskViewModel;
                           _store.DbContext.Tasks.Remove(chosenTask.Task);
                           await _store.DbContext.SaveChangesAsync();
                           CurrentTasksList.Remove(chosenTask);
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
                           OnPropertyChanged("SelectedRepeatingCondition");
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