using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.IRepositories
{
    public interface IUsers
    {
        Entities.Profile GetProfile(string userId, string currentUserId = null);

        List<Entities.Profile> GetTopUsers();

        List<Entities.Badge> GetUserBadges(string userId);

        List<Entities.Profile> GetFriends(string userId);

        List<Entities.Profile> GetUsers(string searchTerm, string userId = null);

        void AddFriend(string firstUserId, string secondUserId);

        void RemoveFriend(string firstUserId, string secondUserId);

        void RecommendMovie(string currentUserId, string movieId, List<string> friends);

        void LikeRecommendation(string userId, string movieId);

        void DislikeRecommendation(string userId, string movieId);

        List<Entities.Profile> GetFriendsForRecommendation(string userId, string movieId);

        List<Entities.Recommendation> GetRecommendations(string userId, bool friendsOnly = false);

        List<Entities.Profile> GetFriendsWhoRecommendedMovie(string userId, string movieId);

        void AddReputation(string userId, int reputationToAdd);

        void AddReputation(List<string> usersIds, int reputationToAdd);

        Dictionary<string, List<string>> GetUsersToAlertForNewMovies(List<string> moviesIds);

        bool IsAdmin(string userId);

        void CheckForBadges(string userId, bool ratings, bool reviews, bool forum);
    }
}
