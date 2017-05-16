namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrdersProducts
    {
        [Key]
        public long PkIdentifier { get; set; }

        public long? FkOrders_Identifier { get; set; }

        public int? Line { get; set; }

        [StringLength(35)]
        public string Code { get; set; }

        [StringLength(70)]
        public string Description { get; set; }

        public long? SplitQuantity { get; set; }

        public virtual Orders Orders { get; set; }
    }
}
