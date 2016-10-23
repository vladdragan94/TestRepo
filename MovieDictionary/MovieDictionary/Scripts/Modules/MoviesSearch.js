var MovieDetails = function () {
    var pageElements = {
        movieItems: $(".movie-item"),
        filterSectionButton: $(".filter-section-button"),
        filterSection: $(".filter-section"),
        closeFilterButton: $(".close-filter")
    };

    var movies = [];

    this.initialize = function () {
        bindEvents();
        initializeYearsSlider();
        initializeFilter();
    };

    var bindEvents = function () {
        bindMovieDetailsEvents();
        bindWatchlistEvents();
        bindFilterEvents();
    };

    var bindMovieDetailsEvents = function () {
        pageElements.movieItems.hover(function () {
            var currentMovie = $(this);
            var movieId = $(this).attr("id").split("-")[1];

            if (!movies[movieId]) {
                AjaxController.Movies.GetMovieDetails(movieId, function (response) {
                    if (response.Success) {
                        currentMovie.find(".genres").html(response.Movie.Genre);
                        currentMovie.find(".runtime").html(response.Movie.Runtime);
                        currentMovie.find(".director").html(response.Movie.Director);
                        currentMovie.find(".actors").html(response.Movie.Actors);
                        movies[movieId] = true;
                    }
                });
            }
        });
    };

    var bindWatchlistEvents = function () {
        $(".movie-item").on("click", ".in-watchlist", function () {
            var button = $(this);
            var movieId = button.attr("id");

            AjaxController.Movies.RemoveMovieFromWatchlist(movieId, function (response) {
                if (response.Success) {
                    button.removeClass("in-watchlist").addClass("add-to-watchlist").removeClass("btn-success").addClass("btn-primary");
                    button.find(".watchlist-button-text").html("Add to watchlist");
                    button.find(".movie-details-icon").hide();
                    Alerter.ShowMessage("Movie removed from watchlist");
                } else {
                    Alerter.ShowError(response.Message);
                }
            });
        });

        $(".movie-item").on("click", ".add-to-watchlist", function () {
            var button = $(this);
            var movieId = button.attr("id").split("-")[1];

            AjaxController.Movies.AddMovieToWatchlist(movieId, function (response) {
                if (response.Success) {
                    button.addClass("in-watchlist").removeClass("add-to-watchlist").addClass("btn-success").removeClass("btn-primary");
                    button.find(".watchlist-button-text").html("In watchlist");
                    button.find(".movie-details-icon").show();
                    Alerter.ShowMessage("Movie added to your watchlist");
                } else {
                    Alerter.ShowError(response.Message);
                }
            });
        });
    };

    var bindFilterEvents = function () {
        pageElements.filterSectionButton.click(function () {
            pageElements.filterSectionButton.hide();
            pageElements.filterSection.slideDown(300);
        });

        pageElements.closeFilterButton.click(function () {
            pageElements.filterSection.slideUp(300);
            pageElements.filterSectionButton.fadeIn(200);
        });
    };

    var initializeYearsSlider = function () {
        if (pageElements.filterSection.length > 0) {
            var amount = $("#amount"),
                min = $(".min"),
                max = $(".max"),
                minInput = $("#min"),
                maxInput = $("#max"),
                slider = $("#slider").slider({
                    orientation: "horizontal",
                    animate: "fast",
                    range: true,
                    min: 1900,
                    max: 2020,
                    values: [1900, 2020],
                    slide: function (event, ui) {
                        adjust(ui.values[0], ui.values[1]);
                    }
                });

            var adjust = function (min, max) {
                minInput.val(min);
                maxInput.val(max);
                slider.find(".ui-slider-handle:first-of-type").attr("data-content", min);
                slider.find(".ui-slider-handle:last-of-type").attr("data-content", max);
            }

            var min = slider.slider("values", 0);
            var max = slider.slider("values", 1);

            adjust(minInput.val(), maxInput.val());

            slider.slider("values", 0, minInput.val()); 
            slider.slider("values", 1, maxInput.val());
        }
    };

    var initializeFilter = function () {
        if (pageElements.filterSection.length > 0) {

        }
    };
};

new MovieDetails().initialize();
