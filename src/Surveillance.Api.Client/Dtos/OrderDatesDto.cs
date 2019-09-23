namespace RedDeer.Surveillance.Api.Client.Dtos
{
    using System;

    public class OrderDatesDto
    {
        public DateTime? AmendedDate { get; set; }

        public DateTime? BookedDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public DateTime? FilledDate { get; set; }

        public DateTime? PlacedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        public DateTime? StatusChangedDate { get; set; }
    }
}