namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IAggregation
    {
        int Count { get; }

        string Key { get; }
    }
}