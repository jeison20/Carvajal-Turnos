namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TurnsAudit")]
    public partial class TurnsAudit
    {
        [Key]
        [Column(Order = 0, TypeName = "smalldatetime")]
        public DateTime EventDate { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(110)]
        public string Subject { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(512)]
        public string Message { get; set; }
    }
}
