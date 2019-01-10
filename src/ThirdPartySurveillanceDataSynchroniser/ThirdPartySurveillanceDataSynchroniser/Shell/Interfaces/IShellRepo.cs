using System.Threading;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Shell.Interfaces
{
    public interface IShellRepo
    {
        Task<bool> CanHitDb(CancellationTokenSource cts);
    }
}