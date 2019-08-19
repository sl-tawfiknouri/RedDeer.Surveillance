namespace Surveillance.Api.App.Types.Trading
{
    using System.Linq;

    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.App.Types.Organisation;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class OrderGraphType : ObjectGraphType<IOrder>
    {
        public OrderGraphType(
            IFinancialInstrumentRepository instrumentsRepository,
            IMarketRepository marketRepository,
            IBrokerRepository brokerRepository,
            IOrderRepository orderRepository,
            IDataLoaderContextAccessor dataLoader)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(i => i.Id).Description("Primary key");

            this.Field<MarketGraphType>(
                "market",
                "Market associated with the order",
                resolve: context =>
                    {
                        var loader = dataLoader.Context.GetOrAddLoader(
                            $"GetMarketById-{context.Source.MarketId}",
                            async () => await marketRepository.GetById(context.Source.MarketId));

                        return loader.LoadAsync();
                    });

            this.Field<BrokerGraphType>(
                "broker",
                "Broker",
                resolve: context =>
                    {
                        var loader = dataLoader.Context.GetOrAddLoader(
                            $"GetBrokerById-{context.Source.BrokerId}",
                            async () => await brokerRepository.GetById(context.Source.BrokerId));

                        return loader.LoadAsync();
                    });

            this.Field(i => i.SecurityId, true).Description("Security Id");

            this.Field<FinancialInstrumentGraphType>(
                "financialInstrument",
                "Instrument subject to trading in the order",
                resolve: context =>
                    {
                        var loader = dataLoader.Context.GetOrAddLoader(
                            $"GetFinancialInstrumentById-{context.Source.SecurityId}",
                            async () => context.Source.SecurityId.HasValue
                                            ? await instrumentsRepository.GetById(context.Source.SecurityId.Value)
                                            : null);

                        return loader.LoadAsync();
                    });

            this.Field(i => i.ClientOrderId).Description("Client Order Id");
            this.Field(i => i.OrderVersion).Description("Order version");
            this.Field(i => i.OrderVersionLinkId).Description("Order version link id");
            this.Field(i => i.OrderGroupId).Description("Order group id");
            this.Field(i => i.OrderType).Description("Order type");
            this.Field(i => i.Direction).Description("Order direction");

            this.Field<OrderDatesGraphType>().Name("orderDates")
                .Description("Dates of key events in the order life cycle");
            this.Field<OrderManagementSystemGraphType>().Name("oms").Description("Order Management System data");
            this.Field<OrderTypeGraphType>().Name("orderTypes")
                .Description("Order type delivered to market i.e. market/limit etc");
            this.Field<OrderDirectionGraphType>().Name("orderDirection")
                .Description("Order direction such as buy/sell short/cover");

            this.Field(i => i.Currency).Description("Order currency values are denominated in");
            this.Field(i => i.SettlementCurrency).Description("Order settlement currency");
            this.Field(i => i.CleanDirty).Description(
                "Order values quoted clean or dirty (fixed income with or without accrued interest)");
            this.Field(i => i.AccumulatedInterest, true).Description("Accumulated interest");
            this.Field(i => i.LimitPrice, true).Description("Order limit price (if applicable)");
            this.Field(i => i.AverageFillPrice, true).Description("Order average fill price");

            this.Field(i => i.OrderedVolume, true).Description("Order volume ordered");
            this.Field(i => i.FilledVolume, true)
                .Description("Order actual filled volume can be larger or smaller than ordered volume");

            this.Field<TraderGraphType>().Name("trader").Description("Trader handling the order salient properties");

            this.Field(i => i.ClearingAgent).Description("Clearing agent used for the trade");
            this.Field(i => i.DealingInstructions).Description("Instructions for dealer");

            this.Field(i => i.OptionStrikePrice, true).Description("The strike price of the option instrument");
            this.Field(i => i.OptionExpirationDate, true).Type(new DateTimeGraphType())
                .Description("The expiration date of the option instrument");
            this.Field(i => i.OptionEuropeanAmerican).Description("The category of the option. European or American");

            this.Field(i => i.CreatedDate).Type(new DateTimeGraphType())
                .Description("The date the system created the order on");
            this.Field(i => i.LifeCycleStatus, true).Description("The order status within the life cycle");

            this.Field(i => i.Live).Description("Order is live, therefore has corresponding order allocations");
            this.Field(i => i.Autoscheduled).Description("Order has been autoscheduled");

            this.Field<ListGraphType<FundGraphType>>(
                "fund",
                "The fund the order was allocated to",
                resolve: context =>
                    {
                        IQueryable<IOrdersAllocation> FundNameQuery(IQueryable<IOrdersAllocation> i)
                        {
                            return i.Where(x => x.Fund == context.Source.Fund);
                        }

                        var loader = dataLoader.Context.GetOrAddLoader(
                            $"GetFundById-{context.Source.Fund}",
                            () => orderRepository.QueryFund(FundNameQuery));

                        return loader.LoadAsync();
                    });

            this.Field<ListGraphType<StrategyGraphType>>(
                "strategy",
                "The strategy the order was allocated to",
                resolve: context =>
                    {
                        IQueryable<IOrdersAllocation> StrategyNameQuery(IQueryable<IOrdersAllocation> i)
                        {
                            return i.Where(x => x.Strategy == context.Source.Strategy);
                        }

                        var loader = dataLoader.Context.GetOrAddLoader(
                            $"GetStrategyById-{context.Source.Strategy}",
                            () => orderRepository.QueryStrategy(StrategyNameQuery));

                        return loader.LoadAsync();
                    });

            this.Field<ListGraphType<ClientAccountGraphType>>(
                "clientAccount",
                "The client account the order was allocated to",
                resolve: context =>
                    {
                        IQueryable<IOrdersAllocation> ClientAccountQuery(IQueryable<IOrdersAllocation> i)
                        {
                            return i.Where(x => x.ClientAccountId == context.Source.ClientAccount);
                        }

                        var loader = dataLoader.Context.GetOrAddLoader(
                            $"GetClientAccountById-{context.Source.ClientAccount}",
                            () => orderRepository.QueryClientAccount(ClientAccountQuery));

                        return loader.LoadAsync();
                    });

            this.Field<ListGraphType<OrderAllocationGraphType>>(
                "orderAllocations",
                resolve: context =>
                    {
                        IQueryable<IOrdersAllocation> OrderAllocationsQuery(IQueryable<IOrdersAllocation> i)
                        {
                            return i.Where(x => x.OrderId == context.Source.ClientOrderId);
                        }

                        var loader = dataLoader.Context.GetOrAddLoader(
                            $"GetOrderAllocationsByClientOrderId-{context.Source.ClientOrderId}",
                            () => orderRepository.QueryAllocations(OrderAllocationsQuery));

                        return loader.LoadAsync();
                    });
        }
    }
}