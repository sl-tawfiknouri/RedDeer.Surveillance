namespace Surveillance.Auditing.DataLayer
{
    using Surveillance.Auditing.DataLayer.Interfaces;

    public class SystemDataLayerConfig : ISystemDataLayerConfig
    {
        public string SurveillanceAuroraConnectionString { get; set; }
    }
}