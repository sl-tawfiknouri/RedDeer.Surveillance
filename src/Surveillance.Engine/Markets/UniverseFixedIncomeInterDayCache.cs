﻿using Domain.Core.Dates;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Interfaces;
using Domain.Core.Markets.Timebars;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surveillance.Engine.Rules.Markets
{
    /// <summary>
    ///     Universe event market cache
    ///     Consider that it is a cache of recent market events
    ///     Before attempting to query for out of range data
    /// </summary>
    public class UniverseFixedIncomeInterDayCache : IUniverseFixedIncomeInterDayCache
    {
        private readonly IRuleRunDataRequestRepository _dataRequestRepository;

        private readonly ILogger _logger;

        private IDictionary<string, FixedIncomeInterDayTimeBarCollection> _latestExchangeFrameBook;

        private ConcurrentDictionary<string, IFixedIncomeInterDayHistoryStack> _marketHistory;

        public UniverseFixedIncomeInterDayCache(IRuleRunDataRequestRepository dataRequestRepository, ILogger logger)
        {
            this._dataRequestRepository =
                dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._latestExchangeFrameBook = new ConcurrentDictionary<string, FixedIncomeInterDayTimeBarCollection>();
            this._marketHistory = new ConcurrentDictionary<string, IFixedIncomeInterDayHistoryStack>();
        }

        public void Add(FixedIncomeInterDayTimeBarCollection value)
        {
            if (value == null)
            {
                this._logger.LogInformation("UniverseFixedIncomeInterDayCache was asked to add null. returning");
                return;
            }

            this._logger.LogInformation(
                $"UniverseFixedIncomeInterDayCache adding {value.Epoch} - {value.Exchange?.MarketIdentifierCode}");

            if (this._latestExchangeFrameBook.ContainsKey(value.Exchange.MarketIdentifierCode))
            {
                this._latestExchangeFrameBook.Remove(value.Exchange.MarketIdentifierCode);
                this._latestExchangeFrameBook.Add(value.Exchange.MarketIdentifierCode, value);
            }
            else
            {
                this._latestExchangeFrameBook.Add(value.Exchange.MarketIdentifierCode, value);
            }

            if (!this._marketHistory.ContainsKey(value.Exchange.MarketIdentifierCode))
            {
                var history = new FixedIncomeInterDayHistoryStack();
                history.Add(value, value.Epoch);
                this._marketHistory.TryAdd(value.Exchange.MarketIdentifierCode, history);
            }
            else
            {
                this._marketHistory.TryGetValue(value.Exchange.MarketIdentifierCode, out var history);

                history?.Add(value, value.Epoch);
                history?.ArchiveExpiredActiveItems(value.Epoch);
            }
        }

        public object Clone()
        {
            var clone = this.MemberwiseClone() as UniverseFixedIncomeInterDayCache;
            clone.SetClone();

            return clone;
        }

        public MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar> Get(MarketDataRequest request)
        {
            if (request == null || !request.IsValid())
            {
                this._logger.LogError("UniverseFixedIncomeInterDayCache received either a null or invalid request");
                return MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar>.MissingData();
            }

            this._logger.LogInformation(
                $"UniverseFixedIncomeInterDayCache fetching for market {request?.MarketIdentifierCode} from {request?.UniverseEventTimeFrom} to {request?.UniverseEventTimeTo} as part of rule run {request?.SystemProcessOperationRuleRunId}");

            if (!this._latestExchangeFrameBook.ContainsKey(request.MarketIdentifierCode))
            {
                this._dataRequestRepository.CreateDataRequest(request);
                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache was not able to find the MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar>.MissingData();
            }

            this._latestExchangeFrameBook.TryGetValue(request.MarketIdentifierCode, out var exchangeFrame);

            if (exchangeFrame == null)
            {
                this._dataRequestRepository.CreateDataRequest(request);
                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache was not able to find the MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar>.MissingData();
            }

            var security =
                exchangeFrame.Securities.FirstOrDefault(sec => Equals(sec.Security.Identifiers, request.Identifiers));

            if (security == null)
            {
                this._dataRequestRepository.CreateDataRequest(request);
                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache was not able to find the security {request.Identifiers} for MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar>.MissingData();
            }

            if (exchangeFrame.Epoch.Date >= request.UniverseEventTimeTo.GetValueOrDefault()
                && exchangeFrame.Epoch.Date <= request.UniverseEventTimeFrom.GetValueOrDefault())
            {
                this._dataRequestRepository.CreateDataRequest(request);

                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache was not able to find the security {request.Identifiers} for MIC {request.MarketIdentifierCode} in the latest exchange frame book within a suitable data range to {request.UniverseEventTimeTo} from {request.UniverseEventTimeFrom}. Recording missing data.");

                return MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar>.MissingData();
            }

            this._logger.LogInformation(
                $"UniverseFixedIncomeInterDayCache was able to find a match for {request.Identifiers} returning data.");
            return new MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar>(security, false, false);
        }

        /// <summary>
        ///     Assumes that any data implies that the whole data set/range is covered
        /// </summary>
        public MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>> GetMarkets(MarketDataRequest request)
        {
            if (request == null || !request.IsValid())
            {
                this._logger.LogError("UniverseFixedIncomeInterDayCache received either a null or invalid request");
                return MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>.MissingData();
            }

            if (!this._marketHistory.TryGetValue(request.MarketIdentifierCode, out var marketStack))
            {
                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache GetMarkets was not able to find a market history entry for {request.MarketIdentifierCode}");
                this._dataRequestRepository.CreateDataRequest(request);
                return MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>.MissingData();
            }

            var securityDataTicks = marketStack.ActiveMarketHistory().Where(amh => amh != null)
                .Select(
                    amh => amh.Securities?.FirstOrDefault(sec => Equals(sec.Security.Identifiers, request.Identifiers)))
                .Where(sec => sec != null).Where(sec => sec.TimeStamp <= request.UniverseEventTimeTo)
                .Where(sec => sec.TimeStamp >= request.UniverseEventTimeFrom).ToList();

            if (!securityDataTicks.Any())
            {
                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache GetMarkets was not able to find market data for the security on {request.MarketIdentifierCode} with ids {request.Identifiers}");

                this._dataRequestRepository.CreateDataRequest(request);
                return MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>.MissingData();
            }

            this._logger.LogInformation(
                $"UniverseFixedIncomeInterDayCache GetMarkets was able to find a market history entry for {request.MarketIdentifierCode} and id {request.Identifiers}");

            return new MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>(securityDataTicks, false, false);
        }

        public MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>> GetMarketsForRange(
            MarketDataRequest request,
            IReadOnlyCollection<DateRange> dates,
            RuleRunMode runMode)
        {
            dates = dates?.Where(dat => dat != null)?.ToList();

            if (dates == null || !dates.Any())
            {
                this._logger.LogError(
                    "UniverseFixedIncomeInterDayCache GetMarketsForRange received either a null or invalid request (dates)");

                return MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>.MissingData();
            }

            if (request == null || !request.IsValid())
            {
                this._logger.LogError(
                    "UniverseFixedIncomeInterDayCache GetMarketsForRange received either a null or invalid request");
                return MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>.MissingData();
            }

            var projectedRequests = dates.Select(
                i => new MarketDataRequest(
                    null,
                    request.MarketIdentifierCode,
                    request.Cfi,
                    request.Identifiers,
                    i.Start,
                    i.End,
                    request.SystemProcessOperationRuleRunId,
                    request.IsCompleted,
                    DataSource.AnyInterday)).ToList();

            var responseList = new List<MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>>();
            foreach (var paramSet in projectedRequests) responseList.Add(this.GetMarkets(paramSet));

            if (!responseList.Any())
            {
                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache GetMarketsForRange had missing data for rule run id {request.SystemProcessOperationRuleRunId}");
                return MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>.MissingData();
            }

            var isMissingData = responseList.Any(o => o.HadMissingData);

            if (runMode == RuleRunMode.ValidationRun && isMissingData)
            {
                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache GetMarketsForRange was running a validation run and had missing data for rule run id {request.SystemProcessOperationRuleRunId}");
                return MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>.MissingData();
            }

            var responses = responseList.Where(i => i.Response != null).SelectMany(i => i.Response).ToList();

            if (isMissingData)
                this._logger.LogInformation(
                    $"UniverseFixedIncomeInterDayCache GetMarketsForRange was running and had missing data for rule run id {request.SystemProcessOperationRuleRunId} but is proceeding on a best effort basis");

            return new MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>>(responses, isMissingData, true);
        }

        public void SetClone()
        {
            this._latestExchangeFrameBook =
                new Dictionary<string, FixedIncomeInterDayTimeBarCollection>(this._latestExchangeFrameBook);
            this._marketHistory = new ConcurrentDictionary<string, IFixedIncomeInterDayHistoryStack>(this._marketHistory);
        }
    }
}
