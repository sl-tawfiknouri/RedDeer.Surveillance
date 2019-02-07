using Surveillance.Systems.DataLayer.Interfaces;

namespace Surveillance.Systems.DataLayer
{
    public class SystemDataLayerConfig : ISystemDataLayerConfig
    {
        public string SurveillanceAuroraConnectionString { get; set; }
    }
}
