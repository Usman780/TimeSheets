using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeCard.DAL;
using TimeCard.Models;

namespace TimeCard.BL
{
    public class UserProjectReportBL
    {
        public List<UserProjectReport> GetAllUserProjectReports()
        {
            return new UserProjectReportDAL().GetAllUserProjectReports();
        }

        public List<UserProjectReport> GetActiveUserProjectReports()
        {
            return new UserProjectReportDAL().GetActiveUserProjectReports();
        }

        public UserProjectReport GetUserProjectReportById(int _Id, timecardEntities de)
        {
            return new UserProjectReportDAL().GetUserProjectReportById(_Id, de);
        }

        public UserProjectReport GetUserProjectReportByUserProjectId(int UserId, int ProjectId)
        {
            return new UserProjectReportDAL().GetUserProjectReportByUserProjectId(UserId, ProjectId);
        }

        public bool AddUserProjectReport(UserProjectReport _upr, timecardEntities de)
        {
            return new UserProjectReportDAL().AddUserProjectReport(_upr, de);
        }

        public bool UpdateUserProjectReport(UserProjectReport _upr, timecardEntities de)
        {
            return new UserProjectReportDAL().UpdateUserProjectReport(_upr, de);
        }

        public bool DeleteUserProjectReport(int _Id)
        {
            return new UserProjectReportDAL().DeleteUserProjectReport(_Id);
        }
    }
}