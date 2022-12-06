﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Reflection;
using Wada.AttendanceTableService;
using Wada.AttendanceTableService.OwnCompanyCalendarAggregation;
using Wada.AttendanceTableService.ValueObjects;

namespace Wada.DesignDepartmentDataBse
{
    public class OwnCompanyHolidayRepository : IOwnCompanyHolidayRepository
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public OwnCompanyHolidayRepository(ILogger logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public void AddRange(IEnumerable<OwnCompanyHoliday> ownCompanyHolidays)
        {
            logger.Debug($"Start {MethodBase.GetCurrentMethod()?.Name}");

            DbConfig dbConfig = new(configuration);
            using var dbContext = new DbContext(dbConfig);

            dbContext.OwnCompanyHolidays!
                .AddRange(
                ownCompanyHolidays.Select(x => new Models.OwnCompanyHoliday(
                    x.HolidayDate,
                    x.HolidayClassification == HolidayClassification.LegalHoliday)));

            int _additionalNumber;
            try
            {
                _additionalNumber = dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                string msg = "登録済みの日付の自社カレンダーは追加・上書きできません";
                logger.Error(ex, msg);
                throw new AttendanceTableServiceException(msg, ex);
            }
            logger.Info($"データベースに{_additionalNumber}件追加しました");

            logger.Debug($"Finish {MethodBase.GetCurrentMethod()?.Name}");
        }

        public IEnumerable<OwnCompanyHoliday> FindByYearMonth(int year, int month)
        {
            logger.Debug($"Start {MethodBase.GetCurrentMethod()?.Name}");

            DbConfig dbConfig = new(configuration);
            using var dbContext = new DbContext(dbConfig);

            var ownHoliday = dbContext.OwnCompanyHolidays!
                .Where(x => x.HolidayDate >= new DateTime(year, month, 1))
                .Where(x => x.HolidayDate < new DateTime(year, month, 1).AddMonths(1))
                .Select(x => OwnCompanyHoliday.ReConstruct(
                    x.HolidayDate,
                    x.LegalHoliday ? HolidayClassification.LegalHoliday : HolidayClassification.RegularHoliday));

            if (!ownHoliday.Any())
            {
                string msg = $"自社カレンダーに該当がありませんでした "
                             + $"対象年月: {year}年{month}月";
                logger.Error(msg);
                throw new AttendanceTableServiceException(msg);
            }

            logger.Debug($"Finish {MethodBase.GetCurrentMethod()?.Name}");

            return ownHoliday.ToList();
        }

        public DateTime MaxDate()
        {
            logger.Debug($"Start {MethodBase.GetCurrentMethod()?.Name}");

            DbConfig dbConfig = new(configuration);
            using var dbContext = new DbContext(dbConfig);

            var maxHoliday = dbContext.OwnCompanyHolidays!
                .Max(x => x.HolidayDate);

            logger.Debug($"Finish {MethodBase.GetCurrentMethod()?.Name}");

            return maxHoliday;
        }
    }
}