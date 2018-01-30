+(function($) {
	'use strict';

	if (typeof window.$ === 'undefined' || !window.$)
		throw "This theme need jQuery";

 
	/**
		//////////////////////////////// * Doc ready //////////////////////////////
	*/
	

	$(function() {
		
		/* sidebar toggle */
	    $('.sidebar-toggler').on('click', function() {
	        $(this).toggleClass('open');
	        $('aside').toggleClass('mini');
	        $('main').toggleClass('slide');
	        $('.logo-toggle').toggleClass('mini');
	        $('.title-parent').toggleClass('slide');
	    });
		
		// datepicker init
		$('.date').datetimepicker({
			format: 'DD-MMM-YYYY'
		});
	    
	    // Linked Pickers
	    // https://eonasdan.github.io/bootstrap-datetimepicker/
	    // begin - no have future date
		$('.date-begin').datetimepicker({
		    format: 'DD-MMM-YYYY'
		});
		$('.date-end').datetimepicker({
		    format: 'DD-MMM-YYYY',
		    useCurrent: false //Important! See issue #1075
		});
		$(".date-begin").on("dp.change", function (e) {
		    $('.date-end').data("DateTimePicker").minDate(e.date);
		});
		$(".date-end").on("dp.change", function (e) {
		    $('.date-begin').data("DateTimePicker").maxDate(e.date);
		});
	    // endOf - no have future date
		$('a.action , .action-button.new a').click(function (evt) {
		    var page_active = $('.paginate_button.current').text();
		    var url = window.location.href;
		    setCookie("page_active", page_active, 1);
		    setCookie("url_active", url, 1);
		    console.log("aaaa");
		});
		function setCookie(cname, cvalue, exdays) {
		    var d = new Date();
		    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
		    var expires = "expires=" + d.toUTCString();
		    document.cookie = cname + "=" + cvalue + "; " + expires;
		}
        
		$('.datetime').datetimepicker({
		    format: 'DD-MMM-YYYY HH:mm',
		    
		    sideBySide: true
		});

		$(".menu-link.disabled").on("click", function (event) {
		    event.preventDefault();
		    event.stopPropagation();
		    return false;
		})
	});
	
})(jQuery);
