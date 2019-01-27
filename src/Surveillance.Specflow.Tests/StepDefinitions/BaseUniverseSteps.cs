using DomainV2.Equity.TimeBars;
using DomainV2.Trading;
using Surveillance.Specflow.Tests.Helpers;
using Surveillance.Universe.Interfaces;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    public class BaseUniverseSteps
    {
        // For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

        private IReadOnlyDictionary<string, IUniverse> _universeLookup;
        protected IUniverse _selectedUniverse;

        private ScenarioContext _scenarioContext;

        public BaseUniverseSteps(ScenarioContext scenarioContext)
        {
            _universeLookup = new Dictionary<string, IUniverse>()
            {
                { "empty",  EmptyUniverse() },
                { "one buy one sell", BuySellAtSamePrice()},
                { "two buy one sell", TwoBuyOneSellAtSamePrice()}
            };

            _scenarioContext = scenarioContext;
        }

        [Given(@"I have the (.*) universe")]
        public void GivenIHaveTheEmptyUniverse(string universe)
        {
            if (string.IsNullOrWhiteSpace(universe))
            {
                _scenarioContext.Pending();
                return;
            }

            if (!_universeLookup.ContainsKey(universe))
            {
                _scenarioContext.Pending();
                return;
            }

            _selectedUniverse = _universeLookup[universe];
        }

        private IUniverse EmptyUniverse()
        {
            var universeEvents =
                new IUniverseEvent[]
                {
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Genesis, DateTime.UtcNow, new object()),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Eschaton, DateTime.UtcNow, new object()),
                };

            return Build(universeEvents);
        }

        private IUniverse BuySellAtSamePrice()
        {
            var orderOne = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            var orderTwo = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderTwo.OrderDirection = DomainV2.Financial.OrderDirections.SELL;

            var universeEvents =
                new IUniverseEvent[]
                {
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Genesis, DateTime.UtcNow, new object()),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderOne),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderTwo),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Eschaton, DateTime.UtcNow, new object()),
                };

            return Build(universeEvents);
        }

        private IUniverse TwoBuyOneSellAtSamePrice()
        {
            var orderOne = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            var orderTwo = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderTwo.OrderDirection = DomainV2.Financial.OrderDirections.SELL;
            var orderThree = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);

            var universeEvents =
                new IUniverseEvent[]
                {
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Genesis, DateTime.UtcNow, new object()),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderOne),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderThree),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderTwo),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Eschaton, DateTime.UtcNow, new object()),
                };

            return Build(universeEvents);
        }

        private Universe.Universe Build(IUniverseEvent[] universeEvents)
        {
            return new Universe.Universe(
                new Order[0],
                new EquityIntraDayTimeBarCollection[0],
                new EquityInterDayTimeBarCollection[0],
                universeEvents);
        }
    }
}
