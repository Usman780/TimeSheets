//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimeCard.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EarnedVacationTime
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Hours { get; set; }
        public string Type { get; set; }
        public Nullable<int> User_Id { get; set; }
    
        public virtual User User { get; set; }
    }
}