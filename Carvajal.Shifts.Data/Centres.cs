namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Centres
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Centres()
        {
            Advices = new HashSet<Advices>();
            Exceptions = new HashSet<Exceptions>();
        }

        [Required]
        [StringLength(35)]
        public string FkUsers_Merchant_Identifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Responsable_Identifier { get; set; }

        [Key]
        [StringLength(35)]
        public string PkIdentifier { get; set; }

        public int? WeeklyCapacity { get; set; }

        public int? CurrentWeekCapacity { get; set; }

        [StringLength(1)]
        public string FirstDay { get; set; }

        [StringLength(7)]
        public string ListOfWorkingDays { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? StartTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EndTime { get; set; }

        [StringLength(175)]
        public string Name { get; set; }

        public short? MumberOfDocks { get; set; }

        public int? TimeBetweenSuppliers { get; set; }

        public bool Status { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastChangeDate { get; set; }

        public int FkTimezones_Identifier { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Advices> Advices { get; set; }

        public virtual Timezones Timezones { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exceptions> Exceptions { get; set; }
    }
}
