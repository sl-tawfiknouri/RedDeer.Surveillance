using RedDeer.Surveillance.Api.Client.Enums;
using System;
using System.Collections.Generic;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string ClientOrderId { get; set; }
        public string OrderVersion { get; set; }
        public string OrderVersionLinkId { get; set; }
        public string OrderGroupId { get; set; }
        public OrderType? OrderType { get; set; }
        public OrderDirection? Direction { get; set; }
        public string Currency { get; set; }
        public string SettlementCurrency { get; set; }
        public string CleanDirty { get; set; }
        public decimal? AccumulatedInterest { get; set; }
        public decimal? LimitPrice { get; set; }
        public decimal? AverageFillPrice { get; set; }
        public long? OrderedVolume { get; set; }
        public long? FilledVolume { get; set; }
        public string ClearingAgent { get; set; }
        public string DealingInstructions { get; set; }
        public decimal? OptionStrikePrice { get; set; }
        public string OptionEuropeanAmerican { get; set; }

        public OrderDatesDto OrderDates { get; set; }
        public TraderDto Trader { get; set; }
        public MarketDto Market { get; set; }
        public BrokerDto Broker { get; set; }

        public FinancialInstrumentDto FinancialInstrument { get; set; }

        public List<OrderAllocationDto> OrderAllocations { get; set; }

        public DateTime? OptionExpirationDate { get; set; }
    }
}
