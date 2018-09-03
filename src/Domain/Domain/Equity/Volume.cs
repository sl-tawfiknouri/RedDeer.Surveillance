namespace Domain.Equity
{
    /// <summary>
    /// Volume traded within parent datetime range context
    /// </summary>
    public struct Volume
    {
        public Volume(int traded)
        {
            Traded = traded;
        }

        /// <summary>
        /// Quantity of equities traded
        /// </summary>
        public int Traded { get; }
    }
}
