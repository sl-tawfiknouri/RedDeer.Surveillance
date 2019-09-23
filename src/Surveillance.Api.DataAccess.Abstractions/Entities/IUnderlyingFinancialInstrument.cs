namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IUnderlyingFinancialInstrument
    {
        string UnderlyingBloombergTicker { get; set; }

        string UnderlyingCfi { get; set; }

        string UnderlyingClientIdentifier { get; set; }

        string UnderlyingCusip { get; set; }

        string UnderlyingExchangeSymbol { get; set; }

        string UnderlyingFigi { get; set; }

        string UnderlyingIsin { get; set; }

        string UnderlyingLei { get; set; }

        string UnderlyingName { get; set; }

        string UnderlyingSedol { get; set; }
    }
}