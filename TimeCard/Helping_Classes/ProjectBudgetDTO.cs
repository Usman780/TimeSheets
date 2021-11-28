using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.Helping_Classes
{
    public class ProjectBudgetDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Budget { get; set; }
        public string TotalCost { get; set; }
        public string Remaining { get; set; }
        public string RemainingBudget { get; set; }

        
    }
}