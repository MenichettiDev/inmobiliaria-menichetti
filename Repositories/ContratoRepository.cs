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
        public List<Contrato> ObtenerFiltrados(
                int? idInquilino,
                int? idInmueble,
                DateTime? fechaDesde,
                DateTime? fechaHasta,
                decimal? montoDesde,
                decimal? montoHasta,
                string? estado,
                int? activo,
                int? venceEnDias // 👈 nuevo
                )
        {
            var lista = new List<Contrato>();
            using var conn = _dbConnection.GetConnection();
            var sql = @"SELECT c.*, i.nombre_inmueble, q.Nombre AS InquilinoNombre, q.Apellido AS InquilinoApellido
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

            // 👇 NUEVO: contratos que vencen dentro de X días
            if (venceEnDias.HasValue)
            {
                where.Add("c.fecha_fin BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @venceEnDias DAY)");
                command.Parameters.AddWithValue("@venceEnDias", venceEnDias.Value);
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
                    Inmueble = new Inmueble { NombreInmueble = reader["nombre_inmueble"].ToString() }
                });
            }

            return lista;
        }


        // Método para obtener todos los contratos
        public List<Contrato> GetAll()
        {
            var contratos = new List<Contrato>();

            try
            {
                using var connection = _dbConnection.GetConnection();
                using var command = new MySqlCommand(@"
            SELECT 
                c.id_contrato, c.id_inquilino, c.id_inmueble, c.fecha_inicio, c.fecha_fin, 
                c.monto_mensual, c.estado AS estado_contrato, c.fecha_terminacion_anticipada, 
                c.multa, c.creado_por, c.modificado_por, c.eliminado_por, c.activo AS activo_contrato,

                i.nombre, i.apellido,

                f.nombre_inmueble, f.uso, f.precio, f.estado AS estado_inmueble, f.activo AS activo_inmueble
            FROM contrato c
            INNER JOIN inquilino i ON c.id_inquilino = i.id_inquilino
            INNER JOIN inmueble f ON c.id_inmueble = f.id_inmueble
        ", connection);

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
                            // Suponiendo que tenés la propiedad NombreCompleto como calculada
                        },
                        IdInmueble = reader.GetInt32("id_inmueble"),
                        Inmueble = new Inmueble
                        {
                            NombreInmueble = reader.GetString("nombre_inmueble"),
                            Uso = reader.GetString("uso"),
                            Precio = reader.GetDecimal("precio"),
                            Estado = reader.GetString("estado_inmueble"),
                            Activo = reader.GetInt32("activo_inmueble")
                        },
                        FechaInicio = reader.GetDateTime("fecha_inicio"),
                        FechaFin = reader.GetDateTime("fecha_fin"),
                        MontoMensual = reader.GetDecimal("monto_mensual"),
                        Estado = reader.GetString("estado_contrato"),
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
                        Activo = reader.GetInt32("activo_contrato")
                    });
                }
            }
            catch (Exception ex)
            {
                // Podés loguear el error acá si usás logging
                Console.WriteLine($"Error al obtener contratos: {ex.Message}");
                // O lanzar de nuevo si querés manejarlo en otra capa
                throw;
            }

            return contratos;
        }

        // Método para obtener un contrato por ID
        public Contrato? GetById(int id)
        {
            try
            {
                using var connection = _dbConnection.GetConnection();
                using var command = new MySqlCommand(@"
            SELECT 
                c.id_contrato, c.id_inquilino, c.id_inmueble, c.fecha_inicio, c.fecha_fin, 
                c.monto_mensual, c.estado AS estado_contrato, c.fecha_terminacion_anticipada, 
                c.multa, c.creado_por, c.modificado_por, c.eliminado_por, c.activo AS activo_contrato,
                i.nombre AS inq_nombre, i.apellido AS inq_apellido,
                f.uso AS inmueble_uso, f.nombre_inmueble, f.precio, f.estado AS estado_inmueble, f.activo AS activo_inmueble
            FROM contrato c
            JOIN inquilino i ON c.id_inquilino = i.id_inquilino
            JOIN inmueble f ON c.id_inmueble = f.id_inmueble
            WHERE c.id_contrato = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Contrato
                    {
                        IdContrato = reader.GetInt32("id_contrato"),
                        IdInquilino = reader.GetInt32("id_inquilino"),
                        Inquilino = new Inquilino
                        {
                            Nombre = reader.GetString("inq_nombre"),
                            Apellido = reader.GetString("inq_apellido")
                        },
                        IdInmueble = reader.GetInt32("id_inmueble"),
                        Inmueble = new Inmueble
                        {
                            Uso = reader.GetString("inmueble_uso"),
                            NombreInmueble = reader.GetString("nombre_inmueble"),
                            Precio = reader.GetDecimal("precio"),
                            Estado = reader.GetString("estado_inmueble"),
                            Activo = reader.GetInt32("activo_inmueble"),
                        },
                        FechaInicio = reader.GetDateTime("fecha_inicio"),
                        FechaFin = reader.GetDateTime("fecha_fin"),
                        MontoMensual = reader.GetDecimal("monto_mensual"),
                        Estado = reader.GetString("estado_contrato"),
                        FechaTerminacionAnticipada = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion_anticipada"))
                            ? null
                            : reader.GetDateTime("fecha_terminacion_anticipada"),
                        Multa = reader.IsDBNull(reader.GetOrdinal("multa"))
                            ? null
                            : reader.GetDecimal("multa"),
                        CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por"))
                            ? null
                            : reader.GetInt32("creado_por"),
                        ModificadoPor = reader.IsDBNull(reader.GetOrdinal("modificado_por"))
                            ? null
                            : reader.GetInt32("modificado_por"),
                        EliminadoPor = reader.IsDBNull(reader.GetOrdinal("eliminado_por"))
                            ? null
                            : reader.GetInt32("eliminado_por"),
                        Activo = reader.GetInt32("activo_contrato"),
                    };
                }

                return null; // No encontrado
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] al obtener contrato por ID {id}: {ex.Message}");
                return null;
            }
        }

        // Método para agregar un nuevo contrato
        public void Add(Contrato contrato)
        {
            // Validaciones
            ValidarFechas(contrato.FechaInicio, contrato.FechaFin);
            ValidarMonto(contrato.MontoMensual);

            if (InmuebleEstaOcupado(contrato.IdInmueble, contrato.FechaInicio, contrato.FechaFin))
                throw new InvalidOperationException("El inmueble ya tiene un contrato vigente en las fechas seleccionadas.");

            using var connection = _dbConnection.GetConnection();
            // connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {

                using var command = new MySqlCommand(
                    "INSERT INTO contrato (id_inquilino, id_inmueble, fecha_inicio, fecha_fin, monto_mensual, creado_por) " +
                    "VALUES (@IdInquilino, @IdInmueble, @FechaInicio, @FechaFin, @MontoMensual, @CreadoPor)", connection);

                command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
                command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
                command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
                command.Parameters.AddWithValue("@CreadoPor", contrato.CreadoPor ?? (object)DBNull.Value);

                command.ExecuteNonQuery();

                // Obtenemos el ID del contrato recién insertado
                int contratoId = (int)command.LastInsertedId;

                // Generamos los pagos
                GenerarPagos(connection, transaction, contratoId, contrato.FechaInicio, contrato.FechaFin, contrato.MontoMensual);
                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                // Error de base de datos
                transaction.Rollback();
                throw new Exception("Error al insertar el contrato en la base de datos. Detalles: " + ex.Message, ex);
            }
            catch (InvalidOperationException ex)
            {
                // Error de lógica, como inmueble ocupado
                transaction.Rollback();
                throw new Exception("No se pudo completar la operación: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // Cualquier otro error
                transaction.Rollback();
                throw new Exception("Ocurrió un error inesperado al agregar el contrato. Detalles: " + ex.Message, ex);
            }
        }


        public void RenovarContrato(Contrato contratoBase, DateTime nuevaFechaInicio, DateTime nuevaFechaFin)
        {
            // Validaciones
            ValidarFechas(nuevaFechaInicio, nuevaFechaFin);
            ValidarMonto(contratoBase.MontoMensual);

            if (InmuebleEstaOcupado(contratoBase.IdInmueble, nuevaFechaInicio, nuevaFechaFin))
                throw new InvalidOperationException("El inmueble ya tiene un contrato vigente en las fechas seleccionadas.");

            using var connection = _dbConnection.GetConnection();
            // connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var command = new MySqlCommand(
                    @"INSERT INTO contrato (
                id_inquilino, 
                id_inmueble, 
                fecha_inicio, 
                fecha_fin, 
                monto_mensual, 
                creado_por
            ) 
            VALUES (
                @IdInquilino, 
                @IdInmueble, 
                @FechaInicio, 
                @FechaFin, 
                @MontoMensual, 
                @CreadoPor
            )", connection);

                command.Parameters.AddWithValue("@IdInquilino", contratoBase.IdInquilino);
                command.Parameters.AddWithValue("@IdInmueble", contratoBase.IdInmueble);
                command.Parameters.AddWithValue("@FechaInicio", nuevaFechaInicio);
                command.Parameters.AddWithValue("@FechaFin", nuevaFechaFin);
                command.Parameters.AddWithValue("@MontoMensual", contratoBase.MontoMensual);
                command.Parameters.AddWithValue("@CreadoPor", contratoBase.CreadoPor ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
                int contratoId = (int)command.LastInsertedId;

                GenerarPagos(connection, transaction, contratoId, nuevaFechaInicio, nuevaFechaFin, contratoBase.MontoMensual);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Podés loguear el error si usás un logger, o lanzar uno más específico si preferís
                transaction.Rollback();
                Console.WriteLine($"[ERROR] al renovar contrato: {ex.Message}");
                throw new Exception("Error al renovar el contrato: " + ex.Message, ex);
            }
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
        // public void Delete(int id)
        // {
        //     using var connection = _dbConnection.GetConnection();
        //     using var command = new MySqlCommand("DELETE FROM contrato WHERE id_contrato = @Id", connection);
        //     command.Parameters.AddWithValue("@Id", id);

        //     command.ExecuteNonQuery();
        // }

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
        //Metodo para generar pagos automaticios==============================================
        private void GenerarPagos(MySqlConnection connection, MySqlTransaction transaction, int contratoId, DateTime fechaInicio, DateTime fechaFin, decimal montoMensual)
        {
            try
            {
                using var command = new MySqlCommand(
                    "INSERT INTO pago (id_contrato, numero_cuota, fecha_vencimiento, importe) VALUES (@ContratoId, @NumeroCuota, @FechaVencimiento, @Importe)",
                    connection,
                    transaction);

                var fechaCuota = fechaInicio;
                int numeroCuota = 1;

                while (fechaCuota <= fechaFin)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ContratoId", contratoId);
                    command.Parameters.AddWithValue("@NumeroCuota", numeroCuota);
                    command.Parameters.AddWithValue("@FechaVencimiento", fechaCuota);
                    command.Parameters.AddWithValue("@Importe", montoMensual);
                    command.ExecuteNonQuery();

                    fechaCuota = AjustarFechaMensual(fechaInicio, numeroCuota);
                    numeroCuota++;
                }
            }
            catch (Exception ex)
            {
                // Podés loguearlo si tenés logger
                Console.WriteLine($"[ERROR] al generar pagos del contrato {contratoId}: {ex.Message}");
                throw new Exception("Error al generar los pagos del contrato: " + ex.Message, ex);
            }
        }

        private DateTime AjustarFechaMensual(DateTime fechaBase, int mesesASumar)
        {
            var nuevaFecha = fechaBase.AddMonths(mesesASumar);

            // Si el día original no existe en el nuevo mes, se ajusta al último día del mes
            if (fechaBase.Day > DateTime.DaysInMonth(nuevaFecha.Year, nuevaFecha.Month))
            {
                return new DateTime(nuevaFecha.Year, nuevaFecha.Month, DateTime.DaysInMonth(nuevaFecha.Year, nuevaFecha.Month));
            }

            return new DateTime(nuevaFecha.Year, nuevaFecha.Month, fechaBase.Day);
        }

        //===============================MULTA y TERMINAR ANTICIPADO========================================================
        public void TerminarContratoAnticipadamente(int idContrato, DateTime fechaTerminacion)
        {
            using var connection = _dbConnection.GetConnection();

            var contrato = GetById(idContrato);
            if (contrato == null)
            {
                throw new Exception("Contrato no encontrado.");
            }

            // VALIDACIONES
            if (fechaTerminacion < DateTime.Today)
            {
                throw new Exception("La fecha de terminación no puede ser anterior al día de hoy.");
            }

            if (fechaTerminacion < contrato.FechaInicio || fechaTerminacion > contrato.FechaFin)
            {
                throw new Exception("La fecha de terminación debe estar dentro del período del contrato.");
            }

            if (contrato.Estado != "vigente")
            {
                throw new Exception("Solo se pueden finalizar contratos que están vigentes.");
            }

            if (contrato.FechaTerminacionAnticipada.HasValue)
            {
                throw new Exception("Este contrato ya fue terminado anticipadamente.");
            }

            // Calcular multa
            var multa = CalcularMulta(fechaTerminacion, contrato.FechaInicio, contrato.MontoMensual);

            // Registrar la multa como un nuevo pago
            using var command = new MySqlCommand(
                "INSERT INTO pago (id_contrato, importe, detalle, fecha_vencimiento) VALUES (@IdContrato, @Importe, @Detalle, @FechaVencimiento)",
                connection);

            command.Parameters.AddWithValue("@IdContrato", idContrato);
            // command.Parameters.AddWithValue("@FechaPago", fechaTerminacion);
            command.Parameters.AddWithValue("@Importe", multa);
            command.Parameters.AddWithValue("@Detalle", $"Multa por terminación anticipada del contrato. Total: {multa}");
            command.Parameters.AddWithValue("@FechaVencimiento", fechaTerminacion.AddDays(30));

            command.ExecuteNonQuery();

            // Actualizar el contrato
            using var updateCommand = new MySqlCommand(
                "UPDATE contrato SET estado = @Estado, fecha_terminacion_anticipada = @FechaTerminacion, multa = @Multa WHERE id_contrato = @IdContrato",
                connection);

            updateCommand.Parameters.AddWithValue("@Estado", "finalizado");
            updateCommand.Parameters.AddWithValue("@FechaTerminacion", fechaTerminacion);
            updateCommand.Parameters.AddWithValue("@Multa", multa);
            updateCommand.Parameters.AddWithValue("@IdContrato", idContrato);

            updateCommand.ExecuteNonQuery();
            AnularPagosPosteriores(connection, idContrato, fechaTerminacion);

        }




        public decimal CalcularMulta(DateTime fechaTerminacion, DateTime fechaInicio, decimal montoMensual)
        {
            var duracionOriginal = fechaInicio - fechaTerminacion;
            var fechaMedioContrato = fechaInicio.AddDays(duracionOriginal.Days / 2);

            if (fechaTerminacion < fechaMedioContrato)
            {
                // Menos de la mitad del contrato, multa de 2 meses
                return montoMensual * 2;
            }
            else
            {
                // Más de la mitad del contrato, multa de 1 mes
                return montoMensual;
            }
        }

        private void AnularPagosPosteriores(MySqlConnection connection, int idContrato, DateTime fechaTerminacion)
        {
            using var command = new MySqlCommand(
                @"UPDATE pago 
            SET estado = 'anulado' 
            WHERE id_contrato = @IdContrato 
            AND fecha_vencimiento > @FechaTerminacion 
            AND (detalle IS NULL OR detalle NOT LIKE '%multa%')", connection);

            command.Parameters.AddWithValue("@IdContrato", idContrato);
            command.Parameters.AddWithValue("@FechaTerminacion", fechaTerminacion);

            int rows = command.ExecuteNonQuery();
            Console.WriteLine($"[INFO] {rows} pagos anulados posteriores a la terminación anticipada.");
        }


    }
}