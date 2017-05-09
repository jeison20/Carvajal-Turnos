namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WorkingHours
    {
        public long? FkUsers_Merchant_Identifier { get; set; }

        public long? FkCentres_Identifier { get; set; }

        [StringLength(1)]
        public string WeekdayName { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? StartTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EndTime { get; set; }

        [Key]
        public bool Status { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastChangeDate { get; set; }

        public virtual Centres Centres { get; set; }

        public virtual Users Users { get; set; }
    }
}
