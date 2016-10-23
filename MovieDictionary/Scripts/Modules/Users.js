var UsersModule = function () {
    var pageElements = {
        findUsersInput: $("#find-users-input"),
        findUsersButton: $(".find-users-button"),
        usersSearchResults: $(".users-search-results"),
        addFriendButton: $(".add-friend-button"),
        expandContentButton: $(".expand-content"),
        markNotificationAsSeenButton: $(".mark-as-seen-button"),
        recommendMovieModal: $("#users-recommendation-modal"),
        recommendMovieFriendsList: $(".friends-list"),
        recommendMovieModalLoadingScreen: $(".recommend-movie-loading-screen"),
        userRecommendationButton: $(".recommendation-users")
    };

    var templates = {
        usersTemplate: Handlebars.compile($("#find-users-template").html()),
        friendsTemplate: Handlebars.compile($("#recommendation-friends-template").html())
    };

    var movieId = null;

    this.initialize = function () {
        bindEvents();
    };

    var bindEvents = function () {
        pageElements.findUsersButton.click(function () {
            var searchTerm = pageElements.findUsersInput.val();

            if (searchTerm) {
                AjaxController.Users.Search(searchTerm, function (response) {
                    if (response.Success) {
                        var text = templates.usersTemplate({ items: response.Users });
                        text = text.trim() || "No results found";
                        pageElements.usersSearchResults.fadeOut(200, function () {
                            pageElements.usersSearchResults.html(text).fadeIn(200);
                        });
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                Alerter.ShowError("You must enter a search term!")
            }
        });

        pageElements.addFriendButton.click(function () {
            var button = $(this);
            var userId = button.attr("id").split("_")[1];

            if (button.hasClass("add-friend")) {
                AjaxController.Users.AddFriend(userId, function (response) {
                    if (response.Success) {
                        button.removeClass("add-friend");
                        button.find(".friend-button-icon").show();
                        button.find(".add-friend-button-text").html("Friends");
                        Alerter.ShowMessage("Friend added");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                AjaxController.Users.RemoveFriend(userId, function (response) {
                    if (response.Success) {
                        button.addClass("add-friend");
                        button.find(".friend-button-icon").hide();
                        button.find(".add-friend-button-text").html("Add friend");
                        Alerter.ShowMessage("Friend removed");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
        });

        pageElements.expandContentButton.click(function () {
            var content = $(this).parent().parent().find(".item-content");

            if (content.hasClass("expanded")) {
                content.slideUp(300);
                content.removeClass("expanded");
            } else {
                content.slideDown(300);
                content.addClass("expanded");
            }
        });

        pageElements.markNotificationAsSeenButton.click(function () {
            var notification = $(this).parent().parent();
            var notificationId = notification.attr("id").split("-")[1];

            AjaxController.Users.MarkNotificationAsSeen(notificationId, function (response) {
                if (response.Success) {
                    notification.addClass("seen-notification");
                    notification.find(".notification-status").remove();
                } else {
                    Alerter.ShowError(response.Message);
                }
            })
        });

        pageElements.userRecommendationButton.click(function () {
            movieId = $(this).attr("id").split("-")[1];
        });

        pageElements.recommendMovieModal.on("show.bs.modal", function () {
            pageElements.recommendMovieFriendsList.hide();
            pageElements.recommendMovieModalLoadingScreen.show();

            AjaxController.Users.GetFriendsWhoRecommendedMovie(movieId, function (response) {
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
    };

    var renderFriends = function (friends) {
        console.log(friends);
        friends = friends || [];

        var text;

        if (friends.length > 0) {
            text = templates.friendsTemplate({ items: friends });
        } else {
            text = "No friends found";
        }

        pageElements.recommendMovieFriendsList.html(text);
        pageElements.recommendMovieFriendsList.fadeIn(200);
    };
};

new UsersModule().initialize();