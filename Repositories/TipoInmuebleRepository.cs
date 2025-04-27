using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories
{
    public class TipoInmuebleRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public TipoInmuebleRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public List<TipoInmueble> GetAll()
        {
            var tipos = new List<TipoInmueble>();

            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM tipo_inmueble where activo = 'Activo'", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tipos.Add(new TipoInmueble
                {
                    IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                    Nombre = reader.GetString("nombre")
                });
            }

            return tipos;
        }

        public TipoInmueble? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM tipo_inmueble WHERE id_tipo_inmueble = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new TipoInmueble
                {
                    IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                    Nombre = reader.GetString("nombre")
                };
            }

            return null;
        }

        public void Add(TipoInmueble tipo)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("INSERT INTO tipo_inmueble (nombre) VALUES (@nombre)", connection);
            command.Parameters.AddWithValue("@nombre", tipo.Nombre);

            command.ExecuteNonQuery();
        }

        public void Update(TipoInmueble tipo)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("UPDATE tipo_inmueble SET nombre = @nombre WHERE id_tipo_inmueble = @id", connection);
            command.Parameters.AddWithValue("@id", tipo.IdTipoInmueble);
            command.Parameters.AddWithValue("@nombre", tipo.Nombre);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("UPDATE tipo_inmueble SET activo = 'Inactivo' WHERE id_tipo_inmueble = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
}
