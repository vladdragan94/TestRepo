using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.IRepositories
{
    public interface IReviews
    {
        List<Entities.Review> GetMovieReviews(string movieId, string userId = null, string orderType = null, int pageNumber = 0, int pageSize = Entities.Constants.ApplicationConfigurations.DefaultReviewsPageSize);

        Entities.Review GetReview(int reviewId, string userId = null);

        int AddReview(Entities.Review movieReview);

        void LikeReview(int reviewId, string userId, bool liked);

        void UnlikeReview(int reviewId, string userId);

        bool DeleteReview(int reviewId, string userId);
    }
}
