using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.IRepositories;

namespace MovieDictionary.DAL
{
    public class ReviewsRepository : IReviews
    {
        public List<Entities.Review> GetMovieReviews(string movieId, string userId = null, string orderType = null, int pageNumber = 0, int pageSize = Entities.Constants.ApplicationConfigurations.DefaultReviewsPageSize)
        {
            using (var dataContext = new MoviesEntities())
            {
                orderType = orderType ?? Entities.Constants.OrderTypes.Votes;

                Func<Reviews, object> orderBy = null;

                if (orderType == Entities.Constants.OrderTypes.Votes)
                    orderBy = item => (item.ReviewsLikes.Count - 2 * item.ReviewsLikes.Where(el => el.Liked == false).Count());
                else if (orderType == Entities.Constants.OrderTypes.Date)
                    orderBy = item => item.DateAdded;

                return dataContext.Reviews
                    .Where(item => item.MovieId == movieId)
                    .OrderByDescending(orderBy)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(item => new Entities.Review()
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Content = item.Content,
                        Rating = item.Rating,
                        UserId = item.UserId,
                        UserName = item.AspNetUsers.UserName,
                        DateAdded = item.DateAdded,
                        MovieId = item.MovieId,
                        NumberOfLikes = item.ReviewsLikes.Where(el => el.Liked == true).Count(),
                        NumberOfDislikes = item.ReviewsLikes.Where(el => el.Liked == false).Count(),
                        RatedByCurrentUser = userId != null ? (item.ReviewsLikes.Any(el => el.UserId == userId) ? (bool?)item.ReviewsLikes.FirstOrDefault(el => el.UserId == userId).Liked : null) : null
                    }).ToList();
            }
        }

        public Entities.Review GetReview(int reviewId, string userId = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                var review = dataContext.Reviews.Find(reviewId);

                if (review == null)
                    return null;

                return new Entities.Review()
                {
                    Id = review.Id,
                    Title = review.Title,
                    Content = review.Content,
                    Rating = review.Rating,
                    UserId = review.UserId,
                    DateAdded = review.DateAdded,
                    MovieId = review.MovieId,
                    NumberOfLikes = review.ReviewsLikes.Where(el => el.Liked == true).Count(),
                    NumberOfDislikes = review.ReviewsLikes.Where(el => el.Liked == false).Count(),
                    RatedByCurrentUser = userId != null ? (review.ReviewsLikes.Any(item => item.UserId == userId) ? (bool?)review.ReviewsLikes.FirstOrDefault(el => el.UserId == userId).Liked : null) : null
                };
            }
        }

        public int AddReview(Entities.Review movieReview)
        {
            using (var dataContext = new MoviesEntities())
            {
                var review = dataContext.Reviews.FirstOrDefault(item => item.MovieId == movieReview.MovieId && item.UserId == movieReview.UserId);

                if (review == null)
                {
                    review = new Reviews();
                    review.MovieId = movieReview.MovieId;
                    review.UserId = movieReview.UserId;
                    dataContext.Reviews.Add(review);
                }

                review.Title = movieReview.Title;
                review.Content = movieReview.Content;
                review.Rating = movieReview.Rating;
                review.DateAdded = movieReview.DateAdded;

                dataContext.SaveChanges();

                return review.Id;
            }
        }

        public void LikeReview(int reviewId, string userId, bool liked)
        {
            string reviewCreatorId = null;
            string movieId = null;
            bool? addReputation = null;
            bool? addDoubleReputation = null; 

            using (var dataContext = new MoviesEntities())
            {                                  
                var like = dataContext.ReviewsLikes.FirstOrDefault(item => item.ReviewId == reviewId && item.UserId == userId);

                if (like == null)
                {
                    like = new ReviewsLikes();
                    like.ReviewId = reviewId;
                    like.UserId = userId;
                    dataContext.ReviewsLikes.Add(like);
                    addReputation = liked;
                }
                else
                {
                    if (like.Liked && !liked)
                        addDoubleReputation = false;
                    else if (!like.Liked && liked)
                        addDoubleReputation = true;
                }

                like.Liked = liked;

                dataContext.SaveChanges();

                var review = dataContext.Reviews.Find(reviewId);
                reviewCreatorId = review.AspNetUsers.Id;
                movieId = review.MovieId;

                CreateReviewLikedTask(addReputation, addDoubleReputation, userId, reviewCreatorId, movieId);
            }
        }

        public void UnlikeReview(int reviewId, string userId)
        {
            string reviewCreatorId = null;
            bool? addReputation = null;

            using (var dataContext = new MoviesEntities())
            {
                var like = dataContext.ReviewsLikes.FirstOrDefault(item => item.ReviewId == reviewId && item.UserId == userId);

                if (like == null)
                    return;

                reviewCreatorId = like.Reviews.AspNetUsers.Id;

                if (like.Liked)
                    addReputation = true;
                else
                    addReputation = false;

                dataContext.ReviewsLikes.Remove(like);
                dataContext.SaveChanges();

                CreateReviewUnlikedTask(addReputation, userId, reviewCreatorId);
            }
        }

        public bool DeleteReview(int reviewId, string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                var review = dataContext.Reviews.Find(reviewId);

                if (review == null)
                    return true;

                bool isAdmin = dataContext.AspNetUsers.Find(userId).AspNetRoles.FirstOrDefault(item => item.Name == Entities.Constants.UserRoles.Admin) != null;

                if (review.UserId != userId && !isAdmin)
                    return false;

                dataContext.ReviewsLikes.RemoveRange(review.ReviewsLikes);
                dataContext.Reviews.Remove(review);
                dataContext.SaveChanges();

                return true;
            }
        }

        private void CreateReviewLikedTask(bool? addReputation, bool? addDoubleReputation, string userId, string reviewCreatorId, string movieId)
        {
            if (userId == reviewCreatorId)
                return;

            var task = new Task(() =>
            {
                var usersRepository = new UsersRepository();

                if (addReputation.HasValue)
                {
                    if (addReputation.Value)
                        usersRepository.AddReputation(reviewCreatorId, Entities.Constants.ReputationAwards.ReviewLiked);
                    else
                        usersRepository.AddReputation(reviewCreatorId, Entities.Constants.ReputationAwards.ReviewDisliked);
                }
                else if (addDoubleReputation.HasValue)
                {
                    if (addDoubleReputation.Value)
                        usersRepository.AddReputation(reviewCreatorId, 2 * Entities.Constants.ReputationAwards.ReviewLiked);
                    else
                        usersRepository.AddReputation(reviewCreatorId, 2 * Entities.Constants.ReputationAwards.ReviewDisliked);
                }

                var notificationsRepository = new NotificationsRepository();
                notificationsRepository.AddNotification(reviewCreatorId, Entities.Constants.NotificationTypes.ReviewVoted, movieId);

                usersRepository.CheckForBadges(reviewCreatorId, false, true, false);
            });
            task.Start();
        }

        private void CreateReviewUnlikedTask(bool? addReputation, string userId, string reviewCreatorId)
        {
            if (userId == reviewCreatorId)
                return;

            var task = new Task(() =>
            {
                var usersRepository = new UsersRepository();

                if (addReputation.HasValue)
                {
                    if (addReputation.Value)
                        usersRepository.AddReputation(reviewCreatorId, (-1) * Entities.Constants.ReputationAwards.ReviewLiked);
                    else
                        usersRepository.AddReputation(reviewCreatorId, (-1) * Entities.Constants.ReputationAwards.ReviewDisliked);
                }
            });
            task.Start();
        }
    }
}
