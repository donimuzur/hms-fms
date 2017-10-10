(function () {
    'use strict';

    angular.module("app").controller("ModulCAFCtrl", ctrl);

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

        $scope.SIRS_number = ''; $scope.coordinator = ''; $scope.police_number = ''; $scope.employee_name = ''; $scope.supervisor = ''; $scope.location = ''; $scope.model = ''; $scope.vendorname = ''; $scope.incidentdate = ''; $scope.incidentlocation = ''; $scope.incidentdescription = ''; $scope.progressupdate = ''; $scope.progressdate = ''; $scope.remark = ''; $scope.estimation = ''; $scope.actual = ''; $scope.incidenttype = ''; $scope.scope = ''; $scope.modifiedby = ''; $scope.inputnonth = '';

        vm.jLoad = jLoad;
        function jLoad(current) {

            var offset = (current * 10) - 10;
            var limit = 15;

            var param = $scope.SIRS_number + "|" + $scope.coordinator + "|" + $scope.police_number + "|" + $scope.employee_name + "|" + $scope.supervisor + "|" + $scope.location + "|" + $scope.model + "|" + $scope.vendorname + "|" + $scope.incidentdate + "|" + $scope.incidentlocation + "|" + $scope.incidentdescription + "|" + $scope.progressupdate + "|" + $scope.progressdate + "|" + $scope.remark + "|" + $scope.estimation + "|" + $scope.actual + "|" + $scope.incidenttype + "|" + $scope.scope + "|" + $scope.modifiedby + "|" + $scope.inputnonth + "|" + offset + "|" + limit;

            //console.info(param);

            CFMService.GetReportModulCafByParam(param, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data.List);
                    $scope.ReportCAF = reply.data.List;

                    $scope.totalItems = reply.data.Count;
                    $scope.itemsPerPage = limit;
                } else {
                    $.growl.error({ message: "Gagal mengambil data Modul Report CAF" })
                }
            });
        }

        $scope.$watch("currentPage", function () {
            setPagingData($scope.currentPage);
            vm.jLoad($scope.currentPage);
        });

        function setPagingData(page) {
            var pagedData = $scope.ReportCAF.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
            $scope.aCandidates = pagedData;
        }

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);
            //$window.alert("Export Excel");

            alasql('SELECT SIRS_number AS `Sirs Number`, coordinator AS `Coordinator`, police_number AS `Police Number`, employee_name AS `Employee Name`, supervisor AS `Supervisor`, location AS `Location`, model AS `Model`, vendor_name AS `Vendor Name`, incident_date AS `Incident Date`, incident_location AS `Incident Location`, incident_description AS `Incident Description`, progress_update AS `Progress Update`, progress_date AS `Progress Date`, remark AS `Remark`, estimation AS `Estimation`, actual AS `Actual`, incident_type AS `Incident Type`, scope AS `Scope`, modified_by AS `ModifiedBy`, input_month AS `InputNonth` INTO XLSX("ReportCAF.xlsx",{headers:true}) FROM ?', [$scope.ReportCAF]);
        }

    }

})();