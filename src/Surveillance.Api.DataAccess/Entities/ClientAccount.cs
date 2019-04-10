using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class ClientAccount : IClientAccount
    {
        public ClientAccount(string id)
        {
            Id = id ?? string.Empty;
        }
        public string Id { get; }
    }
}
