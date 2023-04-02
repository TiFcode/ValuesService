// FormulasController Get method queries the PostgreSQL database named "FormulaEngine" hosted in GCP and returns a json containing "FormulaName" and "FormulaContent" columns from table "Formulas".

using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MyApi.Controllers
{
    // Command
    public class GetFormulasCommand
    {
        public string ConnectionString { get; set; }
        public GetFormulasCommand()
        {
            ConnectionString = string.Empty;
        }
    }

    // Result
    public class GetFormulasResult
    {
        public string FormulasJson { get; set; }
        public GetFormulasResult()
        {
            FormulasJson = string.Empty;
        }
    }

    // Handler
    public class GetFormulasHandler
    {
        public GetFormulasResult Handle(GetFormulasCommand command)
        {
            using (var connection = new NpgsqlConnection(command.ConnectionString))
            {
                connection.Open();

                const string query = "SELECT \"FormulaName\", \"FormulaContent\" FROM \"Formulas\"";
                using (var sqlCmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);

                        var jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };
                        string json = JsonSerializer.Serialize(dt, jsonSerializerOptions);

                        return new GetFormulasResult { FormulasJson = json };
                    }
                }
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FormulasController : ControllerBase
    {
        private readonly string _connectionString = "Server=YOUR_SERVER_IP; Database=FormulaEngine; User Id=YOUR_USERNAME; Password=YOUR_PASSWORD; Port=5432; SSL Mode=Require;TrustServerCertificate=True";

        [HttpGet]
        public string Get()
        {
            var command = new GetFormulasCommand { ConnectionString = _connectionString };
            var handler = new GetFormulasHandler();
            var result = handler.Handle(command);
            return result.FormulasJson;
        }

    }
}