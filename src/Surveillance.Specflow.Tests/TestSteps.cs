using System;
using TechTalk.SpecFlow;

namespace Surveillance.Specflow.Tests
{
    [Binding]
    public class TestSteps
    {
       
        [When(@"I press add")]
        public void WhenIPressAdd()
        {

        }
        
        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int p0)
        {

        }

        [Given(@"I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int p0)
        {

        }

    }
}
