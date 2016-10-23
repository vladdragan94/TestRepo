using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.Entities
{
    public class Movie
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public string Runtime { get; set; }

        public DateTime? Released { get; set; }

        public string Genre { get; set; }

        public decimal? Rating { get; set; }

        public int Votes { get; set; }

        public string Poster { get; set; }

        public string Plot { get; set; }

        public string Director { get; set; }

        public string Writer { get; set; }

        public string Actors { get; set; }

        public string Awards { get; set; }

        public bool InCinema { get; set; }

        public bool ComingSoon { get; set; }

        public int? CurrentUserRating { get; set; }

        public bool ReviewedByCurrentUser { get; set; }

        public bool IsInWatchlist { get; set; }

        public List<Review> Reviews { get; set; }

        public int NumberOfReviews { get; set; }

        public string Trailer { get; set; }

        public string imdbID { get; set; }
    }
}
