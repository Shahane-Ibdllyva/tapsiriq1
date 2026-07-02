using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IEmailTemplateService
    {
        string GetWelcomeEmail(string fullName, string username);
        string GetPasswordResetEmail(string fullName, string resetLink);
    }
}