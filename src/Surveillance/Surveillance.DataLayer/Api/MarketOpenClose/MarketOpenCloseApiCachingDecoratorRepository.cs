﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;

namespace Surveillance.DataLayer.Api.MarketOpenClose
{
    public class MarketOpenCloseApiCachingDecoratorRepository : IMarketOpenCloseApiCachingDecoratorRepository
    {
        private readonly IMarketOpenCloseApiRepository _decoratedRepository;
        private readonly ILogger _logger;

        private IReadOnlyCollection<ExchangeDto> _cachedMarketData;

        private readonly TimeSpan _cacheLength;
        private DateTime _cacheExpiry;

        public MarketOpenCloseApiCachingDecoratorRepository(
            IMarketOpenCloseApiRepository decoratedRepository,
            ILogger<MarketOpenCloseApiCachingDecoratorRepository> logger)
        {
            _cacheExpiry = DateTime.Now.AddMilliseconds(-1);
            _cacheLength = TimeSpan.FromMinutes(30);

            _decoratedRepository = decoratedRepository ?? throw new ArgumentNullException(nameof(decoratedRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ExchangeDto>> Get()
        {
            if (DateTime.Now < _cacheExpiry)
            {
                return _cachedMarketData;
            }

            _logger.LogDebug($"Fetching market open/close data in the cached repository");
            _cachedMarketData = await _decoratedRepository.Get();
            _cacheExpiry = DateTime.Now.Add(_cacheLength);

            return _cachedMarketData;
        }
    }
}