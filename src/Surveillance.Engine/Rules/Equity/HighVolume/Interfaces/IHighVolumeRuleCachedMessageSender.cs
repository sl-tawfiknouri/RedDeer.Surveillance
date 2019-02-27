namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces
{
    public interface IHighVolumeRuleCachedMessageSender
    {
        void Send(IHighVolumeRuleBreach ruleBreach);
        int Flush();
        void Delete();
    }
}