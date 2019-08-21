namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class Fund : IFund
    {
        public Fund(string name)
        {
            this.Name = name ?? string.Empty;
        }

        public string Name { get; set; }

        public IOrderLedger OrderLedger { get; set; }
    }
}