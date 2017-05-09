namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PruebaComercio")]
    public partial class PruebaComercio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdComercio { get; set; }

        [StringLength(50)]
        public string NombreComercio { get; set; }
    }
}
