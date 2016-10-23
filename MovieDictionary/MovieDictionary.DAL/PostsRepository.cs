using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.IRepositories;

namespace MovieDictionary.DAL
{
    public class PostsRepository : IPosts
    {
        public List<Entities.Post> GetQuestions(string userId = null, string orderType = null, int pageNumber = 0, int pageSize = Entities.Constants.ApplicationConfigurations.DefaultPageSize)
        {
            using (var dataContext = new MoviesEntities())
            {
                orderType = orderType ?? Entities.Constants.OrderTypes.Votes;

                Func<ForumPosts, object> orderBy = null;

                if (orderType == Entities.Constants.OrderTypes.Votes)
                    orderBy = item => item.Votes;
                else if (orderType == Entities.Constants.OrderTypes.Date)
                    orderBy = item => item.DateAdded;

                return dataContext.ForumPosts
                    .Where(item => item.IsQuestion == true && item.PostId == null)
                    .OrderByDescending(orderBy)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(item => new Entities.Post()
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        PosterName = item.AspNetUsers.UserName,
                        DateAdded = item.DateAdded,
                        Title = item.Title,
                        Content = item.Content,
                        IsQuestion = true,
                        Votes = item.Votes,
                        LikedByCurrentUser = userId != null ? (item.PostsLikes.Any(el => el.UserId == userId) ? (bool?)item.PostsLikes.FirstOrDefault(el => el.UserId == userId).Liked : null) : null,
                        HasAnswer = item.ForumPosts1.Any(el => el.IsAnswer == true),
                        NumberOfComments = item.ForumPosts1.Count
                    }).ToList();
            }
        }

