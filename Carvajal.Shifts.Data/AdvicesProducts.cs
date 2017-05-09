namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdvicesProducts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PkIdentifier { get; set; }

        public int? FkAdvices_Identifier { get; set; }

        public long? Code { get; set; }

        [StringLength(35)]
        public string Description { get; set; }

        public decimal? ReceivedAndAcceptedQuantity { get; set; }

        public virtual Advices Advices { get; set; }
    }
}
