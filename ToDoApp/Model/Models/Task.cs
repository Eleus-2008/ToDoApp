using System;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class Task : IEntity
    {
        protected const int DefaultPriority = 5;

        public Task(bool isDone, DateTime? date, int priority)
        {
            _isDone = isDone;
            _date = date;
            _priority = priority;
        }

        public Guid Id { get; set; }
        
        public bool IsAdded { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUpdated { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        
        public ToDoList ToDoList { get; set; }
        public Guid ToDoListId { get; set; }

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
                        Date = RepeatingConditions.GetNextDate(Date.Value);
                        OnDateUpdated();
                        return;
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
                if (IsDone)
                {
                    return false;
                }
                
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

        public TimeSpan? TimeOfBeginning { get; set; }

        public TimeSpan? TimeOfEnd { get; set; }

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

        public event EventHandler DateUpdated;
        private void OnDateUpdated()
        {
            DateUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}