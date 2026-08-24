namespace myshop.BLL.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(
        string recipientEmail,
        string recipientName);

    Task SendOrderConfirmationEmailAsync(
        string recipientEmail,
        string recipientName,
        int orderId,
        decimal orderTotal);
}