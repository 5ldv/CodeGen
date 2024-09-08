using CodeGenerator;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CodeGenerator
{
    public class clsBusinessLayerGenerator
    {
        StringBuilder _sbBusinessClass = new StringBuilder();
        List<clsColumn> _ColumnsList = new List<clsColumn>();
        clsColumn _PrimaryKeyColumn;
        string _DatabaseName;
        string _TableSingularName;
        string _TableName;
        string _BusinessClassName;
        string _DataClassName;
        public clsBusinessLayerGenerator(List<clsColumn> TableColumns, string TableName, string DatabaseName)
        {
            _TableName = TableName;
            _DatabaseName = DatabaseName;
            _ColumnsList = TableColumns;
            foreach (clsColumn Column in TableColumns)
            {
                if (Column.IsPrimaryKey)
                    _PrimaryKeyColumn = Column;
            }

            _TableSingularName = _PrimaryKeyColumn.ColumnName.Substring(0, _PrimaryKeyColumn.ColumnName.Length - 2);
            _BusinessClassName = "cls" + _TableSingularName;
            _DataClassName = "cls" + _TableSingularName + "Data";
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
            _sbBusinessClass.Append($"\n   public class {"cls" + _TableSingularName}\n    {{");
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
            _sbBusinessClass.Append($"        public cls{_TableSingularName}()\n");
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
            _sbBusinessClass.Append($"private cls{_TableSingularName}(");
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
            _sbBusinessClass.AppendLine($"        private bool _AddNew{_TableSingularName}()");
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
            _sbBusinessClass.AppendLine($"cls{_TableSingularName}Data.AddNew{_TableSingularName}({Parameters.Substring(0, Parameters.Length - 2)});");
            _sbBusinessClass.AppendLine($"            return (this.{Primary} != -1);");
            _sbBusinessClass.AppendLine("        }");
        }
        private void _GenerateUpdateObjectMethod()
        {
            _sbBusinessClass.AppendLine($"        private bool _Update{_TableSingularName}()");
            _sbBusinessClass.AppendLine("        {");
            _sbBusinessClass.Append($"            return cls{_TableSingularName}.Update{_TableSingularName}(");
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
        public static bool Delete{_TableSingularName}({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})
        {{
            return clsPersonData.Delete{_TableSingularName}({_PrimaryKeyColumn.ColumnName}); 
        }}");
        }
        private void _GenerateMethod_DoesObjectExist()
        {
            _sbBusinessClass.Append($@"
        public static bool Does{_TableSingularName}Exist({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})
        {{
           return clsPersonData.Does{_TableSingularName}Exist({_PrimaryKeyColumn.ColumnName});
        }}");
        }
        private void _GenerateMethod_Find()
        {
            _sbBusinessClass.AppendLine($"        public static {_BusinessClassName} Find({_PrimaryKeyColumn.ColumnDataType} {_PrimaryKeyColumn.ColumnName})");
            _sbBusinessClass.AppendLine("        {");

            foreach (clsColumn Column in _ColumnsList)
            {
                if (!Column.IsPrimaryKey)
                    _sbBusinessClass.AppendLine($"            {Column.ColumnDataType} {Column.ColumnName} = {Column.NullEquivalentValue};");
            }
            _sbBusinessClass.AppendLine($"            \nbool IsFound = {_DataClassName}.Get{_TableSingularName}ByID({_GetParameterList(true, true, false)});");
            _sbBusinessClass.AppendLine($"            \nif(IsFound)");
            _sbBusinessClass.AppendLine($"            return new {_BusinessClassName}({_GetParameterList(true, false, false)});");
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
                    if(_AddNew{_TableSingularName}())
                    {{
                        Mode = enMode.Update;
                        return true;
                    }}
                    else
                    {{
                        return false;
                    }}

                case enMode.Update:
                    return _Update{_TableSingularName}();
            }}
            return false;
        }}");

        }
        private void _GenerateGetObjectsMethod()
        {
            _sbBusinessClass.Append($@"
        public static DataTable Get{_TableName}()
        {{
            return cls{_TableSingularName}Data.Get{_TableName}();
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

