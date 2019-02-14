using DomainV2.Contracts;

namespace Surveillance.Engine.DataCoordinator.Coordinator.Interfaces
{
    public interface IUploadCoordinator
    {
        void AnalyseFileId(UploadCoordinatorMessage message);
    }
}