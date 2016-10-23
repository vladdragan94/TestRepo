using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public bool Seen { get; set; }

        public string NotificationType { get; set; }

        public DateTime DateAdded { get; set; }

        public string MovieId { get; set; }

        public string MovieName { get; set; }

        public int? BadgeId { get; set; }

        public int? ReviewId { get; set; }

        public int? PostId { get; set; }
    }
}
