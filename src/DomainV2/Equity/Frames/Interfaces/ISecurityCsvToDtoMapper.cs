namespace DomainV2.Equity.Frames.Interfaces
{
    public interface ISecurityCsvToDtoMapper
    {
        int FailedParseTotal { get; set; }
        SecurityTick Map(SecurityTickCsv csv);
    }
}