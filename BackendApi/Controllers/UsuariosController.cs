using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendApi.Data;
using BackendApi.Models;
using BackendApi.Models.Dtos;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Email,
                    u.Rol,
                    u.Activo,
                    u.FechaCreacion
                })
                .ToListAsync();
            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] AdminCreateUsuarioRequest request)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { mensaje = "Ya existe una cuenta con ese correo." });

            var user = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Rol = request.Rol,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new
            {
                user.Id,
                user.Nombre,
                user.Email,
                user.Rol,
                user.Activo,
                user.FechaCreacion
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioRequest request)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
                return NotFound(new { mensaje = $"Usuario con id {id} no encontrado" });

            if (user.Email != request.Email &&
                await _context.Usuarios.AnyAsync(u => u.Email == request.Email && u.Id != id))
                return BadRequest(new { mensaje = "Ya existe otra cuenta con ese correo." });

            user.Nombre = request.Nombre;
            user.Email = request.Email;
            user.Rol = request.Rol;
            user.Activo = request.Activo;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
                return NotFound();

            if (user.Rol == "admin")
            {
                var adminCount = await _context.Usuarios.CountAsync(u => u.Rol == "admin" && u.Activo);
                if (adminCount <= 1)
                    return BadRequest(new { mensaje = "No se puede eliminar el ultimo administrador." });
            }

            user.Activo = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
