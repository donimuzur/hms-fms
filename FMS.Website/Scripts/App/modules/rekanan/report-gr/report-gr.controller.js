(function () {
    'use strict';

    angular.module("app").controller("GRDashboardCtrl", ctrl);
	
    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', '$state', '$stateParams', 'CFMService', '$filter'];
	
    function ctrl($http, $translate, $scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, DTOptionsBuilder, $state, $stateParams, CFMService, $filter) {
	    var vm = $scope;

        // Paging
	    vm.currentPage = 1;
	    vm.fullSize = 10;
	    vm.offset = (vm.currentPage * 10) - 10;
	    vm.totalRecords = 0;

        // Roles
	    vm.roles = localStorage.getItem('roles');
		
		vm.toExcel = toExcel;
        function toExcel(tabel) {
            console.info(tabel);
            $window.alert("Export Excel");
			
            /*var exportHref;
            exportHref = MstService.tableToExcel(tabel, 'tabel epaf');
            $timeout(function () { location.href = exportHref; }, 100);*/
        }
		
		vm.toPdf = toPdf;
        function toPdf(tabel) {
            console.info(tabel);
            $window.alert("Export PDF");
			
            /*var exportHref;
            exportHref = MstService.tableToExcel(tabel, 'tabel epaf');
            $timeout(function () { location.href = exportHref; }, 100);*/
        }
		
	    /*
         * Simpan Form
         */

		var formData = {};		
		vm.submitForm = function(filter) {
		    console.log(filter);
            $window.alert("Submit");
            angular.element('#myFilter').trigger('click');
		}

		$scope.newValue = function (value) {
		    console.log(value);

		    $scope.form.Location = value;
		}
		
	}

})();