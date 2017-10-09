(function () {
    'use strict';

    angular.module("app").controller("ModulCCFCtrl", ctrl);

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

        $scope.request_number = ''; $scope.request_date = ''; $scope.requestor = ''; $scope.gs = ''; $scope.police_number = ''; $scope.employee_id = ''; $scope.employee_name = ''; $scope.city = ''; $scope.address = ''; $scope.coordinator_kpi = ''; $scope.vendor_kpi = ''; $scope.ccf_status = ''; $scope.modified_date = ''; $scope.modified_by = '';

        vm.jLoad = jLoad;
        function jLoad(current) {

            var offset = (current * 10) - 10;
            var limit = 15;

            var param = $scope.request_number + "|" + $scope.request_date + "|" + $scope.requestor + "|" + $scope.gs + "|" + $scope.police_number + "|" + $scope.employee_id + "|" + $scope.employee_name + "|" + $scope.city + "|" + $scope.address + "|" + $scope.coordinator_kpi + "|" + $scope.vendor_kpi + "|" + $scope.ccf_status + "|" + $scope.modified_date + "|" + $scope.modified_by + "|" + offset + "|" + limit;

            //console.info(param);

            CFMService.GetReportModulCcfByParam(param, function succ(reply) {
                if (reply.status == 200) {
                    //console.info(reply.data.List);
                    $scope.ReportCCF = reply.data.List;

                    $scope.totalItems = reply.data.Count;
                    $scope.itemsPerPage = limit;
                } else {
                    $.growl.error({ message: "Gagal mengambil data Modul Report CCF" })
                }
            });
        }

        $scope.$watch("currentPage", function () {
            if ($scope.totalItems > 0) {
                setPagingData($scope.currentPage);
                vm.jLoad($scope.currentPage);
            }
        });

        function setPagingData(page) {
            var pagedData = $scope.ReportCCF.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
            $scope.aCandidates = pagedData;
        }

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);
            //$window.alert("Export Excel");

            alasql('SELECT request_number AS `Request Number`, request_date AS `Request Date`,requestor AS `Requestor`,gs AS `GS`,police_number AS `Police Number`,employee_id AS `Employee Id`,employee_name AS `Employee Name`,city AS `City`,address AS `Address`,coordinator_kpi AS `Coordinator KPI`,vendor_kpi AS `Vendor Kpi`,ccf_status AS `CCF Status`,modified_date AS `Modified Date`,modified_by AS `Modified By` INTO XLSX("ReportCCF.xlsx",{headers:true}) FROM ?', [$scope.ReportCCF]);
        }

    }

})();