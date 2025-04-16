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

        public InmuebleRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

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
                    Nombre = reader.GetString("nombre"),
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
            using var command = new MySqlCommand(@"
                SELECT i.*, p.nombre, p.apellido, t.nombre as tipo_nombre FROM inmueble i
                JOIN propietario p ON i.id_propietario = p.id_propietario
                JOIN tipo_inmueble t ON i.id_tipo_inmueble = t.id_tipo_inmueble
                WHERE i.id_inmueble = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Inmueble
                {
                    IdInmueble = reader.GetInt32("id_inmueble"),
                    Nombre = reader.GetString("nombre"),
                    Duenio = new Propietario
                    {
                        IdPropietario = reader.GetInt32("id_propietario"),
                        Nombre = reader.GetString("nombre"),
                        Apellido = reader.GetString("apellido"),
                    },
                    Direccion = reader.GetString("direccion"),
                    Coordenadas = reader.IsDBNull(reader.GetOrdinal("coordenadas"))
                        ? null
                        : reader.GetString("coordenadas"),
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
                    Portada = reader.IsDBNull(reader.GetOrdinal("portada"))
                        ? null
                        : reader.GetString("portada")
                };
            }

            return null; // Devuelve null si no se encuentra ningún inmueble
        }

        // Método para agregar un nuevo inmueble
        public void Add(Inmueble inmueble)
        {
            using var connection = _dbConnection.GetConnection();
            using var command = new MySqlCommand(
                "INSERT INTO inmueble (id_propietario, nombre, direccion, coordenadas, uso, id_tipo_inmueble, ambientes, precio, estado) " +
                "VALUES (@IdPropietario, @Nombre, @Direccion, @Coordenadas, @Uso, @IdTipoInmueble, @Ambientes, @Precio, @Estado)", connection);

            command.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
            command.Parameters.AddWithValue("@Nombre", inmueble.Nombre);
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
                "UPDATE inmueble SET id_propietario = @IdPropietario, nombre = @Nombre, direccion = @Direccion, coordenadas = @Coordenadas, " +
                "uso = @Uso, id_tipo_inmueble = @IdTipoInmueble, ambientes = @Ambientes, precio = @Precio, estado = @Estado " +
                "WHERE id_inmueble = @IdInmueble", connection);

            command.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);
            command.Parameters.AddWithValue("@Nombre", inmueble.Nombre);
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

    }
}