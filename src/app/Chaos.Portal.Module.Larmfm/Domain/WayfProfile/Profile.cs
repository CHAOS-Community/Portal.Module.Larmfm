using System;
using System.Linq;
using System.Xml.Linq;

namespace Chaos.Portal.Module.Larmfm.Domain.WayfProfile
{
	public class Profile
	{
		public string Email { get; set; }
		public string Name { get; set; }
		public string Organization { get; set; }
		public string Title { get; set; }
		public string Country { get; set; }

		public Profile(string email, string name, string organization, string title, string country)
		{
			Email = email;
			Name = name;
			Organization = organization;
			Title = title;
			Country = country;
		}

		public Profile(XDocument data)
		{
			if(data.Root == null) throw new Exception("Root is null");

			try
			{
			    var name = data.Root.Element("Name");
				Name = name != null ? name.Value : "";

				var title = data.Root.Element("Title");
				Title = title != null ? title.Value : "";

				var organization = data.Root.Element("Organization");
                Organization = organization != null ? organization.Value : "";

				var country = data.Root.Element("Country");
                Country = country != null ? country.Value : "";

				var emails = data.Root.Element("Emails");
				if (emails != null && emails.HasElements) Email = emails.Elements().First().Value;
				if (Email == null) Email = "";
			}
			catch (Exception error)
			{
				
				throw new Exception("Failed to parse profile xml", error);
			}
		}

		public string ToXmlString()
		{
			return string.Format("<CHAOS.Profile><Name>{0}</Name><Title>{1}</Title><About></About><Organization>{2}</Organization><Emails><Email>{3}</Email></Emails><Phonenumbers><Phonenumber></Phonenumber></Phonenumbers><Websites><Website></Website></Websites><Skype></Skype><LinkedIn></LinkedIn><Twitter></Twitter><Address></Address><City></City><ZipCode></ZipCode><Country>{4}</Country></CHAOS.Profile>",
				Name, Title, Organization, Email, Country);
		}

		public XDocument ToXml()
		{
			return XDocument.Parse(ToXmlString());
		}

		public void FillDataFrom(Profile profile)
		{
			if (!string.IsNullOrWhiteSpace(profile.Email)) Email = profile.Email;
			if (!string.IsNullOrWhiteSpace(profile.Name)) Name = profile.Name;
			if (!string.IsNullOrWhiteSpace(profile.Organization)) Organization = profile.Organization;
			if (!string.IsNullOrWhiteSpace(profile.Title)) Title = profile.Title;
			if (!string.IsNullOrWhiteSpace(profile.Country)) Country = profile.Country;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Profile);
		}

		public bool Equals(Profile otherProfile)
		{
			return otherProfile != null &&
					Email == otherProfile.Email &&
					Name == otherProfile.Name &&
					Organization == otherProfile.Organization &&
					Title == otherProfile.Title &&
					Country == otherProfile.Country;
		}
	}
}