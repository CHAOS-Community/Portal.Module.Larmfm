using System;
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
			if(!data.Root.HasElements) return;

			try
			{
				Name = data.Root.Element("Name").Value;
				Title = data.Root.Element("Title").Value;
				Organization = data.Root.Element("Organization").Value;
				Country = data.Root.Element("Country").Value;

				var emailElement = data.Root.Element("Emails");
				Email = emailElement != null && emailElement.HasElements ? emailElement.Element("Email").Value : "";
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

		public void FillEmptyDataFrom(Profile profile)
		{
			if (string.IsNullOrWhiteSpace(Email)) Email = profile.Email;
			if (string.IsNullOrWhiteSpace(Name)) Name = profile.Name;
			if (string.IsNullOrWhiteSpace(Organization)) Organization = profile.Organization;
			if (string.IsNullOrWhiteSpace(Title)) Title = profile.Title;
			if (string.IsNullOrWhiteSpace(Country)) Country = profile.Country;
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