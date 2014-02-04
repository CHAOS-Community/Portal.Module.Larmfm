
namespace Chaos.Portal.Module.Larmfm.Test.View
{
    using System;
    using System.Collections.Generic;

    using CHAOS;
    using CHAOS.Extensions;
    using Mcm.Permission;
    using Moq;
    using System.Xml.Linq;
    using Mcm.Data.Dto;
    using System.Linq;
    using Larmfm.View;
    using Object = Mcm.Data.Dto.Object;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    

    [TestClass]
    public class AnnotationViewTest
    {
        Mock<IPermissionManager> PermissionManager = new Mock<IPermissionManager>();

        [TestMethod]
        public void Index_GivenAnnotationObject_ReturnViewDataWithPropertiesSet()
        {
            var view = new AnnotationView(PermissionManager.Object);
            var obj = Make_Annotation_Object();

            var result = (AnnotationViewData)view.Index(obj).First();

            Assert.AreEqual(result.Id, "63a0348b-ab4b-8847-9c71-2d0b4771b0fe");
            Assert.AreEqual(result.StartTime, "00:55:27.0270000");
            Assert.AreEqual(result.Title, "Kashmir");
        }

        [TestMethod]
        public void Index_GivenAnnotationObject_ReturnIndexableFields()
        {
            var view = new AnnotationView(PermissionManager.Object);
            var obj = Make_Annotation_Object();

            var annotationView = (AnnotationViewData)view.Index(obj).First();

            var indexableFields = annotationView.GetIndexableFields();
        }

        [TestMethod]
        public void Should_Convert_Timecode_to_Seconds()
        {
            string time = "00:58:05.5360000";
            double seconds = TimeSpan.Parse(time).TotalSeconds;

            Assert.AreEqual(3485.536, seconds);
        }

        private static Object Make_Annotation_Object()
        {
            return new Object
            {
                Guid = Guid.Parse("63a0348b-ab4b-8847-9c71-2d0b4771b0fe"),
                ObjectTypeID = 64,
                DateCreated = new DateTime(2013, 10, 12, 22, 10, 10),
                Metadatas = new List<Metadata>
                        {
                            new Metadata
                                {
                                    MetadataSchemaGuid  = new UUID("f9f6edd0-f0ca-41ac-b8b3-b0d950fdef4e").ToGuid(),
                                    MetadataXml         = XDocument.Parse(@"<LARM.Annotation.Comment StartTime='00:55:27.0270000' EndTime='00:58:05.5360000'><Title>Kashmir</Title><Description>Min beskrivelse. La la la.</Description></LARM.Annotation.Comment>"),
                                    EditingUserGuid     = new Guid("fe5c76fa-6f74-4604-a2de-ab2bd03a3cba"),
                                    LanguageCode        = "da",
                                    RevisionID          = 1,
                                    DateCreated         = new DateTime(2013, 12, 01, 21, 05, 15)
                                }
                        }
            };
        }
    }
}
