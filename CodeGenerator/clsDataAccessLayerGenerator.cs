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
        StringBuilder _sbDataAccessClass = new StringBuilder();
        List<clsColumn> _ColumnsList = new List<clsColumn>();
        clsColumn _PrimaryKeyColumn;
        string _DatabaseName;
        string _TableSingularName;
        string _TableName;
        public clsDataAccessLayerGenerator(List<clsColumn> TableColumns, string TableName, string DatabaseName)
        {
            _TableName = TableName;
            _DatabaseName = DatabaseName;
            _ColumnsList = TableColumns;

            foreach(clsColumn Column in TableColumns)
            {
                if(Column.IsPrimaryKey)
                    _PrimaryKeyColumn = Column;
            }

            _TableSingularName = _PrimaryKeyColumn.ColumnName.Substring(0, _PrimaryKeyColumn.ColumnName.Length - 2);
        }
        private string _GetParameterList(bool WithPrimaryKey, bool WithReferences, bool WithDataType = true, string Prefix = "", bool AssigningValues = false)
        {
            string ParameterList = "";
            if(WithPrimaryKey)
            {
                if(WithDataType)
                    ParameterList = _PrimaryKeyColumn.ColumnDataType + " ";

                ParameterList += _PrimaryKeyColumn.ColumnName + ", ";
            }

            foreach(clsColumn Column in _ColumnsList)
            {
                if(Column.IsPrimaryKey)
                    continue;

                if(WithReferences)
                    ParameterList += "ref ";

                if(WithDataType)
                    ParameterList += Column.ColumnDataType + " ";

                if(AssigningValues)
                    ParameterList += "                            " + Column.ColumnName + " = ";

                ParameterList += Prefix + Column.ColumnName + ", ";

                if(AssigningValues)
                {
                    ParameterList += "\n";

                }
            }

            return ParameterList.Substring(0, ParameterList.Length - 2);
        }
        private void _GenerateUsings()
        {
            _sbDataAccessClass.Append("using System;\n");
            _sbDataAccessClass.Append("using System.Data;\n");
            _sbDataAccessClass.Append("using System.Data.SqlClient;\n");
        }
        private void _GenerateNamespace()
        {
            _sbDataAccessClass.Append($"\nnamespace {_DatabaseName}_DataAccess\n{{");
        }
        private void _GenerateClassDeclaration()
        {
            _sbDataAccessClass.Append($"\n   public class {"cls" + _TableSingularName + "Data"}\n    {{");
        }
        private void _GenerateFunction_GetObjectByID()
        {
            //
            // DONT FORGET TO CHECK FOR NULLABLE AND DIFFRENET CASTING TYPES
            // 
            StringBuilder sbGetObjectByIDFunction = new StringBuilder();
            StringBuilder sbAssignedColumns = new StringBuilder();

            foreach(clsColumn Column in _ColumnsList)
            {
                if(Column.IsPrimaryKey)
                    continue;

                if(Column.AllowNull)
                {
                    sbAssignedColumns.Append($"\n\nif(reader[\"{Column.ColumnName}\"] != DBNull.Value)");
                    sbAssignedColumns.Append($"\n{Column.ColumnName} = ({Column.ColumnDataType})reader[\"{Column.ColumnName}\"];");
                    sbAssignedColumns.Append($"\nelse");
                    sbAssignedColumns.Append($"\n{Column.ColumnName} = {Column.NullEquivalentValue};\n");
                }
                else
                    sbAssignedColumns.Append($"\n{Column.ColumnName} = ({Column.ColumnDataType})reader[\"{Column.ColumnName}\"];");
            }


            _sbDataAccessClass.Append($@"        public static bool Get{_TableSingularName}ByID({_GetParameterList(true, true)})
            {{
                bool isFound = false;

                SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
                string query = ""SELECT * FROM {_TableName} WHERE {_PrimaryKeyColumn.ColumnName} = @{_PrimaryKeyColumn.ColumnName}"";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue(""@{_PrimaryKeyColumn.ColumnName}"", {_PrimaryKeyColumn.ColumnName});

                try
                {{
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {{
                        isFound = true;
             ");

            _sbDataAccessClass.Append(sbAssignedColumns);

            _sbDataAccessClass.Append($@"
    }}
                    else
                    {{
                        isFound = false;
                    }}

                    reader.Close();
                }}
                catch (Exception ex)
                {{
                    isFound = false;
                }}
                finally
                {{
                    connection.Close();
                }}

                return isFound;
            }}");

        }
        private void _GenerateMethod_AddNewObject()
        {
            StringBuilder sbCommandParameters = new StringBuilder();

            foreach(clsColumn Column in _ColumnsList)
            {
                if(Column.IsPrimaryKey)
                    continue;

                if(Column.AllowNull)
                    sbCommandParameters.Append($@"

            if ({Column.ColumnName} != {Column.NullEquivalentValue})
                command.Parameters.AddWithValue(""@{Column.ColumnName}"", {Column.ColumnName});
            else
                command.Parameters.AddWithValue(""@{Column.ColumnName}"", DBNull.Value);
");
                else
                    sbCommandParameters.Append($"\ncommand.Parameters.AddWithValue(\"@{Column.ColumnName}\", {Column.ColumnName});");
            }
            _sbDataAccessClass.Append($@"        public static int AddNew{_TableSingularName}({_GetParameterList(false, false)})
        {{
            int {_PrimaryKeyColumn.ColumnName} = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @""INSERT INTO {_TableName} ({_GetParameterList(false, false, false)})
                            VALUES ({_GetParameterList(false, false, false, "@")})
                            SELECT SCOPE_IDENTITY();"";

            SqlCommand command = new SqlCommand(query, connection);
");
            _sbDataAccessClass.Append(sbCommandParameters.ToString());
            _sbDataAccessClass.Append($@"

            try
            {{
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {{
                    {_PrimaryKeyColumn.ColumnName} = insertedID;
                }}
            }}
            catch (Exception ex)
            {{

            }}
            finally
            {{
                connection.Close();
            }}

            return {_PrimaryKeyColumn.ColumnName};
        }}");
        }
        private void _GenerateMethod_UpdateObject()
        {
            string AssigningValues = _GetParameterList(false, false, false, "@", true);
            StringBuilder sbCommandParameters = new StringBuilder();

            foreach(clsColumn Column in _ColumnsList)
            {
                if(Column.IsPrimaryKey)
                    continue;

                sbCommandParameters.Append($"command.Parameters.AddWithValue(\"@{Column.ColumnName}\", {Column.ColumnName});");
            }

            _sbDataAccessClass.Append($@" public static bool Update{_TableSingularName}({_GetParameterList(true, false)})
        {{
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @""UPDATE {_TableName}  
                            SET 
{AssigningValues.Substring(0, AssigningValues.Length - 1)}
                            WHERE {_PrimaryKeyColumn.ColumnName} = @{_PrimaryKeyColumn.ColumnName}"";

            SqlCommand command = new SqlCommand(query, connection);
            {sbCommandParameters}
           
            try
            {{
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }}
            catch (Exception ex)
            {{
                return false;
            }}

            finally
            {{
                connection.Close();
            }}

            return (rowsAffected > 0);
        }}");
        }
        private void _GenerateMethod_DeleteObject()
        {
            _sbDataAccessClass.Append($@"
        public static bool Delete{_TableSingularName}({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})
        {{
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @""Delete {_TableName} 
                                where {_PrimaryKeyColumn.ColumnName} = @{_PrimaryKeyColumn.ColumnName}"";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue(""@{_PrimaryKeyColumn.ColumnName}"", {_PrimaryKeyColumn.ColumnName});

            try
            {{
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }}
            catch (Exception ex)
            {{
            }}
            finally
            {{
                connection.Close();
            }}

            return (rowsAffected > 0);
        }}");

        }
        private void _GenerateMethod_DoesObjectExist()
        {
            _sbDataAccessClass.Append($@"
        public static bool Does{_TableSingularName}Exist({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})
        {{
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = ""SELECT Found=1 FROM People WHERE {_PrimaryKeyColumn.ColumnName} = @{_PrimaryKeyColumn.ColumnName}"";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue(""@{_PrimaryKeyColumn.ColumnName}"", {_PrimaryKeyColumn.ColumnName});

            try
            {{
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }}
            catch (Exception ex)
            {{
                isFound = false;
            }}
            finally
            {{
                connection.Close();
            }}

            return isFound;
        }}
");
        }
        private void _GenerateMethod_GetAllObjects()
        {
            _sbDataAccessClass.Append($@" public static DataTable GetAll{_TableName}()
            {{
                DataTable dt = new DataTable();

                SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
                string query = ""SELECT * FROM {_TableName}"";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {{
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {{
                        dt.Load(reader);
                    }}

                    reader.Close();
                }}

                catch (Exception ex)
                {{
                    
                }}
                finally
                {{
                    connection.Close();
                }}

                return dt;
            }}
");

        }
        private void _GenerateClosingCurlyBrackets()
        {
            _sbDataAccessClass.AppendLine("     }");
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
        public static string ConnectionString = ""Data Source=.;Integrated Security=True;TrustServerCertificate=True;"";
    }}
}}
");
            return sbDataAccessSettingClass;
        }
    }
}
