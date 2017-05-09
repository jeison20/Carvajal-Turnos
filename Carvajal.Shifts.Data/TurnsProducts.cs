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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PkIdentifier { get; set; }

        public int? FkTurns_Identifier { get; set; }

        public long? Code { get; set; }

        public decimal? InTurnQuantity { get; set; }

        public virtual Turns Turns { get; set; }
    }
}
