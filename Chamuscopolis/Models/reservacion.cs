//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chamuscopolis.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class reservacion
    {
        public int idRESERVACION { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public decimal monto { get; set; }
        public int CANCHA_idCANCHA { get; set; }
        public int USUARIO_idUSUARIO { get; set; }
    
        public virtual cancha cancha { get; set; }
        public virtual usuario usuario { get; set; }
    }
}
