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
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoResponse>>> GetAll()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo)
                .Select(p => ToResponse(p))
                .ToListAsync();
            return Ok(productos);
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<ProductoResponse>>> GetAllIncludingInactive()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Select(p => ToResponse(p))
                .ToListAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoResponse>> GetById(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound(new { mensaje = $"Producto con id {id} no encontrado" });

            return Ok(ToResponse(producto));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProductoResponse>> Create([FromBody] ProductoRequest request)
        {
            var producto = new Producto
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Stock = request.Stock,
                IdCategoria = request.IdCategoria,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            await _context.Entry(producto).Reference(p => p.Categoria).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, ToResponse(producto));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoRequest request)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound(new { mensaje = $"Producto con id {id} no encontrado" });

            producto.Nombre = request.Nombre;
            producto.Descripcion = request.Descripcion;
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;
            producto.IdCategoria = request.IdCategoria;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound(new { mensaje = $"Producto con id {id} no encontrado" });

            producto.Activo = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ProductoResponse>>> Buscar([FromQuery] string nombre)
        {
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo);

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(p => p.Nombre.Contains(nombre));

            var productos = await query.Select(p => ToResponse(p)).ToListAsync();
            return Ok(productos);
        }

        private static ProductoResponse ToResponse(Producto p) => new()
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            Stock = p.Stock,
            IdCategoria = p.IdCategoria,
            NombreCategoria = p.Categoria?.Nombre,
            Activo = p.Activo,
            FechaCreacion = p.FechaCreacion
        };
    }
}
