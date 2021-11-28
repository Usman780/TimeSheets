using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeCard.BL;
using TimeCard.Helping_Classes;
using TimeCard.Models;

namespace TimeCard.Controllers
{
    public class UserController : Controller
    {
        SessionDTO sessiondto = new SessionDTO();

        #region User 

        [HttpGet]
        public ActionResult GetEmployeesDataTable()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<User> users = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 0).OrderBy(x => x.FirstName).ToList();

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = users.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    users = users.Where(x => x.FirstName.ToLower().Contains(searchValue.ToLower()) ||
                    x.LastName.ToLower().Contains(searchValue.ToLower()) ||
                    x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                    //x.PhoneMail.ToLower().Contains(searchValue.ToLower()) ||
                    x.Email.ToLower().Contains(searchValue.ToLower()) ||
                    x.Password.Contains(searchValue.ToLower())).ToList();
                }

                int totalrowsafterfilterinig = users.Count();

                users = users.Skip(start).Take(length).ToList();

                List<userDTO> userDTOList = new List<userDTO>();

                string str = "";

                foreach (User x in users)
                {
                    if(x.User_Id == null)
                    {
                        str = "Not Assigned";
                    }
                    else
                    {
                        User manager = new UserBL().getUserById(Convert.ToInt32(x.User_Id));
                        str = manager.FirstName + " " + manager.LastName;
                    }
                    userDTO userDTO = new userDTO()
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        Password = x.Password,
                        Phone = x.Phone,
                        PhoneMail = x.PhoneMail,
                        IsChecked = x.IsChecked,
                        Manager = str
                    };

                    userDTOList.Add(userDTO);
                    str = "";
                }


                return Json(new { data = userDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult GetManagersDataTable()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<User> users = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role == 1).OrderBy(x => x.FirstName).ToList();

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = users.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    users = users.Where(x => x.FirstName.ToLower().Contains(searchValue.ToLower()) ||
                    x.LastName.ToLower().Contains(searchValue.ToLower()) ||
                    x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                    //x.PhoneMail.ToLower().Contains(searchValue.ToLower()) ||
                    x.Email.ToLower().Contains(searchValue.ToLower()) ||
                    x.Password.Contains(searchValue.ToLower())).ToList();
                }

                int totalrowsafterfilterinig = users.Count();

                users = users.Skip(start).Take(length).ToList();

                List<userDTO> userDTOList = new List<userDTO>();

                string str = "";

                foreach (User x in users)
                {
                    if (x.User_Id == null)
                    {
                        str = "Not Assigned";
                    }
                    else
                    {
                        User manager = new UserBL().getUserById(Convert.ToInt32(x.User_Id));
                        str = manager.FirstName + " " + manager.LastName;
                    }
                    userDTO userDTO = new userDTO()
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        Password = x.Password,
                        Phone = x.Phone,
                        PhoneMail = x.PhoneMail,
                        Manager = str
                    };

                    userDTOList.Add(userDTO);
                    str = "";
                }


                return Json(new { data = userDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetInactiveEmployeesDataTable()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<User> users = new UserBL().getUserList().Where(x => x.Is_Authorize == 2 && x.Role == 0).OrderBy(x => x.FirstName).ToList();

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = users.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    users = users.Where(x => x.FirstName.ToLower().Contains(searchValue.ToLower()) ||
                    x.LastName.ToLower().Contains(searchValue.ToLower()) ||
                    x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                    //x.PhoneMail.ToLower().Contains(searchValue.ToLower()) ||
                    x.Email.ToLower().Contains(searchValue.ToLower()) ||
                    x.Password.Contains(searchValue.ToLower())).ToList();
                }

                int totalrowsafterfilterinig = users.Count();

                users = users.Skip(start).Take(length).ToList();

                List<userDTO> userDTOList = new List<userDTO>();

                string str = "";

                foreach (User x in users)
                {
                    if (x.User_Id == null)
                    {
                        str = "Not Assigned";
                    }
                    else
                    {
                        User manager = new UserBL().getUserById(Convert.ToInt32(x.User_Id));
                        str = manager.FirstName + " " + manager.LastName;
                    }
                    userDTO userDTO = new userDTO()
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        Password = x.Password,
                        Phone = x.Phone,
                        PhoneMail = x.PhoneMail,
                        Manager = str
                    };

                    userDTOList.Add(userDTO);
                    str = "";
                }


                return Json(new { data = userDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult GetInactiveManagersDataTable()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<User> users = new UserBL().getUserList().Where(x => x.Is_Authorize == 2 && x.Role == 1).OrderBy(x => x.FirstName).ToList();

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = users.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    users = users.Where(x => x.FirstName.ToLower().Contains(searchValue.ToLower()) ||
                    x.LastName.ToLower().Contains(searchValue.ToLower()) ||
                    x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                    //x.PhoneMail.ToLower().Contains(searchValue.ToLower()) ||
                    x.Email.ToLower().Contains(searchValue.ToLower()) ||
                    x.Password.Contains(searchValue.ToLower())).ToList();
                }

                int totalrowsafterfilterinig = users.Count();

                users = users.Skip(start).Take(length).ToList();

                List<userDTO> userDTOList = new List<userDTO>();

                string str = "";

                foreach (User x in users)
                {
                    if (x.User_Id == null)
                    {
                        str = "Not Assigned";
                    }
                    else
                    {
                        User manager = new UserBL().getUserById(Convert.ToInt32(x.User_Id));
                        str = manager.FirstName + " " + manager.LastName;
                    }
                    userDTO userDTO = new userDTO()
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        Password = x.Password,
                        Phone = x.Phone,
                        PhoneMail = x.PhoneMail,
                        Manager = str
                    };

                    userDTOList.Add(userDTO);
                    str = "";
                }


                return Json(new { data = userDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ListUsers()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<User> users = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role != 2).OrderBy(x => x.FirstName).ToList();

                return View(users);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddUser()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<User> managers = new UserBL().getUserList().Where(x => x.Role == 1).ToList();
                List<Carrier> carriers = new UserBL().getCarrierList().Where(x => x.Is_Authorize == 1).ToList();

                ViewBag.carriers = carriers;

                return View(managers);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ListInactiveUsers()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<User> users = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role != 2).OrderBy(x => x.FirstName).ToList();

                return View(users);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult SaveUser(User user, string Carrier_Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                if (Carrier_Id != "")
                {
                    Carrier c = new UserBL().getCarrierById(Convert.ToInt32(Carrier_Id));
                    user.PhoneMail = user.Phone + c.Gateway;
                }

                if (user.Role != 0 && user.Role != 1)
                {
                    user.Role = 0;
                }
                user.Is_Authorize = 1;


                new UserBL().AddUser(user);

                return RedirectToAction("ListUsers", "User");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditUser(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                User u = new UserBL().getUserById(Id);
                List<User> managers = new UserBL().getUserList().Where(x => x.Role == 1).ToList();
                List<Carrier> carriers = new UserBL().getCarrierList().Where(x => x.Is_Authorize == 1).ToList();

                if (u.PhoneMail != null)
                {
                    string[] tokens = u.PhoneMail.Split('@');
                    string carrier = '@' + tokens[1];

                    Carrier c = new UserBL().getCarrierList().Where(x => x.Gateway == carrier).FirstOrDefault();

                    ViewBag.selectedcarrier = c;
                }

                ViewBag.carriers = carriers;
                ViewBag.managers = managers;
                ViewBag.employee = u;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpdateUser(User user, string Carrier_Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                if (Carrier_Id != "")
                {
                    Carrier c = new UserBL().getCarrierById(Convert.ToInt32(Carrier_Id));
                    user.PhoneMail = user.Phone + c.Gateway;
                }

                if (user.Role != 0 && user.Role != 1)
                {
                    user.Role = 0;
                }
                user.Is_Authorize = 1;
                new UserBL().UpdateUser(user);

                return RedirectToAction("ListUsers", "User");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ToggleUser(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                User user = new UserBL().getUserById(Id);
                User u = new User()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password,
                    Phone = user.Phone,
                    Role = user.Role,
                    Is_Authorize = 0,
                    User_Id = user.User_Id
                };
                new UserBL().UpdateUser(u);

                return RedirectToAction("ListUsers", "User");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DeactivateUser(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                User user = new UserBL().getUserById(Id);
                User u = new User()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Password = user.Password,
                    Is_Authorize = 2,
                    Role = user.Role,
                    User_Id = user.User_Id,
                    LabourCategoryId = user.LabourCategoryId,
                    PhoneMail = user.PhoneMail,
                    IsChecked = user.IsChecked
                };
                new UserBL().UpdateUser(u);

                return RedirectToAction("ListUsers", "User");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ActivateUser(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                User user = new UserBL().getUserById(Id);
                User u = new User()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Password = user.Password,
                    Is_Authorize = 1,
                    Role = user.Role,
                    User_Id = user.User_Id,
                    LabourCategoryId = user.LabourCategoryId,
                    PhoneMail = user.PhoneMail,
                    IsChecked = user.IsChecked
                };
                new UserBL().UpdateUser(u);

                return RedirectToAction("ListInactiveUsers", "User");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion


        #region Project 

        [HttpPost]
        public ActionResult GetActiveUserProjectsList(int UserId)
        {
            List<Project> plist = new UserBL().getProjectList().Where(x=>x.Is_Authorize == 1).ToList();

            List<UserProjectReportDTO> uprdto = new List<UserProjectReportDTO>();

            foreach (Project p in plist)
            {
                UserProjectReport upreport = new UserProjectReportBL().GetActiveUserProjectReports().Where(x => x.UserId == UserId && x.ProjectId == p.Id).FirstOrDefault();

                UserProjectReportDTO obj = new UserProjectReportDTO()
                {
                    Id = p.Id,
                    Code = p.Code,
                    IsChecked = upreport == null ? 0 : 1
                };

                uprdto.Add(obj);
            }

            return Json(uprdto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostAddUserProjectReport(FormCollection fc, int count, int UserId)
        {
            if(sessiondto.getName() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            for (int i = 0; i <= count; i++)
            {
                string IsChecked = fc["IsChecked" + i];
                string ProjectId = fc["ProjectId" + i];

                if(ProjectId != null)
                {
                    UserProjectReport upr = new UserProjectReportBL().GetUserProjectReportByUserProjectId(UserId, Convert.ToInt32(ProjectId));
                    
                    timecardEntities de = new timecardEntities();
                    if(upr == null)
                    {
                        if (IsChecked != null)
                        {
                            UserProjectReport obj = new UserProjectReport()
                            {
                                UserId = UserId,
                                ProjectId = Convert.ToInt32(ProjectId),
                                IsActive = 1,
                                CreatedAt = DateTime.Now
                            };

                            bool chk = new UserProjectReportBL().AddUserProjectReport(obj, de);
                        }
                    }
                    else
                    {
                        if (IsChecked == null)
                        {
                            bool chk2 = new UserProjectReportBL().DeleteUserProjectReport(upr.Id);
                        }
                    }
                }
            }
                
            return RedirectToAction("ListUsers", "User", new { msg="Manager record updated" });
        }


        [HttpPost]
        public int validateCode(string Code)
        {
            int codeCount = new UserBL().getProjectList().Where(x => x.Code == Code && x.Is_Authorize == 1).Count();
            if (codeCount > 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        [HttpGet]
        public ActionResult GetProjectsDataTable()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<Project> projects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).OrderBy(x => x.Code).ToList();

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = projects.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    projects = projects.Where(x => x.Code.ToLower().Contains(searchValue.ToLower()) /*||
                    x.LastName.ToLower().Contains(searchValue.ToLower()) ||
                    x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                    x.PhoneMail.ToLower().Contains(searchValue.ToLower()) ||
                    x.Email.ToLower().Contains(searchValue.ToLower()) ||
                    x.Password.Contains(searchValue.ToLower())*/).ToList();
                }

                int totalrowsafterfilterinig = projects.Count();

                projects = projects.Skip(start).Take(length).ToList();

                List<projectDTO> projectDTOList = new List<projectDTO>();
                string budget = "";
                foreach (Project x in projects)
                {
                    if(x.Budget == null || x.Budget == "")
                    {
                        budget = "0";
                    }
                    else
                    {
                        budget = x.Budget;
                    }
                    
                    projectDTO projectDTO = new projectDTO()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Budget = budget,
                        LabourCategories = "<a href='../User/AssignLabourCategory?Id=" + StringCipher.Base64Encode(x.Id.ToString()) + "'>Labor Category</a>",
                        Employees = "<a href='../User/AssignProject?Project_Id=" + x.Id + "'>Assign Employee</a>",
                        IsLocked = x.LockDate==null? "0": Convert.ToDateTime(x.LockDate).ToString("MM/dd/yyyy")
                };

                    projectDTOList.Add(projectDTO);
                }


                return Json(new { data = projectDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult GetProjectLockDate()
        {
            Project p = new UserBL().getProjectList().Where(x=>x.Is_Authorize == 1).FirstOrDefault();

            projectDTO obj = new projectDTO()
            {
                Id = p.Id,
                Code = p.Code,
                IsLocked = p.LockDate == null ? null : Convert.ToDateTime(p.LockDate).ToString("yyy-MM-dd")
            };
            
            return Json(obj, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult GetCompletedProjectsDataTable()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<Project> projects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 2).OrderBy(x => x.Code).ToList();

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = projects.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    projects = projects.Where(x => x.Code.ToLower().Contains(searchValue.ToLower()) /*||
                    x.LastName.ToLower().Contains(searchValue.ToLower()) ||
                    x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                    x.PhoneMail.ToLower().Contains(searchValue.ToLower()) ||
                    x.Email.ToLower().Contains(searchValue.ToLower()) ||
                    x.Password.Contains(searchValue.ToLower())*/).ToList();
                }

                int totalrowsafterfilterinig = projects.Count();

                projects = projects.Skip(start).Take(length).ToList();

                List<projectDTO> projectDTOList = new List<projectDTO>();

                foreach (Project x in projects)
                {

                    projectDTO projectDTO = new projectDTO()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        //LabourCategories = "<a href='../User/AssignLabourCategory?Id=" + StringCipher.Base64Encode(x.Id.ToString()) + "'>Labor Category</a>",
                        //Employees = "<a href='../User/AssignProject?Project_Id=" + x.Id + "'>Assign Employee</a>"
                        LabourCategories = "Labor Category",
                        Employees = "Assign Employee"
                    };

                    projectDTOList.Add(projectDTO);
                }


                return Json(new { data = projectDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ListProjects(string msg)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<Project> projects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 1).OrderBy(x => x.Code).ToList();

                ViewBag.projects = projects;
                ViewBag.Message = msg;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ListCompletedProjects()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<Project> projects = new UserBL().getProjectList().Where(x => x.Is_Authorize == 2).OrderBy(x => x.Code).ToList();

                ViewBag.projects = projects;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddProject()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult SaveProject(Project project)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                project.Is_Authorize = 1;
                new UserBL().AddProject(project);

                string Id = StringCipher.Base64Encode(project.Id.ToString());

                return RedirectToAction("AssignLabourCategory", "User", new { Id = Id });

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AssignLabourCategory(string Id, string message)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                Id = StringCipher.Base64Decode(Id);

                Project p = new UserBL().getProjectById(Convert.ToInt32(Id));
                List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x => x.Is_Authorize == 1).ToList();
                List<ProjectCategory> projectlaborcategories = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == p.Id && x.Is_Authorize == 1).ToList();

                ViewBag.message = message;
                ViewBag.p = p;
                ViewBag.laborcategories = laborcategories;
                ViewBag.projectlaborcategories = projectlaborcategories;

                return View();

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult SaveProjectLabourCategory(int Project_Id, int LaborCategory_Id, string Cost)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                int count = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == Project_Id && x.LaborCategory_Id == LaborCategory_Id && x.Is_Authorize == 1).Count();

                if (count >= 1)
                {
                    return RedirectToAction("AssignLabourCategory", "User", new { Id = StringCipher.Base64Encode(Project_Id.ToString()), message = "Labor Category is already assigned to this project." });
                }
                else
                {
                    ProjectCategory p = new ProjectCategory()
                    {
                        Project_Id = Project_Id,
                        LaborCategory_Id = LaborCategory_Id,
                        Cost = Cost,
                        Is_Authorize = 1
                    };
                    new UserBL().AddProjectCategory(p);
                    return RedirectToAction("AssignLabourCategory", "User", new { Id = StringCipher.Base64Encode(Project_Id.ToString()) });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ToggleProjectCategory(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                ProjectCategory pc = new UserBL().getProjectCategoryById(Id);
                string Project_Id = StringCipher.Base64Encode(pc.Project_Id.ToString());

                ProjectCategory p = new ProjectCategory()
                {
                    Id = pc.Id,
                    Project_Id = pc.Project_Id,
                    LaborCategory_Id = pc.LaborCategory_Id,
                    Is_Authorize = 0
                };
                new UserBL().UpdateProjectCategory(p);

                return RedirectToAction("AssignLabourCategory", "User", new { Id = Project_Id });

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult SaveUpdateProjectLabourCategory(int Project_Id, string Name, string Cost)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                LaborCategory l = new LaborCategory()
                {
                    Name = Name,
                    Is_Authorize = 1
                };
                new UserBL().AddLaborCategory(l);

                ProjectCategory p = new ProjectCategory()
                {
                    Project_Id = Project_Id,
                    LaborCategory_Id = l.Id,
                    Cost = Cost,
                    Is_Authorize = 1
                };
                new UserBL().AddProjectCategory(p);
                return RedirectToAction("AssignLabourCategory", "User", new { Id = StringCipher.Base64Encode(Project_Id.ToString()) });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult UpdateProjectCategoryCost(int LCategory_Id, int Project_Id, string Cost)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                ProjectCategory pcat = new UserBL().getProjectCategoryById(LCategory_Id);

                ProjectCategory p = new ProjectCategory()
                {
                    Id = pcat.Id,
                    Project_Id = pcat.Project_Id,
                    LaborCategory_Id = pcat.LaborCategory_Id,
                    Cost = Cost,
                    Is_Authorize = 1
                };
                new UserBL().UpdateProjectCategory(p);

                return RedirectToAction("AssignLabourCategory", "User", new { Id = StringCipher.Base64Encode(Project_Id.ToString()) });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditProject(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                Project u = new UserBL().getProjectById(Id);
                ViewBag.p = u;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpdateProject(Project project)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                Project p = new UserBL().getProjectById(project.Id);
                Project obj = new Project()
                {
                    Id= p.Id,
                    Code = project.Code,
                    Is_Authorize = p.Is_Authorize,
                    Budget = project.Budget,
                    LockDate = p.LockDate
                };
                //project.Is_Authorize = 1;
                
                new UserBL().UpdateProject(obj);

                return RedirectToAction("ListProjects", "User");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpdateProjectLockDate(string LockDate, int IsRemove)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                timecardEntities de = new timecardEntities();

                de.Database.Log = Console.Write;

                using (DbContextTransaction transaction = de.Database.BeginTransaction())
                {
                    try
                    {
                        List<Project> plist = new UserBL().getProjectList(de).Where(x => x.Is_Authorize != 0).ToList();
                        
                        foreach (Project p in plist)
                        {
                            Project obj = new Project()
                            {
                                Id = p.Id,
                                Code = p.Code,
                                Is_Authorize = p.Is_Authorize,
                                Budget = p.Budget,
                                //LockDate = IsRemove == 0 ? Convert.ToDateTime(LockDate).Date : null //this operator is not working here because of some library conflicts
                            };

                            if(IsRemove == 0)
                            {
                                obj.LockDate = Convert.ToDateTime(LockDate).Date;
                            }
                            else
                            {
                                obj.LockDate = null;
                            }

                            bool chk = new UserBL().UpdateProject(obj, de);

                            if (chk == false)
                            {
                                throw new Exception();
                            }
                        }


                        transaction.Commit();

                        if (IsRemove == 0)
                            return RedirectToAction("ListProjects", "User", new { msg = "Projects locked successfully" });
                        else
                            return RedirectToAction("ListProjects", "User", new { msg = "Projects unlocked" });
                    }
                    catch
                    {
                        transaction.Rollback();
                        return RedirectToAction("ListProjects", "User", new { msg = "Somethings' Wrong" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        
        //public ActionResult RemoveProjectLockDate(int Id)
        //{
        //    try
        //    {
        //        if (sessiondto.getName() != null && sessiondto.getRole() == 2)
        //        {
        //            Project p = new UserBL().getProjectById(Id);

        //            Project obj = new Project()
        //            {
        //                Id = p.Id,
        //                Code = p.Code,
        //                Is_Authorize = p.Is_Authorize,
        //                LockDate = null
        //            };

        //            bool chk = new UserBL().UpdateProject(obj);

        //            if (chk == true)
        //            {
        //                return RedirectToAction("ListProjects", "User", new { msg = "Project is unlocked now" });
        //            }
        //            else
        //            {
        //                return RedirectToAction("ListProjects", "User", new { msg = "Somethings' Wrong" });
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    catch
        //    {
        //        return RedirectToAction("ListProjects", "User", new { msg = "Somethings' Wrong" });
        //    }
        //}

        public ActionResult ToggleProject(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                Project project = new UserBL().getProjectById(Id);
                Project p = new Project()
                {
                    Id = project.Id,
                    Code = project.Code,
                    Is_Authorize = 0,
                    Budget = project.Budget,
                    LockDate = project.LockDate
                };
                new UserBL().UpdateProject(p);

                return RedirectToAction("ListProjects", "User");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult MarkProject(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                Project project = new UserBL().getProjectById(Id);
                Project p = new Project()
                {
                    Id = project.Id,
                    Code = project.Code,
                    Is_Authorize = 2,
                    Budget = project.Budget,
                    LockDate = project.LockDate
                };
                new UserBL().UpdateProject(p);

                return RedirectToAction("ListProjects", "User");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult UnmarkProject(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                Project project = new UserBL().getProjectById(Id);
                Project p = new Project()
                {
                    Id = project.Id,
                    Code = project.Code,
                    Is_Authorize = 1,
                    Budget = project.Budget,
                    LockDate = project.LockDate
                };
                new UserBL().UpdateProject(p);

                return RedirectToAction("ListCompletedProjects", "User");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion


        #region LaborCategory
        public ActionResult ListLaborCategories()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                //List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x=>x.UpdatedAt == null).OrderBy(x => x.Name).ToList();

                //ViewBag.laborcategories = laborcategories;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult GetLaborCategoriesDataTable()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<LaborCategory> laborcategories = new UserBL().getLaborCategoryList().Where(x=>x.UpdatedAt == null).OrderBy(x => x.Name).ToList();

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = laborcategories.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    laborcategories = laborcategories.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) /*||
                    x.LastName.ToLower().Contains(searchValue.ToLower()) ||
                    x.Phone.ToLower().Contains(searchValue.ToLower()) ||
                    x.PhoneMail.ToLower().Contains(searchValue.ToLower()) ||
                    x.Email.ToLower().Contains(searchValue.ToLower()) ||
                    x.Password.Contains(searchValue.ToLower())*/).ToList();
                }

                int totalrowsafterfilterinig = laborcategories.Count();

                laborcategories = laborcategories.Skip(start).Take(length).ToList();

                List<laborDTO> laborDTOList = new List<laborDTO>();

                foreach (LaborCategory x in laborcategories)
                {

                    laborDTO laborDTO = new laborDTO()
                    {
                        Id = x.Id,
                        Name = x.Name
                    };

                    laborDTOList.Add(laborDTO);
                }


                return Json(new { data = laborDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult AddLaborCategory()
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult SaveLaborCategory(LaborCategory project)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                project.Is_Authorize = 1;
                new UserBL().AddLaborCategory(project);

                return RedirectToAction("ListLaborCategories", "User");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditLaborCategory(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                LaborCategory u = new UserBL().getLaborCategoryById(Id);
                ViewBag.p = u;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpdateLaborCategory(LaborCategory project)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                timecardEntities de = new timecardEntities();

                LaborCategory lcat = new UserBL().getLaborCategoryById(project.Id, de);
                if (project.Name.ToLower() == lcat.Name.ToLower())
                {
                    return RedirectToAction("ListLaborCategories", "User");
                }
                else
                {
                    LaborCategory obj = new LaborCategory()
                    {
                        Name = project.Name,
                        Is_Authorize = 1, 
                        OldLabourCategoryId = lcat.Id
                    };

                    bool chk1 = new UserBL().AddLaborCategory(obj, de);
                    if (chk1)
                    {
                        LaborCategory obj2 = new LaborCategory()
                        {
                            Id = lcat.Id,
                            Name = lcat.Name,
                            UpdatedAt = DateTime.Now,
                            Is_Authorize = 0
                        };

                        bool chk2 = new UserBL().UpdateLaborCategory(obj2);
                    }
                    

                    return RedirectToAction("ListLaborCategories", "User");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion


        #region AssignProject
        public ActionResult AssignProject(int Project_Id, string message)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                List<ProjectUserCategory> projectusercats = new UserBL().getProjectUserCategoryList().Where(x => x.ProjectCategory.Project_Id == Project_Id && x.Is_Authorize == 1).ToList();
                List<User> notassignedusers = new UserBL().getUserList().Where(x => x.Is_Authorize == 1 && x.Role != 2).ToList();

                foreach (ProjectUserCategory puc in projectusercats)
                {
                    var item = notassignedusers.Find(l => l.Id == puc.User_Id);
                    notassignedusers.Remove(item);
                }


                Project p = new UserBL().getProjectById(Project_Id);
                List<LaborCategory> laborcategories = new List<LaborCategory>();
                List<ProjectCategory> pcs = new UserBL().getProjectCategoryList().Where(x => x.Project_Id == Project_Id && x.Is_Authorize == 1).ToList();

                foreach (ProjectCategory pc in pcs)
                {
                    LaborCategory mylc = new UserBL().getLaborCategoryById(pc.LaborCategory_Id);
                    laborcategories.Add(mylc);
                }


                List<ProjectUserCategory> pucs = new List<ProjectUserCategory>();

                foreach (ProjectCategory pc in pcs)
                {
                    List<ProjectUserCategory> mypuc = new UserBL().getProjectUserCategoryList().Where(x => x.Is_Authorize == 1 && x.ProjectCategory_Id == pc.Id).ToList();
                   
                    foreach (var item in mypuc)
                    {
                        pucs.Add(item);
                    }

                }


                ViewBag.message = message;
                ViewBag.p = p;
                ViewBag.allusers = notassignedusers;
                ViewBag.laborcategories = laborcategories;
                ViewBag.pucs = pucs;
                ViewBag.pcs = pcs;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult PostAssignProject(int User_Id, int Project_Id, int LaborCategory_Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                ProjectCategory pc = new UserBL().getProjectCategoryList().Where(x => x.Is_Authorize == 1 && x.Project_Id == Project_Id && x.LaborCategory_Id == LaborCategory_Id).FirstOrDefault();

                int count = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == User_Id && x.Is_Authorize == 1 && x.ProjectCategory.Project_Id == Project_Id).Count();

                if (count >= 1)
                {
                    return RedirectToAction("AssignProject", "User", new { Project_Id = Project_Id, message = "This user already have a labor category." });
                }

                else
                {
                    ProjectUserCategory pu = new ProjectUserCategory()
                    {
                        User_Id = User_Id,
                        ProjectCategory_Id = pc.Id,
                        Is_Authorize = 1
                    };
                    new UserBL().AddProjectUserCategory(pu);
                }
                return RedirectToAction("AssignProject", "User", new { Project_Id = Project_Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult UpdateAssignProject(int ProjectUserCategoryId, int User_Id, int Project_Id, int LaborCategory_Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                ProjectUserCategory puc = new UserBL().getProjectUserCategoryById(ProjectUserCategoryId);
                ProjectCategory pc = new UserBL().getProjectCategoryList().Where(x => x.Is_Authorize == 1 && x.Project_Id == Project_Id && x.LaborCategory_Id == LaborCategory_Id).FirstOrDefault();
                int count = new UserBL().getProjectUserCategoryList().Where(x => x.User_Id == User_Id && x.Is_Authorize == 1 && x.ProjectCategory.Project_Id == Project_Id).Count();

                if (count >= 1)
                {
                    return RedirectToAction("AssignProject", "User", new { Project_Id = Project_Id, message = "This user already have a labor category." });
                }
                else
                {
                    ProjectUserCategory pu = new ProjectUserCategory()
                    {
                        Id = puc.Id,
                        User_Id = User_Id,
                        ProjectCategory_Id = pc.Id,
                        Is_Authorize = 1
                    };
                    new UserBL().UpdateProjectUserCategory(pu);
                    return RedirectToAction("AssignProject", "User", new { Project_Id = Project_Id });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DeleteAssignProject(int Id)
        {
            if (sessiondto.getName() != null && sessiondto.getRole() == 2)
            {
                ProjectUserCategory p = new UserBL().getProjectUserCategoryById(Id);
                int userId = p.User_Id;
                int Project_Id = p.ProjectCategory.Project.Id;

                ProjectUserCategory pu = new ProjectUserCategory()
                {
                    Id = p.Id,
                    User_Id = p.User_Id,
                    ProjectCategory_Id = p.ProjectCategory_Id,
                    Is_Authorize = 0
                };
                new UserBL().UpdateProjectUserCategory(pu);

                return RedirectToAction("AssignProject", "User", new { Project_Id = Project_Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

    }
}