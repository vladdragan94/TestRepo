//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MovieDictionary.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reviews
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reviews()
        {
            this.ReviewsLikes = new HashSet<ReviewsLikes>();
        }
    
        public int Id { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public System.DateTime DateAdded { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReviewsLikes> ReviewsLikes { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual Movies Movies { get; set; }
    }
}
