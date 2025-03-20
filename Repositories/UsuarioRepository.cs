using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories
{
    public class UsuarioRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public UsuarioRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Método para buscar un usuario por nombre de usuario
        public Usuario? GetByNombreUsuario(string nombreUsuario)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM usuario WHERE username = @NombreUsuario", connection);
            command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32("id_usuario"),
                    NombreUsuario = reader.GetString("username"),
                    Password = reader.GetString("password"),
                    Rol = reader.GetString("rol")
                };
            }

            return null; // Devuelve null si no se encuentra el usuario
        }
    }
}