using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.Helping_Classes
{
    public class projectDTO
    {
        public int Id { get; set; }
        public string IsLocked { get; set; }
        public string Code { get; set; }
        public string Budget { get; set; }
        public string LabourCategories { get; set; }
        public string Employees { get; set; }

    }
}