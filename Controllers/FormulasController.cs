// FormulasController Get method queries the PostgreSQL database named "FormulaEngine" hosted in GCP and returns a json containing "FormulaName" and "FormulaContent" columns from table "Formulas".
using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MyApi.Controllers
{
    // Model
    public class Formula
    {
        public Formula(
            string formulaName,
            string formulaContent)
        {
            FormulaName = formulaName;
            FormulaContent = formulaContent;
        }

        public string FormulaName { get; set; }
        public string FormulaContent { get; set; }
    }

    // Repository
    public class FormulasRepository
    {
        private readonly string _connectionString;

        public FormulasRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetFormulas()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                const string query = "SELECT \"FormulaName\", \"FormulaContent\" FROM \"Formulas\"";

                using (var sqlCmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        return dt;
                    }
                }
            }
        }
    }

    // Services
    public class FormulasService
    {
        private readonly FormulasRepository _formulasRepository;

        public FormulasService(FormulasRepository formulasRepository)
        {
            _formulasRepository = formulasRepository;
        }

        public string GetFormulasAsJson()
        {
            var dt = _formulasRepository.GetFormulas();
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(dt, jsonSerializerOptions);
            return json;
        }
    }

    // Controllers
    [Route("api/[controller]")]
    [ApiController]
    public class FormulasController : ControllerBase
    {
        private readonly string _connectionString = "Server=YOUR_SERVER_IP; Database=FormulaEngine; User Id=YOUR_USERNAME; Password=YOUR_PASSWORD; Port=5432; SSL Mode=Require;TrustServerCertificate=True";

        [HttpGet]
        public string Get()
        {
            var formulasRepository = new FormulasRepository(_connectionString);
            var formulasService = new FormulasService(formulasRepository);
            string formulasJson = formulasService.GetFormulasAsJson();
            return formulasJson;
        }
    }
}