using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToDoApp.Model;

namespace ToDoApp.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        private Task _task;

        public TaskViewModel(Task task)
        {
            _task = task;
        }

        public Task Task => _task; 

        public int Id => _task.Id;

        public string Name
        {
            get => _task.Name;
            set
            {
                _task.Name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _task.Description;
            set
            {
                _task.Description = value;
                OnPropertyChanged();
            }
        }

        public bool IsDone
        {
            get => _task.IsDone;
            set
            {
                _task.IsDone = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpired
        {
            get => _task.IsExpired;
        }

        public DateTime? DateTimeOfBeginning
        {
            get => _task.DateTimeOfBeginning;
            set
            {
                _task.DateTimeOfBeginning = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DateTimeOfEnd
        {
            get => _task.DateTimeOfEnd;
            set
            {
                _task.DateTimeOfEnd = value;
                OnPropertyChanged();
            }
        }

        public DateTime? RepeatingDate
        {
            get => _task.RepeatingDate;
        }

        public (TypeOfRepeatTimeSpan type, IEnumerable<DayOfWeek> daysOfWeek, int repeats, DateTime? latestPlannedDate)? RepeatingConditions
        {
            get => _task.RepeatingConditions;
            set
            {
                _task.RepeatingConditions = value;
                OnPropertyChanged();
            }
        }

        public int Priority
        {
            get => _task.Priority;
            set
            {
                _task.Priority = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}