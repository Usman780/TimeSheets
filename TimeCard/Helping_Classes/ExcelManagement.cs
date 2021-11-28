
using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using System.Web;
using TimeCard.BL;
using TimeCard.Models;

namespace TimeCard.Helping_Classes
{
    public class ExcelManagement
    {
        public static void generateGenericExcelFile(string saveFile, List<EntriesSearchDTO> entriesdtos = null)
        {
            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (entriesdtos != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getEntriesSheet(true, worksheetPart, entriesdtos);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }

        public static void generateProjectReportExcelFile(string saveFile, List<ProjectReportDTO> plist)
        {

            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (plist != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getProjectReportEntriesSheet(true, worksheetPart, plist);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }

        public static WorksheetPart getEntriesSheet(bool includeDeleteDate, WorksheetPart ws, List<EntriesSearchDTO> entriesdto = null)
        {
            List<EntriesSearchDTO> entriesdtos;
            if (entriesdto == null)
            {
                entriesdtos = null;
            }
            else
                entriesdtos = entriesdto;

            Worksheet workSheet2 = new Worksheet();
            SheetData sheetData2 = new SheetData();

            // the data for sheet 2

            Row row = new Row();


            row.Append(
                new Cell() { CellValue = new CellValue("Date"), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue("Name"), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue("LCAT"), DataType = new EnumValue<CellValues>(CellValues.String) },
                      new Cell() { CellValue = new CellValue("Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Project"), DataType = new EnumValue<CellValues>(CellValues.String) },
                                                new Cell() { CellValue = new CellValue("Status"), DataType = new EnumValue<CellValues>(CellValues.String) }


                      );


            // Insert the header row to the Sheet Data

            sheetData2.AppendChild(row);

            //  Set the cell value to be a numeric value of 100.
            foreach (var item in entriesdtos)
            {
                row = new Row();
                row.Append(
                    new Cell() { CellValue = new CellValue(item.Date), DataType = new EnumValue<CellValues>(CellValues.String) },
                     new Cell() { CellValue = new CellValue(item.Name), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(item.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
                      new Cell() { CellValue = new CellValue(item.Hours.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(item.Project), DataType = new EnumValue<CellValues>(CellValues.String) },
                                                new Cell() { CellValue = new CellValue(item.Status), DataType = new EnumValue<CellValues>(CellValues.String) }

                        );
                sheetData2.AppendChild(row);
            }

            workSheet2.AppendChild(sheetData2);
            ws.Worksheet = workSheet2;
            return ws;
        }

        public static void generateProjectExcelFile(string saveFile, List<EntriesSearchDTO> entriesdtos = null, List<EntriesSearchDTO> entriesdtos1 = null)
        {

            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (entriesdtos != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getProjectEntriesSheet(true, worksheetPart, entriesdtos, entriesdtos1);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }
        
        public static WorksheetPart getProjectEntriesSheet(bool includeDeleteDate, WorksheetPart ws, List<EntriesSearchDTO> entriesdto = null, List<EntriesSearchDTO> entriesdto1= null)
        {
            List<EntriesSearchDTO> entriesdtos;
            if (entriesdto == null)
            {
                entriesdtos = null;
            }
            else
            {
                entriesdtos = entriesdto.OrderBy(x => x.Project).ToList();
            }

            List<EntriesSearchDTO> entriesdtos1;
            if (entriesdto1 == null)
            {
                entriesdtos1 = null;
            }
            else
            {
                entriesdtos1 = entriesdto1.OrderBy(x => x.Project).ToList();
            }

            Worksheet workSheet3 = new Worksheet();
            SheetData sheetData3 = new SheetData();

            // the data for sheet 3

            Row row = new Row();


            row.Append(
                        new Cell() { CellValue = new CellValue("Project"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Employee"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("LCAT"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Total Hours"), DataType = new EnumValue<CellValues>(CellValues.String) }
                       
                      );

            
            // Insert the header row to the Sheet Data
            sheetData3.AppendChild(row);
            

            //int count = entriesdtos.Count();

            //for(int i = 0; i<= count; i++)
            //{
            //    row = new Row();

            //    row.Append(
            //                new Cell() { CellValue = new CellValue(entriesdtos.IndexOf(Name[i])), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                new Cell() { CellValue = new CellValue(item.Name), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                new Cell() { CellValue = new CellValue(item.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                new Cell() { CellValue = new CellValue(item.Hours.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }
            //            );

            //    sheetData3.AppendChild(row);
            //}




            //string a = "";
            //int ttl = 0;
            ////  Set the cell value to be a numeric value of 100.
            //foreach (var item in entriesdtos)
            //{
            //    row = new Row();
            //    row.Append(
            //                new Cell() { CellValue = new CellValue(item.Project), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                new Cell() { CellValue = new CellValue(item.Name), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                new Cell() { CellValue = new CellValue(item.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                new Cell() { CellValue = new CellValue(item.Hours.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }
            //            );

            //    sheetData3.AppendChild(row);
            //    a = item.Project;
            //    if(a != item.Project)
            //    foreach (var item2 in item.Project.Distinct())
            //    {
            //        ttl += Convert.ToInt32(item.Hours);

            //        row = new Row();
            //        row.Append(
            //                    new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                    new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                    new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
            //                    new Cell() { CellValue = new CellValue(ttl.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }

            //                );

            //        sheetData3.AppendChild(row);
            //    }

            //}




            double ttl = 0;
            //  Set the cell value to be a numeric value of 100.

            foreach (var item in entriesdtos1)
            {
                row = new Row();
                row.Append(
                            new Cell() { CellValue = new CellValue(item.Project), DataType = new EnumValue<CellValues>(CellValues.String) } 
                        );
                sheetData3.AppendChild(row);

                

                foreach (var item1 in entriesdtos)
                {
                    if (item.Project == item1.Project)
                    {
                        row = new Row();
                        row.Append(
                            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(item1.Name), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(item1.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(Convert.ToString(item1.Hours)), DataType = new EnumValue<CellValues>(CellValues.String) }
                            );

                        sheetData3.AppendChild(row);

                        ttl += Convert.ToDouble(item1.Hours);
                    }


                }

                //sheetData3.AppendChild(row);


                row = new Row();
                row.Append(
                            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(ttl.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }

                        );

                //sheetData3.AppendChild(row);

                //ttl += Convert.ToInt32(item.Hours);
                sheetData3.AppendChild(row);

                ttl = 0;
            }

            //row = new Row();
            //row.Append(
            //            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
            //            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
            //            new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
            //            new Cell() { CellValue = new CellValue(ttl.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }

            //        );

            //sheetData3.AppendChild(row);






            workSheet3.AppendChild(sheetData3);
            ws.Worksheet = workSheet3;
            return ws;
        }

        public static WorksheetPart getProjectReportEntriesSheet(bool includeDeleteDate, WorksheetPart ws, List<ProjectReportDTO> plist = null)
        {
            Worksheet workSheet3 = new Worksheet();
            SheetData sheetData3 = new SheetData();

            Row row = new Row();
            row.Append(
                        new Cell() { CellValue = new CellValue("Project"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Employee"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("LCAT"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Total Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Total Cost"), DataType = new EnumValue<CellValues>(CellValues.String) }
                      );

            sheetData3.AppendChild(row);


            double ttlhour = 0;
            double ttlcost = 0;
            var distinctList = plist.Select(x => x.Id).Distinct().ToList();

            foreach (var i in distinctList)
            {
                List<ProjectReportDTO> newlist = plist.Where(x => x.Id == i).ToList();

                Row row2 = new Row();
                row2.Append(
                        new Cell() { CellValue = new CellValue(newlist[0].ProjectName), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) }
                      );

                sheetData3.AppendChild(row2);


                foreach(ProjectReportDTO j in newlist)
                {
                    double cost = j.Hours * j.Cost;

                    Row row3 = new Row();
                    row3.Append(
                            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(j.EmployeeName), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(j.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(Convert.ToString(Math.Round(j.Hours, 2))), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(Convert.ToString(Math.Round(cost, 2))), DataType = new EnumValue<CellValues>(CellValues.String) }
                          );


                    sheetData3.AppendChild(row3);

                    ttlhour += j.Hours;
                    ttlcost += cost;
                }
            }

            Row row4 = new Row();
            row4.Append(
                    new Cell() { CellValue = new CellValue(), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(Convert.ToString(Math.Round(ttlhour, 2))), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(Convert.ToString(Math.Round((ttlcost), 2))), DataType = new EnumValue<CellValues>(CellValues.String) }
                  );

            sheetData3.AppendChild(row4);


            workSheet3.AppendChild(sheetData3);
            ws.Worksheet = workSheet3;
            return ws;
        }

        //Done by Haider bhai
    //    public static WorksheetPart getProjectReportEntriesSheet(bool includeDeleteDate, WorksheetPart ws, List<ProjectReportDTO> plist = null)
    //    {
           

    //        Worksheet workSheet3 = new Worksheet();
    //        SheetData sheetData3 = new SheetData();

    //        // the data for sheet 3

    //        Row row = new Row();


    //        row.Append(
    //                    new Cell() { CellValue = new CellValue("Project"), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                    new Cell() { CellValue = new CellValue("Employee"), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                    new Cell() { CellValue = new CellValue("LCAT"), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                    new Cell() { CellValue = new CellValue("Total Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                    new Cell() { CellValue = new CellValue("Total Cost"), DataType = new EnumValue<CellValues>(CellValues.String) }

    //                  );


    //        // Insert the header row to the Sheet Data
    //        sheetData3.AppendChild(row);




    //        double ttlhour = 0;
    //        double ttlcost = 0;
    //        List<int> count = new List<int>();
    //        int flag = 0;
    //        //  Set the cell value to be a numeric value of 100.
    //        int i = 0;
    //        int len = 0;
    //        foreach (var item in plist)
    //        {
    //            len = plist.Count();

    //            if (!count.Contains(item.Id))
    //            {
    //                if (flag != 0)
    //                {
    //                    row = new Row();
    //                    row.Append(
    //                                new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                                new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                                new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                                new Cell() { CellValue = new CellValue(ttlhour.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                                new Cell() { CellValue = new CellValue(Convert.ToString(Math.Round(ttlcost,2))), DataType = new EnumValue<CellValues>(CellValues.String) }

    //                            );
    //                    sheetData3.AppendChild(row);

    //                    ttlhour = 0;
    //                    ttlcost = 0;
    //                }
    //                row = new Row();
    //                row.Append(
    //                            new Cell() { CellValue = new CellValue(item.ProjectName), DataType = new EnumValue<CellValues>(CellValues.String) }
    //                        );
    //                sheetData3.AppendChild(row);
    //                count.Add(item.Id);
    //                flag++;
    //            }
              



               
    //                    row = new Row();
    //                    row.Append(
    //                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                        new Cell() { CellValue = new CellValue(item.EmployeeName), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                        new Cell() { CellValue = new CellValue(item.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                        new Cell() { CellValue = new CellValue(Convert.ToString(item.Hours)), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                        new Cell() { CellValue = new CellValue(Convert.ToString(Math.Round(item.Cost*item.Hours,2))), DataType = new EnumValue<CellValues>(CellValues.String) }
    //                        );

    //                    sheetData3.AppendChild(row);

    //                    ttlhour += Convert.ToDouble(item.Hours);
    //                    ttlcost += Convert.ToDouble(item.Cost*item.Hours);


    //            //sheetData3.AppendChild(row);



    //            if (i == len - 1) {
    //                row = new Row();
    //                row.Append(
    //                            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                            new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                            new Cell() { CellValue = new CellValue(ttlhour.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) },
    //                            new Cell() { CellValue = new CellValue(Math.Round(ttlcost,2).ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }

    //                        );
    //                sheetData3.AppendChild(row);

    //                ttlhour = 0;
    //                ttlcost = 0;
    //            }
   
    //i++;
    //        }

    //        //row = new Row();
    //        //row.Append(
    //        //            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
    //        //            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
    //        //            new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
    //        //            new Cell() { CellValue = new CellValue(ttl.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }

    //        //        );

    //        //sheetData3.AppendChild(row);






    //        workSheet3.AppendChild(sheetData3);
    //        ws.Worksheet = workSheet3;
    //        return ws;
    //    }

        public static void generateEmployeeHoursExcelFile(string saveFile, List<EntriesSearchDTO> entriesdtos = null, List<EntriesSearchDTO> entriesdtos1 = null)
        {

            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (entriesdtos != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getEmployeeHoursEntriesSheet(true, worksheetPart, entriesdtos, entriesdtos1);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }

        public static WorksheetPart getEmployeeHoursEntriesSheet(bool includeDeleteDate, WorksheetPart ws, List<EntriesSearchDTO> entriesdto = null, List<EntriesSearchDTO> entriesdto1 = null)
        {
            List<EntriesSearchDTO> entriesdtos;
            if (entriesdto == null)
            {
                entriesdtos = null;
            }
            else
            {
                entriesdtos = entriesdto.OrderBy(x => x.Name).ToList();
            }

            List<EntriesSearchDTO> entriesdtos1;
            if (entriesdto1 == null)
            {
                entriesdtos1 = null;
            }
            else
            {
                entriesdtos1 = entriesdto1.OrderBy(x => x.Name).ToList();
            }

            Worksheet workSheet3 = new Worksheet();
            SheetData sheetData3 = new SheetData();

            // the data for sheet 3

            Row row = new Row();


            row.Append(
                        new Cell() { CellValue = new CellValue("First Name"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Last Name"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Total Hours"), DataType = new EnumValue<CellValues>(CellValues.String) }

                      );

            // Insert the header row to the Sheet Data
            sheetData3.AppendChild(row);


           




            double ttl = 0;

            foreach (var item in entriesdtos1)
            {
                row = new Row();
                row.Append(
                            new Cell() { CellValue = new CellValue(item.FName), DataType = new EnumValue<CellValues>(CellValues.String) },
                            new Cell() { CellValue = new CellValue(item.LName), DataType = new EnumValue<CellValues>(CellValues.String) }
                        );
                

                foreach (var item1 in entriesdtos)
                {

                    if (item.Name == item1.Name)
                    {

                        ttl += Convert.ToDouble(item1.Hours);
                    }
                }

                row.Append(
                            new Cell() { CellValue = new CellValue(Convert.ToString(ttl + " Hours")), DataType = new EnumValue<CellValues>(CellValues.String) }
                            );

                sheetData3.AppendChild(row);
                ttl = 0;


                
            }

            //row = new Row();
            //row.Append(
            //            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
            //            new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
            //            new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
            //            new Cell() { CellValue = new CellValue(ttl.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }

            //        );

            //sheetData3.AppendChild(row);






            workSheet3.AppendChild(sheetData3);
            ws.Worksheet = workSheet3;
            return ws;
        }

        public static void generateApprovedExcelFile(string saveFile, List<EntriesSearchDTO> entriesdtos = null)
        {
            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (entriesdtos != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getApprovedEntriesSheet(true, worksheetPart, entriesdtos);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }

        public static WorksheetPart getApprovedEntriesSheet(bool includeDeleteDate, WorksheetPart ws, List<EntriesSearchDTO> entriesdto = null)
        {
            List<EntriesSearchDTO> entriesdtos;
            if (entriesdto == null)
            {
                entriesdtos = null;
            }
            else
                entriesdtos = entriesdto;

            Worksheet workSheet2 = new Worksheet();
            SheetData sheetData2 = new SheetData();

            // the data for sheet 2

            Row row = new Row();


            row.Append(
                new Cell() { CellValue = new CellValue("Date"), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue("Name"), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue("LCAT"), DataType = new EnumValue<CellValues>(CellValues.String) },
                      new Cell() { CellValue = new CellValue("Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Project"), DataType = new EnumValue<CellValues>(CellValues.String) },
                                                new Cell() { CellValue = new CellValue("Status"), DataType = new EnumValue<CellValues>(CellValues.String) }


                      );


            // Insert the header row to the Sheet Data

            sheetData2.AppendChild(row);

            //  Set the cell value to be a numeric value of 100.
            foreach (var item in entriesdtos)
            {
                row = new Row();
                row.Append(
                    new Cell() { CellValue = new CellValue(item.Date), DataType = new EnumValue<CellValues>(CellValues.String) },
                     new Cell() { CellValue = new CellValue(item.Name), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(item.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
                      new Cell() { CellValue = new CellValue(item.Hours.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(item.Project), DataType = new EnumValue<CellValues>(CellValues.String) },
                                                new Cell() { CellValue = new CellValue(item.Status), DataType = new EnumValue<CellValues>(CellValues.String) }

                        );
                sheetData2.AppendChild(row);
            }

            workSheet2.AppendChild(sheetData2);
            ws.Worksheet = workSheet2;
            return ws;
        }

        public static void generateMonthlyProjectHourExcelFile(string saveFile, List<ProjectReportDTO> plist, string PassDate)
        {

            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (plist != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getMonthlyProjectHoursSheet(true, worksheetPart, plist, PassDate);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }

        public static WorksheetPart getMonthlyProjectHoursSheet(bool includeDeleteDate, WorksheetPart ws, List<ProjectReportDTO> plist = null, string PassDate="")
        {


            Worksheet workSheet3 = new Worksheet();
            SheetData sheetData3 = new SheetData();

            // the data for sheet 3

            Row row = new Row();
            row.Append(
                        new Cell() { CellValue = new CellValue(PassDate), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) }

                      );

            sheetData3.AppendChild(row);


            row = new Row();
            row.Append(
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) }

                      );

            sheetData3.AppendChild(row);



            row = new Row();


            row.Append(
                        new Cell() { CellValue = new CellValue("Project"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Employee"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("LCAT"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Total Hours"), DataType = new EnumValue<CellValues>(CellValues.String) }

                      );


            // Insert the header row to the Sheet Data
            sheetData3.AppendChild(row);




            double ttl = 0;
            List<int> count = new List<int>();
            int flag = 0;
            //  Set the cell value to be a numeric value of 100.
            int i = 0;
            int len = 0;
            foreach (var item in plist)
            {
                len = plist.Count();

                if (!count.Contains(item.Id))
                {
                    if (flag != 0)
                    {
                        row = new Row();
                        row.Append(
                                    new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                                    new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                                    new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
                                    new Cell() { CellValue = new CellValue(ttl.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }

                                );
                        sheetData3.AppendChild(row);

                        ttl = 0;
                    }
                    row = new Row();
                    row.Append(
                                new Cell() { CellValue = new CellValue(item.ProjectName), DataType = new EnumValue<CellValues>(CellValues.String) }
                            );
                    sheetData3.AppendChild(row);
                    count.Add(item.Id);
                    flag++;
                }





                row = new Row();
                row.Append(
                    new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(item.EmployeeName), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(item.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(Convert.ToString(item.Hours)), DataType = new EnumValue<CellValues>(CellValues.String) }
                    );

                sheetData3.AppendChild(row);

                ttl += Convert.ToDouble(item.Hours);


                //sheetData3.AppendChild(row);



                if (i == len - 1)
                {
                    row = new Row();
                    row.Append(
                                new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                                new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                                new Cell() { CellValue = new CellValue("Total Hours :"), DataType = new EnumValue<CellValues>(CellValues.String) },
                                new Cell() { CellValue = new CellValue(ttl.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }

                            );
                    sheetData3.AppendChild(row);

                    ttl = 0;
                }

                i++;
            }

           
            workSheet3.AppendChild(sheetData3);
            ws.Worksheet = workSheet3;
            return ws;
        }

        public static void generatePendingTimeCardExcelFile(string saveFile, List<EntriesSearchDTO> entriesdtos = null)
        {
            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (entriesdtos != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getPendingTimeCardSheet(true, worksheetPart, entriesdtos);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }

        public static WorksheetPart getPendingTimeCardSheet(bool includeDeleteDate, WorksheetPart ws, List<EntriesSearchDTO> entriesdto = null)
        {
            List<EntriesSearchDTO> entriesdtos;
            if (entriesdto == null)
            {
                entriesdtos = null;
            }
            else
                entriesdtos = entriesdto;

            Worksheet workSheet2 = new Worksheet();
            SheetData sheetData2 = new SheetData();

            // the data for sheet 2

            Row row = new Row();


            row.Append(
                new Cell() { CellValue = new CellValue("Date"), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue("Name"), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue("LCAT"), DataType = new EnumValue<CellValues>(CellValues.String) },
                      new Cell() { CellValue = new CellValue("Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Project"), DataType = new EnumValue<CellValues>(CellValues.String) },
                                                new Cell() { CellValue = new CellValue("Status"), DataType = new EnumValue<CellValues>(CellValues.String) }


                      );


            // Insert the header row to the Sheet Data

            sheetData2.AppendChild(row);

            //  Set the cell value to be a numeric value of 100.
            foreach (var item in entriesdtos)
            {
                row = new Row();
                row.Append(
                    new Cell() { CellValue = new CellValue(item.Date), DataType = new EnumValue<CellValues>(CellValues.String) },
                     new Cell() { CellValue = new CellValue(item.Name), DataType = new EnumValue<CellValues>(CellValues.String) },
                    new Cell() { CellValue = new CellValue(item.LCAT), DataType = new EnumValue<CellValues>(CellValues.String) },
                      new Cell() { CellValue = new CellValue(item.Hours.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(item.Project), DataType = new EnumValue<CellValues>(CellValues.String) },
                                                new Cell() { CellValue = new CellValue(item.Status), DataType = new EnumValue<CellValues>(CellValues.String) }

                        );
                sheetData2.AppendChild(row);
            }

            workSheet2.AppendChild(sheetData2);
            ws.Worksheet = workSheet2;
            return ws;
        }

        public static void generateProjectBudgetStatusExcelFile(string saveFile, List<ProjectBudgetDTO> pbudgetdtos = null)
        {
            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (pbudgetdtos != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getProjectBudgetStatusSheet(true, worksheetPart, pbudgetdtos);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }

        public static WorksheetPart getProjectBudgetStatusSheet(bool includeDeleteDate, WorksheetPart ws, List<ProjectBudgetDTO> pbudgetdto = null)
        {
            List<ProjectBudgetDTO> pbudgetdtos;
            if (pbudgetdto == null)
            {
                pbudgetdtos = null;
            }
            else
            {
                pbudgetdtos = pbudgetdto;
            }

            

            Worksheet workSheet2 = new Worksheet();
            SheetData sheetData2 = new SheetData();

            // the data for sheet 2

            Row row = new Row();


            row.Append(
                        new Cell() { CellValue = new CellValue("Project Code"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Budget"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Total Cost"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Remaining"), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue("Remaining Budget"), DataType = new EnumValue<CellValues>(CellValues.String) }
                      );


            // Insert the header row to the Sheet Data

            sheetData2.AppendChild(row);

            
            //  Set the cell value to be a numeric value of 100.
            foreach (var item in pbudgetdtos)
            {
                
                row = new Row();
                row.Append(
                        new Cell() { CellValue = new CellValue(item.Code), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(item.Budget), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(item.TotalCost), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(item.Remaining), DataType = new EnumValue<CellValues>(CellValues.String) },
                        new Cell() { CellValue = new CellValue(item.RemainingBudget), DataType = new EnumValue<CellValues>(CellValues.String) }
                        );
                sheetData2.AppendChild(row);
            }

            workSheet2.AppendChild(sheetData2);
            ws.Worksheet = workSheet2;
            return ws;
        }

        public static void generateDistributionReportExcelFile(string saveFile, List<DistributionReportDTO> distridtos = null)
        {
            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(saveFile,
            SpreadsheetDocumentType.Workbook);

            int sheetId = 1;
            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            if (distridtos != null)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart = getDistributionReportSheet(true, worksheetPart, distridtos);
                Sheet sheet = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (uint)sheetId,
                    Name = "Entries"
                };
                sheets.Append(sheet);
                sheetId++;
            }

            ssDoc.Close();
        }

        public static WorksheetPart getDistributionReportSheet(bool includeDeleteDate, WorksheetPart ws, List<DistributionReportDTO> distridto = null)
        {
            List<DistributionReportDTO> entriesdtos;
            if (distridto == null)
            {
                entriesdtos = null;
            }
            else
                entriesdtos = distridto;

            Worksheet workSheet2 = new Worksheet();
            SheetData sheetData2 = new SheetData();

            // the data for sheet 2

            Row row = new Row();


            row.Append(
                  new Cell() { CellValue = new CellValue("Employee Name"), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue("Vacation Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue("Operations-Overhead Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue("Holiday Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue("Other Project Hours"), DataType = new EnumValue<CellValues>(CellValues.String) }
                      );


            // Insert the header row to the Sheet Data

            sheetData2.AppendChild(row);
            double ttlhr = 0;
            //  Set the cell value to be a numeric value of 100.
            foreach (DistributionReportDTO item in entriesdtos)
            {
                row = new Row();
                row.Append(
                     new Cell() { CellValue = new CellValue(item.Name), DataType = new EnumValue<CellValues>(CellValues.String) },
                     new Cell() { CellValue = new CellValue(item.VacationHour.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) },
                     new Cell() { CellValue = new CellValue(item.OverheadHour.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) },
                     new Cell() { CellValue = new CellValue(item.HolidayHour.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) },
                     new Cell() { CellValue = new CellValue(item.OtherHour.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }
                        );
                sheetData2.AppendChild(row);

                ttlhr += item.VacationHour + item.OverheadHour + item.OtherHour + item.HolidayHour;
            }

            Row row2 = new Row();


            row2.Append(
                  new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue("Total Hours"), DataType = new EnumValue<CellValues>(CellValues.String) },
                  new Cell() { CellValue = new CellValue(ttlhr.ToString()), DataType = new EnumValue<CellValues>(CellValues.String) }
                      );


            // Insert the header row to the Sheet Data

            sheetData2.AppendChild(row2);


            workSheet2.AppendChild(sheetData2);
            ws.Worksheet = workSheet2;
            return ws;
        }
    }
}