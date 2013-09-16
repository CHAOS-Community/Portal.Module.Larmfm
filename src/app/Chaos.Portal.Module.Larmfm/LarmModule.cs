namespace Chaos.Portal.Module.Larmfm
{
    public class LarmModule : Mcm.McmModule
    {
        #region Implementation of IModule

        protected override Core.Indexing.View.IView CreateObjectView()
        {
            return new View.ObjectView(PermissionManager);
        }

        #endregion
    }
}