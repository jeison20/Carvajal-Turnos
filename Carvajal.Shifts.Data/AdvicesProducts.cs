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
        public long PkIdentifier { get; set; }

        public long? FkAdvices_Identifier { get; set; }

        [StringLength(35)]
        public string Code { get; set; }

        public long? ReceivedAndAcceptedQuantity { get; set; }

        public virtual Advices Advices { get; set; }
    }
}
