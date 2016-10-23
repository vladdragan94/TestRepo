using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.Entities;
using MovieDictionary.IRepositories;

namespace MovieDictionary.BL
{
    public class ReviewsManager
    {
        private IReviews repository;

        private IUsers usersRepository;

        public ReviewsManager()
        {
            this.repository = new DAL.ReviewsRepository() as IReviews;
            this.usersRepository = new DAL.UsersRepository() as IUsers;
        }

        public ReviewsManager(IReviews reviewsRepository, IUsers usersRepository)
        {
            this.repository = reviewsRepository;
            this.usersRepository = usersRepository;
        }

        public List<Review> GetMovieReviews(string movieId, string userId = null, string orderType = null, int pageNumber = 0)
        {
            return repository.GetMovieReviews(movieId, userId, orderType, pageNumber);
        }

        public Review GetReview(int reviewId, string userId = null)
        {
            return repository.GetReview(reviewId, userId);
        }

        public int? AddReview(Review review)
        {
            if (string.IsNullOrWhiteSpace(review.Title) || string.IsNullOrWhiteSpace(review.Content) || review.Rating > 10 || review.Rating < 1)
                return null;

            return repository.AddReview(review);
        }

        public void LikeReview(int reviewId, string userId, bool liked)
        {
            repository.LikeReview(reviewId, userId, liked);
        }

        public void UnlikeReview(int reviewId, string userId)
        {
            repository.UnlikeReview(reviewId, userId);
        }

        public bool DeleteReview(int reviewId, string userId)
        {
            return repository.DeleteReview(reviewId, userId);
        }
    }
}
