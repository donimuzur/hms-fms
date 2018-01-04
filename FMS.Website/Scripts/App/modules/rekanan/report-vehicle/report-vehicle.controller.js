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

    angular.module("app").controller("VehicleDashboardCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', 'CFMService', 'CSFService', '$state', '$stateParams', '$filter'];
	
    function ctrl($http, $translate, $scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, DTOptionsBuilder, CFMService, CSFService, $state, $stateParams, $filter) {
        var vm = $scope;

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

        $scope.police_number = ''; $scope.employee_id = ''; $scope.employee_name = ''; $scope.cost_center = ''; $scope.manufacturer = ''; $scope.model = ''; $scope.series = ''; $scope.transmission = ''; $scope.body_type = ''; $scope.fuel_type = ''; $scope.branding = ''; $scope.color = ''; $scope.airbag = ''; $scope.chasis_number = ''; $scope.engine_number = ''; $scope.vehicle_status = ''; $scope.vehicle_type = ''; $scope.purpose = ''; $scope.start_contract = ''; $scope.end_contract = ''; $scope.vendor_name = ''; $scope.address = ''; $scope.supply_method = ''; $scope.termination_date = ''; $scope.restitution = ''; $scope.price = ''; $scope.vat = ''; $scope.total_monthly = ''; $scope.po_number = ''; $scope.po_line = ''; $scope.function = ''; $scope.city = '';

        vm.jLoad = jLoad;
        function jLoad(current) {

            //UIControlService.loadLoading("Loading");

            var offset = (current * 10) - 10;
            var limit = 10;

            var param = $scope.police_number + "|" + $scope.employee_id + "|" + $scope.employee_name + "|" + $scope.cost_center + "|" + $scope.manufacturer + "|" + $scope.model + "|" + $scope.series + "|" + $scope.transmission + "|" + $scope.body_type + "|" + $scope.fuel_type + "|" + $scope.branding + "|" + $scope.color + "|" + $scope.airbag + "|" + $scope.chasis_number + "|" + $scope.engine_number + "|" + $scope.vehicle_status + "|" + $scope.vehicle_type + "|" + $scope.purpose + "|" + $scope.start_contract + "|" + $scope.end_contract + "|" + $scope.vendor_name + "|" + $scope.address + "|" + $scope.supply_method + "|" + $scope.termination_date + "|" + $scope.restitution + "|" + $scope.price + "|" + $scope.vat + "|" + $scope.total_monthly + "|" + $scope.po_number + "|" + $scope.po_line + "|" + $scope.function + "|" + $scope.city + "|" + offset + "|" + limit;            
            //console.info(param);

            CFMService.GetReportVehicleByParam(param, function succ(reply) {
                if (reply.status == 200) {
                    //UIControlService.unloadLoading();
                    //console.info(reply.data.List);
                    $scope.VehicleReport = reply.data.List;

                    $scope.totalItems = reply.data.Count;
                    $scope.itemsPerPage = limit;
                } else {
                    //UIControlService.unloadLoading();
                    $.growl.error({ message: "Gagal mengambil data Vehicle Report" })
                }
            });
        }

		vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);

            alasql('SELECT police_number AS `Police Number`, chasis_number AS `Chasis Number`, engine_number AS `Engine Number`, employee_id AS `Employee ID`, employee_name AS `Employee Name`, cost_center AS `Cost Center`, manufacturer AS `Make`, model AS `Model`, series AS `Series`, transmission AS `Transmission`, body_type AS `Body Type`, fuel_type AS `Fuel`, branding AS `Branding`, color AS `Colour`, airbag AS `Air Bag`, abs AS `ABS`, vehicle_status AS `Vehicle Status`, vehicle_type AS `Vehicle Type`, purpose AS `Purpose`, start_contract AS `Start Rent`, end_contract AS `End Rent`, vendor_name AS `Vendor`, address AS `Current Location`, supply_method AS `Supply Method`, termination_date AS `Termination Date`, restitution AS `Restitution`, price AS `Monthly Installment`, vat AS `VAT`, Total_Monthly AS `Total Monthly Charge`, po_number AS `PO Number`, po_line AS `PO Line`, city AS `Regional` INTO XLSX("ReportVehicle.xlsx",{headers:true}) FROM ?', [$scope.VehicleReport]);
			
            /*var exportHref;
            exportHref = MstService.tableToExcel(tabel, 'tabel epaf');
            $timeout(function () { location.href = exportHref; }, 100);*/
        }
		
		vm.toPdf = toPdf;
        function toPdf(tabel) {
            //console.info(tabel);
            //$window.alert("Export PDF");            
			
            /*var exportHref;
            exportHref = MstService.tableToExcel(tabel, 'tabel epaf');
            $timeout(function () { location.href = exportHref; }, 100);*/
        }
		
	    /*
         * Simpan Form
         */

        vm.submitForm = function (filter) {
            console.log(filter);
            //$window.alert("Submit");
            angular.element('#myFilter').trigger('click');

            if (filter.CostCenter != null) {
                $scope.costcenter = filter.CostCenter;
            }

            if (filter.police_number != null) {
                $scope.policenumber = filter.police_number;
            }

            if (filter.formDate != null) {
                $scope.startidle = filter.formDate;
            }

            if (filter.formTo != null) {
                $scope.endidle = filter.formTo;
            }

            var offset = 0; //(current * 10) - 10;
            var limit = 10;

            var param = $scope.police_number + "|" + $scope.employee_id + "|" + $scope.employee_name + "|" + $scope.cost_center + "|" + $scope.manufacturer + "|" + $scope.model + "|" + $scope.series + "|" + $scope.transmission + "|" + $scope.body_type + "|" + $scope.fuel_type + "|" + $scope.branding + "|" + $scope.color + "|" + $scope.airbag + "|" + $scope.chasis_number + "|" + $scope.engine_number + "|" + $scope.vehicle_status + "|" + $scope.vehicle_type + "|" + $scope.purpose + "|" + $scope.start_contract + "|" + $scope.end_contract + "|" + $scope.vendor_name + "|" + $scope.address + "|" + $scope.supply_method + "|" + $scope.termination_date + "|" + $scope.restitution + "|" + $scope.price + "|" + $scope.vat + "|" + $scope.total_monthly + "|" + $scope.po_number + "|" + $scope.po_line + "|" + $scope.function + "|" + $scope.city + "|" + offset + "|" + limit;
            //console.info(param);

            CFMService.GetReportVehicleByParam(param, function succ(reply) {
                if (reply.status == 200) {
                    //console.info(reply.data.List);
                    //vm.statusCSF = reply.data;
                    $scope.IdleVehicles = reply.data.List;
                } else {
                    $.growl.error({ message: "Gagal mengambil data CFM Idle" })
                }
            });

        }
		
	}

})();