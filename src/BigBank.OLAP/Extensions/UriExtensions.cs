using System;

namespace BigBank.OLAP.Extensions
{
    internal static class UriExtensions
    {
        public static Uri OmitPort(this Uri uri)
        {
            var builder = new UriBuilder(uri);
            builder.Port = -1;
            return builder.Uri;
        }
    }
}