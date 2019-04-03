using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace TripList
{
    public class Excel
    {
        string path;

        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;

        public Excel(string path, int sheet)
        {
            this.path = path;
            excel = new _Excel.Application();

            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[sheet];
        }

        public void ChangeSheet(int sheet)
        {
            ws = wb.Worksheets[sheet];
        }

        public string ReadCell(int i, int j)
        {
            // i++;
            // j++;

            if (ws.Cells[i, j].Value2 != null)
            {
                return ws.Cells[i, j].Value2.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public void WriteToCell(int i, int j, string value)
        {
            ws.Cells[i, j].Value2 = value;
        }

        public void Save(string path)
        {
            wb.SaveAs(path);
        }

        public void Close()
        {
            try
            {
                wb.Close();
            }
            catch (Exception e)
            { //TODO: Исключение никак не обработано

            }
        }

        // Определение высоты для ячейки
        public void AutoFit_MergeCell(_Excel.Worksheet objSheet, string FirstCell, string SecondCell)
        {
            //FirstCell - объединенная ячейка, в которой хотим выставить высоту
            //SecondCell - обыкновенная ячейка, с такой же шириной как и у FirstCell

            //авто выставление высоты объединенных ячеек
            if (objSheet.get_Range(FirstCell, FirstCell).Value2 != null)    // скопировать текст из объединенной ячейки
            {
                objSheet.get_Range(SecondCell, SecondCell).Value2 =
                    objSheet.get_Range(FirstCell, FirstCell).Value2.ToString();
            }

            objSheet.get_Range(SecondCell, SecondCell).EntireRow.AutoFit(); //применить автовысоту
            double CellHeight = objSheet.get_Range(SecondCell, SecondCell).RowHeight;//узнать высоту
            objSheet.get_Range(SecondCell, SecondCell).Value2 = ""; //очистить ячейку 

            objSheet.get_Range(FirstCell, FirstCell).RowHeight = CellHeight; //и задать нужную высоту для объединенной ячейки
        }
    }
}
