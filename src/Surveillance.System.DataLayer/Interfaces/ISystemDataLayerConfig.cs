namespace Surveillance.Auditing.DataLayer.Interfaces
{
    public interface ISystemDataLayerConfig
    {
        string SurveillanceAuroraConnectionString { get; set; }
        string OverrideMigrationsFolder { get; set; }
    }
}