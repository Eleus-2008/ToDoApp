using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace ToDoApp.Model
{
    public class Task
    {
        protected const int DefaultPriority = 5;
        
        public int Id { get; private set; }        
        
        public string Name { get; set; }
        public string Description { get; set; }

        private bool _isDone;
        public bool IsDone
        {
            get => _isDone;
            set
            {
                if (value)
                {
                    if (RepeatingDate != null)
                    {
                        _repeatingDate.UpdateNextRepeatingDate();
                        TimeSpan? timeDifference = DateTimeOfEnd - DateTimeOfBeginning;
                        DateTimeOfBeginning = RepeatingDate;
                        DateTimeOfEnd = DateTimeOfBeginning + timeDifference;
                    }
                }
                _isDone = value;
            }
        }

        public bool IsExpired
        {
            get
            {
                if (DateTimeOfBeginning == null)
                {
                    return false;
                }

                if (DateTimeOfEnd == null)
                {
                    return DateTime.Now > DateTimeOfBeginning;
                }

                return DateTime.Now > DateTimeOfEnd;
            }
        }

        public DateTime? DateTimeOfBeginning { get; set; }

        public DateTime? DateTimeOfEnd { get; set; }

        public DateTime? RepeatingDate => _repeatingDate?.LatestPlannedDateTime;

        public (TypeOfRepeatTimeSpan type, IEnumerable<DayOfWeek> daysOfWeek, int repeats, DateTime? latestPlannedDate)? RepeatingConditions {
            get
            {
                if (_repeatingDate != null)
                {
                    return (
                        _repeatingDate.Type,
                        _repeatingDate.RepeatingDaysOfWeek,
                        _repeatingDate.RepeatingEveryX,
                        _repeatingDate.LatestPlannedDateTime);
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    _repeatingDate = null;
                }
                else
                {
                    if (value.Value.daysOfWeek.Any())
                    {
                        _repeatingDate = new RepeatingDate(value.Value.type, new List<DayOfWeek>(value.Value.daysOfWeek), value.Value.repeats,
                            value.Value.latestPlannedDate);
                    }
                    else
                    {
                        _repeatingDate = new RepeatingDate(value.Value.type, value.Value.repeats,
                            value.Value.latestPlannedDate);
                    }
                }
            }
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

        private RepeatingDate _repeatingDate;

        //временный конструктор
        public Task(string name)
        {
            Name = name;
        }

        public Task(int id, string name)
        {
            Id = id;
            Name = name;
            Priority = DefaultPriority;
        }
    }
}