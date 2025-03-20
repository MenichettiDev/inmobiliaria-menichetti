using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaApp.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString; // No nullable

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
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