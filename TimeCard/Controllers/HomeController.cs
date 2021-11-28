using TimeCard.Helping_Classes;
using TimeCard.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TimeCard.Models;
using System.Globalization;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using System.Web.WebPages;

namespace TimeCard.Controllers
{
    public class HomeController : Controller
    {
        SessionDTO sessiondto = new SessionDTO();

        public ActionResult Index(string err = "")
        {
            if (sessiondto.getName() == null)
            {
                ViewBag.message = err;
                return View();
            }
            else
            {
                if (sessiondto.getRole() != 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    return RedirectToAction("TimeSheetEntry", "Home");
                }
            }
        }

        public ActionResult Dashboard(string msg="")
        {
            if (sessiondto.getName() ==  null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (sessiondto.getRole() == 0)
            {
                return RedirectToAction("TimeSheetEntry", "Home");
            }
            else if (sessiondto.getRole() == 2)
            {

                int totalemployees = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 0).Count();
                int totalRemovedemployees = new UserBL().getUserList().Where(x => x.Is_Authorize == 0 && x.Role == 0).Count();
                int totalmanagers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 1).Count();
                int totalRemovedmanagers = new UserBL().getUserList().Where(x => x.Is_Authorize == 0 && x.Role == 1).Count();
                int totalprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).Count();
                int totalacceptedtimecards = new UserBL().getEntryTimeList().Where(x => x.Is_Approved == 1).Count();

                ViewBag.totalemployees = totalemployees;
                ViewBag.totalRemovedemployees = totalRemovedemployees;
                ViewBag.totalmanagers = totalmanagers;
                ViewBag.totalRemovedmanagers = totalRemovedmanagers;
                ViewBag.totalprojects = totalprojects;
                ViewBag.totalacceptedtimecards = totalacceptedtimecards;
                ViewBag.Message = msg;

