using System.Threading;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Shell.Interfaces
{
    public interface IShellBmll
    {
        Task<bool> HeartBeating(CancellationToken token);
    }
}