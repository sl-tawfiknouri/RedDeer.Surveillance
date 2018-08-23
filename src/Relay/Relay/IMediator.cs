using System.Threading.Tasks;

namespace Relay
{
    public interface IMediator
    {
        Task Initiate();
    }
}