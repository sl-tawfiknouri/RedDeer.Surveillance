using System.Threading;
using NLog;
using TestHarness.App;

namespace App
{
    public class Program
    {
        static void Main(string[] args)
        {
            LogManager.LoadConfiguration("nlog.config");

            Bootstrapper.Start();

            Thread.Sleep(50);
        }
    }
}