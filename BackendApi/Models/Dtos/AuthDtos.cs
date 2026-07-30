using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models.Dtos
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "La contraseña debe tener mayúsculas, minúsculas, números y caracteres especiales")]
        public string Password { get; set; } = string.Empty;

        public double? Latitud { get; set; }

        public double? Longitud { get; set; }
    }

    public class AuthResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
    }

    public class UpdateProfileRequest
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UpdateUsuarioRequest
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string Rol { get; set; } = "cliente";

        public bool Activo { get; set; } = true;
    }

    public class ReenviarVerificacionRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateLocationRequest
    {
        [Required]
        [Range(-90, 90)]
        public double Latitud { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitud { get; set; }
    }

    public class VerificarEmailRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }

    public class AdminCreateUsuarioRequest
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(20)]
        public string Rol { get; set; } = "cliente";
    }
}
