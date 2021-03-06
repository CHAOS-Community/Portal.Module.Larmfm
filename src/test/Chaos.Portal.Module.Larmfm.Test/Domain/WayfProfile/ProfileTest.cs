﻿using System.Xml.Linq;
using Chaos.Portal.Module.Larmfm.Domain.WayfProfile;
using NUnit.Framework;

namespace Chaos.Portal.Module.Larmfm.Test.Domain.WayfProfile
{
	[TestFixture]
	public class ProfileTest
	{
		[Test]
		public void Constructor_GivenSeperateValues_ShouldPopulateProperties()
		{
			const string email = "Peter@intitut.dk";
			const string name = "Peter";
			const string organization = "Institut";
			const string title = "Walker";
			const string country = "Denmark";

			var profile = new Profile(email, name, organization, title, country);

			Assert.AreEqual(email, profile.Email);
			Assert.AreEqual(name, profile.Name);
			Assert.AreEqual(organization, profile.Organization);
			Assert.AreEqual(title, profile.Title);
			Assert.AreEqual(country, profile.Country);
		}

		[Test]
		public void Constructor_GivenXml_ShouldPopulateProperties()
		{
			const string email = "Peter@intitut.dk";
			const string name = "Peter";
			const string organization = "Institut";
			const string title = "Walker";
			const string country = "Denmark";
			var xml = XDocument.Parse("<CHAOS.Profile><Name>Peter</Name><Title>Walker</Title><About></About><Organization>Institut</Organization><Emails><Email>Peter@intitut.dk</Email></Emails><Phonenumbers><Phonenumber></Phonenumber></Phonenumbers><Websites><Website></Website></Websites><Skype></Skype><LinkedIn></LinkedIn><Twitter></Twitter><Address></Address><City></City><ZipCode></ZipCode><Country>Denmark</Country></CHAOS.Profile>");

			var profile = new Profile(xml);

			Assert.AreEqual(email, profile.Email);
			Assert.AreEqual(name, profile.Name);
			Assert.AreEqual(organization, profile.Organization);
			Assert.AreEqual(title, profile.Title);
			Assert.AreEqual(country, profile.Country);
		}

		[Test]
		public void Constructor_GivenEmptyXml_ShouldHaveEmptyProperties()
		{
			var xml = XDocument.Parse("<CHAOS.Profile></CHAOS.Profile>");

			var profile = new Profile(xml);

			Assert.IsEmpty(profile.Email);
			Assert.IsEmpty(profile.Name);
			Assert.IsEmpty(profile.Organization);
			Assert.IsEmpty(profile.Title);
			Assert.IsEmpty(profile.Country);
		}

		[Test]
		public void Constructor_ShouldParseOutFromToXml()
		{
			const string email = "Peter@intitut.dk";
			const string name = "Peter";
			const string organization = "Institut";
			const string title = "Walker";
			const string country = "Denmark";

			var profile1 = new Profile(email, name, organization, title, country);
			var profile2 = new Profile(profile1.ToXml());

			Assert.AreEqual(email, profile2.Email);
			Assert.AreEqual(name, profile2.Name);
			Assert.AreEqual(organization, profile2.Organization);
			Assert.AreEqual(title, profile2.Title);
			Assert.AreEqual(country, profile2.Country);
		}

		[Test]
		public void Constructor_ShouldParseOutFromToXmlString()
		{
			const string email = "Peter@intitut.dk";
			const string name = "Peter";
			const string organization = "Institut";
			const string title = "Walker";
			const string country = "Denmark";

			var profile1 = new Profile(email, name, organization, title, country);
			var profile2 = new Profile(XDocument.Parse(profile1.ToXmlString()));

			Assert.AreEqual(email, profile2.Email);
			Assert.AreEqual(name, profile2.Name);
			Assert.AreEqual(organization, profile2.Organization);
			Assert.AreEqual(title, profile2.Title);
			Assert.AreEqual(country, profile2.Country);
		}

		[Test]
		public void Equals_GivenEqualProfiles_ShouldReturnTrue()
		{
			const string email = "Peter@intitut.dk";
			const string name = "Peter";
			const string organization = "Institut";
			const string title = "Walker";
			const string country = "Denmark";

			var profile1 = new Profile(email, name, organization, title, country);
			var profile2 = new Profile(email, name, organization, title, country);

			Assert.IsTrue(profile1.Equals(profile2));
			Assert.IsTrue(profile1.Equals(profile1));
		}

