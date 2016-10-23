using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.IRepositories
{
    public interface INotifications
    {
        List<Entities.Notification> GetUserNotifications(string userId);

        void MarkNotifcationAsSeen(int notificationId);

        void AddNotification(string userId, string type, string movieId = null, int? badgeId = null, int? postId = null);

        void AddNotification(List<string> usersIds, string type, string movieId = null, int? badgeId = null, int? postId = null);

        void DeleteOldNotifications();

        DateTime GetLastNotificationsCheckDate();
    }
}
