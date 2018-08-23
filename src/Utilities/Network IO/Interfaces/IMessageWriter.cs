namespace Utilities.Network_IO.Interfaces
{
    /// <summary>
    /// Writes message to chosen output format
    /// </summary>
    public interface IMessageWriter
    {
        void Write(string message);
    }
}
