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

    angular.module("app").controller("PoDashboardCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', '$state', '$stateParams', 'CFMService', 'MstService', '$filter'];

    function ctrl($http, $translate, $scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, DTOptionsBuilder, $state, $stateParams, CFMService, MstService, $filter) {
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

        $scope.police_number = ''; $scope.supply_method = ''; $scope.employee_name = ''; $scope.cost_center = ''; $scope.make = ''; $scope.model = ''; $scope.series = ''; $scope.body_type = ''; $scope.color = ''; $scope.ChasisNumber = ''; $scope.EngineNumber = ''; $scope.VehicleType = ''; $scope.VehicleUsage = ''; $scope.contract_startdate = ''; $scope.contract_enddate = ''; $scope.vendor = ''; $scope.amount = ''; $scope.total = ''; $scope.po_number = ''; $scope.po_line = '';

	    vm.jLoad = jLoad;
	    function jLoad(current) {

	        var offset = (current * 10) - 10;
	        var limit = 15;

	        var param = $scope.police_number + "|" + $scope.supply_method + "|" + $scope.employee_name + "|" + $scope.cost_center + "|" + $scope.make + "|" + $scope.model + "|" + $scope.series + "|" + $scope.body_type + "|" + $scope.color + "|" + $scope.ChasisNumber + "|" + $scope.EngineNumber + "|" + $scope.VehicleType + "|" + $scope.VehicleUsage + "|" + $scope.contract_startdate + "|" + $scope.contract_enddate + "|" + $scope.vendor + "|" + $scope.amount + "|" + $scope.total + "|" + $scope.po_number + "|" + $scope.po_line + "|" + offset + "|" + limit;
	        //console.info(param);

	        CFMService.GetReportPOByParam(param, function succ(reply) {
	            if (reply.status == 200) {
	                //console.info(reply.data.List);
	                $scope.ReportPO = reply.data.List;

	                $scope.totalItems = reply.data.Count;
	                $scope.itemsPerPage = limit;
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
	            var pagedData = $scope.ReportPO.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
	            $scope.aCandidates = pagedData;
	        }
	    }

	    vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);
            //$window.alert("Export Excel");

            alasql('SELECT police_number AS `Police Number`, supply_method AS `Supply Method`, employee_name AS `Employee Name`, cost_center AS `Cost Center`, manufacturer AS `Make`, model AS `Model`, series AS `Series`,body_type AS `Body Type`, color AS `Exterior Colour`, start_date AS `Contract Start`, end_date AS `Contract End`, vendor_name AS `Vendor`, po_number AS `PO Number`, po_line AS `PO Line` INTO XLSX("reportpo.xlsx",{headers:true}) FROM ?', [$scope.ReportPO]);

            // Prepare Excel data:
            //$scope.fileName = "reportpo";
            //$scope.exportData = [];

            // Headers:
            //$scope.exportData.push(["Police Number", "Supply Method", "Employee Name", "Cost Center", "Make", "Model", "Series", "Body Type", "Exterior Colour", "Contract Start", "Contract End", "Vendor", "PO Number", "PO Line"]);

            // Data:
            //var param = $scope.police_number + "|" + $scope.supply_method + "|" + $scope.employee_name + "|" + $scope.cost_center + "|" + $scope.make + "|" + $scope.model + "|" + $scope.series + "|" + $scope.body_type + "|" + $scope.color + "|" + $scope.ChasisNumber + "|" + $scope.EngineNumber + "|" + $scope.VehicleType + "|" + $scope.VehicleUsage + "|" + $scope.contract_startdate + "|" + $scope.contract_enddate + "|" + $scope.vendor + "|" + $scope.amount + "|" + $scope.total + "|" + $scope.po_number + "|" + $scope.po_line + "|0|0";

            //CFMService.GetReportPOByParam(param, function succ(reply) {
            //    if (reply.status == 200) {
            //        angular.forEach(reply.data.List, function (value, key) {
            //            $scope.ReportPO.push([value.police_number, value.supply_method, value.employee_name, value.cost_center, value.make, value.model, value.series, value.body_type, value.color, value.ChasisNumber, value.EngineNumber, value.VehicleType, value.VehicleUsage, value.contract_startdate, value.vendor, value.amount, value.total, value.po_number, value.po_line]);
            //        });

            //        console.info($scope.exportData);
            //        alasql('SELECT police_number AS `Police Number`, supply_method AS `Supply Method`, employee_name AS `Employee Name`, cost_center AS `Cost Center`, manufacturer AS `Make`, model AS `Model`, series AS `Series`,body_type AS `Body Type`, color AS `Exterior Colour`, start_date AS `Contract Start`, end_date AS `Contract End`, vendor_name AS `Vendor`, po_number AS `PO Number`, po_line AS `PO Line` INTO XLSX("reportpo.xlsx",{headers:true}) FROM ?', [$scope.ReportPO]);
            //    } else {
            //        $.growl.error({ message: "Gagal mengambil data Fuel Report" })
            //    }
            //});
			
            /*var exporthref = mstservice.tabletoexcel(tabel, 'wireworkbenchdataexport');
            $timeout(function () { location.href = exporthref; }, 100);*/
        }
		
		vm.toPdf = toPdf;
        function toPdf(tabel) {
            //console.info(tabel);
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
		    angular.element('#myFilter').trigger('click');

		    var offset = 0;
		    var limit = 15;

		    if (formData.formDate != null) {
		        $scope.contract_startdate = $scope.form.formDate;
		    }

		    if (formData.formTo != null) {
		        $scope.contract_enddate = $scope.form.formTo;
		    }

		    if (formData.EmployeeName != null) {
		        $scope.employee_name = $scope.form.EmployeeName;
		    }

		    if (formData.PoliceNumber != null) {
		        $scope.police_number = $scope.form.PoliceNumber;
		    }

		    if (formData.SupplyMethod != null) {
		        $scope.supply_method = $scope.form.SupplyMethod;
		    }

		    var param = $scope.police_number + "|" + $scope.supply_method + "|" + $scope.employee_name + "|" + $scope.cost_center + "|" + $scope.make + "|" + $scope.model + "|" + $scope.series + "|" + $scope.body_type + "|" + $scope.color + "|" + $scope.ChasisNumber + "|" + $scope.EngineNumber + "|" + $scope.VehicleType + "|" + $scope.VehicleUsage + "|" + $scope.contract_startdate + "|" + $scope.contract_enddate + "|" + $scope.vendor + "|" + $scope.amount + "|" + $scope.total + "|" + $scope.po_number + "|" + $scope.po_line + "|" + offset + "|" + limit;
		    //console.info(param);

		    CFMService.GetReportPOByParam(param, function succ(reply) {
		        if (reply.status == 200) {
		            //console.info(reply.data.List);
		            $scope.ReportPO = reply.data.List;

		            $scope.totalItems = reply.data.Count;
		            $scope.itemsPerPage = limit;
		        } else {
		            $.growl.error({ message: "Gagal mengambil data Fuel Report" })
		        }
		    });

		}

		//$scope.EmployeeName = function (key,value) {
		//    console.log(key + " = " + value);
		//    $scope.form.EmployeeID = key;
		//    $scope.form.EmployeeName = value;
		//}

		//$scope.PoliceNumber = function (value) {
		//    console.log(value);
		//    $scope.form.PoliceNumber = value;
		//}
		
	}

})();