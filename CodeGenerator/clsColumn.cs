using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class clsColumn
    {
        public string ColumnName { get; set; }
        public string ColumnDataType { get; set; }
        public bool AllowNull { get; set; }
        public bool IsPrimaryKey { get; set; }
        public string NullEquivalentValue
        {

            get
            {
                if (AllowNull || IsPrimaryKey) { 
                return "null";
                }

                switch (this.ColumnDataType)
                {
                    case "string":
                        return "\"\"";
                    case "char":
                        return @"'\0'";
                    case "int":
                    case "int32":
                    case "int64":
                    case "long":
                    case "short":
                        return "-1";
                    case "float":
                    case "double":
                    case "decimal":
                        return "-1";
                    case "byte":
                        return "0";
                    case "bool":
                        return "false";
                    case "DateTime":
                        return "DateTime.Now";
                    default:
                        return "-1";
                }
            }
        }
        public clsColumn(string ColumnName, string ColumnDataType, bool AllowNull, bool IsPrimaryKey)
        {
            this.ColumnName = ColumnName;
            this.ColumnDataType = ColumnDataType;
            this.AllowNull = AllowNull;
            this.IsPrimaryKey = IsPrimaryKey;
        }

    }
}
