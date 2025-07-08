using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;



namespace CavalryCivil3DPlugin._Library._ExcelLibrary
{
    class ExcelManage
    {
        public static Dictionary<string, string> GetDefinedNames(string tempFullpath)
        {
            Dictionary<string, string> definedRanges = new Dictionary<string, string>();

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(tempFullpath, isEditable: false))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                List<DefinedName> definedNamesObject = workbookPart.Workbook.DefinedNames.Select(x => (DefinedName)x).ToList();

                // Range range = worksheetPart.Worksheet;

                foreach (DefinedName name in definedNamesObject)
                {
                    definedRanges.Add(name.Name, name.Text);
                }
            }

            return definedRanges;
        }

        public static string InitializeExcelFile()
        {
            string filePath = @"C:\Users\hnanca\Desktop\DUMP\sa.xlsx";
            string tempSuffixName = "_Temp.xlsx";
            string tempFullPath = "";

            if (File.Exists(filePath))
            {
                string parentPath = Path.GetDirectoryName(filePath);
                string childPath = Path.GetFileNameWithoutExtension(filePath);
                string tempFileName = childPath + tempSuffixName;
                tempFullPath = Path.Combine(parentPath, tempFileName);

                File.Copy(filePath, tempFullPath, true);
            }
            return tempFullPath;
        }

        public static List<List<string>> GetCellRange(string range)
        {
            // range = "A1K6";
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            string start = range.Split(':')[0];
            string end = range.Split(':')[1];

            char currentColumn = start[0];
            char lastColumn = end[0];

            //char currentColumn = 'A';
            //char lastColumn = 'K';

            int currentColumnNumber = Array.IndexOf(alpha, currentColumn);
            int lastColumnNumber = Array.IndexOf(alpha, lastColumn);

            char[] alphaColumn = alpha.Skip(currentColumnNumber).Take(lastColumnNumber + 1).ToArray();

            int currentRow = Convert.ToInt32(start.Substring(1).ToString());
            int lastRow = Convert.ToInt32(end.Substring(1).ToString());

            //int currentRow = 1;
            //int lastRow = 6;

            List<List<string>> totalCells = new List<List<string>>();

            while (currentRow <= lastRow)
            {
                List<string> currentRowCells = new List<string>();

                foreach (char column in alphaColumn)
                {
                    currentRowCells.Add($"{column}{currentRow}");
                }
                totalCells.Add(currentRowCells);
                currentRow++;
            }
            return totalCells;
        }

        public static List<List<string>> ReadRange(string tempFullpath, Dictionary<string, string> ranges)
        {
            string defaultRange = "cc";
            string cellRange = ranges[defaultRange];
            string decodedRange = DecodeRange(cellRange);

            List<List<string>> allData = new List<List<string>>();
            List<List<string>> cellReferences = GetCellRange(decodedRange);

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(tempFullpath, isEditable: false))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().First();
                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                if (sheetData.Elements<Row>().Any()) //-> Checking if the workbook contains any data.
                {
                    foreach (var row in cellReferences)
                    {
                        List<string> rowData = new List<string>();
                        foreach (string rangeReference in row)
                        {
                            Cell cell = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == rangeReference);

                            string value = "<blank>";

                            if (cell.CellValue != null)
                            {
                                value = cell.CellValue.Text;
                            }
                            // If the cell contains a shared string, retrieve the actual value
                            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                            {
                                SharedStringTablePart sharedStringPart = workbookPart.SharedStringTablePart;
                                if (sharedStringPart != null)
                                    value = sharedStringPart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                            }
                            rowData.Add(value);
                        }
                        allData.Add(rowData);
                    }
                }
            }
            return allData;
        }

        private static string DecodeRange(string codedRange)
        {
            string decode1 = codedRange.Split('!')[1].Replace("$", "");
            return decode1;
        }

    }
}
