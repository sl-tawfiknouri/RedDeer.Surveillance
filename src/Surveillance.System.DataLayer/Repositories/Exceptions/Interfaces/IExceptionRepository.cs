namespace Surveillance.System.DataLayer.Repositories.Exceptions.Interfaces
{
    public interface IExceptionRepository
    {
        void Save(ExceptionDto dto);
    }
}