using System.Security.Claims;
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
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<VentaResponse>> Create([FromBody] VentaRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (request.Items == null || request.Items.Count == 0)
                return BadRequest(new { mensaje = "La venta debe tener al menos un producto." });

            var total = 0m;
            var detalles = new List<DetalleVenta>();

            foreach (var item in request.Items)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto == null || !producto.Activo)
                    return BadRequest(new { mensaje = $"Producto ID {item.ProductoId} no encontrado." });

                if (producto.Stock < item.Cantidad)
                    return BadRequest(new { mensaje = $"Stock insuficiente para \"{producto.Nombre}\"." });

                var subtotal = producto.Precio * item.Cantidad;
                total += subtotal;

                detalles.Add(new DetalleVenta
                {
                    IdProducto = producto.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio
                });

                producto.Stock -= item.Cantidad;
            }

            if (total <= 0)
                return BadRequest(new { mensaje = "La venta debe tener un total mayor a cero." });

            var venta = new Venta
            {
                IdUsuario = userId.Value,
                Total = total,
                MetodoPago = request.MetodoPago,
                Observaciones = request.Observaciones,
                FechaVenta = DateTime.UtcNow
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            foreach (var d in detalles)
            {
                d.IdVenta = venta.Id;
                _context.DetalleVenta.Add(d);
            }

            await _context.SaveChangesAsync();

            return Ok(new VentaResponse
            {
                Id = venta.Id,
                Total = venta.Total,
                MetodoPago = venta.MetodoPago,
                Observaciones = venta.Observaciones,
                FechaVenta = venta.FechaVenta
            });
        }

        [HttpGet("mis-compras")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<VentaResponse>>> GetMyPurchases()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var ventas = await _context.Ventas
                .Where(v => v.IdUsuario == userId)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return Ok(ventas.Select(ToResponse));
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<VentaResponse>>> GetAll()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return Ok(ventas.Select(ToResponse));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<VentaResponse>> GetById(int id)
        {
            var userId = GetUserId();
            var userRol = GetUserRol();

            var venta = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound();

            if (userRol != "admin" && venta.IdUsuario != userId)
                return Forbid();

            return Ok(ToResponse(venta));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound();

            foreach (var detalle in venta.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                if (producto != null)
                    producto.Stock += detalle.Cantidad;
            }

            _context.DetalleVenta.RemoveRange(venta.Detalles);
            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        private string? GetUserRol()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        private static VentaResponse ToResponse(Venta v) => new()
        {
            Id = v.Id,
            Total = v.Total,
            MetodoPago = v.MetodoPago,
            Observaciones = v.Observaciones,
            FechaVenta = v.FechaVenta,
            NombreUsuario = v.Usuario?.Nombre,
            Detalles = v.Detalles.Select(d => new DetalleVentaResponse
            {
                Id = d.Id,
                NombreProducto = d.Producto?.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Cantidad * d.PrecioUnitario
            }).ToList()
        };
    }
}
