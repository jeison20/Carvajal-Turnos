//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Carvajal.Shifts.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class AdvicesProducts
    {
        public long PkIdentifier { get; set; }
        public Nullable<long> FkAdvices_Identifier { get; set; }
        public string Code { get; set; }
        public Nullable<long> ReceivedAndAcceptedQuantity { get; set; }
    
        public virtual Advices Advices { get; set; }
    }
}
