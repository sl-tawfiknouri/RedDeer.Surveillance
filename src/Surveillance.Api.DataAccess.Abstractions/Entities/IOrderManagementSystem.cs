namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IOrderManagementSystem
    {
        string GroupId { get; set; }

        string Version { get; set; }

        string VersionLinkId { get; set; }
    }
}