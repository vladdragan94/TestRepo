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
    
    public partial class ForumPosts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ForumPosts()
        {
            this.ForumPosts1 = new HashSet<ForumPosts>();
            this.PostsLikes = new HashSet<PostsLikes>();
            this.UsersNotifications = new HashSet<UsersNotifications>();
        }
    
        public int Id { get; set; }
        public string UserId { get; set; }
        public System.DateTime DateAdded { get; set; }
        public string Content { get; set; }
        public Nullable<int> PostId { get; set; }
        public bool IsAnswer { get; set; }
        public bool IsQuestion { get; set; }
        public int Votes { get; set; }
        public string Title { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForumPosts> ForumPosts1 { get; set; }
        public virtual ForumPosts ForumPosts2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PostsLikes> PostsLikes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersNotifications> UsersNotifications { get; set; }
    }
}
