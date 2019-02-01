using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Trading;
using Surveillance.Specflow.Tests.StepDefinitions.Orders;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public sealed class UniverseSteps
    {
        private UniverseSelectionState _universeSelectionState;
        private SecuritySelection _securitySelection;
        private ScenarioContext _scenarioContext;

        public UniverseSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState,
            SecuritySelection securitySelection)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;
            _securitySelection = securitySelection;
        }

        [Given(@"I have the orders for a universe from (.*) to (.*) :")]
        public void GivenIHaveTheOrders(string from, string to, Table orderTable)
        {
            var eventList = new List<IUniverseEvent>();

            var fromDate = DateTime.Parse(from);
            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, fromDate, new object());

            var toDate = DateTime.Parse(to).AddDays(1).AddMilliseconds(-1);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, toDate, new object());

            eventList.Add(genesis);

            var orderParams = orderTable.CreateSet<OrderParameters>();

            foreach (var row in orderParams)
            {
                var orderEvent = MapRowToOrderEvent(row);

                if (orderEvent == null)
                    continue;

                eventList.Add(orderEvent);
            }

            eventList.Add(eschaton);
            eventList = eventList.OrderBy(i => i.EventTime).ToList();

            var universe = Build(eventList);

            _universeSelectionState.SelectedUniverse = universe;
        }

        private IUniverseEvent MapRowToOrderEvent(OrderParameters orderParam)
        {
            if (orderParam == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(orderParam.SecurityName)
                || !_securitySelection.Securities.ContainsKey(orderParam.SecurityName))
            {
                _scenarioContext.Pending();
                return null;
            }

            var security = _securitySelection.Securities[orderParam.SecurityName];

            var orderLimitPrice =
                orderParam.LimitPrice != null
                ? new CurrencyAmount(orderParam.LimitPrice, orderParam.Currency) 
                : (CurrencyAmount?)null;

            var orderAveragePrice =
                orderParam.AverageFillPrice != null
                ? new CurrencyAmount(orderParam.AverageFillPrice, orderParam.Currency)
                : (CurrencyAmount?)null;

            var order = new Order(
                security.Instrument,
                security.Market,
                null,
                orderParam.OrderId,
                orderParam.PlacedDate,
                null,
                null,
                null,
                orderParam.PlacedDate,
                orderParam.BookedDate,
                orderParam.AmendedDate,
                orderParam.RejectedDate,
                orderParam.CancelledDate,
                orderParam.FilledDate,
                orderParam.Type,
                orderParam.Direction,
                new DomainV2.Financial.Currency(orderParam.Currency),
                null,
                OrderCleanDirty.NONE,
                null,
                orderLimitPrice,
                orderAveragePrice,
                orderParam.OrderedVolume,
                orderParam.FilledVolume,
                null,
                null,
                null,
                null,
                null,
                null,
                OptionEuropeanAmerican.NONE,
                null);

            var universeEvent = new UniverseEvent(UniverseStateEvent.Order, order.MostRecentDateEvent(), order);

            return universeEvent;
        }

        private Universe.Universe Build(IReadOnlyCollection<IUniverseEvent> universeEvents)
        {
            return new Universe.Universe(
                new Order[0],
                new EquityIntraDayTimeBarCollection[0],
                new EquityInterDayTimeBarCollection[0],
                universeEvents);
        }
    }
}
