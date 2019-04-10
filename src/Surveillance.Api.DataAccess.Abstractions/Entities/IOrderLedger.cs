namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IOrderLedger
    {
        IOrder[] Orders { get; set; }
    }
}