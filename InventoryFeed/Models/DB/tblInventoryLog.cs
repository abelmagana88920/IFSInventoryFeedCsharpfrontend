//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InventoryFeed.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblInventoryLog
    {
        public int il_id { get; set; }
        public Nullable<int> if_id { get; set; }
        public string sent { get; set; }
        public Nullable<System.DateTime> datetime_log { get; set; }
    }
}
