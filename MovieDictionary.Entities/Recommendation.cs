using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.Entities
{
    public class Recommendation
    {
        public string MovieId { get; set; }

        public string MovieName { get; set; }

        public bool? Liked { get; set; }

        public int NumberOfUsers { get; set; }
    }
}
