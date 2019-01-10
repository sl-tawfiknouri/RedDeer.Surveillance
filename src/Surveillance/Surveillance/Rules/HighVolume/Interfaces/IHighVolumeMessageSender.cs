using System.Threading.Tasks;

namespace Surveillance.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeMessageSender
    {
        Task Send(IHighVolumeRuleBreach ruleBreach);
    }
}