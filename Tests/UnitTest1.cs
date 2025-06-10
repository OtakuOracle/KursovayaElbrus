using Libary;
using NUnit.Framework;
using System;

namespace Library.Tests
{
    public class CalculationsTests
    {
        [Test]
        public void Test_BusyIntervalsWithNoFreeSlots() // ѕровер€ет случай с зан€ти€ми без свободных интервалов.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0) };
            int[] durations = { 60, 60 }; //  аждое зан€тие длитс€ 60 минут
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(11, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new string[0], result);
        }

        [Test]
        public void Test_ConsultationTooLongForFreeSlot() // ѕровер€ет случай с консультацией слишком длинной дл€ свободного интервала.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0) };
            int[] durations = { 120 }; // ƒлительность зан€ти€ больше доступного времени
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(10, 0, 0);
            int consultationTime = 60;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new string[0], result);
        }

        [Test]
        public void Test_SingleBusyIntervalWithNoFreeSlots() // ѕровер€ет случай с одним зан€тым интервалом на весь рабочий день.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0) };
            int[] durations = { 120 }; // «ан€тие занимает все врем€ рабочего дн€
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(11, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new string[0], result);
        }
        //досюдла


        [Test]
        public void AvailablePeriods_ExampleScenario_ReturnsExpectedSlots() // ¬озвращает ожидаемые временные слоты дл€ консультаций в заданном сценарии.
        {

            TimeSpan[] startTimes = new[]
            {
                TimeSpan.Parse("10:00"),
                TimeSpan.Parse("11:00"),
                TimeSpan.Parse("15:00"),
                TimeSpan.Parse("15:30"),
                TimeSpan.Parse("16:50")
            };
            int[] durations = { 60, 30, 10, 10, 40 }; // ƒлительности зан€тий в минутах
            TimeSpan beginWorkingTime = TimeSpan.Parse("08:00");
            TimeSpan endWorkingTime = TimeSpan.Parse("18:00");
            int consultationTime = 30;
            var expected = new[]
            {
                "08:00-08:30","08:30-09:00","09:00-09:30","09:30-10:00",
                "11:30-12:00","12:00-12:30","12:30-13:00","13:00-13:30",
                "13:30-14:00","14:00-14:30","14:30-15:00",
                "15:40-16:10","16:10-16:40","17:30-18:00"
            };


            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);


            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AvailablePeriods_NoFreeSlotLongConsultation_ReturnsEmpty() // ¬озвращает пустой результат, когда нет доступных временных слотов дл€ консультации заданной длительности.
        {

            TimeSpan[] startTimes = new[]
            {
                TimeSpan.Parse("08:00"),
                TimeSpan.Parse("09:30"),
                TimeSpan.Parse("11:00"),
                TimeSpan.Parse("13:00"),
                TimeSpan.Parse("15:00"),
                TimeSpan.Parse("17:00")
            };
            int[] durations = { 90, 90, 120, 120, 120, 60 }; // ƒлительности зан€тий в минутах
            TimeSpan beginWorkingTime = TimeSpan.Parse("08:00");
            TimeSpan endWorkingTime = TimeSpan.Parse("18:00");
            int consultationTime = 120;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);


            Assert.IsEmpty(result);
        }

        [Test]
        public void Test_MultipleBusyIntervalsWithFreeSlots() // ѕровер€ет случай с несколькими зан€ти€ми, оставл€ющими свободные интервалы.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0) };
            int[] durations = { 30, 30 }; //  аждое зан€тие занимает 30 минут
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new[] { "09:30-10:00", "10:30-11:00", "11:00-11:30", "11:30-12:00", "12:00-12:30", "12:30-13:00", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30", "15:30-16:00", "16:00-16:30", "16:30-17:00" }, result);
        }

        [Test]
        public void Test_BusyIntervalEndsAtClose() // ѕровер€ет случай с зан€тием, заканчивающимс€ в конце рабочего дн€.
        {
            TimeSpan[] startTimes = { new TimeSpan(16, 0, 0) };
            int[] durations = { 60 }; // «ан€тие занимает 60 минут
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new[] { "09:00-09:30", "09:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "11:30-12:00", "12:00-12:30", "12:30-13:00", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30", "15:30-16:00" }, result);
        }
        [Test]
        public void Test_SingleBusyIntervalInMiddle() // ѕровер€ет случай с одним зан€тым интервалом в середине рабочего дн€.
        {
            TimeSpan[] startTimes = { new TimeSpan(12, 0, 0) };
            int[] durations = { 60 }; // «ан€тие занимает 60 минут
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new[] { "09:00-09:30", "09:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "11:30-12:00", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30", "15:30-16:00", "16:00-16:30", "16:30-17:00" }, result);
        }
        [Test]
        public void Test_AllDayBusy() // ѕровер€ет случай, когда весь рабочий день зан€т.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0) };
            int[] durations = { 480 }; // «ан€тие занимает 480 минут (8 часов)
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30; // ¬рем€ консультации 30 минут

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new string[0], result); // ќжидаем пустой массив, так как все врем€ зан€то.
        }

        [Test]
        public void Test_LongBusyIntervalWithFreeSlots() // ѕровер€ет случай с одним длинным зан€тием и свободными интервалами.
        {
            TimeSpan[] startTimes = { new TimeSpan(11, 0, 0) };
            int[] durations = { 120 }; // «ан€тие занимает 120 минут (2 часа)
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30; // ¬рем€ консультации 30 минут

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new[] {
            "09:00-09:30", "09:30-10:00", "10:00-10:30", "10:30-11:00",
            "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00",
            "15:00-15:30", "15:30-16:00", "16:00-16:30", "16:30-17:00"
        }, result);
        }


    }
}
