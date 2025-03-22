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
            using var command = new MySqlCommand("SELECT * FROM Inquilino", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                inquilinos.Add(new Inquilino
                {
                    IdInquilino = reader.GetInt32("id_inquilino"),
                    Dni = reader.GetString("Dni"),
                    Apellido = reader.GetString("Apellido"),
                    Nombre = reader.GetString("Nombre"),
                    Telefono = reader.GetString("Telefono"),
                    Email = reader.GetString("Email")
                });
            }

            return inquilinos;
        }

        // Método para obtener un inquilino por ID
        public Inquilino? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM Inquilino WHERE id_inquilino = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Inquilino
                {
                    IdInquilino = reader.GetInt32("id_inquilino"),
                    Dni = reader.GetString("Dni"),
                    Apellido = reader.GetString("Apellido"),
                    Nombre = reader.GetString("Nombre"),
                    Telefono = reader.GetString("Telefono"),
                    Email = reader.GetString("Email")
                };
            }

            return null; // Devuelve null si no se encuentra ningún inquilino
        }

        // Método para agregar un nuevo inquilino
        public void Add(Inquilino inquilino)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO Inquilino (Dni, Apellido, Nombre, Telefono, Email) " +
                "VALUES (@Dni, @Apellido, @Nombre, @Telefono, @Email)", connection);

            command.Parameters.AddWithValue("@Dni", inquilino.Dni);
            command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
            command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
            command.Parameters.AddWithValue("@Telefono", inquilino.Telefono);
            command.Parameters.AddWithValue("@Email", inquilino.Email);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un inquilino
        public void Update(Inquilino inquilino)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE Inquilino SET Dni = @Dni, Apellido = @Apellido, Nombre = @Nombre, Telefono = @Telefono, Email = @Email " +
                "WHERE id_inquilino = @Id", connection);

            command.Parameters.AddWithValue("@Id", inquilino.IdInquilino);
            command.Parameters.AddWithValue("@Dni", inquilino.Dni);
            command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
            command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
            command.Parameters.AddWithValue("@Telefono", inquilino.Telefono);
            command.Parameters.AddWithValue("@Email", inquilino.Email);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un inquilino
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM Inquilino WHERE id_inquilino = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}