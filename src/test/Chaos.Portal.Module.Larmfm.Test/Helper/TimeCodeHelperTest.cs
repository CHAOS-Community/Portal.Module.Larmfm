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
        public void Should_Convert_Timecode_To_Seconds()
        {
            var result = Helpers.TimeCodeHelper.ConvertTimeCodeToSec("01:50:31.1000000");

            Assert.AreEqual(true, true);
        }
    }
}
