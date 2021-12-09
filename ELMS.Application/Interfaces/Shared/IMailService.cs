using ELMCOM.Application.DTOs.Mail;
using System.Threading.Tasks;

namespace ELMCOM.Application.Interfaces.Shared
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}