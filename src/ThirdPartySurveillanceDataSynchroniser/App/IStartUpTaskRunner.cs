namespace DataSynchroniser.App
{
    using System.Threading.Tasks;

    public interface IStartUpTaskRunner
    {
        Task Run();
    }
}