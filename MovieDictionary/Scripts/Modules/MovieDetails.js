var MovieDetails = function () {
    var pageElements = {
        ratingSection: $(".rating"),
        ratingBar: $(".rating-bar"),
        ratingScore: $(".rating-score"),
        userRating: $(".user-rating"),
        siteRating: $(".site-rating"),
        ratingStars: $(".rating-star"),
        unrateMovieButton: $(".unrate-movie-button"),
        numberOfVotes: $(".number-of-votes"),
        writeReviewButton: $(".write-review-button"),
        reviewsSection: $(".reviews-section"),
        writeReviewSection: $(".add-review"),
        closeReviewSection: $(".close-review"),
        postReviewButton: $(".post-review"),
        reviewLikes: $(".review-likes"),
        reviewsOrderButton: $(".reviews-order-button"),
        loadMoreReviewsButton: $(".load-more-reviews"),
        recommendMovieModal: $("#recommend-movie-modal"),
        recommendMovieButton: $("#recommend-movie-button"),
        recommendMovieFriendsList: $(".friends-list"),
        recommendMovieModalLoadingScreen: $(".recommend-movie-loading-screen")
    };

    var templates = {
        movieReviewsTemplate: Handlebars.compile($("#movie-reviews-template").html()),
        friendsTemplate: Handlebars.compile($("#friends-template").html())
    };

    var movieId = $("#movie-id").val();
    var ratedByCurrentUser = $("#current-user-rating").length > 0;
    var currentUserRating = $("#current-user-rating").val();
    var reviewsPageNumber = 0;
    var friendsList = [];

    this.initialize = function () {
        initializeUserRating();
        loadReviews("Votes");
        bindEvents();
    };

    var bindEvents = function () {
        bindWatchlistEvents();
        bindRatingEvents();
        bindRatingSectionBehaviorEvents();
        bindReviewsEvents();
        bindRecommendationEvents();
    };

    var bindWatchlistEvents = function () {
        $(".actions").on("click", ".in-watchlist", function () {
            var button = $(this);

            AjaxController.Movies.RemoveMovieFromWatchlist(movieId, function (response) {
                if (response.Success) {
                    button.removeClass("in-watchlist").addClass("add-to-watchlist").removeClass("btn-success").addClass("btn-info");
                    button.find(".watchlist-button-text").html("Add to watchlist");
                    button.find(".movie-details-icon").hide();
                    Alerter.ShowMessage("Movie removed from watchlist");
                } else {
                    Alerter.ShowError(response.Message);
                }
            });
        });

        $(".actions").on("click", ".add-to-watchlist", function () {
            var button = $(this);

            AjaxController.Movies.AddMovieToWatchlist(movieId, function (response) {
                if (response.Success) {
                    button.addClass("in-watchlist").removeClass("add-to-watchlist").addClass("btn-success").removeClass("btn-info");
                    button.find(".watchlist-button-text").html("In watchlist");
                    button.find(".movie-details-icon").show();
                    Alerter.ShowMessage("Movie added to your watchlist");
                } else {
                    Alerter.ShowError(response.Message);
                }
            });
        });
    };

    var bindRatingEvents = function () {
        pageElements.ratingStars.click(function () {
            var rating = parseInt($(this).attr("id").split("-")[1]);

            AjaxController.Movies.RateMovie(movieId, rating, function (response) {
                if (response.Success) {
                    currentUserRating = rating;
                    pageElements.siteRating.html(Number(response.Rating).toFixed(1));

                    if (!ratedByCurrentUser)
                        pageElements.numberOfVotes.html(parseInt(pageElements.numberOfVotes.html()) + 1)

                    ratedByCurrentUser = true;

                    pageElements.ratingStars.removeClass("yellow-star").addClass("blue-star");
                    pageElements.unrateMovieButton.css("visibility", "visible");

                    for (i = 0; i < pageElements.ratingStars.length; i++) {
                        var star = $(pageElements.ratingStars[i]);
                        var starValue = parseInt(star.attr("id").split("-")[1]);

                        if (starValue <= rating && star.hasClass("fa-star-o"))
                            star.removeClass("fa-star-o").addClass("fa-star");
                        else if (starValue > rating && star.hasClass("fa-star"))
                            star.removeClass("fa-star").addClass("fa-star-o");
                    }
                } else {
                    Alerter.ShowError(response.Message);
                }
            });
        });

        pageElements.unrateMovieButton.click(function () {
            AjaxController.Movies.UnrateMovie(movieId, function (response) {
                if (response.Success) {
                    ratedByCurrentUser = false;
                    pageElements.siteRating.html(Number(response.Rating).toFixed(1));
                    pageElements.numberOfVotes.html(parseInt(pageElements.numberOfVotes.html()) - 1)
                    pageElements.ratingStars.removeClass("blue-star").addClass("yellow-star");
                    pageElements.userRating.hide();
                    pageElements.unrateMovieButton.css("visibility", "hidden");
                    initializeRating(response.Rating, true);
                } else {
                    Alerter.ShowError(response.Message);
                }
            });
        });
    };

    var bindRatingSectionBehaviorEvents = function () {
        pageElements.ratingStars.last().css("padding-right", "26px");

        pageElements.ratingSection.mouseenter(function () {
            if (ratedByCurrentUser) {
                pageElements.unrateMovieButton.css("visibility", "visible");
            }
        });

        pageElements.ratingSection.mouseleave(function () {
            pageElements.unrateMovieButton.css("visibility", "hidden");
        });

        pageElements.ratingStars.hover(function () {
            var value = parseInt($(this).attr("id").split("-")[1]);
            pageElements.userRating.find("span:first").html(value);
        });

        pageElements.ratingBar.mouseenter(function () {
            pageElements.userRating.show();
        });

        pageElements.ratingBar.mouseleave(function () {
            if (!ratedByCurrentUser) {
                pageElements.userRating.hide();
                pageElements.ratingStars.removeClass("yellow-star").removeClass("blue-star").addClass("yellow-star");
            }
            else {
                pageElements.userRating.find("span:first").html(currentUserRating);
            }
        });
    };

    var bindReviewsEvents = function () {
        pageElements.writeReviewButton.click(function () {
            $(this).hide();
            $("html, body").animate({ scrollTop: $("#reviews-section").offset().top - 50 }, 300);
            pageElements.writeReviewSection.slideDown(300, function () {
                pageElements.writeReviewSection.find(".review-title").focus();
            });
        });

        pageElements.closeReviewSection.click(function () {
            pageElements.writeReviewButton.fadeIn(300);
            pageElements.writeReviewSection.slideUp(300);
        });

        pageElements.postReviewButton.click(function () {
            if (!currentUserRating) {
                Alerter.ShowError("You must rate a movie before adding a review!");
                return;
            }

            var title = $(this).parent().find(".review-title").val();
            var content = $(this).parent().find(".review-content").val();

            if (title && content) {
                AjaxController.Reviews.AddReview(movieId, title, content, currentUserRating, function (response) {
                    if (response.Success) {
                        var review = {
                            Id: response.ReviewId,
                            Title: title,
                            Content: content,
                            Rating: currentUserRating,
                            UserName: $("#username").val(),
                            UserId: $("#user-id").val(),
                            NumberOfLikes: 0,
                            NumberOfDislikes: 0,
                            DateAdded: new Date().toDateString().substr(4)
                        };

                        var reviews = [];
                        reviews.push(review);
                        var text = templates.movieReviewsTemplate({ items: reviews });
                        pageElements.reviewsSection.prepend(text);
                        initializeNewReviewItems(false);
                        pageElements.writeReviewButton.hide();
                        pageElements.writeReviewSection.hide();
                        var numberOfReviews = parseInt($("#number-of-reviews").html() || 0) + 1;
                        $("#number-of-reviews").html(numberOfReviews);
                        $("#number-of-reviews").parent().children().show();
                        pageElements.reviewsOrderButton.parent().show();
                        $(".movie-review").fadeIn(200);
                        Alerter.ShowMessage("Review added");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                Alerter.ShowError("You must add a title and content for the review!");
            }
        });

        pageElements.reviewsSection.on("click", ".like-button", function () {
            var button = $(this);
            var reviewId = button.attr("id").split("-")[1];

            if (button.hasClass("active-like")) {
                AjaxController.Reviews.UnlikeReview(reviewId, function (response) {
                    if (response.Success) {
                        button.removeClass("active-like").addClass("default-like");
                        var numberOfLikes = parseInt(button.parent().find(".number-of-likes").html()) - 1;
                        button.parent().find(".number-of-likes").html(" " + numberOfLikes);
                        Alerter.ShowMessage("Review unliked");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                AjaxController.Reviews.LikeReview(reviewId, true, function (response) {
                    if (response.Success) {
                        button.removeClass("default-like").addClass("active-like");
                        var numberOfLikes = parseInt(button.parent().find(".number-of-likes").html()) + 1;
                        button.parent().find(".number-of-likes").html(" " + numberOfLikes);

                        var dislikeButton = button.parent().parent().find(".dislike-button");

                        if (dislikeButton.hasClass("active-dislike")) {
                            dislikeButton.removeClass("active-dislike").addClass("default-like");
                            var numberOfDislikes = parseInt(dislikeButton.parent().find(".number-of-dislikes").html()) - 1;
                            dislikeButton.parent().find(".number-of-dislikes").html(" " + numberOfDislikes);
                        }

                        Alerter.ShowMessage("Review liked");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
        });

        pageElements.reviewsSection.on("click", ".dislike-button", function () {
            var button = $(this);
            var reviewId = button.attr("id").split("-")[1];

            if (button.hasClass("active-dislike")) {
                AjaxController.Reviews.UnlikeReview(reviewId, function (response) {
                    if (response.Success) {
                        button.removeClass("active-dislike").addClass("default-like");
                        var numberOfDislikes = parseInt(button.parent().find(".number-of-dislikes").html()) - 1;
                        button.parent().find(".number-of-dislikes").html(" " + numberOfDislikes);
                        Alerter.ShowMessage("Review unliked");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                AjaxController.Reviews.LikeReview(reviewId, false, function (response) {
                    if (response.Success) {
                        button.removeClass("default-like").addClass("active-dislike");
                        var numberOfLikes = parseInt(button.parent().find(".number-of-dislikes").html()) + 1;
                        button.parent().find(".number-of-dislikes").html(" " + numberOfLikes);

                        var likeButton = button.parent().parent().find(".like-button");

                        if (likeButton.hasClass("active-like")) {
                            likeButton.removeClass("active-like").addClass("default-like");
                            var numberOfLikes = parseInt(likeButton.parent().find(".number-of-likes").html()) - 1;
                            likeButton.parent().find(".number-of-likes").html(" " + numberOfLikes);
                        }

                        Alerter.ShowMessage("Review disliked");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
        });

        pageElements.reviewsOrderButton.click(function () {
            var button = $(this);
            pageElements.reviewsOrderButton.removeClass("active-order");
            button.addClass("active-order");
            var orderType = button.attr("id").split("-")[1];
            reviewsPageNumber = 0;
            loadReviews(orderType);
        });

        pageElements.loadMoreReviewsButton.click(function () {
            var orderType = $(".reviews-order-button.active-order").attr("id").split("-")[1];
            loadReviews(orderType);
        });
    };

    var bindRecommendationEvents = function () {
        pageElements.recommendMovieModal.on("change", ".friend-checkbox", function () {
            var checkBox = $(this);
            var friendId = checkBox.attr("id").split("_")[1];

            if (checkBox.prop("checked"))
                friendsList.push(friendId);
            else {
                var index = friendsList.indexOf(friendId);

                if (index != -1)
                    friendsList.splice(index, 1);
            }
        });

        pageElements.recommendMovieButton.click(function () {
            if (friendsList.length > 0) {
                AjaxController.Users.RecommendMovie(movieId, friendsList, function (response) {
                    if (response.Success) {
                        pageElements.recommendMovieModal.modal("hide");
                        Alerter.ShowMessage("Movie recommended to friends");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
            else {
                Alerter.ShowError("You haven't selected any friends");
            }
        });

        pageElements.recommendMovieModal.on("show.bs.modal", function () {
            if (!$("#user-id").val()) {
                Alerter.ShowError("You are not logged in!");
                return false;
            }

            var movieTitle = $("#movie-title").html();
            pageElements.recommendMovieModal.find(".recomend-movie-name").html(movieTitle);
            pageElements.recommendMovieFriendsList.hide();
            pageElements.recommendMovieModalLoadingScreen.show();
            pageElements.recommendMovieButton.prop("disabled", false);

            AjaxController.Users.GetFriendsForRecommendation(movieId, function (response) {
                pageElements.recommendMovieModalLoadingScreen.fadeOut(300, function () {
                    setTimeout(function () {
                        renderFriends(response.Users);
                    }, 50);

                    if (!response.Success) {
                        Alerter.ShowError(response.Message);
                    }
                });
            });
        });

        pageElements.recommendMovieModal.on("hide.bs.modal", function () {
            friendsList = [];
        });
    };

    var initializeUserRating = function () {
        if (ratedByCurrentUser) {
            pageElements.userRating.show();
            pageElements.ratingStars.addClass("blue-star");
            initializeRating(currentUserRating, false);
        }
    };

    var initializeRating = function (rating, showFractionalStar) {
        for (i = 0; i < pageElements.ratingStars.length; i++) {
            var star = $(pageElements.ratingStars[i]);
            var starValue = parseInt(star.attr("id").split("-")[1]);

            if (starValue <= rating && star.hasClass("fa-star-o"))
                star.removeClass("fa-star-o").addClass("fa-star");
            else if (starValue > rating && star.hasClass("fa-star"))
                star.removeClass("fa-star").addClass("fa-star-o");
        }

        var fractionalStarValue = parseInt($(".fractional-star").attr("id").split("-")[1]);

        if (showFractionalStar) {
            if (fractionalStarValue == parseInt(rating) + 1) {
                $(".fractional-star").removeClass("fa-star-o").addClass("fa-star");
            }
            else {
                $(".fractional-star").removeClass("fa-star").addClass("fa-star-o");
            }
        }
    };

    var initializeReviewRating = function (ratingStars, rating) {
        for (i = 0; i < ratingStars.length; i++) {
            var star = $(ratingStars[i]);
            var starValue = parseInt(star.attr("data-id").split("-")[2]);

            if (starValue <= rating && star.hasClass("fa-star-o"))
                star.removeClass("fa-star-o").addClass("fa-star");
            else if (starValue > rating && star.hasClass("fa-star"))
                star.removeClass("fa-star").addClass("fa-star-o");
        }
    };

    var renderReviews = function (reviews) {
        var text = templates.movieReviewsTemplate({ items: reviews });

        pageElements.reviewsSection.fadeOut(200, function () {
            pageElements.reviewsSection.html(text);

            initializeNewReviewItems(true);
            $(".movie-review").fadeIn(200);
        });

        pageElements.reviewsSection.fadeIn(200);
    };

    var renderMoreReviews = function (reviews) {
        var text = templates.movieReviewsTemplate({ items: reviews });
        pageElements.reviewsSection.append(text);
        initializeNewReviewItems(true);
        $(".movie-review").fadeIn(200);
    };

    var renderFriends = function (friends) {
        friends = friends || [];

        var text;

        if (friends.length > 0) {
            text = templates.friendsTemplate({ items: friends });
        } else {
            text = "No friends found for this recommendation";
            pageElements.recommendMovieButton.prop("disabled", true);
        }

        pageElements.recommendMovieFriendsList.html(text);
        pageElements.recommendMovieFriendsList.fadeIn(200);
    };

    var initializeNewReviewItems = function (parseDate) {
        var reviewElements = $(".movie-review.new-review-item");

        for (var i = 0; i < reviewElements.length; i++) {
            var currentReview = $(reviewElements[i]);
            var ratedByCurrentUser = currentReview.find(".rated-by-current-user").html();

            if (ratedByCurrentUser) {
                if (ratedByCurrentUser == "true") {
                    currentReview.find(".like-button").addClass("active-like");
                } else {
                    currentReview.find(".dislike-button").addClass("active-dislike");
                }
            }

            var rating = parseInt(currentReview.find(".review-rating-value").html());
            var ratingStars = currentReview.find(".review-rating-star");
            initializeReviewRating(ratingStars, rating);

            var movieDate = currentReview.find(".movie-date-added");
            var parsedDate = movieDate.html();

            if (parseDate) {
                parsedDate = new Date(parseInt(movieDate.html().substr(6))).toDateString().substr(4);
            }

            var index = parsedDate.length - 5;
            parsedDate = parsedDate.substr(0, index) + "," + parsedDate.substr(index);
            movieDate.html(parsedDate);

            currentReview.removeClass("new-review-item");
        }
    };

    var loadReviews = function (orderType) {
        AjaxController.Reviews.GetReviews(movieId, orderType, reviewsPageNumber, function (response) {
            if (response.Success) {
                if (reviewsPageNumber == 0) {
                    renderReviews(response.Reviews);
                } else {
                    renderMoreReviews(response.Reviews);
                }

                reviewsPageNumber++;

                var totalReviews = parseInt($("#number-of-reviews").html());

                if (reviewsPageNumber * 5 >= totalReviews) {
                    pageElements.loadMoreReviewsButton.hide();
                } else {
                    pageElements.loadMoreReviewsButton.show();
                }
            } else {
                Alerter.ShowError(response.Message);
            }
        });
    };
};

new MovieDetails().initialize();