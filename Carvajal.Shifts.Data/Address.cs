namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class Address
    {
        [StringLength(35)]
        public string FkUsers_Identifier { get; set; }

        [StringLength(70)]
        public string AddressStreet { get; set; }

        [StringLength(70)]
        public string AddressNumber { get; set; }

        [StringLength(9)]
        public string PostCode { get; set; }

        [StringLength(35)]
        public string Town { get; set; }

        [StringLength(9)]
        public string Region { get; set; }

        [StringLength(35)]
        public string Country { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FkCountries_Identifier { get; set; }

        public virtual Countries Countries { get; set; }

        public virtual Users Users { get; set; }
    }
}
