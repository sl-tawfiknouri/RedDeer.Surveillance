using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Utility.Interfaces
{
    public interface IApiHeartbeat
    {
        Task<bool> HeartsBeating();
    }
}