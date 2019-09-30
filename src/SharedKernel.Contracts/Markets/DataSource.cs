namespace SharedKernel.Contracts.Markets
{
    /// <summary>
    /// The data source.
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// The all.
        /// </summary>
        Any = 0,

        /// <summary>
        /// The none.
        /// </summary>
        None = 1,

        /// <summary>
        /// The all inter day.
        /// </summary>
        AnyInterday = 2,

        /// <summary>
        /// The all intraday.
        /// </summary>
        AnyIntraday = 3,

        /// <summary>
        /// The data provider.
        /// </summary>
        Bmll = 4,

        /// <summary>
        /// The fact set.
        /// </summary>
        Factset = 5,

        /// <summary>
        /// The mark it.
        /// </summary>
        Markit = 6,

        /// <summary>
        /// The thomson reuters.
        /// </summary>
        Refinitive = 7
    }
}