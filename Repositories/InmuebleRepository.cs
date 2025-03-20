using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories
{
    public class InmuebleRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public InmuebleRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Método para obtener todos los inmuebles
        public List<Inmueble> GetAll()
        {
            var inmuebles = new List<Inmueble>();

            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM Inmueble", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                inmuebles.Add(new Inmueble
                {
                    IdInmueble = reader.GetInt32("Id"),
                    IdPropietario = reader.GetInt32("PropietarioId"),
                    Direccion = reader.GetString("Direccion"),
                    Uso = reader.GetString("Uso"),
                    Tipo = reader.GetString("Tipo"),
                    Ambientes = reader.GetInt32("Ambientes"),
                    Precio = reader.GetDecimal("Precio"),
                    Estado = reader.GetBoolean("Disponible")
                });
            }

            return inmuebles;
        }

        // Método para obtener un inmueble por ID
        public Inmueble? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM Inmueble WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Inmueble
                {
                    IdInmueble = reader.GetInt32("Id"),
                    IdPropietario = reader.GetInt32("PropietarioId"),
                    Direccion = reader.GetString("Direccion"),
                    Uso = reader.GetString("Uso"),
                    Tipo = reader.GetString("Tipo"),
                    Ambientes = reader.GetInt32("Ambientes"),
                    Precio = reader.GetDecimal("Precio"),
                    Estado = reader.GetBoolean("Disponible")
                };
            }

            return null; // Devuelve null si no se encuentra ningún inmueble
        }

        // Método para agregar un nuevo inmueble
        public void Add(Inmueble inmueble)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO Inmueble (PropietarioId, Direccion, Uso, Tipo, Ambientes, Precio, Disponible) " +
                "VALUES (@PropietarioId, @Direccion, @Uso, @Tipo, @Ambientes, @Precio, @Disponible)", connection);

            command.Parameters.AddWithValue("@PropietarioId", inmueble.IdPropietario);
            command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@Uso", inmueble.Uso);
            command.Parameters.AddWithValue("@Tipo", inmueble.Tipo);
            command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
            command.Parameters.AddWithValue("@Precio", inmueble.Precio);
            command.Parameters.AddWithValue("@Disponible", inmueble.Estado);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un inmueble
        public void Update(Inmueble inmueble)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE Inmueble SET PropietarioId = @PropietarioId, Direccion = @Direccion, Uso = @Uso, Tipo = @Tipo, " +
                "Ambientes = @Ambientes, Precio = @Precio, Disponible = @Disponible WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", inmueble.IdInmueble);
            command.Parameters.AddWithValue("@PropietarioId", inmueble.IdPropietario);
            command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@Uso", inmueble.Uso);
            command.Parameters.AddWithValue("@Tipo", inmueble.Tipo);
            command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
            command.Parameters.AddWithValue("@Precio", inmueble.Precio);
            command.Parameters.AddWithValue("@Disponible", inmueble.Estado);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un inmueble
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM Inmueble WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}