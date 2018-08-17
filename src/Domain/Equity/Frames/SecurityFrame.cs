namespace Domain.Equity.Trading.Frames
{
    /// <summary>
    /// Stock market update for a security
    /// </summary>
    public class SecurityFrame
    {
        public SecurityFrame(Security security, Spread spread, Volume volume)
        {
            Security = security;
            Spread = spread;
            Volume = volume;
        }

        /// <summary>
        /// The security the frame related to
        /// </summary>
        public Security Security { get; }

        /// <summary>
        /// Price spread at the frame point
        /// </summary>
        public Spread Spread { get; }

        /// <summary>
        /// The volume of the security traded since the last frame
        /// </summary>
        public Volume Volume { get; }
    }
}