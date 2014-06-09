namespace Chaos.Portal.Module.Larmfm.Test.View
{
    using System;
    using System.Collections.Generic;

    using CHAOS;
    using CHAOS.Extensions;
    using System.Xml.Linq;
    using Mcm.Data.Dto;
    using System.Linq;
    using Larmfm.View;
    using NUnit.Framework;
    using Object = Mcm.Data.Dto.Object;

    [TestFixture]
    public class SearchViewTest : TestBase
    {
        [Test]
        public void Index_GivenRadioObject_ReturnViewDataWithPropertiesSet()
        {
            var view = new SearchView(McmRepository.Object);
            var obj  = Make_Radio_Object();

            var result = (SearchViewData) view.Index(obj).First();

            Assert.AreEqual(result.Id, "00000000-0000-0000-0000-000000000001");
            Assert.AreEqual(result.Title, "P7 MIX");
            Assert.AreEqual(result.Type, "Radio");
            Assert.AreEqual(result.PubStartDate, "2012-02-21T16:03:00Z");
            Assert.AreEqual(result.PubEndDate, "2012-02-22T00:03:00Z");
            Assert.AreEqual(result.PubStartDate, (Helpers.DateTimeHelper.ParseAndFormatDate("2012-02-21T16:03:00")));
            Assert.AreEqual(result.PubEndDate, Helpers.DateTimeHelper.ParseAndFormatDate("2012-02-22T00:03:00"));
        }

        [Test]
        public void Index_GivenScheduleObject_ReturnViewDataWithPropertiesSet()
        {
            var view = new SearchView(McmRepository.Object);
            var obj  = Make_Schedule_Object();

            var result = (SearchViewData)view.Index(obj).First();

            Assert.AreEqual(result.Id, ("00000000-0000-0000-0000-000000000002"));
            Assert.AreEqual(result.Title, ("A-1967-04-01-P-0042.pdf"));
            Assert.AreEqual(result.Type, ("Schedule"));
            Assert.AreEqual(result.FreeText, ("Test data content."));
            Assert.AreEqual(result.Url, ("http://s3-eu-west-1.amazonaws.com/chaosdata/Hvideprogrammer/arkiv_B/1976_10-12/PDF/B-1976-12-02-P-0107.pdf"));
            Assert.AreEqual(result.PubStartDate,  ("1967-04-01T00:00:00Z"));
            Assert.AreEqual(result.PubStartDate, (Helpers.DateTimeHelper.ParseAndFormatDate("1967-04-01T00:00:00")));
            Assert.AreEqual(result.PubEndDate, (string.Empty));
        }

        [Test]
        public void Index_GivenScheduleNoteObject_ReturnViewDataWithPropertiesSet()
        {
            var view = new SearchView(McmRepository.Object);
            var obj = Make_ScheduleNote_Object();

            var result = (SearchViewData)view.Index(obj).First();

            Assert.AreEqual(result.Id, ("00000000-0000-0000-0000-000000000003"));
            Assert.AreEqual(result.Title, ("A-1964-10-24-S-0321.pdf"));
            Assert.AreEqual(result.Type, ("ScheduleNote"));
            Assert.AreEqual(result.FreeText, ("Test data content."));
            Assert.AreEqual(result.Url, ("http://s3-eu-west-1.amazonaws.com/chaosdata/Hvideprogrammer/arkiv_A/1964_10_2/PDF/A-1964-10-24-S-0321.pdf"));
            Assert.AreEqual(result.PubStartDate, ("1964-10-24T00:00:00Z"));
            Assert.AreEqual(result.PubStartDate, (Helpers.DateTimeHelper.ParseAndFormatDate("1964-10-24T00:00:00")));
            Assert.AreEqual(result.PubEndDate, (string.Empty));
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

            Assert.AreEqual(result.Any(item => item.Key == "Id" && item.Value == "00000000-0000-0000-0000-000000000001"), true);
            Assert.AreEqual(result.Any(item => item.Key == "Title" && item.Value == "P7 MIX"), true);
            Assert.AreEqual(result.Any(item => item.Key == "Type" && item.Value == "Radio"), true);
            Assert.AreEqual(result.First(item => item.Key == "FreeText").Value, ("test text"));
            Assert.AreEqual(result.First(item => item.Key == "PubStartDate").Value, ("2012-02-21T16:03:00"));
            Assert.AreEqual(result.First(item => item.Key == "PubEndDate").Value, ("2012-02-22T00:03:00"));
        }

        [Test]
        public void DateTimeHelper_ParseAndFormatDate()
        {
            string datestring = "1056-11-13T00:00:00";
            string parseddatestring = Chaos.Portal.Module.Larmfm.Helpers.DateTimeHelper.ParseAndFormatDate(datestring);
            Assert.AreEqual(parseddatestring, ("1956-11-13T00:00:00Z"));

            datestring = "1956-11-13T00:00:00";
            parseddatestring = Chaos.Portal.Module.Larmfm.Helpers.DateTimeHelper.ParseAndFormatDate(datestring);
            Assert.AreEqual(parseddatestring, ("1956-11-13T00:00:00Z"));

            datestring = "1956-13-13T00:00:00";
            parseddatestring = Chaos.Portal.Module.Larmfm.Helpers.DateTimeHelper.ParseAndFormatDate(datestring);
            Assert.AreEqual(parseddatestring, (string.Empty));
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
                                },
                            new Metadata
                                {
                                    MetadataSchemaGuid = Guid.Parse("17d59e41-13fb-469a-a138-bb691f13f2ba"),
                                    MetadataXml = XDocument.Parse(@"<Larm.Metadata><Title></Title><Description>Montage fra åbningen af Christianias urtehospital, hvor man hører behandlere, patienter og gæster. Hunden Sofus piber, da den behandles. Drengen Christian får at vide, at han skal spise hvidløg og propolis for at komme af med sin halsbetændelse. En pige tjekkes for lus. En anden får et råd om blomsten Arnica, som styrker kredsløbet. En medarbejder fortæller om økonomien. Folk opfordres til at lægge en skilling. Medarbejderne betragter urtehospitalet som et lærested, de får indtil videre ikke løn. En jordemoder fortæller om sit arbejde. Pludselig får hun besøg af en mor og en baby. Babyen lå den gale vej, da hun skulle fødes. Så er der en samtale om øret. En medarbejder ser sammen med en rigtig læge på en betændt negl og diskuterer behandling. Nogle indbudte gæster giver deres forskellige meninger om urtehospitalet. En medarbejder fortæller, at de i første omgang ikke vil have nogen patienter indlagt. Stedet er åbent for alle christianitter. Man kan få urtebehandling, zoneterapi, massage, fodbade mm. Nogle medarbejdere er uddannede, andre har bare en masse erfaring. De sender ting, de ikke kan klare, videre til hospitalet. Så er der spillemandsmusik. En medarbejder fortæller, at de gerne vil arbejde for at forebygge sygdomme og hjælpe folk til selvhjælp.                                                                             Peter Kristiansen, tilrettelægger / Christian, patient med halsbetændeler</Description><Genre></Genre><Subjects></Subjects><Tags></Tags><Note></Note><RelatedObjects></RelatedObjects><Contributors /></Larm.Metadata>
                                                                            ")
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
                        Id = 3880365,
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
                        Id = 3880365,
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