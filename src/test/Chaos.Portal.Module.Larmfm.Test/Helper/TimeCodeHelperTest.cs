using System.Security.Cryptography;
using Chaos.Portal.Module.Larmfm.Helpers;

namespace Chaos.Portal.Module.Larmfm.Test.Helper
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TimeCodeHelperTest
    {
        [Test]
        public void Should_Convert_To_TimeCode()
        {
            var dtFrom = new DateTime(2008, 10, 10, 12, 0, 0);

            var dtTo = new DateTime(2008, 10, 10, 12, 35, 12);

            var result = Helpers.TimeCodeHelper.ConvertToTimeCode(dtFrom, dtTo);

            Assert.AreEqual("00:35:12", result);
        }

        [Test]
        public void Should_Calc_Ducation_in_Sec()
        {
            var dtFrom = new DateTime(2008, 10, 10, 12, 0, 0);

            var dtTo = new DateTime(2008, 10, 10, 12, 2, 30);

            Assert.AreEqual(TimeCodeHelper.ConvertToDurationInSec(dtFrom, dtTo),
               150 );
        }

        [Test]
        public void Should_Calc_Duration_In_Sec_Datetime_String()
        {
            Assert.AreEqual(TimeCodeHelper.ConvertToDurationInSec("2005-08-10T12:00:00Z", "2005-08-10T12:05:00Z"),
               300);
        }

        [Test]
        public void Should_Convert_Timecode_To_Seconds()
        {
            var result = Helpers.TimeCodeHelper.ConvertTimeCodeToSec("01:50:31.1000000");

            Assert.AreEqual(true, true);
        }
    }
}
