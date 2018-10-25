// ReSharper disable UnusedMember.Global
namespace Domain.Equity.Interfaces
{
    public interface ISecurityIdentifiers
    {
        string ReddeerId { get; }
        string ClientIdentifier { get; }
        string Figi { get; }
        string Isin { get; }
        string Sedol { get; }
        string Cusip { get; }
        string ExchangeSymbol { get; }
    }
}