using System;
using System.Globalization;

namespace Chaos.Portal.Module.Larmfm.Helpers
{
    public static class DateTimeHelper
    {
        private const string dateTimeFormat1 = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
        private const string dateTimeFormat2 = "dd'-'MM'-'yyyy HH':'mm':'ss";
        private const string dateTimeFormat3 = "dd'/'MM'/'yyyy HH':'mm':'ss";
        private const string dateTimeFormat4 = "yyyy'-'MM'-'dd HH':'mm':'ss";
        private const string dateTimeFormat5 = "yyyy-MM-ddTHH:mm:ss.fffzzz";
        private const string dateTimeFormat6 = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z";

        /// <summary>
        /// TryParse to DateTime and returns a formatted string. Returns string.Empty if parse failed.
        /// </summary>
        /// <param name="datetext"></param>
        /// <returns></returns>
        public static string ParseAndFormatDate(string datetext)
        {
            if (string.IsNullOrEmpty(datetext))
                return string.Empty;

            // HACK: Programoversigt has a year issue. The OCR says 1056, but it is 1956.
            datetext = datetext.Replace("1056", "1956").Replace("Z", "").Replace(".000", "");

            CultureInfo formatProvider = CultureInfo.InvariantCulture;
            DateTime result;

            if (DateTime.TryParseExact(datetext, DateTimeFormats, formatProvider, DateTimeStyles.None, out result))
            {
                return result.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", formatProvider);
            }

            //TODO
            return result.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", formatProvider);
        }

        private static string[] DateTimeFormats
        {
            get
            {
                return new string[] { dateTimeFormat1, dateTimeFormat2, dateTimeFormat3, dateTimeFormat4, dateTimeFormat5, dateTimeFormat6 };
            }
        }
    }
}
