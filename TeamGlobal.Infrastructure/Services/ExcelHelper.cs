using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using TeamGlobal.Infrastructure.ViewModel;

namespace TeamGlobal.Infrastructure.Services
{
    public class ExcelHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string FromDate { set; get; }
        private string ToDate { set; get; }
        private string Origin { set; get; }
        private string Destination { set; get; }

        public ExcelHelper(string fromDate, string toDate, string origin, string destination)
        {
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.Origin = origin;
            this.Destination = destination;
        }

        public string GetContext(List<SearchResultViewModel> SearchResultList)
        {
            try
            {
                var excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;
                var workbook = excel.Workbooks.Add(Type.Missing);

                var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;

                worksheet.Name = "Team Global";

                worksheet.Range["A1:D1"].Merge();

                worksheet.Range["A1:D1"].Value = "Sailing Schedule";
                worksheet.Cells[1, 1].EntireRow.Font.Bold = true;

                worksheet.Cells[3, 1] = "Origin";
                worksheet.Cells[3, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[3, 1]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[3, 1].Font.Bold = true;

                worksheet.Cells[3, 2] = Origin;
                worksheet.Cells[3, 2].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[3, 1]).EntireColumn.ColumnWidth = 25;

                worksheet.Cells[4, 1] = "Destination";
                worksheet.Cells[4, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[4, 1]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[4, 1].Font.Bold = true;

                worksheet.Cells[4, 2] = Destination;
                worksheet.Cells[4, 2].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[3, 1]).EntireColumn.ColumnWidth = 25;

                worksheet.Range["A6:I6"].Merge();
                worksheet.Range["A6:I6"].Value = string.Format($"Displaying {SearchResultList.Count} vessel based on searching cutoff dates between {FromDate} and {ToDate}.");
                worksheet.Range["A6:I6"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                worksheet.Cells[8, 1] = "Direct/Transhipment";
                worksheet.Cells[8, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 1]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 1].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 2] = "Vessel/Voyage";
                worksheet.Cells[8, 2].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 2]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 2].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 3] = "Cut-off Date/time";
                worksheet.Cells[8, 3].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 3]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 3].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 4] = "ETD";
                worksheet.Cells[8, 4].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 4]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 4].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 5] = "ETA CFS";
                worksheet.Cells[8, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 5]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 5].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 6] = "Port of Loading";
                worksheet.Cells[8, 6].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 6]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 6].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 7] = "Transit Time Cut-Off Origin CFS to Port Of Discharge";
                worksheet.Cells[8, 7].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 7]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 7].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 8] = "Transit Time Port of Loading to Port of Discharge";
                worksheet.Cells[8, 8].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 8]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 8].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 9] = "Transit Time Port of Loading to CFS Destination";
                worksheet.Cells[8, 9].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 9]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 9].EntireRow.Font.Bold = true;

                worksheet.Cells[8, 10] = "Transit Time Cut-Off Origin CFS to CFS Destination";
                worksheet.Cells[8, 10].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8, 10]).EntireColumn.ColumnWidth = 25;
                worksheet.Cells[8, 10].EntireRow.Font.Bold = true;

                for (int i = 0; i < SearchResultList.Count; i++)
                {
                    AddData(worksheet, SearchResultList);
                }

                var data = DateTime.Now.Ticks;

                var path = System.Web.Hosting.HostingEnvironment.MapPath("\\Report\\") + data + ".xlsx";

                workbook.SaveAs(path); ;
                workbook.Close();

                excel.Quit();

                releaseObject(worksheet);
                releaseObject(workbook);
                releaseObject(excel);
                return path;
            }
            catch (Exception exception)
            {
                log.Debug(exception.Message);
                log.Debug(exception.StackTrace);
                throw;
            }
        }

        private void AddData(Worksheet worksheet, List<SearchResultViewModel> searchResultList)
        {
            for (int i = 1; i <= searchResultList.Count; i++)
            {
                worksheet.Cells[8 + i, 1] = searchResultList[i - 1].DirectTransship;
                worksheet.Cells[8 + i, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 1]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 1].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 2] = searchResultList[i - 1].VesselVoyage;
                worksheet.Cells[8 + i, 2].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 2]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 2].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 3] = searchResultList[i - 1].CutOffDate;
                worksheet.Cells[8 + i, 3].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 3]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 3].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 4] = searchResultList[i - 1].ETD;
                worksheet.Cells[8 + i, 4].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 4]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 4].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 5] = searchResultList[i - 1].ETA;
                worksheet.Cells[8 + i, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 5]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 5].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 6] = searchResultList[i - 1].PortOfLoading;
                worksheet.Cells[8 + i, 6].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 6]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 6].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 7] = searchResultList[i - 1].TransitTimeCutOffOriginCFStoPortOfDischarge; //"Transit Time Cut-Off Origin CFS to Port Of Discharge";
                worksheet.Cells[8 + i, 7].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 7]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 7].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 8] = searchResultList[i - 1].TransitTimePortofLoadingtoPortofDischarge; //"Transit Time Port of Loading to Port of Discharge";
                worksheet.Cells[8 + i, 8].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 8]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 8].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 9] = searchResultList[i - 1].TransitTimePortofLoadingtoCFSDestination; //"Transit Time Port of Loading to CFS Destination";
                worksheet.Cells[8 + i, 9].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 9]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 9].EntireRow.Font.Bold = true;

                worksheet.Cells[8 + i, 10] = searchResultList[i - 1].TransitTimeCutOffOriginCFStoCFSDestination; //"Transit Time Cut-Off Origin CFS to CFS Destination";
                worksheet.Cells[8 + i, 10].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                ((Range)worksheet.Cells[8 + i, 10]).EntireColumn.ColumnWidth = 25;
                //worksheet.Cells[8 + i, 10].EntireRow.Font.Bold = true;
            }
        }

        private void AddData(Worksheet worksheet)
        {
            throw new NotImplementedException();
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception exception)
            {
                log.Debug(exception.Message);
                log.Debug(exception.StackTrace);
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}