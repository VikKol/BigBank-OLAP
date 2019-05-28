using System;
using System.Globalization;

namespace BigBank.IntegrationTests.Core
{
    internal static class DateTimeHelpers
    {
        public static DateTime Parse(string datetimeString)
        {
            return DateTime.ParseExact(datetimeString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}