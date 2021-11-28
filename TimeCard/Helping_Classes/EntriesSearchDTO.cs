using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeCard.Helping_Classes
{
    public class EntriesSearchDTO
    {
        public string Date { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public double Hours { get; set; }
        public string Project { get; set; }
        public string Status { get; set; }
        public string RejectReason { get; set; }
        public string LCAT { get; set; }
    }
}