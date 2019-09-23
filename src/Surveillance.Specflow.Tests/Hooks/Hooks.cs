namespace Surveillance.Specflow.Tests.Hooks
{
    using TechTalk.SpecFlow;

    [Binding]
    public sealed class Hooks
    {
        [AfterScenario]
        public void AfterScenario()
        {
            // TODO: implement logic that has to run after executing each scenario
        }

        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        [BeforeScenario]
        public void BeforeScenario()
        {
            // TODO: implement logic that has to run before executing each scenario
        }
    }
}