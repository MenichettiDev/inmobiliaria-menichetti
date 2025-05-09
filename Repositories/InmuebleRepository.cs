using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using InmobiliariaApp.Data;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories
{
    public class InmuebleRepository
    {
        private readonly DatabaseConnection _dbConnection;
        private readonly ImagenRepository _imagenRepository;


        public InmuebleRepository(DatabaseConnection dbConnection, ImagenRepository imagenRepository)
        {
            _dbConnection = dbConnection;
            _imagenRepository = imagenRepository;
        }
        public List<Inmueble> ObtenerFiltrados(string? uso,
            int? ambientes,
            decimal? precioDesde,
            decimal? precioHasta,
            string? estado,
            int? activo,
            int page,
            int pageSize,
            out int totalItems)
        {
            var lista = new List<Inmueble>();
            totalItems = 0;

            using var connection = _dbConnection.GetConnection();

            var filtros = @" FROM Inmueble i
                        INNER JOIN Propietario p ON i.id_propietario = p.id_propietario
                        WHERE 1=1";

            using var countCommand = new MySqlCommand();
            countCommand.Connection = connection;

            // Agregamos filtros dinámicamente
            if (!string.IsNullOrEmpty(uso))
            {
                filtros += " AND i.Uso LIKE @uso";
                countCommand.Parameters.AddWithValue("@uso", "%" + uso + "%");
            }

            if (ambientes.HasValue)
            {
                filtros += " AND i.Ambientes = @ambientes";
                countCommand.Parameters.AddWithValue("@ambientes", ambientes.Value);
            }

            if (precioDesde.HasValue)
            {
                filtros += " AND i.Precio >= @precioDesde";
                countCommand.Parameters.AddWithValue("@precioDesde", precioDesde.Value);
            }

            if (precioHasta.HasValue)
            {
                filtros += " AND i.Precio <= @precioHasta";
                countCommand.Parameters.AddWithValue("@precioHasta", precioHasta.Value);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                filtros += " AND i.Estado = @estado";
                countCommand.Parameters.AddWithValue("@estado", estado);
            }

            if (activo.HasValue)
            {
                filtros += " AND i.Activo = @activo";
                countCommand.Parameters.AddWithValue("@activo", activo.Value);
            }

            // Total count
            countCommand.CommandText = "SELECT COUNT(*) " + filtros;
            totalItems = Convert.ToInt32(countCommand.ExecuteScalar());

            // Ahora la consulta real paginada
            var sql = @"SELECT i.*, p.Nombre, p.Apellido " + filtros + " LIMIT @limit OFFSET @offset";

            using var command = new MySqlCommand(sql, connection);

            foreach (MySqlParameter p in countCommand.Parameters)
                command.Parameters.Add((MySqlParameter)((ICloneable)p).Clone());

            command.Parameters.AddWithValue("@limit", pageSize);
            command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Inmueble
                {
                    IdInmueble = Convert.ToInt32(reader["id_inmueble"]),
                    NombreInmueble = reader["nombre_inmueble"].ToString(),
                    Direccion = reader["Direccion"].ToString(),
                    Uso = reader["Uso"].ToString(),
                    Ambientes = Convert.ToInt32(reader["Ambientes"]),
                    Precio = Convert.ToDecimal(reader["Precio"]),
                    Estado = reader["Estado"].ToString(),
                    Activo = Convert.ToInt32(reader["Activo"]),
                    Portada = reader["Portada"].ToString(),
                    Duenio = new Propietario
                    {
                        Nombre = reader["Nombre"].ToString(),
                        Apellido = reader["Apellido"].ToString()
                    }
                });
            }

            return lista;
        }


        // public List<Inmueble> ObtenerFiltrados(string? uso, int? ambientes, decimal? precioDesde, decimal? precioHasta, string? estado)
        // {
        //     var lista = new List<Inmueble>();

        //     using var connection = _dbConnection.GetConnection();

        //     var sql = @"SELECT i.*, p.Nombre, ' ', p.Apellido
        //         FROM Inmueble i
        //         INNER JOIN Propietario p ON i.id_propietario = p.id_propietario
        //         WHERE 1=1";

        //     using var command = new MySqlCommand();
        //     command.Connection = connection;

        //     if (!string.IsNullOrEmpty(uso))
        //     {
        //         sql += " AND i.Uso LIKE @uso";
        //         command.Parameters.AddWithValue("@uso", "%" + uso + "%");
        //     }

        //     if (ambientes.HasValue)
        //     {
        //         sql += " AND i.Ambientes = @ambientes";
        //         command.Parameters.AddWithValue("@ambientes", ambientes.Value);
        //     }

        //     if (precioDesde.HasValue)
        //     {
        //         sql += " AND i.Precio >= @precioDesde";
        //         command.Parameters.AddWithValue("@precioDesde", precioDesde.Value);
        //     }

        //     if (precioHasta.HasValue)
        //     {
        //         sql += " AND i.Precio <= @precioHasta";
        //         command.Parameters.AddWithValue("@precioHasta", precioHasta.Value);
        //     }

        //     if (!string.IsNullOrEmpty(estado))
        //     {
        //         sql += " AND i.Estado = @estado";
        //         command.Parameters.AddWithValue("@estado", estado);
        //     }

        //     command.CommandText = sql;

        //     using var reader = command.ExecuteReader();

        //     while (reader.Read())
        //     {
        //         lista.Add(new Inmueble
        //         {
        //             IdInmueble = Convert.ToInt32(reader["id_inmueble"]),
        //             NombreInmueble = reader["nombre_inmueble"].ToString(),
        //             Direccion = reader["Direccion"].ToString(),
        //             Uso = reader["Uso"].ToString()!,
        //             Ambientes = Convert.ToInt32(reader["Ambientes"]),
        //             Precio = Convert.ToDecimal(reader["Precio"]),
        //             Estado = reader["Estado"].ToString()!,
        //             Activo = Convert.ToInt32(reader["Activo"]),
        //             Portada = reader["Portada"].ToString(),
        //             Duenio = new Propietario
        //             {
        //                 Nombre = reader["Nombre"].ToString()!,
        //                 Apellido = reader["Apellido"].ToString()!,
        //             }
        //         });
        //     }

        //     return lista;
        // }


        // Método para obtener todos los inmuebles
        public List<Inmueble> GetAll()
        {
            var inmuebles = new List<Inmueble>();

            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("SELECT * FROM inmueble i, propietario p where p.id_propietario = i.id_propietario", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                inmuebles.Add(new Inmueble
                {
                    IdInmueble = reader.GetInt32("id_inmueble"),
                    NombreInmueble = reader.GetString("nombre_inmueble"),
                    IdPropietario = reader.GetInt32("id_propietario"),
                    Duenio = new Propietario
                    {
                        IdPropietario = reader.GetInt32("id_propietario"),
                        Nombre = reader.GetString("nombre"),
                        Apellido = reader.GetString("apellido"),
                        Telefono = reader.GetString("telefono"),
                        Email = reader.GetString("dni"),
                        Dni = reader.GetString("email"),
                    },
                    Direccion = reader.GetString("direccion"),
                    Coordenadas = reader.IsDBNull(reader.GetOrdinal("coordenadas"))
                        ? null
                        : reader.GetString("coordenadas"),
                    Uso = reader.GetString("uso"),
                    IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                    Tipo = new TipoInmueble
                    {
                        IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                        Nombre = reader.GetString("nombre")
                    },
                    Ambientes = reader.GetInt32("ambientes"),
                    Precio = reader.GetDecimal("precio"),
                    Estado = reader.GetString("estado"),
                    Activo = reader.GetInt32("activo"),
                    Portada = reader.IsDBNull(reader.GetOrdinal("portada"))
                        ? null
                        : reader.GetString("portada")
                });
            }

            return inmuebles;
        }

        // Método para obtener un inmueble por ID
        public Inmueble? GetById(int id)
        {
            using var connection = _dbConnection.GetConnection();

            Inmueble? inmueble = null;

            using (var command = new MySqlCommand(@"
        SELECT i.*, p.nombre, p.apellido, t.nombre as tipo_nombre 
        FROM inmueble i
        JOIN propietario p ON i.id_propietario = p.id_propietario
        JOIN tipo_inmueble t ON i.id_tipo_inmueble = t.id_tipo_inmueble
        WHERE i.id_inmueble = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    inmueble = new Inmueble
                    {
                        IdInmueble = reader.GetInt32("id_inmueble"),
                        NombreInmueble = reader.GetString("nombre_inmueble"),
                        Duenio = new Propietario
                        {
                            IdPropietario = reader.GetInt32("id_propietario"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                        },
                        Direccion = reader.GetString("direccion"),
                        Coordenadas = reader.IsDBNull(reader.GetOrdinal("coordenadas")) ? null : reader.GetString("coordenadas"),
                        Uso = reader.GetString("uso"),
                        IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                        Tipo = new TipoInmueble
                        {
                            Nombre = reader.GetString("tipo_nombre")
                        },
                        Ambientes = reader.GetInt32("ambientes"),
                        Precio = reader.GetDecimal("precio"),
                        Estado = reader.GetString("estado"),
                        Activo = reader.GetInt32("activo"),
                        Portada = reader.IsDBNull(reader.GetOrdinal("portada")) ? null : reader.GetString("portada"),
                    };
                }
            }
            // Si se encontró el inmueble, buscamos las imágenes desde el repo de imágenes
            if (inmueble != null)
            {
                inmueble.Imagenes = _imagenRepository.BuscarPorInmueble(id);
            }

            return inmueble;
        }


        // Método para agregar un nuevo inmueble
        public void Add(Inmueble inmueble)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO inmueble (id_propietario, nombre_inmueble, direccion, coordenadas, uso, id_tipo_inmueble, ambientes, precio, estado) " +
                "VALUES (@IdPropietario, @Nombre, @Direccion, @Coordenadas, @Uso, @IdTipoInmueble, @Ambientes, @Precio, @Estado)", connection);

            command.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
            command.Parameters.AddWithValue("@Nombre", inmueble.NombreInmueble);
            command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@Coordenadas", string.IsNullOrEmpty(inmueble.Coordenadas) ? (object)DBNull.Value : inmueble.Coordenadas);
            command.Parameters.AddWithValue("@Uso", inmueble.Uso);
            command.Parameters.AddWithValue("@IdTipoInmueble", inmueble.IdTipoInmueble);
            command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
            command.Parameters.AddWithValue("@Precio", inmueble.Precio);
            command.Parameters.AddWithValue("@Estado", inmueble.Estado);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un inmueble
        public void Update(Inmueble inmueble)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE inmueble SET id_propietario = @IdPropietario, nombre_inmueble = @Nombre, direccion = @Direccion, coordenadas = @Coordenadas, " +
                "uso = @Uso, id_tipo_inmueble = @IdTipoInmueble, ambientes = @Ambientes, precio = @Precio, estado = @Estado " +
                "WHERE id_inmueble = @IdInmueble", connection);

            command.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);
            command.Parameters.AddWithValue("@Nombre", inmueble.NombreInmueble);
            command.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
            command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@Coordenadas", string.IsNullOrEmpty(inmueble.Coordenadas) ? (object)DBNull.Value : inmueble.Coordenadas);
            command.Parameters.AddWithValue("@Uso", inmueble.Uso);
            command.Parameters.AddWithValue("@IdTipoInmueble", inmueble.IdTipoInmueble);
            command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
            command.Parameters.AddWithValue("@Precio", inmueble.Precio);
            command.Parameters.AddWithValue("@Estado", inmueble.Estado);
            // command.Parameters.AddWithValue("@Activo", inmueble.Activo);

            command.ExecuteNonQuery();
        }

        public void bajaLogica(Inmueble inmueble)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE inmueble SET activo = 0 " +
                "WHERE id_inmueble = @IdInmueble", connection);

            command.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);

            command.ExecuteNonQuery();
        }

        public void altaLogica(Inmueble inmueble)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "UPDATE inmueble SET activo = 1 " +
                "WHERE id_inmueble = @IdInmueble", connection);

            command.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)//baja logica
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand("UPDATE inmueble SET estado = @Estado " +
                "WHERE id_inmueble = @IdInmueble", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        //metodo para subir la portada de un inmueble
        public int ModificarPortada(int id, string url)

        {
            int res = -1;
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(@"UPDATE inmueble SET portada = @Portada 
                                        WHERE id_inmueble = @IdInmueble", connection);

            command.Parameters.AddWithValue("@Portada", string.IsNullOrEmpty(url) ? DBNull.Value : url);
            command.Parameters.AddWithValue("@IdInmueble", id);

            res = command.ExecuteNonQuery();
            connection.Close();

            return res;
        }

        //Filtro para index==================================================
        public List<Inmueble> IndexFiltrados(string? uso,
    int? ambientes,
    decimal? precioDesde,
    decimal? precioHasta,
    string? estado,
    int? activo,
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    int? idPropietario,
    int page,
    int pageSize,
    out int totalItems)
        {
            var lista = new List<Inmueble>();
            totalItems = 0;

            using var connection = _dbConnection.GetConnection();

            var filtros = @" FROM Inmueble i
                        INNER JOIN Propietario p ON i.id_propietario = p.id_propietario
                        WHERE 1=1";

            using var countCommand = new MySqlCommand();
            countCommand.Connection = connection;

            // Agregamos filtros dinámicamente
            if (!string.IsNullOrEmpty(uso))
            {
                filtros += " AND i.Uso LIKE @uso";
                countCommand.Parameters.AddWithValue("@uso", "%" + uso + "%");
            }

            if (ambientes.HasValue)
            {
                filtros += " AND i.Ambientes = @ambientes";
                countCommand.Parameters.AddWithValue("@ambientes", ambientes.Value);
            }

            if (precioDesde.HasValue)
            {
                filtros += " AND i.Precio >= @precioDesde";
                countCommand.Parameters.AddWithValue("@precioDesde", precioDesde.Value);
            }

            if (precioHasta.HasValue)
            {
                filtros += " AND i.Precio <= @precioHasta";
                countCommand.Parameters.AddWithValue("@precioHasta", precioHasta.Value);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                filtros += " AND i.Estado = @estado";
                countCommand.Parameters.AddWithValue("@estado", estado);
            }

            if (activo.HasValue)
            {
                filtros += " AND i.Activo = @activo";
                countCommand.Parameters.AddWithValue("@activo", activo.Value);
            }
            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                filtros += @"
                AND i.id_inmueble NOT IN (
                    SELECT c.id_inmueble
                    FROM contrato c
                    WHERE c.activo = 1
                    AND @fechaDesde <= COALESCE(c.fecha_terminacion_anticipada, c.fecha_fin)
                    AND @fechaHasta >= c.fecha_inicio
                )";
                countCommand.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value);
                countCommand.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value);
            }

            if (idPropietario.HasValue)
            {
                filtros += " AND i.id_propietario = @idPropietario";
                countCommand.Parameters.AddWithValue("@idPropietario", idPropietario.Value);
            }


            // Total count
            countCommand.CommandText = "SELECT COUNT(*) " + filtros;
            totalItems = Convert.ToInt32(countCommand.ExecuteScalar());

            // Ahora la consulta real paginada
            var sql = @"SELECT i.*, p.Nombre, p.Apellido " + filtros + " LIMIT @limit OFFSET @offset";

            using var command = new MySqlCommand(sql, connection);

            foreach (MySqlParameter p in countCommand.Parameters)
                command.Parameters.Add((MySqlParameter)((ICloneable)p).Clone());

            command.Parameters.AddWithValue("@limit", pageSize);
            command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Inmueble
                {
                    IdInmueble = Convert.ToInt32(reader["id_inmueble"]),
                    NombreInmueble = reader["nombre_inmueble"].ToString(),
                    Direccion = reader["Direccion"].ToString(),
                    Uso = reader["Uso"].ToString(),
                    Ambientes = Convert.ToInt32(reader["Ambientes"]),
                    Precio = Convert.ToDecimal(reader["Precio"]),
                    Estado = reader["Estado"].ToString(),
                    Activo = Convert.ToInt32(reader["Activo"]),
                    Portada = reader["Portada"].ToString(),
                    Duenio = new Propietario
                    {
                        Nombre = reader["Nombre"].ToString(),
                        Apellido = reader["Apellido"].ToString()
                    }
                });
            }

            return lista;
        }


    }
}