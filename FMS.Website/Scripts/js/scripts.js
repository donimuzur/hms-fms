+(function($) {
	'use strict';

	if (typeof window.$ === 'undefined' || !window.$)
		throw "This theme need jQuery";

 
	/**
		//////////////////////////////// * Doc ready //////////////////////////////
	*/
	

	$(function() {
		
		/* sidebar toggle */
		$('.sidebar-toggler').on('click', function(){
			$(this).toggleClass('open');
			$('aside').toggleClass('mini');
			$('main').toggleClass('slide');
			$('.logo-toggle').toggleClass('mini');
			$('.title-parent').toggleClass('slide');
		})
		
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
		
        
		
	});
	
})(jQuery);
