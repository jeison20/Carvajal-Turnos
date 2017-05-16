namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Turns
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Turns()
        {
            TurnsProducts = new HashSet<TurnsProducts>();
        }

        [Key]
        public long PkIdentifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Merchant_Identifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Manufacturer_Identifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Requester_Identifier { get; set; }

        [StringLength(35)]
        public string FkUsers_Modifier_Identifier { get; set; }

        [StringLength(35)]
        public string Orders_OrderNumber { get; set; }

        [StringLength(35)]
        public string ReceivingAdvice_ReceivingAdviceNumber { get; set; }

        public short? Dock { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? StartDateTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EndDateTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? FixedTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? VariableTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastChangeDate { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }

        public virtual Users Users2 { get; set; }

        public virtual Users Users3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TurnsProducts> TurnsProducts { get; set; }
    }
}
