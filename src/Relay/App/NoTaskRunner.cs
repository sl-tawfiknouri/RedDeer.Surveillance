using System.Threading.Tasks;

namespace RedDeer.Relay.App
{
    public class NoTaskRunner : IStartUpTaskRunner
    {
        public async Task Run()
        {
            await Task.Run(() => { });
        }
    }
}
