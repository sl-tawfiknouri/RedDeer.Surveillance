namespace Domain.Equity.Trading
{
    /// <summary>
    /// Stock market update for a security
    /// </summary>
    public class SecurityTick
    {
        public SecurityTick(Security security, Spread spread, Volume volume)
        {
            Security = security;
            Spread = spread;
            Volume = volume;
        }

        /// <summary>
        /// The security the tick related to
        /// </summary>
        public Security Security { get; }

        /// <summary>
        /// Price spread at the tick point
        /// </summary>
        public Spread Spread { get; }

        /// <summary>
        /// The volume of the security traded since the last tick
        /// </summary>
        public Volume Volume { get; }
    }
}