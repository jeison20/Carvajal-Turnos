//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            this.Advices = new HashSet<Advices>();
            this.Advices1 = new HashSet<Advices>();
            this.BlockCalendar = new HashSet<BlockCalendar>();
            this.BlockCalendar1 = new HashSet<BlockCalendar>();
            this.Centres = new HashSet<Centres>();
            this.LinkedCentres = new HashSet<LinkedCentres>();
            this.LinkedProducts = new HashSet<LinkedProducts>();
            this.LinkedUsers = new HashSet<LinkedUsers>();
            this.LinkedUsers1 = new HashSet<LinkedUsers>();
            this.Orders = new HashSet<Orders>();
            this.Orders1 = new HashSet<Orders>();
            this.Turns = new HashSet<Turns>();
            this.Turns1 = new HashSet<Turns>();
            this.Turns2 = new HashSet<Turns>();
            this.Turns3 = new HashSet<Turns>();
        }
    
        public string PkIdentifier { get; set; }
        public Nullable<bool> ChangePasswordNextTime { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> LastAccess { get; set; }
        public string FkRole_Identifier { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> LastChangeDate { get; set; }
        public string FkCompanies_Identifier { get; set; }
        public long FkCountries_Identifier { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Advices> Advices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Advices> Advices1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlockCalendar> BlockCalendar { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlockCalendar> BlockCalendar1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centres> Centres { get; set; }
        public virtual Companies Companies { get; set; }
        public virtual Countries Countries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedCentres> LinkedCentres { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedProducts> LinkedProducts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedUsers> LinkedUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedUsers> LinkedUsers1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders1 { get; set; }
        public virtual Roles Roles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Turns> Turns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Turns> Turns1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Turns> Turns2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Turns> Turns3 { get; set; }
    }
}
