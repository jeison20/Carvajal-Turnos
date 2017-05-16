namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TurnsProducts
    {
        [Key]
        public long PkIdentifier { get; set; }

        public long? FkTurns_Identifier { get; set; }

        [StringLength(35)]
        public string Code { get; set; }

        public long? ScheludedQuantity { get; set; }

        public virtual Turns Turns { get; set; }
    }
}
