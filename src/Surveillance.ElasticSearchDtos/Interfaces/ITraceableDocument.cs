namespace Surveillance.ElasticSearchDtos.Interfaces
{
    public interface ITraceableDocument
    {
        /// <summary>
        /// Origin for the document i.e. machine name - service
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        string Origin { get; set; }
    }
}
