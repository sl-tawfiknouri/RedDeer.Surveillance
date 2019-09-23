namespace DataImport.MessageBusIO.Interfaces
{
    using System.Threading.Tasks;

    using Contracts.Email;

    public interface IEmailNotificationMessageSender
    {
        Task Send(SendSimpleEmailToRecipient message);
    }
}