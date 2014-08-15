
namespace Chaos.Portal.Module.Larmfm.Test.Helper
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class DateTimeHelperTest
    {
        [Test]
        public void Should_Convert_DateTimestrings()
        {
           
            var result = Helpers.DateTimeHelper.ParseAndFormatDate("2001-01-17T05:15:00Z");
            Assert.AreEqual("2001-01-17T05:15:00Z", result);

            result = Helpers.DateTimeHelper.ParseAndFormatDate("2001-01-17T05:00:00.000Z");

            Assert.AreEqual("2001-01-17T05:00:00Z", result);
        }
    }
}
