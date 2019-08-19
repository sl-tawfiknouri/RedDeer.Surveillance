namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class ClientAccount : IClientAccount
    {
        public ClientAccount(string id)
        {
            this.Id = id ?? string.Empty;
        }

        public string Id { get; }
    }
}