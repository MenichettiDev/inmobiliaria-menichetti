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
            using var command = new MySqlCommand("SELECT * FROM Propietario", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                propietarios.Add(new Propietario
                {
                    IdPropietario = reader.GetInt32("Id"),
                    Dni = reader.GetString("Dni"),
                    Apellido = reader.GetString("Apellido"),
                    Nombre = reader.GetString("Nombre"),
                    Telefono = reader.GetString("Telefono"),
                    Email = reader.GetString("Email")
                });
            }

            return propietarios;
        }

        // Método para obtener un propietario por ID
        public Propietario? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM Propietario WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Propietario
                {
                    IdPropietario = reader.GetInt32("Id"),
                    Dni = reader.GetString("Dni"),
                    Apellido = reader.GetString("Apellido"),
                    Nombre = reader.GetString("Nombre"),
                    Telefono = reader.GetString("Telefono"),
                    Email = reader.GetString("Email")
                };
            }

            return null;
        }

        // Método para agregar un nuevo propietario
        public void Add(Propietario propietario)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO Propietario (Dni, Apellido, Nombre, Telefono, Email) " +
                "VALUES (@Dni, @Apellido, @Nombre, @Telefono, @Email)", connection);

            command.Parameters.AddWithValue("@Dni", propietario.Dni);
            command.Parameters.AddWithValue("@Apellido", propietario.Apellido);
            command.Parameters.AddWithValue("@Nombre", propietario.Nombre);
            command.Parameters.AddWithValue("@Telefono", propietario.Telefono);
            command.Parameters.AddWithValue("@Email", propietario.Email);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un propietario
        public void Update(Propietario propietario)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE Propietario SET Dni = @Dni, Apellido = @Apellido, Nombre = @Nombre, Telefono = @Telefono, Email = @Email " +
                "WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", propietario.IdPropietario);
            command.Parameters.AddWithValue("@Dni", propietario.Dni);
            command.Parameters.AddWithValue("@Apellido", propietario.Apellido);
            command.Parameters.AddWithValue("@Nombre", propietario.Nombre);
            command.Parameters.AddWithValue("@Telefono", propietario.Telefono);
            command.Parameters.AddWithValue("@Email", propietario.Email);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un propietario
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM Propietario WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}