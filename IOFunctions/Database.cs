using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Utilities.IOFunctions
{
    public enum DBType
    {
        MySQL,
        SQLServer,
        PostgreSQL
    }

    /// <summary>
    /// This class is used to abstract a Database connection with MySql or SqlServer and handle
    /// creating the DbConnection object, DbCommand object, and returning the results of 
    /// Select commands. The Enum DBType is used to specify which DB is used.
    /// </summary>

    public class Database
    {
        private DBType _dbType;
        private DbConnection _dbConnection;
        private DbCommand _dbCommand;

        public Database(string connection, DBType typeOfDB)
        {
            _dbType = typeOfDB;
            _dbConnection = CreateDbConnection(connection);
        }

        public List<object[]> Select(string selectStatement)
        {
            var data = new List<object[]>();

            _dbConnection.Open();
            _dbCommand = CreateDbCommand(selectStatement, _dbConnection);
            DbDataReader reader = _dbCommand.ExecuteReader();

            while (reader.Read())
            {
                object[] line = new object[reader.FieldCount];
                reader.GetValues(line);
                data.Add(line);
            }

            reader.Close();
            _dbConnection.Close();
            return data;
        }

        #region Helper Methods
        private DbConnection CreateDbConnection(string connection)
        {
            if (_dbType == DBType.MySQL)
                return new MySqlConnection(connection);
            else if (_dbType == DBType.SQLServer)
                return new SqlConnection(connection);
            else if (_dbType == DBType.PostgreSQL)
                return new NpgsqlConnection(connection);
            throw new Exception("Unimplemented type of DB.");
        }

        private DbCommand CreateDbCommand(string sqlCommand, DbConnection dbConnection)
        {
            if (_dbType == DBType.MySQL)
                return new MySqlCommand(sqlCommand, dbConnection as MySqlConnection);
            else if (_dbType == DBType.SQLServer)
                return new SqlCommand(sqlCommand, dbConnection as SqlConnection);
            else if (_dbType == DBType.PostgreSQL)
                return new NpgsqlCommand(sqlCommand, dbConnection as NpgsqlConnection);
            throw new Exception("Unimplemented type of DBConnection.");
        }
        #endregion
    }
}
