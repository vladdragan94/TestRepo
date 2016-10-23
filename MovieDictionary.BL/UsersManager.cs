using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.IRepositories;

namespace MovieDictionary.BL
{
    public class UsersManager
    {
        private IUsers repository;

        private INotifications notificationsRepository;

        public UsersManager()
        {
            this.repository = new DAL.UsersRepository() as IUsers;
            this.notificationsRepository = new DAL.NotificationsRepository() as INotifications;
        }

        public UsersManager(IUsers usersRepository, INotifications notificationsRepository)
        {
            this.repository = usersRepository;
            this.notificationsRepository = notificationsRepository;
        }

        public Entities.Profile GetUserProfile(string userId, string currentUserId = null)
        {
            var profile = repository.GetProfile(userId, currentUserId);
            profile.Badges = repository.GetUserBadges(userId);

            if (userId == currentUserId)
            {
                var recommendations = repository.GetRecommendations(userId);
                var groups = recommendations.GroupBy(item => item.MovieId);
                profile.Recommendations = repository.GetRecommendations(userId).GroupBy(item => item.MovieId).Select(item => new Entities.Recommendation()
                {
                    MovieId = item.Key,
                    MovieName = item.First().MovieName,
                    NumberOfUsers = item.Count()
                }).ToList();
                profile.Notifications = notificationsRepository.GetUserNotifications(userId);
                profile.Friends = repository.GetFriends(userId);

                var task = new Task(() =>
                {
                    var lastCheckDate = notificationsRepository.GetLastNotificationsCheckDate();

                    if (lastCheckDate < DateTime.Now.AddDays((-1) * Entities.Constants.ApplicationConfigurations.DefaultNotificationsDurationInDays))
                        notificationsRepository.DeleteOldNotifications();
                });
                task.Start();
            }

            return profile;
        }

        public List<Entities.Profile> GetFriends(string userId)
        {
            return repository.GetFriends(userId);
        }

        public List<Entities.Profile> GetUsers(string searchTerm, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return null;

            return repository.GetUsers(searchTerm, userId);
        }

        public void AddFriend(string firstUserId, string secondUserId)
        {
            repository.AddFriend(firstUserId, secondUserId);
        }

        public void RemoveFriend(string firstUserId, string secondUserId)
        {
            repository.RemoveFriend(firstUserId, secondUserId);
        }

        public void RecommendMovie(string currentUserId, string movieId, List<string> friends)
        {
            if (friends.Count == 0)
                return;

            repository.RecommendMovie(currentUserId, movieId, friends);
        }

        public void LikeRecommendation(string userId, string movieId)
        {
            repository.LikeRecommendation(userId, movieId);

            var task = new Task(() =>
            {
                var friendsIds = repository.GetFriendsWhoRecommendedMovie(userId, movieId).Select(item => item.UserId).ToList();
                repository.AddReputation(friendsIds, Entities.Constants.ReputationAwards.RecommendationLiked);
            });
            task.Start();
        }

        public void DislikeRecommendation(string userId, string movieId)
        {
            repository.DislikeRecommendation(userId, movieId);

            var task = new Task(() =>
            {
                var friendsIds = repository.GetFriendsWhoRecommendedMovie(userId, movieId).Select(item => item.UserId).ToList();
                repository.AddReputation(friendsIds, Entities.Constants.ReputationAwards.RecommendationDisliked);
            });
            task.Start();
        }

        public List<Entities.Profile> GetFriendsForRecommendation(string userId, string movieId)
        {
            return repository.GetFriendsForRecommendation(userId, movieId);
        }

        public List<Entities.Recommendation> GetRecommendations(string userId, bool friendsOnly = false)
        {
            return repository.GetRecommendations(userId, friendsOnly);
        }

        public List<Entities.Profile> GetFriendsWhoRecommendedMovie(string userId, string movieId)
        {
            return repository.GetFriendsWhoRecommendedMovie(userId, movieId);
        }

        public List<Entities.Profile> GetTopUsers()
        {
            return repository.GetTopUsers();
        }

        public void MarkNotificationAsSeen(int notificationId)
        {
            notificationsRepository.MarkNotifcationAsSeen(notificationId);
        }
    }
}
