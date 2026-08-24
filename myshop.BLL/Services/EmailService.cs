using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using myshop.BLL.Email;
using myshop.BLL.Interfaces;

namespace myshop.BLL.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendWelcomeEmailAsync(
        string recipientEmail,
        string recipientName)
    {
        var html = EmailTemplates.Welcome(recipientName);

        await SendEmailAsync(
            recipientEmail,
            recipientName,
            "Welcome to ShopHub!",
            html);
    }

    public async Task SendOrderConfirmationEmailAsync(
        string recipientEmail,
        string recipientName,
        int orderId,
        decimal orderTotal)
    {
        var html = EmailTemplates.OrderConfirmation(
            recipientName,
            orderId,
            orderTotal);

        await SendEmailAsync(
            recipientEmail,
            recipientName,
            $"Order Confirmation #{orderId}",
            html);
    }

    private async Task SendEmailAsync(
        string recipientEmail,
        string recipientName,
        string subject,
        string htmlBody)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

        message.To.Add(
            new MailboxAddress(
                recipientName,
                recipientEmail));

        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlBody
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _settings.Username,
            _settings.Password);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}