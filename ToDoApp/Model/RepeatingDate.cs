using System;
using System.Collections.Generic;
using ToDoApp.Model.Enums;

namespace ToDoApp.Model
{
    public class RepeatingDate
    {
        
        public TypeOfRepeatTimeSpan Type { get; set; }

        private int _repeatingEveryX;
        public int RepeatingEveryX
        {
            get => _repeatingEveryX;
            set
            {
                if (value < 1)
                {
                    _repeatingEveryX = 1;
                }

                _repeatingEveryX = value;
            }
        }

        public List<DayOfWeek> RepeatingDaysOfWeek { get; set; } = new List<DayOfWeek>();

        /// <summary>
        /// Вычисляет следующую дату (не учитывая время) повторяющейся задачи 
        /// </summary>
        /// <param name="latestPlannedDate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public DateTime GetNextDateTime(DateTime latestPlannedDate)
        {
            switch (Type)
            {
                case TypeOfRepeatTimeSpan.Day:
                    return latestPlannedDate.AddDays(RepeatingEveryX);
                case TypeOfRepeatTimeSpan.DayOfWeek:
                    foreach (var dayOfWeek in RepeatingDaysOfWeek)
                    {
                        if (dayOfWeek < latestPlannedDate.DayOfWeek)
                        {
                            return latestPlannedDate.AddDays(dayOfWeek + 7 - latestPlannedDate.DayOfWeek +
                                                             (RepeatingEveryX - 1) * 7);
                        }

                        if (dayOfWeek >= latestPlannedDate.DayOfWeek)
                        {
                            return latestPlannedDate.AddDays(dayOfWeek - latestPlannedDate.DayOfWeek +
                                                             (RepeatingEveryX - 1) * 7);
                        }
                    }

                    throw new ArgumentOutOfRangeException(null, "Ошибка в вычислении даты повтора");
                case TypeOfRepeatTimeSpan.Month:
                    return latestPlannedDate.AddMonths(RepeatingEveryX);
                case TypeOfRepeatTimeSpan.Year:
                    return latestPlannedDate.AddYears(RepeatingEveryX);
                default:
                    throw new ArgumentOutOfRangeException(null, "Ошибка в вычислении даты повтора");
            }
        }
    }
}