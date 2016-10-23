using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.Entities
{
    public class Constants
    {
        public class MoviesApi
        {
            public const string MovieIDUrl = "http://www.omdbapi.com/?i={0}&plot=short&r=json";

            public const string MovieTitleUrl = "http://www.omdbapi.com/?t={0}&type=movie&plot=short&r=json";

            public const string MovieTitleYearUrl = "http://www.omdbapi.com/?t={0}&y={1}&type=movie&plot=short&r=json";

            public const string MovieSearchUrl = "http://www.omdbapi.com/?s={0}&type=movie&r=json";

            public const string InfoNotAvailable = "N/A";

            public const string PosterFormatResponse = "SX300";

            public const string DefaultPosterFormat = "SX640_SY720";

            public const char AwardsCharSeparator = '.';

            public const char GenresCharSeparator = ',';
        }

        public class CinemaCity
        {
            public const string LatestMoviesUrl = "http://cinemacity.ro/en/";

            public const string RequiredAttribute = "class";

            public const string MovieTitleClass = "featureName";

            public const string MovieSpec2D = "2D";

            public const string MovieSpec3D = "3D";

            public const string MovieSpec4DX = "4DX";
        }

        public class Youtube
        {
            public const string TrailerSearchUrl = "https://www.youtube.com/results?search_query={0}&gl=US";

            public const string RequiredAttribute = "class";

            public const string ResultItemContentClass = "yt-lockup-content";

            public const string ResultItemTitleClass = "yt-lockup-title";

            public const string ResultItemChannelClass = "yt-lockup-byline";

            public const string TrailersChannel = "Movieclips";

            public const string LinkRequiredAttribute = "href";

            public const string TrailerKeyword = "trailer";

            public static readonly string[] TrailerIdSeparator = new string[] { "?v=" };
        }

        public class OrderTypes
        {
            public const string Rating = "Rating";

            public const string Votes = "Votes";

            public const string Date = "Released";
        }

        public class UserRoles
        {
            public const string Admin = "Admin";
        }

        public class MovieProperties
        {
            public static readonly string DefaultPoster = ApplicationConfigurations.ApplicationBaseUrl + "Content/Images/filmroll.jpg";

            public const string DefaultPosterExtension = ".jpg";

            public const string DefaultPostersDirectory = "Content/Posters/";
        }

        public class ApplicationConfigurations
        {
            public static readonly string ApplicationBaseUrl = ConfigurationManager.AppSettings["ApplicationBaseUrl"];

            public const string CryptoString = "71f6cc23-a094-44cd-af4c-1156611fc7d6";

            public const int DefaultLatestMoviesRefreshIntervalInDays = 1;

            public const int DefaultNotificationsDurationInDays = 7;

            public const int DefaultPageSize = 10;

            public const int DefaultMoviesPageSize = 20;

            public const int DefaultReviewsPageSize = 5;

            public const int DefaultSearchSuggestionPageSize = 5;

            public const string LatestMoviesDate = "LatestMoviesDate";

            public const string LastNotificationCheckDate = "LastNotificationCheckDate";

            public const int BadgeAwardedAt = 10;
        }

        public class ErrorMessages
        {
            public const string NotLoggedIn = "You are not logged in!";

            public const string InvalidLoginAttempt = "Invalid username and/or password.";

            public const string DefaultError = "An error has occurred while performing this action.";

            public const string PageNotFound = "Page not found.";

            public const string AlreadyLoggedIn = "You are aldready logged in, you can reload the page and log off if you want to log in with a different account.";
        }

        public class NotificationTypes
        {
            public const string BadgeAwarded = "BadgeAwarded";

            public const string NewRecommendation = "NewRecommendation";

            public const string ReviewVoted = "ReviewVoted";

            public const string PostVoted = "PostVoted";

            public const string AnswerMarkedAsAccepted = "AnswerMarkedAsAccepted";

            public const string MovieInCinema = "MovieInCinema";

            public const string RecommendationLiked = "RecommendationLiked";
        }

        public class BadgeTypes
        {
            public const string TopReviewer = "TopReviewer";

            public const string CommunityHelper = "CommunityHelper";

            public static readonly List<string> GenresFan = new List<string> { "Comedy", "Sci-Fi", "Horror", "Animation", "Romance",
                                                                               "Drama", "Fantasy", "Action", "Western" };
        }

        public class ReputationAwards
        {
            public const int ReviewLiked = 5;

            public const int ReviewDisliked = -5;

            public const int PostUpvoted = 1;

            public const int PostDownvoted = -1;

            public const int AnswerMarkedAsAccepted = 10;

            public const int RecommendationLiked = 3;

            public const int RecommendationDisliked = -1;

            public const int BadgeAwarded = 20;
        }
    }
}
