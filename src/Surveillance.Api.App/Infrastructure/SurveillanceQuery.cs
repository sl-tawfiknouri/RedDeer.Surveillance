namespace Surveillance.Api.App.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Extensions;
    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules.Interfaces;

    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Microsoft.AspNetCore.Http;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
    using Surveillance.Api.App.Services;
    using Surveillance.Api.App.Types;
    using Surveillance.Api.App.Types.Aggregation;
    using Surveillance.Api.App.Types.Engine;
    using Surveillance.Api.App.Types.Organisation;
    using Surveillance.Api.App.Types.Rules;
    using Surveillance.Api.App.Types.TickPriceHistory;
    using Surveillance.Api.App.Types.Trading;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;
    using Surveillance.Api.DataAccess.Repositories;

    public class SurveillanceQuery : ObjectGraphType
    {
        public SurveillanceQuery(
            IRefinitivTickPriceHistoryService refinitivTickPriceHistoryService,
            IFinancialInstrumentRepository financialInstrumentRepository,
            IOrderRepository orderRepository,
            IMarketRepository marketRepository,
            IBrokerRepository brokerRepository,
            IRuleBreachRepository ruleBreachRepository,
            ISystemProcessOperationRuleRunRepository ruleRunRepository,
            ISystemProcessOperationUploadFileRepository fileUploadRepository,
            ISystemProcessOperationDataSynchroniserRepository dataSynchroniserRepository,
            ISystemProcessOperationDistributeRuleRepository distributeRuleRepository,
            IActiveRulesService ruleService,
            IDataLoaderContextAccessor dataAccessor,
            IHttpContextAccessor ctx)
        {
            this.Field<ListGraphType<FinancialInstrumentGraphType>>(
                "financialInstruments",
                "The financial instruments known to surveillance",
                new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }),
                context =>
                    {
                        var id = context.GetArgument<int?>("id");

                        IQueryable<IFinancialInstrument> IdQuery(IQueryable<IFinancialInstrument> i)
                        {
                            return i.Where(x => id == null || x.Id == id);
                        }

                        return financialInstrumentRepository.Query(IdQuery);
                    });

            this.Field<ListGraphType<SystemProcessOperationRuleRunGraphType>>(
                "ruleRuns",
                "The rule runs executed by the system",
                new QueryArguments(new QueryArgument<ListGraphType<StringGraphType>> { Name = "correlationIds" }),
                context =>
                    {
                        var correlationIds = context.GetArgument<List<string>>("correlationIds");

                        IQueryable<ISystemProcessOperationRuleRun> filter(IQueryable<ISystemProcessOperationRuleRun> i)
                        {
                            return i.Where(x => correlationIds == null || correlationIds.Contains(x.CorrelationId));
                        }

                        return ruleRunRepository.Query(filter);
                    });

            this.Field<ListGraphType<SystemProcessOperationUploadFileGraphType>>(
                "uploadFiles",
                "The files uploaded by the system",
                resolve: context =>
                    {
                        var dataLoader = dataAccessor.Context.GetOrAddLoader(
                            "allUploadfiles",
                            fileUploadRepository.GetAllDb);

                        return dataLoader.LoadAsync();
                    });

            this.Field<ListGraphType<SystemProcessOperationDataSynchroniserRequestGraphType>>(
                "dataSynchroniser",
                "The data synchroniser requests logged by the system",
                resolve: context =>
                    {
                        var dataLoader = dataAccessor.Context.GetOrAddLoader(
                            "allDataSynchroniser",
                            dataSynchroniserRepository.GetAllDb);

                        return dataLoader.LoadAsync();
                    });

            this.Field<ListGraphType<SystemProcessOperationDistributeRuleGraphType>>(
                "distributeRule",
                "The rules distributed by the schedule disassembler",
                resolve: context =>
                    {
                        var dataLoader = dataAccessor.Context.GetOrAddLoader(
                            "allDistributeRule",
                            distributeRuleRepository.GetAllDb);

                        return dataLoader.LoadAsync();
                    });

            this.Field<OrderLedgerGraphType>(
                "metaLedger",
                "Consolidated company portfolio incorporating all funds",
                resolve: context =>
                    {
                        var dataLoader = dataAccessor.Context.GetOrAddLoader(
                            "allMetaLedger",
                            orderRepository.QueryUnallocated);

                        return dataLoader.LoadAsync();
                    });

            this.Field<ListGraphType<FundGraphType>>(
                "funds",
                "The list of funds under surveillance",
                new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                context =>
                    {
                        var id = context.GetArgument<string>("id");

                        IQueryable<IOrdersAllocation> IdQuery(IQueryable<IOrdersAllocation> i)
                        {
                            return i.Where(x => string.IsNullOrWhiteSpace(id) || x.Fund == id);
                        }

                        return orderRepository.QueryFund(IdQuery);
                    });

            this.Field<ListGraphType<StrategyGraphType>>(
                "strategies",
                "The list of strategies employed by surveilled orders",
                new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                context =>
                    {
                        var id = context.GetArgument<string>("id");

                        IQueryable<IOrdersAllocation> IdQuery(IQueryable<IOrdersAllocation> i)
                        {
                            return i.Where(x => string.IsNullOrWhiteSpace(id) || x.Strategy == id);
                        }

                        return orderRepository.QueryStrategy(IdQuery);
                    });

            this.Field<ListGraphType<ClientAccountGraphType>>(
                "clientAccounts",
                "The list of client accounts allocated to by surveilled orders",
                new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                context =>
                    {
                        var id = context.GetArgument<string>("id");

                        IQueryable<IOrdersAllocation> IdQuery(IQueryable<IOrdersAllocation> i)
                        {
                            return i.Where(x => string.IsNullOrWhiteSpace(id) || x.ClientAccountId == id);
                        }

                        return orderRepository.QueryClientAccount(IdQuery);
                    });

            this.Field<ListGraphType<MarketGraphType>>(
                "markets",
                "The list of markets that have surveilled orders have been issued against",
                new QueryArguments(new QueryArgument<StringGraphType> { Name = "mic" }),
                context =>
                    {
                        var id = context.GetArgument<string>("mic");

                        IQueryable<IMarket> MicQuery(IQueryable<IMarket> i)
                        {
                            return i.Where(x => id == null || x.MarketId == id);
                        }

                        return marketRepository.Query(MicQuery);
                    });

            this.Field<ListGraphType<BrokerGraphType>>(
                "brokers",
                "The list of brokers that  orders have been placed with",
                new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                context =>
                    {
                        var id = context.GetArgument<int?>("id");

                        IQueryable<IBroker> IdQuery(IQueryable<IBroker> i)
                        {
                            return i.Where(x => id == null || x.Id == id);
                        }
                        
                        return brokerRepository.Query(IdQuery);
                    });

            this.Field<ListGraphType<RuleBreachGraphType>>(
                "ruleBreaches",
                "Policy rule violations detected by the surveillance engine",
                new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }),
                context =>
                    {
                        var id = context.GetArgument<int?>("id");

                        IQueryable<IRuleBreach> IdQuery(IQueryable<IRuleBreach> i)
                        {
                            return i.Where(x => id == null || x.Id == id);
                        }

                        return ruleBreachRepository.Query(IdQuery);
                    });

            this.Field<ListGraphType<TraderGraphType>>(
                "traders",
                "Traders that have been recorded in the orders file",
                new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }),
                context =>
                    {
                        var id = context.GetArgument<string>("id");

                        IQueryable<IOrder> IdQuery(IQueryable<IOrder> i)
                        {
                            return i.Where(x => id == null || x.TraderId == id);
                        }

                        return orderRepository.TradersQuery(IdQuery);
                    });

            this.Field<ListGraphType<OrderGraphType>>(
                "orders",
                "Orders uploaded by client",
                new QueryArguments(
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "ids" },
                    new QueryArgument<IntGraphType> { Name = "take" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "traderIds" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "excludeTraderIds" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "reddeerIds" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "statuses" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "directions" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "types" },
                    new QueryArgument<DateTimeGraphType> { Name = "placedDateFrom" },
                    new QueryArgument<DateTimeGraphType> { Name = "placedDateTo" }),
                context =>
                    {
                        var options = new OrderQueryOptions
                                          {
                                              Ids = context.GetArgument<List<int>>("ids"),
                                              Take = context.GetArgument<int?>("take"),
                                              TraderIds = context.GetArgument<List<string>>("traderIds")?.ToHashSet(),
                                              ExcludeTraderIds =
                                                  context.GetArgument<List<string>>("excludeTraderIds")?.ToHashSet(),
                                              ReddeerIds = context.GetArgument<List<string>>("reddeerIds"),
                                              Statuses = context.GetArgument<List<int>>("statuses"),
                                              Directions = context.GetArgument<List<int>>("directions"),
                                              Types = context.GetArgument<List<int>>("types"),
                                              PlacedDateFrom = context.GetArgument<DateTime?>("placedDateFrom"),
                                              PlacedDateTo = context.GetArgument<DateTime?>("placedDateTo")
                                          };
                        return orderRepository.Query(options);
                    });

            this.Field<ListGraphType<AggregationGraphType>>(
                "orderAggregation",
                "Aggregation of orders uploaded by client",
                new QueryArguments(
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "ids" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "traderIds" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "excludeTraderIds" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "reddeerIds" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "statuses" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "directions" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "types" },
                    new QueryArgument<DateTimeGraphType> { Name = "placedDateFrom" },
                    new QueryArgument<DateTimeGraphType> { Name = "placedDateTo" },
                    new QueryArgument<StringGraphType> { Name = "tzName" }),
                context =>
                    {
                        var options = new OrderQueryOptions
                                          {
                                              Ids = context.GetArgument<List<int>>("ids"),
                                              TraderIds = context.GetArgument<List<string>>("traderIds")?.ToHashSet(),
                                              ExcludeTraderIds =
                                                  context.GetArgument<List<string>>("excludeTraderIds")?.ToHashSet(),
                                              ReddeerIds = context.GetArgument<List<string>>("reddeerIds"),
                                              Statuses = context.GetArgument<List<int>>("statuses"),
                                              Directions = context.GetArgument<List<int>>("directions"),
                                              Types = context.GetArgument<List<int>>("types"),
                                              PlacedDateFrom = context.GetArgument<DateTime?>("placedDateFrom"),
                                              PlacedDateTo = context.GetArgument<DateTime?>("placedDateTo"),
                                              TzName = context.GetArgument<string>("tzName")
                                          };
                        return orderRepository.AggregationQuery(options);
                    });

            this.Field<ListGraphType<RulesTypeEnumGraphType>>(
                "rules",
                "The category of the rule",
                resolve: context => ruleService.EnabledRules());

            this.Field<ListGraphType<OrganisationTypeEnumGraphType>>(
                "organisationFactors",
                "The type of organisation factors",
                resolve: context => OrganisationalFactors.None.GetEnumPermutations());

            this.Field<ListGraphType<OrderTypeGraphType>>(
                "orderTypes",
                "The type of the order given to market",
                resolve: context => OrderTypes.NONE.GetEnumPermutations());

            this.Field<ListGraphType<InstrumentTypeGraphType>>(
                "instrumentTypes",
                "A primitive perspective on the asset class. To see further details use the CFI code",
                resolve: context => InstrumentTypes.None.GetEnumPermutations());

            this.Field<ListGraphType<TickPriceHistoryTimeBarGraphType>>(
                "tickPriceHistoryTimeBars",
                "Tick Price History TimeBar",
                new QueryArguments(
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "rics" },
                    new QueryArgument<DateTimeGraphType> { Name = "startDateTime" },
                    new QueryArgument<DateTimeGraphType> { Name = "endDateTime" }),
                context =>
                {
                    var rics = context.GetArgument<List<string>>("rics");
                    var startDateTime = context.GetArgument<DateTime?>("startDateTime");
                    var endDateTime = context.GetArgument<DateTime?>("endDateTime");

                    return refinitivTickPriceHistoryService.GetEndOfDayTimeBarsAsync(startDateTime, endDateTime, rics);
                });
        }
    }
}