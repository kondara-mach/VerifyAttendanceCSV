﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using Wada.AttendanceTableService;
using Wada.AttendanceTableService.AttendanceTableAggregation;

namespace Wada.AttendanceSpreadSheet.Tests
{
    [TestClass()]
    public class AttendanceTableRepositoryTests
    {
        [TestMethod()]
        public void 正常系_勤怠表が読み込めること()
        {
            // given
            DotNetEnv.Env.Load(".env");

            Mock<ILogger> mock_logger = new();
            IStreamOpener streamOpener = new StreamOpener(mock_logger.Object);
            string path = DotNetEnv.Env.GetString("TEST_XLS_PATH");
            using Stream xlsStream = streamOpener.Open(path);

            Mock<IWadaHolidayRepository> mock_holiday = new();
            #region mock_setup
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/1"))).Returns(HolidayClassification.LegalHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/2"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/3"))).Returns(HolidayClassification.RegularHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/4"))).Returns(HolidayClassification.RegularHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/5"))).Returns(HolidayClassification.RegularHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/6"))).Returns(HolidayClassification.RegularHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/7"))).Returns(HolidayClassification.RegularHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/8"))).Returns(HolidayClassification.LegalHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/9"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/10"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/11"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/12"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/13"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/14"))).Returns(HolidayClassification.RegularHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/15"))).Returns(HolidayClassification.LegalHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/16"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/17"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/18"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/19"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/20"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/21"))).Returns(HolidayClassification.RegularHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/22"))).Returns(HolidayClassification.LegalHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/23"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/24"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/25"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/26"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/27"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/28"))).Returns(HolidayClassification.RegularHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/29"))).Returns(HolidayClassification.LegalHoliday);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/30"))).Returns(HolidayClassification.None);
            mock_holiday.Setup(x => x.FindByDay(DateTime.Parse("2022/5/31"))).Returns(HolidayClassification.None);
            #endregion

            // when
            IAttendanceTableRepository attendanceTableRepository = new AttendanceTableRepository(mock_logger.Object, mock_holiday.Object);
            int month = 5;
            var actual = attendanceTableRepository.ReadByMonth(xlsStream, month);

            // then
            uint employeeNumber = (uint)DotNetEnv.Env.GetInt("EMPLOYEE_NUMBER");
            AttendanceTable expected = TestAttendanceTableFactory.Create(
                employeeNumber,
                new AttendanceYear(2022),
                new AttendanceMonth(month),
                CreateTestRecords());
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.EmployeeNumber, actual.EmployeeNumber);
            Assert.AreEqual(expected.Year, actual.Year);
            Assert.AreEqual(expected.Month, actual.Month);
            CollectionAssert.AreEqual(expected.AttendanceRecords.ToList(), actual.AttendanceRecords.ToList());
            mock_holiday.Verify(x => x.FindByDay(It.IsAny<DateTime>()), Times.Exactly(19));
        }

        private static ICollection<AttendanceRecord> CreateTestRecords()
        {
            ICollection<AttendanceRecord> records = new List<AttendanceRecord>
            {
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022),new AttendanceMonth(5),2),HolidayClassification.None,DayOffClassification.PaidLeave,null,null,null,OrderedLunchBox.None),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 9), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/09 08:00")), new AttendanceTime(DateTime.Parse("2022/05/09 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 10), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/10 08:00")), new AttendanceTime(DateTime.Parse("2022/05/10 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 11), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/11 08:00")), new AttendanceTime(DateTime.Parse("2022/05/11 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 12), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/12 08:00")), new AttendanceTime(DateTime.Parse("2022/05/12 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 13), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/13 08:00")), new AttendanceTime(DateTime.Parse("2022/05/13 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 14), HolidayClassification.RegularHoliday, DayOffClassification.TransferedAttendance, new AttendanceTime(DateTime.Parse("2022/05/14 08:00")), new AttendanceTime(DateTime.Parse("2022/05/14 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 16), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/16 08:00")), new AttendanceTime(DateTime.Parse("2022/05/16 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 17), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/17 08:00")), new AttendanceTime(DateTime.Parse("2022/05/17 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 18), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/18 08:00")), new AttendanceTime(DateTime.Parse("2022/05/18 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 19), HolidayClassification.None, DayOffClassification.SubstitutedHoliday, null, null, null, OrderedLunchBox.None),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 20), HolidayClassification.None, DayOffClassification.PaidLeave, null, null, null, OrderedLunchBox.None),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 23), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/23 08:00")), new AttendanceTime(DateTime.Parse("2022/05/23 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 24), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/24 08:00")), new AttendanceTime(DateTime.Parse("2022/05/24 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 25), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/25 08:00")), new AttendanceTime(DateTime.Parse("2022/05/25 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 26), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/26 08:00")), new AttendanceTime(DateTime.Parse("2022/05/26 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 27), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/27 08:00")), new AttendanceTime(DateTime.Parse("2022/05/27 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 30), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/30 08:00")), new AttendanceTime(DateTime.Parse("2022/05/30 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
                new AttendanceRecord(new AttendanceDay(new AttendanceYear(2022), new AttendanceMonth(5), 31), HolidayClassification.None, DayOffClassification.None, new AttendanceTime(DateTime.Parse("2022/05/31 08:00")), new AttendanceTime(DateTime.Parse("2022/05/31 17:00")), new TimeSpan(1, 0, 0), OrderedLunchBox.Orderd),
            };
            return records;
        }
    }
}