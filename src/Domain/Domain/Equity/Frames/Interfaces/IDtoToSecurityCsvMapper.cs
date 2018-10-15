namespace Domain.Equity.Frames.Interfaces
{
    public interface IDtoToSecurityCsvMapper
    {
        int FailedMapTotal { get; set; }
        SecurityTickCsv Map(SecurityTick securityTick);
    }
}