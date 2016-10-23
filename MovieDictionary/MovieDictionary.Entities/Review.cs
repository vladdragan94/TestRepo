using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public string MovieId { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int Rating { get; set; }

        public DateTime DateAdded { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int NumberOfLikes { get; set; }

        public int NumberOfDislikes { get; set; }

        public bool? RatedByCurrentUser { get; set; }
    }
}
