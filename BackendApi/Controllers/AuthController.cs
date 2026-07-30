using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BackendApi.Data;
using BackendApi.Models;
using BackendApi.Models.Dtos;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { mensaje = "Ya existe una cuenta con ese correo." });

            var token = Guid.NewGuid().ToString("N");

            var user = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Rol = "cliente",
                Activo = true,
                Verificado = false,
                TokenVerificacion = token,
                Latitud = request.Latitud,
                Longitud = request.Longitud,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                Verificado = false,
                Token = user.TokenVerificacion!
            });
        }

        [HttpPost("verificar")]
        public async Task<IActionResult> VerificarEmail([FromBody] VerificarEmailRequest request)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenVerificacion == request.Token);
            if (user == null)
                return BadRequest(new { mensaje = "Token invalido." });

            user.Verificado = true;
            user.TokenVerificacion = null;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Email verificado correctamente." });
        }

        [HttpPost("reenviar-verificacion")]
        public async Task<IActionResult> ReenviarVerificacion([FromBody] ReenviarVerificacionRequest request)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest(new { mensaje = "Usuario no encontrado." });

            if (user.Verificado)
                return BadRequest(new { mensaje = "El email ya esta verificado." });

            user.TokenVerificacion = Guid.NewGuid().ToString("N");
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Token regenerado.", token = user.TokenVerificacion });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !user.Activo || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { mensaje = "Correo o contrasena incorrectos." });

            if (!user.Verificado && user.Rol != "admin")
                return Unauthorized(new { mensaje = "Debes verificar tu correo antes de iniciar sesion. Revisa tu bandeja de entrada." });

            return Ok(new AuthResponse
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                Verificado = user.Verificado,
                Latitud = user.Latitud,
                Longitud = user.Longitud,
                Token = GenerateToken(user)
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<AuthResponse>> Me()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null || !user.Activo) return Unauthorized();

            return Ok(new AuthResponse
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                Verificado = user.Verificado,
                Latitud = user.Latitud,
                Longitud = user.Longitud,
                Token = ""
            });
        }

        [HttpPut("perfil")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null) return NotFound();

            if (user.Email != request.Email &&
                await _context.Usuarios.AnyAsync(u => u.Email == request.Email && u.Id != userId))
                return BadRequest(new { mensaje = "Ya existe otra cuenta con ese correo." });

            user.Nombre = request.Nombre;
            user.Email = request.Email;
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                Verificado = user.Verificado,
                Latitud = user.Latitud,
                Longitud = user.Longitud,
                Token = GenerateToken(user)
            });
        }

        [HttpPut("ubicacion")]
        [Authorize]
        public async Task<IActionResult> UpdateUbicacion([FromBody] UpdateLocationRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null) return NotFound();

            user.Latitud = request.Latitud;
            user.Longitud = request.Longitud;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Ubicacion actualizada correctamente." });
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return BadRequest(new { mensaje = "La contrasena actual es incorrecta." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Contrasena actualizada correctamente." });
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        private string GenerateToken(Usuario user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol),
                new Claim(ClaimTypes.Name, user.Nombre)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
