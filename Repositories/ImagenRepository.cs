using Inmobiliaria_.Net_Core.Models;
using InmobiliariaApp.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace InmobiliariaApp.Models
{
    public class ImagenRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public ImagenRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public int Alta(Imagen imagen)
        {
            int res = -1;
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                @"INSERT INTO Imagenes (inmueble_id, url) 
                VALUES (@InmuebleId, @Url)", connection);

            command.Parameters.AddWithValue("@InmuebleId", imagen.InmuebleId);
            command.Parameters.AddWithValue("@Url", imagen.Url);

            res = command.ExecuteNonQuery();
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                @"DELETE FROM Imagenes WHERE id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);
            res = command.ExecuteNonQuery();
            return res;
        }

        public int Modificacion(Imagen imagen)
        {
            int res = -1;
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                @"UPDATE Imagenes SET url = @Url 
                WHERE id = @Id", connection);

            command.Parameters.AddWithValue("@Id", imagen.Id);
            command.Parameters.AddWithValue("@Url", imagen.Url);

            res = command.ExecuteNonQuery();
            return res;
        }

        public Imagen? ObtenerPorId(int id)
        {
            Imagen? res = null;
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                @"SELECT id, inmueble_id, url 
                FROM Imagenes 
                WHERE id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                res = new Imagen
                {
                    Id = reader.GetInt32("id"),
                    InmuebleId = reader.GetInt32("inmueble_id"),
                    Url = reader.GetString("url")
                };
            }

            return res;
        }

        public List<Imagen> ObtenerTodos()
        {
            var res = new List<Imagen>();
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                @"SELECT id, inmueble_id, url 
                FROM Imagenes", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                res.Add(new Imagen
                {
                    Id = reader.GetInt32("id"),
                    InmuebleId = reader.GetInt32("inmueble_id"),
                    Url = reader.GetString("url")
                });
            }

            return res;
        }

        public List<Imagen> BuscarPorInmueble(int inmuebleId)
        {
            var res = new List<Imagen>();
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                @"SELECT id, inmueble_id, url 
                FROM Imagenes 
                WHERE inmueble_id = @InmuebleId", connection);

            command.Parameters.AddWithValue("@InmuebleId", inmuebleId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                res.Add(new Imagen
                {
                    Id = reader.GetInt32("id"),
                    InmuebleId = reader.GetInt32("inmueble_id"),
                    Url = reader.GetString("url")
                });
            }

            return res;
        }
    }
}
