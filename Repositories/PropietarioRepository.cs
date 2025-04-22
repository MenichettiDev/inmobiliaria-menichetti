using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories
{
    public class PropietarioRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public PropietarioRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Método para obtener todos los propietarios
        public List<Propietario> GetAll()
        {
            var propietarios = new List<Propietario>();

            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM propietario", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                propietarios.Add(new Propietario
                {
                    IdPropietario = reader.GetInt32("id_propietario"),
                    Dni = reader.GetString("dni"),
                    Apellido = reader.GetString("apellido"),
                    Nombre = reader.GetString("nombre"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono"))
                        ? null
                        : reader.GetString("telefono"),
                    Email = reader.IsDBNull(reader.GetOrdinal("email"))
                        ? null
                        : reader.GetString("email")
                });
            }

            return propietarios;
        }

        //Paginado
        public List<Propietario> Buscar(string dni, string apellido, string nombre, int offset, int limit)
        {
            var propietarios = new List<Propietario>();
            using var connection = _dbConnection.GetConnection();

            var filtros = new List<string>();
            if (!string.IsNullOrEmpty(dni)) filtros.Add("dni LIKE @dni");
            if (!string.IsNullOrEmpty(apellido)) filtros.Add("apellido LIKE @apellido");
            if (!string.IsNullOrEmpty(nombre)) filtros.Add("nombre LIKE @nombre");

            string whereClause = filtros.Count > 0 ? "WHERE " + string.Join(" AND ", filtros) : "";

            string query = $"SELECT * FROM propietario {whereClause} LIMIT @limit OFFSET @offset";
            using var command = new MySqlCommand(query, connection);

            if (!string.IsNullOrEmpty(dni)) command.Parameters.AddWithValue("@dni", $"%{dni}%");
            if (!string.IsNullOrEmpty(apellido)) command.Parameters.AddWithValue("@apellido", $"%{apellido}%");
            if (!string.IsNullOrEmpty(nombre)) command.Parameters.AddWithValue("@nombre", $"%{nombre}%");

            command.Parameters.AddWithValue("@limit", limit);
            command.Parameters.AddWithValue("@offset", offset);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                propietarios.Add(new Propietario
                {
                    IdPropietario = reader.GetInt32("id_propietario"),
                    Dni = reader.GetString("dni"),
                    Apellido = reader.GetString("apellido"),
                    Nombre = reader.GetString("nombre"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email")
                });
            }

            return propietarios;
        }


        public int Contar(string dni, string apellido, string nombre)
        {
            using var connection = _dbConnection.GetConnection();

            var filtros = new List<string>();
            if (!string.IsNullOrEmpty(dni)) filtros.Add("dni LIKE @dni");
            if (!string.IsNullOrEmpty(apellido)) filtros.Add("apellido LIKE @apellido");
            if (!string.IsNullOrEmpty(nombre)) filtros.Add("nombre LIKE @nombre");

            string whereClause = filtros.Count > 0 ? "WHERE " + string.Join(" AND ", filtros) : "";

            string query = $"SELECT COUNT(*) FROM propietario {whereClause}";
            using var command = new MySqlCommand(query, connection);

            if (!string.IsNullOrEmpty(dni)) command.Parameters.AddWithValue("@dni", $"%{dni}%");
            if (!string.IsNullOrEmpty(apellido)) command.Parameters.AddWithValue("@apellido", $"%{apellido}%");
            if (!string.IsNullOrEmpty(nombre)) command.Parameters.AddWithValue("@nombre", $"%{nombre}%");

            return Convert.ToInt32(command.ExecuteScalar());
        }



        // Método para obtener un propietario por ID
        public Propietario? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM propietario WHERE id_propietario = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Propietario
                {
                    IdPropietario = reader.GetInt32("id_propietario"),
                    Dni = reader.GetString("dni"),
                    Apellido = reader.GetString("apellido"),
                    Nombre = reader.GetString("nombre"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono"))
                        ? null
                        : reader.GetString("telefono"),
                    Email = reader.IsDBNull(reader.GetOrdinal("email"))
                        ? null
                        : reader.GetString("email")
                };
            }

            return null; // Devuelve null si no se encuentra ningún propietario
        }

        // Método para agregar un nuevo propietario
        public void Add(Propietario propietario)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO propietario (dni, apellido, nombre, telefono, email) " +
                "VALUES (@Dni, @Apellido, @Nombre, @Telefono, @Email)", connection);

            command.Parameters.AddWithValue("@Dni", propietario.Dni);
            command.Parameters.AddWithValue("@Apellido", propietario.Apellido);
            command.Parameters.AddWithValue("@Nombre", propietario.Nombre);
            command.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(propietario.Telefono) ? (object)DBNull.Value : propietario.Telefono);
            command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(propietario.Email) ? (object)DBNull.Value : propietario.Email);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un propietario
        public void Update(Propietario propietario)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE propietario SET dni = @Dni, apellido = @Apellido, nombre = @Nombre, telefono = @Telefono, email = @Email " +
                "WHERE id_propietario = @Id", connection);

            command.Parameters.AddWithValue("@Id", propietario.IdPropietario);
            command.Parameters.AddWithValue("@Dni", propietario.Dni);
            command.Parameters.AddWithValue("@Apellido", propietario.Apellido);
            command.Parameters.AddWithValue("@Nombre", propietario.Nombre);
            command.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(propietario.Telefono) ? (object)DBNull.Value : propietario.Telefono);
            command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(propietario.Email) ? (object)DBNull.Value : propietario.Email);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un propietario
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM propietario WHERE id_propietario = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}