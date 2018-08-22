using System.Threading;
using TestHarness;
using NLog;

namespace App
{
    public class Program
    {
        static void Main(string[] args)
        {
            LogManager.LoadConfiguration("nlog.config");

            var mediator = new Mediator(null);
            mediator.Initiate(null);

            Thread.Sleep(1000);
        }
    }
}