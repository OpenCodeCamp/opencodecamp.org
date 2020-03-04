$(function () {
    $('button#newsletter-submit-button').on('click', function (evt) {
        evt.preventDefault();

        let lang = $('input[type="hidden"]#hdn_ui_culture').val();

        $.post('/' + lang + '/Marketing/SubscribeToNewsletter', $('form#SubscribeToNewsletter_form').serialize(), function (data, textStatus) {
            //console.log('post');
            if (data === null || data === '') { // If not, an error occurred.
                $('form#SubscribeToNewsletter_form > div#form-error-msg').hide(20);
                $('form#SubscribeToNewsletter_form').hide(50);
                $('div#subscribeToNewsletter-successMsg-ctn').show(150);
            } else {
                $('form#SubscribeToNewsletter_form > div#form-error-msg').show(80);
            }
        }).fail(function (response) {
            //console.log('fail');
            //alert('Error: ' + response.responseText);
            $('form#SubscribeToNewsletter_form > div#form-error-msg').show(80);
        });
    });
});