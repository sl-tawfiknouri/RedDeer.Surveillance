using System.Linq;
using Domain.Core.Financial.Assets;
using Domain.Core.Trading.Orders;
using Domain.Core.Extensions;
using Domain.Surveillance.Rules.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Api.App.Types;
using Surveillance.Api.App.Types.Engine;
using Surveillance.Api.App.Types.Organisation;
using Surveillance.Api.App.Types.Rules;
using Surveillance.Api.App.Types.Trading;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;
using System.Collections.Generic;
using System;
using System.Globalization;
using Surveillance.Api.App.Types.Aggregation;
using Surveillance.Api.DataAccess.Repositories;

namespace Surveillance.Api.App.Infrastructure
{
    public class SurveillanceQuery : ObjectGraphType
    {
        public SurveillanceQuery(
            IFinancialInstrumentRepository financialInstrumentRepository,
            IOrderRepository orderRepository,
            IMarketRepository marketRepository,
            IRuleBreachRepository ruleBreachRepository,
            ISystemProcessOperationRuleRunRepository ruleRunRepository,
            ISystemProcessOperationUploadFileRepository fileUploadRepository,
            ISystemProcessOperationDataSynchroniserRepository dataSynchroniserRepository,
            ISystemProcessOperationDistributeRuleRepository distributeRuleRepository,
            IActiveRulesService ruleService,
            IDataLoaderContextAccessor dataAccessor,
            IHttpContextAccessor ctx)
        {
             Field<ListGraphType<FinancialInstrumentGraphType>>(
                "financialInstruments",
                "The financial instruments known to surveillance",
                arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int?>("id");

                    IQueryable<DataAccess.Abstractions.Entities.IFinancialInstrument> IdQuery(IQueryable<DataAccess.Abstractions.Entities.IFinancialInstrument> i)
                                   => i.Where(x => id == null || x.Id == id);

                    return financialInstrumentRepository.Query(IdQuery);
                });

            Field<ListGraphType<SystemProcessOperationRuleRunGraphType>>(
                "ruleRuns",
                description: "The rule runs executed by the system",
                arguments: new QueryArguments(new QueryArgument<ListGraphType<StringGraphType>> { Name = "correlationIds" }),
                resolve: context =>
                {
                    var correlationIds = context.GetArgument<List<string>>("correlationIds");

                    IQueryable<ISystemProcessOperationRuleRun> filter(IQueryable<ISystemProcessOperationRuleRun> i)
                        => i.Where(x => correlationIds == null || correlationIds.Contains(x.CorrelationId));

                    return ruleRunRepository.Query(filter);
                });

            Field<ListGraphType<SystemProcessOperationUploadFileGraphType>>(
                "uploadFiles",
                description: "The files uploaded by the system",
                resolve: context =>
                {
                    var dataLoader = dataAccessor.Context.GetOrAddLoader("allUploadfiles", fileUploadRepository.GetAllDb);

                    return dataLoader.LoadAsync();
                });

            Field<ListGraphType<SystemProcessOperationDataSynchroniserRequestGraphType>>(
                "dataSynchroniser",
                description: "The data synchroniser requests logged by the system",
                resolve: context =>
                {
                    var dataLoader = dataAccessor.Context.GetOrAddLoader("allDataSynchroniser", dataSynchroniserRepository.GetAllDb);

                    return dataLoader.LoadAsync();
                });

            Field<ListGraphType<SystemProcessOperationDistributeRuleGraphType>>(
                "distributeRule",
                description: "The rules distributed by the schedule disassembler",
                resolve: context =>
                {
                    var dataLoader = dataAccessor.Context.GetOrAddLoader("allDistributeRule", distributeRuleRepository.GetAllDb);

                    return dataLoader.LoadAsync();
                });

            Field<OrderLedgerGraphType>(
                "metaLedger",
                description: "Consolidated company portfolio incorporating all funds",
                resolve: context =>
                {
                    var dataLoader = dataAccessor.Context.GetOrAddLoader("allMetaLedger", orderRepository.QueryUnallocated);

                    return dataLoader.LoadAsync();
                });

            Field<ListGraphType<FundGraphType>>(
                "funds",
                description: "The list of funds under surveillance",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>("id");
                    IQueryable<IOrdersAllocation> IdQuery(IQueryable<IOrdersAllocation> i) => i.Where(x => string.IsNullOrWhiteSpace(id) || x.Fund == id);

                    return orderRepository.QueryFund(IdQuery);
                });

