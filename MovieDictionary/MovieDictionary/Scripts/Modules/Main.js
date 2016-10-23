var MainModule = function () {
    var pageElements = {
        logInForm: $("#login-form"),
        registerForm: $("#register-form"),
        moviesSerchResults: $("#movies-search-results"),
        searchMoviesInput: $("#search-for-movies"),
        loginRegisterModal: $("#login-register-modal"),
        loginRegisterPart: $(".modal-login-register"),
        loginModalPart: $(".modal-login"),
        registerModalPart: $(".modal-register"),
        loginRegisterModalLoadingScreen: $(".login-register-modal-loading-screen"),
        loginRegisterModalLoadingMessage: $(".loading-message"),
        searchingMoviesLoadingScreen: $(".loading-movies"),
        loginRegisterSwitchButton: $(".login-register-switch"),
        backToTopButton: $(".back-to-top"),
        movieItems: $(".movie-item"),
        addToWatchlist: $(".add-to-watchlist-icon")
    };

    var templates = {
        searchResultsTemplate: Handlebars.compile($("#search-results-template").html())
    };

    var attemptingLoginOrRegister = false;

    this.initialize = function () {
        bindEvents();
    };

    var bindEvents = function () {
        bindLoginRegisterActions();
        bindMoviesSearchResultsBehaviorEvents();
        bindMoviesEvents();
        bindModalEvents();
        bindScrollingEvents();
    };

    var bindLoginRegisterActions = function () {
        pageElements.logInForm.submit(function () {
            if (!attemptingLoginOrRegister) {
                attemptingLoginOrRegister = true;

                pageElements.loginRegisterModalLoadingScreen.fadeTo(100, 1, function () {
                    $(this).css("visibility", "visible");
                });

                var formData = $(this).serialize();

                var timedOut = false;
                var attempt = setTimeout(function () {
                    pageElements.loginModalPart.find(".validation-summary-errors").fadeOut(200, function () {
                        $(this).html("Operation timed out, please try again.").fadeIn(200);
                    });
                    pageElements.loginRegisterModalLoadingScreen.fadeTo(300, 0, function () {
                        $(this).css("visibility", "hidden");
                        attemptingLoginOrRegister = false;
                    });
                    timedOut = true;
                }, 10000);

                AjaxController.Account.Login(formData, function (response) {
                    clearTimeout(attempt);

                    if (timedOut)
                        return;

                    if (response.Success)
                        location.href = location.href;
                    else {
                        pageElements.loginRegisterModalLoadingScreen.fadeTo(300, 0, function () {
                            $(this).css("visibility", "hidden");
                            attemptingLoginOrRegister = false;
                        });
                        pageElements.loginModalPart.find(".validation-summary-errors").fadeOut(200, function () {
                            $(this).html(response.Message).fadeIn(200);
                        });
                    }
                });
            }

            return false;
        });

        pageElements.registerForm.submit(function () {
            if (!attemptingLoginOrRegister) {
                attemptingLoginOrRegister = true;

                pageElements.loginRegisterModalLoadingScreen.fadeTo(100, 1, function () {
                    $(this).css("visibility", "visible");
                });

                var formData = $(this).serialize();

                var timedOut = false;
                var attempt = setTimeout(function () {
                    pageElements.registerModalPart.find(".validation-summary-errors").fadeOut(200, function () {
                        $(this).html("Operation timed out, please try again.").fadeIn(200);
                    });
                    pageElements.loginRegisterModalLoadingScreen.fadeTo(300, 0, function () {
                        $(this).css("visibility", "hidden");
                        attemptingLoginOrRegister = false;
                    });
                    timedOut = true;
                }, 10000);

                AjaxController.Account.Register(formData, function (response) {
                    clearTimeout(attempt);

                    if (timedOut)
                        return;

                    if (response.Success)
                        location.href = location.href;
                    else {
                        pageElements.loginRegisterModalLoadingScreen.fadeTo(300, 0, function () {
                            $(this).css("visibility", "hidden");
                            attemptingLoginOrRegister = false;
                        });
                        pageElements.registerModalPart.find(".validation-summary-errors").fadeOut(200, function () {
                            $(this).html(response.Message).fadeIn(200);
                        })
                    }
                });
            }

            return false;
        });
    };

    var bindMoviesSearchResultsBehaviorEvents = function () {
        pageElements.searchMoviesInput.keyup(function () {
            var input = $(this);
            var searchTerm = input.val();

            if (searchTerm) {
                pageElements.searchingMoviesLoadingScreen.show();

                AjaxController.Movies.SearchMovies(searchTerm, function (response) {
                    if (input.val() == searchTerm) {
                        renderSearchResults(response);
                        pageElements.moviesSerchResults.fadeIn();
                    }

                    pageElements.searchingMoviesLoadingScreen.fadeOut();
                });
            } else {
                pageElements.moviesSerchResults.fadeOut();
            }
        });

        pageElements.searchMoviesInput.focusin(function () {
            if (pageElements.searchMoviesInput.val()) {
                pageElements.moviesSerchResults.fadeIn();
            }
        });

        pageElements.searchMoviesInput.focusout(function () {
            setTimeout(function () {
                if ($("#movies-search-results:hover").length == 0 && $(".movie-link:focus").length == 0) {
                    pageElements.moviesSerchResults.fadeOut();
                }
            }, 50);
        });

        pageElements.moviesSerchResults.mouseleave(function () {
            setTimeout(function () {
                if ($("#search-for-movies:focus").length == 0 && $(".movie-link:focus").length == 0) {
                    pageElements.moviesSerchResults.fadeOut();
                }
            }, 50);
        });

        pageElements.moviesSerchResults.on("focusout", ".movie-link", function () {
            setTimeout(function () {
                if ($("#search-for-movies:focus").length == 0 && $("#movies-search-results:hover").length == 0 && $(".movie-link:focus").length == 0) {
                    pageElements.moviesSerchResults.fadeOut();
                }
            }, 50);
        });
    };

    var bindMoviesEvents = function () {
        pageElements.addToWatchlist.click(function () {
            var button = $(this);
            var movieId = button.parent().attr("id").split("-")[1];

            if (button.hasClass("fa-calendar-o")) {
                AjaxController.Movies.AddMovieToWatchlist(movieId, function (response) {
                    if (response.Success) {
                        button.removeClass("fa-calendar-o").addClass("fa-calendar-check-o");
                        button.prop("title", "In watchlist");
                        Alerter.ShowMessage("Movie added to your watchlist");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            } else {
                AjaxController.Movies.RemoveMovieFromWatchlist(movieId, function (response) {
                    if (response.Success) {
                        button.removeClass("fa-calendar-check-o").addClass("fa-calendar-o");
                        button.prop("title", "Add to watchlist");

                        if (location.href.indexOf("Watchlist") != -1)
                            button.parent().hide();

                        Alerter.ShowMessage("Movie removed from watchlist");
                    } else {
                        Alerter.ShowError(response.Message);
                    }
                });
            }
        });
    };

    var bindModalEvents = function () {
        pageElements.loginRegisterSwitchButton.click(function () {
            var button = $(this);

            if (button.hasClass("show-login")) {
                button.removeClass("show-login").addClass("show-register");
                $(".login-register-text").html("Already have an account?");
                button.html("Log in");
                $(".login-register-modal-title").html("Register");
                pageElements.loginRegisterModalLoadingMessage.html("Creating account...");
                pageElements.registerModalPart.show();
                pageElements.loginModalPart.hide();
            } else {
                button.removeClass("show-register").addClass("show-login");
                $(".login-register-text").html("Don't have an account?");
                button.html("Register");
                $(".login-register-modal-title").html("Log in");
                pageElements.loginRegisterModalLoadingMessage.html("Logging in...");
                pageElements.loginModalPart.show();
                pageElements.registerModalPart.hide();
            }

            pageElements.loginRegisterModalLoadingScreen.css("visibility", "hidden");
        });
    };

    var bindScrollingEvents = function () {
        pageElements.backToTopButton.click(function () {
            $("html, body").animate({ scrollTop: 0 }, 500);

            return false;
        });

        $(window).scroll(function () {
            if ($(this).scrollTop() > 100) {
                pageElements.backToTopButton.fadeIn();
            } else {
                pageElements.backToTopButton.fadeOut();
            }
        });
    };

    var renderSearchResults = function (movies) {
        var text = templates.searchResultsTemplate({ items: movies });
        pageElements.moviesSerchResults.html(text);
    };
};

new MainModule().initialize();