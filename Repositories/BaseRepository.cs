using System;
using System.Collections.Generic;
using InmobiliariaApp.Data;
using MySql.Data.MySqlClient;

namespace InmobiliariaApp.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly DatabaseConnection _dbConnection;
        protected readonly string _tableName;

        protected BaseRepository(DatabaseConnection dbConnection, string tableName)
        {
            _dbConnection = dbConnection;
            _tableName = tableName;
        }

        // Método para obtener todos los registros
        public virtual List<T> GetAll(string fields = "*", string condition = "")
        {
            var items = new List<T>();

            using var connection = _dbConnection.GetConnection();
            string query = $"SELECT {fields} FROM {_tableName} " + (string.IsNullOrEmpty(condition) ? "" : $"WHERE {condition}");
            using var command = new MySqlCommand(query, connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(MapToEntity(reader));
            }

            return items;
        }

        // Método para obtener un registro por ID
        public virtual T? GetById(int id, string fields = "*")
        {
            using var connection = _dbConnection.GetConnection();
            string query = $"SELECT {fields} FROM {_tableName} WHERE id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapToEntity(reader);
            }

            return null;
        }

        // Método para agregar un nuevo registro
        public virtual void Add(string columns, string values, params MySqlParameter[] parameters)
        {
            using var connection = _dbConnection.GetConnection();
            string query = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";

            using var command = new MySqlCommand(query, connection);
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            command.ExecuteNonQuery();
        }

        // Método para actualizar un registro
        public virtual void Update(string setClause, int id)
        {
            using var connection = _dbConnection.GetConnection();
            string query = $"UPDATE {_tableName} SET {setClause} WHERE id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un registro
        public virtual void Delete(int id)
        {
            using var connection = _dbConnection.GetConnection();
            string query = $"DELETE FROM {_tableName} WHERE id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        // Método abstracto para mapear un registro a una entidad
        protected abstract T MapToEntity(MySqlDataReader reader);
    }
}