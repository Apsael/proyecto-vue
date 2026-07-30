using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BackendApi.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _config["Smtp:FromName"] ?? "La Dolce Vita",
                _config["Smtp:FromEmail"]!
            ));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(
                _config["Smtp:Host"]!,
                int.Parse(_config["Smtp:Port"] ?? "587"),
                SecureSocketOptions.StartTls
            );
            await client.AuthenticateAsync(_config["Smtp:User"]!, _config["Smtp:Pass"]!);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string BuildVerificationEmail(string nombre, string token)
    {
        var url = $"http://localhost:5173/verificar?token={token}";
        return $@"
<h2>Bienvenido, {nombre}!</h2>
<p>Gracias por registrarte en <strong>La Dolce Vita</strong>.</p>
<p>Para verificar tu correo, haz clic en el siguiente boton:</p>
<div style='text-align:center;margin:30px 0'>
  <a href='{url}' style='background-color:#e91e63;color:white;padding:12px 30px;text-decoration:none;border-radius:5px;display:inline-block'>
    Verificar mi cuenta
  </a>
</div>
<p style='color:#999;font-size:14px'>O copia este enlace: {url}</p>";
    }

    public string BuildReceiptEmail(string nombre, int ventaId, decimal total, List<(string producto, int cantidad, decimal subtotal)> items)
    {
        var itemsHtml = string.Concat(items.Select(i =>
            $"<tr><td>{i.producto}</td><td>{i.cantidad}</td><td>Bs {i.subtotal:F2}</td></tr>"
        ));
        return $@"
<h2>Gracias por tu compra, {nombre}!</h2>
<p>Resumen de tu pedido <strong>#{ventaId}</strong>:</p>
<table style='width:100%;border-collapse:collapse;margin:20px 0'>
  <thead><tr style='background:#f5f5f5'><th style='padding:10px;text-align:left'>Producto</th><th style='padding:10px;text-align:left'>Cant.</th><th style='padding:10px;text-align:left'>Subtotal</th></tr></thead>
  <tbody>{itemsHtml}</tbody>
  <tfoot><tr style='font-weight:bold'><td colspan='2' style='padding:10px;text-align:right'>Total:</td><td style='padding:10px'>Bs {total:F2}</td></tr></tfoot>
</table>
<p>Te notificaremos cuando tu pedido este listo.</p>";
    }
}
