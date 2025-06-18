using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBoard.Tests.Helpers
{
    public static class TestHelpers
    {
        /// <summary>
        /// מתודת עזר לקבלת זמן ישראל (במידה והשתמשת בזה בפרויקט)א
        /// </summary>
        public static DateTime GetIsraelTime()
        {
            try
            {
                var israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
            }
            catch
            {
                try
                {
                    var israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jerusalem");
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                }
                catch
                {
                    return DateTime.UtcNow.AddHours(3);
                }
            }
        }
    }
}
