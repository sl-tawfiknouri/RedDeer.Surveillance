using DataImport.Recorders.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Trade;
using System;

namespace DataImport.Recorders
{
    public class RedDeerAuroraOrderAllocationRecorder : IRedDeerAuroraOrderAllocationRecorder
    {
        private readonly IOrderAllocationRepository _orderAllocationRepository;
        private readonly ILogger<RedDeerAuroraOrderAllocationRecorder> _logger;
        private object _lock = new object();

        public RedDeerAuroraOrderAllocationRecorder(
            IOrderAllocationRepository orderAllocationRepository,
            ILogger<RedDeerAuroraOrderAllocationRecorder> logger)
        {
            _orderAllocationRepository = orderAllocationRepository ?? throw new ArgumentNullException(nameof(orderAllocationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        {
            _logger.LogError($"RedDeerAuroraOrderAllocationRecorder {error.Message}");
        }

        public void OnNext(OrderAllocation value)
        {
            if (value == null)
            {
                _logger.LogError($"RedDeerAuroraOrderAllocationRecorder OnNext was passed a null value. Returning.");
                return;
            }

            try
            {
                lock (_lock)
                {
                    _logger.LogInformation($"RedDeerAuroraOrderAllocationRecorder {value.OrderId} {value.Fund} Passing allocations data to repository");
                    _orderAllocationRepository.Create(value).Wait();
                    _logger.LogInformation($"RedDeerAuroraOrderAllocationRecorder {value.OrderId} {value.Fund} Completed passing allocations data to repository");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"RedDeerAuroraOrderAllocationRecorder had an error saving {e.Message}");
            }
        }
    }
}
