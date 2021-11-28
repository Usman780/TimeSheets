using TimeCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.DAL
{
    public class UserDAL
    {
        timecardEntities db;

        #region EarnedVacationTime
        public List<EarnedVacationTime> getEarnedVacationTimesList()
        {
            List<EarnedVacationTime> EarnedVacationTimes;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            //EarnedVacationTimes = db.EarnedVacationTimes.ToList();
            EarnedVacationTimes = db.sp_GetAll_EarnedVacationTimes().ToList();
            //}

            return EarnedVacationTimes;
        }

        public EarnedVacationTime getEarnedVacationTimeById(int _Id)
        {
            EarnedVacationTime EarnedVacationTime;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            EarnedVacationTime = db.EarnedVacationTimes.FirstOrDefault(x => x.Id == _Id);
            //}

            return EarnedVacationTime;
        }

        public bool AddEarnedVacationTime(EarnedVacationTime _EarnedVacationTime)
        {
            using (db = new timecardEntities())
            {
                db.EarnedVacationTimes.Add(_EarnedVacationTime);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateEarnedVacationTime(EarnedVacationTime _EarnedVacationTime)
        {
            using (db = new timecardEntities())
            {
                db.Entry(_EarnedVacationTime).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return true;
        }
        #endregion

        #region User
        public List<User> getUsersList()
        {
            List<User> users;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
                //users = db.Users.ToList();
                users = db.sp_GetAll_Users().ToList();
            //}

            return users;
        }

        public User getUserById(int _Id)
        {
            User user;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
                user = db.Users.FirstOrDefault(x => x.Id == _Id);
            //}

            return user;
        }

        public bool AddUser(User _user)
        {
            using (db = new timecardEntities())
            {
                db.Users.Add(_user);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateUser(User _user)
        {
            using (db = new timecardEntities())
            {
                db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return true;
        }        
        #endregion

        #region Project
        public List<Project> getProjectsList(timecardEntities de = null)
        {
            List<Project> users = new List<Project>();
            
            if (de != null)
            {
                de = new timecardEntities();
                //users = de.Projects.ToList();
                users = de.sp_GetAll_Projects().ToList();
            }
            else
            {
                db = new timecardEntities();
                //using (db = new timecardEntities())
                //{
                users = db.sp_GetAll_Projects().ToList();
                //users = db.Projects.ToList();
                //}
            }
            return users;
        }

        public Project getProjectById(int _Id)
        {
            Project user;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
                user = db.Projects.FirstOrDefault(x => x.Id == _Id);
            //}

            return user;
        }

        public bool AddProject(Project _user)
        {
            using (db = new timecardEntities())
            {
                db.Projects.Add(_user);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateProject(Project _user, timecardEntities de = null)
        {
            if (de != null)
            {
                de.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
            }
            else
            {
                using (db = new timecardEntities())
                {
                    db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return true;
        }
        #endregion

        #region EntryTime
        public List<EntryTime> getEntryTimesList()
        {
            List<EntryTime> users;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
                users = db.sp_GetAll_EntryTimes().ToList();
              //  users = db.EntryTimes.ToList();
            //}

            return users;
        }

        public EntryTime getEntryTimeById(int _Id)
        {
            EntryTime user;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
                user = db.EntryTimes.FirstOrDefault(x => x.Id == _Id);
            //

            return user;
        }

        public bool AddEntryTime(EntryTime _user)
        {
            using (db = new timecardEntities())
            {
                db.EntryTimes.Add(_user);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateEntryTime(EntryTime _user)
        {
            using (db = new timecardEntities())
            {
                //db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.EntryTime_AddUpdateEntryTime_sp("Update", _user.Id, _user.ProjectUserCategory_Id, _user.Date, _user.Type, _user.Hour, _user.Is_Approved, _user.Is_Authorize, _user.Created_At, _user.EntryTime_Id, _user.RejectReason, _user.DeleteReason);
                db.SaveChanges();
            }
            return true;
        }

        public void DeleteEntry(int _id)
        {
            using (db = new timecardEntities())
            {
                db.EntryTimes.Remove(db.EntryTimes.FirstOrDefault(x => x.Id == _id));
                db.SaveChanges();
            }
        }
        #endregion

        #region ProjectUserCategory
        public List<ProjectUserCategory> getProjectUserCategoriesList()
        {
            List<ProjectUserCategory> users;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
                users = db.sp_GetAll_ProjectUserCategorys().ToList();
              //  users = db.ProjectUserCategories.ToList();
            //}

            return users;
        }

        public ProjectUserCategory getProjectUserCategoryById(int _Id)
        {
            ProjectUserCategory user;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
                user = db.ProjectUserCategories.FirstOrDefault(x => x.Id == _Id);
            //}

            return user;
        }

        public bool AddProjectUserCategory(ProjectUserCategory _user)
        {
            using (db = new timecardEntities())
            {
                db.ProjectUserCategories.Add(_user);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateProjectUserCategory(ProjectUserCategory _user)
        {
            using (db = new timecardEntities())
            {
                db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return true;
        }

        public void DeleteProjectUser(int _id)
        {
            using (db = new timecardEntities())
            {
                db.ProjectUserCategories.Remove(db.ProjectUserCategories.FirstOrDefault(x => x.Id == _id));
                db.SaveChanges();
            }
        }
        #endregion

        #region Carrier
        public List<Carrier> getCarriersList()
        {
            List<Carrier> users;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            users = db.sp_GetAll_Carriers().ToList();
           // users = db.Carriers.ToList();
            //}

            return users;
        }

        public Carrier getCarrierById(int _Id)
        {
            Carrier user;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            user = db.Carriers.FirstOrDefault(x => x.Id == _Id);
            //}

            return user;
        }

        public bool AddCarrier(Carrier _user)
        {
            using (db = new timecardEntities())
            {
                db.Carriers.Add(_user);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateCarrier(Carrier _user)
        {
            using (db = new timecardEntities())
            {
                db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return true;
        }

        //public void DeleteUser(int _id)
        //{
        //    using (db = new campaignEntities())
        //    {
        //        db.Users.Remove(db.Users.FirstOrDefault(x => x.Id == _id));
        //        db.SaveChanges();
        //    }
        //}
        #endregion

        #region LaborCategory
        public List<LaborCategory> getLaborCategoriesList()
        {
            List<LaborCategory> users;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            users = db.sp_GetAll_LaborCategorys().ToList();
          //  users = db.LaborCategories.ToList();
            //}

            return users;
        }

        public LaborCategory getLaborCategoryById(int _Id, timecardEntities de = null)
        {
            if (de != null)
            {
                LaborCategory user;
                de = new timecardEntities();
                user = de.LaborCategories.FirstOrDefault(x => x.Id == _Id);
                return user;
            }
            else
            {
                LaborCategory user;
                db = new timecardEntities();
                user = db.LaborCategories.FirstOrDefault(x => x.Id == _Id);
                return user;
            }
        }

        public bool AddLaborCategory(LaborCategory _user, timecardEntities de = null)
        {
            if (de != null)
            {
                de.LaborCategories.Add(_user);
                de.SaveChanges();
                return true;
            }
            else
            {
                using (db = new timecardEntities())
                {
                    db.LaborCategories.Add(_user);
                    db.SaveChanges();
                }
                return true;
            }
        }

        public bool UpdateLaborCategory(LaborCategory _user, timecardEntities de = null)
        {
            if (de != null)
            {
                de.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            else
            {
                using (db = new timecardEntities())
                {
                    db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return true;
            }
        }

        //public void DeleteUser(int _id)
        //{
        //    using (db = new campaignEntities())
        //    {
        //        db.Users.Remove(db.Users.FirstOrDefault(x => x.Id == _id));
        //        db.SaveChanges();
        //    }
        //}
        #endregion

        #region ProjectCategory
        public List<ProjectCategory> getProjectCategoriesList()
        {
            List<ProjectCategory> users;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            users = db.sp_GetAll_ProjectCategories().ToList();
           // users = db.ProjectCategories.ToList();
            //}

            return users;
        }

        public ProjectCategory getProjectCategoryById(int _Id)
        {
            ProjectCategory user;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            user = db.ProjectCategories.FirstOrDefault(x => x.Id == _Id);
            //}

            return user;
        }

        public bool AddProjectCategory(ProjectCategory _user)
        {
            using (db = new timecardEntities())
            {
                db.ProjectCategories.Add(_user);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateProjectCategory(ProjectCategory _user)
        {
            using (db = new timecardEntities())
            {
                db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return true;
        }
        #endregion

        #region StartDate
        public List<StartDate> getStartDatesList()
        {
            List<StartDate> users;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            users = db.sp_GetAll_StartDates().ToList();
            //users = db.StartDates.ToList();
            //}

            return users;
        }

        public StartDate getStartDateById(int _Id)
        {
            StartDate user;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            user = db.StartDates.FirstOrDefault(x => x.Id == _Id);
            //}

            return user;
        }

        public bool AddStartDate(StartDate _user)
        {
            using (db = new timecardEntities())
            {
                db.StartDates.Add(_user);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateStartDate(StartDate _user)
        {
            using (db = new timecardEntities())
            {
                db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return true;
        }
        #endregion

        #region Vacation Bank
        public List<VacationBank> getVacationBanksList()
        {
            List<VacationBank> users;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            users = db.sp_GetAll_VacationBanks().ToList();
           // users = db.VacationBanks.ToList();
            //}

            return users;
        }

        public VacationBank getVacationBankById(int _Id)
        {
            VacationBank user;
            db = new timecardEntities();
            //using (db = new timecardEntities())
            //{
            user = db.VacationBanks.FirstOrDefault(x => x.Id == _Id);
            //}

            return user;
        }

        public bool AddVacationBank(VacationBank _user)
        {
            using (db = new timecardEntities())
            {
                db.VacationBanks.Add(_user);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateVacationBank(VacationBank _user)
        {
            using (db = new timecardEntities())
            {
                //db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.VacationBank_AddUpdateVacationBank_sp("Update", _user.Id, _user.VacationHoursPerYear, _user.AccuralVacationHours, _user.PayperiodCount, _user.Is_Authorize, _user.User_Id, _user.Created_Date);
                db.SaveChanges();
            }
            return true;
        }

        public void DeleteVacationBank(int _id)
        {
            using (db = new timecardEntities())
            {
                db.VacationBanks.Remove(db.VacationBanks.FirstOrDefault(x => x.Id == _id));
                db.SaveChanges();
            }
        }
        #endregion
    }
}