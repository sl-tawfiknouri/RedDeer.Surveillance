namespace Surveillance.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeRuleCachedMessageSender
    {
        void Send(IHighVolumeRuleBreach ruleBreach);
        int Flush();
        void Delete();
    }
}