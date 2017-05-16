namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Exceptions
    {
        [StringLength(35)]
        public string FkUsers_Merchant_Identifier { get; set; }

        [StringLength(35)]
        public string FkCentres_Identifier { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? StartDateTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EndDateTime { get; set; }

        public int? Dock { get; set; }

        [Key]
        [Column(Order = 0)]
        public bool GeneralRuleToApply { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string FkBlockingReasons_Identifier { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool Status { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastChangeDate { get; set; }

        [StringLength(35)]
        public string FkUsers_Creator_Identifier { get; set; }

        public virtual BlockingReasons BlockingReasons { get; set; }

        public virtual Centres Centres { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }
    }
}
