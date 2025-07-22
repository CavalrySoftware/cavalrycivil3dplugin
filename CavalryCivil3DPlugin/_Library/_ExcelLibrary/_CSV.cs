using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using CavalryCivil3DPlugin.Consoles;
using Path = System.IO.Path;

namespace CavalryCivil3DPlugin._Library._ExcelLibrary
{
    public class _CSV
    {

        public static string GetTempFileCSV(string _path)
        {

            string tempSuffixName = "_Temp.csv";
            string tempFullPath = "";

            if (File.Exists(_path))
            {
                string parentPath = Path.GetDirectoryName(_path);
                string childPath = Path.GetFileNameWithoutExtension(_path);
                string tempFileName = childPath + tempSuffixName;
                tempFullPath = Path.Combine(parentPath, tempFileName);

                File.Copy(_path, tempFullPath, true);
            }

            return tempFullPath;
        }


        public static Dictionary<string, Dictionary<string, string>> GetDictionaryByFirstColumn(string _csvPath)
        {
            //StaticConsole console = new StaticConsole();  
            string tempPath = GetTempFileCSV(_csvPath);
            Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();

            if (!File.Exists(_csvPath)) return dict; 


            int index = 0;
            List<string> headers = new List<string>();

            var allLines = File.ReadAllLines(tempPath);


            foreach (string line in File.ReadLines(tempPath).ToList())
            {
                Dictionary<string, string> rowDictionary = new Dictionary<string, string>();
                string id = "";

                if (index == 0)
                {
                    headers = line.Split(',').ToList();
                    index++;
                    continue;
                };

                int column = 0;

                foreach (string entry in line.Split(',').ToList())
                {
                    if (column == 0)
                    {
                        id = entry;
                        column++;
                        continue;
                    }

                    rowDictionary[headers[column]] = entry;
                    column++;
                }

                dict[id] = rowDictionary;

            }

            File.Delete(tempPath);
            return dict;
        }

    }
}
