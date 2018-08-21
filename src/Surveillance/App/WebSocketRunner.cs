using System;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.App
{
    public class WebSocketRunner : IStartUpTaskRunner
    {
        public WebSocketRunner()
        {
        }

        public async Task Run()
        {
            await Task.Run(() => 
            {
            });
        }
    }
}
