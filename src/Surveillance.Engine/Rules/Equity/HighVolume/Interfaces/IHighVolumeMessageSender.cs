using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces
{
    public interface IHighVolumeMessageSender
    {
        Task Send(IHighVolumeRuleBreach ruleBreach);
    }
}