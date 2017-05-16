namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UnloadingTime")]
    public partial class UnloadingTime
    {
        [StringLength(35)]
        public string FkUsers_Merchant_Identifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Manufacturer_Identifier { get; set; }

        [StringLength(35)]
        public string ProductCode { get; set; }

        public decimal? AmountPerPallet { get; set; }

        [StringLength(1)]
        public string PalletType { get; set; }

        [Column("UnloadingTime", TypeName = "smalldatetime")]
        public DateTime? UnloadingTime1 { get; set; }

        [Key]
        public bool Status { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastChangeDate { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }
    }
}
