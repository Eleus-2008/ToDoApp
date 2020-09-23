using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ToDoApp.Model;

namespace ToDoApp.ViewModel
{
    // Порядок проверки условий ошибок не совсем логичен
    // При обнулении свойств из-за двойной проверки возможно некорректное поведение
    public class DateViewModel : INotifyPropertyChanged
    {
        private TaskViewModel _task;

        private DateTime? _tempDateOfBeginning;

        public DateTime? TempDateOfBeginning
        {
            get => _tempDateOfBeginning;
            set
            {
                _tempDateOfBeginning = value;
                OnPropertyChanged();
                IsValid = CheckAllProps();
            }
        }

        private DateTime? _tempDateOfEnd;

        public DateTime? TempDateOfEnd
        {
            get => _tempDateOfEnd;
            set
            {
                _tempDateOfEnd = value;
                OnPropertyChanged();
                IsValid = CheckAllProps();
            }
        }

        private TimeSpan? _tempTimeOfBeginning;

        public string TempTimeOfBeginning
        {
            get => _tempTimeOfBeginning?.ToString();
            set
            {
                if (value.Trim(' ') == "")
                {
                    _tempTimeOfBeginning = null;
                    OnPropertyChanged();
                    IsValid = CheckAllProps();
                }
                else if (TimeSpan.TryParse(value, out var parsedDate))
                {
                    _tempTimeOfBeginning = parsedDate;
                    OnPropertyChanged();
                    RemoveError("TempTimeOfBeginning", ERROR_TIME_IS_INCORRECT);
                    IsValid = CheckAllProps();
                }
                else
                {
                    AddError("TempTimeOfBeginning", ERROR_TIME_IS_INCORRECT);
                    OnPropertyChanged("Errors");
                    IsValid = false;
                }
            }
        }

        private TimeSpan? _tempTimeOfEnd;

        public string TempTimeOfEnd
        {
            get => _tempTimeOfEnd?.ToString();
            set
            {
                if (value.Trim(' ') == "")
                {
                    _tempTimeOfEnd = null;
                    OnPropertyChanged();
                    IsValid = CheckAllProps();
                }
                else if (TimeSpan.TryParse(value, out var parsedDate))
                {
                    _tempTimeOfEnd = parsedDate;
                    OnPropertyChanged();
                    IsValid = CheckAllProps();
                    RemoveError("TempTimeOfEnd", ERROR_TIME_IS_INCORRECT);
                }
                else
                {
                    AddError("TempTimeOfEnd", ERROR_TIME_IS_INCORRECT);
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
                           _task.DateTimeOfBeginning = new DateTime(_tempDateOfBeginning.Value.Year,
                               _tempDateOfBeginning.Value.Month, _tempDateOfBeginning.Value.Day,
                               _tempTimeOfBeginning.Value.Hours, _tempTimeOfBeginning.Value.Minutes,
                               _tempTimeOfBeginning.Value.Seconds);

                           _task.DateTimeOfEnd = new DateTime(_tempDateOfEnd.Value.Year, _tempDateOfEnd.Value.Month,
                               _tempDateOfEnd.Value.Day,
                               _tempTimeOfEnd.Value.Hours, _tempTimeOfEnd.Value.Minutes, _tempTimeOfEnd.Value.Seconds);
                       }, obj => IsValid));
            }
        }


        private const string ERROR_DATE_IS_EXPIRED = "Указанная дата уже наступила";
        private const string ERROR_ENDDATE_IS_EARLIER = "Дата окончания задачи наступит раньше даты начала";
        private const string ERROR_TIME_IS_INCORRECT = "Указано некорректное время";
        private const string ERROR_DATE_IS_NOT_SPECIFIED = "Не указана дата";
        private const string ERROR_TIME_IS_NOT_SPECIFIED = "Не указано время";
        private const string ERROR_ENDING_TIME_IS_EARLIER = "Время окончания задачи наступит раньше времени начала";

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
            if (!_tempDateOfBeginning.HasValue && _tempTimeOfBeginning.HasValue)
            {
                AddError("TempDateOfBeginning", ERROR_DATE_IS_NOT_SPECIFIED);
            }
            else
            {
                RemoveError("TempDateOfBeginning", ERROR_DATE_IS_NOT_SPECIFIED);
            }

            if (!_tempDateOfEnd.HasValue && _tempTimeOfEnd.HasValue)
            {
                AddError("TempDateOfEnd", ERROR_DATE_IS_NOT_SPECIFIED);
            }
            else
            {
                RemoveError("TempDateOfEnd", ERROR_DATE_IS_NOT_SPECIFIED);
            }


            if (!_tempTimeOfBeginning.HasValue && _tempTimeOfEnd.HasValue)
            {
                AddError("TempTimeOfBeginning", ERROR_TIME_IS_NOT_SPECIFIED);
            }
            else
            {
                RemoveError("TempTimeOfBeginning", ERROR_TIME_IS_NOT_SPECIFIED);
            }

            if (!_tempTimeOfEnd.HasValue && _tempTimeOfBeginning.HasValue)
            {
                AddError("TempTimeOfEnd", ERROR_TIME_IS_NOT_SPECIFIED);
            }
            else
            {
                RemoveError("TempTimeOfEnd", ERROR_TIME_IS_NOT_SPECIFIED);
            }


            if (!_tempDateOfBeginning.HasValue && _tempDateOfEnd.HasValue)
            {
                AddError("TempDateOfBeginning", ERROR_DATE_IS_NOT_SPECIFIED);
            }
            else
            {
                RemoveError("TempDateOfBeginning", ERROR_DATE_IS_NOT_SPECIFIED);
            }


            if (_tempDateOfBeginning.HasValue)
            {
                if (_tempDateOfBeginning.Value.Date < DateTime.Now.Date)
                {
                    AddError("TempDateOfBeginning", ERROR_DATE_IS_EXPIRED);
                }
                else
                {
                    RemoveError("TempDateOfBeginning", ERROR_DATE_IS_EXPIRED);
                }
            }

            if (_tempDateOfBeginning.HasValue && _tempTimeOfBeginning.HasValue)
            {
                if (_tempDateOfBeginning.Value.Date == DateTime.Now.Date)
                {
                    if (_tempTimeOfBeginning.Value < new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))
                    {
                        AddError("TempTimeOfBeginning", ERROR_DATE_IS_EXPIRED);
                    }
                }
            }

            if (_tempDateOfBeginning.HasValue && _tempDateOfEnd.HasValue)
            {
                if (_tempDateOfEnd.Value.Date < _tempDateOfBeginning.Value.Date)
                {
                    AddError("TempDateOfEnd", ERROR_ENDDATE_IS_EARLIER);
                }
                else
                {
                    RemoveError("TempDateOfEnd", ERROR_ENDDATE_IS_EARLIER);
                }
            }

            if (_tempDateOfBeginning.HasValue && _tempDateOfEnd.HasValue &&
                _tempTimeOfBeginning.HasValue && _tempTimeOfEnd.HasValue)
            {
                if (_tempDateOfBeginning.Value.Date == _tempDateOfEnd.Value.Date)
                {
                    if (_tempTimeOfEnd.Value <= _tempTimeOfBeginning.Value)
                    {
                        AddError("TempTimeOfEnd", ERROR_ENDING_TIME_IS_EARLIER);
                    }
                    else
                    {
                        RemoveError("TempTimeOfEnd", ERROR_ENDING_TIME_IS_EARLIER);
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