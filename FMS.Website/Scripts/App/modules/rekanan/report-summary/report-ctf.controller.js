(function () {
    'use strict';

    angular.module("app").controller("ModulCTFCtrl", ctrl);

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

        $scope.request_number = ''; $scope.request_date = ''; $scope.coordinator = ''; $scope.effective_date = ''; $scope.transfer_to_idle = ''; $scope.buy_cost = ''; $scope.extend_vehicle = ''; $scope.withd_pic = ''; $scope.withd_phone = ''; $scope.withd_date = ''; $scope.withd_city = ''; $scope.withd_address = ''; $scope.employee_contribution = ''; $scope.penalty = ''; $scope.refund_cost = ''; $scope.buy_cost_total = ''; $scope.user_decision = ''; $scope.penalty_po_number = ''; $scope.penalty_po_line = ''; $scope.penalty_price = ''; $scope.ctf_status = ''; $scope.created_by = ''; $scope.created_date = ''; $scope.modified_by = ''; $scope.modified_date = '';

        vm.jLoad = jLoad;
        function jLoad(current) {

            var offset = (current * 10) - 10;
            var limit = 15;

            var param = $scope.request_number + "|" + $scope.request_date + "|" + $scope.coordinator + "|" + $scope.effective_date + "|" + $scope.transfer_to_idle + "|" + $scope.buy_cost + "|" + $scope.extend_vehicle + "|" + $scope.withd_pic + "|" + $scope.withd_phone + "|" + $scope.withd_date + "|" + $scope.withd_city + "|" + $scope.withd_address + "|" + $scope.employee_contribution + "|" + $scope.penalty + "|" + $scope.refund_cost + "|" + $scope.buy_cost_total + "|" + $scope.user_decision + "|" + $scope.penalty_po_number + "|" + $scope.penalty_po_line + "|" + $scope.penalty_price + "|" + $scope.ctf_status + "|" + $scope.created_by + "|" + $scope.created_date + "|" + $scope.modified_by + "|" + $scope.modified_date + "|" + offset + "|" + limit;

            //console.info(param);

            CFMService.GetReportModulCtfByParam(param, function succ(reply) {
                if (reply.status == 200) {
                    //console.info(reply.data.List);
                    $scope.ReportCTF = reply.data.List;

                    $scope.totalItems = reply.data.Count;
                    $scope.itemsPerPage = limit;
                } else {
                    $.growl.error({ message: "Gagal mengambil data Modul Report CTF" })
                }
            });
        }

        $scope.$watch("currentPage", function () {
            if ($scope.totalItems > 0) {
                //setPagingData($scope.currentPage);
                //vm.jLoad($scope.currentPage);
            }
        });

        function setPagingData(page) {
            var pagedData = $scope.ReportCTF.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
            $scope.aCandidates = pagedData;
        }

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);
            //$window.alert("Export Excel");

            alasql('SELECT request_number AS `Request Number`, request_date AS `Request Date`, coordinator AS `Coordinator`, effective_date AS `Effective Date`, transfer_to_idle AS `Transfer Idle`, buy_cost AS `Buy Cost`, extend_vehicle AS `Extend Vehicle`, withd_pic AS `With Pic`, withd_phone AS `With Phone`, withd_date AS `With Date`, withd_city AS `With City`, withd_address AS `With Address`, employee_contribution AS `Employee Contribution`, penalty AS `Penalty`, refund_cost AS `Refund Cost`, buy_cost_total AS `Buy Total`, user_decision AS `User Decision`, penalty_po_number AS `Penalty PO Number`, penalty_po_line AS `Penalty PO Line`, penalty_price AS `Penalty Price`, ctf_status AS `CTF Status`, created_by AS `Created By`, created_date AS `Created Date`, modified_by AS `Modified By`, modified_date AS `Modified Date` INTO XLSX("ReportCTF.xlsx",{headers:true}) FROM ?', [$scope.ReportCTF]);
        }

    }

})();