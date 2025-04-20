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
                    FechaPago = reader.GetDateTime("fecha_estimada"),
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

        public List<Pago> ObtenerFiltrados(
                int? idContrato,
                int? idInquilino,
                DateTime? fechaDesde,
                DateTime? fechaHasta,
                decimal? importeMin,
                decimal? importeMax)
        {
            var lista = new List<Pago>();
            using var conn = _dbConnection.GetConnection();

            var sql = @"SELECT p.*, c.id_inquilino, q.Nombre AS InquilinoNombre, q.Apellido AS InquilinoApellido,
                    i.nombre_inmueble
                FROM pago p
                INNER JOIN contrato c ON p.id_contrato = c.id_contrato
                INNER JOIN inquilino q ON c.id_inquilino = q.id_inquilino
                INNER JOIN inmueble i ON c.id_inmueble = i.id_inmueble";

            var where = new List<string>();
            var command = new MySqlCommand();
            command.Connection = conn;

            if (idContrato.HasValue)
            {
                where.Add("p.id_contrato = @idContrato");
                command.Parameters.AddWithValue("@idContrato", idContrato);
            }

            if (idInquilino.HasValue)
            {
                where.Add("c.id_inquilino = @idInquilino");
                command.Parameters.AddWithValue("@idInquilino", idInquilino);
            }

            if (fechaDesde.HasValue)
            {
                where.Add("p.fecha_estimada >= @fechaDesde");
                command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
            }

            if (fechaHasta.HasValue)
            {
                where.Add("p.fecha_estimada <= @fechaHasta");
                command.Parameters.AddWithValue("@fechaHasta", fechaHasta);
            }

            if (importeMin.HasValue)
            {
                where.Add("p.importe >= @importeMin");
                command.Parameters.AddWithValue("@importeMin", importeMin);
            }

            if (importeMax.HasValue)
            {
                where.Add("p.importe <= @importeMax");
                command.Parameters.AddWithValue("@importeMax", importeMax);
            }

            if (where.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", where);
            }

            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = (int)reader["id_pago"],
                    IdContrato = (int)reader["id_contrato"],
                    FechaPago = (DateTime)reader["fecha_estimada"],
                    Importe = (decimal)reader["importe"],
                    Detalle = reader["detalle"]?.ToString(),
                    Contrato = new Contrato
                    {
                        IdContrato = (int)reader["id_contrato"],
                        IdInquilino = (int)reader["id_inquilino"],
                        Inquilino = new Inquilino
                        {
                            Nombre = reader["InquilinoNombre"].ToString()!,
                            Apellido = reader["InquilinoApellido"].ToString()!
                        },
                        Inmueble = new Inmueble
                        {
                            NombreInmueble = reader["nombre_inmueble"].ToString()!
                        }
                    }
                });
            }

            return lista;
        }


        //Obtenemos los pagos de un contrato
        public List<Pago> ObtenerPorContrato(int idContrato)
        {
            var lista = new List<Pago>();
            using var conn = _dbConnection.GetConnection();

            var sql = @"SELECT p.*, c.id_inquilino, q.Nombre AS InquilinoNombre, q.Apellido AS InquilinoApellido,
                       i.nombre_inmueble
                FROM pago p
                INNER JOIN contrato c ON p.id_contrato = c.id_contrato
                INNER JOIN inquilino q ON c.id_inquilino = q.id_inquilino
                INNER JOIN inmueble i ON c.id_inmueble = i.id_inmueble
                WHERE p.id_contrato = @idContrato";

            using var command = new MySqlCommand(sql, conn);
            command.Parameters.AddWithValue("@idContrato", idContrato);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = (int)reader["id_pago"],
                    IdContrato = (int)reader["id_contrato"],
                    FechaPago = (DateTime)reader["fecha_estimada"],
                    Importe = (decimal)reader["importe"],
                    NumeroCuota = (int)reader["numero_cuota"],
                    Detalle = reader["detalle"]?.ToString(),
                    Contrato = new Contrato
                    {
                        IdContrato = (int)reader["id_contrato"],
                        IdInquilino = (int)reader["id_inquilino"],
                        Inquilino = new Inquilino
                        {
                            Nombre = reader["InquilinoNombre"].ToString()!,
                            Apellido = reader["InquilinoApellido"].ToString()!
                        },
                        Inmueble = new Inmueble
                        {
                            NombreInmueble = reader["nombre_inmueble"].ToString()!
                        }
                    }
                });
            }

            return lista;
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
                    FechaPago = reader.GetDateTime("fecha_estimada"),
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
                "INSERT INTO pago (id_contrato, fecha_estimada, importe, detalle, creado_por, modificado_por, eliminado_por) " +
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
                "UPDATE pago SET id_contrato = @ContratoId, fecha_estimada = @FechaPago, importe = @Importe, " +
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