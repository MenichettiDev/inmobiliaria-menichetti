using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;

namespace InmobiliariaApp.Controllers
{
    public class TestController : Controller
    {
        private readonly DatabaseConnection _dbConnection;

        public TestController(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IActionResult Index()
        {
            try
            {
                using var connection = _dbConnection.GetConnection();
                ViewBag.Message = "Conexión exitosa a la base de datos.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al conectar a la base de datos: {ex.Message}";
            }

            return View();
        }
    }
}