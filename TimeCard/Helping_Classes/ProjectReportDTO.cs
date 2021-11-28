using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.Helping_Classes
{
    public class ProjectReportDTO
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string ProjectName { get; set; }
        public string EmployeeName { get; set; }
        public string Status { get; set; }
        public int LcatId { get; set; }
        public string LCAT { get; set; }
        public double Hours { get; set; }
        public double Cost { get; set; }
        public string Budget { get; set; }
    }
}