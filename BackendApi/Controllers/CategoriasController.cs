using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendApi.Data;
using BackendApi.Models;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetAll()
        {
            return Ok(await _context.Categorias.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetById(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound(new { mensaje = $"Categoria con id {id} no encontrada" });
            return Ok(categoria);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Categoria>> Create([FromBody] Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido." });

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.Id)
                return BadRequest(new { mensaje = "El id no coincide." });

            var existente = await _context.Categorias.FindAsync(id);
            if (existente == null)
                return NotFound();

            existente.Nombre = categoria.Nombre;
            existente.Descripcion = categoria.Descripcion;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            var tieneProductos = await _context.Productos.AnyAsync(p => p.IdCategoria == id && p.Activo);
            if (tieneProductos)
                return BadRequest(new { mensaje = "No se puede eliminar una categoria con productos activos." });

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
