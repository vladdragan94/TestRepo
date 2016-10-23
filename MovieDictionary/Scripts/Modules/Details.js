var disableArrowKeys = function (e) {
    if ($("input[type=radio]").is(':focus')) {
        if (e.keyCode == 37) {
            e.preventDefault();
            window.scrollBy(-100, 0);
        }
        else if (e.keyCode == 38) {
            e.preventDefault();
            window.scrollBy(0, -100);
        }
        else if (e.keyCode == 39) {
            e.preventDefault();
            window.scrollBy(100, 0);
        }
        else if (e.keyCode == 40) {
            e.preventDefault();
            window.scrollBy(0, 100);
        }
    }
}
$(document).keydown(disableArrowKeys);