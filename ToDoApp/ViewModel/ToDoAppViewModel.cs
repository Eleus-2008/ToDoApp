using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ToDoApp.Model;
using ToDoApp.Model.Enums;

namespace ToDoApp.ViewModel
{
    public class ToDoAppViewModel : INotifyPropertyChanged
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ObservableCollection<ToDoListViewModel> DefaultToDoLists { get; set; } =
            new ObservableCollection<ToDoListViewModel>();

        public ObservableCollection<ToDoListViewModel> ToDoLists { get; set; } =
            new ObservableCollection<ToDoListViewModel>();

        private BindingList<TaskViewModel> _currentTasksList;

        public BindingList<TaskViewModel> CurrentTasksList
        {
            get => _currentTasksList;
            set
            {
                _currentTasksList = value;
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
                    value.ToDoList.Tasks = _unitOfWork.ToDoLists.GetAll().SelectMany(x => x.Tasks).ToList();
                }
                if (value.Name == "Мой день")
                {
                    value.ToDoList.Tasks = _unitOfWork.ToDoLists.GetAll().SelectMany(x => x.Tasks).Where(task =>
                    {
                        if (!task.Date.HasValue)
                        {
                            return true;
                        }

                        return task.Date == DateTime.Today;
                    }).ToList();
                }
                _currentList = value;
                CurrentTasksList = new BindingList<TaskViewModel>(_currentList.Tasks.ToList());
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
                    case 2: CurrentTask.RepeatingConditions = new RepeatingConditions
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
                    case 3: CurrentTask.RepeatingConditions = new RepeatingConditions
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
                    case 4: CurrentTask.RepeatingConditions = new RepeatingConditions
                        {
                            Type = TypeOfRepeatTimeSpan.DayOfWeek,
                            RepeatInterval = 1,
                            RepeatingDaysOfWeek = new List<DayOfWeek>
                            {
                                CurrentTask.Date.Value.DayOfWeek
                            }
                        };
                        break;
                    case 5: CurrentTask.RepeatingConditions = new RepeatingConditions
                        {
                            Type = TypeOfRepeatTimeSpan.Month,
                            RepeatInterval = 1
                        };
                        break;
                    case 6: CurrentTask.RepeatingConditions = new RepeatingConditions
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
                    case 0: CurrentTask.Priority = 2;
                        break;
                    case 1: CurrentTask.Priority = 5;
                        break;
                    case 2: CurrentTask.Priority = 8;
                        break;
                }
                OnPropertyChanged();
            }
        }

        public ToDoAppViewModel()
        {
            CurrentTask = new TaskViewModel(new Task());
            CurrentList = new ToDoListViewModel(new ToDoList());
            CurrentTasksList = new BindingList<TaskViewModel>();
            OnPropertyChanged("IsRepeatComboboxEnabled");

            InitializeToDoLists();
        }

        private void InitializeToDoLists()
        {
            ToDoLists = new ObservableCollection<ToDoListViewModel>(_unitOfWork.ToDoLists.GetAll()
                .SkipWhile(list => list.Name == "Все задачи")
                .Select(list => new ToDoListViewModel(list)));

            if (_unitOfWork.ToDoLists.GetAll().All(list => list.Name != "Все задачи"))
            {
                _unitOfWork.ToDoLists.Add(new ToDoList
                {
                    Name = "Все задачи"
                });
            }

            DefaultToDoLists.Add(new ToDoListViewModel(new ToDoList
            {
                Name = "Задачи",
                Tasks = _unitOfWork.ToDoLists.GetAll().SelectMany(x => x.Tasks).ToList()
            }));

            DefaultToDoLists.Add(new ToDoListViewModel(new ToDoList
            {
                Name = "Мой день",
                Tasks = _unitOfWork.ToDoLists.GetAll().SelectMany(x => x.Tasks).Where(task =>
                {
                    if (!task.Date.HasValue)
                    {
                        return true;
                    }

                    return task.Date == DateTime.Today;
                }).ToList()
            }));

            CurrentList = DefaultToDoLists[0];
        }

        private RelayCommand _addListCommand;

        public RelayCommand AddListCommand
        {
            get
            {
                return _addListCommand ??
                       (_addListCommand = new RelayCommand(obj =>
                       {
                           var textBox = obj as TextBox;
                           var newList = new ToDoList
                           {
                               Name = textBox.Text
                           };
                           _unitOfWork.ToDoLists.Add(newList);
                           ToDoLists.Add(new ToDoListViewModel(newList));
                           CurrentList = ToDoLists.Last();
                           textBox.Text = string.Empty;
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

        private RelayCommand _editListNameCommand;

        public RelayCommand EditListNameCommand
        {
            get
            {
                return _editListNameCommand ??
                       (_editListNameCommand = new RelayCommand(obj =>
                       {
                           var chosenItem = obj as ListBoxItem;
                           var chosenToDoList = chosenItem.Content as ToDoListViewModel;
                           _unitOfWork.ToDoLists.Update(chosenToDoList.ToDoList);
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
                           var chosenItem = obj as ListBoxItem;
                           var chosenToDoList = chosenItem.Content as ToDoListViewModel;
                           foreach (var taskViewModel in chosenToDoList.Tasks)
                           {
                               _unitOfWork.Tasks.Remove(taskViewModel.Task);
                           }        
                           _unitOfWork.ToDoLists.Remove(chosenToDoList.ToDoList);
                           ToDoLists.Remove(chosenToDoList);
                           CurrentList = DefaultToDoLists[0];
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
                               if (CurrentList == DefaultToDoLists.First(list => list.Name == "Задачи") ||
                                   CurrentList == DefaultToDoLists.First(list => list.Name == "Мой день"))
                               {
                                   CurrentTask.Task.ToDoList = _unitOfWork.ToDoLists.GetAll()
                                       .First(list => list.Name == "Все задачи");
                               }
                               else
                               {
                                   CurrentTask.Task.ToDoList = CurrentList.ToDoList;
                               }

                               _unitOfWork.Tasks.Add(CurrentTask.Task);
                               CurrentTasksList.Insert(0, CurrentTask);

                               CurrentTask = new TaskViewModel(new Task());
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

        private RelayCommand _doTaskCommand;

        public RelayCommand DoTaskCommand
        {
            get
            {
                return _doTaskCommand ??
                       (_doTaskCommand = new RelayCommand(obj =>
                       {
                           var chosenItem = obj as ListBoxItem;
                           var chosenTask = chosenItem.Content as TaskViewModel;
                           _unitOfWork.Tasks.Update(chosenTask.Task);
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
                           var chosenItem = obj as ListBoxItem;
                           var chosenTask = chosenItem.Content as TaskViewModel;
                           _unitOfWork.Tasks.Remove(chosenTask.Task);
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