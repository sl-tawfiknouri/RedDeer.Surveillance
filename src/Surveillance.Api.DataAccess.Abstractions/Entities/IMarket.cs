namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IMarket
    {
        int Id { get; set; }
        string MarketId { get; set; }
        string MarketName { get; set; }
    }
}