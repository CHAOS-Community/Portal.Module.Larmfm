namespace Chaos.Portal.Module.Larmfm.Test.Extensions
{
    using Core.Indexing;
    using Larmfm.Extensions;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class SearchTest : TestBase
    {
        [Test]
        public void User_GivenOneWord_SendQUeryToView()
        {
            var extension = new Search(PortalApplication.Object);
            var query = "firstname";
            PortalApplication.Setup(m => m.ViewManager.GetView("Users").Query(It.IsAny<IQuery>()));

            extension.Users(query);

            var expected = "";
            PortalApplication.Verify(m => m.ViewManager.GetView("Users").Query(It.Is<IQuery>(q => q.Query == expected)));
        }
    }
}
