(function () {
	'use strict';

    angular.module("app")
        .directive('maskingNpwp', npwp)
        .directive('maskingTelepon', function () {
		return {
			restrict: "EA",
			link: function (scope, elm, attr) {
				$(elm).mask("(999) 999-9999");
			}
		};})
        .directive('noSpace', nospace)
        .directive('bismillah', function () {
            return {
                restrict: 'C',
                link: function (scope, elt, attrs) {
                    alert('Tree clicked!');
                    $(elt).tree();
                }
            };
        });


	function npwp() {
		return {
			restrict: "EA",
			link: function (scope, elm, attr) {
				$(elm).mask("99.999.999.9-999.999");
			}
		};
	}

	function nospace() {
		return {
			restrict: "EA",
			link: function (scope, elm, attr) {
				$(elm).on({
					keydown: function (e) {
						if (e.which === 32) {
							return false;
						}
					},
					change: function () {
						this.value = this.value.replace(/\s/g, "");
					}
				});
			}
		};
	}
})();