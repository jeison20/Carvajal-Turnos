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
    
    public partial class Client
    {
        public long Id { get; set; }
        public string User { get; set; }
        public string Token { get; set; }
        public bool Active { get; set; }
        public System.DateTime RefreshTokenLifeTime { get; set; }
        public string AllowedOrigin { get; set; }
    }
}