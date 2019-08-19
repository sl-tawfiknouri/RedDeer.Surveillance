namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces
{
    public interface IHighVolumeRuleCachedMessageSender
    {
        void Delete();

        int Flush();

        void Send(IHighVolumeRuleBreach ruleBreach);
    }
}