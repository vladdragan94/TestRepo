using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.Entities
{
    public class Profile
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public int Reputation { get; set; }

        public List<Badge> Badges { get; set; }

        public List<Notification> Notifications { get; set; }

        public List<Profile> Friends { get; set; }

        public List<Recommendation> Recommendations { get; set; }

        public bool IsFriend { get; set; }
    }
}
