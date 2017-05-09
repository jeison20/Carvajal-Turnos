namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PkIdentifier { get; set; }

        [Required]
        [StringLength(35)]
        public string Name { get; set; }
    }
}
