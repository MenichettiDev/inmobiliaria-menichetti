using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;
using System;

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
            using var command = new MySqlCommand("SELECT * FROM Pago", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                pagos.Add(new Pago
                {
                    IdPago = reader.GetInt32("id_pago"),
                    IdContrato = reader.GetInt32("id_contrato"),
                    FechaPago = reader.GetDateTime("fecha_pago"),
                    Importe = reader.GetDecimal("importe")
                });
            }

            return pagos;
        }

        // Método para obtener un pago por ID
        public Pago? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM Pago WHERE id_pago = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Pago
                {
                    IdPago = reader.GetInt32("id_pago"),
                    IdContrato = reader.GetInt32("id_contrato"),
                    FechaPago = reader.GetDateTime("fecha_pago"),
                    Importe = reader.GetDecimal("importe")
                };
            }

            return null; // Devuelve null si no se encuentra ningún pago
        }

        // Método para agregar un nuevo pago
        public void Add(Pago pago)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO Pago (id_contrato, fecha_pago, importe) " +
                "VALUES (@ContratoId, @FechaPago, @Importe)", connection);

            command.Parameters.AddWithValue("@ContratoId", pago.IdContrato);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un pago
        public void Update(Pago pago)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE Pago SET id_contrato = @ContratoId, fecha_pago = @FechaPago, importe = @Importe WHERE id_pago = @Id", connection);

            command.Parameters.AddWithValue("@Id", pago.IdPago);
            command.Parameters.AddWithValue("@ContratoId", pago.IdContrato);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un pago
        public void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("DELETE FROM Pago WHERE id_pago = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}