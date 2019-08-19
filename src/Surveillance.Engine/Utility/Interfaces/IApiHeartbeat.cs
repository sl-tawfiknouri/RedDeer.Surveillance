namespace Surveillance.Engine.Rules.Utility.Interfaces
{
    using System.Threading.Tasks;

    public interface IApiHeartbeat
    {
        Task<bool> HeartsBeating();
    }
}