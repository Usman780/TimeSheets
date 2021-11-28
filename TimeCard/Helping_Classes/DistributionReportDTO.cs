using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.Helping_Classes
{
    public class DistributionReportDTO
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public double VacationHour { set; get; }
        public double OverheadHour { set; get; }
        public double HolidayHour { set; get; }
        public double OtherHour { set; get; }
    }
}