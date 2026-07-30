using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models.Dtos
{
    public class VentaItemRequest
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }

    public class VentaRequest
    {
        [Required]
        public List<VentaItemRequest> Items { get; set; } = new();

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; } = "efectivo";

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(100)]
        public string? DireccionEnvio { get; set; }

        public double? LatitudEntrega { get; set; }

        public double? LongitudEntrega { get; set; }
    }

    public class VentaResponse
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string? DireccionEnvio { get; set; }
        public string Estado { get; set; } = "pendiente";
        public double? LatitudEntrega { get; set; }
        public double? LongitudEntrega { get; set; }
        public DateTime FechaVenta { get; set; }
        public string? NombreUsuario { get; set; }
        public string? EmailUsuario { get; set; }
        public List<DetalleVentaResponse> Detalles { get; set; } = new();
    }

    public class DetalleVentaResponse
    {
        public int Id { get; set; }
        public string? NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
