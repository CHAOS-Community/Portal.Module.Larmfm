namespace Chaos.Portal.Module.Larmfm.Extensions
{
  using System;
  using Core;
  using Core.Extension;
  using Mcm.Data;

  public class Annotation : AExtension
  {
    public IMcmRepository McmRepository { get; set; }

    public Annotation(IPortalApplication portalApplication, IMcmRepository mcmRepository) : base(portalApplication)
    {
      McmRepository = mcmRepository;
    }

    public void Delete(Guid id)
    {
      var obj = McmRepository.ObjectGet(id);

      if (obj.ObjectTypeID != 64) throw new Exception("Given id is not an Annotation");

      McmRepository.ObjectDelete(id);
      ViewManager.Delete(id.ToString());
    }
  }
}
