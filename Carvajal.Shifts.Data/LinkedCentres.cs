namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LinkedCentres
    {
        public long? FkUsers_Identifier { get; set; }

        public long? FkCentres_Identifier { get; set; }

        [Key]
        public bool IsResponsableForCentre { get; set; }

        public virtual Centres Centres { get; set; }

        public virtual Users Users { get; set; }
    }
}
