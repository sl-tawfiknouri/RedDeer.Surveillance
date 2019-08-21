namespace Surveillance.Api.App.Configuration.Interfaces
{
    public interface IEnvironmentService
    {
        bool IsEc2Instance();

        bool IsUnitTest();
    }
}