namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IFund
    {
        string Name { get; set; }

        IOrderLedger OrderLedger { get; set; }
    }
}