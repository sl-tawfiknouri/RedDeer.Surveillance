namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IHighVolumeVenueFilter : IUniverseRule
    {
        HashSet<Order> UniverseEventsPassedFilter { get; set; }
    }
}