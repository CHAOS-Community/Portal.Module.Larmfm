using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Portal.Module.Larmfm.Helpers
{
    public static class TimeCodeHelper
    {

        public static string ConvertToTimeCode(string fromdt, string todt)
        {
            DateTime dtFrom = DateTime.MinValue;
            DateTime dtTo = DateTime.MinValue;
               

            //Check if both can be parsed as datetimes
            if (!DateTime.TryParse(fromdt, out dtFrom) && !DateTime.TryParse(todt, out dtTo))
                return string.Empty;

            if (dtTo.CompareTo(dtFrom) > 0)
                return string.Empty;

            return ConvertToTimeCode(dtFrom, dtTo);
        }

        public static string ConvertTimeCodeToSec(string timecode)
        {
            if (string.IsNullOrEmpty(timecode)) return "0";

            if (timecode.Split('.').Count() != 0) timecode = timecode.Split('.').First();

            return TimeSpan.Parse(timecode).TotalSeconds.ToString();
        }

        public static string ConvertToTimeCode(DateTime fromdt, DateTime todt)
        {
            var t = TimeSpan.FromSeconds(
                ConvertToDurationInSec(fromdt, todt)
                );

            

            return t.ToString(@"hh\:mm\:ss");
        }

        public static uint ConvertToDurationInSec(string fromdt, string todt)
        {
            DateTime dtFrom = DateTime.MinValue;
            DateTime dtTo = DateTime.MinValue;

            //Check if both can be parsed as datetimes
            if (!DateTime.TryParse(fromdt, out dtFrom) && !DateTime.TryParse(todt, out dtTo))
                return 0;

            if (dtTo.CompareTo(dtFrom) > 0)
                return 0;

            return ConvertToDurationInSec(dtFrom, dtTo);
        }

        public static uint ConvertToDurationInSec(DateTime fromdt, DateTime todt)
        {
            return ((uint)todt.Subtract(fromdt).TotalSeconds);
        }
    }
}
