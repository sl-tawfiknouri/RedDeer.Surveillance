using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    public class BaseUniverseSteps
    {
        // For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

        private IReadOnlyDictionary<string, object> _universeLookup;

        public BaseUniverseSteps()
        {
            _universeLookup = new Dictionary<string, object>()
            {
                { "empty", new object() }
            };
        }

        [Given(@"I have the (.*) universe")]
        public void GivenIHaveTheEmptyUniverse(string universe)
        {
            // oh nice, should we give them free form text names or numbers?
            // names! =)

            // don't do anything for now! (- -)

            var x = universe;

        }
    }
}
