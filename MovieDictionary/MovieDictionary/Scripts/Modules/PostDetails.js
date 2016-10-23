var PostDetails = function () {
    var pageElements = {
        repliesSection: $(".replies-section"),
        addReplyButton: $(".create-post"),
        closePost: $(".close-post"),
        replyButton: $(".reply-button"),
        replySection: $(".add-post"),
        markAnswer: $(".mark-answer")
    };

    var templates = {
        replyTemplate: Handlebars.compile($("#reply-template").html())
    };

    this.initialize = function () {
        bindEvents();
    };

    var bindEvents = function () {
        pageElements.replyButton.click(function () {
            pageElements.replySection.find(".post-content").val("");

            $("html, body").animate({ scrollTop: $(".post-content").offset().top + 50 }, 300);
            pageElements.replySection.slideDown(300, function () {
                pageElements.replySection.find(".post-content").focus();
            });
        });

        pageElements.closePost.click(function () {
            pageElements.replySection.slideUp(300);
        });

        pageElements.addReplyButton.click(function () {
            var parent = $(this).parent().parent();
            var content = parent.find(".post-content").val();

            if (content) {
                var postId = parent.attr("id").split("-")[3];
                var title = parent.find("#reply-title").val();

                AjaxController.Forum.AddPost(title, content, false, postId, function (response) {
                    if (response.Success) {
                        var post = {
                            Id: response.PostId,
                            Title: title,
                            Content: content,
                            Votes: 0,
                            NumberOfComments: 0,
                            PosterName: $("#username").val(),
                            UserId: $("#user-id").val(),
                            IsQuestion: false,
                            DateAdded: new Date().toDateString().substr(4)
                        };

                        var text = templates.replyTemplate(post);
                        pageElements.repliesSection.prepend(text);
                        pageElements.replySection.hide();
                        initializeNewForumItems();
                        $(".forum-post-reply-item").fadeIn(200);
                        Alerter.ShowMessage("Replied");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                Alerter.ShowError("You must add content for your reply!");
            }
        });

        $(document).on("click", ".post-vote-up", function () {
            var button = $(this);
            var postId = button.attr("id").split("-")[3];

            if (button.hasClass("post-vote-up-active")) {
                AjaxController.Forum.UnlikePost(postId, function (response) {
                    if (response.Success) {
                        button.removeClass("post-vote-up-active")
                        var numberOfVotes = parseInt(button.parent().find(".number-of-votes").html()) - 1;
                        button.parent().find(".number-of-votes").html(numberOfVotes);
                        button.prop("title", "Vote up");
                        Alerter.ShowMessage("Post unvoted");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                AjaxController.Forum.LikePost(postId, true, function (response) {
                    if (response.Success) {
                        button.addClass("post-vote-up-active");
                        var numberOfVotes = parseInt(button.parent().find(".number-of-votes").html()) + 1;
                        var voteDownButton = button.parent().find(".post-vote-down");

                        if (voteDownButton.hasClass("post-vote-down-active")) {
                            voteDownButton.removeClass("post-vote-down-active");
                            numberOfVotes++;
                        }

                        button.parent().find(".number-of-votes").html(numberOfVotes);
                        button.prop("title", "Unvote");
                        Alerter.ShowMessage("Post voted up");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
        });

        $(document).on("click", ".post-vote-down", function () {
            var button = $(this);
            var postId = button.attr("id").split("-")[3];

            if (button.hasClass("post-vote-down-active")) {
                AjaxController.Forum.UnlikePost(postId, function (response) {
                    if (response.Success) {
                        button.removeClass("post-vote-down-active")
                        var numberOfVotes = parseInt(button.parent().find(".number-of-votes").html()) + 1;
                        button.parent().find(".number-of-votes").html(numberOfVotes);
                        button.prop("title", "Vote down");
                        Alerter.ShowMessage("Post unvoted");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                AjaxController.Forum.LikePost(postId, false, function (response) {
                    if (response.Success) {
                        button.addClass("post-vote-down-active");
                        var numberOfVotes = parseInt(button.parent().find(".number-of-votes").html()) - 1;
                        var voteUpButton = button.parent().find(".post-vote-up");

                        if (voteUpButton.hasClass("post-vote-up-active")) {
                            voteUpButton.removeClass("post-vote-up-active");
                            numberOfVotes--;
                        }

                        button.parent().find(".number-of-votes").html(numberOfVotes);
                        button.prop("title", "Unvote");
                        Alerter.ShowMessage("Post voted down");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
        });

        pageElements.markAnswer.click(function () {
            var button = $(this);
            var postId = button.attr("id").split("-")[2];

            if (button.hasClass("accepted-answer")) {
                AjaxController.Forum.UnmarkAnswer(postId, function (response) {
                    if (response.Success) {
                        button.removeClass("accepted-answer");
                        button.parent().find(".accepted-answer-text").hide();
                        button.hide();
                        button.prop("title", "Mark answer");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                AjaxController.Forum.MarkAsAnswer(postId, function (response) {
                    if (response.Success) {
                        pageElements.markAnswer.removeClass("accepted-answer");
                        pageElements.markAnswer.prop("title", "Mark answer");
                        pageElements.markAnswer.hide();
                        $(".accepted-answer-text").hide();
                        button.addClass("accepted-answer");
                        button.parent().find(".accepted-answer-text").show();
                        button.show();
                        button.prop("title", "Unmark answer");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
        });
    };

    var initializeNewForumItems = function () {
        var newForumItem = $(".forum-post-item.new-forum-item");

        var postDate = newForumItem.find(".post-date-added");
        var parsedDate = postDate.html();

        var index = parsedDate.length - 5;
        parsedDate = parsedDate.substr(0, index) + "," + parsedDate.substr(index);
        postDate.html(parsedDate);

        newForumItem.removeClass("new-forum-item");
    };
};

new PostDetails().initialize();