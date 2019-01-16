namespace DomainV2.Files
{
    /// <summary>
    /// Version 0.1 of the Allocation File
    /// </summary>
    public class AllocationFileCsv
    {
        // Foreign Key to Orders via the client provided order id
        public string OrderId { get; set; }

        // Identifiers for the allocated accounting entity
        public string Fund { get; set; }
        public string Strategy { get; set; }
        public string ClientAccountId { get; set; }

        // The allocation
        public string OrderFilledVolume { get; set; }

        /* IO */
        public int RowId { get; set; }
    }
}
