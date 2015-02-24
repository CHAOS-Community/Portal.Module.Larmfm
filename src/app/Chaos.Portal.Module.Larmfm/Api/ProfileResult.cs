using System.Collections.Generic;
using CHAOS.Serialization;
using Chaos.Portal.Core.Data.Model;

namespace Chaos.Portal.Module.Larmfm.Api
{
  public class ProfileResult : AResult
  {
    public ProfileResult()
    {
      Emails = new List<string>();
      PhoneNumbers = new List<string>();
      Websites = new List<string>();
    }

    [Serialize]
    public string Name { get; set; }

    [Serialize]
    public string Title { get; set; }

    [Serialize]
    public string About { get; set; }

    [Serialize]
    public string Organization { get; set; }

    [Serialize]
    public IList<string> Emails { get; set; }

    [Serialize]
    public IList<string> PhoneNumbers { get; set; }
    
    [Serialize]
    public IList<string> Websites { get; set; }

    [Serialize]
    public string Skype { get; set; }

    [Serialize]
    public string LinkedIn { get; set; }

    [Serialize]
    public string Twitter { get; set; }

    [Serialize]
    public string Address { get; set; }

    [Serialize]
    public string City { get; set; }

    [Serialize]
    public string ZipCode { get; set; }

    [Serialize]
    public string Country { get; set; }

    public static ProfileResult CreateNullObject()
    {
      return new ProfileResult
        {
          Name = "",
          Title = "",
          About = "",
          Organization = "",
          Skype = "",
          LinkedIn = "",
          Twitter = "",
          Address = "",
          City = "",
          ZipCode = "",
          Country = "",
        };
    }
  }
}