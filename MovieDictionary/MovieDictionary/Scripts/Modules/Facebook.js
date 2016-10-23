window.fbAsyncInit = function () {
    FB.init({
        appId: '1074891809221294',
        cookie: true,  
        xfbml: true,  
        version: 'v2.5' 
    });
};

(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

function loginIntoApp() {
    FB.api('/me', { fields: 'email,name' }, function (response) {
        AjaxController.Account.FacebookLogin(response.email, response.name, function (response) {
            if (response.Success)
                location.href = location.href;
            else {
                $(".validation-summary-errors").fadeOut(200, function () {
                    $(this).html(response.Message).fadeIn(200);
                });
            }
        });
    });
}