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

        // Método para buscar un usuario por email
        public Usuario? GetByEmail(string email)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM usuario WHERE email = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32("id_usuario"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password"),
                    Rol = reader.GetString("rol")
                };
            }

            return null; // Devuelve null si no se encuentra el usuario
        }

        // Método para agregar un nuevo usuario
        public void Add(Usuario usuario)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO usuario (email, password, rol) " +
                "VALUES (@Email, @Password, @Rol)", connection);

            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@Password", usuario.Password);
            command.Parameters.AddWithValue("@Rol", usuario.Rol);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un usuario
        public void Update(Usuario usuario)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE usuario SET email = @Email, password = @Password, rol = @Rol " +
                "WHERE id_usuario = @Id", connection);

            command.Parameters.AddWithValue("@Id", usuario.IdUsuario);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@Password", usuario.Password);
            command.Parameters.AddWithValue("@Rol", usuario.Rol);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un usuario
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM usuario WHERE id_usuario = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}