//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PhonebookImportServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class phonebook
    {
        public int id { get; set; }
        public Nullable<int> sys_country_id { get; set; }
        public string number { get; set; }
        public string description { get; set; }
        public string company { get; set; }
        public string phone_type { get; set; }
        public bool @public { get; set; }
        public bool vip { get; set; }
    }
}