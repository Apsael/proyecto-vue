using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models.Dtos
{
    public class ProductoRequest
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [StringLength(500)]
        public string? ImagenUrl { get; set; }

        public int IdCategoria { get; set; }
    }

    public class ProductoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
        public int IdCategoria { get; set; }
        public string? NombreCategoria { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
