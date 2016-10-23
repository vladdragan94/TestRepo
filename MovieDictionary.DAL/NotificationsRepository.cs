using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.IRepositories;
using System.Globalization;

namespace MovieDictionary.DAL
{
    public class NotificationsRepository : INotifications
    {
        public List<Entities.Notification> GetUserNotifications(string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.UsersNotifications.Where(item => item.UserId == userId).OrderBy(item => item.Seen).Select(item => new Entities.Notification
                {
                    Id = item.Id,
                    UserId = userId,
                    NotificationType = item.NotificationTypes.Name,
                    DateAdded = item.DateAdded,
                    BadgeId = item.BadgeId,
                    MovieId = item.MovieId,
                    MovieName = item.MovieId != null ? item.Movies.Title : null,
                    PostId = item.PostId,
                    Seen = item.Seen
                }).ToList();
            }
        }

        public void MarkNotifcationAsSeen(int notificationId)
        {
            using (var dataContext = new MoviesEntities())
            {
                var notification = dataContext.UsersNotifications.Find(notificationId);

                if (notification == null)
                    return;

                notification.Seen = true;

                dataContext.SaveChanges();
            }
        }

        public void AddNotification(string userId, string type, string movieId = null, int? badgeId = null, int? postId = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(type))
                    return;

                var notificationType = dataContext.NotificationTypes.FirstOrDefault(item => item.Name == type);

                if (notificationType == null)
                {
                    notificationType = new NotificationTypes();
                    notificationType.Name = type;
                    dataContext.NotificationTypes.Add(notificationType);
                    dataContext.SaveChanges();
                }

                UsersNotifications notification = null;
                
                switch (type)
                {
                    case Entities.Constants.NotificationTypes.BadgeAwarded:
                        notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.BadgeId == badgeId.Value);
                        break;
                    case Entities.Constants.NotificationTypes.NewRecommendation:
                        notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.MovieId == movieId);
                        break;
                    case Entities.Constants.NotificationTypes.ReviewVoted:
                        notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.MovieId == movieId);
                        break;
                    case Entities.Constants.NotificationTypes.PostVoted:
                        notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.PostId == postId.Value);
                        break;
                    case Entities.Constants.NotificationTypes.MovieInCinema:
                        notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.MovieId == movieId);
                        break;
                    default:
                        return;
                }

                if (notification != null)
                    return;

                var notificationTypeId = notificationType.Id;
                var dateAdded = DateTime.Now;

                notification = new UsersNotifications()
                {
                    MovieId = movieId,
                    BadgeId = badgeId,
                    PostId = postId,
                    DateAdded = dateAdded,
                    NotificationTypeId = notificationTypeId,
                    UserId = userId,
                    Seen = false
                };

                dataContext.UsersNotifications.Add(notification);
                dataContext.SaveChanges();
            }
        }

        public void AddNotification(List<string> usersIds, string type, string movieId = null, int? badgeId = null, int? postId = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                if (string.IsNullOrEmpty(type) || usersIds == null || usersIds.Count == 0)
                    return;

                var notificationType = dataContext.NotificationTypes.FirstOrDefault(item => item.Name == type);

                if (notificationType == null)
                {
                    notificationType = new NotificationTypes();
                    notificationType.Name = type;
                    dataContext.NotificationTypes.Add(notificationType);
                    dataContext.SaveChanges();
                }

                foreach (var userId in usersIds)
                {
                    if (string.IsNullOrEmpty(userId))
                        continue;

                    UsersNotifications notification = null;

                    switch (type)
                    {
                        case Entities.Constants.NotificationTypes.BadgeAwarded:
                            notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.BadgeId == badgeId.Value);
                            break;
                        case Entities.Constants.NotificationTypes.NewRecommendation:
                            notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.MovieId == movieId);
                            break;
                        case Entities.Constants.NotificationTypes.ReviewVoted:
                            notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.MovieId == movieId);
                            break;
                        case Entities.Constants.NotificationTypes.PostVoted:
                            notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.PostId == postId.Value);
                            break;
                        case Entities.Constants.NotificationTypes.MovieInCinema:
                            notification = dataContext.UsersNotifications.FirstOrDefault(item => item.NotificationTypes.Name == type && item.UserId == userId && item.MovieId == movieId);
                            break;
                        default:
                            return;
                    }

                    if (notification != null)
                        return;

                    var notificationTypeId = notificationType.Id;
                    var dateAdded = DateTime.Now;

                    notification = new UsersNotifications()
                    {
                        MovieId = movieId,
                        BadgeId = badgeId,
                        PostId = postId,
                        DateAdded = dateAdded,
                        NotificationTypeId = notificationTypeId,
                        UserId = userId,
                        Seen = false
                    };

                    dataContext.UsersNotifications.Add(notification);
                }

                dataContext.SaveChanges();
            }
        }

        public void DeleteOldNotifications()
        {
            using (var dataContext = new MoviesEntities())
            {
                var oldDate = DateTime.Now.AddDays((-1) * Entities.Constants.ApplicationConfigurations.DefaultNotificationsDurationInDays);
                var notifications = dataContext.UsersNotifications.Where(item => item.DateAdded < oldDate);
                dataContext.UsersNotifications.RemoveRange(notifications);
                dataContext.SaveChanges();

                var lastCheckDate = dataContext.ApplicationConfigurations.FirstOrDefault(item => item.Name == Entities.Constants.ApplicationConfigurations.LastNotificationCheckDate);

                if (lastCheckDate == null)
                {
                    lastCheckDate = new ApplicationConfigurations()
                    {
                        Name = Entities.Constants.ApplicationConfigurations.LastNotificationCheckDate
                    };
                }

                lastCheckDate.Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                dataContext.SaveChanges();
            }
        }

        public DateTime GetLastNotificationsCheckDate()
        {
            using (var dataContext = new MoviesEntities())
            {
                var lastNotificationCheckDate = dataContext.ApplicationConfigurations.FirstOrDefault(item => item.Name == Entities.Constants.ApplicationConfigurations.LastNotificationCheckDate);

                if (lastNotificationCheckDate == null)
                {
                    lastNotificationCheckDate = new ApplicationConfigurations()
                    {
                        Name = Entities.Constants.ApplicationConfigurations.LastNotificationCheckDate,
                        Value = DateTime.Now.AddDays((-1) * Entities.Constants.ApplicationConfigurations.DefaultNotificationsDurationInDays).ToString(CultureInfo.InvariantCulture)
                    };

                    dataContext.ApplicationConfigurations.Add(lastNotificationCheckDate);
                    dataContext.SaveChanges();
                }

                var parsedDate = lastNotificationCheckDate.Value.ToString(CultureInfo.InvariantCulture);

                return Convert.ToDateTime(parsedDate, CultureInfo.InvariantCulture);
            }
        }
    }
}
