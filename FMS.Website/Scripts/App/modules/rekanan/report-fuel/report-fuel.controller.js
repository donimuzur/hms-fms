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

    angular.module("app").controller("FuelDashboardCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', '$state', '$stateParams', 'CFMService', '$filter'];

    function ctrl($http, $translate, $scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, DTOptionsBuilder, $state, $stateParams, CFMService, $filter) {
        var vm = $scope;

        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles');
        //console.info(vm.roles);

        $scope.currentPage = 1;
        vm.offset = (vm.currentPage * 10) - 10;
        vm.totalRecords = 0;

        vm.init = init;
        function init() {
            if (vm.roles == 'SYSTEM.ROLE_HR' || vm.roles == 'SYSTEM.ROLE_FLEET') {
                vm.jLoad(1);
            } else {
                $state.go('home');
            }
        }

        $scope.police_number = ''; $scope.cost_center = ''; $scope.fuel_amount = ''; $scope.last_km = ''; $scope.cost = ''; $scope.created_date = ''; $scope.manufacturer = ''; $scope.model = ''; $scope.series = ''; $scope.body_type = ''; $scope.fuel_type = ''; $scope.address = ''; $scope.function = ''; $scope.vehicle_type = ''; $scope.vehicle_usage = ''; $scope.city = "";

	    vm.jLoad = jLoad;
	    function jLoad(current) {

	        var offset = (current * 10) - 10;
	        var limit = 15;

	        var param = $scope.police_number + "|" + $scope.cost_center + "|" + $scope.fuel_amount + "|" + $scope.last_km + "|" + $scope.cost + "|" + $scope.created_date + "|" + $scope.manufacturer + "|" + $scope.model + "|" + $scope.series + "|" + $scope.body_type + "|" + $scope.fuel_type + "|" + $scope.address + "|" + $scope.function + "|" + $scope.vehicle_type + "|" + $scope.vehicle_usage + "|" + $scope.city + "|" + offset + "|" + limit;
	        //console.info(param);

	        CFMService.GetReportFuelByParam(param, function succ(reply) {
	            if (reply.status == 200) {
	                //console.info(reply.data.List);
	                var newList = [];
	                angular.forEach(reply.data.List, function (filterObj) {
	                    filterObj.fungsi = filterObj.function;
	                    delete filterObj.function;

	                    newList.push(filterObj);
	                });

	                $scope.ReportFuel = newList;
	                $scope.totalItems = reply.data.Count;
	                $scope.itemsPerPage = limit;

	                //$scope.km = function (int) {
	                //    var total = 0;
	                //    angular.forEach($scope.ReportFuel, function (el) {
	                //        total += el.last_km / el.fuel_amount;
	                //    });
	                //    return total;
	                //};

	            } else {
	                $.growl.error({ message: "Gagal mengambil data Fuel Report" })
	            }
	        });
	    }

	    $scope.$watch("currentPage", function () {
	        setPagingData($scope.currentPage);
	        vm.jLoad($scope.currentPage);
	    });

	    function setPagingData(page) {
	        if (!page) {
	            var pagedData = $scope.ReportFuel.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
	            $scope.aCandidates = pagedData;
	        }
	    }
		
		vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);
            //$window.alert("Export Excel");
			
            alasql('SELECT police_number AS `Police Number`, fuel_amount AS ` Liter`, last_km AS `Odometer`, km AS `KM/Lt`, cost_center AS `Cost`, fuel_type AS `Fuel Type`, cost AS `CC`, fungsi AS `Function`, address AS `Regional`, manufacturer AS `Make`, model AS `Model`, series AS `Series`, body_type AS `Body Type`, vehicle_type AS `Vehicle Type`, vehicle_usage AS `Vehicle Usage`, city AS `Location` INTO XLSX("ReportFuel.xlsx",{headers:true}) FROM ?', [$scope.ReportFuel]);

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
		vm.submitForm = function (filter) {
		    console.log(filter);
		    angular.element('#myFilter').trigger('click');

		    if (filter.created_date != null) {
		        $scope.created_date = filter.created_date;
		    }

		    if (filter.vehicle_type != null) {
		        $scope.vehicle_type = filter.vehicle_type;
		    }

		    if (filter.police_number != null) {
		        $scope.police_number = filter.police_number;
		    }

		    if (filter.cost_center != null) {
		        $scope.cost_center = filter.cost_center;
		    }

		    if (filter.fungsi != null) {
		        $scope.fungsi = filter.fungsi;
		    }

            var offset = 0;
            var limit = 15;

            var param = $scope.police_number + "|" + $scope.cost_center + "|" + $scope.fuel_amount + "|" + $scope.last_km + "|" + $scope.cost + "|" + $scope.created_date + "|" + $scope.manufacturer + "|" + $scope.model + "|" + $scope.series + "|" + $scope.body_type + "|" + $scope.fuel_type + "|" + $scope.address + "|" + $scope.function + "|" + $scope.vehicle_type + "|" + $scope.vehicle_usage + "|" + $scope.city + "|" + offset + "|" + limit;
		    //console.info(param);

            CFMService.GetReportFuelByParam(param, function succ(reply) {
                if (reply.status == 200) {
                    //console.info(reply.data.List);

                    var newList = [];
                    angular.forEach(reply.data.List, function (filterObj) {
                        filterObj.fungsi = filterObj.function;
                        delete filterObj.function;

                        newList.push(filterObj);
                    });

                    $scope.ReportFuel = newList;
                    $scope.totalItems = reply.data.Count;
                    $scope.itemsPerPage = limit;
                } else {
                    $.growl.error({ message: "Gagal mengambil data Fuel Report" })
                }
            });

		}
		
	}

})();