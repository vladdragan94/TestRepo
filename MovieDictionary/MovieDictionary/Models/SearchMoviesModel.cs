using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieDictionary.Models
{
    public class SearchMoviesModel
    {
        public List<Entities.Movie> Movies { get; set; }

        public int NumberOfMovies { get; set; }

        public int PageNumber { get; set; }

        public bool CanFilter { get; set; }

        public string OrderType { get; set; }

        public List<string> Genres { get; set; }

        public string Title { get; set; }

        public string SelectedGenre { get; set; }

        public string Awards { get; set; }

        public int? YearStart { get; set; }

        public int? YearEnd { get; set; }
    }
}