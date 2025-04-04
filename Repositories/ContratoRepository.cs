using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;

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
            using var command = new MySqlCommand("SELECT * FROM contrato", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                contratos.Add(new Contrato
                {
                    IdContrato = reader.GetInt32("id_contrato"),
                    IdInquilino = reader.GetInt32("id_inquilino"),
                    IdInmueble = reader.GetInt32("id_inmueble"),
                    FechaInicio = reader.GetDateTime("fecha_inicio"),
                    FechaFin = reader.GetDateTime("fecha_fin"),
                    MontoMensual = reader.GetDecimal("monto_mensual"),
                    Estado = reader.GetString("estado"),
                    FechaTerminacionAnticipada = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion_anticipada"))
                        ? null
                        : (DateTime?)reader.GetDateTime("fecha_terminacion_anticipada"),
                    Multa = reader.IsDBNull(reader.GetOrdinal("multa"))
                        ? null
                        : (decimal?)reader.GetDecimal("multa"),
                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por"))
                        ? null
                        : (int?)reader.GetInt32("creado_por"),
                    ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por"))
                        ? null
                        : (int?)reader.GetInt32("modificado_por"),
                    EliminadoPor = reader.IsDBNull(reader.GetOrdinal("eliminado_por"))
                        ? null
                        : (int?)reader.GetInt32("eliminado_por")
                });
            }

            return contratos;
        }

        // Método para obtener un contrato por ID
        public Contrato? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM contrato WHERE id_contrato = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Contrato
                {
                    IdContrato = reader.GetInt32("id_contrato"),
                    IdInquilino = reader.GetInt32("id_inquilino"),
                    IdInmueble = reader.GetInt32("id_inmueble"),
                    FechaInicio = reader.GetDateTime("fecha_inicio"),
                    FechaFin = reader.GetDateTime("fecha_fin"),
                    MontoMensual = reader.GetDecimal("monto_mensual"),
                    Estado = reader.GetString("estado"),
                    FechaTerminacionAnticipada = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion_anticipada"))
                        ? null
                        : (DateTime?)reader.GetDateTime("fecha_terminacion_anticipada"),
                    Multa = reader.IsDBNull(reader.GetOrdinal("multa"))
                        ? null
                        : (decimal?)reader.GetDecimal("multa"),
                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por"))
                        ? null
                        : (int?)reader.GetInt32("creado_por"),
                    ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por"))
                        ? null
                        : (int?)reader.GetInt32("modificado_por"),
                    EliminadoPor = reader.IsDBNull(reader.GetOrdinal("eliminado_por"))
                        ? null
                        : (int?)reader.GetInt32("eliminado_por")
                };
            }

            return null; // Devuelve null si no se encuentra ningún contrato
        }

        // Método para agregar un nuevo contrato
        public void Add(Contrato contrato)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO contrato (id_inquilino, id_inmueble, fecha_inicio, fecha_fin, monto_mensual, estado, " +
                "fecha_terminacion_anticipada, multa, creado_por, modificado_por, eliminado_por) " +
                "VALUES (@IdInquilino, @IdInmueble, @FechaInicio, @FechaFin, @MontoMensual, @Estado, " +
                "@FechaTerminacionAnticipada, @Multa, @CreadoPor, @ModificadoPor, @EliminadoPor)", connection);

            command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
            command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
            command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
            command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
            command.Parameters.AddWithValue("@Estado", contrato.Estado);
            command.Parameters.AddWithValue("@FechaTerminacionAnticipada", contrato.FechaTerminacionAnticipada ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Multa", contrato.Multa ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreadoPor", contrato.CreadoPor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ModificadoPor", contrato.ModificadoPor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EliminadoPor", contrato.EliminadoPor ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un contrato
        public void Update(Contrato contrato)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE contrato SET id_inquilino = @IdInquilino, id_inmueble = @IdInmueble, fecha_inicio = @FechaInicio, " +
                "fecha_fin = @FechaFin, monto_mensual = @MontoMensual, estado = @Estado, " +
                "fecha_terminacion_anticipada = @FechaTerminacionAnticipada, multa = @Multa, " +
                "creado_por = @CreadoPor, modificado_por = @ModificadoPor, eliminado_por = @EliminadoPor " +
                "WHERE id_contrato = @IdContrato", connection);

            command.Parameters.AddWithValue("@IdContrato", contrato.IdContrato);
            command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
            command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
            command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
            command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
            command.Parameters.AddWithValue("@Estado", contrato.Estado);
            command.Parameters.AddWithValue("@FechaTerminacionAnticipada", contrato.FechaTerminacionAnticipada ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Multa", contrato.Multa ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreadoPor", contrato.CreadoPor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ModificadoPor", contrato.ModificadoPor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EliminadoPor", contrato.EliminadoPor ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un contrato
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM contrato WHERE id_contrato = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}