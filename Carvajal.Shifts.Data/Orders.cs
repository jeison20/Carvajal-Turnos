namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orders()
        {
            OrdersProducts = new HashSet<OrdersProducts>();
        }

        [Key]
        public long PkIdentifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Merchant_Identifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Manufacturer_Identifier { get; set; }

        [StringLength(35)]
        public string OrderNumber { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? MaxDeliveryDate { get; set; }

        [StringLength(3)]
        public string OrderType { get; set; }

        public int? FkStatus_Identifier { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastChangeDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ProcessingDate { get; set; }

        public virtual Status Status { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdersProducts> OrdersProducts { get; set; }
    }
}
