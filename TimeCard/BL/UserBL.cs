using TimeCard.DAL;
using TimeCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.BL
{
    public class UserBL
    {

        #region EarnedVacationTime
        public List<EarnedVacationTime> getEarnedVacationTimeList()
        {
            return new UserDAL().getEarnedVacationTimesList();
        }

        public EarnedVacationTime getEarnedVacationTimeById(int _id)
        {
            return new UserDAL().getEarnedVacationTimeById(_id);
        }

        public bool AddEarnedVacationTime(EarnedVacationTime _EarnedVacationTime)
        {
            if (_EarnedVacationTime.Date == null || _EarnedVacationTime.Hours == null || _EarnedVacationTime.Type == null || _EarnedVacationTime.User_Id == null)
                return false;
            return new UserDAL().AddEarnedVacationTime(_EarnedVacationTime);
        }

        public bool UpdateEarnedVacationTime(EarnedVacationTime _EarnedVacationTime)
        {
            if (_EarnedVacationTime.Date == null || _EarnedVacationTime.Hours == null || _EarnedVacationTime.Type == null || _EarnedVacationTime.User_Id == null)
                return false;

            return new UserDAL().UpdateEarnedVacationTime(_EarnedVacationTime);
        }

        #endregion

        #region User
        public List<User> getUserList()
        {
            return new UserDAL().getUsersList();
        }

        public User getUserById(int _id)
        {
            return new UserDAL().getUserById(_id);
        }

        public bool AddUser(User _user)
        {
            if (_user.FirstName == null || _user.LastName == null || _user.Email == null || _user.Password == null)
                return false;
            return new UserDAL().AddUser(_user);
        }

        public bool UpdateUser(User _user)
        {
            if (_user.FirstName == null || _user.LastName == null || _user.Email == null || _user.Password == null)
                return false;

            return new UserDAL().UpdateUser(_user);
        }
        
        #endregion


        #region Project
        public List<Project> getProjectList(timecardEntities de=null)
        {
            return new UserDAL().getProjectsList(de);
        }

        public Project getProjectById(int _id)
        {
            return new UserDAL().getProjectById(_id);
        }

        public bool AddProject(Project _user)
        {
            return new UserDAL().AddProject(_user);
        }

        public bool UpdateProject(Project _user, timecardEntities de = null)
        {
            return new UserDAL().UpdateProject(_user, de);
        }
        
        #endregion


        #region EntryTime
        public List<EntryTime> getEntryTimeList()
        {
            return new UserDAL().getEntryTimesList();
        }

        public EntryTime getEntryTimeById(int _id)
        {
            return new UserDAL().getEntryTimeById(_id);
        }

        public bool AddEntryTime(EntryTime _user)
        {
            return new UserDAL().AddEntryTime(_user);
        }

        public bool UpdateEntryTime(EntryTime _user)
        {

            return new UserDAL().UpdateEntryTime(_user);
        }

        public void DeleteEntry(int _id)
        {
            new UserDAL().DeleteEntry(_id);
        }
        #endregion


        #region ProjectUserCategory
        public List<ProjectUserCategory> getProjectUserCategoryList()
        {
            return new UserDAL().getProjectUserCategoriesList();
        }

        public ProjectUserCategory getProjectUserCategoryById(int _id)
        {
            return new UserDAL().getProjectUserCategoryById(_id);
        }

        public bool AddProjectUserCategory(ProjectUserCategory _user)
        {
            return new UserDAL().AddProjectUserCategory(_user);
        }

        public bool UpdateProjectUserCategory(ProjectUserCategory _user)
        {

            return new UserDAL().UpdateProjectUserCategory(_user);
        }

        public void DeleteProjectUserCategory(int _id)
        {
            new UserDAL().DeleteProjectUser(_id);
        }
        #endregion


        #region Carrier
        public List<Carrier> getCarrierList()
        {
            return new UserDAL().getCarriersList();
        }

        public Carrier getCarrierById(int _id)
        {
            return new UserDAL().getCarrierById(_id);
        }

        public bool AddCarrier(Carrier _user)
        {
            if (_user.Name == null || _user.Gateway == null || _user.Country == null)
                return false;
            return new UserDAL().AddCarrier(_user);
        }

        public bool UpdateCarrier(Carrier _user)
        {
            if (_user.Name == null || _user.Gateway == null || _user.Country == null)
                return false;

            return new UserDAL().UpdateCarrier(_user);
        }

        //public void DeleteUser(int _id)
        //{
        //    new UserDAL().DeleteUser(_id);
        //}
        #endregion


        #region LaborCategory
        public List<LaborCategory> getLaborCategoryList()
        {
            return new UserDAL().getLaborCategoriesList();
        }

        public LaborCategory getLaborCategoryById(int _id, timecardEntities de = null)
        {
            return new UserDAL().getLaborCategoryById(_id, de);
        }

        public bool AddLaborCategory(LaborCategory _user, timecardEntities de = null)
        {
            if (_user.Name == null)
                return false;
            return new UserDAL().AddLaborCategory(_user, de);
        }

        public bool UpdateLaborCategory(LaborCategory _user, timecardEntities de = null)
        {
            if (_user.Name == null)
                return false;

            return new UserDAL().UpdateLaborCategory(_user, de);
        }

        //public void DeleteUser(int _id)
        //{
        //    new UserDAL().DeleteUser(_id);
        //}
        #endregion


        #region ProjectCategory
        public List<ProjectCategory> getProjectCategoryList()
        {
            return new UserDAL().getProjectCategoriesList();
        }

        public ProjectCategory getProjectCategoryById(int _id)
        {
            return new UserDAL().getProjectCategoryById(_id);
        }

        public bool AddProjectCategory(ProjectCategory _user)
        {
            return new UserDAL().AddProjectCategory(_user);
        }

        public bool UpdateProjectCategory(ProjectCategory _user)
        {

            return new UserDAL().UpdateProjectCategory(_user);
        }
        #endregion


        #region StartDate
        public List<StartDate> getStartDateList()
        {
            return new UserDAL().getStartDatesList();
        }

        public StartDate getStartDateById(int _id)
        {
            return new UserDAL().getStartDateById(_id);
        }

        public bool AddStartDate(StartDate _user)
        {
            return new UserDAL().AddStartDate(_user);
        }

        public bool UpdateStartDate(StartDate _user)
        {

            return new UserDAL().UpdateStartDate(_user);
        }
        #endregion

        #region Vacation Bank
        public List<VacationBank> getVacationBankList()
        {
            return new UserDAL().getVacationBanksList();
        }

        public VacationBank getVacationBankById(int _id)
        {
            return new UserDAL().getVacationBankById(_id);
        }

        public bool AddVacationBank(VacationBank _user)
        {
            return new UserDAL().AddVacationBank(_user);
        }

        public bool UpdateVacationBank(VacationBank _user)
        {

            return new UserDAL().UpdateVacationBank(_user);
        }

        public void DeleteVacationBank(int _id)
        {
            new UserDAL().DeleteVacationBank(_id);
        }
        #endregion
    }
}