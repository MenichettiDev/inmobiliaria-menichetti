using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;
using System;

namespace InmobiliariaApp.Repositories
{
    public class ContratoRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public ContratoRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Método para obtener todos los contratos
        public List<Contrato> GetAll()
        {
            var contratos = new List<Contrato>();

            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM Contrato", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                contratos.Add(new Contrato
                {
                    IdContrato = reader.GetInt32("Id"),
                    IdInquilino = reader.GetInt32("IdInquilino"),
                    IdInmueble = reader.GetInt32("IdInmueble"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    MontoMensual = reader.GetDecimal("MontoMensual"),
                    Vigente = reader.GetBoolean("Vigente")
                });
            }

            return contratos;
        }

        // Método para obtener un contrato por ID
        public Contrato? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM Contrato WHERE id_contrato = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Contrato
                {
                    IdContrato = reader.GetInt32("Id"),
                    IdInquilino = reader.GetInt32("IdInquilino"),
                    IdInmueble = reader.GetInt32("IdInmueble"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    MontoMensual = reader.GetDecimal("MontoMensual"),
                    Vigente = reader.GetBoolean("Vigente")
                };
            }

            return null; // Devuelve null si no se encuentra ningún contrato
        }

        // Método para agregar un nuevo contrato
        public void Add(Contrato contrato)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO Contrato (id_inquilino, id_inmueble, fecha_inicio, fecha_fin, monto_mensual, estado) " +
                "VALUES (@InquilinoId, @InmuebleId, @FechaInicio, @FechaFin, @MontoMensual, @Vigente)", connection);

            command.Parameters.AddWithValue("@InquilinoId", contrato.IdInquilino);
            command.Parameters.AddWithValue("@InmuebleId", contrato.IdInmueble);
            command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
            command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
            command.Parameters.AddWithValue("@Vigente", contrato.Vigente);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un contrato
        public void Update(Contrato contrato)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE Contrato SET id_inquilino = @InquilinoId, id_inmueble = @InmuebleId, fecha_inicio = @FechaInicio, " +
                "fecha_fin = @FechaFin, monto_mensual = @MontoMensual, estado = @Vigente WHERE id_contato = @Id", connection);

            command.Parameters.AddWithValue("@Id", contrato.IdContrato);
            command.Parameters.AddWithValue("@InquilinoId", contrato.IdInquilino);
            command.Parameters.AddWithValue("@InmuebleId", contrato.IdInmueble);
            command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
            command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
            command.Parameters.AddWithValue("@Vigente", contrato.Vigente);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un contrato
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM Contrato WHERE id_contrato = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}