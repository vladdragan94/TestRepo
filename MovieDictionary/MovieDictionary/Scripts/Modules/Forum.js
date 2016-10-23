var ForumModule = function () {
    var pageElements = {
        forumTabs: $(".forum-tabs-button"),
        questionsTab: $(".questions"),
        postsTab: $(".posts"),
        createPostButton: $(".create-post-button"),
        addPostButton: $(".create-post"),
        closePost: $(".close-post"),
        questionsSection: $(".questions-section"),
        postsSection: $(".posts-section"),
        loadMoreQuestions: $("#load-more-questions"),
        loadMorePosts: $("#load-more-posts"),
        questionsOrder: $(".questions-order"),
        postsOrder: $(".posts-order"),
        numberOfQuestions: $("#number-of-questions"),
        numberOfPosts: $("#number-of-posts")
    };

    var templates = {
        questionsTemplate: Handlebars.compile($("#forum-questions-template").html()),
        postsTemplate: Handlebars.compile($("#forum-posts-template").html())
    };

    var questionsPageNumber = 0;
    var postsPageNumber = 0;

    this.initialize = function () {
        loadQuestions();
        loadPosts();
        bindEvents();
    };

    var bindEvents = function () {
        pageElements.forumTabs.click(function () {
            var button = $(this);
            var tab = $(this).attr("id").split("-")[0];

            pageElements.forumTabs.removeClass("active-tab");
            button.addClass("active-tab");

            if (tab == "questions") {
                pageElements.postsTab.fadeOut(200, function () {
                    pageElements.questionsTab.fadeIn(200, function () {
                        pageElements.postsTab.hide();
                        pageElements.questionsTab.show();
                    });
                });
            } else {
                pageElements.questionsTab.fadeOut(200, function () {
                    pageElements.postsTab.fadeIn(200, function () {
                        pageElements.questionsTab.hide();
                        pageElements.postsTab.show();
                    });
                });
            }
        });

        pageElements.createPostButton.click(function () {
            var button = $(this);
            button.hide();
            var parent = button.parent();
            parent.find(".post-title").val("");
            parent.find(".post-content").val("");
            parent.find(".add-post").slideDown(300, function () {
                parent.find(".post-title").focus();
            });
        });

        pageElements.closePost.click(function () {
            var button = $(this);
            var parent = button.parent().parent().parent();
            parent.find(".create-post-button").fadeIn(200);
            parent.find(".add-post").slideUp(300);
        });

        pageElements.addPostButton.click(function () {
            var button = $(this);
            var parent = $(this).parent();
            var title = parent.find(".post-title").val();
            var content = parent.find(".post-content").val();
            var isQuestion = button.attr("id").split("-")[1] == "question";

            if (title && content) {
                AjaxController.Forum.AddPost(title, content, isQuestion, null, function (response) {
                    if (response.Success) {
                        if (isQuestion) {
                            var questions = [];
                            var question = {
                                Id: response.PostId,
                                Title: title,
                                Content: content,
                                Votes: 0,
                                NumberOfComments: 0,
                                PosterName: $("#username").val(),
                                UserId: $("#user-id").val(),
                                IsQuestion: true,
                                HasAnswer: false,
                                DateAdded: new Date().toDateString().substr(4)
                            };
                            questions.push(question);

                            var text = templates.questionsTemplate({ items: questions });
                            pageElements.questionsSection.prepend(text);
                            pageElements.numberOfQuestions.html(parseInt(pageElements.numberOfQuestions.html()) + 1);
                            pageElements.questionsOrder.show();
                            $(".forum-post-item").fadeIn(200);
                            Alerter.ShowMessage("Questions added");
                        } else {
                            var posts = [];
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
                            posts.push(post);

                            var text = templates.postsTemplate({ items: posts });
                            pageElements.postsSection.prepend(text);
                            pageElements.numberOfPosts.html(parseInt(pageElements.numberOfPosts.html()) + 1);
                            pageElements.postsOrder.show();
                            $(".forum-post-item").fadeIn(200);
                            Alerter.ShowMessage("Post added");
                        }

                        parent.parent().hide();
                        parent.parent().parent().find(".create-post-button").show();
                        initializeNewForumItems(false);
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                Alerter.ShowError("You must add a title and content for your post!")
            }
        });

        pageElements.loadMoreQuestions.click(function () {
            var orderType = $(".questions-order").find(".posts-order-button.active-order").attr("id").split("-")[1];
            loadQuestions(orderType);
        });

        pageElements.loadMorePosts.click(function () {
            var orderType = $(".posts-order").find(".posts-order-button.active-order").attr("id").split("-")[1];
            loadPosts(orderType);
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
                        Alerter.ShowMessage("Post voted down");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
        });

        pageElements.questionsOrder.find(".posts-order-button").click(function () {
            var button = $(this);
            button.siblings().removeClass("active-order");
            button.addClass("active-order");
            var orderType = button.attr("id").split("-")[1];
            questionsPageNumber = 0;
            loadQuestions(orderType);
        });

        pageElements.postsOrder.find(".posts-order-button").click(function () {
            var button = $(this);
            button.siblings().removeClass("active-order");
            button.addClass("active-order");
            var orderType = button.attr("id").split("-")[1];
            postsPageNumber = 0;
            loadPosts(orderType);
        });
    };

    var loadQuestions = function (orderType) {
        AjaxController.Forum.GetQuestions(orderType, questionsPageNumber, function (response) {
            if (response.Success) {
                if (questionsPageNumber == 0) {
                    renderQuestions(response.Questions);
                } else {
                    renderMoreQuestions(response.Questions);
                }

                questionsPageNumber++;

                var totalQuestions = parseInt($("#number-of-questions").html());

                if (questionsPageNumber * 10 >= totalQuestions) {
                    pageElements.loadMoreQuestions.hide();
                } else {
                    pageElements.loadMoreQuestions.show();
                }
            } else {
                Alerter.ShowError(response.Message);
            }
        });
    };

    var loadPosts = function (orderType) {
        AjaxController.Forum.GetPosts(orderType, postsPageNumber, function (response) {
            if (response.Success) {
                if (postsPageNumber == 0) {
                    renderPosts(response.Posts);
                } else {
                    renderMorePosts(response.Posts);
                }

                postsPageNumber++;

                var totalPosts = parseInt($("#number-of-posts").html());

                if (postsPageNumber * 10 >= totalPosts) {
                    pageElements.loadMorePosts.hide();
                } else {
                    pageElements.loadMorePosts.show();
                }
            } else {
                Alerter.ShowError(response.Message);
            }
        });
    };

    var renderQuestions = function (questions) {
        var text = templates.questionsTemplate({ items: questions });

        pageElements.questionsSection.fadeOut(200, function () {
            pageElements.questionsSection.html(text);

            initializeNewForumItems(true);
            $(".forum-post-item").fadeIn(200);
        });

        pageElements.questionsSection.fadeIn(200);
    };

    var renderPosts = function (posts) {
        var text = templates.postsTemplate({ items: posts });
        pageElements.postsSection.fadeOut(200, function () {
            pageElements.postsSection.html(text);

            initializeNewForumItems(true);
            $(".forum-post-item").fadeIn(200);
        });

        pageElements.postsSection.fadeIn(200);
    };

    var renderMoreQuestions = function (questions) {
        var text = templates.questionsTemplate({ items: questions });
        pageElements.questionsSection.append(text);
        initializeNewForumItems(true);
        $(".forum-post-item").fadeIn(200);
    };

    var renderMorePosts = function (posts) {
        var text = templates.postsTemplate({ items: posts });
        pageElements.postsSection.append(text);
        initializeNewForumItems(true);
        $(".forum-post-item").fadeIn(200);
    };

    var initializeNewForumItems = function (parseDate) {
        var forumItems = $(".forum-post-item.new-forum-item");

        for (var i = 0; i < forumItems.length; i++) {
            var currentPost = $(forumItems[i]);
            var ratedByCurrentUser = currentPost.find(".rated-by-current-user").html();

            if (ratedByCurrentUser) {
                if (ratedByCurrentUser == "true") {
                    currentPost.find(".post-vote-up").addClass("post-vote-up-active");
                } else {
                    currentPost.find(".post-vote-down").addClass("post-vote-down-active");
                }
            }

            var postDate = currentPost.find(".post-date-added");
            var parsedDate = postDate.html();

            if (parseDate) {
                parsedDate = new Date(parseInt(postDate.html().substr(6))).toDateString().substr(4);
            }

            var index = parsedDate.length - 5;
            parsedDate = parsedDate.substr(0, index) + "," + parsedDate.substr(index);
            postDate.html(parsedDate);

            currentPost.removeClass("new-forum-item");
        }
    };
};

new ForumModule().initialize();