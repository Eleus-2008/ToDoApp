﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ToDoApp.Model;
using ToDoApp.Model.Enums;

// ReSharper disable ExplicitCallerInfoArgument

namespace ToDoApp.ViewModel
{
    public class RepeatDateViewModel : INotifyPropertyChanged
    {
        private readonly TaskViewModel _task;

        private int _repeatInterval = 1;

        public int RepeatInterval
        {
            get => _repeatInterval;
            set
            {
                _repeatInterval = value < 1 ? 1 : value;
                OnPropertyChanged();
            }
        }

        private TypeOfRepeatTimeSpan _typeOfRepeatTimeSpan = TypeOfRepeatTimeSpan.Day;

        public int SelectedTypeOfRepeatTimeSpan
        {
            get => (int) _typeOfRepeatTimeSpan;
            set
            {
                _typeOfRepeatTimeSpan = (TypeOfRepeatTimeSpan) value;
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

        public bool IsMondaySelected
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

        public bool IsTuesdaySelected
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

        public bool IsWednesdaySelected
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

        public bool IsThursdaySelected
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

        public bool IsFridaySelected
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

        public bool IsSaturdaySelected
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

        public bool IsSundaySelected
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
                           _repeatingDaysOfWeek.Sort((x, y) =>
                           {
                               if (x == DayOfWeek.Sunday)
                               {
                                   return 1;
                               }
                               
                               if (y == DayOfWeek.Sunday)
                               {
                                   return -1;
                               }
                               
                               if (x > y)
                               {
                                   return 1;
                               }

                               if (y > x)
                               {
                                   return -1;
                               }

                               return 0;
                           });
                           _task.RepeatingConditions = new RepeatingConditions
                           {
                               RepeatInterval = _repeatInterval,
                               Type = _typeOfRepeatTimeSpan,
                               RepeatingDaysOfWeek = _repeatingDaysOfWeek
                           };
                       }, obj =>
                       {
                           if (_typeOfRepeatTimeSpan == TypeOfRepeatTimeSpan.DayOfWeek &&
                               !_repeatingDaysOfWeek.Any()) return false;
                           return true;
                       }));
            }
        }

        private void UpdateDaysOfWeekProperties()
        {
            OnPropertyChanged("IsMondaySelected");
            OnPropertyChanged("IsTuesdaySelected");
            OnPropertyChanged("IsWednesdaySelected");
            OnPropertyChanged("IsThursdaySelected");
            OnPropertyChanged("IsFridaySelected");
            OnPropertyChanged("IsSaturdaySelected");
            OnPropertyChanged("IsSundaySelected");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}