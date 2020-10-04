using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ToDoApp.Model;

namespace ToDoApp.ViewModel
{
    public class DateViewModel : INotifyPropertyChanged
    {
        private TaskViewModel _task;

        private DateTime? _date;

        public DateTime? Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
                IsValid = CheckAllProps();
            }
        }

        private TimeSpan? _timeOfBeginning;

        public string TimeOfBeginning
        {
            get => _timeOfBeginning?.ToString();
            set
            {
                if (value.Trim(' ') == "")
                {
                    _timeOfBeginning = null;
                    OnPropertyChanged();
                    IsValid = CheckAllProps();
                }
                else if (TimeSpan.TryParse(value, out var parsedDate))
                {
                    _timeOfBeginning = parsedDate;
                    OnPropertyChanged();
                    RemoveError("TimeOfBeginning", ERROR_TIME_IS_INCORRECT);
                    IsValid = CheckAllProps();
                }
                else
                {
                    AddError("TimeOfBeginning", ERROR_TIME_IS_INCORRECT);
                    OnPropertyChanged("Errors");
                    IsValid = false;
                }
            }
        }

        private TimeSpan? _timeOfEnd;

        public string TimeOfEnd
        {
            get => _timeOfEnd?.ToString();
            set
            {
                if (value.Trim(' ') == "")
                {
                    _timeOfEnd = null;
                    OnPropertyChanged();
                    IsValid = CheckAllProps();
                }
                else if (TimeSpan.TryParse(value, out var parsedDate))
                {
                    _timeOfEnd = parsedDate;
                    OnPropertyChanged();
                    IsValid = CheckAllProps();
                    RemoveError("TimeOfEnd", ERROR_TIME_IS_INCORRECT);
                }
                else
                {
                    AddError("TimeOfEnd", ERROR_TIME_IS_INCORRECT);
                    IsValid = false;
                }
            }
        }

        private bool _isValid;

        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }

        public string Errors
        {
            get
            {
                var allErrors = string.Empty;
                var firstErrorList = _errors.Values.FirstOrDefault();
                if (firstErrorList != null)
                {
                    IEnumerable<string> allErrorLists = null;
                    foreach (var errorList in _errors.Values)
                    {
                        allErrorLists = firstErrorList.Union(errorList);
                    }

                    foreach (var error in allErrorLists)
                    {
                        allErrors += error + Environment.NewLine;
                    }
                }

                return allErrors;
            }
        }

        public DateViewModel(TaskViewModel task)
        {
            _task = task;
        }

        private RelayCommand _saveDateCommand;

        public RelayCommand SaveDateCommand
        {
            get
            {
                return _saveDateCommand ??
                       (_saveDateCommand = new RelayCommand(obj =>
                       {
                           _task.Date = _date;
                           _task.TimeOfBeginning = _timeOfBeginning;
                           _task.TimeOfEnd = _timeOfEnd;
                       }, obj => IsValid));
            }
        }


        private const string ERROR_DATE_IS_EXPIRED = "Указанная дата уже наступила";
        private const string ERROR_TIME_IS_INCORRECT = "Указано некорректное время";
        private const string ERROR_DATE_IS_NOT_SPECIFIED = "Не указана дата";
        private const string ERROR_TIME_IS_NOT_SPECIFIED = "Не указано время";

        private const string ERROR_ENDING_TIME_IS_EARLIER =
            "Время окончания задачи наступит раньше времени начала";

        private Dictionary<String, List<String>> _errors =
            new Dictionary<string, List<string>>();

        public void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
            }
        }

        public void RemoveError(string propertyName, string error)
        {
            if (_errors.ContainsKey(propertyName) &&
                _errors[propertyName].Contains(error))
            {
                _errors[propertyName].Remove(error);
                if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);
            }
        }

        private bool CheckAllProps()
        {
            if (!_date.HasValue && (_timeOfBeginning.HasValue || _timeOfEnd.HasValue))
            {
                AddError("Date", ERROR_DATE_IS_NOT_SPECIFIED);
            }
            else
            {
                RemoveError("Date", ERROR_DATE_IS_NOT_SPECIFIED);
            }

            if (!_timeOfBeginning.HasValue && _timeOfEnd.HasValue)
            {
                AddError("TimeOfBeginning", ERROR_TIME_IS_NOT_SPECIFIED);
            }
            else
            {
                RemoveError("TimeOfBeginning", ERROR_TIME_IS_NOT_SPECIFIED);
            }

            if (_date.HasValue)
            {
                if (_date < DateTime.Now.Date)
                {
                    AddError("Date", ERROR_DATE_IS_EXPIRED);
                }
                else
                {
                    RemoveError("Date", ERROR_DATE_IS_EXPIRED);
                }
            }

            if (_date.HasValue && _timeOfBeginning.HasValue)
            {
                if (_date == DateTime.Now.Date)
                {
                    if (_timeOfBeginning < DateTime.Now.TimeOfDay)
                    {
                        AddError("TimeOfBeginning", ERROR_DATE_IS_EXPIRED);
                    }
                    else
                    {
                        RemoveError("TimeOfBeginning", ERROR_DATE_IS_EXPIRED);
                    }
                }
            }

            OnPropertyChanged("Errors");

            return _errors.Count == 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}