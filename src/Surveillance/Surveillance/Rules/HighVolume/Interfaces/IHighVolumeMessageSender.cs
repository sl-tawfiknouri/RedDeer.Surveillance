namespace Surveillance.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeMessageSender
    {
        void Send(IHighVolumeRuleBreach ruleBreach);
    }
}