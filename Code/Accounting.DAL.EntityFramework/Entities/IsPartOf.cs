//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Accounting.DAL.EntityFramework.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class IsPartOf
    {
        public long ParentID { get; set; }
        public long ChildID { get; set; }
        public decimal Count { get; set; }
    
        public virtual Product ParentProduct { get; set; }
        public virtual Product ChildProduct { get; set; }
    }
}
