using Libary;
using NUnit.Framework;
using System;

namespace Library.Tests
{
    public class CalculationsTests
    {
        [Test]
        public void Test_BusyIntervalsWithNoFreeSlots() // ��������� ������ � ��������� ��� ��������� ����������.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0) };
            int[] durations = { 60, 60 }; // ������ ������� ������ 60 �����
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(11, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new string[0], result);
        }

        [Test]
        public void Test_ConsultationTooLongForFreeSlot() // ��������� ������ � ������������� ������� ������� ��� ���������� ���������.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0) };
            int[] durations = { 120 }; // ������������ ������� ������ ���������� �������
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(10, 0, 0);
            int consultationTime = 60;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new string[0], result);
        }

        [Test]
        public void Test_SingleBusyIntervalWithNoFreeSlots() // ��������� ������ � ����� ������� ���������� �� ���� ������� ����.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0) };
            int[] durations = { 120 }; // ������� �������� ��� ����� �������� ���
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(11, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new string[0], result);
        }
        //�������


        [Test]
        public void AvailablePeriods_ExampleScenario_ReturnsExpectedSlots() // ���������� ��������� ��������� ����� ��� ������������ � �������� ��������.
        {

            TimeSpan[] startTimes = new[]
            {
                TimeSpan.Parse("10:00"),
                TimeSpan.Parse("11:00"),
                TimeSpan.Parse("15:00"),
                TimeSpan.Parse("15:30"),
                TimeSpan.Parse("16:50")
            };
            int[] durations = { 60, 30, 10, 10, 40 }; // ������������ ������� � �������
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
        public void AvailablePeriods_NoFreeSlotLongConsultation_ReturnsEmpty() // ���������� ������ ���������, ����� ��� ��������� ��������� ������ ��� ������������ �������� ������������.
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
            int[] durations = { 90, 90, 120, 120, 120, 60 }; // ������������ ������� � �������
            TimeSpan beginWorkingTime = TimeSpan.Parse("08:00");
            TimeSpan endWorkingTime = TimeSpan.Parse("18:00");
            int consultationTime = 120;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);


            Assert.IsEmpty(result);
        }

        [Test]
        public void Test_MultipleBusyIntervalsWithFreeSlots() // ��������� ������ � ����������� ���������, ������������ ��������� ���������.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0) };
            int[] durations = { 30, 30 }; // ������ ������� �������� 30 �����
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new[] { "09:30-10:00", "10:30-11:00", "11:00-11:30", "11:30-12:00", "12:00-12:30", "12:30-13:00", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30", "15:30-16:00", "16:00-16:30", "16:30-17:00" }, result);
        }

        [Test]
        public void Test_BusyIntervalEndsAtClose() // ��������� ������ � ��������, ��������������� � ����� �������� ���.
        {
            TimeSpan[] startTimes = { new TimeSpan(16, 0, 0) };
            int[] durations = { 60 }; // ������� �������� 60 �����
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new[] { "09:00-09:30", "09:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "11:30-12:00", "12:00-12:30", "12:30-13:00", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30", "15:30-16:00" }, result);
        }
        [Test]
        public void Test_SingleBusyIntervalInMiddle() // ��������� ������ � ����� ������� ���������� � �������� �������� ���.
        {
            TimeSpan[] startTimes = { new TimeSpan(12, 0, 0) };
            int[] durations = { 60 }; // ������� �������� 60 �����
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new[] { "09:00-09:30", "09:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "11:30-12:00", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30", "15:30-16:00", "16:00-16:30", "16:30-17:00" }, result);
        }
        [Test]
        public void Test_AllDayBusy() // ��������� ������, ����� ���� ������� ���� �����.
        {
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0) };
            int[] durations = { 480 }; // ������� �������� 480 ����� (8 �����)
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30; // ����� ������������ 30 �����

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new string[0], result); // ������� ������ ������, ��� ��� ��� ����� ������.
        }

        [Test]
        public void Test_LongBusyIntervalWithFreeSlots() // ��������� ������ � ����� ������� �������� � ���������� �����������.
        {
            TimeSpan[] startTimes = { new TimeSpan(11, 0, 0) };
            int[] durations = { 120 }; // ������� �������� 120 ����� (2 ����)
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30; // ����� ������������ 30 �����

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            Assert.AreEqual(new[] {
            "09:00-09:30", "09:30-10:00", "10:00-10:30", "10:30-11:00",
            "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00",
            "15:00-15:30", "15:30-16:00", "16:00-16:30", "16:30-17:00"
        }, result);
        }


    }
}
