using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class Fund : IFund
    {
        public Fund(string name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; set; }

        public IOrderLedger OrderLedger { get; set; }
    }
}
