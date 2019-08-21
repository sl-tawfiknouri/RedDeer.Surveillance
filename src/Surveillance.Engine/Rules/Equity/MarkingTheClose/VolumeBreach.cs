namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose
{
    public class VolumeBreach
    {
        public decimal? BuyVolumeBreach { get; set; }

        public bool HasBuyVolumeBreach { get; set; }

        public bool HasSellVolumeBreach { get; set; }

        public decimal? SellVolumeBreach { get; set; }

        public bool HasBreach()
        {
            return this.HasBuyVolumeBreach || this.HasSellVolumeBreach;
        }
    }
}