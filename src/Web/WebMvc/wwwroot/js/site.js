$(window).resize(function () {
	if ($(window).width() > 550) {
		$('header.application_header > nav.application-nav').show(); // Always visible
	}
	else {
		$('header.application_header > nav.application-nav').hide(); // Hide by default
	}
});

// Mobile menu management
$('a.application-nav-mobile-icon').on('click', function (e) {
	e.preventDefault();
	$('header.application_header > nav.application-nav').toggle();
});