using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.Helping_Classes
{
    public class UserProjectReportDTO
    {
        public int Id { set; get; }
        public int UserId { set; get; }
        public int ProjectId { set; get; }
        public string Code { set; get; }
        public int IsChecked { set; get; }
    }
}