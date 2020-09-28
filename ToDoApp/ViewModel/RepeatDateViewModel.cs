using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Model;
using ToDoApp.Model.Enums;

// ReSharper disable ExplicitCallerInfoArgument

namespace ToDoApp.ViewModel
{
    public class RepeatDateViewModel : INotifyPropertyChanged
    {
        private TaskViewModel _task;

        private int? _repeatInterval;

        public int? RepeatInterval
        {
            get => _repeatInterval;
            set
            {
                _repeatInterval = value;
                OnPropertyChanged();
            }
        }

        private TypeOfRepeatTimeSpan? _typeOfRepeatTimeSpan;

        public int SelectedTypeOfRepeatTimeSpan
        {
            get { return _typeOfRepeatTimeSpan == null ? -1 : (int) _typeOfRepeatTimeSpan; }
            set
            {
                _typeOfRepeatTimeSpan = value == -1 ? (TypeOfRepeatTimeSpan?) null : (TypeOfRepeatTimeSpan) value;
                OnPropertyChanged();
                OnPropertyChanged("IsDaysOfWeekVisible");
            }
        }

        public bool IsDaysOfWeekVisible
        {
            get => _typeOfRepeatTimeSpan == TypeOfRepeatTimeSpan.DayOfWeek;
        }

        private List<DayOfWeek> _repeatingDaysOfWeek = new List<DayOfWeek>();

        #region DaysOfWeekProps

        public bool IsMondayChoosen
        {
            get => _repeatingDaysOfWeek.Contains(DayOfWeek.Monday);
            set
            {
                if (value)
                {
                    _repeatingDaysOfWeek.Add(DayOfWeek.Monday);
                }
                else
                {
                    _repeatingDaysOfWeek.Remove(DayOfWeek.Monday);
                }
            }
        }

        public bool IsTuesdayChoosen
        {
            get => _repeatingDaysOfWeek.Contains(DayOfWeek.Tuesday);
            set
            {
                if (value)
                {
                    _repeatingDaysOfWeek.Add(DayOfWeek.Tuesday);
                }
                else
                {
                    _repeatingDaysOfWeek.Remove(DayOfWeek.Tuesday);
                }
            }
        }

        public bool IsWednesdayChoosen
        {
            get => _repeatingDaysOfWeek.Contains(DayOfWeek.Wednesday);
            set
            {
                if (value)
                {
                    _repeatingDaysOfWeek.Add(DayOfWeek.Wednesday);
                }
                else
                {
                    _repeatingDaysOfWeek.Remove(DayOfWeek.Wednesday);
                }
            }
        }

        public bool IsThursdayChoosen
        {
            get => _repeatingDaysOfWeek.Contains(DayOfWeek.Thursday);
            set
            {
                if (value)
                {
                    _repeatingDaysOfWeek.Add(DayOfWeek.Thursday);
                }
                else
                {
                    _repeatingDaysOfWeek.Remove(DayOfWeek.Thursday);
                }
            }
        }

        public bool IsFridayChoosen
        {
            get => _repeatingDaysOfWeek.Contains(DayOfWeek.Friday);
            set
            {
                if (value)
                {
                    _repeatingDaysOfWeek.Add(DayOfWeek.Friday);
                }
                else
                {
                    _repeatingDaysOfWeek.Remove(DayOfWeek.Friday);
                }
            }
        }

        public bool IsSaturdayChoosen
        {
            get => _repeatingDaysOfWeek.Contains(DayOfWeek.Saturday);
            set
            {
                if (value)
                {
                    _repeatingDaysOfWeek.Add(DayOfWeek.Saturday);
                }
                else
                {
                    _repeatingDaysOfWeek.Remove(DayOfWeek.Saturday);
                }
            }
        }

        public bool IsSundayChoosen
        {
            get => _repeatingDaysOfWeek.Contains(DayOfWeek.Sunday);
            set
            {
                if (value)
                {
                    _repeatingDaysOfWeek.Add(DayOfWeek.Sunday);
                }
                else
                {
                    _repeatingDaysOfWeek.Remove(DayOfWeek.Sunday);
                }
            }
        }

        #endregion

        public RepeatDateViewModel(TaskViewModel task)
        {
            _task = task;

            if (_task.RepeatingConditions != null)
            {
                _typeOfRepeatTimeSpan = _task.RepeatingConditions.Type;
                _repeatInterval = _task.RepeatingConditions.RepeatInterval;
                _repeatingDaysOfWeek = _task.RepeatingConditions.RepeatingDaysOfWeek;

                UpdateDaysOfWeekProperties();
            }
        }


        private RelayCommand _saveDateCommand;

        public RelayCommand SaveDateCommand
        {
            get
            {
                return _saveDateCommand ??
                       (_saveDateCommand = new RelayCommand(obj =>
                       {
                           // ReSharper disable once PossibleInvalidOperationException
                           _task.RepeatingConditions.Type = _typeOfRepeatTimeSpan.Value;
                           // ReSharper disable once PossibleInvalidOperationException
                           _task.RepeatingConditions.RepeatInterval = _repeatInterval.Value;
                           _task.RepeatingConditions.RepeatingDaysOfWeek = _repeatingDaysOfWeek;
                       }, obj =>
                       {
                           if (!_typeOfRepeatTimeSpan.HasValue || !_repeatInterval.HasValue) return false;
                           if (_typeOfRepeatTimeSpan == TypeOfRepeatTimeSpan.DayOfWeek &&
                               !_repeatingDaysOfWeek.Any()) return false;
                           return true;
                       }));
            }
        }

        private void UpdateDaysOfWeekProperties()
        {
            OnPropertyChanged("IsMondayChoosen");
            OnPropertyChanged("IsTuesdayChoosen");
            OnPropertyChanged("IsWednesdayChoosen");
            OnPropertyChanged("IsThursdayChoosen");
            OnPropertyChanged("IsFridayChoosen");
            OnPropertyChanged("IsSaturdayChoosen");
            OnPropertyChanged("IsSundayChoosen");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}