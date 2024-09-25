using CodeGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class clsDataAccessLayerGenerator
    {
        public string TableSingularName { get; }
        public string TableName { get; }
        public string TableClassName { get; }
        StringBuilder _sbDataAccessClass = new StringBuilder();
        List<clsColumn> _ColumnsList = new List<clsColumn>();
        clsColumn _PrimaryKeyColumn;
        string _DatabaseName;
        public clsDataAccessLayerGenerator(List<clsColumn> TableColumns, string TableName, string DatabaseName)
        {
            this.TableName = TableName;
            _DatabaseName = DatabaseName;
            _ColumnsList = TableColumns;
            foreach (clsColumn Column in TableColumns)
            {
                if (Column.IsPrimaryKey)
                    _PrimaryKeyColumn = Column;
            }

            TableSingularName = _PrimaryKeyColumn.ColumnName.Substring(0, _PrimaryKeyColumn.ColumnName.Length - 2);
            TableClassName = $"cls{TableSingularName}Data";


        }
        private string _GetParameterList(bool WithPrimaryKey, bool WithReferences, bool WithDataType = true, string Prefix = "", bool AssigningValues = false)
        {
            string ParameterList = "";
            if (WithPrimaryKey)
            {
                if (WithDataType)
                    ParameterList = _PrimaryKeyColumn.ColumnDataType + " ";

                ParameterList += _PrimaryKeyColumn.ColumnName + ", ";
            }

            foreach (clsColumn Column in _ColumnsList)
            {
                if (Column.IsPrimaryKey)
                    continue;

                if (WithReferences)
                    ParameterList += "ref ";

                if (WithDataType)
                    ParameterList += Column.ColumnDataType + " ";

                if (AssigningValues)
                    ParameterList += "                            " + Column.ColumnName + " = ";

                ParameterList += Prefix + Column.ColumnName + ", ";

                if (AssigningValues)
                {
                    ParameterList += "\r\n";

                }
            }

            return ParameterList.Substring(0, ParameterList.Length - 2);
        }
        private void _GenerateUsings()
        {
            _sbDataAccessClass.Append("using System;\r\n");
            _sbDataAccessClass.Append("using System.Data;\r\n");
            _sbDataAccessClass.Append("using System.Data.SqlClient;\r\n");
        }
        private void _GenerateNamespace()
        {
            _sbDataAccessClass.Append($"\r\nnamespace {_DatabaseName}_DataAccess\r\n{{");
        }
        private void _GenerateClassDeclaration()
        {
            _sbDataAccessClass.AppendLine($"\r\n    public class {TableClassName}\r\n    {{");
        }
        private void _GenerateFunction_GetObjectByID()
        {
            StringBuilder sbAssignedColumns = new StringBuilder();

            foreach (clsColumn Column in _ColumnsList)
            {
                if (Column.IsPrimaryKey)
                    continue;

                if (Column.AllowNull)
                {
                    sbAssignedColumns.Append($"\r\n\r\n                                if(reader[\"{Column.ColumnName}\"] != DBNull.Value)");
                    sbAssignedColumns.Append($"\r\n                                    {Column.ColumnName} = ({Column.ColumnDataType})reader[\"{Column.ColumnName}\"];");
                    sbAssignedColumns.Append($"\r\n                                else");
                    sbAssignedColumns.Append($"\r\n                                    {Column.ColumnName} = {Column.NullEquivalentValue};\r\n");
                }
                else
                    sbAssignedColumns.Append($"\r\n                                {Column.ColumnName} = ({Column.ColumnDataType})reader[\"{Column.ColumnName}\"];");
            }
            _sbDataAccessClass.AppendLine($@"        public static bool Get{TableSingularName}ByID({_GetParameterList(true, true)})
        {{
            bool isFound = false;

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = ""SELECT * FROM {TableName} WHERE {_PrimaryKeyColumn.ColumnName} = @{_PrimaryKeyColumn.ColumnName}"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.Parameters.AddWithValue(""@{_PrimaryKeyColumn.ColumnName}"", {_PrimaryKeyColumn.ColumnName});

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {{
                            if (reader.Read())
                            {{
                                isFound = true;

{sbAssignedColumns}
                            }}
                            else
                                isFound = false;
                        }}
                    }}
                }}
            }}
            catch (Exception ex)
            {{
                isFound = false;
            }}

            return isFound;
        }}");
        }
        private void _GenerateMethod_AddNewObject()
        {
            StringBuilder sbCommandParameters = new StringBuilder();

            foreach (clsColumn Column in _ColumnsList)
            {
                if (Column.IsPrimaryKey)
                    continue;

                if (Column.AllowNull)
                    sbCommandParameters.Append($@"

                        if({Column.ColumnName} != {Column.NullEquivalentValue})
                            command.Parameters.AddWithValue(""@{Column.ColumnName}"", {Column.ColumnName});
                        else
                            command.Parameters.AddWithValue(""@{Column.ColumnName}"", DBNull.Value);");
                else
                    sbCommandParameters.Append($"\r\n                        command.Parameters.AddWithValue(\"@{Column.ColumnName}\", {Column.ColumnName});");
            } 
            _sbDataAccessClass.AppendLine($@"        public static int AddNew{TableSingularName}({_GetParameterList(false, false)})
        {{
            int {_PrimaryKeyColumn.ColumnName} = -1;

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = @""INSERT INTO {TableName} ({_GetParameterList(false, false, false)})
                            VALUES ({_GetParameterList(false, false, false, "@")})
                            SELECT SCOPE_IDENTITY();"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
{sbCommandParameters}

                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                            {_PrimaryKeyColumn.ColumnName} = insertedID;
                    }}
                }}
            }}
            catch (Exception ex)
            {{

            }}

            return {_PrimaryKeyColumn.ColumnName};
        }}");

        }
        private void _GenerateMethod_UpdateObject()
        {
            string AssigningValues = _GetParameterList(false, false, false, "@", true);
            StringBuilder sbCommandParameters = new StringBuilder();

            foreach (clsColumn Column in _ColumnsList)
            {
                sbCommandParameters.Append($"\r\n                        command.Parameters.AddWithValue(\"@{Column.ColumnName}\", {Column.ColumnName});");
            }

            _sbDataAccessClass.AppendLine($@"        public static bool Update{TableSingularName}({_GetParameterList(true, false)})
        {{
            int rowsAffected = 0;

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = @""UPDATE {TableName}  
                            SET 
{AssigningValues.Substring(0, AssigningValues.Length - 2)}
                            WHERE {_PrimaryKeyColumn.ColumnName} = @{_PrimaryKeyColumn.ColumnName}"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
{sbCommandParameters}

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }}
                }}
            }}
            catch (Exception ex)
            {{
                return false;
            }}

            return (rowsAffected > 0);
        }}");
        }
        private void _GenerateMethod_DeleteObject()
        {
            _sbDataAccessClass.AppendLine($@"        public static bool Delete{TableSingularName}({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})
        {{
            int rowsAffected = 0;

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = @""Delete {TableName} 
                                where {_PrimaryKeyColumn.ColumnName} = @{_PrimaryKeyColumn.ColumnName}"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.Parameters.AddWithValue(""@{_PrimaryKeyColumn.ColumnName}"", {_PrimaryKeyColumn.ColumnName});

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }}
                }}

            }}
            catch (Exception ex)
            {{

            }}

            return (rowsAffected > 0);
        }}");

        }
        private void _GenerateMethod_DoesObjectExist()
        {
            _sbDataAccessClass.AppendLine($@"        public static bool Does{TableSingularName}Exist({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})
        {{
            bool isFound = false;

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = ""SELECT Found = 1 FROM {TableName} WHERE {_PrimaryKeyColumn.ColumnName} = @{_PrimaryKeyColumn.ColumnName}"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.Parameters.AddWithValue(""@{_PrimaryKeyColumn.ColumnName}"", {_PrimaryKeyColumn.ColumnName});

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        isFound = reader.HasRows;
                    }}
                }}
            }}
            catch (Exception ex)
            {{
                isFound = false;
            }}

            return isFound;
        }}");
        }
        private void _GenerateMethod_GetAllObjects()
        {
            _sbDataAccessClass.AppendLine($@"        public static DataTable GetAll{TableName}()
        {{
            DataTable dt = new DataTable();

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = ""SELECT * FROM {TableName}"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                            dt.Load(reader);
                    }}
                }}
            }}
            catch (Exception ex)
            {{

            }}

            return dt;
        }}");

        }
        private void _GenerateClosingCurlyBrackets()
        {
            _sbDataAccessClass.AppendLine("    }");
            _sbDataAccessClass.AppendLine("}");
        }
        public StringBuilder GenerateClass()
        {
            _GenerateUsings();
            _GenerateNamespace();
            _GenerateClassDeclaration();
            _GenerateFunction_GetObjectByID();
            _GenerateMethod_AddNewObject();
            _GenerateMethod_UpdateObject();
            _GenerateMethod_DeleteObject();
            _GenerateMethod_DoesObjectExist();
            _GenerateMethod_GetAllObjects();
            _GenerateClosingCurlyBrackets();
            return _sbDataAccessClass;
        }
        public static StringBuilder GenerateDataAccessSettingsClass(string DatabaseName)
        {
            StringBuilder sbDataAccessSettingClass = new StringBuilder();
            sbDataAccessSettingClass.Append($@"using System;

namespace {DatabaseName}_DataAccess
{{
    static class clsDataAccessSettings
    {{
        public static string ConnectionString = ""Data Source=.;Database={DatabaseName};Integrated Security=True;TrustServerCertificate=True;"";
    }}
}}
");
            return sbDataAccessSettingClass;
        }
    }
}
