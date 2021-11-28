using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TimeCard.BL;
using TimeCard.Helping_Classes;
using TimeCard.Models;

namespace TimeCard.Controllers
{
    public class AuthController : Controller
    {
        SessionDTO sessiondto = new SessionDTO();

        [HttpPost]
        public ActionResult PostLogin(string Email, string Password)
        {
            List<User> Users = new UserBL().getUserList().Where(x => x.Is_Authorize == 1).ToList();

            foreach (User User in Users)
            {
                if (User.Email == Email && User.Password == Password)
                {
                    SessionDTO session = new SessionDTO();
                    session.Name = User.FirstName + ' ' + User.LastName;
                    session.Id = User.Id;
                    session.Role = User.Role;
                    Session["Session"] = session;

                    SessionDTO sdto = (SessionDTO)Session["Session"];
                    if (User.Role == 0)
                    {
                        return RedirectToAction("TimeSheetEntry", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }
            return RedirectToAction("Index", "Home", new { err = "Incorrect Email or Password" });
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword(string err = "")
        {
            if (sessiondto.getName() == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.err = err;
                return View();
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            if (sessiondto.getName() == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                User u = new UserBL().getUserById(sessiondto.getId());

                if (u.Password == OldPassword)
                {
                    if (NewPassword == ConfirmPassword)
                    {
                        User user = new Models.User()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LabourCategoryId = u.LabourCategoryId,
                            LastName = u.LastName,
                            Phone = u.Phone,
                            Email = u.Email,
                            Password = NewPassword,
                            Is_Authorize = u.Is_Authorize,
                            Role = u.Role,
                            User_Id = u.User_Id,
                            PhoneMail = u.PhoneMail
                        };

                        new UserBL().UpdateUser(user);

                        return RedirectToAction("ChangePassword", "Auth", new { err = "Password updated successfully." });

                    }
                    else
                    {
                        return RedirectToAction("ChangePassword", "Auth", new { err = "New password and confirm password doesn't match." });
                    }
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Auth", new { err = "You have entered wrong old password." });

                }
            }
        }

    }
}