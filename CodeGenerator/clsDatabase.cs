using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace CodeGenerator
{
    public class clsDatabase
    {
        public static List<string> GetAllDatabases()
        {
            List<string> Databases = new List<string>();
            string query = "SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')";
            try
            {
                using(SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using(SqlCommand sqlCommand = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using(SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                Databases.Add(reader["name"].ToString());
                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {

            }
            return Databases;
        }
        public static List<string> GetTablesOfDatabase(string Database)
        {
            List<string> TableNames = new List<string>();
            string query = $"USE {Database} SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME <> 'sysdiagrams' ";
            try
            {
                using(SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using(SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                TableNames.Add(reader["TABLE_NAME"].ToString());
                            }
                        }

                    }
                }
            }
            catch(Exception ex)
            {

            }
            return TableNames;
        }
        public static List<clsColumn> GetColumnsOfTable(string Database, string Table)
        {
            List<clsColumn> ColumnsList = new List<clsColumn>();
            string query = $@"USE {Database}
SELECT COLUMN_NAME AS ColumnName, 
       CASE 
           WHEN DATA_TYPE IN ('nvarchar', 'varchar', 'char', 'text', 'nchar') THEN 'string'
           WHEN DATA_TYPE = 'int' THEN 'int'
           WHEN DATA_TYPE = 'bigint' THEN 'long'
           WHEN DATA_TYPE = 'smallint' THEN 'short'
           WHEN DATA_TYPE = 'tinyint' THEN 'byte'
           WHEN DATA_TYPE = 'bit' THEN 'bool'
           WHEN DATA_TYPE IN ('decimal', 'numeric', 'money', 'smallmoney') THEN 'decimal'
           WHEN DATA_TYPE = 'float' THEN 'double'
           WHEN DATA_TYPE = 'real' THEN 'float'
           WHEN DATA_TYPE IN ('datetime', 'datetime2', 'smalldatetime', 'date') THEN 'DateTime'
           WHEN DATA_TYPE = 'time' THEN 'TimeSpan'
           WHEN DATA_TYPE = 'uniqueidentifier' THEN 'Guid'
           WHEN DATA_TYPE IN ('binary', 'varbinary') THEN 'byte[]'
           ELSE 'object'
       END + 
       CASE 
           WHEN ((IS_NULLABLE = 'YES' AND DATA_TYPE NOT IN ('nvarchar', 'varchar', 'char', 'text', 'nchar')) 
                 OR COLUMN_NAME IN (
                     SELECT COLUMN_NAME 
                     FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                     WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_NAME), 'IsPrimaryKey') = 1 
                       AND TABLE_NAME = '{Table}'
                 )) 
           THEN '?' 
           ELSE ''
       END AS DataType,
       
       CAST(CASE 
           WHEN IS_NULLABLE = 'YES' THEN 1 
           ELSE 0
       END AS bit) AS AllowNull,

       CAST(CASE 
           WHEN COLUMN_NAME IN (
               SELECT COLUMN_NAME 
               FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
               WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_NAME), 'IsPrimaryKey') = 1 
                 AND TABLE_NAME = '{Table}'
           ) THEN 1
           ELSE 0
       END AS bit) AS IsPrimaryKey
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = '{Table}';
";
            try
            {
                using(SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using(SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                string ColumnName = reader["ColumnName"].ToString();
                                string DataType = reader["DataType"].ToString();
                                bool AllowNull = (bool)reader["AllowNull"];
                                bool IsPrimaryKey = (bool)reader["IsPrimaryKey"];

                                clsColumn Column = new clsColumn(ColumnName, DataType, AllowNull, IsPrimaryKey);
                                ColumnsList.Add(Column);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return ColumnsList;

        }
        public static bool DoesDatabaseExist(string DatabaseName)
        {
            bool DoesExists = false;
            string query = $"SELECT Found = 1 FROM sys.databases WHERE name = @DatabaseName";
            try
            {
                using(SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using(SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DatabaseName", DatabaseName);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        DoesExists = reader.HasRows;
                    }
                }
            }
            catch(Exception ex)
            {
                DoesExists = false;
            }

            return DoesExists;
        }

    }

}

