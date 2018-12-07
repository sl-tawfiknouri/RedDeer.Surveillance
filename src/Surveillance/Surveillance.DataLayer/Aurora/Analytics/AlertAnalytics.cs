﻿namespace Surveillance.DataLayer.Aurora.Analytics
{
    public class AlertAnalytics
    {
        // Primary key
        public int Id { get; set; }

        // Foreign key - must be set
        public int SystemProcessOperationId { get; set; }

        // raw alerts
        public int CancelledOrderAlertsRaw { get; set; }
        public int HighProfitAlertsRaw { get; set; }
        public int HighVolumeAlertsRaw { get; set; }
        public int LayeringAlertsRaw { get; set; }
        public int MarkingTheCloseAlertsRaw { get; set; }
        public int SpoofingAlertsRaw { get; set; }
        public int WashTradeAlertsRaw { get; set; }
        // end raw alerts

        // de duplicated alerts
        public int CancelledOrderAlertsAdjusted { get; set; }
        public int HighProfitAlertsAdjusted { get; set; }
        public int HighVolumeAlertsAdjusted { get; set; }
        public int LayeringAlertsAdjusted { get; set; }
        public int MarkingTheCloseAlertsAdjusted { get; set; }
        public int SpoofingAlertsAdjusted { get; set; }
        public int WashTradeAlertsAdjusted { get; set; }
        // end de duplicated alerts
    }
}