using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeGen.Global
{
    public class clsUtil
    {
        public static bool Createfile(string FolderPath, string FileName, string Content)
        {
            string FilePath = Path.Combine(FolderPath, FileName);

            if (File.Exists(FilePath))
                return false;

            File.WriteAllText(FilePath, Content);
            return true;
        }
        public static bool DoesFileExist(string FolderPath, string FileName)
        {
            string FilePath = Path.Combine(FolderPath, FileName);

            if (File.Exists(FilePath))
                return true;
            else
                return false;
        }
        public static bool DoesFileExist(string FilePath)
        {
            if (File.Exists(FilePath))
                return true;
            else
                return false;
        }
    }
}
