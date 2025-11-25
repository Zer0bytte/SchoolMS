using Microsoft.AspNetCore.Identity.UI.Services;

namespace SchoolMS.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
    }
}
