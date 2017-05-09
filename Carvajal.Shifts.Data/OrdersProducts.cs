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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PkIdentifier { get; set; }

        public int? FkOrders_Identifier { get; set; }

        public int? Line { get; set; }

        public long? Code { get; set; }

        [StringLength(35)]
        public string Description { get; set; }

        public decimal? SplitQuantity { get; set; }

        public virtual Orders Orders { get; set; }
    }
}
