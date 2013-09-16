﻿namespace Chaos.Portal.Module.Larmfm.Test.View
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Object = Chaos.Mcm.Data.Dto.Object;

    [TestFixture]
    public class SearchViewTest
    {
        [Test]
        public void Index_GivenProgramObject_ReturnViewDataWithTitle()
        {
            var view = new SearchView();
            var obj  = Make_Object();

            var result = (SearchViewData) view.Index(obj).First();

            Assert.That(result.Id, Is.EqualTo("00000000-0000-0000-0000-000000000001"));
            Assert.That(result.Title, Is.EqualTo("P7 MIX"));
        }

        [Test]
        public void GetIndexableFields_GivenSearchViewData_ReturnTitleField()
        {
            var data = new SearchViewData
                {
                    Id = "00000000-0000-0000-0000-000000000001",
                    Title = "P7 MIX"
                };

            var result = data.GetIndexableFields().ToList();

            Assert.That(result.Any(item => item.Key == "Id" && item.Value == "00000000-0000-0000-0000-000000000001"),
                        Is.True);
            Assert.That(result.Any(item => item.Key == "Title" && item.Value == "P7 MIX"), Is.True);
        }

        private static Object Make_Object()
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
                                    MetadataXml =
                                        XDocument.Parse(
                                            "<Larm.Program><PublicationDateTime>2012-02-21T16:03:00</PublicationDateTime><PublicationEndDateTime>2012-02-22T00:03:00</PublicationEndDateTime><PublicationChannel>DR P7 Mix</PublicationChannel><Title>P7 MIX</Title><Abstract></Abstract><Description>Musik med pop og sjæl. </Description><Publisher></Publisher><Subjects /><Contributors></Contributors><Creators><Creator><Name></Name><RoleName></RoleName><RoleID></RoleID></Creator></Creators><Locations /><Identifiers><DR.ProductionNumber></DR.ProductionNumber><DR.ArchiveNumber></DR.ArchiveNumber><SB.DomsID>c431cb1d-081a-47de-a7d4-cd4275a7063a</SB.DomsID></Identifiers></Larm.Program>")
                                }
                        }
                };
        }
    }
}