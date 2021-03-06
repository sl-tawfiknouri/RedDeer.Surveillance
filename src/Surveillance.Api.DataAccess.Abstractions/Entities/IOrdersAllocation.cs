﻿namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    using System;

    public interface IOrdersAllocation
    {
        bool AutoScheduled { get; set; }

        string ClientAccountId { get; set; }

        DateTime CreatedDate { get; set; }

        string Fund { get; set; }

        int Id { get; set; }

        bool Live { get; set; }

        long OrderFilledVolume { get; set; }

        string OrderId { get; set; }

        string Strategy { get; set; }
    }
}