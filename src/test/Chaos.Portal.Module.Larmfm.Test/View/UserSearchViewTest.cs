namespace Chaos.Portal.Module.Larmfm.Test.View
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using CHAOS;
    using CHAOS.Extensions;
    using Larmfm.View;
    using Mcm.Data.Dto;
    using NUnit.Framework;
    using Object = Mcm.Data.Dto.Object;

    [TestFixture]
    public class UserSearchViewTest : TestBase
    {
        [Test]
        public void Index_NotGivenAnObject_Ignore()
        {
            var view = new UserSearchView();

            var results = view.Index("not an a valid input");

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Index_GivenWrongObjectType_Ignore()
        {
            var view = new UserSearchView();
            var wrongObjectType = new Object();

            var results = view.Index(wrongObjectType);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Index_MissingMetadata_Ignore()
        {
            var view = new UserSearchView();
            var wrongObjectType = new Object{ObjectTypeID = 55};

            var results = view.Index(wrongObjectType);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Index_GivenProfileObject_MapToUserSearchViewData()
        {
            var view = new UserSearchView();
            var profileObject = Make_ProfileObject();

            var result = (UserSearchViewData)view.Index(profileObject).First();

            Assert.That(result.Name, Is.EqualTo("John Doe"));
        }
        
        private Object Make_ProfileObject()
        {
            return new Object
                {
                    ObjectTypeID = 55,
                    Metadatas = new[]
                        {
                            new Metadata
                                {
                                    MetadataSchemaGuid = new UUID("1FD4E56E-3F3A-4F25-BA3E-3D9F80D5D49E").ToGuid(),
                                    MetadataXml = XDocument.Parse(@"<CHAOS.Profile>
                                                                        <Name>John Doe</Name>
                                                                        <Title></Title>
                                                                        <About />
                                                                        <Organization></Organization>
                                                                        <Emails>
                                                                        <Email>john@doe.com</Email>
                                                                        </Emails>
                                                                        <Phonenumbers />
                                                                        <Websites />
                                                                        <Skype />
                                                                        <LinkedIn />
                                                                        <Twitter />
                                                                        <Address />
                                                                        <City />
                                                                        <ZipCode />
                                                                        <Country>United States</Country>
                                                                    </CHAOS.Profile>")
                                }
                        }
                };
        }

        [Test]
        public void GetIndexableFields_GivenValidData_ReturnFields()
        {
            var viewData = new UserSearchViewData
                {
                    Identifier = "10000000-0000-0000-0000-000000000001",
                    Name = "John Doe"
                };

            var fields = viewData.GetIndexableFields().ToDictionary(item => item.Key);

            Assert.That(fields["Id"].Value, Is.EqualTo(viewData.Identifier));
            Assert.That(fields["Name"].Value, Is.EqualTo("john doe"));
        }
    }
}