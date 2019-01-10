using System.Threading;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Shell.Interfaces
{
    public interface IShellFactset
    {
        Task<bool> HeartBeating(CancellationToken token);
    }
}