                return View();
            }
            else
            {
                int assignedemployees = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 0 && x.User_Id == sessiondto.getId()).Count();
                int pendingcard = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User.User_Id == sessiondto.getId() && x.Is_Approved == 0 && x.Is_Authorize == 1).Count();
                int assignedprojects = new UserProjectReportBL().GetActiveUserProjectReports().Where(x => x.UserId == sessiondto.getId() && x.IsActive == 1).Count();


                ViewBag.assignedemployees = assignedemployees;
                ViewBag.pendingcard = pendingcard;
                ViewBag.assignedprojects = assignedprojects;

                ViewBag.Message = msg;

                return View();

            }
        }

        #region TimeSheet
        public ActionResult TimeSheetEntry(string msg="")
        {
            if (sessiondto.getName() != null)
            {
                DateTime startingdate = new UserBL().getStartDateList().Where(x => x.IsActive == 1).FirstOrDefault().StartingDate;

                DateTime date1 = getStartingDate(startingdate);
                DateTime date2 = date1.AddDays(+13);

                if (sessiondto.getRole() == 0 || sessiondto.getRole() == 1)
                {
                    List<Project> myprojects = new List<Project>();
                    List<LaborCategory> mylaborcategories = new List<LaborCategory>();

                    List<ProjectUserCategory> myproj = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();

                    foreach (ProjectUserCategory p in myproj)
                    {
                        myprojects.Add(p.ProjectCategory.Project);
                        mylaborcategories.Add(p.ProjectCategory.LaborCategory);
                    }

                    List<EntryTime> myentries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.Date.Date >= date1.Date && x.Date.Date <= date2.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                    List<double> workhours = new List<double>();
                    double totalhours = 0;


                    ViewBag.date1 = date1.ToLongDateString();
                    ViewBag.date2 = date2.ToLongDateString();
                    ViewBag.laborcategoriescount = mylaborcategories.Count();
                    ViewBag.laborcategories = mylaborcategories;
                    //ViewBag.myEntries = myentries;
                    ViewBag.myEntries = myentries.Where(x=> x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
                    ViewBag.workhours = workhours;
                    ViewBag.totalhours = totalhours;
                    //ViewBag.projects = myprojects.OrderBy(x => x.Code);
                    ViewBag.projects = myprojects.OrderBy(x => x.Code).Where(x=>x.Is_Authorize == 1);
                    ViewBag.startdate = date1;
                    ViewBag.enddate = date2;
                    ViewBag.Message = msg;
                }

                if (sessiondto.getRole() == 2)
                {
                    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                    List<User> allusers = new UserBL().getUserList().Where(x => x.Role != 2 && x.Is_Authorize == 1).ToList();
                    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Date.Date >= date1 && x.Date.Date <= date2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    List<double> workhours = new List<double>();
                    double totalhours = 0;
                    List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

                    ViewBag.laborcategoriescount = laborcategories.Count();
                    ViewBag.laborcategories = laborcategories;
                    //ViewBag.allEntries = allentries;
                    ViewBag.allEntries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
                    ViewBag.workhours = workhours;
                    ViewBag.totalhours = totalhours;
                    ViewBag.projects = allprojects.OrderBy(x => x.Code).Where(x=> x.Is_Authorize == 1);
                    ViewBag.allusers = allusers;
                    ViewBag.startdate = date1;
                    ViewBag.enddate = date2;
                    ViewBag.Message = msg;

                }

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult SaveTimeSheetEntry(FormCollection fc, int count)
        {
            if (sessiondto.getName() != null)
            {
                string Message = "";
                User u = new UserBL().getUserById(sessiondto.getId());
                List<EntryTime> todayentries = new List<EntryTime>();

                Project p = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).FirstOrDefault();
                
                for(int i = 0; i <= count; i++)
                {
                    string Date = fc["Date" + i];

                    if(p.LockDate >= Convert.ToDateTime(Date).Date)
                    {
                        return RedirectToAction("TimeSheetEntry", "Home", new { msg = "The timecard period you submitted has been locked. Please contact your timecard administrator." });
                    }

                }

                for (int i = 0; i <= count; i++)
                {
                    string Project_Id = fc["Projects" + i];
                    string Date = fc["Date" + i];
                    string Hour = fc["Hours" + i];

                    //if(Convert.ToDouble(Hour) > 80)
                    //{
                    //    return RedirectToAction("TimeSheetEntry", "Home", new { msg = "80 Hours Max" });
                    //}
                    int ProjectUserCategory_Id = new UserBL().getProjectUserCategoryList().Where(x => x.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id) && x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).FirstOrDefault().Id;
                    if (Project_Id != "" && Date != null && Hour != null)
                    {
                        int ecount = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize != 0 && Convert.ToDateTime(x.Date).Date == Convert.ToDateTime(Date).Date  && x.ProjectUserCategory_Id == ProjectUserCategory_Id && (x.Is_Approved == 0 || x.Is_Approved == 1)).Count();
                        if (ecount == 0)
                        {
                            EntryTime e = new EntryTime()
                            {
                                Date = Convert.ToDateTime(Date),
                                Hour = Convert.ToDouble(Hour),
                                ProjectUserCategory_Id = ProjectUserCategory_Id,
                                Is_Approved = 0,
                                Type = "Work",
                                Is_Authorize = 1,
                                Created_At = DateTime.Now
                            };
                            new UserBL().AddEntryTime(e);

                            todayentries.Add(e);
                        }
                        else
                        {
                            string pName = new UserBL().getProjectById(Convert.ToInt32(Project_Id)).Code;
                            Message += "Entry Already Exist # " + Date + ", " + pName + "/ " ; 
                        }


                        //if(new UserBL().getProjectById(Convert.ToInt32(Project_Id)).Code == "Vacation")
                        //{
                        //    VacationBank v = new UserBL().getVacationBankList().Where(x => x.User_Id == u.Id).FirstOrDefault();
                        //    double availablehours = Convert.ToDouble(v.AvailableVacationHours) - e.Hour;

                        //    VacationBank vacation = new VacationBank()
                        //    {
                        //        Id = v.Id,
                        //        User_Id = v.User_Id,
                        //        VacationHoursPerYear = v.VacationHoursPerYear,
                        //        AccuralVacationHoursPerYear = v.AccuralVacationHoursPerYear,
                        //        Is_Authorize = v.Is_Authorize,
                        //        AvailableVacationHours = availablehours
                        //    };
                        //    new UserBL().UpdateVacationBank(vacation);
                        //}
                    }
                }


                if (u.User_Id != null)
                {
                    foreach (EntryTime e in todayentries)
                    {
                        User manager = new UserBL().getUserById(Convert.ToInt32(u.User_Id));

                        MailMessage msg = new MailMessage();

                        string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
                        text += "  font-family: 'Bree Serif', serif; }";
                        text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
                        text += " .line {       margin-bottom: 20px; }";

                        msg.From = new MailAddress("timesheets@strattechnologies.com");
                        msg.To.Add(manager.Email);
                        msg.Subject = "Time Card Entry";
                        msg.IsBodyHtml = true;
                        string temp = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'></div> </nav><center><div><p class='text-center'>" + sessiondto.getName() + " has submitted " + e.Hour + " hours for " + e.Date.ToString("MM/dd/yyyy") + ". This entry is pending your approval. Please log in and accept or reject the entry at <br /><a href='http://timesheets.strattechnologies.com' target='_blank'>http://timesheets.strattechnologies.com</a></div></center>";

                        temp += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                        string link = "http://strategic24-001-site1.ctempurl.com/Home/TimeCardMail?Id=" + StringCipher.Base64Encode(e.Id.ToString());
                        link = link.Replace("+", "%20");
                        temp = temp.Replace("ACCEPT/REJECT", link);
                        msg.Body = temp;

                        using (SmtpClient client = new SmtpClient())
                        {
                            client.EnableSsl = false;
                            client.UseDefaultCredentials = false;
                            client.Credentials = new NetworkCredential("timesheets@strattechnologies.com", "T1imeS4h33ts!3");
                            client.Host = "mail.strattechnologies.com";
                            client.Port = 587;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;

                            client.Send(msg);
                        }
                    }
                }

                return RedirectToAction("TimeSheetEntry", "Home", new { msg = Message});
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpdateTimeSheetEntry(int Id, int Project_Id, string Date1, float Hour)
        {
            if (sessiondto.getName() != null)
            {
                User u = new UserBL().getUserById(sessiondto.getId());
                EntryTime e1 = new UserBL().getEntryTimeById(Id);

                EntryTime oldentry = new EntryTime()
                {
                    Id = e1.Id,
                    ProjectUserCategory_Id = e1.ProjectUserCategory_Id,
                    Date = Convert.ToDateTime(Date1),
                    //Date = Date1,
                    Type = "Work",
                    Hour = Hour,
                    Is_Approved = 0,
                    Is_Authorize = 1,
                    RejectReason = e1.RejectReason,
                    DeleteReason = e1.DeleteReason,
                    Created_At = DateTime.Now
                };
                new UserBL().UpdateEntryTime(oldentry);

                if (u.User_Id != null)
                {
                    User manager = new UserBL().getUserById(Convert.ToInt32(u.User_Id));

                    MailMessage msg = new MailMessage();

                    string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
                    text += "  font-family: 'Bree Serif', serif; }";
                    text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
                    text += " .line {       margin-bottom: 20px; }";

                    msg.From = new MailAddress("timesheets@strattechnologies.com");
                    msg.To.Add(manager.Email);
                    msg.Subject = "Time Card Entry";
                    msg.IsBodyHtml = true;
                    string temp = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'></div> </nav><center><div><p class='text-center'>" + sessiondto.getName() + " has submitted " + oldentry.Hour + " hours for " + oldentry.Date.ToString("MM/dd/yyyy") + ". This entry is pending your approval. Please log in and accept or reject the entry at <br /><a href='http://timesheets.strattechnologies.com' target='_blank'>http://timesheets.strattechnologies.com</a></div></center>";


                    temp += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                    string link = "http://strategic24-001-site1.ctempurl.com/Home/TimeCardMail?Id=" + StringCipher.Base64Encode(oldentry.Id.ToString());
                    link = link.Replace("+", "%20");
                    temp = temp.Replace("ACCEPT/REJECT", link);
                    msg.Body = temp;

                    using (SmtpClient client = new SmtpClient())
                    {
                        client.EnableSsl = false;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential("timesheets@strattechnologies.com", "T1imeS4h33ts!3");
                        client.Host = "mail.strattechnologies.com";
                        client.Port = 587;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;

                        client.Send(msg);
                    }
                }

                return RedirectToAction("TimeSheetEntry", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DeleteTimeSheetEntry(string Id, string deletereason)
        {
            if (sessiondto.getRole() == 1)
            {
                EntryTime e = new UserBL().getEntryTimeById(Convert.ToInt32(Id));

                EntryTime et = new EntryTime()
                {
                    Id = e.Id,
                    ProjectUserCategory_Id = e.ProjectUserCategory_Id,
                    Date = e.Date,
                    Type = e.Type,
                    Hour = e.Hour,
                    Is_Approved = e.Is_Approved,
                    Is_Authorize = 0,
                    Created_At = e.Created_At,
                    EntryTime_Id = e.EntryTime_Id,
                    RejectReason = e.RejectReason,
                    DeleteReason = deletereason
                };
                new UserBL().UpdateEntryTime(et);

                MailMessage msg = new MailMessage();

                string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
                text += "  font-family: 'Bree Serif', serif; }";
                text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
                text += " .line {       margin-bottom: 20px; }";

                msg.From = new MailAddress("timesheets@strattechnologies.com");
                msg.To.Add(e.ProjectUserCategory.User.Email);
                msg.Subject = "Rejection of Time Card Entry";
                msg.IsBodyHtml = true;
                string temp = "";

                if (deletereason != null)
                {
                    temp = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'></div> </nav><center><div><p>Your following entry has been deleted. </p><p>Entry Date: " + e.Date + " </p><p>Entry Hours: " + e.Hour + " </p><p>Entry Project: " + e.ProjectUserCategory.ProjectCategory.Project.Code + " </p><br /><h1 class='text-center'>Reason!</h1><h3 class='text-center'> " + deletereason + " </h3><br></div></center>";

                }
                temp += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                msg.Body = temp;

                using (SmtpClient client = new SmtpClient())
                {
                    client.EnableSsl = false;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("timesheets@strattechnologies.com", "T1imeS4h33ts!3");
                    client.Host = "mail.strattechnologies.com";
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    client.Send(msg);
                }

                return RedirectToAction("AssignedEmployeesTimeSheet", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public DateTime getStartingDate(DateTime dbDate)
        {
            DateTime now = DateTime.Now;
            DateTime endingDate = dbDate.AddDays(+13);

            //pay periods 
            if (now > endingDate)
            {
                dbDate = endingDate.AddDays(+1);
                return getStartingDate(dbDate);
            }
            return dbDate;
        }

        public ActionResult AssignedEmployeesTimeSheet(int check=-1, string startdate = "", string enddate = "", string Employee_Id = "", string Project_Id = "", string Status = "")
        {
            if (sessiondto.getRole() == 1)
            {
                if (check == -1)
                {
                    List<Project> allprojects = new List<Project>();
                    List<ProjectCategory> myprojcats = new List<ProjectCategory>();
                    List<ProjectUserCategory> myprojusers = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
                    List<LaborCategory> mylaborcategories = new List<LaborCategory>();

                    if (myprojusers != null)
                    {
                        foreach (ProjectUserCategory pu in myprojusers)
                        {
                            ProjectCategory p = new UserBL().getProjectCategoryById(pu.ProjectCategory_Id);
                            allprojects.Add(p.Project);
                            mylaborcategories.Add(p.LaborCategory);
                        }
                    }

                    List<User> allusers = new UserBL().getUserList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
                    List<EntryTime> enteries = new List<EntryTime>();
                    foreach (User u in allusers)
                    {
                        List<EntryTime> enteries2 = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == u.Id).ToList();

                        enteries.AddRange(enteries2);
                    }




                    //  List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId()).ToList();

                    ViewBag.userscount = allusers.Count();
                    ViewBag.allEntries = enteries.OrderByDescending(x => x.Date).Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    //ViewBag.projects = allprojects.OrderBy(x => x.Code);

                    ViewBag.projects = allprojects.OrderBy(x => x.Code).Where(x => x.Is_Authorize == 1).ToList();
                    ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                    ViewBag.laborcategories = mylaborcategories.OrderBy(x => x.Name);
                    ViewBag.laborcategoriescount = mylaborcategories.Count();
                }
                else
                {
                    List<Project> allprojects = new List<Project>();
                    List<ProjectCategory> myprojcats = new List<ProjectCategory>();
                    List<ProjectUserCategory> myprojusers = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
                    List<LaborCategory> mylaborcategories = new List<LaborCategory>();

                    if (myprojusers != null)
                    {
                        foreach (ProjectUserCategory pu in myprojusers)
                        {
                            ProjectCategory p = new UserBL().getProjectCategoryById(pu.ProjectCategory_Id);
                            allprojects.Add(p.Project);
                            mylaborcategories.Add(p.LaborCategory);
                        }
                    }

                    List<User> allusers = new UserBL().getUserList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
                    List<EntryTime> enteries = new List<EntryTime>();
                    foreach (User u in allusers)
                    {
                        List<EntryTime> enteries2 = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == u.Id).ToList();

                        enteries.AddRange(enteries2);
                    }


                    if (startdate != "")
                    {
                        DateTime d1 = DateTime.Parse(startdate);
                        enteries = enteries.Where(x => x.Date >= d1.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                    }
                    if (enddate != "")
                    {
                        DateTime d2 = DateTime.Parse(enddate);
                        enteries = enteries.Where(x => x.Date <= d2.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Project_Id != "")
                    {
                        enteries = enteries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                    }
                    if (Employee_Id != "")
                    {
                        enteries = enteries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(Employee_Id) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Status == "Pending")
                    {
                        enteries = enteries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Status == "Accepted")
                    {
                        enteries = enteries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Status == "Rejected")
                    {
                        enteries = enteries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }



                    ViewBag.userscount = allusers.Count();
                    ViewBag.allEntries = enteries.OrderByDescending(x => x.Date).Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                    ViewBag.projects = allprojects.OrderBy(x => x.Code).Where(x => x.Is_Authorize == 1).ToList();
                    ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                    ViewBag.laborcategories = mylaborcategories.OrderBy(x => x.Name);
                    ViewBag.laborcategoriescount = mylaborcategories.Count();

                    ViewBag.sdate = startdate;
                    ViewBag.edate = enddate;
                    ViewBag.eid = Employee_Id;
                    ViewBag.pid = Project_Id;
                    ViewBag.sts = Status;




                }

                return View();
            }
            else if (sessiondto.getRole() == 2)
            {
                List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
                List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().ToList();

                ViewBag.userscount = allusers.Count();
                ViewBag.allEntries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
                ViewBag.projects = allprojects.OrderBy(x => x.Code);
                ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                ViewBag.laborcategoriescount = laborcategories.Count();

                return View();

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SearchAssignedEmployeesTimeSheet(string startdate = "", string enddate = "", string Employee_Id = "", string Project_Id = "", string Status = "")
        {
            return RedirectToAction("AssignedEmployeesTimeSheet", "Home", new { check = 1, startdate = startdate, enddate = enddate, Employee_Id = Employee_Id, Project_Id = Project_Id, Status = Status });
        }
        
        public ActionResult SearchManagerEntryTime(string startdate = "", string enddate = "", string Project_Id = "", string Status = "")
        {
            return RedirectToAction("ManagerEntryTime", "Home", new { check = 1, startdate = startdate, enddate = enddate,  Project_Id = Project_Id, Status = Status });
        }

        public ActionResult ManagerEntryTime(int check = -1, string startdate = "", string enddate = "", string Project_Id = "", string Status = "")
        {
            if (sessiondto.getRole() == 1)
            {
                if (check == -1)
                {
                    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
                    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                    List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

                    ViewBag.userscount = allusers.Count();
                    ViewBag.allEntries = allentries.Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
                    ViewBag.projects = allprojects.OrderBy(x => x.Code);
                    ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                    ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                    ViewBag.laborcategoriescount = laborcategories.Count();

                    return View();
                }
                else
                {
                    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
                    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                    List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();


                    if (startdate != "")
                    {
                        DateTime d1 = DateTime.Parse(startdate);
                        allentries = allentries.Where(x => x.Date >= d1.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                    }
                    if (enddate != "")
                    {
                        DateTime d2 = DateTime.Parse(enddate);
                        allentries = allentries.Where(x => x.Date <= d2.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Project_Id != "")
                    {
                        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                    }

                    if (Status == "Pending")
                    {
                        allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Status == "Accepted")
                    {
                        allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Status == "Rejected")
                    {
                        allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }

                    ViewBag.userscount = allusers.Count();
                    ViewBag.allEntries = allentries.Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
                    ViewBag.projects = allprojects.OrderBy(x => x.Code);
                    ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                    ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                    ViewBag.laborcategoriescount = laborcategories.Count();

                    return View();


                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public string AssignedEmployeesReport(string startdate, string enddate, string Project_Id, string Employee_Id, string Status)
        {
            List<User> allusers = new UserBL().getUserList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
            List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
            List<EntryTime> allentries = new List<EntryTime>();

            foreach (User u in allusers)
            {
                List<ProjectUserCategory> userentries = u.ProjectUserCategories.Where(x => x.User_Id == u.Id).ToList();

                foreach (ProjectUserCategory p in userentries)
                {
                    List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == p.Id).ToList();
                    foreach (EntryTime e in enteries)
                    {
                        allentries.Add(e);
                    }

                }
            }

            if (startdate != "")
            {
                DateTime d1 = DateTime.Parse(startdate);
                allentries = allentries.Where(x => x.Date >= d1.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

            }
            if (enddate != "")
            {
                DateTime d2 = DateTime.Parse(enddate);
                allentries = allentries.Where(x => x.Date <= d2.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Project_Id != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

            }
            if (Employee_Id != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(Employee_Id) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Pending")
            {
                allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Accepted")
            {
                allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Rejected")
            {
                allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }

            allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

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

            return JsonConvert.SerializeObject(entriesdtos, Formatting.Indented,
                   new JsonSerializerSettings()
                   {
                       ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                   });
        }

        public ActionResult ProjectReport(int check = -1, string startdate = "", string enddate = "", int Project_Id = -1)
        {
            if (sessiondto.getRole() == 1)
            {
                List<UserProjectReport> uprlist = new UserProjectReportBL().GetActiveUserProjectReports().Where(x => x.UserId == sessiondto.getId()).ToList();
                if (uprlist.Count == 0)
                {
                    return RedirectToAction("Dashboard", "Home", new { msg = "No project is assigned" });
                }

                List<int> pId = new List<int>();

                foreach (UserProjectReport i in uprlist)
                {
                    pId.Add((int)i.ProjectId);
                }

                List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

                ViewBag.projects = allprojects.OrderBy(x => x.Code);

                var pucList = new UserBL().getProjectUserCategoryList().OrderBy(x => x.User.FirstName).Select(x => new { x.User_Id, x.ProjectCategory_Id }).Distinct().ToList();

                
                List<ProjectReportDTO> prdto = new List<ProjectReportDTO>();

                foreach (var puc in pucList)
                {
                    List<ProjectUserCategory> puclist2 = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == puc.User_Id && x.ProjectCategory_Id == puc.ProjectCategory_Id).ToList();


                    foreach (ProjectUserCategory puc2 in puclist2)
                    {
                        List<EntryTime> e = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == puc2.Id && x.Is_Authorize == 1 && x.Is_Approved != 2 && x.ProjectUserCategory.User.Is_Authorize != 0 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();

                        if (check == 1)
                        {
                            if (startdate != "")
                            {
                                DateTime d1 = DateTime.Parse(startdate);
                                e = e.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

                            }
                            if (enddate != "")
                            {
                                DateTime d2 = DateTime.Parse(enddate);
                                e = e.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
                            }
                            if (Project_Id != -1)
                            {
                                e = e.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id)).OrderByDescending(x => x.Date).ToList();

                            }
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
                                    Id = e[0].ProjectUserCategory.ProjectCategory.Project_Id,
                                    EmployeeName = e[0].ProjectUserCategory.User.FirstName + " " + e[0].ProjectUserCategory.User.LastName,
                                    Status = usr.FirstName,
                                    ProjectName = e[0].ProjectUserCategory.ProjectCategory.Project.Code,
                                    Hours = hrs,
                                    LCAT = pcost.ProjectCategory.LaborCategory.Name,
                                    Cost = cst
                                };

                                prdto.Add(obj);
                        }
                    }
                }

                ViewBag.ProjectReportList = prdto;
                ViewBag.ProjectId = pId;
                ViewBag.SD = startdate;
                ViewBag.ED = enddate;
                ViewBag.PId = Project_Id;

                return View();
            }
            else if (sessiondto.getRole() == 2)
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

                        if (check == 1)
                        {
                            if (startdate != "")
                            {
                                DateTime d1 = DateTime.Parse(startdate);
                                e = e.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

                            }
                            if (enddate != "")
                            {
                                DateTime d2 = DateTime.Parse(enddate);
                                e = e.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
                            }
                            if (Project_Id != -1)
                            {
                                e = e.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id)).OrderByDescending(x => x.Date).ToList();

                            }
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
                                EmployeeName = e[0].ProjectUserCategory.User.FirstName + " " + e[0].ProjectUserCategory.User.LastName,
                                Status = usr.FirstName,
                                ProjectName = e[0].ProjectUserCategory.ProjectCategory.Project.Code,
                                Hours = hrs,
                                LCAT = pcost.ProjectCategory.LaborCategory.Name,
                                Cost = cst
                            };

                            prdto.Add(obj);
                        }
                    }
                }

                List<ProjectReportDTO> finalprdto = new List<ProjectReportDTO>();
                
                foreach (var VARIABLE1 in  prdto)
                {

                    
                    if (finalprdto != null)
                    {
                        bool check1 = false;
                        foreach (var item in finalprdto)
                        {
                            if (item.EmployeeName == VARIABLE1.EmployeeName &&
                                item.ProjectName == VARIABLE1.ProjectName)
                            {
                                check1 = true;
                                item.LCAT = item.LCAT + "," + VARIABLE1.LCAT;
                                item.Hours = item.Hours +  VARIABLE1.Hours;
                                item.Cost = item.Cost+ VARIABLE1.Cost;
                                
                            }
                        }
                        if (!check1)
                        {
                            finalprdto.Add(VARIABLE1);
                        }
                    }
                    else
                    {
                        finalprdto.Add(VARIABLE1);
                    }

                }
                ////fine duplicate project name   
                //var query = prdto.GroupBy(x => x.ProjectName)
                //    .Where(g => g.Count() > 1)
                //    .Select(y => y.Key)
                //    .ToList();

                //var queryobject = prdto.GroupBy(x => query)
                //    .Where(g => g.Count() > 1)
                //    .Select(y => y.Key)
                //    .ToList();

                //var groupedResult = prdto.Distinct();

                //var query123 = prdto.GroupBy(x => new { x.EmployeeName, x.ProjectName });
               
                //var VARIABLE = query123.ToArray();
                //var vList = query123.ToList()[0].GetEnumerator(
                //var xx = VARIABLE
                //foreach (var VARIABE in xx)
                //{
                //    var z = xx[0];
                //}
               
                //for (int i = 0; i < VARIABLE.Length; i++)
                //{
                //    Console.WriteLine(VARIABLE[i]);
                //    Console.WriteLine(VARIABLE[i].GetEnumerator());
                //    //Console.WriteLine(VARIABLE[i].);
                //}

                //for (int i = 0; i < prdto.Count; i++)
                //{
                //    if (query[0] == prdto[i].ProjectName || query[1] == prdto[i].ProjectName || query[2] == prdto[i].ProjectName)
                //    {
                //        finalprdto.Add(prdto[i]);
                //    }
                //    //if (query[1] == prdto[i].ProjectName)
                //    //{
                //    //    finalprdto.Add(prdto[i]);
                //    //}     
                //    //if (query[2] == prdto[i].ProjectName)
                //    //{
                //    //    finalprdto.Add(prdto[i]);
                //    //}   
                //}
                ////finalprdto = prdto;
                ////ViewBag.ProjectReportList = prdto;

                ViewBag.ProjectReportList = finalprdto;

                ViewBag.SD = startdate;
                ViewBag.ED = enddate;
                ViewBag.PId = Project_Id;

                return View();

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //Old Code Done by Haider Bhai
        //public ActionResult ProjectReport(int check = -1, string startdate = "", string enddate = "", int Project_Id = -1)
        //{
        //    if (sessiondto.getRole() == 1)
        //    {
        //        List<UserProjectReport> uprlist = new UserProjectReportBL().GetActiveUserProjectReports().Where(x => x.UserId == sessiondto.getId()).ToList();
        //        if (uprlist.Count == 0)
        //        {
        //            return RedirectToAction("Dashboard", "Home", new { msg = "No project is assigned" });
        //        }

        //        List<int> pId = new List<int>();

        //        foreach (UserProjectReport i in uprlist)
        //        {
        //            pId.Add((int)i.ProjectId);
        //        }


        //        List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved != 2 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize != 0).ToList();
        //        List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
        //        List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
        //        List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

        //        ViewBag.userscount = allusers.Count();
        //        ViewBag.projects = allprojects.OrderBy(x => x.Code);
        //        ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
        //        ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
        //        ViewBag.laborcategoriescount = laborcategories.Count();


        //        if (check != 1)
        //        {
        //            var a = allentries.GroupBy(x => new { x.ProjectUserCategory.User.FirstName }).Select(x => x.FirstOrDefault());

        //            ViewBag.allEntries1 = a;
        //            ViewBag.allEntries = allentries.OrderByDescending(x => x.Date);

        //            var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
        //            List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
        //            foreach (EntryTime item in ab)
        //            {
        //                List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User.Is_Authorize != 0).ToList();
        //                var ll = allentries2.GroupBy(c => c.ProjectUserCategory.User_Id).Select(p => new { EmployeeId = p.Key, EmployeeName = new UserBL().getUserById(p.Key).FirstName + " " + new UserBL().getUserById(p.Key).LastName, Hours = p.Sum(q => q.Hour) });
        //                foreach (var it in ll)
        //                {
        //                    Project p = new UserBL().getProjectById(item.ProjectUserCategory.ProjectCategory.Project_Id);
        //                    List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.Is_Authorize == 1).ToList();
        //                    List<ProjectUserCategory> pucs = new List<ProjectUserCategory>();

        //                    foreach (ProjectCategory pc in pcs)
        //                    {
        //                        //List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.Is_Authorize == 1 && x.ProjectCategory_Id == pc.Id).ToList();
        //                        List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.ProjectCategory_Id == pc.Id).ToList();

        //                        foreach (var item2 in mypuc)
        //                        {
        //                            pucs.Add(item2);
        //                        }

        //                    }

        //                    //ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).FirstOrDefault();
        //                    ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).LastOrDefault();

        //                    string pucc = "";
        //                    double pcost = 0.0;

        //                    if (puc == null)
        //                    {
        //                        pucc = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name;
        //                    }
        //                    else
        //                    {
        //                        pucc = puc.ProjectCategory.LaborCategory.Name;
        //                        if (puc.ProjectCategory.Cost == null || puc.ProjectCategory.Cost == "")
        //                        {
        //                            pcost = 0.0;
        //                        }
        //                        else
        //                        {
        //                            pcost = Convert.ToDouble(puc.ProjectCategory.Cost);
        //                        }
        //                    }

        //                    User usr = new UserBL().getUserById(it.EmployeeId);
        //                    if (usr.Is_Authorize == 2)
        //                    {
        //                        usr.FirstName = "Inactive";
        //                    }
        //                    else
        //                    {
        //                        usr.FirstName = "";
        //                    }

        //                    ProjectReportDTO obj = new ProjectReportDTO()
        //                    {
        //                        Id = p.Id,
        //                        EmployeeName = it.EmployeeName.ToString(),
        //                        Status = usr.FirstName,
        //                        ProjectName = p.Code,
        //                        Hours = it.Hours,
        //                        LCAT = pucc,
        //                        Cost = Convert.ToDouble(pcost),
        //                    };
        //                    plist.Add(obj);
        //                }

        //            }


        //            ViewBag.ProjectId = pId;
        //            ViewBag.ProjectReportList = plist;
        //        }
        //        else
        //        {
        //            if (startdate != "")
        //            {
        //                DateTime d1 = DateTime.Parse(startdate);
        //                allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();
        //            }
        //            if (enddate != "")
        //            {
        //                DateTime d2 = DateTime.Parse(enddate);
        //                allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
        //            }
        //            if (Project_Id != -1)
        //            {
        //                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id)).OrderByDescending(x => x.Date).ToList();

        //            }

        //            var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
        //            List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
        //            foreach (EntryTime item in ab)
        //            {
        //                List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User.Is_Authorize != 0 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();

        //                var ll = allentries2.GroupBy(c => c.ProjectUserCategory.User_Id).Select(p => new { EmployeeId = p.Key, EmployeeName = new UserBL().getUserById(p.Key).FirstName + " " + new UserBL().getUserById(p.Key).LastName, Hours = p.Sum(q => q.Hour) });
        //                foreach (var it in ll)
        //                {
        //                    Project p = new UserBL().getProjectById(item.ProjectUserCategory.ProjectCategory.Project_Id);
        //                    List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.Is_Authorize == 1).ToList();
        //                    List<ProjectUserCategory> pucs = new List<ProjectUserCategory>();

        //                    foreach (ProjectCategory pc in pcs)
        //                    {
        //                        //List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.Is_Authorize == 1 && x.ProjectCategory_Id == pc.Id).ToList();
        //                        List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.ProjectCategory_Id == pc.Id).ToList();

        //                        foreach (var item2 in mypuc)
        //                        {
        //                            pucs.Add(item2);
        //                        }
        //                    }

        //                    //ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).FirstOrDefault();
        //                    ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).LastOrDefault();
        //                    string pucc = "";
        //                    double pcost = 0.0;

        //                    if (puc == null)
        //                    {
        //                        pucc = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name;
        //                    }
        //                    else
        //                    {
        //                        pucc = puc.ProjectCategory.LaborCategory.Name;
        //                        if (puc.ProjectCategory.Cost == null || puc.ProjectCategory.Cost == "")
        //                        {
        //                            pcost = 0.0;
        //                        }
        //                        else
        //                        {
        //                            pcost = Convert.ToDouble(puc.ProjectCategory.Cost);
        //                        }

        //                    }
        //                    User usr = new UserBL().getUserById(it.EmployeeId);
        //                    if (usr.Is_Authorize == 2)
        //                    {
        //                        usr.FirstName = "Inactive";
        //                    }
        //                    else
        //                    {
        //                        usr.FirstName = "";
        //                    }
        //                    ProjectReportDTO obj = new ProjectReportDTO()
        //                    {
        //                        Id = p.Id,
        //                        EmployeeName = it.EmployeeName.ToString(),
        //                        Status = usr.FirstName,
        //                        ProjectName = p.Code,
        //                        Hours = it.Hours,
        //                        LCAT = pucc,
        //                        Cost = Convert.ToDouble(pcost),
        //                    };
        //                    plist.Add(obj);
        //                }

        //            }

        //            ViewBag.ProjectId = pId;
        //            ViewBag.ProjectReportList = plist;
        //        }

        //        ViewBag.SD = startdate;
        //        ViewBag.ED = enddate;
        //        ViewBag.PId = Project_Id;

        //        return View();
        //    }
        //    else if (sessiondto.getRole() == 2)
        //    {
        //        //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).ToList();

        //        List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved != 2 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize != 0 && (x.ProjectUserCategory.Is_Authorize == 0 || x.ProjectUserCategory.Is_Authorize == 1)).ToList();
        //        List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
        //        List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
        //        List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

        //        ViewBag.userscount = allusers.Count();
        //        ViewBag.projects = allprojects.OrderBy(x => x.Code);
        //        ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
        //        ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
        //        ViewBag.laborcategoriescount = laborcategories.Count();


        //        if (check != 1)
        //        {
        //            //new update
        //            var a = allentries.GroupBy(x => new { x.ProjectUserCategory.User.FirstName }).Select(x => x.FirstOrDefault());

        //            ViewBag.allEntries1 = a;
        //            ViewBag.allEntries = allentries.OrderByDescending(x => x.Date);
        //            //new update


        //            //var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Where(x=>x.).Select(x => x.FirstOrDefault());
        //            var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
        //            List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
        //            foreach (EntryTime item in ab)
        //            {
        //                //List<EntryTime> allentries2 = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1  && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id).ToList();

        //                List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User.Is_Authorize != 0).ToList();
        //                var ll = allentries2.GroupBy(c => c.ProjectUserCategory.User_Id).Select(p => new { EmployeeId = p.Key, EmployeeName = new UserBL().getUserById(p.Key).FirstName + " " + new UserBL().getUserById(p.Key).LastName, Hours = p.Sum(q => q.Hour) });
        //                foreach (var it in ll)
        //                {
        //                    Project p = new UserBL().getProjectById(item.ProjectUserCategory.ProjectCategory.Project_Id);
        //                    List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.Is_Authorize == 1).ToList();
        //                    List<ProjectUserCategory> pucs = new List<ProjectUserCategory>();

        //                    foreach (ProjectCategory pc in pcs)
        //                    {
        //                        //List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.Is_Authorize == 1 && x.ProjectCategory_Id == pc.Id).ToList();
        //                        List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.ProjectCategory_Id == pc.Id).ToList();

        //                        foreach (var item2 in mypuc)
        //                        {
        //                            pucs.Add(item2);
        //                        }

        //                    }

        //                    //ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).FirstOrDefault();
        //                    ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).LastOrDefault();

        //                    //LaborCategory l = new UserBL().getLaborCategoryList().Where(x => x.Name == item.ProjectUserCategory.ProjectCategory.LaborCategory.Name).FirstOrDefault();
        //                    //EntryTime al = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory.Name == it.EmployeeName).FirstOrDefault();

        //                    string pucc = "";
        //                    double pcost = 0.0;

        //                    if (puc == null)
        //                    {
        //                        pucc = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name;
        //                    }
        //                    else
        //                    {
        //                        pucc = puc.ProjectCategory.LaborCategory.Name;
        //                        if (puc.ProjectCategory.Cost == null || puc.ProjectCategory.Cost == "")
        //                        {
        //                            pcost = 0.0;
        //                        }
        //                        else
        //                        {
        //                            pcost = Convert.ToDouble(puc.ProjectCategory.Cost);
        //                        }
        //                        //pcost = puc.ProjectCategory.Cost;
        //                        //if(pcost == "" || pcost == null)
        //                        //{
        //                        //    pcost = "0";
        //                        //}
        //                    }
        //                    //LaborCategory lb = new UserBL().getLaborCategoryById((int)user.LabourCategoryId);
        //                    User usr = new UserBL().getUserById(it.EmployeeId);
        //                    if (usr.Is_Authorize == 2)
        //                    {
        //                        usr.FirstName = "Inactive";
        //                    }
        //                    else
        //                    {
        //                        usr.FirstName = "";
        //                    }
        //                    ProjectReportDTO obj = new ProjectReportDTO()
        //                    {
        //                        EmployeeName = it.EmployeeName.ToString(),
        //                        Status = usr.FirstName,
        //                        ProjectName = p.Code,
        //                        Hours = it.Hours,
        //                        // LCAT=lb.Name
        //                        //  LCAT = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name,

        //                        LCAT = pucc,
        //                        Cost = Convert.ToDouble(pcost),
        //                        //LCAT = al.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
        //                    };
        //                    plist.Add(obj);
        //                }

        //            }

        //            //  List<EntryTime> finallist = new List<EntryTime>();
        //            //foreach()

        //            ViewBag.ProjectReportList = plist;

        //        }
        //        else
        //        {
        //            if (startdate != "")
        //            {
        //                DateTime d1 = DateTime.Parse(startdate);
        //                allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

        //            }
        //            if (enddate != "")
        //            {
        //                DateTime d2 = DateTime.Parse(enddate);
        //                allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
        //            }
        //            if (Project_Id != -1)
        //            {
        //                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id)).OrderByDescending(x => x.Date).ToList();

        //            }

        //            //new update
        //            //var a = allentries.GroupBy(x => new { x.ProjectUserCategory.User.FirstName }).Select(x => x.FirstOrDefault());

        //            //ViewBag.allEntries1 = a;
        //            //ViewBag.allEntries = allentries;
        //            //new update


        //            var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
        //            List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
        //            foreach (EntryTime item in ab)
        //            {
        //                List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User.Is_Authorize != 0 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
        //                //allentries = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
        //                //var ll = allentries2.GroupBy(c => c.ProjectUserCategory.User_Id).Where(x=>x.Key == item.ProjectUserCategory.User_Id).Select(p => new { EmployeeId = p.Key, EmployeeName = new UserBL().getUserById(p.Key).FirstName + new UserBL().getUserById(p.Key).LastName, Hours = p.Sum(q => q.Hour) });
        //                var ll = allentries2.GroupBy(c => c.ProjectUserCategory.User_Id).Select(p => new { EmployeeId = p.Key, EmployeeName = new UserBL().getUserById(p.Key).FirstName + " " + new UserBL().getUserById(p.Key).LastName, Hours = p.Sum(q => q.Hour) });
        //                foreach (var it in ll)
        //                {
        //                    Project p = new UserBL().getProjectById(item.ProjectUserCategory.ProjectCategory.Project_Id);
        //                    List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.Is_Authorize == 1).ToList();
        //                    List<ProjectUserCategory> pucs = new List<ProjectUserCategory>();

        //                    foreach (ProjectCategory pc in pcs)
        //                    {
        //                        //List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.Is_Authorize == 1 && x.ProjectCategory_Id == pc.Id).ToList();
        //                        List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.ProjectCategory_Id == pc.Id).ToList();

        //                        foreach (var item2 in mypuc)
        //                        {
        //                            pucs.Add(item2);
        //                        }

        //                    }

        //                    //ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).FirstOrDefault();
        //                    ProjectUserCategory puc = pucs.Where(x => x.User_Id == it.EmployeeId).LastOrDefault();
        //                    string pucc = "";
        //                    double pcost = 0.0;

        //                    if (puc == null)
        //                    {
        //                        pucc = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name;
        //                    }
        //                    else
        //                    {
        //                        pucc = puc.ProjectCategory.LaborCategory.Name;
        //                        if (puc.ProjectCategory.Cost == null || puc.ProjectCategory.Cost == "")
        //                        {
        //                            pcost = 0.0;
        //                        }
        //                        else
        //                        {
        //                            pcost = Convert.ToDouble(puc.ProjectCategory.Cost);
        //                        }

        //                    }
        //                    User usr = new UserBL().getUserById(it.EmployeeId);
        //                    if (usr.Is_Authorize == 2)
        //                    {
        //                        usr.FirstName = "Inactive";
        //                    }
        //                    else
        //                    {
        //                        usr.FirstName = "";
        //                    }
        //                    ProjectReportDTO obj = new ProjectReportDTO()
        //                    {
        //                        EmployeeName = it.EmployeeName.ToString(),
        //                        Status = usr.FirstName,

        //                        //EmployeeName = item.ProjectUserCategory.User.FirstName,
        //                        ProjectName = p.Code,
        //                        Hours = it.Hours,
        //                        //Hours = allentries.Where(x=>x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User_Id == item.ProjectUserCategory.User_Id).Sum(x=>x.Hour),

        //                        LCAT = pucc,
        //                        Cost = Convert.ToDouble(pcost),


        //                    };
        //                    plist.Add(obj);
        //                }

        //            }

        //            //  List<EntryTime> finallist = new List<EntryTime>();
        //            //foreach()

        //            ViewBag.ProjectReportList = plist;

        //            //old work
        //            //ViewBag.allEntries = allentries;

        //        }

        //        ViewBag.SD = startdate;
        //        ViewBag.ED = enddate;
        //        ViewBag.PId = Project_Id;

        //        return View();

        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        public ActionResult ProjectSearchReport(string startdate = "", string enddate = "", string Project_Id = "")
        {
            return RedirectToAction("ProjectReport", "Home", new { check = 1, startdate = startdate, enddate = enddate, Project_Id = Project_Id });
        }

       

        public ActionResult EmployeeHoursSearchReport(string startdate = "", string enddate = "")
        {
            return RedirectToAction("EmployeeHoursReport", "Home", new { check = 1, startdate = startdate, enddate = enddate });
        }

        public ActionResult ApprovedTimeSheet(string msg = "", int check = -1, string startdate = "", string enddate = "", string Employee_Id = "", string Project_Id = "", string LaborCategory_Id = "")
        {
            if (sessiondto.getRole() == 2)
            {
                if (check == -1)
                {
                    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Role == 0).OrderByDescending(x => x.Date).ToList();
                    List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 0).ToList();
                    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                    List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().ToList();

                    ViewBag.Message = msg;
                    ViewBag.userscount = allusers.Count();
                    ViewBag.allEntries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
                    ViewBag.projects = allprojects.OrderBy(x => x.Code);
                    ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                    ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                    ViewBag.laborcategoriescount = laborcategories.Count();
                }
                else
                {
                    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Role == 0).OrderByDescending(x => x.Date).ToList();
                    List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
                    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                    List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();



                    if (startdate != "")
                    {
                        DateTime d1 = DateTime.Parse(startdate);
                        allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

                    }
                    if (enddate != "")
                    {
                        DateTime d2 = DateTime.Parse(enddate);
                        allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Employee_Id != "")
                    {
                        allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(Employee_Id)).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Project_Id != "")
                    {
                        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id)).OrderByDescending(x => x.Date).ToList();

                    }
                    if (LaborCategory_Id != "")
                    {
                        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategory_Id)).OrderByDescending(x => x.Date).ToList();
                    }

                    ViewBag.sdate = startdate;
                    ViewBag.edate = enddate;
                    ViewBag.eid = Employee_Id;
                    ViewBag.pid = Project_Id;
                    ViewBag.lcatid = LaborCategory_Id;

                    ViewBag.Message = msg;
                    ViewBag.userscount = allusers.Count();
                    ViewBag.allEntries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
                    ViewBag.projects = allprojects.OrderBy(x => x.Code);
                    ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                    ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                    ViewBag.laborcategoriescount = laborcategories.Count();
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult ApprovedTimeSheetSearch(string startdate = "", string enddate = "", string Employee_Id = "", string Project_Id = "", string LaborCategory_Id = "")
        {
            return RedirectToAction("ApprovedTimeSheet", "Home", new { check = 1, startdate = startdate, enddate = enddate, Employee_Id = Employee_Id, Project_Id = Project_Id, LaborCategory_Id = LaborCategory_Id });
        }

        public ActionResult DeleteApprovedTimeSheet(int id = -1)
        {
            if (sessiondto.getName() != null)
            {
                EntryTime entryTime = new UserBL().getEntryTimeById(id);

                EntryTime e = new EntryTime()
                {
                    Id = entryTime.Id,
                    ProjectUserCategory_Id = entryTime.ProjectUserCategory_Id,
                    Date = entryTime.Date,
                    Type = entryTime.Type,
                    Hour = entryTime.Hour,
                    Is_Approved = entryTime.Is_Approved,
                    Is_Authorize = 0,
                    Created_At = entryTime.Created_At,
                    EntryTime_Id = entryTime.EntryTime_Id,
                    RejectReason = entryTime.RejectReason,
                    DeleteReason = entryTime.DeleteReason
                };

                new UserBL().UpdateEntryTime(e);

                return RedirectToAction("ApprovedTimeSheet", "Home", new { msg = "Entry Deleted Successful!" });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult MonthlyProjectHours(int check = -1, string month = "")
        {
            if (sessiondto.getRole() == 2)
            {
                List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Role == 0).OrderByDescending(x => x.Date).ToList();
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
                //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved ==1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1 && x.ProjectUserCategory.User.Role==1).ToList();
                List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
                List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

                
                ViewBag.userscount = allusers.Count();
                ViewBag.projects = allprojects.OrderBy(x => x.Code);
                ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                ViewBag.laborcategoriescount = laborcategories.Count();


                if (check == -1)
                {
                    ////new update
                    //var a = allentries.GroupBy(x => new { x.ProjectUserCategory.User.FirstName }).Select(x => x.FirstOrDefault());

                    //ViewBag.allEntries1 = a;
                    //ViewBag.allEntries = allentries.OrderByDescending(x => x.Date);
                    ////new update
                    


                    DateTime currentDate = DateTime.Now;
                    DateTime currentMonthStartDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    DateTime currentMonthEndDate = currentMonthStartDate.AddMonths(1).AddDays(-1);

                    #region comment
                    //string tempDate = Convert.ToString(dt.Year) + "-0" + Convert.ToString(dt.Month) ;

                    //string startdate = tempDate + "-1";
                    //string enddate = "";


                    //int m = Convert.ToInt32(tempDate.Substring(5));

                    //switch (m)
                    //{
                    //    case 1:
                    //        enddate = tempDate + "-31";
                    //        break;
                    //    case 2:
                    //        enddate = tempDate + "-28";
                    //        break;
                    //    case 3:
                    //        enddate = tempDate + "-31";
                    //        break;
                    //    case 4:
                    //        enddate = tempDate + "-30";
                    //        break;
                    //    case 5:
                    //        enddate = tempDate + "-31";
                    //        break;
                    //    case 6:
                    //        enddate = tempDate + "-30";
                    //        break;
                    //    case 7:
                    //        enddate = tempDate + "-31";
                    //        break;
                    //    case 8:
                    //        enddate = tempDate + "-31";
                    //        break;
                    //    case 9:
                    //        enddate = tempDate + "-30";
                    //        break;
                    //    case 10:
                    //        enddate = tempDate + "-31";
                    //        break;
                    //    case 11:
                    //        enddate = tempDate + "-30";
                    //        break;
                    //    case 12:
                    //        enddate = tempDate + "-31";
                    //        break;
                    //}
                    #endregion

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


                    var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
                    List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
                    foreach (EntryTime item in ab)
                    {
                        //List<EntryTime> allentries2 = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1  && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id).ToList();

                        List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
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
                            //LaborCategory l = new UserBL().getLaborCategoryList().Where(x => x.Name == item.ProjectUserCategory.ProjectCategory.LaborCategory.Name).FirstOrDefault();
                            //EntryTime al = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory.Name == it.EmployeeName).FirstOrDefault();
                            string pucc = "";

                            if (puc == null)
                            {
                                pucc = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name;
                            }
                            else
                            {
                                pucc = puc.ProjectCategory.LaborCategory.Name;
                            }
                            //LaborCategory lb = new UserBL().getLaborCategoryById((int)user.LabourCategoryId);
                            ProjectReportDTO obj = new ProjectReportDTO()
                            {
                                EmployeeName = it.EmployeeName.ToString(),
                                ProjectName = p.Code,
                                Hours = it.Hours,
                                // LCAT=lb.Name
                                //  LCAT = item.ProjectUserCategory.ProjectCategory.LaborCategory.Name,

                                LCAT = pucc,
                                //LCAT = al.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
                            };
                            plist.Add(obj);
                        }

                    }

                    //  List<EntryTime> finallist = new List<EntryTime>();
                    //foreach()

                    ViewBag.ProjectReportList = plist;
                    ViewBag.CurrentDate = currentDate.ToString("MMM-yyyy");


                }
                else
                {
                    
                    DateTime startdate = Convert.ToDateTime(month + "-1");
                    DateTime enddate = startdate.AddMonths(1).AddDays(-1);

                    


                    //int m = Convert.ToInt32(month.Substring(5));
                    //switch(m)
                    //{
                    //    case 1:
                    //        enddate = month + "-31";
                    //        break;
                    //    case 2:
                    //        enddate = month + "-28";
                    //        break;
                    //    case 3:
                    //        enddate = month + "-31";
                    //        break;
                    //    case 4:
                    //        enddate = month + "-30";
                    //        break;
                    //    case 5:
                    //        enddate = month + "-31";
                    //        break;
                    //    case 6:
                    //        enddate = month + "-30";
                    //        break;
                    //    case 7:
                    //        enddate = month + "-31";
                    //        break;
                    //    case 8:
                    //        enddate = month + "-31";
                    //        break;
                    //    case 9:
                    //        enddate = month + "-30";
                    //        break;
                    //    case 10:
                    //        enddate = month + "-31";
                    //        break;
                    //    case 11:
                    //        enddate = month + "-30";
                    //        break;
                    //    case 12:
                    //        enddate = month + "-31";
                    //        break;
                    //}

                    if (startdate != null)
                    {
                        //DateTime d1 = DateTime.Parse(startdate);
                        allentries = allentries.Where(x => x.Date >= startdate.Date).OrderByDescending(x => x.Date).ToList();

                    }
                    if (enddate != null)
                    {
                        //DateTime d2 = DateTime.Parse(enddate);
                        allentries = allentries.Where(x => x.Date <= enddate.Date).OrderByDescending(x => x.Date).ToList();
                    }
                    

                    


                    var ab = allentries.GroupBy(x => new { x.ProjectUserCategory.ProjectCategory.Project_Id }).Select(x => x.FirstOrDefault());
                    List<ProjectReportDTO> plist = new List<ProjectReportDTO>();
                    foreach (EntryTime item in ab)
                    {
                        List<EntryTime> allentries2 = allentries.Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
                        
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
                                //EmployeeName = item.ProjectUserCategory.User.FirstName,
                                ProjectName = p.Code,
                                Hours = it.Hours,
                                //Hours = allentries.Where(x=>x.ProjectUserCategory.ProjectCategory.Project_Id == item.ProjectUserCategory.ProjectCategory.Project_Id && x.ProjectUserCategory.User_Id == item.ProjectUserCategory.User_Id).Sum(x=>x.Hour),

                                LCAT = pucc,


                            };
                            plist.Add(obj);
                        }

                    }

                    

                    ViewBag.ProjectReportList = plist;
                    ViewBag.chk = 1;
                    ViewBag.sdate = startdate;
                    ViewBag.edate = enddate;
                    ViewBag.month = month;
                    DateTime temp = Convert.ToDateTime(month);
                    ViewBag.CurrentDate = temp.ToString("MMM-yyyy");



                }

                

                return View();

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SearchMonthlyProjectHours(string month="")
        {
            return RedirectToAction("MonthlyProjectHours", "Home", new { check =1, month = month });
        }

        public ActionResult PendingTimeCard(int check = -1, string startdate = "", string enddate = "", string Employee_Id = "")
        {
            if (sessiondto.getRole() == 2)
            {
                if (check == -1)
                {
                    //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 0 && x.ProjectUserCategory.User.Role == 1).OrderByDescending(x => x.Date).ToList();
                    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 0).OrderByDescending(x => x.Date).ToList();
                    List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
                    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                    List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

                    ViewBag.userscount = allusers.Count();
                    ViewBag.allEntries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
                    ViewBag.projects = allprojects.OrderBy(x => x.Code);
                    ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                    ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                    ViewBag.laborcategoriescount = laborcategories.Count();
                }
                else
                {
                    //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 0 && x.ProjectUserCategory.User.Role == 1).OrderByDescending(x => x.Date).ToList();
                    List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 0).OrderByDescending(x => x.Date).ToList();
                    List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
                    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                    List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();


                    if (startdate != "")
                    {
                        DateTime d1 = DateTime.Parse(startdate);
                        allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

                    }
                    if (enddate != "")
                    {
                        DateTime d2 = DateTime.Parse(enddate);
                        allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
                    }
                    if (Employee_Id != "")
                    {
                        allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(Employee_Id)).OrderByDescending(x => x.Date).ToList();
                    }

                    ViewBag.sdate = startdate;
                    ViewBag.edate = enddate;
                    ViewBag.eid = Employee_Id;

                    ViewBag.userscount = allusers.Count();
                    ViewBag.allEntries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
                    ViewBag.projects = allprojects.OrderBy(x => x.Code);
                    ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                    ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                    ViewBag.laborcategoriescount = laborcategories.Count();

                }
                return View();

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SearchPendingTimeCard(string startdate = "", string enddate = "", string Employee_Id = "")
        {
            return RedirectToAction("PendingTimeCard", "Home", new { check = 1, startdate = startdate, enddate = enddate, Employee_Id = Employee_Id });
        }

        public ActionResult ProjectBudgetStatus(string Code = "")
        {
            if (sessiondto.getRole() == 2)
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
                        Remaining = remaining.ToString()
                    };

                    budgetDTOlist.Add(obj2);
                }


                ViewBag.ProjectBudgetList = budgetDTOlist;
                ViewBag.code = Code;

                //#region OldCode
                //List<Project> plist = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

                //if (Code != "")
                //{
                //    plist = plist.Where(x => x.Code.ToLower().Contains(Code.ToLower())).ToList();
                //}

                //List<ProjectBudgetDTO> budgetDTOlist = new List<ProjectBudgetDTO>();
                //double totalcost = 0;
                //double totalhours = 0;
                //double remaining = 0;
                //foreach (Project p in plist)
                //{
                //    totalcost = new UserBL().getProjectCategoryList().Where(x => x.Is_Authorize == 1 && x.Project_Id == p.Id).Sum(y => Convert.ToDouble(y.Cost));

                //    totalhours = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.ProjectCategory.Project.Id == p.Id).Sum(y => Convert.ToDouble(y.Hour));

                //    totalcost = totalcost * totalhours;

                //    if (p.Budget == null || p.Budget == "")
                //    {
                //        p.Budget = "0";
                //    }
                //    remaining = (Convert.ToDouble(p.Budget) - totalcost) / Convert.ToDouble(p.Budget);

                //    if (remaining < -1)
                //    {
                //        remaining = 0;
                //    }
                //    else
                //    {
                //        remaining = remaining * 100;

                //        remaining = Math.Round(remaining, 2);
                //    }
                //    ProjectBudgetDTO obj = new ProjectBudgetDTO()
                //    {
                //        Code = p.Code,
                //        Budget = p.Budget,
                //        TotalCost = totalcost.ToString(),
                //        Remaining = remaining.ToString(),
                //    };

                //    budgetDTOlist.Add(obj);
                //}

                //budgetDTOlist = budgetDTOlist.OrderBy(x => x.Code).ToList();

                //ViewBag.ProjectBudget = budgetDTOlist;
                //ViewBag.code = Code;
                //#endregion

                return View();
            }
            else if (sessiondto.getRole() == 1)
            {
                List<int> PIdlist = new UserProjectReportBL().GetActiveUserProjectReports().Where(x => x.UserId == sessiondto.getId()).Select(x => Convert.ToInt32(x.ProjectId)).ToList();

                if(PIdlist.Count == 0)
                {
                    return RedirectToAction("Dashboard", "Home", new { msg ="No project is assigned"});
                }

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
                        Id = p.Id,
                        Code = p.Code,
                        Budget = p.Budget,
                        TotalCost = totalcost.ToString(),
                        Remaining = remaining.ToString()
                    };

                    budgetDTOlist.Add(obj2);
                }


                ViewBag.ProjectBudgetList = budgetDTOlist;
                ViewBag.ProjectId = PIdlist;

                ViewBag.code = Code;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SearchProjectBudgetStatus(string Code = "")
        {
            return RedirectToAction("ProjectBudgetStatus", "Home", new { Code = Code});
        }

        public ActionResult DistributionReport(string Startdate = "", string Enddate = "", string Employee_Id = "")
        {
            if (sessiondto.getRole() == 2)
            {
                List<User> user = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 0).ToList();

                ViewBag.userscount = user.Count();
                ViewBag.allusers = user.OrderBy(x => x.FirstName);

                List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Role == 0).ToList();
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();

                if (Startdate != "")
                {
                    DateTime d1 = DateTime.Parse(Startdate);
                    allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

                }
                if (Enddate != "")
                {
                    DateTime d2 = DateTime.Parse(Enddate);
                    allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
                }
                
                if (Employee_Id != "")
                {
                    user = user.Where(x => x.Id == Convert.ToInt32(Employee_Id)).ToList();
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

                ViewBag.dlist = dlist.OrderByDescending(x => x.VacationHour);
                
                //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Role == 0).ToList();
                //allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).ToList();
                //List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();

                

                ViewBag.sdate = Startdate;
                ViewBag.edate = Enddate;
                ViewBag.eid = Employee_Id;

                

                //ViewBag.vacationCount = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Code == "Vacation").Sum(x=>x.Hour);
                //ViewBag.opheadCount = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Code == "Operations - Overhead").Sum(x => x.Hour);
                //ViewBag.otherCount = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project.Code != "Operations - Overhead" && x.ProjectUserCategory.ProjectCategory.Project.Code != "Vacation").Sum(x => x.Hour);

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SearchDistributionReport(string Startdate = "", string Enddate = "", string Employee_Id = "")
        {
            return RedirectToAction("DistributionReport", "Home", new { Startdate = Startdate, Enddate = Enddate, Employee_Id = Employee_Id });
        }

        [HttpGet]
        public ActionResult GetTimeSheetDataTable()
        {
            List<EntryTime> enteries = null;

            if (sessiondto.getRole() == 1)
            {
                enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id != -1).ToList();

            }
            else if (sessiondto.getRole() == 2)
            {
                enteries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).ToList();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            int totalrows = enteries.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                enteries = enteries.Where(x => x.Date.ToString().ToLower().Contains(searchValue.ToLower()) ||
                x.ProjectUserCategory.User.LastName.ToLower().Contains(searchValue.ToLower()) ||
                x.ProjectUserCategory.User.FirstName.ToLower().Contains(searchValue.ToLower()) ||
                x.ProjectUserCategory.ProjectCategory.LaborCategory.Name.ToLower().Contains(searchValue.ToLower()) ||
                x.Hour.ToString().ToLower().Contains(searchValue.ToLower()) ||
                x.ProjectUserCategory.ProjectCategory.Project.Code.ToLower().Contains(searchValue.ToLower())).ToList();
            }

            int totalrowsafterfilterinig = enteries.Count();

            enteries = enteries.Skip(start).Take(length).ToList();

            List<enteriesDTO> enteriesDTOList = new List<enteriesDTO>();
            double totalHours = 0;

            foreach (EntryTime x in enteries)
            {

                enteriesDTO enteriesDTO = new enteriesDTO()
                {
                    Id = x.Id,
                    Date = x.Date.ToString("MM/dd/yyyy"),
                    Name = x.ProjectUserCategory.User.FirstName + " " + x.ProjectUserCategory.User.LastName,
                    LCAT = x.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
                    Hour = x.Hour,
                    Project = x.ProjectUserCategory.ProjectCategory.Project.Code

                };

                if (x.Is_Approved == 1)
                {
                    enteriesDTO.Status = "Accepted";
                }
                else if (x.Is_Approved == 2)
                {
                    enteriesDTO.Status = "<a onclick='ReasonEntry(" + x.RejectReason + ")' data-toggle='modal' data-target='#reasonModal'><strong>Reason</strong></a>";
                }
                else
                {
                    enteriesDTO.Status = "Pending";
                }

                enteriesDTO.Project = enteriesDTO.Project + " / " + enteriesDTO.Status;

                totalHours = totalHours + x.Hour;

                enteriesDTOList.Add(enteriesDTO);
            }

            enteriesDTOList[0].TotalHours = Math.Round(totalHours,2);

            return Json(new { data = enteriesDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult AssignedEmployeesTimeSheet()
        //{
        //    if (sessiondto.getRole() == 1)
        //    {
        //        List<EntryTime> allentries = new List<EntryTime>();
        //        List<Project> allprojects = new List<Project>();
        //        List<ProjectCategory> myprojcats = new List<ProjectCategory>();
        //        List<ProjectUserCategory> myprojusers = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
        //        List<LaborCategory> mylaborcategories = new List<LaborCategory>();

        //        if (myprojusers != null)
        //        {
        //            foreach (ProjectUserCategory pu in myprojusers)
        //            {
        //                ProjectCategory p = new UserBL().getProjectCategoryById(pu.ProjectCategory_Id);
        //                allprojects.Add(p.Project);
        //                mylaborcategories.Add(p.LaborCategory);
        //            }
        //        }

        //        List<User> allusers = new UserBL().getUserList().Where(x => x.User_Id == sessiondto.getId()).ToList();

        //        foreach (User u in allusers)
        //        {
        //            List<ProjectUserCategory> userentries = u.ProjectUserCategories.Where(x => x.User_Id == u.Id).ToList();

        //            foreach (ProjectUserCategory p in userentries)
        //            {
        //                List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == p.Id).ToList();
        //                foreach (EntryTime e in enteries)
        //                {
        //                    allentries.Add(e);
        //                }

        //            }
        //        }

        //        List<EntryTime> enteries2 = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id != -1).ToList();   

        //        ViewBag.userscount = allusers.Count();
        //        ViewBag.allEntries = allentries.OrderByDescending(x => x.Date);
        //        ViewBag.allEntries2 = enteries2.OrderByDescending(x => x.Date);
        //        ViewBag.projects = allprojects.OrderBy(x => x.Code);
        //        ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
        //        ViewBag.laborcategories = mylaborcategories.OrderBy(x => x.Name);
        //        ViewBag.laborcategoriescount = mylaborcategories.Count();

        //        return View();
        //    }
        //    else if (sessiondto.getRole() == 2)
        //    {
        //        List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).ToList();
        //        List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
        //        List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
        //        List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

        //        ViewBag.userscount = allusers.Count();
        //        ViewBag.allEntries = allentries.OrderByDescending(x => x.Date);
        //        ViewBag.projects = allprojects.OrderBy(x => x.Code);
        //        ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
        //        ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
        //        ViewBag.laborcategoriescount = laborcategories.Count();

        //        return View();

        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
        
        [HttpGet]
        public string AdminEmployeesReport(string startdate="", string enddate="", string Project_Id = "", string Employee_Id = "", string LaborCategory_Id = "", string Status = "")
        {
            //List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            
            List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1 && x.ProjectUserCategory.User.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();
            List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();

            if (startdate != "")
            {
                DateTime d1 = DateTime.Parse(startdate);
                allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

            }
            if (enddate != "")
            {
                DateTime d2 = DateTime.Parse(enddate);
                allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
            }
            if (Project_Id != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id)).OrderByDescending(x => x.Date).ToList();

            }
            if (Employee_Id != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(Employee_Id)).OrderByDescending(x => x.Date).ToList();
            }
            if (LaborCategory_Id != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategory_Id)).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Pending")
            {
                allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Accepted")
            {
                allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Rejected")
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

            return JsonConvert.SerializeObject(entriesdtos, Formatting.Indented,
                   new JsonSerializerSettings()
                   {
                       ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                   });

        }

        [HttpGet]
        public string LaborCategoriesSearch(string projectId)
        {
            if (projectId == "")
            {
                List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

                return JsonConvert.SerializeObject(laborcategories, Formatting.Indented,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
            }
            else
            {
                List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Is_Authorize == 1 && x.Project_Id == Convert.ToInt32(projectId)).ToList();
                List<LaborCategory> laborcategories = new List<LaborCategory>();

                foreach (ProjectCategory pc in pcs)
                {
                    laborcategories.Add(pc.LaborCategory);
                }


                return JsonConvert.SerializeObject(laborcategories, Formatting.Indented,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
            }
        }
        #endregion
        
        #region Vacation Bank
        public ActionResult EditAnnualVacationTime(string Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                int id = Convert.ToInt32(StringCipher.Base64Decode(Id));
                VacationBank vb = new UserBL().getVacationBankById(id);
                ViewBag.vb = vb;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpdateAnnualVacationTime(VacationBank v)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                VacationBank va = new UserBL().getVacationBankById(v.Id);

                VacationBank vacation = new VacationBank()
                {
                    Id = v.Id,
                    VacationHoursPerYear = v.VacationHoursPerYear,
                    AccuralVacationHours = va.AccuralVacationHours,
                    Is_Authorize = va.Is_Authorize,
                    User_Id = va.User_Id,
                    PayperiodCount = va.PayperiodCount,
                    Created_Date = va.Created_Date
                };
                new UserBL().UpdateVacationBank(vacation);

                return RedirectToAction("EditVacation", "Home", new { Id = StringCipher.Base64Encode(v.Id.ToString()), msg = "Vacation hours updated successfully." });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ListVacationBank(string msg = "")
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<VacationBank> vacationbanklist = new UserBL().getVacationBankList().OrderBy(x => x.User.Role).ToList();

                ViewBag.msg = msg;
                ViewBag.vacationbanklist = vacationbanklist;
                ViewBag.vacationbanklistcount = vacationbanklist.Count();

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult GetVacationBanksDataTable()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<VacationBank> vacationbanklist = new UserBL().getVacationBankList().Where(x=>x.User.Is_Authorize == 1).OrderBy(x => x.User.Role).ToList();

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = vacationbanklist.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    vacationbanklist = vacationbanklist.Where(x => x.User.FirstName.ToLower().Contains(searchValue.ToLower())
                    || x.User.LastName.ToLower().Contains(searchValue.ToLower())
                    || x.VacationHoursPerYear.ToString().ToLower().Contains(searchValue.ToLower())
                    /*||
                    x.LastName.ToLower().Contains(searchValue.ToLower()) ||
                    x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                    x.PhoneMail.ToLower().Contains(searchValue.ToLower()) ||
                    x.Email.ToLower().Contains(searchValue.ToLower()) ||
                    x.Password.Contains(searchValue.ToLower())*/).ToList();
                }

                int totalrowsafterfilterinig = vacationbanklist.Count();

                vacationbanklist = vacationbanklist.Skip(start).Take(length).ToList();

                List<bankDTO> bankDTOList = new List<bankDTO>();

                foreach (VacationBank x in vacationbanklist)
                {
                    User user = new UserBL().getUserById(x.User_Id);
                    bankDTO bankDTO = new bankDTO()
                    {
                        Id = x.Id,
                        Name = user.FirstName + " "+ user.LastName,
                        VacationHours = "<span>"+ x.VacationHoursPerYear + "<a class='green' href='../Home/EditAnnualVacationTime?Id=" + StringCipher.Base64Encode(x.Id.ToString()) + "'> <i class='ace-icon fa fa-pencil bigger-160'></i></a></span>",
                    };

                    if(x.AccuralVacationHours == null)
                    {
                        bankDTO.AnvalVacations = "<span>0 <a class='green' href='../Home/ManuallyAddVacationHours?Id=" + StringCipher.Base64Encode(x.Id.ToString()) + "'> <i class='ace-icon fa fa-pencil bigger-160'></i></a></span>";
                    }
                    else
                    {
                        bankDTO.AnvalVacations = "<span>" + x.AccuralVacationHours.Value.ToString("0.00") + " " + "<a class='green' href='../Home/ManuallyAddVacationHours?Id=" + StringCipher.Base64Encode(x.Id.ToString()) + "'> <i class='ace-icon fa fa-pencil bigger-160'></i></a></span>";
                    }

                    bankDTOList.Add(bankDTO);
                }


                return Json(new { data = bankDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ManuallyAddVacationHours(string Id, string msg = "")
        {
            int id = Convert.ToInt32(StringCipher.Base64Decode(Id));
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {

                ViewBag.msg = msg;
                VacationBank vb = new UserBL().getVacationBankById(id);
                ViewBag.accural = vb.AccuralVacationHours;
                ViewBag.User = vb.User.FirstName + " " + vb.User.LastName;
                ViewBag.UserId = vb.User_Id;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult PostManuallyAddVacationHours(string User_Id, string MHours, string msg = "")
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                VacationBank vb = new UserBL().getVacationBankList().Where(x => x.User_Id == Convert.ToInt32(User_Id)).FirstOrDefault();
                if (MHours[0] == '-')
                {
                    vb.AccuralVacationHours = vb.AccuralVacationHours - Convert.ToDouble(MHours.Split('-')[1]);
                }
                else if (MHours[0] == '+')
                {
                    vb.AccuralVacationHours = vb.AccuralVacationHours + Convert.ToDouble(MHours.Split('+')[1]);
                }
                else
                {
                    vb.AccuralVacationHours = vb.AccuralVacationHours + Convert.ToDouble(MHours);
                }

                VacationBank vacationBank = new VacationBank()
                {
                    Id = vb.Id,
                    VacationHoursPerYear = vb.VacationHoursPerYear,
                    AccuralVacationHours = vb.AccuralVacationHours,
                    PayperiodCount = vb.PayperiodCount,
                    Is_Authorize = vb.Is_Authorize,
                    User_Id = vb.User_Id,
                    Created_Date = vb.Created_Date
                };
                new UserBL().UpdateVacationBank(vacationBank);
                EarnedVacationTime eVT = new EarnedVacationTime()
                {
                    Date = DateTime.Now,
                    Hours = MHours,
                    Type = "manual adjustment",
                    User_Id = Convert.ToInt32(User_Id)
                };

                new UserBL().AddEarnedVacationTime(eVT);
                return RedirectToAction("ManuallyAddVacationHours", "Home", new { Id = StringCipher.Base64Encode(vacationBank.Id.ToString()), msg = "Accrued hours assigned to the user." });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddVacationBank(string msg = "")
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<User> userlist = new UserBL().getUserList().Where(x => (x.Role == 0 || x.Role == 1) && x.Is_Authorize == 1 && x.VacationBanks.FirstOrDefault() == null).OrderBy(x => x.Role).ToList();

                ViewBag.msg = msg;
                ViewBag.userlist = userlist;
                ViewBag.userlistcount = userlist.Count();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult PostVacationBank(VacationBank v, string msg = "")
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                v.Is_Authorize = 1;
                v.PayperiodCount = 0;
                v.AccuralVacationHours = 0;
                v.Created_Date = DateTime.Now;
                new UserBL().AddVacationBank(v);

                return RedirectToAction("AddVacationBank", "Home", new { msg = "Vacation hours assigned to the user." });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditVacation(string Id, string msg = "")
        {
            Id = StringCipher.Base64Decode(Id);
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                VacationBank vacation = new UserBL().getVacationBankById(Convert.ToInt32(Id));
                List<User> userlist = new UserBL().getUserList().Where(x => x.Role == 0 || x.Role == 1).OrderBy(x => x.Role).ToList();

                ViewBag.msg = msg;
                ViewBag.userlist = userlist;
                ViewBag.vacation = vacation;
                ViewBag.userlistcount = userlist.Count();

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        // Manager's Employees List
        public ActionResult EmployeesVacationBank(string User_Id)
        {
            User_Id = StringCipher.Base64Decode(User_Id);
            if (sessiondto.getName() != null && sessiondto.getRole() == 1)
            {
                List<User> assignedemployees = new UserBL().getUserList().Where(x => x.User_Id == Convert.ToInt32(User_Id) && x.Is_Authorize == 1).ToList();
                ViewBag.AssignedEmployees = assignedemployees.OrderBy(x => x.FirstName);
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult UpdateVacationBank(VacationBank v)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                VacationBank va = new UserBL().getVacationBankById(v.Id);

                VacationBank vacation = new VacationBank()
                {
                    Id = v.Id,
                    VacationHoursPerYear = v.VacationHoursPerYear,
                    AccuralVacationHours = va.AccuralVacationHours,
                    Is_Authorize = va.Is_Authorize,
                    User_Id = va.User_Id,
                    PayperiodCount = va.PayperiodCount,
                    Created_Date = va.Created_Date
                    //AvailableVacationHours = v.VacationHoursPerYear + v.AccuralVacationHoursPerYear
                };
                new UserBL().UpdateVacationBank(vacation);

                return RedirectToAction("EditVacation", "Home", new { Id = StringCipher.Base64Encode(v.Id.ToString()), msg = "Vacation hours updated successfully." });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DeleteVacationBank(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                new UserBL().DeleteVacationBank(Id);

                return RedirectToAction("ListVacationBank", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult VacationBank(string User_Id)
        {
            User_Id = StringCipher.Base64Decode(User_Id);
            //Employee
            if (sessiondto.getName() != null && (sessiondto.getRole() == 0 || sessiondto.getRole() == 1))
            {
                VacationBank myvacationbank = new UserBL().getVacationBankList().FirstOrDefault(x => x.User_Id == Convert.ToInt32(User_Id));
                User user = new UserBL().getUserById(Convert.ToInt32(User_Id));
                ViewBag.TableTitle = user.FirstName + " " + user.LastName;                

                List<EarnedVacationTime> EarnedVacationTime = new UserBL().getEarnedVacationTimeList().Where(x => x.User_Id == Convert.ToInt32(User_Id)).ToList();
                double TotalAccuralHours = 0;
                foreach (var eVT in EarnedVacationTime)
                {
                    TotalAccuralHours = TotalAccuralHours + Convert.ToDouble(eVT.Hours);
                }

                ViewBag.TotalAccuralHours = TotalAccuralHours;

                double TotalHoursUsed = 0.0;
                List<EntryTime> vocationEntries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == Convert.ToInt32(User_Id) && x.Date.Date <= DateTime.Now && x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.ProjectCategory.Project.Code == "Vacation").OrderByDescending(x => x.Date).ToList();

                foreach (var item in vocationEntries)
                {
                    TotalHoursUsed = TotalHoursUsed + item.Hour;
                }
                ViewBag.UsedVacationTimeList = vocationEntries;
                ViewBag.TotalHoursUsed = TotalHoursUsed;

                if (myvacationbank != null)
                {
                    ViewBag.VacationHours = myvacationbank.VacationHoursPerYear;
                    ViewBag.AccuralVacationHours =  TotalAccuralHours - TotalHoursUsed;
                }
                else
                {
                    ViewBag.VacationHours = null;
                    ViewBag.AccuralVacationHours = null;
                }

                List<EarnedVacationTime> eVT1 = new UserBL().getEarnedVacationTimeList().Where(x => x.User_Id == Convert.ToInt32(User_Id)).ToList();
                ViewBag.EVTList = eVT1;
                ViewBag.EVTListcount = eVT1.Count();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        public ActionResult TimeCardMail(string Id, string err = "")
        {
            Id = StringCipher.Base64Decode(Id);
            if (sessiondto.getName() != null && sessiondto.getRole() == 1)
            {
                EntryTime e = new UserBL().getEntryTimeById(Convert.ToInt32(Id));

                ViewBag.e = e;
                ViewBag.message = err;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ToggleTimeCard(int Id, string reason, int option)
        {
            //if (sessiondto.getName() != null)
            //{
                try
                {

                    if (Id == 0 && option == 100)
                    {
                        List<User> myemployees = new UserBL().getUserList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();

                        foreach (User u in myemployees)
                        {
                            List<EntryTime> userpendingentries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == u.Id && x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                            foreach (EntryTime e in userpendingentries)
                            {
                                EntryTime et = new EntryTime()
                                {
                                    Id = e.Id,
                                    ProjectUserCategory_Id = e.ProjectUserCategory_Id,
                                    Date = e.Date,
                                    Hour = e.Hour,
                                    Type = "Work",
                                    Is_Approved = 1,
                                    Is_Authorize = 1,
                                    RejectReason = reason,
                                    DeleteReason = e.DeleteReason,
                                    EntryTime_Id = e.EntryTime_Id,
                                    Created_At = DateTime.Now
                                };
                                new UserBL().UpdateEntryTime(et);


                                //Subtracting the accural hours if project is vacation
                                if (e.ProjectUserCategory.ProjectCategory.Project.Code == "Vacation")
                                {
                                    VacationBank temp = new UserBL().getVacationBankList().Where(x => x.User_Id == e.ProjectUserCategory.User_Id).FirstOrDefault();
                                    VacationBank vb = new VacationBank()
                                    {
                                        Id = temp.Id,
                                        VacationHoursPerYear = temp.VacationHoursPerYear,
                                        AccuralVacationHours = temp.AccuralVacationHours - e.Hour,
                                        PayperiodCount = temp.PayperiodCount,
                                        Is_Authorize = temp.Is_Authorize,
                                        User_Id = temp.User_Id,
                                        Created_Date = temp.Created_Date
                                    };

                                    new UserBL().UpdateVacationBank(vb);

                                }

                            }
                        }

                    }
                    else
                    {
                        EntryTime e = new UserBL().getEntryTimeById(Id);
                        User manager = new UserBL().getUserById(Convert.ToInt32(e.ProjectUserCategory.User.User_Id));

                        EntryTime et = new EntryTime()
                        {
                            Id = e.Id,
                            ProjectUserCategory_Id = e.ProjectUserCategory_Id,
                            Date = e.Date,
                            Hour = e.Hour,
                            Type = "Work",
                            Is_Approved = option,
                            Is_Authorize = 1,
                            RejectReason = reason,
                            DeleteReason = e.DeleteReason,
                            EntryTime_Id = e.EntryTime_Id,
                            Created_At = DateTime.Now
                        };
                        new UserBL().UpdateEntryTime(et);

                        if (et.Is_Approved == 1)
                        {
                            //Subtracting the accural hours if project is vacation
                            if (e.ProjectUserCategory.ProjectCategory.Project.Code == "Vacation")
                            {
                                VacationBank temp = new UserBL().getVacationBankList().Where(x => x.User_Id == e.ProjectUserCategory.User_Id).FirstOrDefault();
                                VacationBank vb = new VacationBank()
                                {
                                    Id = temp.Id,
                                    VacationHoursPerYear = temp.VacationHoursPerYear,
                                    AccuralVacationHours = temp.AccuralVacationHours - e.Hour,
                                    PayperiodCount = temp.PayperiodCount,
                                    Is_Authorize = temp.Is_Authorize,
                                    User_Id = temp.User_Id,
                                    Created_Date = temp.Created_Date
                                };
                                new UserBL().UpdateVacationBank(vb);
                            }
                        }
                        if (et.Is_Approved == 2)
                        {
                            MailMessage msg = new MailMessage();

                            string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
                            text += "  font-family: 'Bree Serif', serif; }";
                            text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
                            text += " .line {       margin-bottom: 20px; }";

                            msg.From = new MailAddress("timesheets@strattechnologies.com");
                            msg.To.Add(e.ProjectUserCategory.User.Email);
                            msg.Subject = "Rejection of Time Card Entry";
                            msg.IsBodyHtml = true;
                            string temp = "";

                            if (reason != null)
                            {
                                temp = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'></div> </nav><center><div><p>Name: " + e.ProjectUserCategory.User.FirstName + " " + e.ProjectUserCategory.User.LastName + "</p><br /><p>Project : " + e.ProjectUserCategory.ProjectCategory.Project.Code + "</p><br /><p>Date: " + e.Date.ToString("MM/dd/yyyy") + "</p><br /><p>Hours: " + e.Hour + "</p><br /><h1 class='text-center'>Reason!</h1><h3 class='text-center'> " + reason + " </h3><br></div></center>";
                            }

                            temp += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                            msg.Body = temp;
                            using (SmtpClient client = new SmtpClient())
                            {
                                client.EnableSsl = false;
                                client.UseDefaultCredentials = false;
                                client.Credentials = new NetworkCredential("timesheets@strattechnologies.com", "T1imeS4h33ts!3");
                                client.Host = "mail.strattechnologies.com";
                                client.Port = 587;
                                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                client.Send(msg);
                            }
                        }
                    }
                    return RedirectToAction("PendingTimeCards", "Home");
                }
                catch (Exception e)
                {
                    //return RedirectToAction("PendingTimeCards", "Home", new { msg = "Somethings' Wrong. Please try again"});
                    return RedirectToAction("PendingTimeCards", "Home", new { msg = "Failure Sending Mail"});
                }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}
        }

        public ActionResult DeleteEntry(string Id)
        {
            if (sessiondto.getName() != null)
            {
                EntryTime e = new UserBL().getEntryTimeById(Convert.ToInt32(Id));

                new UserBL().DeleteEntry(Convert.ToInt32(Id));

                return RedirectToAction("TimeSheetEntry", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult timezone()
        {
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            if (easternTime.DayOfWeek.ToString() == "Sunday")
            {
                EarnedVacationTime lastentry = new UserBL().getEarnedVacationTimeList().LastOrDefault();


                if (lastentry == null || lastentry.Date.Value.ToString("MM/dd/yyyy") != DateTime.Now.ToString("MM/dd/yyyy"))
                {
                    List<VacationBank> vblist = new UserBL().getVacationBankList();
                    foreach (VacationBank vacationBank in vblist)
                    {

                        VacationBank vb = new VacationBank()
                        {
                            Id = vacationBank.Id,
                            VacationHoursPerYear = vacationBank.VacationHoursPerYear,
                            AccuralVacationHours = vacationBank.AccuralVacationHours + (vacationBank.VacationHoursPerYear / 52),
                            PayperiodCount = vacationBank.PayperiodCount,
                            Is_Authorize = vacationBank.Is_Authorize,
                            User_Id = vacationBank.User_Id,
                            Created_Date = vacationBank.Created_Date,
                        };

                        new UserBL().UpdateVacationBank(vb);
                        EarnedVacationTime eVT = new EarnedVacationTime()
                        {
                            Date = timeUtc,
                            Hours = (vacationBank.VacationHoursPerYear / 52).ToString(),
                            User_Id = vacationBank.User_Id,
                            Type = "automatic accrual"
                        };
                        new UserBL().AddEarnedVacationTime(eVT);

                    }
                    ViewBag.msg = "Database updated successfully for current week!";
                    return View();
                }
                ViewBag.msg = "Database had been updated for current week. You can not update it again for this week!";
                return View();
            }
            else
            {
                ViewBag.msg = "Database had been updated for current week. You can not update it again for this week!";
                return View();
            }

            //var timeUtc = DateTime.UtcNow;
            //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            //DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            //List<User> users = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role != 2).ToList();

            //foreach (User u in users)
            //{
            //    EntryTime e = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == u.Id).LastOrDefault();

            //    if (e == null)
            //    {
            //        if (easternTime.Hour == 17 && DateTime.Today.Minute <= 30 && !(DateTime.Now.DayOfWeek.ToString() == "Saturday" || DateTime.Now.DayOfWeek.ToString() == "Sunday"))
            //        {
            //            string employeeEmail = u.Email;
            //            string employeename = u.FirstName + ' ' + u.LastName;
            //            ManagerEmailSend(employeeEmail, employeename);
            //        }
            //    }
            //    else
            //    {
            //        int empyear = e.Created_At.Year;
            //        int empmonth = e.Created_At.Month;
            //        int empday = e.Created_At.Day;
            //        int emphour = e.Created_At.Hour;
            //        //today time is greater than 4:00 and day is not saturday and sunday
            //        if ((empyear == DateTime.Now.Year && empmonth == DateTime.Now.Month && empday == DateTime.Now.Day) && !(DateTime.Now.DayOfWeek.ToString() == "Saturday" || DateTime.Now.DayOfWeek.ToString() == "Sunday"))
            //        {
            //            if (easternTime.Hour == 17 && DateTime.Today.Minute <= 30)
            //            {
            //                string employeeEmail = u.Email;
            //                string employeename = u.FirstName + ' ' + u.LastName;
            //                ManagerEmailSend(employeeEmail, employeename);
            //            }
            //        }
            //    }
            //}

        }

        public bool ManagerEmailSend(string employeeEmail, string employeename)
        {
            try
            {
                MailMessage msg = new MailMessage();

                string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
                text += "  font-family: 'Bree Serif', serif; }";
                text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
                text += " .line {       margin-bottom: 20px; }";

                msg.From = new MailAddress("timesheets@strattechnologies.com");
                msg.To.Add(employeeEmail);
                msg.Subject = "Time Card Entry";
                msg.IsBodyHtml = true;
                string temp = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'></div> </nav><center><div><h1 class='text-center'>Reminder!</h1><h3 class='text-center'><strong>" + employeename + "</strong> please enter your hours and submit your timecard ASAP.</h3><br></div></center>";

                temp += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";

                msg.Body = temp;

                using (SmtpClient client = new SmtpClient())
                {
                    client.EnableSsl = false;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("timesheets@strattechnologies.com", "T1imeS4h33ts!3");
                    client.Host = "mail.strattechnologies.com";
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    client.Send(msg);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public ActionResult PendingTimeCards(string msg="")
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 1)
            {
                List<User> myemployees = new UserBL().getUserList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
                List<EntryTime> pendingentries = new List<EntryTime>();

                foreach (User u in myemployees)
                {
                    List<EntryTime> userpendingentries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == u.Id && x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                    foreach (EntryTime e in userpendingentries)
                    {
                        pendingentries.Add(e);
                    }
                }

                ViewBag.Message = msg;
                ViewBag.pendingentries = pendingentries;
                ViewBag.pendingentriescount = pendingentries.Count();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult MyReport(DateTime startdate, DateTime enddate, string Project_Id, string submitButton, string LaborCategory_Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() != 2)
            {
                DateTime StartDate = DateTime.Today.AddDays(-14);
                DateTime EndDate = DateTime.Today;


                DateTime d1 = startdate;
                DateTime d2 = enddate;

                List<Project> myprojects = new List<Project>();
                List<ProjectUserCategory> myproj = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
                List<LaborCategory> laborcategories = new List<LaborCategory>();

                foreach (ProjectUserCategory p in myproj)
                {
                    myprojects.Add(p.ProjectCategory.Project);
                    laborcategories.Add(p.ProjectCategory.LaborCategory);
                }

                List<EntryTime> myentries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.Date.Date >= d1 && x.Date.Date <= d2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();

                if (submitButton == "startbackward")
                {
                    d1 = d1.Date.AddDays(-1);

                    if (StartDate.Date < d1.Date)
                    {
                        myentries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.Date.Date >= d1 && x.Date.Date <= d2 && x.Is_Approved != 2 && x.ProjectUserCategory.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    else
                    {
                        return RedirectToAction("TimeSheetEntry", "Home");
                    }
                }
                else if (submitButton == "startforward")
                {
                    d1 = d1.Date.AddDays(+1);

                    if (d1.Date > EndDate.Date)
                    {
                        return RedirectToAction("TimeSheetEntry", "Home");
                    }
                    else
                    {
                        myentries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.Date.Date >= d1 && x.Date.Date <= d2 && x.Is_Approved != 2 && x.ProjectUserCategory.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                }
                else if (submitButton == "endbackward")
                {
                    d2 = d2.Date.AddDays(-1);

                    if (StartDate.Date < d2.Date)
                    {
                        myentries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.Date.Date >= d1 && x.Date.Date <= d2 && x.Is_Approved != 2 && x.ProjectUserCategory.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                    else
                    {
                        return RedirectToAction("TimeSheetEntry", "Home");

                    }
                }
                else if (submitButton == "endforward")
                {
                    d2 = d2.Date.AddDays(+1);

                    if (d2.Date > EndDate.Date)
                    {
                        return RedirectToAction("TimeSheetEntry", "Home");
                    }
                    else
                    {
                        myentries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.Date.Date >= d1 && x.Date.Date <= d2 && x.Is_Approved != 2 && x.ProjectUserCategory.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                    }
                }

                if (Project_Id != "")
                {
                    myentries = myentries.Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id) && x.Is_Authorize == 1).ToList();
                }

                if (LaborCategory_Id != "")
                {
                    myentries = myentries.Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId() && x.ProjectUserCategory.ProjectCategory.LaborCategory_Id == Convert.ToInt32(LaborCategory_Id) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
                }

                List<double> workhours = new List<double>();
                double totalhours = 0;


                DateTime startingdate = new UserBL().getStartDateList().Where(x => x.IsActive == 1).FirstOrDefault().StartingDate;
                DateTime date1 = getStartingDate(startingdate);
                DateTime date2 = date1.AddDays(+13);


                ViewBag.date1 = date1.ToLongDateString();
                ViewBag.date2 = date2.ToLongDateString();
                ViewBag.laborcategoriescount = laborcategories.Count();
                ViewBag.laborcategories = laborcategories;
                ViewBag.myEntries = myentries;
                ViewBag.workhours = workhours;
                ViewBag.totalhours = totalhours;
                ViewBag.projects = myprojects;
                ViewBag.startdate = d1;
                ViewBag.enddate = d2;

                return View("TimeSheetEntry");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EmployeeReport()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 0)
            {
                List<EntryTime> allentries = new List<EntryTime>();
                List<Project> allprojects = new List<Project>();
                List<ProjectCategory> myprojcats = new List<ProjectCategory>();
                List<ProjectUserCategory> myprojusers = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
                List<LaborCategory> mylaborcategories = new List<LaborCategory>();
                if (myprojusers != null)
                {
                    foreach (ProjectUserCategory pu in myprojusers)
                    {
                        ProjectCategory p = new UserBL().getProjectCategoryById(pu.ProjectCategory_Id);
                        allprojects.Add(p.Project);
                        mylaborcategories.Add(p.LaborCategory);
                    }
                }
                User user = new UserBL().getUserById(sessiondto.getId());
                List<ProjectUserCategory> userentries = user.ProjectUserCategories.Where(x => x.User_Id == user.Id).ToList();
                foreach (ProjectUserCategory p in userentries)
                {
                    List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == p.Id).ToList();
                    foreach (EntryTime e in enteries)
                    {
                        allentries.Add(e);
                    }
                }
                ViewBag.allEntries = allentries.OrderByDescending(x => x.Date);
                ViewBag.projects = allprojects.OrderBy(x => x.Code);
                ViewBag.user = user;
                ViewBag.laborcategories = mylaborcategories;
                ViewBag.laborcategoriescount = mylaborcategories.Count();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public string EmployeesReports(string startdate, string enddate, string Project_Id, string Status)
        {
            User user = new UserBL().getUserList().Where(x => x.Id == sessiondto.getId() && x.Is_Authorize == 1).FirstOrDefault();
            List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
            List<EntryTime> allentries = new List<EntryTime>();
            //foreach (User u in allusers)
            //{
            List<ProjectUserCategory> userentries = user.ProjectUserCategories.Where(x => x.User_Id == user.Id).ToList();
            foreach (ProjectUserCategory p in userentries)
            {
                List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == p.Id).ToList();
                foreach (EntryTime e in enteries)
                {
                    allentries.Add(e);
                }
            }
            //}
            if (startdate != "")
            {
                DateTime d1 = DateTime.Parse(startdate);
                allentries = allentries.Where(x => x.Date >= d1.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (enddate != "")
            {
                DateTime d2 = DateTime.Parse(enddate);
                allentries = allentries.Where(x => x.Date <= d2.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Project_Id != "")
            {
                allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Pending")
            {
                allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Accepted")
            {
                allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
            }
            if (Status == "Rejected")
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

            return JsonConvert.SerializeObject(entriesdtos, Formatting.Indented,
                   new JsonSerializerSettings()
                   {
                       ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                   });
        }

        //public ActionResult EmployeeReport() //Arqum
        //{
        //    if (sessiondto.getName() != null && sessiondto.getRole() == 0)
        //    {
        //        List<EntryTime> allentries = new List<EntryTime>();
        //        List<Project> allprojects = new List<Project>();
        //        List<ProjectCategory> myprojcats = new List<ProjectCategory>();
        //        List<ProjectUserCategory> myprojusers = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
        //        List<LaborCategory> mylaborcategories = new List<LaborCategory>();

        //        if (myprojusers != null)
        //        {
        //            foreach (ProjectUserCategory pu in myprojusers)
        //            {
        //                ProjectCategory p = new UserBL().getProjectCategoryById(pu.ProjectCategory_Id);
        //                allprojects.Add(p.Project);
        //                mylaborcategories.Add(p.LaborCategory);
        //            }
        //        }

        //        User user = new UserBL().getUserById(sessiondto.getId());
        //        List<ProjectUserCategory> userentries = user.ProjectUserCategories.Where(x => x.User_Id == user.Id).ToList();

        //        foreach (ProjectUserCategory p in userentries)
        //        {
        //            List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == p.Id).ToList();
        //            foreach (EntryTime e in enteries)
        //            {
        //                allentries.Add(e);
        //            }
        //        }
        //        ViewBag.allEntries = allentries.OrderByDescending(x => x.Date);
        //        ViewBag.projects = allprojects.OrderBy(x => x.Code);
        //        ViewBag.user = user;
        //        ViewBag.laborcategories = mylaborcategories;
        //        ViewBag.laborcategoriescount = mylaborcategories.Count();

        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //[HttpGet]
        //public ActionResult EmployeesReports(string startdate, string enddate, string Project_Id, string Status) //Arqum
        //{
        //    User user = new UserBL().getUserList().Where(x => x.Id == sessiondto.getId() && x.Is_Authorize == 1).FirstOrDefault();
        //    List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
        //    List<EntryTime> allentries = new List<EntryTime>();
        //    List<ProjectUserCategory> userentries = user.ProjectUserCategories.Where(x => x.User_Id == user.Id).ToList();

        //    foreach (ProjectUserCategory p in userentries)
        //    {
        //        List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory_Id == p.Id).ToList();
        //        foreach (EntryTime e in enteries)
        //        {
        //            allentries.Add(e);
        //        }
        //    }

        //    if (startdate != "" && startdate != null)
        //    {
        //        DateTime d1 = DateTime.Parse(startdate);
        //        allentries = allentries.Where(x => x.Date >= d1.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (enddate != "" && enddate != null)
        //    {
        //        DateTime d2 = DateTime.Parse(enddate);
        //        allentries = allentries.Where(x => x.Date <= d2.Date && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (Project_Id != "" && Project_Id != null)
        //    {
        //        allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.Project_Id == Convert.ToInt32(Project_Id) && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (Status == "Pending")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 0 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (Status == "Accepted")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 1 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }
        //    if (Status == "Rejected")
        //    {
        //        allentries = allentries.Where(x => x.Is_Approved == 2 && x.Is_Authorize == 1).OrderByDescending(x => x.Date).ToList();
        //    }

        //    int start = Convert.ToInt32(Request["start"]);
        //    int length = Convert.ToInt32(Request["length"]);
        //    string searchValue = Request["search[value]"];
        //    string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
        //    string sortDirection = Request["order[0][dir]"];
        //    if (sortColumnName != "" && sortColumnName != null)
        //    {
        //        if (sortColumnName != "0")
        //        {
        //            if (sortDirection == "asc")
        //            {
        //                allentries = allentries.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
        //            }
        //            else
        //            {
        //                allentries = allentries.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
        //            }
        //        }
        //    }
        //    int totalrows = allentries.Count();
        //    //filter
        //    //if (searchValue != "")
        //    //{
        //    //    allentries = allentries.Where(x => x.ProjectUserCategory.ProjectCategory.LaborCategory.Name.Contains(searchValue) || x.ProjectUserCategory.User.FirstName.Contains(searchValue) || x.ProjectUserCategory.User.LastName.ToLower().Contains(searchValue.ToLower())).ToList();
        //    //    //allentries = allentries.Where(x => x.Date.Equals(searchValue) || x.Hour.Equals(searchValue) || x.ProjectUserCategory.ProjectCategory.LaborCategory.Name.ToLower().Contains(searchValue.ToLower()) ||x.ProjectUserCategory.ProjectCategory.Project.Code.ToLower().Contains(searchValue.ToLower()) ||x.ProjectUserCategory.User.FirstName.ToLower().Contains(searchValue.ToLower()) || x.ProjectUserCategory.User.LastName.ToLower().Contains(searchValue.ToLower())).ToList();
        //    //}

        //    int totalrowsafterfilterinig = allentries.Count();
        //    // pagination
        //    allentries = allentries.Skip(start).Take(length).ToList();

        //    List<EntriesSearchDTO> entriesdtos = new List<EntriesSearchDTO>();
        //    foreach (EntryTime e in allentries)
        //    {
        //        EntriesSearchDTO edto = new EntriesSearchDTO()
        //        {
        //            Id = e.Id,
        //            Name = e.ProjectUserCategory.User.FirstName + ' ' + e.ProjectUserCategory.User.LastName,
        //            Hours = e.Hour,
        //            Date = e.Date.ToString("MM/dd/yyyy"),
        //            LCAT = e.ProjectUserCategory.ProjectCategory.LaborCategory.Name,
        //            Project = e.ProjectUserCategory.ProjectCategory.Project.Code,
        //            RejectReason = e.RejectReason
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
        //    return Json(new { data = entriesdtos, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        //}

        public VacationBank checkVacations(int user_Id)
        {
            VacationBank myvacationbank = new UserBL().getVacationBankList().Where(x => x.User_Id == user_Id).FirstOrDefault();

            //user vacations
            if (myvacationbank == null) return null;
            DateTime sdate = (DateTime)myvacationbank.Created_Date;
            TimeSpan diffResult = DateTime.Now - sdate;
            //Change this line
            double Vacationsperpayperiod = (myvacationbank.VacationHoursPerYear + Convert.ToDouble(myvacationbank.AccuralVacationHours)) / 52;
            int payperiodsCount = (diffResult.Days + 1) / 7;
            if (payperiodsCount > myvacationbank.PayperiodCount)
            {
                VacationBank va = new VacationBank()
                {
                    Id = myvacationbank.Id,
                    VacationHoursPerYear = myvacationbank.VacationHoursPerYear,
                    AccuralVacationHours = myvacationbank.AccuralVacationHours + Vacationsperpayperiod,
                    PayperiodCount = myvacationbank.PayperiodCount + 1,
                    Is_Authorize = myvacationbank.Is_Authorize,
                    User_Id = myvacationbank.User_Id,
                    Created_Date = myvacationbank.Created_Date
                };
                new UserBL().UpdateVacationBank(va);

                return va;
            }
            return myvacationbank;
        }

        public ActionResult EmployeeHoursReport(int check = -1, string startdate = "", string enddate = "")
        {
            if (sessiondto.getRole() == 1)
            {
                List<Project> allprojects = new List<Project>();
                List<ProjectCategory> myprojcats = new List<ProjectCategory>();
                List<ProjectUserCategory> myprojusers = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == sessiondto.getId() && x.Is_Authorize == 1).ToList();
                List<LaborCategory> mylaborcategories = new List<LaborCategory>();

                if (myprojusers != null)
                {
                    foreach (ProjectUserCategory pu in myprojusers)
                    {
                        ProjectCategory p = new UserBL().getProjectCategoryById(pu.ProjectCategory_Id);
                        allprojects.Add(p.Project);
                        mylaborcategories.Add(p.LaborCategory);
                    }
                }

                List<User> allusers = new UserBL().getUserList().Where(x => x.User_Id == sessiondto.getId() && x.IsChecked != "1099").ToList();
                List<EntryTime> enteries = new List<EntryTime>();
                foreach (User u in allusers)
                {
                    List<EntryTime> enteries2 = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == u.Id).ToList();
                    enteries2 = new UserBL().getEntryTimeList().Where(x => x.Is_Approved == 1).ToList();

                    enteries.AddRange(enteries2);
                }




                //  List<EntryTime> enteries = new UserBL().getEntryTimeList().Where(x => x.ProjectUserCategory.User_Id == sessiondto.getId()).ToList();

                ViewBag.userscount = allusers.Count();
                ViewBag.allEntries = enteries.Where(x => x.ProjectUserCategory.User.Is_Authorize == 1).OrderByDescending(x => x.Date);
                ViewBag.projects = allprojects.OrderBy(x => x.Code);
                ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                ViewBag.laborcategories = mylaborcategories.OrderBy(x => x.Name);
                ViewBag.laborcategoriescount = mylaborcategories.Count();

                return View();
            }
            else if (sessiondto.getRole() == 2)
            {
                List<EntryTime> allentries = new UserBL().getEntryTimeList().Where(x => x.Is_Authorize == 1 && x.Is_Approved == 1 && x.ProjectUserCategory.User.Is_Authorize == 1 && x.ProjectUserCategory.ProjectCategory.Project.Is_Authorize == 1).ToList();
                List<User> allusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.IsChecked != "1099").ToList();
                List<Project> allprojects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).ToList();
                List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();

                ViewBag.userscount = allusers.Count();
                ViewBag.projects = allprojects.OrderBy(x => x.Code);
                ViewBag.allusers = allusers.OrderBy(x => x.FirstName);
                ViewBag.laborcategories = laborcategories.OrderBy(x => x.Name);
                ViewBag.laborcategoriescount = laborcategories.Count();

                if (check != 1)
                {
                    //new update
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

                    ViewBag.allEntries = entriesdtos;
                    ViewBag.allEntries1 = entriesdtos1;
                    //new update

                    //old work
                    //ViewBag.allEntries = allentries.Where(x=> x.Is_Approved == 1).OrderByDescending(x => x.Date);
                }
                else
                {
                    if (startdate != "")
                    {
                        DateTime d1 = DateTime.Parse(startdate);
                        allentries = allentries.Where(x => x.Date >= d1.Date).OrderByDescending(x => x.Date).ToList();

                    }
                    if (enddate != "")
                    {
                        DateTime d2 = DateTime.Parse(enddate);
                        allentries = allentries.Where(x => x.Date <= d2.Date).OrderByDescending(x => x.Date).ToList();
                    }



                    //new update
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


                        entriesdtos.Add(edto);
                    }


                    var a = entriesdtos.GroupBy(x => new { x.Name }).Select(x => x.FirstOrDefault());
                    ViewBag.test = a;
                    List<EntriesSearchDTO> entriesdtos1 = new List<EntriesSearchDTO>();
                    foreach (var e in ViewBag.test)
                    {
                        EntriesSearchDTO edto1 = new EntriesSearchDTO()
                        {
                            Name = e.Name,
                        };
                        entriesdtos1.Add(edto1);
                    }

                    ViewBag.allEntries = entriesdtos;
                    ViewBag.allEntries1 = entriesdtos1;
                    //new update
                    //old work
                    //ViewBag.allEntries = allentries.Where(x=> x.Is_Approved == 1);
                }
                ViewBag.SD = startdate;
                ViewBag.ED = enddate;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}