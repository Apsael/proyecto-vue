using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "cliente";

        public bool Activo { get; set; } = true;

        public bool Verificado { get; set; } = false;

        [StringLength(100)]
        public string? TokenVerificacion { get; set; }

        public double? Latitud { get; set; }

        public double? Longitud { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
