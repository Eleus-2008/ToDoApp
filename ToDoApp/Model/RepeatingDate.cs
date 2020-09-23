using System;
using System.Collections.Generic;

namespace ToDoApp.Model
{
    public enum TypeOfRepeatTimeSpan
    {
        Day,
        DayOfWeek,
        Month,
        Year
    }
    
    // лень разбираться, зачем в конструкторе передавать запланированное время
    // сделал необязательным параметром
    // переписать, сделать удобную обёртку над datetime, возможно передавать дату в метод, а не конструктор
    public class RepeatingDate
    {
        public TypeOfRepeatTimeSpan Type { get; private set; }
        public int RepeatingEveryX { get; private set; }
        public List<DayOfWeek> RepeatingDaysOfWeek { get; private set; } = new List<DayOfWeek>();

        public DateTime LatestPlannedDateTime { get; private set; }

        public RepeatingDate(TypeOfRepeatTimeSpan type, int repeatTimes, DateTime? plannedDateTime = null)
        {
            Type = type;
            RepeatingEveryX = repeatTimes;
            LatestPlannedDateTime = plannedDateTime ?? DateTime.Now;
            UpdateNextRepeatingDate();
        }

        public RepeatingDate(TypeOfRepeatTimeSpan type, List<DayOfWeek> repeatingDaysOfWeek, int repeatTimes, DateTime? plannedDateTime = null)
        {
            if (repeatingDaysOfWeek == null || repeatingDaysOfWeek.Count == 0)
            {
                throw new ArgumentException("Неправильно задаётся дата повтора");
            }
            else
            {
                Type = type;
                RepeatingDaysOfWeek = repeatingDaysOfWeek;
                RepeatingEveryX = repeatTimes;
                LatestPlannedDateTime = plannedDateTime ?? DateTime.Now;
                UpdateNextRepeatingDate();
            }
        }

        public DateTime UpdateNextRepeatingDate()
        {
            switch (Type)
            {
                case TypeOfRepeatTimeSpan.Day:
                    LatestPlannedDateTime = LatestPlannedDateTime.AddDays(RepeatingEveryX);
                    return LatestPlannedDateTime;
                    break;
                case TypeOfRepeatTimeSpan.DayOfWeek:
                    for (int dayOfWeek = (int)LatestPlannedDateTime.DayOfWeek; dayOfWeek < (int)LatestPlannedDateTime.DayOfWeek + 7; dayOfWeek++)
                    {
                        int numbOfDay = dayOfWeek;
                        if (numbOfDay == 7)
                        {
                            numbOfDay -= 7;
                        }

                        if (RepeatingDaysOfWeek.Contains((DayOfWeek) numbOfDay))
                        {
                            LatestPlannedDateTime = LatestPlannedDateTime.AddDays(dayOfWeek - (int) LatestPlannedDateTime.DayOfWeek +
                                                          7 * RepeatingEveryX);
                            return LatestPlannedDateTime;
                        }
                    }
                    throw new ArgumentOutOfRangeException(null, "Ошибка в вычислении даты повтора");
                case TypeOfRepeatTimeSpan.Month:
                    LatestPlannedDateTime = LatestPlannedDateTime.AddMonths(RepeatingEveryX);
                    return LatestPlannedDateTime;
                    break;
                case TypeOfRepeatTimeSpan.Year:
                    LatestPlannedDateTime = LatestPlannedDateTime.AddYears(RepeatingEveryX);
                    return LatestPlannedDateTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(null, "Ошибка в вычислении даты повтора");
            }
        }
    }
}