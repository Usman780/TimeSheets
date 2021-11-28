using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.Helping_Classes
{
    public class enteriesDTO
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public string LCAT { get; set; }
        public double Hour { get; set; }
        public string Project { get; set; }
        public string Status { get; set; }
        public double TotalHours { get; set; } 
    }
}