using TestHarness.Factory;

namespace TestHarness.App
{
    public static class Bootstrapper
    {
        public static void Start()
        {
            var appFactory = new AppFactory();
            var mediator = new Mediator(appFactory);

            mediator.Initiate();
        }
    }
}
