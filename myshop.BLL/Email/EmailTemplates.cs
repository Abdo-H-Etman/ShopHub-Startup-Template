namespace myshop.BLL.Email;

public static class EmailTemplates
{
    public static string Welcome(
        string recipientName)
    {
        return $"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Welcome to ShopHub</title>
        </head>

        <body style="margin:0; padding:0; background:#f4f6f8; font-family:Arial,sans-serif;">

            <div style="max-width:600px; margin:40px auto; background:white;
                        border-radius:10px; overflow:hidden;
                        box-shadow:0 2px 10px rgba(0,0,0,0.08);">

                <div style="background:#0d6efd; padding:25px; text-align:center;">
                    <h1 style="color:white; margin:0;">
                        ShopHub
                    </h1>
                </div>

                <div style="padding:35px;">

                    <h2>Welcome, {recipientName}! 👋</h2>

                    <p>
                        Thank you for creating an account with ShopHub.
                    </p>

                    <p>
                        Your account has been successfully created and
                        you can now start shopping.
                    </p>

                    <div style="text-align:center; margin:30px 0;">
                        <a href="http://localhost:5186/"
                           style="background:#0d6efd;
                                  color:white;
                                  padding:12px 25px;
                                  text-decoration:none;
                                  border-radius:5px;">
                            Start Shopping
                        </a>
                    </div>

                    <p>
                        Thanks for choosing ShopHub!
                    </p>

                    <p>
                        <strong>The ShopHub Team</strong>
                    </p>

                </div>

                <div style="background:#f8f9fa; padding:20px;
                            text-align:center; color:#777; font-size:12px;">
                    © 2026 ShopHub. All rights reserved.
                </div>

            </div>

        </body>
        </html>
        """;
    }

    public static string OrderConfirmation(
        string recipientName,
        int orderId,
        decimal orderTotal)
    {
        return $"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Order Confirmation</title>
        </head>

        <body style="margin:0; padding:0; background:#f4f6f8; font-family:Arial,sans-serif;">

            <div style="max-width:600px; margin:40px auto; background:white;
                        border-radius:10px; overflow:hidden;">

                <div style="background:#198754; padding:25px; text-align:center;">
                    <h1 style="color:white; margin:0;">
                        Order Confirmed
                    </h1>
                </div>

                <div style="padding:35px;">

                    <h2>Thank you, {recipientName}!</h2>

                    <p>
                        Your order has been successfully placed.
                    </p>

                    <div style="background:#f8f9fa;
                                padding:20px;
                                margin:25px 0;
                                border-radius:6px;">

                        <p>
                            <strong>Order Number:</strong> #{orderId}
                        </p>

                        <p>
                            <strong>Order Total:</strong>
                            {orderTotal:C}
                        </p>

                    </div>

                    <p>
                        We will process your order as soon as possible.
                    </p>

                    <p>
                        Thank you for shopping with ShopHub!
                    </p>

                    <p>
                        <strong>The ShopHub Team</strong>
                    </p>

                </div>

                <div style="background:#f8f9fa; padding:20px;
                            text-align:center; color:#777; font-size:12px;">
                    © 2026 ShopHub. All rights reserved.
                </div>

            </div>

        </body>
        </html>
        """;
    }
}