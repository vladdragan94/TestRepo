using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.IRepositories;

namespace MovieDictionary.DAL
{
    public class UsersRepository : IUsers
    {
        public Entities.Profile GetProfile(string userId, string currentUserId = null)
        {
            using (var dataContex = new MoviesEntities())
            {
                var user = dataContex.AspNetUsers.Find(userId);

                if (user == null)
                    return null;

                return new Entities.Profile()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Reputation = user.Reputation,
                    IsFriend = currentUserId != null ? user.UsersFriendships1.Any(item => item.AspNetUsers.Id == currentUserId) : false
                };
            }
        }

        public List<Entities.Badge> GetUserBadges(string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.UsersBadges.Where(item => item.UserId == userId).Select(item => item.Badges).Select(item => new Entities.Badge()
                {
                    Name = item.Name,
                    Description = item.Description
                }).ToList();
            }
        }

        public List<Entities.Profile> GetFriends(string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.UsersFriendships.Where(item => item.FirstUser == userId).Select(item => item.AspNetUsers1).Select(item => new Entities.Profile()
                {
                    UserId = item.Id,
                    UserName = item.UserName
                }).ToList();
            }
        }

        public List<Entities.Profile> GetUsers(string searchTerm, string userId = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.AspNetUsers.Where(item => item.UserName.Contains(searchTerm)).Select(item => new Entities.Profile()
                {
                    UserId = item.Id,
                    UserName = item.UserName
                }).ToList();
            }
        }

        public void AddFriend(string firstUserId, string secondUserId)
        {
            using (var dataContext = new MoviesEntities())
            {
                var friendship = dataContext.UsersFriendships.FirstOrDefault(item => item.FirstUser == firstUserId && item.SecondUser == secondUserId);

                if (friendship != null)
                    return;

                friendship = new UsersFriendships()
                {
                    FirstUser = firstUserId,
                    SecondUser = secondUserId
                };

                dataContext.UsersFriendships.Add(friendship);
                dataContext.SaveChanges();
            }
        }

        public void RemoveFriend(string firstUserId, string secondUserId)
        {
            using (var dataContext = new MoviesEntities())
            {
                var friendship = dataContext.UsersFriendships.FirstOrDefault(item => item.FirstUser == firstUserId && item.SecondUser == secondUserId);

                if (friendship == null)
                    return;

                dataContext.UsersFriendships.Remove(friendship);
                dataContext.SaveChanges();
            }
        }

        public void RecommendMovie(string currentUserId, string movieId, List<string> friends)
        {
            using (var dataContext = new MoviesEntities())
            {
                foreach (var userId in friends)
                {
                    var recommendation = dataContext.UsersRecommendations.FirstOrDefault(item => item.SenderId == currentUserId && item.ReceiverId == userId && item.MovieId == movieId);

                    if (recommendation != null)
                        continue;

                    recommendation = new UsersRecommendations()
                    {
                        SenderId = currentUserId,
                        ReceiverId = userId,
                        MovieId = movieId
                    };

                    var user = dataContext.AspNetUsers.Find(userId);

                    if (user.UsersRecommendations1.Any(item => item.MovieId == movieId && item.Liked != null))
                    {
                        var liked = user.UsersRecommendations1.FirstOrDefault(item => item.MovieId == movieId).Liked;
                        recommendation.Liked = liked;
                    }

                    dataContext.UsersRecommendations.Add(recommendation);
                }

                dataContext.SaveChanges();

                CreateMovieRecommendedTask(friends, movieId);
            }
        }

        public void LikeRecommendation(string userId, string movieId)
        {
            var friendsIds = new List<string>();

            using (var dataContext = new MoviesEntities())
            {
                var recommendations = dataContext.UsersRecommendations.Where(item => item.ReceiverId == userId && item.MovieId == movieId);

                if (!recommendations.Any())
                    return;

                foreach (var item in recommendations)
                {
                    if (item.Liked != true)
                        friendsIds.Add(item.AspNetUsers.Id);

                    item.Liked = true;
                }

                dataContext.SaveChanges();

                CreateRecommendationLikedTask(friendsIds, movieId);
            }
        }

        public void DislikeRecommendation(string userId, string movieId)
        {
            var friendsIds = new List<string>();

            using (var dataContext = new MoviesEntities())
            {
                var recommendations = dataContext.UsersRecommendations.Where(item => item.ReceiverId == userId && item.MovieId == movieId);

                if (!recommendations.Any())
                    return;

                foreach (var item in recommendations)
                {
                    if (item.Liked != false)
                        friendsIds.Add(item.AspNetUsers.Id);

                    item.Liked = false;
                }

                dataContext.SaveChanges();

                CreateRecommendationDislikedTask(friendsIds);
            }
        }

        public List<Entities.Profile> GetFriendsForRecommendation(string userId, string movieId)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.UsersFriendships.Where(item => item.FirstUser == userId && !item.AspNetUsers1.UsersRecommendations1.Any(el => el.MovieId == movieId && el.SenderId == userId)).Select(item => item.AspNetUsers1).Select(item => new Entities.Profile()
                {
                    UserId = item.Id,
                    UserName = item.UserName
                }).ToList();
            }
        }

        public List<Entities.Recommendation> GetRecommendations(string userId, bool friendsOnly = false)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.UsersRecommendations.Where(item => item.ReceiverId == userId && (friendsOnly ? item.AspNetUsers.UsersFriendships1.Any(el => el.FirstUser == userId) : true)).Select(item => new Entities.Recommendation()
                {
                    MovieId = item.MovieId,
                    MovieName = item.Movies.Title,
                    Liked = item.Liked
                }).ToList();
            }
        }

        public List<Entities.Profile> GetFriendsWhoRecommendedMovie(string userId, string movieId)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.UsersRecommendations.Where(item => item.MovieId == movieId && item.ReceiverId == userId).Select(item => item.AspNetUsers).Select(item => new Entities.Profile()
                {
                    UserId = item.Id,
                    UserName = item.UserName
                }).ToList();
            }
        }

        public List<Entities.Profile> GetTopUsers()
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.AspNetUsers
                    .OrderByDescending(item => item.Reputation)
                    .Take(Entities.Constants.ApplicationConfigurations.DefaultPageSize)
                    .Select(item => new Entities.Profile()
                    {
                        UserId = item.Id,
                        UserName = item.UserName,
                        Reputation = item.Reputation
                    }).ToList();
            }
        }

        public void AddReputation(string userId, int reputationToAdd)
        {
            using (var dataContext = new MoviesEntities())
            {
                var user = dataContext.AspNetUsers.Find(userId);

                if (user == null)
                    return;

                user.Reputation += reputationToAdd;

                dataContext.SaveChanges();
            }
        }

        public void AddReputation(List<string> usersIds, int reputationToAdd)
        {
            using (var dataContext = new MoviesEntities())
            {
                foreach (var userId in usersIds)
                {
                    var user = dataContext.AspNetUsers.Find(userId);

                    if (user == null)
                        return;

                    user.Reputation += reputationToAdd;
                }

                dataContext.SaveChanges();
            }
        }

        public Dictionary<string, List<string>> GetUsersToAlertForNewMovies(List<string> moviesIds)
        {
            using (var dataContext = new MoviesEntities())
            {
                var users = new Dictionary<string, List<string>>();

                foreach (var watchlist in dataContext.UsersWatchlists)
                {
                    var movieId = watchlist.MovieId;
                    var userId = watchlist.UserId;

                    if (moviesIds.Contains(movieId))
                    {
                        if (!users.ContainsKey(userId))
                        {
                            users.Add(userId, new List<string>());
                            users[userId].Add(movieId);
                        }
                        else
                            users[userId].Add(movieId);
                    }
                }

                return users;
            }
        }

        public void CheckForBadges(string userId, bool ratings, bool reviews, bool forum)
        {
            using (var dataContext = new MoviesEntities())
            {
                var user = dataContext.AspNetUsers.Find(userId);

                if (user == null)
                    return;

                if (ratings)
                {
                    foreach (var genre in Entities.Constants.BadgeTypes.GenresFan)
                    {
                        var ratingsCount = user.UsersRatings.Where(item => item.Movies.Genre.Contains(genre)).Count();

                        if (ratingsCount >= Entities.Constants.ApplicationConfigurations.BadgeAwardedAt)
                        {
                            var badge = dataContext.Badges.FirstOrDefault(item => item.Name == genre);

                            if (badge == null)
                            {
                                badge = new Badges();
                                badge.Name = genre;
                                dataContext.Badges.Add(badge);
                                dataContext.SaveChanges();
                            }

                            var badgeId = badge.Id;

                            var userBadge = dataContext.UsersBadges.FirstOrDefault(item => item.BadgeId == badgeId && item.UserId == userId);

                            if (userBadge == null)
                            {
                                userBadge = new UsersBadges()
                                {
                                    UserId = userId,
                                    BadgeId = badgeId
                                };
                                dataContext.UsersBadges.Add(userBadge);
                            }
                        }
                    }
                }

                if (reviews)
                {
                    var reviewsLikes = user.Reviews.SelectMany(item => item.ReviewsLikes);
                    var likesCount = reviewsLikes.Where(item => item.Liked == true).Count();
                    var dislikesCount = reviewsLikes.Where(item => item.Liked == false).Count();

                    if (likesCount - dislikesCount >= Entities.Constants.ApplicationConfigurations.BadgeAwardedAt)
                    {
                        var badge = dataContext.Badges.FirstOrDefault(item => item.Name == Entities.Constants.BadgeTypes.TopReviewer);

                        if (badge == null)
                        {
                            badge = new Badges();
                            badge.Name = Entities.Constants.BadgeTypes.TopReviewer;
                            dataContext.Badges.Add(badge);
                            dataContext.SaveChanges();
                        }

                        var badgeId = badge.Id;

                        var userBadge = dataContext.UsersBadges.FirstOrDefault(item => item.BadgeId == badgeId && item.UserId == userId);

                        if (userBadge == null)
                        {
                            userBadge = new UsersBadges()
                            {
                                UserId = userId,
                                BadgeId = badgeId
                            };
                            dataContext.UsersBadges.Add(userBadge);
                        }
                    }
                }

                if (forum)
                {
                    var forumLikes = user.ForumPosts.SelectMany(item => item.PostsLikes);
                    var likesCount = forumLikes.Where(item => item.Liked == true).Count();
                    var dislikesCount = forumLikes.Where(item => item.Liked == false).Count();
                    var answersCount = user.ForumPosts.Where(item => item.IsAnswer == true).Count();

                    if (likesCount + answersCount - dislikesCount >= Entities.Constants.ApplicationConfigurations.BadgeAwardedAt)
                    {
                        var badge = dataContext.Badges.FirstOrDefault(item => item.Name == Entities.Constants.BadgeTypes.CommunityHelper);

                        if (badge == null)
                        {
                            badge = new Badges();
                            badge.Name = Entities.Constants.BadgeTypes.CommunityHelper;
                            dataContext.Badges.Add(badge);
                            dataContext.SaveChanges();
                        }

                        var badgeId = badge.Id;

                        var userBadge = dataContext.UsersBadges.FirstOrDefault(item => item.BadgeId == badgeId && item.UserId == userId);

                        if (userBadge == null)
                        {
                            userBadge = new UsersBadges()
                            {
                                UserId = userId,
                                BadgeId = badgeId
                            };
                            dataContext.UsersBadges.Add(userBadge);
                        }
                    }
                }

                dataContext.SaveChanges();
            }
        }

        public bool IsAdmin(string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                var isAdmin = dataContext.AspNetUsers.Find(userId).AspNetRoles.FirstOrDefault(item => item.Name == Entities.Constants.UserRoles.Admin) != null;

                return isAdmin;
            }
        }

        private void CreateRecommendationLikedTask(List<string> friendsIds, string movieId)
        {
            var task = new Task(() =>
            {
                var usersRepository = new UsersRepository();
                var notificationsRepository = new NotificationsRepository();

                usersRepository.AddReputation(friendsIds, Entities.Constants.ReputationAwards.RecommendationLiked);
                notificationsRepository.AddNotification(friendsIds, Entities.Constants.NotificationTypes.RecommendationLiked, movieId);
            });
            task.Start();
        }

        private void CreateRecommendationDislikedTask(List<string> friendsIds)
        {
            var task = new Task(() =>
            {
                var usersRepository = new UsersRepository();

                usersRepository.AddReputation(friendsIds, Entities.Constants.ReputationAwards.RecommendationDisliked);
            });
            task.Start();
        }

        private void CreateMovieRecommendedTask(List<string> friends, string movieId)
        {
            var task = new Task(() =>
            {
                var notificationsRepository = new NotificationsRepository();

                notificationsRepository.AddNotification(friends, Entities.Constants.NotificationTypes.NewRecommendation, movieId);
            });
            task.Start();
        }
    }
}
