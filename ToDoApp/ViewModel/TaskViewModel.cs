using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToDoApp.Model;
using ToDoApp.Model.Enums;

namespace ToDoApp.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public Task Task { get; }

        public string Name
        {
            get => Task.Name;
            set
            {
                Task.Name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => Task.Description;
            set
            {
                Task.Description = value;
                OnPropertyChanged();
            }
        }

        public bool IsDone
        {
            get => Task.IsDone;
            set
            {
                Task.IsDone = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpired => Task.IsExpired;

        public bool IsActual => Task.IsActual;

        public DateTime? Date
        {
            get => Task.Date;
            set
            {
                Task.Date = value;
                OnPropertyChanged();
                OnPropertyChanged("TextDateTime");
            }
        }
        public TimeSpan? TimeOfBeginning
        {
            get => Task.TimeOfBeginning;
            set
            {
                Task.TimeOfBeginning = value;
                OnPropertyChanged();
                OnPropertyChanged("TextDateTime");
            }
        }

        public TimeSpan? TimeOfEnd
        {
            get => Task.TimeOfEnd;
            set
            {
                Task.TimeOfEnd = value;
                OnPropertyChanged();
                OnPropertyChanged("TextDateTime");
            }
        }

        public string TextDateTime
        {
            get
            {
                var text = string.Empty;
                if (Date.HasValue)
                {
                    if (DateTime.Today.Year == Date.Value.Year)
                    {
                        text = Date.Value.ToString("ddd, d MMM");
                    }
                    else
                    {
                        text += Date.Value.ToString(" yyyy");
                    }

                    if (TimeOfBeginning.HasValue)
                    {
                        text += TimeOfBeginning.Value.ToString("hh:mm");
                        if (TimeOfEnd.HasValue)
                        {
                            text += " - ";
                            text += TimeOfEnd.Value.ToString("hh:mm");
                        }
                    }
                }

                return text;
            }
        }

        public RepeatingConditions RepeatingConditions
        {
            get => Task.RepeatingConditions;
            set => Task.RepeatingConditions = value;
        }

        public int Priority
        {
            get => Task.Priority;
            set
            {
                Task.Priority = value;
                OnPropertyChanged();
            }
        }

        public string TextPriority
        {
            get
            {
                if (Priority <= 3)
                {
                    return "Низкий";
                }

                if (4 <= Priority && Priority >= 7)
                {
                    return "Средний";
                }

                if (8 <= Priority)
                {
                    return "Высокий";
                }

                return "Средний";
            }
            set
            {
                switch (value.ToLower())
                {
                    case "низкий": Priority = 2;
                        break;
                    case "средний": Priority = 5;
                        break;
                    case "высокий": Priority = 8;
                        break;
                    default: Priority = 5;
                        break;
                }
                OnPropertyChanged();
            }
        }

        public TaskViewModel(Task task)
        {
            Task = task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}