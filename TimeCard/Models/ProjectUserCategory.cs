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
    
    public partial class ProjectUserCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProjectUserCategory()
        {
            this.EntryTimes = new HashSet<EntryTime>();
        }
    
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int ProjectCategory_Id { get; set; }
        public int Is_Authorize { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntryTime> EntryTimes { get; set; }
        public virtual ProjectCategory ProjectCategory { get; set; }
        public virtual User User { get; set; }
    }
}
