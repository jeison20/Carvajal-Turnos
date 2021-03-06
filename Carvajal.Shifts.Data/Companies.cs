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
    
    public partial class Companies
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Companies()
        {
            this.Centres = new HashSet<Centres>();
            this.UnloadingTime = new HashSet<UnloadingTime>();
            this.Users = new HashSet<Users>();
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
        public string Companies_Identifier { get; set; }
        public string AddressStreet { get; set; }
        public string AddressNumber { get; set; }
        public string PostCode { get; set; }
        public string Town { get; set; }
        public string Region { get; set; }
        public long FkCountries_Identifier { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centres> Centres { get; set; }
        public virtual Countries Countries { get; set; }
        public virtual Roles Roles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UnloadingTime> UnloadingTime { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users { get; set; }
    }
}
