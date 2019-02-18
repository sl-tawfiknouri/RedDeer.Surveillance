using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeMessageSender
    {
        Task Send(IHighVolumeRuleBreach ruleBreach);
    }
}