using Surveillance.Auditing.DataLayer.Interfaces;

namespace Surveillance.Auditing.DataLayer
{
    public class SystemDataLayerConfig : ISystemDataLayerConfig
    {
        public string SurveillanceAuroraConnectionString { get; set; }
    }
}
