namespace Chaos.Portal.Module.Larmfm
{
    using Core.Module;
    using Mcm.Data;

    public interface ILarmModule : IModuleConfig
	{
		LarmConfiguration Configuration { get; set; }
	}
}