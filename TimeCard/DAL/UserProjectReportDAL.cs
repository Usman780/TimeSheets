using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeCard.Models;

namespace TimeCard.DAL
{
    public class UserProjectReportDAL
    {
        public List<UserProjectReport> GetAllUserProjectReports()
        {
            timecardEntities db = new timecardEntities();

            return db.sp_GetAll_UserProjectReports().ToList();
            //return db.UserProjectReports.ToList();
        }

        public List<UserProjectReport> GetActiveUserProjectReports()
        {
            timecardEntities db = new timecardEntities();

            return db.UserProjectReports.Where(x=>x.IsActive == 1).ToList();
        }

        public UserProjectReport GetUserProjectReportById(int _Id, timecardEntities de)
        {
            return de.UserProjectReports.Where(x => x.Id == _Id).FirstOrDefault();
        }

        public UserProjectReport GetUserProjectReportByUserProjectId(int UserId, int ProjectId)
        {
            timecardEntities db = new timecardEntities();

            return db.UserProjectReports.Where(x => x.UserId == UserId && x.ProjectId == ProjectId).FirstOrDefault();
        }

        public bool AddUserProjectReport(UserProjectReport _upr, timecardEntities de)
        {
            try
            {
                de.UserProjectReports.Add(_upr);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUserProjectReport(UserProjectReport _upr, timecardEntities de)
        {
            try
            {
                de.Entry(_upr).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteUserProjectReport(int _Id)
        {
            try
            {
                timecardEntities db = new timecardEntities();

                db.UserProjectReports.Remove(db.UserProjectReports.Where(x => x.Id == _Id).FirstOrDefault());
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}