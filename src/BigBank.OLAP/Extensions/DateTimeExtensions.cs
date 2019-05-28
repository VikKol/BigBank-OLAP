using System;
using System.Globalization;

namespace BigBank.OLAP.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToIsoDateTimeString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}