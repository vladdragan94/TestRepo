var notification = $(".notification-message");
var timeout = null;
var showing = false;

Alerter = {
    ShowMessage: function (message) {
        if (!message)
            return;

        notification.removeClass("alert-success").removeClass("alert-danger").addClass("alert-success");
        showNotification(message);
    },

    ShowError: function (message) {
        if (!message)
            return;

        notification.removeClass("alert-danger").removeClass("alert-success").addClass("alert-danger");
        showNotification(message);
    }
}

var showNotification = function (message) {
    if (!showing) {
        showing = true;

        notification.fadeOut(150, function () {
            notification.find(".message").html(message);
        });

        notification.fadeIn(150, function () {
            showing = false;
        });

        clearTimeout(timeout);
        timeout = setTimeout(function () {
            notification.fadeOut(150);
        }, 5000);
    }
};

$(".close-notification").click(function () {
    clearTimeout(timeout);
    notification.fadeOut(200);
});