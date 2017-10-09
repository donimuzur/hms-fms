(function () {
	'use strict';

	angular.module('app').filter("ucwords", function () {
		return function (input) {
			if (input) { //when input is defined the apply filter
				input = input.toLowerCase().replace(/\b[a-z]/g, function (letter) {
					return letter.toUpperCase();
				});
			}
			return input;
		};
	});
})();