namespace Chaos.Portal.Module.Larmfm.Test.Extensions
{
    using Core.Data.Model;
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

            var expected = "(Name:firstname*)(Name_split:firstname*)";
            PortalApplication.Verify(m => m.ViewManager.GetView("Users").Query(It.Is<IQuery>(q => q.Query == expected)));
        }
        
        [Test]
        public void User_GivenNameWithSpaces_SendQUeryToView()
        {
            var extension = new Search(PortalApplication.Object);
            var query = "firstname lastname";
            PortalApplication.Setup(m => m.ViewManager.GetView("Users").Query(It.IsAny<IQuery>()));

            extension.Users(query);

            var expected = "(Name:firstname\\ lastname*)(Name_split:firstname\\ lastname*)";
            PortalApplication.Verify(m => m.ViewManager.GetView("Users").Query(It.Is<IQuery>(q => q.Query == expected)));
        }

        [Test]
        public void User_UppercaseQuery_QueryIsMadeLowerCase()
        {
            var extension = new Search(PortalApplication.Object);
            var query = "Firstname";
            PortalApplication.Setup(m => m.ViewManager.GetView("Users").Query(It.IsAny<IQuery>()));

            extension.Users(query);

            var expected = "(Name:firstname*)(Name_split:firstname*)";
            PortalApplication.Verify(m => m.ViewManager.GetView("Users").Query(It.Is<IQuery>(q => q.Query == expected)));
        }
        
        [Test]
        public void User_PageIndexNotSpecified_UseDefault()
        {
            var extension = new Search(PortalApplication.Object);
            PortalApplication.Setup(m => m.ViewManager.GetView("Users").Query(It.IsAny<IQuery>()));

            extension.Users("");

            PortalApplication.Verify(m => m.ViewManager.GetView("Users").Query(It.Is<IQuery>(q => q.PageIndex == 0)));
        }
        
        [Test]
        public void User_PageSizeNotSpecified_UseDefault()
        {
            var extension = new Search(PortalApplication.Object);
            PortalApplication.Setup(m => m.ViewManager.GetView("Users").Query(It.IsAny<IQuery>()));

            extension.Users("");

            PortalApplication.Verify(m => m.ViewManager.GetView("Users").Query(It.Is<IQuery>(q => q.PageSize == 10)));
        }

        [Test]
        public void User_GivenQuery_ReturnListFromView()
        {
            var extension = new Search(PortalApplication.Object);
            var expected = new PagedResult<IResult>(0, 0, null);
            PortalApplication.Setup(m => m.ViewManager.GetView("Users").Query(It.IsAny<IQuery>())).Returns(expected);

            var actual = extension.Users("query");

            Assert.That(actual, Is.SameAs(expected));
        }
    }
}
