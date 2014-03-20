using Chaos.Mcm;
using Chaos.Mcm.Data;

namespace Chaos.Portal.Module.Larmfm
{
	public interface ILarmModule : IMcmModule
	{
		LarmConfiguration Configuration { get; set; }
		IMcmRepository McmRepository { get; }
	}
}