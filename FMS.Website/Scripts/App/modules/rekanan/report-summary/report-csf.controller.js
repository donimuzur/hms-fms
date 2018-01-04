(function () {
    'use strict';

    angular.module("app").controller("ModulCSFCtrl", ctrl);

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
            if (vm.roles == 'SYSTEM.ROLE_HR' || vm.roles == 'SYSTEM.ROLE_FLEET') {
                vm.jLoad(1);
            } else {
                $state.go('home');
            }
        }

        $scope.requestnumber = ''; $scope.requestdate = ''; $scope.coordinator = ''; $scope.effectivedate = ''; $scope.refrencenumber = ''; $scope.employeeid = ''; $scope.employeename = ''; $scope.costcenter = ''; $scope.grouplevel = ''; $scope.assignedto = ''; $scope.vehicletype = ''; $scope.vehicleusage = ''; $scope.flexipoint = ''; $scope.policenumber = ''; $scope.manufacturer = ''; $scope.model = ''; $scope.series = ''; $scope.bodytype = ''; $scope.color = ''; $scope.contractstart = ''; $scope.contractend = ''; $scope.expecteddate = ''; $scope.supplymethode = ''; $scope.isproject = ''; $scope.starrent = ''; $scope.vehiclestatus = ''; $scope.temporarycar = ''; $scope.coordinator2 = ''; $scope.modifyby = '';

        vm.jLoad = jLoad;
        function jLoad(current) {

            var offset = (current * 10) - 10;
            var limit = 15;

            var param = $scope.requestnumber + "|" + $scope.requestdate + "|" + $scope.coordinator + "|" + $scope.effectivedate + "|" + $scope.refrencenumber + "|" + $scope.employeeid + "|" + $scope.employeename + "|" + $scope.costcenter + "|" + $scope.grouplevel + "|" + $scope.assignedto + "|" + $scope.vehicletype + "|" + $scope.vehicleusage + "|" + $scope.flexipoint + "|" + $scope.policenumber + "|" + $scope.manufacturer + "|" + $scope.model + "|" + $scope.series + "|" + $scope.bodytype + "|" + $scope.color + "|" + $scope.contractstart + "|" + $scope.contractend + "|" + $scope.expecteddate + "|" + $scope.supplymethode + "|" + $scope.isproject + "|" + $scope.starrent + "|" + $scope.vehiclestatus + "|" + $scope.temporarycar + "|" + $scope.coordinator2 + "|" + $scope.modifyby + "|" + offset + "|" + limit;

            //console.info(param);

            CFMService.GetReportCSFByParam(param, function succ(reply) {
                if (reply.status == 200) {
                    //console.info(reply.data.List);
                    $scope.ReportCSF = reply.data.List;

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
            var pagedData = $scope.ReportCSF.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
            $scope.aCandidates = pagedData;
        }

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);
            //$window.alert("Export Excel");

            alasql('SELECT request_number AS `Request Number`, request_date AS `Request Date`, coordinator AS `Coordinator`, new_cost_center AS `New Cost Center`, new_location AS `New Location`, new_address AS `New Address`, effective_date AS `Effective Date`, temporary_deliverable_date AS `Temporary Deliverable Date`, relocation_type AS `Relocation Type`, police_number AS `Police Number`, manufacturer AS `Manufacturer`, model AS `Model`, series AS `Series`, body_type AS `Body Type`, vendor AS `Vendor`, start_period AS `Start Period`, end_period AS `End Period`, withd_city AS `With City`, withd_address AS `With Adress`, withd_pic AS `With Pic`, withd_phone AS `With Phone`, withd_datetime AS `With DateTime`, deliv_city AS `Delive City`, deliv_address AS `Delive Adress`, deliv_pic AS `Delive Pic`, deliv_phone AS `Delive Phone`, expected_date AS `Expected Date`, change_police_number AS `Change Police Number`, new_police_number AS `New Police Number`, relocate_po_line AS `Relocate POLine`, relocate_po_number AS `Relocate PoNumber`, modified_by AS `Modify By` INTO XLSX("ReportCSF.xlsx",{headers:true}) FROM ?', [$scope.ReportCSF]);
        }

    }

})();