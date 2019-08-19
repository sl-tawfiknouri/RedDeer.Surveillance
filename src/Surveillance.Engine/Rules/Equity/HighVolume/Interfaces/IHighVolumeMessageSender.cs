namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces
{
    using System.Threading.Tasks;

    public interface IHighVolumeMessageSender
    {
        Task Send(IHighVolumeRuleBreach ruleBreach);
    }
}