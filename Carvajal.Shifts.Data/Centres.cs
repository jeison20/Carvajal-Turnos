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
            LinkedCentres = new HashSet<LinkedCentres>();
            WorkingHours = new HashSet<WorkingHours>();
        }

        public long? FkUsers_Identifier { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PkIdentifier { get; set; }

        public int? WeeklyCapacity { get; set; }

        public int? CurrentWeekCapacity { get; set; }

        [StringLength(1)]
        public string FirstDay { get; set; }

        [StringLength(70)]
        public string Name { get; set; }

        public short? MumberOfDock { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? TimeBetweenSuppliers { get; set; }

        public bool Status { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastChangeDate { get; set; }

        public short? Timezone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Advices> Advices { get; set; }

        public virtual Users Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exceptions> Exceptions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedCentres> LinkedCentres { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingHours> WorkingHours { get; set; }
    }
}
