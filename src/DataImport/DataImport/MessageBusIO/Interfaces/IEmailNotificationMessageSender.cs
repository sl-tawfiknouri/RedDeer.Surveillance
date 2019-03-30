using System.Threading.Tasks;
using Contracts.Email;

namespace DataImport.MessageBusIO.Interfaces
{
    public interface IEmailNotificationMessageSender
    {
        Task Send(SendEmailToRecipient message);
    }
}