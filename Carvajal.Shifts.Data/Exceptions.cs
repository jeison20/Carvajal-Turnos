namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Exceptions
    {
        public long? FkUsers_Merchant_Identifier { get; set; }

        public long? FkCentres_Identifier { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? StartDateTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EndDateTime { get; set; }

        public int? Dock { get; set; }

        [Key]
        [Column(Order = 0)]
        public bool GeneralRuleToApply { get; set; }

        public int? FkReason_Identifier { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool Status { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastChangeDate { get; set; }

        public long? FkUsers_Creator_Identifier { get; set; }

        public virtual Centres Centres { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }
    }
}
