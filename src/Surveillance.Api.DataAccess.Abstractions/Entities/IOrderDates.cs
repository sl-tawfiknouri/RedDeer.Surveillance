namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IOrderDates
    {
        string AmendedDate { get; set; }
        string BookedDate { get; set; }
        string CancelledDate { get; set; }
        string FilledDate { get; set; }
        string PlacedDate { get; set; }
        string RejectedDate { get; set; }
        string StatusChangedDate { get; set; }
    }
}