using TestHarness.Configuration.Interfaces;
using TestHarness.Factory;

namespace TestHarness.App
{
    public static class Bootstrapper
    {
        public static void Start(INetworkConfiguration config)
        {
            var appFactory = new AppFactory(config);
            var mediator = new Mediator(appFactory);

            mediator.Initiate();
        }
    }
}
