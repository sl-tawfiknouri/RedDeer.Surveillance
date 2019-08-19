namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class OrderManagementSystem : IOrderManagementSystem
    {
        public OrderManagementSystem(string orderVersion, string orderVersionLinkId, string orderGroupId)
        {
            this.Version = orderVersion ?? string.Empty;
            this.VersionLinkId = orderVersionLinkId ?? string.Empty;
            this.GroupId = orderGroupId ?? string.Empty;
        }

        public string GroupId { get; set; }

        public string Version { get; set; }

        public string VersionLinkId { get; set; }
    }
}