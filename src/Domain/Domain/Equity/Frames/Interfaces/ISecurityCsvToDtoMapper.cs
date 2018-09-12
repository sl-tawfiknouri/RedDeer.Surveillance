namespace Domain.Equity.Frames.Interfaces
{
    public interface ISecurityCsvToDtoMapper
    {
        int FailedParseTotal { get; set; }
        SecurityTick Map(SecurityTickCsv csv);
    }
}