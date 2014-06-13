namespace Chaos.Portal.Module.Larmfm.Domain
{
    public interface ITranscoder
    {
        void Transcode(string inputKey, string outputKey);
    }
}