using System.Linq;
using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.App.Types.Organisation;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderGraphType : ObjectGraphType<IOrder>
    {
        public OrderGraphType(
            IFinancialInstrumentRepository instrumentsRepository,
            IMarketRepository marketRepository,
            IOrderRepository orderRepository,
            IDataLoaderContextAccessor dataLoader)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Field(i => i.Id).Description("Primary key");

            Field<MarketGraphType>(
                "market",
                description: "Market associated with the order",
                resolve: context =>
                {
                    var loader = dataLoader.Context.GetOrAddLoader(
                        $"GetMarketById-{context.Source.MarketId}",
                        async () => await marketRepository.GetById(context.Source.MarketId));

                    return loader.LoadAsync();
                });

            Field(i => i.SecurityId, nullable: true).Description("Security Id");

            Field<FinancialInstrumentGraphType>(
                "financialInstrument",
                description: "Instrument subject to trading in the order",
                resolve: context =>
                {
                    var loader = dataLoader.Context.GetOrAddLoader(
                        $"GetFinancialInstrumentById-{context.Source.SecurityId}",
                        async () => context.Source.SecurityId.HasValue ? await instrumentsRepository.GetById(context.Source.SecurityId.Value) : null);

                    return loader.LoadAsync();
                });

            Field(i => i.ClientOrderId).Description("Client Order Id");
            Field(i => i.OrderVersion).Description("Order version");
            Field(i => i.OrderVersionLinkId).Description("Order version link id");
            Field(i => i.OrderGroupId).Description("Order group id");
            Field(i => i.OrderType).Description("Order type");
            Field(i => i.Direction).Description("Order direction");

            Field<OrderDatesGraphType>().Name("orderDates").Description("Dates of key events in the order life cycle");
            Field<OrderManagementSystemGraphType>().Name("oms").Description("Order Management System data");
            Field<OrderTypeGraphType>().Name("orderTypes").Description("Order type delivered to market i.e. market/limit etc");
            Field<OrderDirectionGraphType>().Name("orderDirection").Description("Order direction such as buy/sell short/cover");

            Field(i => i.Currency).Description("Order currency values are denominated in");
            Field(i => i.SettlementCurrency).Description("Order settlement currency");
            Field(i => i.CleanDirty).Description("Order values quoted clean or dirty (fixed income with or without accrued interest)");
            Field(i => i.AccumulatedInterest, nullable: true).Description("Accumulated interest");
            Field(i => i.LimitPrice, nullable: true).Description("Order limit price (if applicable)");
            Field(i => i.AverageFillPrice, nullable: true).Description("Order average fill price");

            Field(i => i.OrderedVolume, nullable: true).Description("Order volume ordered");
            Field(i => i.FilledVolume, nullable: true).Description("Order actual filled volume can be larger or smaller than ordered volume");

            Field<TraderGraphType>().Name("trader").Description("Trader handling the order salient properties");

            Field(i => i.ClearingAgent).Description("Clearing agent used for the trade");
            Field(i => i.DealingInstructions).Description("Instructions for dealer");

            Field(i => i.OptionStrikePrice, nullable: true).Description("The strike price of the option instrument");
            Field(i => i.OptionExpiration).Description("The expiration date of the option instrument");
            Field(i => i.OptionEuropeanAmerican).Description("The category of the option. European or American");

            Field(i => i.Created).Description("The date the system created the order on");
            Field(i => i.LifeCycleStatus, nullable: true).Description("The order status within the life cycle");

            Field(i => i.Live).Description("Order is live, therefore has corresponding order allocations");
            Field(i => i.Autoscheduled).Description("Order has been autoscheduled");

            Field<ListGraphType<FundGraphType>>(
                "fund",
                description: "The fund the order was allocated to",
                resolve: context =>
                {
                    IQueryable<IOrdersAllocation> FundNameQuery(IQueryable<IOrdersAllocation> i) => i.Where(x => x.Fund == context.Source.Fund);

                    var loader = dataLoader.Context.GetOrAddLoader(
                        $"GetFundById-{context.Source.Fund}",
                        () => orderRepository.QueryFund(FundNameQuery));

                    return loader.LoadAsync();
                });

            Field<ListGraphType<StrategyGraphType>>(
                "strategy",
                description: "The strategy the order was allocated to",
                resolve: context =>
                {
                    IQueryable<IOrdersAllocation> StrategyNameQuery(IQueryable<IOrdersAllocation> i) => i.Where(x => x.Strategy == context.Source.Strategy);

                    var loader = dataLoader.Context.GetOrAddLoader(
                        $"GetStrategyById-{context.Source.Strategy}",
                        () => orderRepository.QueryStrategy(StrategyNameQuery));

                    return loader.LoadAsync();
                });

            Field<ListGraphType<ClientAccountGraphType>>(
                "clientAccount",
                description: "The client account the order was allocated to",
                resolve: context =>
                {
                    IQueryable<IOrdersAllocation> ClientAccountQuery(IQueryable<IOrdersAllocation> i) => i.Where(x => x.ClientAccountId == context.Source.ClientAccount);

                    var loader = dataLoader.Context.GetOrAddLoader(
                        $"GetClientAccountById-{context.Source.ClientAccount}",
                        () => orderRepository.QueryClientAccount(ClientAccountQuery));

                    return loader.LoadAsync();
                });

            Field<ListGraphType<OrderAllocationGraphType>>(
                "orderAllocations",
                resolve: context =>
                {
                    IQueryable<IOrdersAllocation> OrderAllocationsQuery(IQueryable<IOrdersAllocation> i) => i.Where(x => x.OrderId == context.Source.ClientOrderId);

                    var loader = dataLoader.Context.GetOrAddLoader(
                        $"GetOrderAllocationsByClientOrderId-{context.Source.ClientOrderId}",
                        () => orderRepository.QueryAllocations(OrderAllocationsQuery));

                    return loader.LoadAsync();
                });
        }
    }
}