        public List<Entities.Post> GetPosts(string userId = null, string orderType = null, int pageNumber = 0, int pageSize = Entities.Constants.ApplicationConfigurations.DefaultPageSize)
        {
            using (var dataContext = new MoviesEntities())
            {
                orderType = orderType ?? Entities.Constants.OrderTypes.Votes;

                Func<ForumPosts, object> orderBy = null;

                if (orderType == Entities.Constants.OrderTypes.Votes)
                    orderBy = item => item.Votes;
                else if (orderType == Entities.Constants.OrderTypes.Date)
                    orderBy = item => item.DateAdded;

                return dataContext.ForumPosts
                    .Where(item => item.IsQuestion == false && item.PostId == null)
                    .OrderByDescending(orderBy)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(item => new Entities.Post()
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        PosterName = item.AspNetUsers.UserName,
                        DateAdded = item.DateAdded,
                        Title = item.Title,
                        Content = item.Content,
                        IsQuestion = false,
                        Votes = item.Votes,
                        LikedByCurrentUser = userId != null ? (item.PostsLikes.Any(el => el.UserId == userId) ? (bool?)item.PostsLikes.FirstOrDefault(el => el.UserId == userId).Liked : null) : null,
                        NumberOfComments = item.ForumPosts1.Count
                    }).ToList();
            }
        }

        public Entities.Post GetPost(int postId, string userId = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                var post = dataContext.ForumPosts.Find(postId);

                if (post == null)
                    return null;

                return new Entities.Post()
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    PosterName = post.AspNetUsers.UserName,
                    DateAdded = post.DateAdded,
                    Title = post.Title,
                    Content = post.Content,
                    Votes = post.Votes,
                    IsQuestion = post.IsQuestion,
                    HasAnswer = post.ForumPosts1.Any(el => el.IsAnswer == true),
                    NumberOfComments = post.ForumPosts1.Count,
                    LikedByCurrentUser = userId != null ? (post.PostsLikes.Any(el => el.UserId == userId) ? (bool?)post.PostsLikes.FirstOrDefault(el => el.UserId == userId).Liked : null) : null
                };
            }
        }

        public List<Entities.Post> GetReplies(int postId, string userId = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.ForumPosts.Where(item => item.PostId == postId)
                    .OrderByDescending(item => item.IsAnswer).ThenByDescending(item => item.DateAdded).ThenByDescending(item => item.Votes)
                    .Select(item => new Entities.Post()
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        PosterName = item.AspNetUsers.UserName,
                        DateAdded = item.DateAdded,
                        Title = item.Title,
                        Content = item.Content,
                        IsAnswer = item.IsAnswer,
                        Votes = item.Votes,
                        LikedByCurrentUser = userId != null ? (item.PostsLikes.Any(el => el.UserId == userId) ? (bool?)item.PostsLikes.FirstOrDefault(el => el.UserId == userId).Liked : null) : null
                    }).ToList();
            }
        }

        public int AddPost(Entities.Post forumPost)
        {
            using (var dataContext = new MoviesEntities())
            {
                var post = new ForumPosts()
                {
                    UserId = forumPost.UserId,
                    Title = forumPost.Title,
                    Content = forumPost.Content,
                    DateAdded = forumPost.DateAdded,
                    PostId = forumPost.PostId,
                    IsQuestion = forumPost.IsQuestion
                };

                dataContext.ForumPosts.Add(post);
                dataContext.SaveChanges();

                return post.Id;
            }
        }

        public void LikePost(int postId, string userId, bool liked)
        {
            string postCreatorId = null;
            bool? addReputation = null;
            bool? addDoubleReputation = null;

            using (var dataContext = new MoviesEntities())
            {
                var like = dataContext.PostsLikes.FirstOrDefault(item => item.PostId == postId && item.UserId == userId);
                var post = dataContext.ForumPosts.Find(postId);

                if (post == null)
                    return;

                if (like == null)
                {
                    like = new PostsLikes();
                    like.PostId = postId;
                    like.UserId = userId;
                    dataContext.PostsLikes.Add(like);
                    addReputation = liked;

                    if (liked)
                        post.Votes++;
                    else
                        post.Votes--;
                }
                else
                {
                    if (like.Liked && !liked)
                    {
                        post.Votes -= 2;
                        addDoubleReputation = false;
                    }
                    else if (!like.Liked && liked)
                    {
                        post.Votes += 2;
                        addDoubleReputation = true;
                    }
                }

                like.Liked = liked;

                dataContext.SaveChanges();

                postCreatorId = dataContext.ForumPosts.Find(postId).AspNetUsers.Id;

                CreatePostLikedTask(addReputation, addDoubleReputation, userId, postCreatorId, postId);
            }
        }

        public void UnlikePost(int postId, string userId)
        {
            string postCreatorId = null;
            bool? addReputation = null;

            using (var dataContext = new MoviesEntities())
            {
                var like = dataContext.PostsLikes.FirstOrDefault(item => item.PostId == postId && item.UserId == userId);

                if (like == null)
                    return;

                var post = dataContext.ForumPosts.Find(postId);

                if (post == null)
                    return;

                postCreatorId = post.AspNetUsers.Id;

                if (like.Liked)
                {
                    post.Votes--;
                    addReputation = true;
                }
                else
                {
                    post.Votes++;
                    addReputation = false;
                }

                dataContext.PostsLikes.Remove(like);
                dataContext.SaveChanges();
            }

            CreatePostUnlikedTask(addReputation, userId, postCreatorId);
        }

        public bool MarkAsAnswer(int postId, string userId)
        {
            string answerCreatorId = null;
            string previousAnswerCreatorId = null;

            using (var dataContext = new MoviesEntities())
            {
                var post = dataContext.ForumPosts.Find(postId);

                if (post == null || post.PostId == null)
                    return false;

                var question = post.ForumPosts2;

                if (!question.IsQuestion || question.UserId != userId || post.UserId == userId)
                    return false;

                answerCreatorId = post.AspNetUsers.Id;

                var previousAnswer = question.ForumPosts1.FirstOrDefault(item => item.IsAnswer == true);

                if (previousAnswer != null)
                {
                    previousAnswer.IsAnswer = false;
                    previousAnswerCreatorId = previousAnswer.UserId;
                }

                post.IsAnswer = true;

                dataContext.SaveChanges();
            }

            CreateAnswerMarkedTask(userId, previousAnswerCreatorId, answerCreatorId, postId);

            return true;
        }

        public bool UnmarkAnswer(int postId, string userId)
        {
            string answerCreatorId = null;

            using (var dataContext = new MoviesEntities())
            {
                var post = dataContext.ForumPosts.Find(postId);

                if (post.PostId == null)
                    return false;

                var question = post.ForumPosts2;

                if (!question.IsQuestion || question.UserId != userId)
                    return false;

                answerCreatorId = post.AspNetUsers.Id;

                post.IsAnswer = false;

                dataContext.SaveChanges();
            }

            CreateAnswerUnmarkedTask(answerCreatorId);

            return true;
        }

        public bool DeletePost(int postId, string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                bool isAdmin = dataContext.AspNetUsers.Find(userId).AspNetRoles.FirstOrDefault(item => item.Name == Entities.Constants.UserRoles.Admin) != null;

                if (!isAdmin)
                    return false;

                var post = dataContext.ForumPosts.Find(postId);

                if (post == null)
                    return true;

                dataContext.PostsLikes.RemoveRange(post.PostsLikes);
                dataContext.PostsLikes.RemoveRange(post.ForumPosts1.SelectMany(item => item.PostsLikes));
                dataContext.ForumPosts.RemoveRange(post.ForumPosts1);
                dataContext.ForumPosts.Remove(post);
                dataContext.SaveChanges();

                return true;
            }
        }

        public Entities.ForumStats GetForumStats()
        {
            using (var dataContext = new MoviesEntities())
            {
                return new Entities.ForumStats()
                {
                    NumberOfQuestions = dataContext.ForumPosts.Where(item => item.IsQuestion && item.PostId == null).Count(),
                    NumberOfPosts = dataContext.ForumPosts.Where(item => !item.IsQuestion && item.PostId == null).Count()
                };
            }
        }

        private void CreatePostLikedTask(bool? addReputation, bool? addDoubleReputation, string userId, string postCreatorId, int postId)
        {
            if (userId == postCreatorId)
                return;

            var task = new Task(() =>
            {
                var usersRepository = new UsersRepository();

                if (addReputation.HasValue)
                {
                    if (addReputation.Value)
                        usersRepository.AddReputation(postCreatorId, Entities.Constants.ReputationAwards.PostUpvoted);
                    else
                        usersRepository.AddReputation(postCreatorId, Entities.Constants.ReputationAwards.PostDownvoted);
                }
                else if (addDoubleReputation.HasValue)
                {
                    if (addDoubleReputation.Value)
                        usersRepository.AddReputation(postCreatorId, 2 * Entities.Constants.ReputationAwards.PostUpvoted);
                    else
                        usersRepository.AddReputation(postCreatorId, 2 * Entities.Constants.ReputationAwards.PostDownvoted);
                }

                var notificationsRepository = new NotificationsRepository();
                notificationsRepository.AddNotification(postCreatorId, Entities.Constants.NotificationTypes.PostVoted, postId: postId);

                usersRepository.CheckForBadges(postCreatorId, false, false, true);
            });
            task.Start();
        }

        private void CreatePostUnlikedTask(bool? addReputation, string userId, string postCreatorId)
        {
            if (userId == postCreatorId)
                return;

            var task = new Task(() =>
            {
                var usersRepository = new UsersRepository();

                if (addReputation.HasValue)
                {
                    if (addReputation.Value)
                        usersRepository.AddReputation(postCreatorId, (-1) * Entities.Constants.ReputationAwards.PostUpvoted);
                    else
                        usersRepository.AddReputation(postCreatorId, (-1) * Entities.Constants.ReputationAwards.PostDownvoted);
                }
            });
            task.Start();
        }

        private void CreateAnswerMarkedTask(string userId, string previousAnswerCreatorId, string answerCreatorId, int postId)
        {
            var task = new Task(() =>
            {
                var usersRepository = new UsersRepository();

                usersRepository.AddReputation(previousAnswerCreatorId, (-1) * Entities.Constants.ReputationAwards.AnswerMarkedAsAccepted);
                usersRepository.AddReputation(answerCreatorId, Entities.Constants.ReputationAwards.AnswerMarkedAsAccepted);

                var notificationsRepository = new NotificationsRepository();
                notificationsRepository.AddNotification(answerCreatorId, Entities.Constants.NotificationTypes.AnswerMarkedAsAccepted, postId: postId);

                usersRepository.CheckForBadges(answerCreatorId, false, false, true);
            });
            task.Start();
        }

        private void CreateAnswerUnmarkedTask(string answerCreatorId)
        {
            var task = new Task(() =>
            {
                var usersRepository = new UsersRepository();

                usersRepository.AddReputation(answerCreatorId, (-1) * Entities.Constants.ReputationAwards.AnswerMarkedAsAccepted);
            });
            task.Start();
        }
    }
}
