using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdVenta { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;

        public Venta? Venta { get; set; }
        public Producto? Producto { get; set; }
    }
}
