﻿namespace Surveillance.DataLayer.Aurora.Analytics
{
    public class AlertAnalytics
    {
        // end raw alerts

        // de duplicated alerts
        public int CancelledOrderAlertsAdjusted { get; set; }

        // raw alerts
        public int CancelledOrderAlertsRaw { get; set; }

        public int FixedIncomeHighProfitAlertsAdjusted { get; set; }

        public int FixedIncomeHighProfitAlertsRaw { get; set; }

        public int FixedIncomeHighVolumeIssuanceAlertsAdjusted { get; set; }

        public int FixedIncomeHighVolumeIssuanceAlertsRaw { get; set; }

        public int FixedIncomeWashTradeAlertsAdjusted { get; set; }

        public int FixedIncomeWashTradeAlertsRaw { get; set; }

        public int HighProfitAlertsAdjusted { get; set; }

        public int HighProfitAlertsRaw { get; set; }

        public int HighVolumeAlertsAdjusted { get; set; }

        public int HighVolumeAlertsRaw { get; set; }

        // Primary key
        public int Id { get; set; }

        public int LayeringAlertsAdjusted { get; set; }

        public int LayeringAlertsRaw { get; set; }

        public int MarkingTheCloseAlertsAdjusted { get; set; }

        public int MarkingTheCloseAlertsRaw { get; set; }

        public int PlacingOrdersAlertsAdjusted { get; set; }

        public int PlacingOrdersAlertsRaw { get; set; }

        public int RampingAlertsAdjusted { get; set; }

        public int RampingAlertsRaw { get; set; }

        public int SpoofingAlertsAdjusted { get; set; }

        public int SpoofingAlertsRaw { get; set; }

        // Foreign key - must be set
        public int SystemProcessOperationId { get; set; }

        public int WashTradeAlertsAdjusted { get; set; }

        public int WashTradeAlertsRaw { get; set; }

        // end de duplicated alerts
    }
}