
function set_val_dir() {
	$(".dir-auto").html(function (i, text) {
		var arabic = /[\u0600-\u06FF]/g; //setting arabic unicode
		var isRTL = $(this).val().charAt(0).match(arabic);
		console.log($(this).val())
		if (isRTL || $(this).val().length==0) {
			//$(this).attr('direction', 'rtl');
			//$(this).parent().attr('direction', 'rtl');
			$(this).addClass('dir-rtl').removeClass('dir-ltr');
		} else {
			//$(this).attr('direction', 'ltr');
			//$(this).parent().attr('direction', 'ltr');
			$(this).addClass('dir-ltr').removeClass('dir-rtl');
		}
		/*return text.replace(/(.+\n?)/g, "<p>$1</p>");*/
	});
}


function set_txt_dir() {
	$(".dir-txt-auto").each(function (i, text) {
		var arabic = /[\u0600-\u06FF]/g; //setting arabic unicode
		var isRTL = $(this).text().trim().charAt(0).match(arabic);
		console.log("text:", $(this).text().trim().charAt(0))
		if (isRTL || $(this).text().length == 0) {
			//$(this).attr('direction', 'rtl');
			//$(this).parent().attr('direction', 'rtl');
			$(this).addClass('dir-rtl').removeClass('dir-ltr');
		} else {
			//$(this).attr('direction', 'ltr');
			//$(this).parent().attr('direction', 'ltr');
			$(this).addClass('dir-ltr').removeClass('dir-rtl');
		}
		/*return text.replace(/(.+\n?)/g, "<p>$1</p>");*/
	});
}


// Fired once elements with dir-auto class changed
$(".dir-auto").on('change keyup paste', function () {
	set_val_dir();
});

// Fired once when document is ready
$(document).ready(function () {
	set_val_dir();
});

// Fired once when document is ready
$(document).ready(function () {
	set_txt_dir();
});
