using System.Threading.Tasks;

namespace Surveillance.Utility.Interfaces
{
    public interface IApiHeartbeat
    {
        Task<bool> HeartsBeating();
    }
}