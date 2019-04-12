using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class OrderManagementSystem : IOrderManagementSystem
    {
        public OrderManagementSystem(
            string orderVersion,
            string orderVersionLinkId,
            string orderGroupId)
        {
            Version = orderVersion ?? string.Empty;
            VersionLinkId = orderVersionLinkId ?? string.Empty;
            GroupId = orderGroupId ?? string.Empty;
        }

        public string Version { get; set; }
        public string VersionLinkId { get; set; }
        public string GroupId { get; set; }
    }
}
