using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendApi.Services;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MailController : ControllerBase
{
    private readonly EmailService _emailService;

    public MailController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendMailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.To) || !request.To.Contains('@'))
            return BadRequest(new { success = false, message = "Email invalido" });

        if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body))
            return BadRequest(new { success = false, message = "Faltan parametros: subject, body" });

        var result = await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);

        if (result)
            return Ok(new { success = true, message = "Correo enviado correctamente" });

        return StatusCode(500, new { success = false, message = "Error al enviar el correo" });
    }
}

public class SendMailRequest
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
