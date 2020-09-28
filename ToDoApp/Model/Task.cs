using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using ToDoApp.Model.Enums;

namespace ToDoApp.Model
{
    public class Task
    {
        protected const int DefaultPriority = 5;

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        
        public ToDoList ToDoList { get; set; }

        private bool _isDone;

        public bool IsDone
        {
            get => _isDone;
            set
            {
                if (value)
                {
                    if (RepeatingConditions != null && Date.HasValue)
                    {
                        Date = RepeatingConditions.GetNextDateTime(Date.Value);
                    }
                }

                _isDone = value;
            }
        }

        public bool IsExpired
        {
            get
            {
                // ���� ���� �� �������, ������ ������� �� ����� ����������
                if (Date == null)
                {
                    return false;
                }

                if (DateTime.Today > Date)
                {
                    return true;
                }

                if (DateTime.Today == Date)
                {
                    if (TimeOfBeginning.HasValue && TimeOfEnd.HasValue)
                    {
                        return DateTime.Now.TimeOfDay > TimeOfEnd;
                    }

                    // ���� ������� ������ ���� ������, �� ������ �� ��������� ����������� �� ����� ���
                    if (TimeOfBeginning.HasValue && !TimeOfEnd.HasValue)
                    {
                        return DateTime.Today > Date;
                    }
                }

                return false;
            }
        }

        public bool IsActual
        {
            get
            {
                if (Date == null)
                {
                    return true;
                }

                if (Date == DateTime.Today)
                {
                    if (TimeOfBeginning.HasValue && TimeOfEnd.HasValue)
                    {
                        return (TimeOfBeginning <= DateTime.Now.TimeOfDay) || (DateTime.Now.TimeOfDay < TimeOfEnd);
                    }

                    // ���� ������� ������ ���� ������, �� ������ ��������� ���������� �� ����� ���
                    if (TimeOfBeginning.HasValue && !TimeOfEnd.HasValue)
                    {
                        return DateTime.Now.TimeOfDay >= TimeOfBeginning;
                    }
                }

                return false;
            }
        }

        private DateTime? _date;
        public DateTime? Date
        {
            get => _date?.Date;
            set => _date = value;
        }

        private TimeSpan? _timeOfBeginning;
        public TimeSpan? TimeOfBeginning
        {
            get => _timeOfBeginning;
            set => _timeOfBeginning = value;
        }

        private TimeSpan? _timeOfEnd;
        public TimeSpan? TimeOfEnd
        {
            get => _timeOfEnd;
            set => _timeOfEnd = value;
        }

        private int _priority;
        public int Priority
        {
            get => _priority;
            set
            {
                if (value < 1)
                {
                    _priority = 1;
                }
                else if (value > 10)
                {
                    _priority = 10;
                }
                else
                {
                    _priority = value;
                }
            }
        }

        public RepeatingConditions RepeatingConditions { get; set; }

        public Task()
        {
            Priority = DefaultPriority;
        }
    }
}