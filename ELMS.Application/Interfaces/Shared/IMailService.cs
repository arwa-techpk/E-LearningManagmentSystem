using ELMS.Application.DTOs.Mail;
using System.Threading.Tasks;

namespace ELMS.Application.Interfaces.Shared
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}