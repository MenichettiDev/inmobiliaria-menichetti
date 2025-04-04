using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories
{
    public class PagoRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public PagoRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Método para obtener todos los pagos
        public List<Pago> GetAll()
        {
            var pagos = new List<Pago>();

            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM pago", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                pagos.Add(new Pago
                {
                    IdPago = reader.GetInt32("id_pago"),
                    IdContrato = reader.GetInt32("id_contrato"),
                    FechaPago = reader.GetDateTime("fecha_pago"),
                    Importe = reader.GetDecimal("importe"),
                    Detalle = reader.IsDBNull(reader.GetOrdinal("detalle"))
                        ? null
                        : reader.GetString("detalle"),
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

            return pagos;
        }

        // Método para obtener un pago por ID
        public Pago? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM pago WHERE id_pago = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Pago
                {
                    IdPago = reader.GetInt32("id_pago"),
                    IdContrato = reader.GetInt32("id_contrato"),
                    FechaPago = reader.GetDateTime("fecha_pago"),
                    Importe = reader.GetDecimal("importe"),
                    Detalle = reader.IsDBNull(reader.GetOrdinal("detalle"))
                        ? null
                        : reader.GetString("detalle"),
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

            return null; // Devuelve null si no se encuentra ningún pago
        }

        // Método para agregar un nuevo pago
        public void Add(Pago pago)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO pago (id_contrato, fecha_pago, importe, detalle, creado_por, modificado_por, eliminado_por) " +
                "VALUES (@ContratoId, @FechaPago, @Importe, @Detalle, @CreadoPor, @ModificadoPor, @EliminadoPor)", connection);

            command.Parameters.AddWithValue("@ContratoId", pago.IdContrato);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);
            command.Parameters.AddWithValue("@Detalle", string.IsNullOrEmpty(pago.Detalle) ? (object)DBNull.Value : pago.Detalle);
            command.Parameters.AddWithValue("@CreadoPor", pago.CreadoPor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ModificadoPor", pago.ModificadoPor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EliminadoPor", pago.EliminadoPor ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un pago
        public void Update(Pago pago)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE pago SET id_contrato = @ContratoId, fecha_pago = @FechaPago, importe = @Importe, " +
                "detalle = @Detalle, creado_por = @CreadoPor, modificado_por = @ModificadoPor, eliminado_por = @EliminadoPor " +
                "WHERE id_pago = @Id", connection);

            command.Parameters.AddWithValue("@Id", pago.IdPago);
            command.Parameters.AddWithValue("@ContratoId", pago.IdContrato);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);
            command.Parameters.AddWithValue("@Detalle", string.IsNullOrEmpty(pago.Detalle) ? (object)DBNull.Value : pago.Detalle);
            command.Parameters.AddWithValue("@CreadoPor", pago.CreadoPor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ModificadoPor", pago.ModificadoPor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EliminadoPor", pago.EliminadoPor ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un pago
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM pago WHERE id_pago = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}