using Surveillance.Rules.HighVolume.Interfaces;

namespace Surveillance.Rules.HighVolume
{
    public interface IHighVolumeRuleCachedMessageSender : IHighVolumeMessageSender
    {
        int Flush();
        void Send(IHighVolumeRuleBreach ruleBreach);
    }
}