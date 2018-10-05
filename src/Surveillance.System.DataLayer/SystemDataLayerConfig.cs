using Surveillance.System.DataLayer.Interfaces;

namespace Surveillance.System.DataLayer
{
    public class SystemDataLayerConfig : ISystemDataLayerConfig
    {
        public string SurveillanceAuroraConnectionString { get; set; }
    }
}
