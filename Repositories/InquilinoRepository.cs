using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories
{
    public class InquilinoRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public InquilinoRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Método para obtener todos los inquilinos
        public List<Inquilino> GetAll()
        {
            var inquilinos = new List<Inquilino>();

            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM inquilino", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                inquilinos.Add(new Inquilino
                {
                    IdInquilino = reader.GetInt32("id_inquilino"),
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

            return inquilinos;
        }

        // Método para obtener un inquilino por ID
        public Inquilino? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM inquilino WHERE id_inquilino = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Inquilino
                {
                    IdInquilino = reader.GetInt32("id_inquilino"),
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

            return null; // Devuelve null si no se encuentra ningún inquilino
        }

        // Método para agregar un nuevo inquilino
        public void Add(Inquilino inquilino)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO inquilino (dni, apellido, nombre, telefono, email) " +
                "VALUES (@Dni, @Apellido, @Nombre, @Telefono, @Email)", connection);

            command.Parameters.AddWithValue("@Dni", inquilino.Dni);
            command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
            command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
            command.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(inquilino.Telefono) ? (object)DBNull.Value : inquilino.Telefono);
            command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(inquilino.Email) ? (object)DBNull.Value : inquilino.Email);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un inquilino
        public void Update(Inquilino inquilino)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE inquilino SET dni = @Dni, apellido = @Apellido, nombre = @Nombre, telefono = @Telefono, email = @Email " +
                "WHERE id_inquilino = @Id", connection);

            command.Parameters.AddWithValue("@Id", inquilino.IdInquilino);
            command.Parameters.AddWithValue("@Dni", inquilino.Dni);
            command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
            command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
            command.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(inquilino.Telefono) ? (object)DBNull.Value : inquilino.Telefono);
            command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(inquilino.Email) ? (object)DBNull.Value : inquilino.Email);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un inquilino
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM inquilino WHERE id_inquilino = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}