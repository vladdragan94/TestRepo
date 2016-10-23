var baseUrl = $("#application-base-url").val();

var appControllers = {
    Movies: baseUrl + "Movies/",
    Account: baseUrl + "Account/",
    Reviews: baseUrl + "Reviews/",
    Forum: baseUrl + "Posts/",
    Users: baseUrl + "Users/"
};

var actions = {
    Movies: {
        AddMovieToWatchlist: appControllers.Movies + "AddMovieToWatchlist",
        RemoveMovieFromWatchlist: appControllers.Movies + "RemoveMovieFromWatchlist",
        Search: appControllers.Movies + "Search",
        RateMovie: appControllers.Movies + "RateMovie",
        UnrateMovie: appControllers.Movies + "UnrateMovie",
        GetMovieDetails: appControllers.Movies + "GetMovieDetails",
        Details: appControllers.Movies + "Details"
    },

    Account: {
        Login: appControllers.Account + "Login",
        Register: appControllers.Account + "Register",
        FacebookLogin: appControllers.Account + "FacebookLogin"
    },

    Reviews: {
        AddReview: appControllers.Reviews + "AddReview",
        LikeReview: appControllers.Reviews + "LikeReview",
        UnlikeReview: appControllers.Reviews + "UnlikeReview",
        DeleteReview: appControllers.Reviews + "DeleteReview",
        GetReviews: appControllers.Reviews + "GetReviews"
    },

    Forum: {
        GetQuestions: appControllers.Forum + "GetQuestions",
        GetPosts: appControllers.Forum + "GetPosts",
        AddPost: appControllers.Forum + "AddPost",
        LikePost: appControllers.Forum + "LikePost",
        UnlikePost: appControllers.Forum + "UnlikePost",
        MarkAsAnswer: appControllers.Forum + "MarkAsAnswer",
        UnmarkAnswer: appControllers.Forum + "UnmarkAnswer",
        DeletePost: appControllers.Forum + "DeletePost"
    },

    Users: {
        Search: appControllers.Users + "Search",
        AddFriend: appControllers.Users + "AddFriend",
        RemoveFriend: appControllers.Users + "RemoveFriend",
        RecommendMovie: appControllers.Users + "RecommendMovie",
        LikeRecommendation: appControllers.Users + "LikeRecommendation",
        DislikeRecommendation: appControllers.Users + "DislikeRecommendation",
        GetFriendsForRecommendation: appControllers.Users + "GetFriendsForRecommendation",
        GetFriendsWhoRecommendedMovie: appControllers.Users + "GetFriendsWhoRecommendedMovie",
        MarkNotificationAsSeen: appControllers.Users + "MarkNotificationAsSeen"
    }
}

