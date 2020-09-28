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
            }
        }
        public TimeSpan? TimeOfBeginning
        {
            get => Task.TimeOfBeginning;
            set
            {
                Task.TimeOfBeginning = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan? TimeOfEnd
        {
            get => Task.TimeOfEnd;
            set
            {
                Task.TimeOfEnd = value;
                OnPropertyChanged();
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