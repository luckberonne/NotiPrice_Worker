using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotiPrice_Worker.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string message, CancellationToken cancellationToken);
    }
}
