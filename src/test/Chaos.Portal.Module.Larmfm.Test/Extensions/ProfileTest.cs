using System;
using System.Xml.Linq;
using Chaos.Mcm.Data.Dto;
using Chaos.Portal.Core.Data.Model;
using Chaos.Portal.Core.Exceptions;
using Chaos.Portal.Module.Larmfm.Extensions;
using NUnit.Framework;
using Object = Chaos.Mcm.Data.Dto.Object;

namespace Chaos.Portal.Module.Larmfm.Test.Extensions
{
  [TestFixture]
  public class ProfileTest : TestBase
  {
    [Test, ExpectedException(typeof (InsufficientPermissionsException))]
    public void Get_GivenAnonymousUser_Throw()
    {
      var extension = Make_ProfileExtension();
      PortalRequest.Setup(p => p.IsAnonymousUser).Returns(true);

      extension.Get();
    }

    [Test]
    public void Get_GivenProfileObjectDoesntExist_ReturnEmptyProfile()
    {
      var extension = Make_ProfileExtension();
      PortalRequest.Setup(p => p.User).Returns(new UserInfo {Guid = new Guid("10000000-0000-0000-0000-000000000001")});

      var result = extension.Get();

      Assert.That(result.Name, Is.EqualTo(string.Empty));
      Assert.That(result.Title, Is.EqualTo(string.Empty));
      Assert.That(result.About, Is.EqualTo(string.Empty));
      Assert.That(result.Organization, Is.EqualTo(string.Empty));
      Assert.That(result.Emails, Is.Empty);
      Assert.That(result.PhoneNumbers, Is.Empty);
      Assert.That(result.Websites, Is.Empty);
      Assert.That(result.Skype, Is.EqualTo(string.Empty));
      Assert.That(result.LinkedIn, Is.EqualTo(string.Empty));
      Assert.That(result.Twitter, Is.EqualTo(string.Empty));
      Assert.That(result.Address, Is.EqualTo(string.Empty));
      Assert.That(result.City, Is.EqualTo(string.Empty));
      Assert.That(result.ZipCode, Is.EqualTo(string.Empty));
      Assert.That(result.Country, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Get_GivenProfileObjectExists_ReturnProfileResult()
    {
      var extension = Make_ProfileExtension();
      PortalRequest.Setup(p => p.User).Returns(new UserInfo { Guid = new Guid("10000000-0000-0000-0000-000000000001") });
      McmRepository.Setup(
        m => m.ObjectGet(new Guid("10000000-0000-0000-0000-000000000001"), true, false, false, false, false))
                   .Returns(new Object
                     {
                       Metadatas = new []{new Metadata
                         {
                           MetadataXml = XDocument.Parse("<CHAOS.Profile><Name>John Doe</Name><Title>Phd</Title><About>about text</About><Organization>DTU</Organization><Emails><Email>john.doe@dtu.dk</Email></Emails><Phonenumbers><Phonenumber>(+45) 8888 8888</Phonenumber></Phonenumbers><Websites><Website>www.example.com</Website></Websites><Skype>john.dtu.doe</Skype><LinkedIn>link</LinkedIn><Twitter>link</Twitter><Address>street and number</Address><City>city name</City><Zipcode>1234</Zipcode><Country>DK</Country></CHAOS.Profile>")
                         }}
                     });

      var result = extension.Get();

      Assert.That(result.Name, Is.EqualTo("John Doe"));
      Assert.That(result.Title, Is.EqualTo("Phd"));
      Assert.That(result.About, Is.EqualTo("about text"));
      Assert.That(result.Organization, Is.EqualTo("DTU"));
      Assert.That(result.Emails[0], Is.EqualTo("john.doe@dtu.dk"));
      Assert.That(result.PhoneNumbers[0], Is.EqualTo("(+45) 8888 8888"));
      Assert.That(result.Websites[0], Is.EqualTo("www.example.com"));
      Assert.That(result.Skype, Is.EqualTo("john.dtu.doe"));
      Assert.That(result.LinkedIn, Is.EqualTo("link"));
      Assert.That(result.Twitter, Is.EqualTo("link"));
      Assert.That(result.Address, Is.EqualTo("street and number"));
      Assert.That(result.City, Is.EqualTo("city name"));
      Assert.That(result.ZipCode, Is.EqualTo("1234"));
      Assert.That(result.Country, Is.EqualTo("DK"));
    }

    private Profile Make_ProfileExtension()
    {
      return (Profile) new Profile(PortalApplication.Object, McmRepository.Object, new LarmSettings()).WithPortalRequest(PortalRequest.Object);
    }
  }
}