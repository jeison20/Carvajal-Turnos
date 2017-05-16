namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Advices
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Advices()
        {
            AdvicesProducts = new HashSet<AdvicesProducts>();
        }

        [Key]
        public long PkIdentifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Merchant_Identifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Manufacturer_Identifier { get; set; }

        [StringLength(35)]
        public string AdviceNumber { get; set; }

        public bool ManualAdvise { get; set; }

        [StringLength(35)]
        public string Orders_OrderNumber { get; set; }

        [StringLength(35)]
        public string FkCentres_Identifier { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ReceiptDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ProcessingDate { get; set; }

        public virtual Centres Centres { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdvicesProducts> AdvicesProducts { get; set; }
    }
}
