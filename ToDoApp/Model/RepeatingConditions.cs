using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Enums;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class RepeatingConditions : IEntity
    {
        public int Id { get; set; }
        
        public TypeOfRepeatTimeSpan Type { get; set; }

        private int _repeatInterval = 1;
        public int RepeatInterval
        {
            get => _repeatInterval;
            set
            {
                if (value < 1)
                {
                    _repeatInterval = 1;
                }

                _repeatInterval = value;
            }
        }
        
        public List<DayOfWeek> RepeatingDaysOfWeek { get; set; } = new List<DayOfWeek>();

        public string SerializedDaysOfWeek
        {
            get
            {
                return string.Join(",", RepeatingDaysOfWeek.ConvertAll(input => nameof(input)));
            }
            set
            {
                RepeatingDaysOfWeek = Array.ConvertAll<string,DayOfWeek>(value.Split(','),
                    input => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), input))
                    .ToList();
            }
        }

        /// <summary>
        /// Вычисляет следующую дату (не учитывая время) повторяющейся задачи 
        /// </summary>
        /// <param name="latestPlannedDate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public DateTime GetNextDate(DateTime latestPlannedDate)
        {
            switch (Type)
            {
                case TypeOfRepeatTimeSpan.Day:
                    return latestPlannedDate.AddDays(RepeatInterval);
                case TypeOfRepeatTimeSpan.DayOfWeek:
                    var j = (int) latestPlannedDate.DayOfWeek + 7;
                    for (var i = (int) latestPlannedDate.DayOfWeek + 1; i < j; i++)
                    {
                        var dayOfWeekNumber = i;
                        if (dayOfWeekNumber == 7)
                        {
                            dayOfWeekNumber -= 7;
                        }

                        var dayOfWeek = (DayOfWeek) dayOfWeekNumber;
                        if (RepeatingDaysOfWeek.Contains(dayOfWeek))
                        {
                            if (dayOfWeek < latestPlannedDate.DayOfWeek && dayOfWeek != DayOfWeek.Sunday)
                            {
                                return latestPlannedDate.AddDays(dayOfWeekNumber + 7 -
                                                                 (int) latestPlannedDate.DayOfWeek +
                                                                 (RepeatInterval - 1) * 7);
                            }

                            if (latestPlannedDate.DayOfWeek == DayOfWeek.Sunday)
                            {
                                return latestPlannedDate.AddDays(dayOfWeekNumber -
                                                                 (int) latestPlannedDate.DayOfWeek +
                                                                 (RepeatInterval - 1) * 7);
                            }
                            
                            if (dayOfWeek >= latestPlannedDate.DayOfWeek)
                            {
                                return latestPlannedDate.AddDays(dayOfWeekNumber - (int) latestPlannedDate.DayOfWeek);
                            }

                            if (dayOfWeek == DayOfWeek.Sunday)
                            {
                                return latestPlannedDate.AddDays(7 - (int) latestPlannedDate.DayOfWeek);
                            }
                        }
                    }


                    throw new ArgumentOutOfRangeException(null, "Ошибка в вычислении даты повтора");
                case TypeOfRepeatTimeSpan.Month:
                    return latestPlannedDate.AddMonths(RepeatInterval);
                case TypeOfRepeatTimeSpan.Year:
                    return latestPlannedDate.AddYears(RepeatInterval);
                default:
                    throw new ArgumentOutOfRangeException(null, "Ошибка в вычислении даты повтора");
            }
        }
    }
}