		[Test]
		public void Equals_GivenNotEqualProfiles_ShouldReturnFalse()
		{
			const string email = "Peter@intitut.dk";
			const string name = "Peter";
			const string name2 = "Hans";
			const string organization = "Institut";
			const string title = "Walker";
			const string country = "Denmark";

			var profile1 = new Profile(email, name, organization, title, country);
			var profile2 = new Profile(email, name2, organization, title, country);

			Assert.IsFalse(profile1.Equals(profile2));
		}

		[Test]
		public void ToXmlString_ShouldCorrectXml()
		{
			const string email = "Peter@intitut.dk";
			const string name = "Peter";
			const string organization = "Institut";
			const string title = "Walker";
			const string country = "Denmark";

			var profile = new Profile(email, name, organization, title, country);
			var xmlString = profile.ToXmlString();

			Assert.That(xmlString, Is.EqualTo("<CHAOS.Profile><Name>Peter</Name><Title>Walker</Title><About></About><Organization>Institut</Organization><Emails><Email>Peter@intitut.dk</Email></Emails><Phonenumbers><Phonenumber></Phonenumber></Phonenumbers><Websites><Website></Website></Websites><Skype></Skype><LinkedIn></LinkedIn><Twitter></Twitter><Address></Address><City></City><ZipCode></ZipCode><Country>Denmark</Country></CHAOS.Profile>"));
		}

		[Test]
		public void FillDataFrom_EmptyProfileGivenFullProfile_ShouldOverwriteEverything()
		{
			const string email = "Peter@intitut.dk";
			const string name = "Peter";
			const string organization = "Institut";
			const string title = "Walker";
			const string country = "Denmark";

			var existingProfile = new Profile("", "", null, null, null);
			var fillingProfile = new Profile(email, name, organization, title, country);

			existingProfile.FillDataFrom(fillingProfile);

			Assert.AreEqual(email, existingProfile.Email);
			Assert.AreEqual(name, existingProfile.Name);
			Assert.AreEqual(organization, existingProfile.Organization);
			Assert.AreEqual(title, existingProfile.Title);
			Assert.AreEqual(country, existingProfile.Country);
		}

		[Test]
		public void FillDataFrom_PartiallyEmptyProfileGivenFullProfile_ShouldOverwriteEmptyProperties()
		{
			const string existingEmail = "Peter@intitut.dk";
			const string existingName = "Hans";
			const string existingOrganization = "Institut";
			const string existingTitle = "Walker";
			const string existingCountry = "Denmark";

			const string email = "Petesdfsdfr@intitut.dk";
			const string title = "Weep";

			var existingProfile = new Profile(existingEmail, existingName, existingOrganization, existingTitle, existingCountry);
			var fillingProfile = new Profile(email, null, "", title, "  ");

			existingProfile.FillDataFrom(fillingProfile);

			Assert.AreEqual(email, existingProfile.Email);
			Assert.AreEqual(existingName, existingProfile.Name);
			Assert.AreEqual(existingOrganization, existingProfile.Organization);
			Assert.AreEqual(title, existingProfile.Title);
			Assert.AreEqual(existingCountry, existingProfile.Country);
		}

		[Test]
		public void FillDataFrom_HasEmptyEmail_ShouldNotFillInEmail()
		{
			const string existingEmail = "Peter@intitut.dk";
			const string existingName = "Hans";
			const string existingOrganization = "Institut";
			const string existingTitle = "Walker";
			const string existingCountry = "Denmark";

			const string email = "";
			const string name = "Peter";
			const string organization = "Skolen";
			const string title = "Runner";
			const string country = "Sverige";

			var existingProfile = new Profile(existingEmail, existingName, existingOrganization, existingTitle, existingCountry);
			var fillingProfile = new Profile(email, name, organization, title, country);

			existingProfile.FillDataFrom(fillingProfile);

			Assert.AreEqual(existingEmail, existingProfile.Email);
			Assert.AreEqual(name, existingProfile.Name);
			Assert.AreEqual(organization, existingProfile.Organization);
			Assert.AreEqual(title, existingProfile.Title);
			Assert.AreEqual(country, existingProfile.Country);
		}
	}
}