using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Application.Interfaces;
using Infrastructure.Settings;
using Polly;


namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IAsyncPolicy _retryPolicy;

        public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;

            // ⚡ Ekspert səviyyəsi: Retry Policy (Exponential Backoff)
            _retryPolicy = Policy
                .Handle<SmtpException>()       // SMTP xətaları (5xx, 4xx)
                .Or<TimeoutException>()        // Zaman aşımı
                .WaitAndRetryAsync(
                    3, // Maksimum 3 dəfə təkrar cəhd
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2, 4, 8 saniyə
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        // Hər təkrar cəhddə log yaz
                        _logger.LogWarning(
                            exception,
                            "⚠️ Mail göndərilmədi ({RetryCount}). {TimeSpan} sonra yenidən cəhd edilir. Xəta: {ErrorMessage}",
                            retryCount, timeSpan, exception.Message
                        );
                    }
                );
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            // Retry policy daxilində göndərmə əməliyyatını icra et
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 15000 // 15 saniyə
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml,
                };
                mailMessage.To.Add(to);

                _logger.LogInformation($"📨 Mail göndərilir: {to} - {subject}");
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"✅ Mail uğurla göndərildi: {to}");
            });
        }
    }
}