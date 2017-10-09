(function () {
    'use strict';

    angular.module("app").directive('datepicker', function () {
        return {
            link: function (scope, el, attr) {
                $(el).datepicker({
                    onSelect: function (dateText) {
                        console.log(dateText);
                        var expression = attr.ngModel + " = " + "'" + dateText + "'";
                        scope.$apply(expression);
                    }
                });
            }
        };
    });

    angular.module("app").controller("KPIDashboardCtrl", ctrl);
	
    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', '$state', '$stateParams', 'CFMService', '$filter'];

    function ctrl($http, $translate, $scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, DTOptionsBuilder, $state, $stateParams, CFMService, $filter) {
	    var vm = this;

	    vm.coordinator = localStorage.getItem('username');
	    vm.roles = localStorage.getItem('roles');

	    $scope.currentPage = 1;
	    vm.offset = (vm.currentPage * 10) - 10;
	    vm.totalRecords = 0;

	    vm.init = init;
	    function init() {
	        if (vm.roles == 'SYSTEM.ROLE_HR') {
	            vm.jLoad(1);
	        } else {
	            $state.go('home');
	        }
	    }

	    vm.jLoad = jLoad;
	    function jLoad(current) {

	        //UIControlService.loadLoading("Loading");

	        var offset = (current * 10) - 10;
	        var limit = 15;

	        //var param = $scope.policenumber + "|" + $scope.make + "|" + $scope.model + "|" + $scope.series + "|" + $scope.bodytype + "|" + $scope.color + "|" + $scope.group + "|" + $scope.costcenter + "|" + $scope.supplier + "|" + $scope.startcontract + "|" + $scope.endcontract + "|" + $scope.startidle + "|" + $scope.endidle + "|" + $scope.idleduration + "|" + $scope.monthinstalment + "|" + $scope.totmonthinstalment + "|" + offset + "|" + limit;
	        //console.info(param);

	        CFMService.GetReportKPIMonitoring(function succ(reply) {
	            if (reply.status == 200) {
	                //UIControlService.unloadLoading();
	                console.info(reply.data.List);
	                //$scope.IdleVehicles = reply.data.List;

	                //$scope.totalItems = reply.data.Count;
	                //$scope.itemsPerPage = limit;
	            } else {
	                //UIControlService.unloadLoading();
	                $.growl.error({ message: "Gagal mengambil data CSF" })
	            }
	        });
	    }

	    $scope.$watch("currentPage", function () {
	        //setPagingData($scope.currentPage);
	        //vm.jLoad($scope.currentPage);
	    });

	    function setPagingData(page) {
	        var pagedData = $scope.IdleVehicles.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
	        $scope.aCandidates = pagedData;
	    }
		
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
        $scope.submitForm = function () {
            formData = $scope.form;
            if (formData) {
                console.log(formData);
                $window.alert("Submit");
                angular.element('#myFilter').trigger('click');
            }

        }

		$scope.newValue = function (value) {
		    $scope.form.formCity = value;
		}
		
	}

})();