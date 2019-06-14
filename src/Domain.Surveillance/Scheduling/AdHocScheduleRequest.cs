﻿using System;
using Domain.Surveillance.Aws;

namespace Domain.Surveillance.Scheduling
{
    /// <summary>
    /// Should be JSON serialisable
    /// </summary>
    [Serializable]
    public class AdHocScheduleRequest
    {
        public string Id { get; set; }
        public DateTime ScheduleFor { get; set; }
        public SurveillanceSqsQueue Queue { get; set; }
        public string JsonSqsMessage { get; set; }
        public string OriginatingService { get; set; }
        public bool Processed { get; set; }
    }
}
