using System;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IOrderDates
    {
        DateTime? AmendedDate { get; set; }
        DateTime? BookedDate { get; set; }
        DateTime? CancelledDate { get; set; }
        DateTime? FilledDate { get; set; }
        DateTime? PlacedDate { get; set; }
        DateTime? RejectedDate { get; set; }
        DateTime? StatusChangedDate { get; set; }
    }
}