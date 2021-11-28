using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TimeCard.BL;
using TimeCard.Helping_Classes;
using TimeCard.Models;

namespace TimeCard.Controllers
{
    public class ExcelReportController : Controller
    {
        SessionDTO sessiondto = new SessionDTO();

        public string GetBaseUrl()
        {
            var request = HttpContext.ApplicationInstance.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}",
                request.Url.Scheme, //request.Url.Scheme gives https or http
                request.Url.Authority, //request.Url.Authority gives qawithexperts/com
                appUrl); //appUrl = /questions/111/ok-this-is-url

            return baseUrl; //this will return complete url
        }

        public FileResult DownloadReport(string FileAddress = "")
        {
            //var FileVirtualPath = "~/Content/Medical_Record/" + FileAddress;
            var FileVirtualPath = FileAddress;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        public ActionResult EmployeeEntriesExcelSheet(string ProjectId, string StartDate, string EndDate, string EStatus)
        {
            User user = new UserBL().getUserList().Where(x => x.Id == sessiondto.getId() && x.Is_Authorize == 1).FirstOrDefault();
            List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
            List<EntryTime> allentries = new List<EntryTime>();

            List<ProjectUserCategory> userentries = user.ProjectUserCategories.Where(x => x.User_Id == user.Id).ToList();

            foreach (ProjectUserCategory p in userentries)
            {
                List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == p.Id).ToList();
                foreach (EntryTime e in enteries)
                {
                    allentries.Add(e);
                }

            }
            if (StartDate != "")
            {
                DateTime d1 = DateTime.Parse(StartDate);
                allentries = allentries.Where(x => x.Date >= d1.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

            }
            if (EndDate != "")
            {
                DateTime d2 = DateTime.Parse(EndDate);
                allentries = allentries.Where(x => x.Date <= d2.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (ProjectId != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(ProjectId) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

            }
            if (EStatus == "Pending")
            {
                allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (EStatus == "Accepted")
            {
                allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (EStatus == "Rejected")
            {
                allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }

            List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();

            foreach (EntryTime e in allentries)
            {
                EntriesSearchDTO edto = new EntriesSearchDTO()
                {
                    Id = e.Id,
                    Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName,
                    Hours = e.Hour,
                    Date = e.Date.ToString("MM/dd/yyyy"),
                    LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
                    Project = e.ProjectUserCategory.ProjectCategory.Project.Code
                };
                if (e.Is_Approved == 1)
                {
                    edto.Status = "Accepted";

                }
                if (e.Is_Approved == 0)
                {
                    edto.Status = "Pending";
                }
                if (e.Is_Approved == 2)
                {
                    edto.Status = "Rejected";
                    edto.RejectReason = e.RejectReason;
                }

                entriesdtos.Add(edto);
            }
            entriesdtos = entriesdtos.OrderByDescending(x => x.Date).ToList();

            string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Employee Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
            ExcelManagement.generateGenericExcelFile(path, entriesdtos);
            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Entries Employee Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        }

        #region Excel
        /// <summary>
        /// This method is to download the excel sheet with all the employees of the given query.
        /// The reason why these queries are implemented here and not in the separate class is because
        /// they have specific 
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <param name="departname"></param>
        /// <param name="divisionName"></param>
        //public ActionResult EntriesExcelSheet(string ProjectId, string StartDate, string EndDate, string EmployeeId, string LaborCategories, string EStatus)
        //{
        //    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
        //    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

        //    if (StartDate != "")
        //    {
        //        DateTime d1 = DateTime.Parse(StartDate);
        //        allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

        //    }
        //    if (EndDate != "")
        //    {
        //        DateTime d2 = DateTime.Parse(EndDate);
        //        allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (ProjectId != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(ProjectId)).OrderByDescending(x => x.Date).ToList();

        //    }
        //    if (EmployeeId != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(EmployeeId)).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (LaborCategories != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategories)).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Pending")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Accepted")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Rejected")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }

        //    allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();

        //    List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();

        //    foreach (EntryTime e in allentries)
        //    {
        //        EntriesSearchDTO edto = new EntriesSearchDTO()
        //        {
        //            Id = e.Id,
        //            Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName,
        //            Hours = e.Hour,
        //            Project = e.ProjectUserCategory.ProjectCategory.Project.Code,
        //            LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
        //            Date = e.Date.ToString("MM/dd/yyyy")
        //        };
        //        if (e.Is_Approved == 1)
        //        {
        //            edto.Status = "Accepted";

        //        }
        //        if (e.Is_Approved == 0)
        //        {
        //            edto.Status = "Pending";
        //        }
        //        if (e.Is_Approved == 2)
        //        {
        //            edto.Status = "Rejected";
        //            edto.RejectReason = e.RejectReason;
        //        }

        //        entriesdtos.Add(edto);
        //    }
        //    entriesdtos = entriesdtos.OrderByDescending(x => x.Date).ToList();

        //    if (sessiondto.getRole() == 2)
        //    {
        //        string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
        //        ExcelManagement.generateGenericExcelFile(path, entriesdtos);
        //        return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        //    }
        //    else
        //    {
        //        string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
        //        ExcelManagement.generateGenericExcelFile(path, entriesdtos);
        //        return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        //    }

        //} 









        //public ActionResult ProjectEntriesExcelSheet(string ProjectId="", string StartDate= "", string EndDate = "")
        //{
        //    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
        //    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

        //    if (StartDate != "")
        //    {
        //        DateTime d1 = DateTime.Parse(StartDate);
        //        allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

        //    }
        //    if (EndDate != "")
        //    {
        //        DateTime d2 = DateTime.Parse(EndDate);
        //        allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (ProjectId != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(ProjectId)).OrderByDescending(x => x.Date).ToList();
        //    }
        //    /*if (EmployeeId != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(EmployeeId)).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (LaborCategories != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategories)).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Pending")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Accepted")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Rejected")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }*/

        //    List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();

        //    foreach (EntryTime e in allentries)
        //    {
        //        EntriesSearchDTO edto = new EntriesSearchDTO()
        //        {
        //            Id = e.Id,
        //            Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName,
        //            Hours = e.Hour,
        //            Project = e.ProjectUserCategory.ProjectCategory.Project.Code,
        //            LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
        //            Date = e.Date.ToString("MM/dd/yyyy")
        //        };
        //        //if (e.Is_Approved == 1)
        //        //{
        //        //    edto.Status = "Accepted";

        //        //}
        //        //if (e.Is_Approved == 0)
        //        //{
        //        //    edto.Status = "Pending";
        //        //}
        //        //if (e.Is_Approved == 2)
        //        //{
        //        //    edto.Status = "Rejected";
        //        //    edto.RejectReason = e.RejectReason;
        //        //}

        //        entriesdtos.Add(edto);
        //    }
        //    entriesdtos = entriesdtos.Distinct().OrderByDescending(x => x.Project).ToList();

        //    if (sessiondto.getRole() == 2)
        //    {
        //        string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
        //        ExcelManagement.generateProjectExcelFile(path, entriesdtos);
        //        return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        //    }
        //    else
        //    {
        //        string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
        //        ExcelManagement.generateProjectExcelFile(path, entriesdtos);
        //        return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        //    }

        //}
        #endregion

        #region Async ExcelReport Employees TimeSheet
        [HttpPost]
        public FileResult AsyncEntriesExcelSheet(string ProjectId, string StartDate, string EndDate, string EmployeeId, string LaborCategories, string EStatus)
        {
            // var local = new Byte[1000000];
            string reportName = "Employee Assignments";
            //if (isCompleted == 1)
            //{
            //    reportName = "Completed Employee Assignments";
            //}
            //MainMailClass mail = new MainMailClass();
            int role = SessionDTO.getrole();
            int id = SessionDTO.getid();
            User user = new UserBL().getUserById(id);
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            string local;
            new System.Threading.Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                local = EntriesExcelSheet(ProjectId, StartDate, EndDate, EmployeeId, LaborCategories, EStatus, user.Role);
                //mail.DownloadReport(local, MediaTypeNames.Text.Plain, reportName);
                DownloadEmployeesTimeSheetFile(local, role);
                MailSender.SendDownloadFileLinkEmail(user.Email, local, BaseUrl);
            }).Start();
            return null;
        }

        public string EntriesExcelSheet(string ProjectId, string StartDate, string EndDate, string EmployeeId, string LaborCategories, string EStatus, int role)
        {
            List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
            List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

            if (StartDate != "")
            {
                DateTime d1 = DateTime.Parse(StartDate);
                allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();
            }
            if (EndDate != "")
            {
                DateTime d2 = DateTime.Parse(EndDate);
                allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
            }
            if (ProjectId != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(ProjectId)).OrderByDescending(x => x.Date).ToList();

            }
            if (EmployeeId != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(EmployeeId)).OrderByDescending(x => x.Date).ToList();
            }
            if (LaborCategories != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategories)).OrderByDescending(x => x.Date).ToList();
            }
            if (EStatus == "Pending")
            {
                allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (EStatus == "Accepted")
            {
                allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (EStatus == "Rejected")
            {
                allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }

            allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
            List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();
            foreach (EntryTime e in allentries)
            {
                EntriesSearchDTO edto = new EntriesSearchDTO()
                {
                    Id = e.Id,
                    Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName,
                    Hours = e.Hour,
                    Project = e.ProjectUserCategory.ProjectCategory.Project.Code,
                    LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
                    Date = e.Date.ToString("MM/dd/yyyy")
                };
                if (e.Is_Approved == 1)
                {
                    edto.Status = "Accepted";
                }
                if (e.Is_Approved == 0)
                {
                    edto.Status = "Pending";
                }
                if (e.Is_Approved == 2)
                {
                    edto.Status = "Rejected";
                    edto.RejectReason = e.RejectReason;
                }

                entriesdtos.Add(edto);
            }
            entriesdtos = entriesdtos.OrderByDescending(x => x.Date).ToList();
            //string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Employees TimeSheet Report ("+ DateTime.UtcNow + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
            //ExcelManagement.generateGenericExcelFile(path, entriesdtos);
            //return path;
            //if (sessiondto.getRole() == 2)
            if (role == 2)
            {
                //string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Employees TimeSheet Report by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateGenericExcelFile(path, entriesdtos);
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
                return path;
            }
            else
            {
                //string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Employees TimeSheet Report by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateGenericExcelFile(path, entriesdtos);
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
                return path;
            }

        }

        public ActionResult DownloadEmployeesTimeSheetFile(string path, int role = -1)
        {
            if (role == 2)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Employees TimeSheet Report by Admin (" + DateTime.UtcNow + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else if (role == 1)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Employees TimeSheet Report by Manager (" + DateTime.UtcNow + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
                return null;
        }

        #endregion

        #region Async ExcelReport Project Report
        [HttpPost]
        public FileResult AsyncProjectEntriesExcelSheet(int ProjectId = -1, string StartDate = "", string EndDate = "")
        {
            string reportName = "Employee Assignments";
            int role = SessionDTO.getrole();
            int id = SessionDTO.getid();
            User user = new UserBL().getUserById(id);
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            string local;
            new System.Threading.Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                local = ProjectEntriesExcelSheet(ProjectId, StartDate, EndDate, user.Role);
                DownloadProjectReportFile(local, role);
                MailSender.SendDownloadFileLinkEmail(user.Email, local, BaseUrl);
            }).Start();
            return null;
        }

        public string ProjectEntriesExcelSheet(int ProjectId = -1, string StartDate = "", string EndDate = "", int role =-1)
        {
            var pucList = new UserBL().getProjectUserCategoryList().OrderBy(x => x.User.FirstName).Select(x => new { x.User_Id, x.ProjectCategory_Id }).Distinct().ToList();
            List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
            ViewBag.projects = allprojects.OrderBy(x => x.Code);
            List<ProjectReportDTO> prdto = new List<ProjectReportDTO>();

            foreach (var puc in pucList)
            {
                List<ProjectUserCategory> puclist2 = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == puc.User_Id && x.ProjectCategory_Id == puc.ProjectCategory_Id).ToList();
                foreach (ProjectUserCategory puc2 in puclist2)
                {
                    List<EntryTime> e = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == puc2.Id && x.Is_Authorize == 1 && x.Is_Approved != 2 && x.ProjectUserCategory.User.Is_Authorize != 0 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
                    if (StartDate != "")
                    {
                        DateTime d1 = DateTime.Parse(StartDate);
                        e = e.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();
                    }
                    if (EndDate != "")
                    {
                        DateTime d2 = DateTime.Parse(EndDate);
                        e = e.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
                    }
                    if (ProjectId != -1)
                    {
                        e = e.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == ProjectId).OrderByDescending(x => x.Date).ToList();
                    }

                    ProjectUserCategory pcost = puclist2.Where(x => x.User_Id == puc2.User_Id).FirstOrDefault();
                    if (e.Count != 0)
                    {
                        double hrs = e.Sum(x => x.Hour);
                        //double cst = Convert.ToDouble(e[0].ProjectUserCategory.ProjectCategory.Cost);
                        double cst = 0.0;
                        if (pcost.ProjectCategory.Cost != null || pcost.ProjectCategory.Cost != "")
                        {
                            cst = Convert.ToDouble(pcost.ProjectCategory.Cost);
                        }
                        User usr = new UserBL().getUserById(e[0].ProjectUserCategory.User.Id);
                        if (usr.Is_Authorize == 2)
                        {
                            usr.FirstName = "Inactive";
                        }
                        else
                        {
                            usr.FirstName = "";
                        }
                        ProjectReportDTO obj = new ProjectReportDTO()
                        {
                            EmpId = e[0].ProjectUserCategory.User.Id,
                            EmployeeName = e[0].ProjectUserCategory.User.FirstName + " " + e[0].ProjectUserCategory.User.LastName,
                            Status = usr.FirstName,
                            Id = e[0].ProjectUserCategory.ProjectCategory.Project.Id,
                            ProjectName = e[0].ProjectUserCategory.ProjectCategory.Project.Code,
                            Hours = hrs,
                            LcatId = pcost.ProjectCategory.LaborCategory.Id,
                            LCAT = pcost.ProjectCategory.LaborCategory.Name,
                            Cost = cst
                        };
                        prdto.Add(obj);
                    }
                }
            }
            //string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
            //ExcelManagement.generateProjectReportExcelFile(path, prdto);
            //return path;

            if (role == 2)
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Project Report by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                //string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                //// ExcelManagement.generateProjectExcelFile(path, entriesdtos, entriesdtos1);
                ExcelManagement.generateProjectReportExcelFile(path, prdto);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Project Report by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                //string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateProjectReportExcelFile(path, prdto);
                return path;
                ////return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
        }

        public ActionResult DownloadProjectReportFile(string path, int role = -1)
        {
            if (role == 2)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Employees TimeSheet Report by Admin (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else if (role == 1)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Employees TimeSheet Report by Manager (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
                return null;
        }
        #endregion

        //public ActionResult ProjectEntriesExcelSheet(int ProjectId = -1, string StartDate = "", string EndDate = "")
        //{

        //    var pucList = new UserBL().getProjectUserCategoryList().OrderBy(x => x.User.FirstName).Select(x => new { x.User_Id, x.ProjectCategory_Id }).Distinct().ToList();

        //    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

        //    ViewBag.projects = allprojects.OrderBy(x => x.Code);


        //    List<ProjectReportDTO> prdto = new List<ProjectReportDTO>();

        //    foreach (var puc in pucList)
        //    {
        //        List<ProjectUserCategory> puclist2 = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == puc.User_Id && x.ProjectCategory_Id == puc.ProjectCategory_Id).ToList();


        //        foreach (ProjectUserCategory puc2 in puclist2)
        //        {
        //            List<EntryTime> e = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == puc2.Id && x.Is_Authorize == 1 && x.Is_Approved != 2 && x.ProjectUserCategory.User.Is_Authorize != 0 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();


        //            if (StartDate != "")
        //            {
        //                DateTime d1 = DateTime.Parse(StartDate);
        //                e = e.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

        //            }
        //            if (EndDate != "")
        //            {
        //                DateTime d2 = DateTime.Parse(EndDate);
        //                e = e.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
        //            }
        //            if (ProjectId != -1)
        //            {
        //                e = e.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == ProjectId ).OrderByDescending(x => x.Date).ToList();

        //            }

        //            ProjectUserCategory pcost = puclist2.Where(x => x.User_Id == puc2.User_Id).FirstOrDefault();

        //            if (e.Count != 0)
        //            {
        //                double hrs = e.Sum(x => x.Hour);
        //                //double cst = Convert.ToDouble(e[0].ProjectUserCategory.ProjectCategory.Cost);
        //                double cst = 0.0;
        //                if (pcost.ProjectCategory.Cost != null || pcost.ProjectCategory.Cost != "")
        //                {
        //                    cst = Convert.ToDouble(pcost.ProjectCategory.Cost);
        //                }

        //                User usr = new UserBL().getUserById(e[0].ProjectUserCategory.User.Id);
        //                if (usr.Is_Authorize == 2)
        //                {
        //                    usr.FirstName = "Inactive";
        //                }
        //                else
        //                {
        //                    usr.FirstName = "";
        //                }

        //                ProjectReportDTO obj = new ProjectReportDTO()
        //                {
        //                    EmpId = e[0].ProjectUserCategory.User.Id,
        //                    EmployeeName = e[0].ProjectUserCategory.User.FirstName + " " + e[0].ProjectUserCategory.User.LastName,
        //                    Status = usr.FirstName,
        //                    Id = e[0].ProjectUserCategory.ProjectCategory.Project.Id,
        //                    ProjectName = e[0].ProjectUserCategory.ProjectCategory.Project.Code,
        //                    Hours = hrs,
        //                    LcatId = pcost.ProjectCategory.LaborCategory.Id,
        //                    LCAT = pcost.ProjectCategory.LaborCategory.Name,
        //                    Cost = cst
        //                };

        //                prdto.Add(obj);
        //            }
        //        }
        //    }

        //    if (sessiondto.getRole() == 2)
        //    {
        //        string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
        //        // ExcelManagement.generateProjectExcelFile(path, entriesdtos, entriesdtos1);
        //        ExcelManagement.generateProjectReportExcelFile(path, prdto);
        //        return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        //    }
        //    else
        //    {
        //        string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
        //        ExcelManagement.generateProjectReportExcelFile(path, prdto);
        //        return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        //    }

        //}

        //Old Code Done By Hiader Bhai
        //public ActionResult ProjectEntriesExcelSheet(string ProjectId = "", string StartDate = "", string EndDate = "")
        //{
        //    //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize != 0).OrderByDescending(x => x.Date).ToList();
        //    //List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
        //    //List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
        //    if (ProjectId == "-1")
        //        ProjectId = "";

        //    if (StartDate != "")
        //    {
        //        DateTime d1 = DateTime.Parse(StartDate);
        //        allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();ProjectEntriesExcelSheet

        //    }
        //    if (EndDate != "")
        //    {
        //        DateTime d2 = DateTime.Parse(EndDate);
        //        allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (ProjectId != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(ProjectId)).Distinct().OrderByDescending(x => x.Date).ToList();
        //    }
        //    /*if (EmployeeId != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(EmployeeId)).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (LaborCategories != "")
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategories)).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Pending")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Accepted")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (EStatus == "Rejected")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }*/

        //    //List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();

        //    //foreach (EntryTime e in allentries)
        //    //{
        //    //    EntriesSearchDTO edto = new EntriesSearchDTO()
        //    //    {
        //    //        Id = e.Id,
        //    //        Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName,
        //    //        Hours = e.Hour,
        //    //        Project = e.ProjectUserCategory.ProjectCategory.Project.Code,
        //    //        LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
        //    //        Date = e.Date.ToString("MM/dd/yyyy")
        //    //    };


        //    //    entriesdtos.Add(edto);
        //    //}
        //    ////entriesdtos = entriesdtos.OrderByDescending(x => x.Project).ToList();


        //    //var a = entriesdtos.GroupBy(x => new { x.Project }).Select(x => x.FirstOrDefault());



        //    //ViewBag.test = a;
        //    //List<EntriesSearchDTO> entriesdtos1 = new List<EntriesSearchDTO>();

        //    //foreach(var e in ViewBag.test)
        //    //{
        //    //    EntriesSearchDTO edto1 = new EntriesSearchDTO()
        //    //    {

        //    //        Project = e.Project,

        //    //    };


        //    //    entriesdtos1.Add(edto1);
        //    //}





        //    var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
        //    List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
        //    foreach (EntryTime item in ab)
        //    {
        //        List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id & x.ProjectUserCategory.User.Is_Authorize != 0 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
        //        var ll = allentries2.GroupBy(c => c.ProjectUserCategory.User_Id).Select(p => new { EmployeeId = p.Key, EmployeeName = new UserBL().getUserById(p.Key).FirstName + " " + new UserBL().getUserById(p.Key).LastName, Hours = p.Sum(q => q.Hour) });
        //        foreach (var it in ll)
        //        {
        //            Project p = new UserBL().getProjectById(item.ProjectUserCategory.ProjectCategory.Project_Id);
        //            List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.Is_Authorize == 1).ToList();
        //            List<ProjectUserCategory> pucs = new List<ProjectUserCategory>();

        //            foreach (ProjectCategory pc in pcs)
        //            {
        //                //List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.Is_Authorize == 1 && x.ProjectCategory_Id == pc.Id).ToList();
        //                List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.ProjectCategory_Id == pc.Id).ToList();

        //                foreach (var item2 in mypuc)
        //                {
        //                    pucs.Add(item2);
        //                }

        //            }

        //            //ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).FirstOrDefault();
        //            ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).LastOrDefault();
        //            string pucc = "";
        //            double pcost = 0.0;

        //            if (puc == null)
        //            {
        //                pucc = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name;
        //            }
        //            else
        //            {
        //                pucc = puc.ProjectCategory.LaborCategory.Name;
        //                if (puc.ProjectCategory.Cost == null || puc.ProjectCategory.Cost == "")
        //                {
        //                    pcost = 0.0;
        //                }
        //                else
        //                {
        //                    pcost = Convert.ToDouble(puc.ProjectCategory.Cost);
        //                }
        //            }

        //            User usr = new UserBL().getUserById(it.EmployeeId);
        //            if (usr.Is_Authorize == 2)
        //            {
        //                usr.FirstName = " (Inactive)";
        //            }
        //            else
        //            {
        //                usr.FirstName = "";
        //            }

        //            ProjectReportDTO obj = new ProjectReportDTO()
        //            {
        //                EmployeeName = it.EmployeeName.ToString() + usr.FirstName,
        //                ProjectName = p.Code,
        //                Hours = it.Hours,
        //                LCAT = pucc,
        //                Cost = Convert.ToDouble(pcost),
        //                Id = p.Id,


        //            };
        //            plist.Add(obj);
        //        }

        //    }



        //    //ViewBag.test1 = entriesdtos;

        //    if (sessiondto.getRole() == 2)
        //    {
        //        string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
        //       // ExcelManagement.generateProjectExcelFile(path, entriesdtos, entriesdtos1);
        //        ExcelManagement.generateProjectReportExcelFile(path,plist);
        //        return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        //    }
        //    else
        //    {
        //        string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
        //        ExcelManagement.generateProjectReportExcelFile(path, plist);
        //        return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

        //    }

        //}

        #region Async ExcelReport Project Report
        [HttpPost]
        public FileResult AsyncEmployeeHoursEntriesExcelSheet( string StartDate = "", string EndDate = "")
        {
            string reportName = "Employee Assignments";
            int role = SessionDTO.getrole();
            int id = SessionDTO.getid();
            User user = new UserBL().getUserById(id);
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            string local;
            new System.Threading.Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                local = EmployeeHoursEntriesExcelSheet( StartDate, EndDate, user.Role);
                DownloadEmployeeHoursEntriesExcelSheetFile(local, role);
                MailSender.SendDownloadFileLinkEmail(user.Email, local, BaseUrl);
            }).Start();
            return null;
        }
        
        public string EmployeeHoursEntriesExcelSheet( string StartDate = "", string EndDate = "", int role=-1)
        {
            List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize ==1).OrderByDescending(x => x.Date).ToList();
            //List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
            //List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

            if (StartDate != "")
            {
                DateTime d1 = DateTime.Parse(StartDate);
                allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

            }
            if (EndDate != "")
            {
                DateTime d2 = DateTime.Parse(EndDate);
                allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
            }
            /*if (ProjectId != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(ProjectId)).Distinct().OrderByDescending(x => x.Date).ToList();
            }
            if (EmployeeId != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(EmployeeId)).OrderByDescending(x => x.Date).ToList();
            }
            if (LaborCategories != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategories)).OrderByDescending(x => x.Date).ToList();
            }
            if (EStatus == "Pending")
            {
                allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (EStatus == "Accepted")
            {
                allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (EStatus == "Rejected")
            {
                allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }*/

            List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();

            foreach (EntryTime e in allentries)
            {
                if (e.ProjectUserCategory.User.IsChecked != "1099")
                {
                    EntriesSearchDTO edto = new EntriesSearchDTO()
                    {
                        Id = e.Id,
                        Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName,
                        FName = e.ProjectUserCategory.User.FirstName,
                        LName = e.ProjectUserCategory.User.LastName,
                        Hours = e.Hour,
                        Project = e.ProjectUserCategory.ProjectCategory.Project.Code,
                        LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
                        Date = e.Date.ToString("MM/dd/yyyy")
                    };
                    entriesdtos.Add(edto);
                }
            }
            //entriesdtos = entriesdtos.OrderByDescending(x => x.Project).ToList();
            var a = entriesdtos.GroupBy(x => new { x.Name }).Select(x => x.FirstOrDefault());
            ViewBag.test = a;
            List<EntriesSearchDTO> entriesdtos1 = new List<EntriesSearchDTO>();

            foreach (var e in ViewBag.test)
            {
                EntriesSearchDTO edto1 = new EntriesSearchDTO()
                {
                    Name = e.Name,
                    FName = e.FName,
                    LName = e.LName
                };
                entriesdtos1.Add(edto1);
            }
            //ViewBag.test1 = entriesdtos;
            if (role == 2)
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Employee Hours Entries ExcelSheet by Admin (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateEmployeeHoursExcelFile(path, entriesdtos, entriesdtos1);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Admin Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Employee Hours Entries ExcelSheet by Manager (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateProjectExcelFile(path, entriesdtos, entriesdtos1);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Entries Manager Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
        }

        public ActionResult DownloadEmployeeHoursEntriesExcelSheetFile(string path, int role = -1)
        {
            if (role == 2)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Employee Hours Entries ExcelSheet by Admin (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else if (role == 1)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Employee Hours Entries ExcelSheet by Manager (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
                return null;
        }
        #endregion

        #region Async ExcelReport Approved Time SheetExcel
        [HttpPost]
        public FileResult AsyncApprovedTimeSheetExcel(string StartDate = "", string EndDate = "", string ProjectId = "", string EmployeeId = "", string LaborCategories = "")
        {
            string reportName = "Employee Assignments";
            int role = SessionDTO.getrole();
            int id = SessionDTO.getid();
            User user = new UserBL().getUserById(id);
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            string local;
            new System.Threading.Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                local = ApprovedTimeSheetExcel(StartDate, EndDate, ProjectId, EmployeeId, LaborCategories, user.Role);
                DownloadMonthlyProjectHoursExcelFile(local, role);
                MailSender.SendDownloadFileLinkEmail(user.Email, local, BaseUrl);
            }).Start();
            return null;
        }

        public string ApprovedTimeSheetExcel(string StartDate = "", string EndDate = "", string ProjectId = "", string EmployeeId = "", string LaborCategories = "", int role=-1)
        {
            List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Role == 0).OrderByDescending(x => x.Date).ToList();
            List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
            List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

            if (StartDate != "")
            {
                DateTime d1 = DateTime.Parse(StartDate);
                allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();
            }
            if (EndDate != "")
            {
                DateTime d2 = DateTime.Parse(EndDate);
                allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
            }
            if (ProjectId != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(ProjectId)).OrderByDescending(x => x.Date).ToList();
            }
            if (EmployeeId != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(EmployeeId)).OrderByDescending(x => x.Date).ToList();
            }
            if (LaborCategories != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategories)).OrderByDescending(x => x.Date).ToList();
            }
            allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
            List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();
            foreach (EntryTime e in allentries)
            {
                EntriesSearchDTO edto = new EntriesSearchDTO()
                {
                    Id = e.Id,
                    Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName,
                    Hours = e.Hour,
                    Project = e.ProjectUserCategory.ProjectCategory.Project.Code,
                    LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
                    Date = e.Date.ToString("MM/dd/yyyy")
                };
                if (e.Is_Approved == 1)
                {
                    edto.Status = "Accepted";
                }
                if (e.Is_Approved == 0)
                {
                    edto.Status = "Pending";
                }
                if (e.Is_Approved == 2)
                {
                    edto.Status = "Rejected";
                    edto.RejectReason = e.RejectReason;
                }

                entriesdtos.Add(edto);
            }
            entriesdtos = entriesdtos.OrderByDescending(x => x.Date).ToList();

            if (role == 2)
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Approved TimeSheet Excel Report by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateApprovedExcelFile(path, entriesdtos);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Approved Time Sheet (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Approved TimeSheet Excel Report by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateApprovedExcelFile(path, entriesdtos);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Approved Time Sheet (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
        }

        public ActionResult DownloadApprovedTimeSheetExcelFile(string path, int role = -1)
        {
            if (role == 2)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Approved TimeSheet Excel Report by Admin (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else if (role == 1)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Approved TimeSheet Excel Report by Manager (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
                return null;
        }
        #endregion

        #region Async ExcelReport Monthly Project Hours Excel
        public FileResult AsyncMonthlyProjectHoursExcel(int check= -1, string StartDate = "", string EndDate = "", string Month = "")
        {
            string reportName = "Employee Assignments";
            int role = SessionDTO.getrole();
            int id = SessionDTO.getid();
            User user = new UserBL().getUserById(id);
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            string local;
            new System.Threading.Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                local = MonthlyProjectHoursExcel(check, StartDate, EndDate, Month, user.Role);
                DownloadProjectReportFile(local, role);
                MailSender.SendDownloadFileLinkEmail(user.Email, local, BaseUrl);
            }).Start();
            return null;
        }
        
        public string MonthlyProjectHoursExcel(int check= -1, string StartDate = "", string EndDate = "", string Month= "", int role=-1)
        {
            //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1 && x.ProjectUserCategory.User.Role == 1).OrderByDescending(x => x.Date).ToList();
            List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Role == 0).OrderByDescending(x => x.Date).ToList();
            allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
            String PassDate = "";

            if (check == 1)
            {
                if (StartDate != "")
                {
                    DateTime d1 = DateTime.Parse(StartDate);
                    allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();
                }
                if (EndDate != "")
                {
                    DateTime d2 = DateTime.Parse(EndDate);
                    allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
                }

                DateTime temp = Convert.ToDateTime(Month);
                PassDate = temp.ToString("MMM-yyyy");
            }
            else
            {
                DateTime currentDate = DateTime.Now;
                DateTime currentMonthStartDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime currentMonthEndDate = currentMonthStartDate.AddMonths(1).AddDays(-1);

                if (currentMonthStartDate != null)
                {
                    //DateTime d1 = DateTime.Parse(currentMonthStartDate);
                    allentries = allentries.Where(x => x.Date >= currentMonthStartDate.Date).OrderByDescending(x => x.Date).ToList();
                }
                if (currentMonthEndDate != null)
                {
                    //DateTime d2 = DateTime.Parse(currentMonthEndDate);
                    allentries = allentries.Where(x => x.Date <= currentMonthEndDate.Date).OrderByDescending(x => x.Date).ToList();
                }
            }
            var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
            List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
            foreach (EntryTime item in ab)
            {
                List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id & x.ProjectUserCategory.User.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
                var ll = allentries2.GroupBy(c => c.ProjectUserCategory.User_Id).Select(p => new { EmployeeId = p.Key, EmployeeName = new UserBL().getUserById(p.Key).FirstName + " " + new UserBL().getUserById(p.Key).LastName, Hours = p.Sum(q => q.Hour) });
                foreach (var it in ll)
                {
                    Project p = new UserBL().getProjectById(item.ProjectUserCategory.ProjectCategory.Project_Id);
                    List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.Is_Authorize == 1).ToList();
                    List<ProjectUserCategory> pucs = new List<ProjectUserCategory>();

                    foreach (ProjectCategory pc in pcs)
                    {
                        List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.Is_Authorize == 1 && x.ProjectCategory_Id == pc.Id).ToList();
                        foreach (var item2 in mypuc)
                        {
                            pucs.Add(item2);
                        }
                    }

                    ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).FirstOrDefault();
                    string pucc = "";
                    if (puc == null)
                    {
                        pucc = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name;
                    }
                    else
                    {
                        pucc = puc.ProjectCategory.LaborCategory.Name;
                    }
                    ProjectReportDTO obj = new ProjectReportDTO()
                    {
                        EmployeeName = it.EmployeeName.ToString(),
                        ProjectName = p.Code,
                        Hours = it.Hours,
                        LCAT = pucc,
                        Id = p.Id,
                    };
                    plist.Add(obj);
                }
            }
            if (role == 2)
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Monthly Project Hours Excel Report by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateMonthlyProjectHourExcelFile(path, plist, PassDate);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Monthly Project Hours (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Monthly Project Hours Excel Report by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateMonthlyProjectHourExcelFile(path, plist, PassDate);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Monthly Project Hours (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
        }

        public ActionResult DownloadMonthlyProjectHoursExcelFile(string path, int role = -1)
        {
            if (role == 2)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Monthly Project Hours Excel Report by Admin (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else if (role == 1)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Monthly Project Hours Excel Report by Manager (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
                return null;
        }
        #endregion

        #region Async ExcelReport Pending Time Card Excel
        [HttpPost]
        public FileResult AsyncPendingTimeCardExcel(string StartDate = "", string EndDate = "", string EmployeeId = "")
        {
            string reportName = "Employee Assignments";
            int role = SessionDTO.getrole();
            int id = SessionDTO.getid();
            User user = new UserBL().getUserById(id);
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            string local;
            new System.Threading.Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                local = PendingTimeCardExcel(StartDate, EndDate, EmployeeId, user.Role);
                DownloadPendingTimeCardExcelFile(local, role);
                MailSender.SendDownloadFileLinkEmail(user.Email, local, BaseUrl);
            }).Start();
            return null;
        }
        
        public string PendingTimeCardExcel(string StartDate = "", string EndDate = "", string EmployeeId = "", int role=-1)
        {
            //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 0 && x.ProjectUserCategory.User.Role == 1).OrderByDescending(x => x.Date).ToList();
            List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 0).OrderByDescending(x => x.Date).ToList();
            List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 1).ToList();
            List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

            if (StartDate != "")
            {
                DateTime d1 = DateTime.Parse(StartDate);
                allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();
            }
            if (EndDate != "")
            {
                DateTime d2 = DateTime.Parse(EndDate);
                allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
            }
            if (EmployeeId != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(EmployeeId)).OrderByDescending(x => x.Date).ToList();
            }

            allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
            List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();
            string desination = "";
            foreach (EntryTime e in allentries)
            {
                if(e.ProjectUserCategory.User.Role == 1)
                {
                    desination = " (Manager)";
                }
                else
                {
                    desination = "";
                }
                EntriesSearchDTO edto = new EntriesSearchDTO()
                {
                    Id = e.Id,
                    Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName + desination,
                    Hours = e.Hour,
                    Project = e.ProjectUserCategory.ProjectCategory.Project.Code,
                    LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
                    Date = e.Date.ToString("MM/dd/yyyy")
                };
                if (e.Is_Approved == 1)
                {
                    edto.Status = "Accepted";
                }
                if (e.Is_Approved == 0)
                {
                    edto.Status = "Pending";
                }
                if (e.Is_Approved == 2)
                {
                    edto.Status = "Rejected";
                    edto.RejectReason = e.RejectReason;
                }
                entriesdtos.Add(edto);
            }
            entriesdtos = entriesdtos.OrderByDescending(x => x.Date).ToList();

            if (role == 2)
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Pending TimeCard Excel File by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generatePendingTimeCardExcelFile(path, entriesdtos);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Pending time Card (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");

            }
            else
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Pending TimeCard Excel File by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generatePendingTimeCardExcelFile(path, entriesdtos);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Pending time Card (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
        }

        public ActionResult DownloadPendingTimeCardExcelFile(string path, int role = -1)
        {
            if (role == 2)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Pending TimeCard Excel File by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else if (role == 1)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Pending TimeCard Excel File by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
                return null;
        }
        #endregion

        #region Async ExcelReport Project Budget Status Excel
        [HttpPost]
        public FileResult AsyncProjectBudgetStatusExcel(string Code = "")
        {
            string reportName = "Employee Assignments";
            int role = SessionDTO.getrole();
            int id = SessionDTO.getid();
            User user = new UserBL().getUserById(id);
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            string local;
            new System.Threading.Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                local = ProjectBudgetStatusExcel(Code, user.Role);
                DownloadProjectBudgetStatusExcelFile(local, role);
                MailSender.SendDownloadFileLinkEmail(user.Email, local, BaseUrl);
            }).Start();
            return null;
        }
        
        public string ProjectBudgetStatusExcel(string Code = "", int role=-1)
        {
            List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize != 0).ToList();
            var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
            List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
            foreach (EntryTime item in ab)
            {
                List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User.Is_Authorize != 0).ToList();
                var ll = allentries2.GroupBy(c => c.ProjectUserCategory.User_Id).Select(p => new { EmployeeId = p.Key, EmployeeName = new UserBL().getUserById(p.Key).FirstName + " " + new UserBL().getUserById(p.Key).LastName, Hours = p.Sum(q => q.Hour) });
                foreach (var it in ll)
                {
                    Project p = new UserBL().getProjectById(item.ProjectUserCategory.ProjectCategory.Project_Id);
                    List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.Is_Authorize == 1).ToList();
                    List<ProjectUserCategory> pucs = new List<ProjectUserCategory>();

                    foreach (ProjectCategory pc in pcs)
                    {
                        List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.Is_Authorize == 1 && x.ProjectCategory_Id == pc.Id).ToList();
                        foreach (var item2 in mypuc)
                        {
                            pucs.Add(item2);
                        }
                    }

                    ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).FirstOrDefault();
                    string pucc = "";
                    double pcost = 0.0;

                    if (puc == null)
                    {
                        pucc = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name;
                    }
                    else
                    {
                        pucc = puc.ProjectCategory.LaborCategory.Name;
                        if (puc.ProjectCategory.Cost == null || puc.ProjectCategory.Cost == "")
                        {
                            pcost = 0.0;
                        }
                        else
                        {
                            pcost = Convert.ToDouble(puc.ProjectCategory.Cost);
                        }
                    }
                    ProjectReportDTO obj = new ProjectReportDTO()
                    {
                        ProjectName = p.Code,
                        Cost = Convert.ToDouble(pcost * it.Hours),
                    };

                    plist.Add(obj);
                }
            }

            List<Project> plist2 = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
            if (Code != "")
            {
                plist2 = plist2.Where(x => x.Code.ToLower().Contains(Code.ToLower())).ToList();
            }
            List<ProjectBudgetDTO> budgetDTOlist = new List<ProjectBudgetDTO>();

            double remaining = 0;
            double totalcost = 0;
            foreach (Project p in plist2)
            {
                totalcost = plist.Where(x => x.ProjectName == p.Code).Sum(y => Convert.ToDouble(y.Cost));

                if (p.Budget == null || p.Budget == "")
                {
                    p.Budget = "0";
                }
                remaining = (Convert.ToDouble(p.Budget) - totalcost) / Convert.ToDouble(p.Budget);
                remaining = remaining * 100;
                remaining = Math.Round(remaining, 2);
                totalcost = Math.Round(totalcost, 2);

                ProjectBudgetDTO obj2 = new ProjectBudgetDTO()
                {
                    Code = p.Code,
                    Budget = p.Budget,
                    TotalCost = totalcost.ToString(),
                    Remaining = remaining.ToString(),
                    RemainingBudget = (Convert.ToDouble(p.Budget)-totalcost).ToString()
                };
                budgetDTOlist.Add(obj2);
            }

            if (role == 2)
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Project Budget Status Excel Report by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateProjectBudgetStatusExcelFile(path, budgetDTOlist);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Budget Status Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Project Budget Status Excel Report by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateProjectBudgetStatusExcelFile(path, budgetDTOlist);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project Budget Status Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
        }

        public ActionResult DownloadProjectBudgetStatusExcelFile(string path, int role = -1)
        {
            if (role == 2)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Project Budget Status Excel Report by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else if (role == 1)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Project Budget Status Excel Report by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
                return null;
        }
        #endregion

        #region Async ExcelReport Distribution Report Excel
        [HttpPost]
        public FileResult AsyncDistributionReportExcel(string StartDate = "", string EndDate = "", string EmployeeId = "")
        {
            string reportName = "Employee Assignments";
            int role = SessionDTO.getrole();
            int id = SessionDTO.getid();
            User user = new UserBL().getUserById(id);
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            string local;
            new System.Threading.Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                local = DistributionReportExcel( StartDate, EndDate, EmployeeId, user.Role);
                DownloadDistributionReportExcelFile(local, role);
                MailSender.SendDownloadFileLinkEmail(user.Email, local, BaseUrl);
            }).Start();
            return null;
        }

        public string DistributionReportExcel(string StartDate = "", string EndDate = "", string EmployeeId = "", int role=-1)
        {
            List<User> user = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 0).ToList();
            List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Role == 0).ToList();
            allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();

            if (StartDate != "")
            {
                DateTime d1 = DateTime.Parse(StartDate);
                allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();
            }
            if (EndDate != "")
            {
                DateTime d2 = DateTime.Parse(EndDate);
                allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
            }

            if (EmployeeId != "")
            {
                user = user.Where(x => x.Id == Convert.ToInt32(EmployeeId)).ToList();
            }
            List<DistributionReportDTO> dlist = new List<DistributionReportDTO>();
            foreach (User i in user)
            {
                DistributionReportDTO obj = new DistributionReportDTO()
                {
                    Name = i.FirstName + " " + i.LastName,
                    VacationHour = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Code == "Vacation" && x.ProjectUserCategory.User.Id == i.Id).Sum(x => x.Hour),
                    OverheadHour = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Code == "Operations - Overhead" && x.ProjectUserCategory.User.Id == i.Id).Sum(x => x.Hour),
                    HolidayHour = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Code == "Holiday" && x.ProjectUserCategory.User.Id == i.Id).Sum(x => x.Hour),
                    OtherHour = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Code != "Operations - Overhead" && x.ProjectUserCategory.ProjectCategory.Project.Code != "Holiday" && x.ProjectUserCategory.ProjectCategory.Project.Code != "Vacation" && x.ProjectUserCategory.User.Id == i.Id).Sum(x => x.Hour)
                };
                dlist.Add(obj);
            }
            if (role == 2)
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Distribution Excel Report by Admin (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateDistributionReportExcelFile(path, dlist);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Distribution Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
            {
                string path = Server.MapPath("~") + "\\Content\\Excel Reports\\Distribution Excel Report by Manager (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx";
                ExcelManagement.generateDistributionReportExcelFile(path, dlist);
                return path;
                //return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Distribution Report (" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
        }

        public ActionResult DownloadDistributionReportExcelFile(string path, int role = -1)
        {
            if (role == 2)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Distribution Excel Report by Admin(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else if (role == 1)
            {
                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Distribution Excel Report by Manager(" + DateTime.Now.ToString("MM-dd-yyyy") + ").xlsx");
            }
            else
                return null;
        }
        #endregion
    }
}