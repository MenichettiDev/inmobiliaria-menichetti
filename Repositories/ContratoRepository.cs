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

        //Metodo para obtener contratos filtrados
        public List<Contrato> ObtenerFiltrados(int? idInquilino, int? idInmueble, DateTime? fechaDesde, DateTime? fechaHasta, decimal? montoDesde, decimal? montoHasta, string? estado, int? activo)
        {
            var lista = new List<Contrato>();
            using var conn = _dbConnection.GetConnection();
            var sql = @"SELECT c.*, i.Nombre AS InmuebleNombre, q.Nombre AS InquilinoNombre, q.Apellido AS InquilinoApellido
                FROM Contrato c
                INNER JOIN Inmueble i ON c.id_inmueble = i.id_inmueble
                INNER JOIN Inquilino q ON c.id_inquilino = q.id_inquilino";
            var where = new List<string>();
            var command = new MySqlCommand();
            command.Connection = conn;

            if (idInquilino.HasValue)
            {
                where.Add("c.id_inquilino = @idInquilino");
                command.Parameters.AddWithValue("@idInquilino", idInquilino);
            }
            if (idInmueble.HasValue)
            {
                where.Add("c.id_inmueble = @idInmueble");
                command.Parameters.AddWithValue("@idInmueble", idInmueble);
            }
            if (fechaDesde.HasValue)
            {
                where.Add("c.fecha_inicio >= @fechaDesde");
                command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
            }
            if (fechaHasta.HasValue)
            {
                where.Add("c.fecha_fin <= @fechaHasta");
                command.Parameters.AddWithValue("@fechaHasta", fechaHasta);
            }
            if (montoDesde.HasValue)
            {
                where.Add("c.monto_mensual >= @montoDesde");
                command.Parameters.AddWithValue("@montoDesde", montoDesde);
            }
            if (montoHasta.HasValue)
            {
                where.Add("c.monto_mensual <= @montoHasta");
                command.Parameters.AddWithValue("@montoHasta", montoHasta);
            }
            if (!string.IsNullOrEmpty(estado))
            {
                where.Add("c.estado = @estado");
                command.Parameters.AddWithValue("@estado", estado);
            }
            if (activo.HasValue)
            {
                where.Add("c.activo = @activo");
                command.Parameters.AddWithValue("@activo", activo);
            }

            if (where.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", where);
            }

            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Contrato
                {
                    IdContrato = (int)reader["id_contrato"],
                    IdInquilino = (int)reader["id_inquilino"],
                    IdInmueble = (int)reader["id_inmueble"],
                    FechaInicio = (DateTime)reader["fecha_inicio"],
                    FechaFin = (DateTime)reader["fecha_fin"],
                    MontoMensual = (decimal)reader["monto_mensual"],
                    Estado = reader["estado"].ToString()!,
                    FechaTerminacionAnticipada = reader["fecha_terminacion_anticipada"] as DateTime?,
                    Multa = reader["multa"] as decimal?,
                    Activo = Convert.ToInt32(reader["activo"]),
                    Inquilino = new Inquilino
                    {
                        Nombre = reader["InquilinoNombre"].ToString()!,
                        Apellido = reader["InquilinoApellido"].ToString()!
                    },
                    Inmueble = new Inmueble { Nombre = reader["InmuebleNombre"].ToString() }
                });
            }

            return lista;
        }


        // Método para obtener todos los contratos
        public List<Contrato> GetAll()
        {
            var contratos = new List<Contrato>();

            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(@$"SELECT * FROM contrato c, inquilino i, inmueble f 
            where c.id_inquilino = i.id_inquilino and c.id_inmueble = f.id_inmueble", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                contratos.Add(new Contrato
                {
                    IdContrato = reader.GetInt32("id_contrato"),
                    IdInquilino = reader.GetInt32("id_inquilino"),
                    Inquilino = new Inquilino
                    {
                        Nombre = reader.GetString("nombre"),
                        Apellido = reader.GetString("apellido")
                    },
                    IdInmueble = reader.GetInt32("id_inmueble"),
                    Inmueble = new Inmueble
                    {
                        Uso = reader.GetString("uso"),
                        Precio = reader.GetDecimal("precio"),
                        Estado = reader.GetString("estado"),
                        Activo = reader.GetInt32("activo"),
                    },
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
                        : (int?)reader.GetInt32("eliminado_por"),
                    Activo = reader.GetInt32("activo"),

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
                        : (int?)reader.GetInt32("eliminado_por"),
                    Activo = reader.GetInt32("activo"),
                };
            }

            return null; // Devuelve null si no se encuentra ningún contrato
        }

        // Método para agregar un nuevo contrato
        public void Add(Contrato contrato)
        {
            // Validaciones
            ValidarFechas(contrato.FechaInicio, contrato.FechaFin);
            ValidarMonto(contrato.MontoMensual);

            if (InmuebleEstaOcupado(contrato.IdInmueble, contrato.FechaInicio, contrato.FechaFin))
                throw new InvalidOperationException("El inmueble ya tiene un contrato vigente en las fechas seleccionadas.");

            // Console.WriteLine("Contrato a insertar: " + contrato.IdInmueble + " " + contrato.IdInquilino + " " + contrato.FechaInicio + " " + contrato.FechaFin + " " + contrato.MontoMensual);
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO contrato (id_inquilino, id_inmueble, fecha_inicio, fecha_fin, monto_mensual, " +
                "creado_por) " +
                "VALUES (@IdInquilino, @IdInmueble, @FechaInicio, @FechaFin, @MontoMensual, " +
                " @CreadoPor)", connection);

            command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
            command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
            command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
            command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
            command.Parameters.AddWithValue("@CreadoPor", contrato.CreadoPor ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un contrato
        public void Update(Contrato contrato)
        {
            // Validaciones
            ValidarFechas(contrato.FechaInicio, contrato.FechaFin);
            ValidarMonto(contrato.MontoMensual);

            if (InmuebleEstaOcupado(contrato.IdInmueble, contrato.FechaInicio, contrato.FechaFin, contrato.IdContrato))
                throw new InvalidOperationException("El inmueble ya tiene un contrato vigente en las fechas seleccionadas.");


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

        public void bajaLogica(Contrato contrato)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE contrato SET activo = 0 " +
                "WHERE id_contrato = @IdContrato", connection);

            command.Parameters.AddWithValue("@IdContrato", contrato.IdContrato);

            command.ExecuteNonQuery();
        }

        public void altaLogica(Contrato contrato)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE contrato SET activo = 1 " +
                "WHERE id_contrato = @IdContrato", connection);

            command.Parameters.AddWithValue("@IdContrato", contrato.IdContrato);

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

        //==========================================================Validacion de existencia de un contrato==========================================================
        private void ValidarFechas(DateTime inicio, DateTime fin)
        {
            if (inicio >= fin)
                throw new ArgumentException("La fecha de inicio debe ser anterior a la de fin.");
        }

        private void ValidarMonto(decimal monto)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto mensual debe ser mayor a cero.");
        }

        private bool InmuebleEstaOcupado(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idContratoActual = null)
        {
            using var connection = _dbConnection.GetConnection();
            var query = @"
        SELECT COUNT(*) FROM contrato 
        WHERE id_inmueble = @IdInmueble 
        AND activo = 1
        AND (@FechaInicio <= fecha_fin AND @FechaFin >= fecha_inicio)
        " + (idContratoActual.HasValue ? "AND id_contrato != @IdContratoActual" : "");

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdInmueble", idInmueble);
            command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
            command.Parameters.AddWithValue("@FechaFin", fechaFin);

            if (idContratoActual.HasValue)
                command.Parameters.AddWithValue("@IdContratoActual", idContratoActual.Value);

            var count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }


    }
}