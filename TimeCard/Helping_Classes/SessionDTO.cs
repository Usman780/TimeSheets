using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.Helping_Classes
{
    public class SessionDTO
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Role { get; set; }
        public string Email { get; set; }


        public string getName()
        {
            SessionDTO sdto = (SessionDTO)HttpContext.Current.Session["Session"];
            if (sdto == null)
                return null;

            return sdto.Name;
        }

        public int getId()
        {
            SessionDTO sdto = (SessionDTO)HttpContext.Current.Session["Session"];
            if (sdto == null)
                return -1;

            return sdto.Id;
        }

        public static int getid()
        {
            SessionDTO sdto = (SessionDTO)HttpContext.Current.Session["Session"];
            if (sdto == null)
                return -1;

            return sdto.Id;
        }

        public int getRole()
        {
            SessionDTO sdto = (SessionDTO)HttpContext.Current.Session["Session"];
            if (sdto == null)
                return -1;

            return sdto.Role;
        }

        public static int getrole()
        {
            SessionDTO sdto = (SessionDTO)HttpContext.Current.Session["Session"];
            if (sdto == null)
                return -1;

            return sdto.Role;
        }

        public string getEmail()
        {
            SessionDTO sdto = (SessionDTO)HttpContext.Current.Session["Session"];
            if (sdto == null)
                return null;

            return sdto.Email;
        }         
        
        public static string getemail()
        {
            SessionDTO sdto = (SessionDTO)HttpContext.Current.Session["Session"];
            if (sdto == null)
                return null;

            return sdto.Email;
        }       
    }
}