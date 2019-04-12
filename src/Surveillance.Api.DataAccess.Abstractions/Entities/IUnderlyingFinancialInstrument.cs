namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IUnderlyingFinancialInstrument
    {
        string UnderlyingCfi { get; set; }
        string UnderlyingName { get; set; }
        string UnderlyingSedol { get; set; }
        string UnderlyingIsin { get; set; }
        string UnderlyingFigi { get; set; }
        string UnderlyingCusip { get; set; }
        string UnderlyingLei { get; set; }
        string UnderlyingExchangeSymbol { get; set; }
        string UnderlyingBloombergTicker { get; set; }
        string UnderlyingClientIdentifier { get; set; }
    }
}
