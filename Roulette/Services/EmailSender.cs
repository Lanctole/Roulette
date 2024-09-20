using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Roulette.Models;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;

namespace Roulette.Services;

/// <summary>
/// Реализует функционал отправки электронных писем через SMTP сервер.
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailSender> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="EmailSender"/>.
    /// </summary>
    /// <param name="smtpSettings">Настройки SMTP-сервера.</param>
    /// <param name="logger">Логгер для записи информации и ошибок.</param>
    public EmailSender(IOptions<SmtpSettings> smtpSettings, ILogger<EmailSender> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Отправляет электронное письмо.
    /// </summary>
    /// <param name="email">Адрес электронной почты получателя.</param>
    /// <param name="subject">Тема письма.</param>
    /// <param name="htmlMessage">Содержание письма в формате HTML.</param>
    /// <returns>Задача, представляющая асинхронную операцию отправки письма.</returns>
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            _logger.LogInformation("Начало отправки письма на {Email} с темой {Subject}", email, subject);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("admin@media-random.ru", _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlMessage };
            _logger.LogInformation("Message создано");

            using var client = new SmtpClient();
            _logger.LogInformation("Клиент создан");
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.None);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);

            _logger.LogInformation("Отправка письма...");
            await client.SendAsync(message);
            _logger.LogInformation("Письмо успешно отправлено на {Email}", email);

            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке письма на {Email}", email);
            throw new InvalidOperationException("Не удалось отправить письмо.", ex);
        }
    }
}