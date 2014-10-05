namespace Chaos.Portal.Module.Larmfm.Extensions
{
  using System;
  using System.Linq;
  using Core;
  using Core.Extension;
  using Mcm.Data;

  public class RadioProgram: AExtension
  {
    public IMcmRepository McmRepository { get; set; }

    public RadioProgram(IPortalApplication portalApplication, IMcmRepository mcmRepository) : base(portalApplication)
    {
      McmRepository = mcmRepository;
    }

    public void Delete(Guid id)
    {
      var obj = McmRepository.ObjectGet(id, false, false, true);

      if (obj.ObjectTypeID != 24) throw new Exception("Given id is not a Radio Program Object");

      foreach (var relation in obj.ObjectRelationInfos.Where(rel => rel.Object2TypeID == 24))
      {
        McmRepository.ObjectDelete(relation.Object2Guid);
        ViewManager.Delete(relation.Object2Guid.ToString());
      }

      McmRepository.ObjectDelete(id);
      ViewManager.Delete(id.ToString());
    }
  }
}
