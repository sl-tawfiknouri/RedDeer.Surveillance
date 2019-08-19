namespace Surveillance.Engine.Rules.Analytics.Streams.Factory.Interfaces
{
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

    public interface IUniverseAlertStreamFactory
    {
        IUniverseAlertStream Build();
    }
}