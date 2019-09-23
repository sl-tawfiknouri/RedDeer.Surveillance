namespace Domain.Core.Markets.Timebars
{
    /// <summary>
    ///     Volume traded within parent datetime range context
    /// </summary>
    public struct Volume
    {
        public Volume(long traded)
        {
            this.Traded = traded;
        }

        /// <summary>
        ///     Quantity of equities traded
        /// </summary>
        public long Traded { get; }
    }
}