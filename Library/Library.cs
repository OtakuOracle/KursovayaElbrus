using System;
using System.Collections.Generic;

namespace Libary
{
    public class Calculations
    {
        public static string[] AvailablePeriods(
            TimeSpan[] startTimes,
            int[] durations,
            TimeSpan beginWorkingTime,
            TimeSpan endWorkingTime,
            int consultationTime)
        {
            if (startTimes.Length != durations.Length)
                throw new ArgumentException("startTimes and durations arrays must have the same length.");

            var busyIntervals = new List<(TimeSpan Start, TimeSpan End)>();

            for (int i = 0; i < startTimes.Length; i++)
            {
                busyIntervals.Add((startTimes[i], startTimes[i] + TimeSpan.FromMinutes(durations[i])));
            }

            busyIntervals = busyIntervals.OrderBy(x => x.Start).ToList(); // сортировка по началу

            var freeIntervals = new List<(TimeSpan Start, TimeSpan End)>();

            if (busyIntervals.Count == 0 || beginWorkingTime < busyIntervals[0].Start) // интервал до первого занятия
            {
                TimeSpan freeEnd = busyIntervals.Count > 0 ? busyIntervals[0].Start : endWorkingTime;
                if (freeEnd > beginWorkingTime)
                    freeIntervals.Add((beginWorkingTime, freeEnd));
            }

            for (int i = 0; i < busyIntervals.Count - 1; i++) // промежутки между занятиями
            {
                var endCurrent = busyIntervals[i].End;
                var startNext = busyIntervals[i + 1].Start;

                if (startNext > endCurrent)
                    freeIntervals.Add((endCurrent, startNext));
            }


            if (busyIntervals.Count > 0 && busyIntervals.Last().End < endWorkingTime)  // интервал после последнего занятия
            {
                freeIntervals.Add((busyIntervals.Last().End, endWorkingTime));
            }
            else if (busyIntervals.Count == 0 && beginWorkingTime < endWorkingTime)
            {
                freeIntervals.Add((beginWorkingTime, endWorkingTime));
            }

            var availableSlots = new List<string>(); // Разбиваем свободные интервалы на куски по consultationTime

            foreach (var (start, end) in freeIntervals)
            {
                var slotStart = start;
                while (slotStart + TimeSpan.FromMinutes(consultationTime) <= end)
                {
                    var slotEnd = slotStart + TimeSpan.FromMinutes(consultationTime);
                    availableSlots.Add($"{slotStart:hh\\:mm}-{slotEnd:hh\\:mm}");
                    slotStart = slotEnd;
                }
            }

            return availableSlots.ToArray();
        }
    }
}
