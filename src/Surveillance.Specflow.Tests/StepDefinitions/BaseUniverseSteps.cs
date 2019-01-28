using DomainV2.Equity.TimeBars;
using DomainV2.Trading;
using Surveillance.Specflow.Tests.Helpers;
using Surveillance.Universe.Interfaces;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public sealed class UniverseSteps
    {
        // For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

        private IReadOnlyDictionary<string, IUniverse> _universeLookup;
        private UniverseSelectionState _universeSelectionState;

        private ScenarioContext _scenarioContext;

        public UniverseSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            _universeLookup = new Dictionary<string, IUniverse>()
            {
                { "empty",  EmptyUniverse() },
                { "buy sell", BuySellAtSamePrice()},
                { "buy buy sell", TwoBuyOneSellAtSamePrice()},
                { "buy sell at p1 buy sell at p2", BuySellAtOnePriceBuySellAtAnotherPrice()},
                { "buy sell at p1 buy sell at p2 buy at p3", BuySellAtOnePriceBuySellAtAnotherPriceBuyAtAnotherPrice()}
            };

            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;
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

            _universeSelectionState.SelectedUniverse = _universeLookup[universe];
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

        private IUniverse BuySellAtOnePriceBuySellAtAnotherPrice()
        {
            var orderOne = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            var orderTwo = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderTwo.OrderDirection = DomainV2.Financial.OrderDirections.SELL;

            var orderThree = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderThree.OrderAverageFillPrice = 
                new DomainV2.Financial.CurrencyAmount(
                    orderThree.OrderAverageFillPrice.Value.Value * 1.2m,
                    orderThree.OrderAverageFillPrice.Value.Currency);

            var orderFour = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderFour.OrderAverageFillPrice =
                new DomainV2.Financial.CurrencyAmount(
                    orderFour.OrderAverageFillPrice.Value.Value * 1.2m,
                    orderFour.OrderAverageFillPrice.Value.Currency);
            orderFour.OrderDirection = DomainV2.Financial.OrderDirections.SELL;

            var universeEvents =
                new IUniverseEvent[]
                {
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Genesis, DateTime.UtcNow, new object()),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderOne),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderTwo),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderThree),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderFour),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Eschaton, DateTime.UtcNow, new object()),
                };

            return Build(universeEvents);
        }

        private IUniverse BuySellAtOnePriceBuySellAtAnotherPriceBuyAtAnotherPrice()
        {
            var orderOne = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            var orderTwo = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderTwo.OrderDirection = DomainV2.Financial.OrderDirections.SELL;

            var orderThree = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderThree.OrderAverageFillPrice =
                new DomainV2.Financial.CurrencyAmount(
                    orderThree.OrderAverageFillPrice.Value.Value * 1.2m,
                    orderThree.OrderAverageFillPrice.Value.Currency);

            var orderFour = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderFour.OrderAverageFillPrice =
                new DomainV2.Financial.CurrencyAmount(
                    orderFour.OrderAverageFillPrice.Value.Value * 1.2m,
                    orderFour.OrderAverageFillPrice.Value.Currency);
            orderFour.OrderDirection = DomainV2.Financial.OrderDirections.SELL;

            var orderFive = OrderHelper.Orders(DomainV2.Financial.OrderStatus.Filled);
            orderFive.OrderAverageFillPrice =
                new DomainV2.Financial.CurrencyAmount(
                    orderFive.OrderAverageFillPrice.Value.Value * 1.4m,
                    orderFive.OrderAverageFillPrice.Value.Currency);

            var universeEvents =
                new IUniverseEvent[]
                {
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Genesis, DateTime.UtcNow, new object()),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderOne),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderTwo),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderThree),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderFour),
                    new Universe.UniverseEvent(Universe.UniverseStateEvent.Order, DateTime.UtcNow, orderFive),
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
