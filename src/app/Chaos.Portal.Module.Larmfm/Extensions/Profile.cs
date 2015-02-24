using System;
using System.Linq;
using Chaos.Mcm.Data;
using Chaos.Portal.Core;
using Chaos.Portal.Core.Exceptions;
using Chaos.Portal.Core.Extension;
using Chaos.Portal.Module.Larmfm.Api;
using Object = Chaos.Mcm.Data.Dto.Object;

namespace Chaos.Portal.Module.Larmfm.Extensions
{
  public class Profile : AExtension
  {
    public IMcmRepository Repository { get; set; }
    public LarmSettings Settings { get; set; }

    public Profile(IPortalApplication portalApplication, IMcmRepository repository, LarmSettings settings) : base(portalApplication)
    {
      Repository = repository;
      Settings = settings;
    }

    public ProfileResult Get()
    {
      if(Request.IsAnonymousUser) throw new InsufficientPermissionsException("User is not logged in");

      var userId = Request.User.Guid;

      var userObject = Repository.ObjectGet(userId, true);

      return Map(userObject);
    }

    private ProfileResult Map(Object user)
    {
      if (user == null) return ProfileResult.CreateNullObject();

      var metadata = user.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == Settings.UserProfileMetadataSchemaGuid);

      if (metadata == null) return ProfileResult.CreateNullObject();

      var root = metadata.MetadataXml.Root;
      var result = new ProfileResult();
      result.Name = root.Element("Name").Value;
      result.Title = root.Element("Title").Value;
      result.About = root.Element("About").Value;
      result.Organization = root.Element("Organization").Value;
      
      foreach (var email in root.Descendants("Email"))
        result.Emails.Add(email.Value);

      foreach (var phonenumber in root.Descendants("Phonenumber"))
        result.PhoneNumbers.Add(phonenumber.Value);

      foreach (var website in root.Descendants("Website"))
        result.Websites.Add(website.Value);

      result.Skype = root.Element("Skype").Value;
      result.LinkedIn = root.Element("LinkedIn").Value;
      result.Twitter = root.Element("Twitter").Value;
      result.Address = root.Element("Address").Value;
      result.City = root.Element("City").Value;
      result.ZipCode = root.Element("Zipcode").Value;
      result.Country = root.Element("Country").Value;

      return result;
    }
  }
}