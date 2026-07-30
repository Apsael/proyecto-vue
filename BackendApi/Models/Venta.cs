using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; } = "efectivo";

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "pendiente";

        [StringLength(100)]
        public string? DireccionEnvio { get; set; }

        public double? LatitudEntrega { get; set; }

        public double? LongitudEntrega { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        public Usuario? Usuario { get; set; }
        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
