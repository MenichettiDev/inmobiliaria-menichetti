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

        // Obtener todos los usuarios
        public List<Usuario> GetAll()
        {
            var usuarios = new List<Usuario>();
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM usuario", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                usuarios.Add(new Usuario
                {
                    IdUsuario = reader.GetInt32("id_usuario"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password"),
                    Rol = reader.GetString("rol"),
                    FotoPerfil = reader.IsDBNull(reader.GetOrdinal("foto_archivo")) ? null : reader.GetString(reader.GetOrdinal("foto_archivo"))
                });
            }
            return usuarios;
        }

        // Obtener un usuario por ID
        public Usuario? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM usuario WHERE id_usuario = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32("id_usuario"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password"),
                    Rol = reader.GetString("rol"),
                    FotoPerfil = reader.IsDBNull(reader.GetOrdinal("foto_archivo")) ? null : reader.GetString(reader.GetOrdinal("foto_archivo"))

                };
            }
            return null;
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
                    Rol = reader.GetString("rol"),
                    FotoPerfil = reader.IsDBNull(reader.GetOrdinal("foto_archivo")) ? null : reader.GetString(reader.GetOrdinal("foto_archivo"))

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

        public void ActualizarFotoPerfil(int idUsuario, string fotoPerfilUrl)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("UPDATE usuario SET foto_archivo = @fotoPerfil WHERE id_usuario = @id", connection);
            command.Parameters.AddWithValue("@fotoPerfil", (object?)fotoPerfilUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@id", idUsuario);
            command.ExecuteNonQuery();
        }


    }
}