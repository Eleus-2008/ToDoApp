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
                RepeatingDaysOfWeek = Array.ConvertAll<string,DayOfWeek>(value.Split(','), input =>
                {
                    return (DayOfWeek)Enum.Parse(typeof(DayOfWeek), input);
                }).ToList();
            }
        }

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
                    return latestPlannedDate.AddDays(RepeatInterval);
                case TypeOfRepeatTimeSpan.DayOfWeek:
                    foreach (var dayOfWeek in RepeatingDaysOfWeek)
                    {
                        if (dayOfWeek < latestPlannedDate.DayOfWeek)
                        {
                            return latestPlannedDate.AddDays(dayOfWeek + 7 - latestPlannedDate.DayOfWeek +
                                                             (RepeatInterval - 1) * 7);
                        }

                        if (dayOfWeek >= latestPlannedDate.DayOfWeek)
                        {
                            return latestPlannedDate.AddDays(dayOfWeek - latestPlannedDate.DayOfWeek +
                                                             (RepeatInterval - 1) * 7);
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