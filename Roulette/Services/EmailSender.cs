using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Roulette.Models;

namespace Roulette.Services;

/// <summary>
/// Реализует функционал отправки электронных писем через SMTP сервер.
/// </summary>
public class EmailSender: IEmailSender
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
            var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке письма.");
            throw new InvalidOperationException("Не удалось отправить письмо.", ex);
        }
    }
}