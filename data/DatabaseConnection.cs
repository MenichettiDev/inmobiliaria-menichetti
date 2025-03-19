using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaApp.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Método para obtener una conexión abierta
        public MySqlConnection GetConnection()
        {
        var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}