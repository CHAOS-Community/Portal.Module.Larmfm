namespace Chaos.Portal.Module.Larmfm.Test.View
{
    using System;
    using System.Collections.Generic;

    using CHAOS;
    using CHAOS.Extensions;

    using NUnit.Framework;
    using System.Xml.Linq;
    using Mcm.Data.Dto;
    using System.Linq;
    using Larmfm.View;
    using Object = Mcm.Data.Dto.Object;

    [TestFixture]
    public class SearchViewTest
    {
        [Test]
        public void Index_GivenRadioObject_ReturnViewDataWithPropertiesSet()
        {
            var view = new SearchView();
            var obj  = Make_Radio_Object();

            var result = (SearchViewData) view.Index(obj).First();

            Assert.That(result.Id, Is.EqualTo("00000000-0000-0000-0000-000000000001"));
            Assert.That(result.Title, Is.EqualTo("P7 MIX"));
            Assert.That(result.Type, Is.EqualTo("Radio"));
            Assert.That(result.PubStartDate,  Is.EqualTo("2012-02-21T16:03:00Z"));
            Assert.That(result.PubEndDate, Is.EqualTo("2012-02-22T00:03:00Z"));
            Assert.That(result.PubStartDate, Is.EqualTo(Helpers.DateTimeHelper.ParseAndFormatDate("2012-02-21T16:03:00")));
            Assert.That(result.PubEndDate, Is.EqualTo(Helpers.DateTimeHelper.ParseAndFormatDate("2012-02-22T00:03:00")));
        }


        [Test]
        public void Index_GivenScheduleObject_ReturnViewDataWithPropertiesSet()
        {
            var view = new SearchView();
            var obj  = Make_Schedule_Object();

            var result = (SearchViewData)view.Index(obj).First();

            Assert.That(result.Id, Is.EqualTo("00000000-0000-0000-0000-000000000002"));
            Assert.That(result.Title, Is.EqualTo("A-1967-04-01-P-0042.pdf"));
            Assert.That(result.Type, Is.EqualTo("Schedule"));
            Assert.That(result.FreeText, Is.EqualTo("Test data content."));
            Assert.That(result.Url, Is.EqualTo("http://s3-eu-west-1.amazonaws.com/chaosdata/Hvideprogrammer/arkiv_B/1976_10-12/PDF/B-1976-12-02-P-0107.pdf"));
            Assert.That(result.PubStartDate,  Is.EqualTo("1967-04-01T00:00:00Z"));
            Assert.That(result.PubStartDate, Is.EqualTo(Helpers.DateTimeHelper.ParseAndFormatDate("1967-04-01T00:00:00")));
            Assert.That(result.PubEndDate, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Index_GivenScheduleNoteObject_ReturnViewDataWithPropertiesSet()
        {
            var view = new SearchView();
            var obj = Make_ScheduleNote_Object();

            var result = (SearchViewData)view.Index(obj).First();

            Assert.That(result.Id, Is.EqualTo("00000000-0000-0000-0000-000000000003"));
            Assert.That(result.Title, Is.EqualTo("A-1964-10-24-S-0321.pdf"));
            Assert.That(result.Type, Is.EqualTo("ScheduleNote"));
            Assert.That(result.FreeText, Is.EqualTo("Test data content."));
            Assert.That(result.Url, Is.EqualTo("http://s3-eu-west-1.amazonaws.com/chaosdata/Hvideprogrammer/arkiv_A/1964_10_2/PDF/A-1964-10-24-S-0321.pdf"));
            Assert.That(result.PubStartDate, Is.EqualTo("1964-10-24T00:00:00Z"));
            Assert.That(result.PubStartDate, Is.EqualTo(Helpers.DateTimeHelper.ParseAndFormatDate("1964-10-24T00:00:00")));
            Assert.That(result.PubEndDate, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetIndexableFields_GivenSearchViewData_ReturnTitleField()
        {
            var data = new SearchViewData
                {
                    Id       = "00000000-0000-0000-0000-000000000001",
                    Title    = "P7 MIX",
                    Type     = "Radio",
                    FreeText = "test text",
                    PubStartDate = "2012-02-21T16:03:00",
                    PubEndDate = "2012-02-22T00:03:00"
                };

            var result = data.GetIndexableFields().ToList();

            Assert.That(result.Any(item => item.Key == "Id" && item.Value == "00000000-0000-0000-0000-000000000001"), Is.True);
            Assert.That(result.Any(item => item.Key == "Title" && item.Value == "P7 MIX"), Is.True);
            Assert.That(result.Any(item => item.Key == "Type" && item.Value == "Radio"), Is.True);
            Assert.That(result.First(item => item.Key == "FreeText").Value, Is.EqualTo("test text"));
            Assert.That(result.First(item => item.Key == "PubStartDate").Value, Is.EqualTo("2012-02-21T16:03:00"));
            Assert.That(result.First(item => item.Key == "PubEndDate").Value, Is.EqualTo("2012-02-22T00:03:00"));
        }

        private static Object Make_Radio_Object()
        {
            return new Object
                {
                    Guid = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    ObjectTypeID = 24,
                    Metadatas = new List<Metadata>
                        {
                            new Metadata
                                {
                                    MetadataSchemaGuid = Guid.Parse("00000000-0000-0000-0000-0000df820000"),
                                    MetadataXml        = XDocument.Parse("<Larm.Program><PublicationDateTime>2012-02-21T16:03:00</PublicationDateTime><PublicationEndDateTime>2012-02-22T00:03:00</PublicationEndDateTime><PublicationChannel>DR P7 Mix</PublicationChannel><Title>P7 MIX</Title><Abstract></Abstract><Description>Musik med pop og sjæl. </Description><Publisher></Publisher><Subjects /><Contributors></Contributors><Creators><Creator><Name></Name><RoleName></RoleName><RoleID></RoleID></Creator></Creators><Locations /><Identifiers><DR.ProductionNumber></DR.ProductionNumber><DR.ArchiveNumber></DR.ArchiveNumber><SB.DomsID>c431cb1d-081a-47de-a7d4-cd4275a7063a</SB.DomsID></Identifiers></Larm.Program>")
                                }
                        }
                };
        }

        private static Object Make_Schedule_Object()
        {
            return new Object
            {
                Guid = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                ObjectTypeID = 86,
                Metadatas = new List<Metadata>
                        {
                            new Metadata
                                {
                                    MetadataSchemaGuid = new UUID("70c26faf-b1ee-41e8-b916-a5a16b25ca69").ToGuid(),
                                    MetadataXml        = XDocument.Parse(@"<Larm.HvideProgram><Titel></Titel><Filename>A-1967-04-01-P-0042.pdf</Filename><AllText>Test data content.</AllText><Date>1967-04-01T00:00:00</Date><Type>Program</Type></Larm.HvideProgram>")
                                }
                        },
                Files = new List<FileInfo>
                {
                    new FileInfo{
                        ID = 3880365,
                        Filename = "B-1976-12-02-P-0107.pdf",
                        OriginalFilename = "B-1976-12-02-P-0107.pdf",
                        Token = "HTTP Download",
                        StringFormat = "{BASE_PATH}{FOLDER_PATH}{FILENAME}",
                        BasePath = "http://s3-eu-west-1.amazonaws.com/chaosdata",
                        FolderPath = "/Hvideprogrammer/arkiv_B/1976_10-12/PDF/",
                        FormatID = 38,
                        Format = "PDF",
                        FormatCategory = "PDF",
                        FormatType = "Other"
                    }
                }
            };
        }

        private static Object Make_ScheduleNote_Object()
        {
            return new Object
            {
                Guid = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                ObjectTypeID = 87,
                Metadatas = new List<Metadata>
                        {
                            new Metadata
                                {
                                    MetadataSchemaGuid = new UUID("70c26faf-b1ee-41e8-b916-a5a16b25ca69").ToGuid(),
                                    MetadataXml        = XDocument.Parse(@"<Larm.HvideProgram><Titel></Titel><Filename>A-1964-10-24-S-0321.pdf</Filename><AllText>Test data content.</AllText><Date>1964-10-24T00:00:00</Date><Type>Supplerende</Type></Larm.HvideProgram>")
                                }
                        },
                Files = new List<FileInfo>
                {
                    new FileInfo{
                        ID = 3880365,
                        Filename = "A-1964-10-24-S-0321.pdf",
                        OriginalFilename = "A-1964-10-24-S-0321.pdf",
                        Token = "HTTP Download",
                        StringFormat = "{BASE_PATH}{FOLDER_PATH}{FILENAME}",
                        BasePath = "http://s3-eu-west-1.amazonaws.com/chaosdata",
                        FolderPath = "/Hvideprogrammer/arkiv_A/1964_10_2/PDF/",
                        FormatID = 38,
                        Format = "PDF",
                        FormatCategory = "PDF",
                        FormatType = "Other"
                    }
                }
            };
        }
    }
}