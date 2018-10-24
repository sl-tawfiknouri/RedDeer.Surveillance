namespace Surveillance.DataLayer.Aurora.Exceptions.Interfaces
{
    public interface IExceptionRepository
    {
        void Save(ExceptionDto dto);
    }
}