namespace BackendApi.Models.Dtos
{
    public class EmpresaConfigResponse
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Horario { get; set; } = string.Empty;
    }

    public class UpdateEmpresaConfigRequest
    {
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Horario { get; set; }
    }
}
