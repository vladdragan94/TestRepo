using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string PosterName { get; set; }

        public DateTime DateAdded { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int? PostId { get; set; }

        public bool IsAnswer { get; set; }

        public bool IsQuestion { get; set; }

        public int Votes { get; set; }

        public int NumberOfComments { get; set; }

        public bool? LikedByCurrentUser { get; set; }

        public bool HasAnswer { get; set; }

        public List<Post> Replies { get; set; }
    }
}
