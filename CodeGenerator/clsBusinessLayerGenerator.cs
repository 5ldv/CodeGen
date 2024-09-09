using CodeGenerator;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CodeGenerator
{
    public class clsBusinessLayerGenerator
    {
        public string TableSingularName { get; }
        public string TableName { get; }
        public string TableClassName { get; }
        StringBuilder _sbBusinessClass = new StringBuilder();
        List<clsColumn> _ColumnsList = new List<clsColumn>();
        clsColumn _PrimaryKeyColumn;
        string _DatabaseName;
        string _DALClassName;
        public clsBusinessLayerGenerator(List<clsColumn> TableColumns, string TableName, string DatabaseName)
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
            TableClassName = "cls" + TableSingularName;
            _DALClassName = "cls" + TableSingularName + "Data";
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
                    ParameterList += "\n";

                }
            }

            return ParameterList.Substring(0, ParameterList.Length - 2);
        }
        private StringBuilder _GetClassProperties()
        {
            StringBuilder _sbProperties = new StringBuilder();

            foreach (clsColumn column in _ColumnsList)
            {
                _sbProperties.Append($"        public {column.ColumnDataType} {column.ColumnName} {{ set; get; }}\n");
            }
            return _sbProperties;
        }
        private void _GenerateUsings()
        {
            _sbBusinessClass.Append("using System;\n");
            _sbBusinessClass.Append("using System.Data;\n");
            _sbBusinessClass.Append($"using {_DatabaseName}_DataAccess;\n");
        }
        private void _GenerateNamespace()
        {
            _sbBusinessClass.Append($"\nnamespace {_DatabaseName}_Business\n{{");
        }
        private void _GenerateClassDeclaration()
        {
            _sbBusinessClass.Append($"\n   public class {"cls" + TableSingularName}\n    {{");
        }
        private void _GenerateClassProperties()
        {
            _sbBusinessClass.Append($@"
        public enum enMode {{AddNew = 0,Update = 1}};
        public enMode Mode = enMode.AddNew;
");

            _sbBusinessClass.Append(_GetClassProperties());

        }
        private void _GenerateClassDefaultConstructor()
        {
            _sbBusinessClass.Append($"        public cls{TableSingularName}()\n");
            _sbBusinessClass.Append($"        {{");
            foreach (clsColumn column in _ColumnsList)
            {
                _sbBusinessClass.AppendLine($"this.{column.ColumnName} = {column.NullEquivalentValue};");
            }
            _sbBusinessClass.AppendLine("            Mode = enMode.AddNew;");
            _sbBusinessClass.AppendLine("            }");
        }
        private void _GenerateClassConstructor()
        {
            _sbBusinessClass.Append($"private cls{TableSingularName}(");
            string Parameters = "";
            foreach (clsColumn column in _ColumnsList)
            {
                Parameters += $"{column.ColumnDataType} {column.ColumnName}, ";
            }
            _sbBusinessClass.Append(Parameters.Substring(0, Parameters.Length - 2) + ")");
            _sbBusinessClass.AppendLine("        {");
            foreach (clsColumn column in _ColumnsList)
            {
                _sbBusinessClass.AppendLine($"            this.{column.ColumnName} = {column.ColumnName};");
            }
            _sbBusinessClass.AppendLine("            Mode = enMode.Update;");
            _sbBusinessClass.AppendLine("        }");
        }
        private void _GenerateAddNewObjectMethod()
        {
            _sbBusinessClass.AppendLine($"        private bool _AddNew{TableSingularName}()");
            _sbBusinessClass.AppendLine("        {");
            string Primary = "";
            string Parameters = "";

            foreach (clsColumn column in _ColumnsList)
            {
                if (column.IsPrimaryKey)
                    Primary = column.ColumnName;

                Parameters += $"this.{column.ColumnName}, ";
            }

            _sbBusinessClass.Append($"            this.{Primary} = ");
            _sbBusinessClass.AppendLine($"cls{TableSingularName}Data.AddNew{TableSingularName}({Parameters.Substring(0, Parameters.Length - 2)});");
            _sbBusinessClass.AppendLine($"            return (this.{Primary} != -1);");
            _sbBusinessClass.AppendLine("        }");
        }
        private void _GenerateUpdateObjectMethod()
        {
            _sbBusinessClass.AppendLine($"        private bool _Update{TableSingularName}()");
            _sbBusinessClass.AppendLine("        {");
            _sbBusinessClass.Append($"            return cls{TableSingularName}.Update{TableSingularName}(");
            string Parameters = "";
            foreach (clsColumn column in _ColumnsList)
            {
                Parameters += $"this.{column.ColumnName}, ";
            }
            _sbBusinessClass.Append($"{Parameters.Substring(0, Parameters.Length - 2)});");
            _sbBusinessClass.Append("        }");
        }
        private void _GenerateMethod_DeleteObject()
        {
            _sbBusinessClass.Append($@"
        public static bool Delete{TableSingularName}({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})
        {{
            return clsPersonData.Delete{TableSingularName}({_PrimaryKeyColumn.ColumnName}); 
        }}");
        }
        private void _GenerateMethod_DoesObjectExist()
        {
            _sbBusinessClass.Append($@"
        public static bool Does{TableSingularName}Exist({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})
        {{
           return clsPersonData.Does{TableSingularName}Exist({_PrimaryKeyColumn.ColumnName});
        }}");
        }
        private void _GenerateMethod_Find()
        {
            _sbBusinessClass.AppendLine($"        public static {TableClassName} Find({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})");
            _sbBusinessClass.AppendLine("        {");

            foreach (clsColumn Column in _ColumnsList)
            {
                if (!Column.IsPrimaryKey)
                    _sbBusinessClass.AppendLine($"            {Column.ColumnDataType} {Column.ColumnName} = {Column.NullEquivalentValue};");
            }
            _sbBusinessClass.AppendLine($"            \nbool IsFound = {_DALClassName}.Get{TableSingularName}ByID({_GetParameterList(true, true, false)});");
            _sbBusinessClass.AppendLine($"            \nif(IsFound)");
            _sbBusinessClass.AppendLine($"            return new {TableClassName}({_GetParameterList(true, false, false)});");
            _sbBusinessClass.AppendLine($"            else");
            _sbBusinessClass.AppendLine($"            return null;");
            _sbBusinessClass.Append("        }");
        }
        private void _GenerateSaveMethod()
        {
            _sbBusinessClass.Append($@"
        public bool Save()
        {{
            switch(Mode)
            {{
                case enMode.AddNew:
                    if(_AddNew{TableSingularName}())
                    {{
                        Mode = enMode.Update;
                        return true;
                    }}
                    else
                    {{
                        return false;
                    }}

                case enMode.Update:
                    return _Update{TableSingularName}();
            }}
            return false;
        }}");

        }
        private void _GenerateGetObjectsMethod()
        {
            _sbBusinessClass.Append($@"
        public static DataTable Get{TableName}()
        {{
            return cls{TableSingularName}Data.Get{TableName}();
        }}
");
        }
        private void _GenerateClosingCurlyBrackets()
        {
            _sbBusinessClass.AppendLine("    }");
            _sbBusinessClass.AppendLine("}");
        }
        public StringBuilder GenerateClass()
        {
            _GenerateUsings();
            _GenerateNamespace();
            _GenerateClassDeclaration();
            _GenerateClassProperties();
            _GenerateClassDefaultConstructor();
            _GenerateClassConstructor();
            _GenerateAddNewObjectMethod();
            _GenerateUpdateObjectMethod();
            _GenerateMethod_DeleteObject();
            _GenerateMethod_DoesObjectExist();
            _GenerateMethod_Find();
            _GenerateSaveMethod();
            _GenerateGetObjectsMethod();
            _GenerateClosingCurlyBrackets();
            return _sbBusinessClass;
        }
    }

}

