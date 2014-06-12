namespace Chaos.Portal.Module.Larmfm.Domain
{
    using System.IO;

    public interface IStorage
    {
        void Write(string key, Stream stream);
    }
}