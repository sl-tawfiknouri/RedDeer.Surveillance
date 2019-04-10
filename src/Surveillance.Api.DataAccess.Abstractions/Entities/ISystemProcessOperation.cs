namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface ISystemProcessOperation
    {
        int Id { get; set; }
        int OperationState { get; set; }
        string SystemProcessId { get; set; }

        string Start { get; }
        string End { get; }
    }
}