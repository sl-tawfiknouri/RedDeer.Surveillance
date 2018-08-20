namespace TestHarness.Network_IO
{
    public interface IWebsocketFactory
    {
       IWebsocket Build(string connection);
    }
}