AjaxController = {

    Movies: {
        SearchMovies: function (searchTerm, callback) {
            var url = actions.Movies.Search;
            var data = {
                title: searchTerm
            };

            $.get(url, data, callback);
        },

        AddMovieToWatchlist: function (movieId, callback) {
            var url = actions.Movies.AddMovieToWatchlist;
            var data = {
                movieId: movieId
            };

            $.post(url, data, callback);
        },

        RemoveMovieFromWatchlist: function (movieId, callback) {
            var url = actions.Movies.RemoveMovieFromWatchlist;
            var data = {
                movieId: movieId
            };

            $.post(url, data, callback);
        },

        RateMovie: function (movieId, rating, callback) {
            var url = actions.Movies.RateMovie;
            var data = {
                movieId: movieId,
                rating: rating
            };

            $.post(url, data, callback);
        },

        UnrateMovie: function (movieId, callback) {
            var url = actions.Movies.UnrateMovie;
            var data = {
                movieId: movieId,
            };

            $.post(url, data, callback);
        },

        GetMovieDetails: function (movieId, callback) {
            var url = actions.Movies.GetMovieDetails;
            var data = {
                movieId: movieId
            };

            $.get(url, data, callback);
        }
    },

    Reviews: {
        AddReview: function (movieId, title, content, rating, callback) {
            var url = actions.Reviews.AddReview;
            var data = {
                movieId: movieId,
                title: title,
                content: content,
                rating: rating
            };

            $.post(url, data, callback);
        },
        
        LikeReview: function (reviewId, liked, callback) {
            var url = actions.Reviews.LikeReview;
            var data = {
                reviewId: reviewId,
                liked: liked
            };

            $.post(url, data, callback);
        },

        UnlikeReview: function (reviewId, callback) {
            var url = actions.Reviews.UnlikeReview;
            var data = {
                reviewId: reviewId
            };

            $.post(url, data, callback);
        },

        DeleteReview: function (reviewId, callback) {
            var url = actions.Reviews.DeleteReview;
            var data = {
                reviewId: reviewId
            };

            $.post(url, data, callback);
        },

        GetReviews: function (movieId, orderType, pageNumber, callback) {
            var url = actions.Reviews.GetReviews
            var data = {
                movieId: movieId,
                reviewsOrderType: orderType,
                reviewsPageNumber: pageNumber
            };

            $.get(url, data, callback);
        }
    },

    Forum: {
        GetQuestions: function (orderType, pageNumber, callback) {
            var url = actions.Forum.GetQuestions;
            var data = {
                orderType: orderType,
                pageNumber: pageNumber
            };

            $.get(url, data, callback);
        },

        GetPosts: function (orderType, pageNumber, callback) {
            var url = actions.Forum.GetPosts;
            var data = {
                orderType: orderType,
                pageNumber: pageNumber
            };

            $.get(url, data, callback);
        },

        AddPost: function (title, content, isQuestion, postId, callback) {
            var url = actions.Forum.AddPost;
            var data = {
                title: title,
                content: content,
                isQuestion: isQuestion,
                postId: postId
            };

            $.get(url, data, callback);
        },

        LikePost: function (postId, liked, callback) {
            var url = actions.Forum.LikePost;
            var data = {
                postId: postId,
                liked: liked
            };

            $.post(url, data, callback);
        },

        UnlikePost: function (postId, callback) {
            var url = actions.Forum.UnlikePost;
            var data = {
                postId: postId
            };

            $.post(url, data, callback);
        },

        MarkAsAnswer: function (postId, callback) {
            var url = actions.Forum.MarkAsAnswer;
            var data = {
                postId: postId
            };

            $.post(url, data, callback);
        },

        UnmarkAnswer: function (postId, callback) {
            var url = actions.Forum.UnmarkAnswer;
            var data = {
                postId: postId
            };

            $.post(url, data, callback);
        },

        DeletePost: function (postId, callback) {
            var url = actions.Forum.DeletePost;
            var data = {
                postId: postId
            };

            $.post(url, data, callback);
        },
    },

    Users: {
        Search: function (searchTerm, callback) {
            var url = actions.Users.Search;
            var data = {
                searchTerm: searchTerm
            };

            $.get(url, data, callback);
        },

        AddFriend: function (userId, callback) {
            var url = actions.Users.AddFriend;
            var data = {
                userId: userId
            };

            $.post(url, data, callback);
        },

        RemoveFriend: function (userId, callback) {
            var url = actions.Users.RemoveFriend;
            var data = {
                userId: userId
            };

            $.post(url, data, callback);
        },

        RecommendMovie: function (movieId, friends, callback) {
            var url = actions.Users.RecommendMovie;
            var data = {
                movieId: movieId,
                friends: friends
            };

            $.post(url, data, callback);
        },

        LikeRecommendation: function (movieId, callback) {
            var url = actions.Users.LikeRecommendation;
            var data = {
                movieId: movieId
            };

            $.post(url, data, callback);
        },

        DislikeRecommendation: function (movieId, callback) {
            var url = actions.Users.DislikeRecommendation;
            var data = {
                movieId: movieId
            };

            $.post(url, data, callback);
        },

        GetFriendsForRecommendation: function (movieId, callback) {
            var url = actions.Users.GetFriendsForRecommendation;
            var data = {
                movieId: movieId
            };

            $.get(url, data, callback);
        },

        GetFriendsWhoRecommendedMovie: function (movieId, callback) {
            var url = actions.Users.GetFriendsWhoRecommendedMovie;
            var data = {
                movieId: movieId
            };

            $.get(url, data, callback);
        },

        MarkNotificationAsSeen: function (notificationId, callback) {
            var url = actions.Users.MarkNotificationAsSeen;
            var data = {
                notificationId: notificationId
            };

            $.post(url, data, callback);
        }
    },

    Account: {
        Login: function (loginFormData, callback) {
            var url = actions.Account.Login;

            $.post(url, loginFormData, callback);
        },

        Register: function (registerFormData, callback) {
            var url = actions.Account.Register;

            $.post(url, registerFormData, callback);
        },

        FacebookLogin: function (email, username, callback) {
            var url = actions.Account.FacebookLogin;
            var data = {
                email: email,
                username: username
            };

            $.post(url, data, callback);
        }
    }
}