            Field<ListGraphType<StrategyGraphType>>(
                "strategies",
                description: "The list of strategies employed by surveilled orders",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>("id");
                    IQueryable<IOrdersAllocation> IdQuery(IQueryable<IOrdersAllocation> i) => i.Where(x => string.IsNullOrWhiteSpace(id) || x.Strategy == id);

                    return orderRepository.QueryStrategy(IdQuery);
                });

            Field<ListGraphType<ClientAccountGraphType>>(
                "clientAccounts",
                description: "The list of client accounts allocated to by surveilled orders",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>("id");
                    IQueryable<IOrdersAllocation> IdQuery(IQueryable<IOrdersAllocation> i) => i.Where(x => string.IsNullOrWhiteSpace(id) || x.ClientAccountId == id);

                    return orderRepository.QueryClientAccount(IdQuery);
                });

            Field<ListGraphType<MarketGraphType>>(
                "markets",
                description: "The list of markets that have surveilled orders have been issued against",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "mic" }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>("mic");
                    IQueryable<IMarket> MicQuery(IQueryable<IMarket> i) => i.Where(x => id == null || x.MarketId == id);

                    return marketRepository.Query(MicQuery);
                });

            Field<ListGraphType<RuleBreachGraphType>>(
                "ruleBreaches",
                description: "Policy rule violations detected by the surveillance engine",
                arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int?>("id");
                    IQueryable<IRuleBreach> IdQuery(IQueryable<IRuleBreach> i) => i.Where(x => id == null || x.Id == id);

                    return ruleBreachRepository.Query(IdQuery);
                });

            Field<ListGraphType<TraderGraphType>>(
                "traders",
                "Traders that have been recorded in the orders file",
                arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>("id");
                    IQueryable<IOrder> IdQuery(IQueryable<IOrder> i) => i.Where(x => id == null || x.TraderId == id);

                    return orderRepository.TradersQuery(IdQuery);
                });

            Field<ListGraphType<OrderGraphType>>(
                "orders",
                description: "Orders uploaded by client",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "ids" },
                    new QueryArgument<IntGraphType> { Name = "take" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "traderIds" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "reddeerIds" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "directions" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "types" },
                    new QueryArgument<StringGraphType> { Name = "placedDateFrom" },
                    new QueryArgument<StringGraphType> { Name = "placedDateTo" }
                    ),
                resolve: context =>
                {
                    var options = new OrderQueryOptions
                    {
                        Ids = context.GetArgument<List<int>>("ids"),
                        Take = context.GetArgument<int?>("take"),
                        TraderIds = context.GetArgument<List<string>>("traderIds"),
                        ReddeerIds = context.GetArgument<List<string>>("reddeerIds"),
                        Directions = context.GetArgument<List<int>>("directions"),
                        Types = context.GetArgument<List<int>>("types"),
                        PlacedDateFrom = context.GetArgument<string>("placedDateFrom"),
                        PlacedDateTo = context.GetArgument<string>("placedDateTo")
                    };
                    return orderRepository.Query(options);
                });

            Field<ListGraphType<AggregationGraphType>>(
                "orderAggregation",
                description: "Aggregation of orders uploaded by client",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "ids" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "traderIds" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "reddeerIds" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "directions" },
                    new QueryArgument<ListGraphType<IntGraphType>> { Name = "types" },
                    new QueryArgument<StringGraphType> { Name = "placedDateFrom" },
                    new QueryArgument<StringGraphType> { Name = "placedDateTo" },
                    new QueryArgument<StringGraphType> { Name = "tzName" }
                    ),
                resolve: context =>
                {
                    var options = new OrderQueryOptions
                    {
                        Ids = context.GetArgument<List<int>>("ids"),
                        TraderIds = context.GetArgument<List<string>>("traderIds"),
                        ReddeerIds = context.GetArgument<List<string>>("reddeerIds"),
                        Directions = context.GetArgument<List<int>>("directions"),
                        Types = context.GetArgument<List<int>>("types"),
                        PlacedDateFrom = context.GetArgument<string>("placedDateFrom"),
                        PlacedDateTo = context.GetArgument<string>("placedDateTo"),
                        TzName = context.GetArgument<string>("tzName")
                    };
                    return orderRepository.AggregationQuery(options);
                });

            Field<ListGraphType<RulesTypeEnumGraphType>>(
                "rules",
                "The category of the rule",
                resolve: context => ruleService.EnabledRules());

            Field<ListGraphType<OrganisationTypeEnumGraphType>>(
                "organisationFactors",
                "The type of organisation factors",
                resolve: context => OrganisationalFactors.None.GetEnumPermutations());

            Field<ListGraphType<OrderTypeGraphType>>(
                "orderTypes",
                "The type of the order given to market",
                resolve: context => OrderTypes.NONE.GetEnumPermutations());

            Field<ListGraphType<InstrumentTypeGraphType>>(
                "instrumentTypes",
                "A primitive perspective on the asset class. To see further details use the CFI code",
                resolve: context => InstrumentTypes.None.GetEnumPermutations());

        }
    }
}
