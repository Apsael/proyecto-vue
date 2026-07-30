using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendApi.Models.Dtos;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _configFilePath;

        public ConfigController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _configFilePath = Path.Combine(env.ContentRootPath, "empresa.json");
        }

        [HttpGet("empresa")]
        public ActionResult<EmpresaConfigResponse> GetEmpresaConfig()
        {
            return Ok(LoadConfig());
        }

        [Authorize(Roles = "admin")]
        [HttpPut("empresa")]
        public ActionResult<EmpresaConfigResponse> UpdateEmpresaConfig([FromBody] UpdateEmpresaConfigRequest request)
        {
            var config = LoadConfig();
            config.Latitud = request.Latitud;
            config.Longitud = request.Longitud;

            if (!string.IsNullOrWhiteSpace(request.Nombre))
                config.Nombre = request.Nombre;
            if (!string.IsNullOrWhiteSpace(request.Direccion))
                config.Direccion = request.Direccion;
            if (!string.IsNullOrWhiteSpace(request.Telefono))
                config.Telefono = request.Telefono;
            if (!string.IsNullOrWhiteSpace(request.Email))
                config.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Horario))
                config.Horario = request.Horario;

            SaveConfig(config);
            return Ok(config);
        }

        private EmpresaConfigResponse LoadConfig()
        {
            if (System.IO.File.Exists(_configFilePath))
            {
                var json = System.IO.File.ReadAllText(_configFilePath);
                var saved = JsonSerializer.Deserialize<EmpresaConfigResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (saved != null) return saved;
            }

            return new EmpresaConfigResponse
            {
                Nombre = "Heladería La Dolce Vita",
                Direccion = "Calle Beni #123, Santa Cruz, Bolivia",
                Telefono = "+591 7000 1234",
                Email = "info@ladolcevita.com",
                Latitud = _config.GetValue<double>("Empresa:Latitud", -17.7853),
                Longitud = _config.GetValue<double>("Empresa:Longitud", -63.1806),
                Horario = "Lun - Dom: 10:00 AM - 10:00 PM"
            };
        }

        private void SaveConfig(EmpresaConfigResponse config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            System.IO.File.WriteAllText(_configFilePath, json);
        }
    }
}
