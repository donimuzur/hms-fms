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

    angular.module("app").controller("CFMDashboardCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', '$state', '$stateParams', 'CFMService', '$filter'];

    function ctrl($http, $translate, $scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, DTOptionsBuilder, $state, $stateParams, CFMService, $filter) {
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

        $scope.policenumber = '';
        $scope.make = '';
        $scope.model = '';
        $scope.series = '';
        $scope.bodytype = '';
        $scope.color = '';
        $scope.group = '';
        $scope.costcenter = '';
        $scope.supplier = '';
        $scope.startcontract = '';
        $scope.endcontract = '';
        $scope.startidle = '';
        $scope.endidle = '';
        $scope.idleduration = '';
        $scope.monthinstalment = '';
        $scope.totmonthinstalment = '';

        vm.jLoad = jLoad;
        function jLoad(current) {

            UIControlService.loadLoading("Loading");

            var offset = (current * 10) - 10;
            var limit = 15;

            var param = $scope.policenumber + "|" + $scope.make + "|" + $scope.model + "|" + $scope.series + "|" + $scope.bodytype + "|" + $scope.color + "|" + $scope.group + "|" + $scope.costcenter + "|" + $scope.supplier + "|" + $scope.startcontract + "|" + $scope.endcontract + "|" + $scope.startidle + "|" + $scope.endidle + "|" + $scope.idleduration + "|" + $scope.monthinstalment + "|" + $scope.totmonthinstalment + "|" + offset + "|" + limit;
            console.info(param);

            CFMService.GetReportCFMIdleByParam(param, function succ(reply) {
                if (reply.status == 200) {
                    UIControlService.unloadLoading();
                    //console.info(reply.data.List);
                    $scope.IdleVehicles = reply.data.List;

                    $scope.totalItems = reply.data.Count;
                    $scope.itemsPerPage = limit;
                } else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Gagal mengambil data CSF" })
                }
            });
        }

        $scope.$watch("currentPage", function () {
            //setPagingData($scope.currentPage);
            vm.jLoad($scope.currentPage);
        });

        function setPagingData(page) {
            var pagedData = $scope.IdleVehicles.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
            $scope.aCandidates = pagedData;
        }

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);
            alasql('SELECT police_number AS `Police Number`, make AS `Make`, model AS `Model`, series AS `Series`, body_type AS `Body Type`, color AS `Colour`, car_group_level AS `Group`, start_contract AS `Start Contract`, end_contract AS `End Contract`, supplier AS `Supplier`, cost_center AS `Cost Center`, start_idle AS `Start Idle`, end_idle AS `End Idle`, idle_duration AS `Idle Duration`, monthly_instaltement AS `Monthly Instalment`, total_monthly_instaltement AS `Total Monthly Instalment` INTO XLSX("ReportCFMIdle.xlsx",{headers:true}) FROM ?', [$scope.IdleVehicles]);
        }

        vm.toPdf = toPdf;
        function toPdf(tabel) {
            console.info(tabel);
            $window.alert("Export PDF");
        }

        /*
         * Simpan Form
         */

        var formData = {};
        $scope.submitForm = function () {
            formData = $scope.form;
            if (formData) {
                console.log(formData);
                //$window.alert("Submit");
                angular.element('#myFilter').trigger('click');

                if (formData.CostCenter != null) {
                    $scope.costcenter = $scope.form.CostCenter;
                }

                if (formData.PoliceNumber != null) {
                    $scope.policenumber = $scope.form.PoliceNumber;
                }

                if (formData.formDate != null) {
                    $scope.startidle = $scope.form.formDate;
                }

                if (formData.formTo != null) {
                    $scope.endidle = $scope.form.formTo;
                }

                var offset = 0; //(current * 10) - 10;
                var limit = 15;

                var param = $scope.policenumber + "|" + $scope.make + "|" + $scope.model + "|" + $scope.series + "|" + $scope.bodytype + "|" + $scope.color + "|" + $scope.group + "|" + $scope.costcenter + "|" + $scope.supplier + "|" + $scope.startcontract + "|" + $scope.endcontract + "|" + $scope.startidle + "|" + $scope.endidle + "|" + $scope.idleduration + "|" + $scope.monthinstalment + "|" + $scope.totmonthinstalment + "|" + offset + "|" + limit;
                console.info(param);

                CFMService.GetReportCFMIdleByParam(param, function succ(reply) {
                    if (reply.status == 200) {
                        //vm.statusCSF = reply.data;
                        $scope.IdleVehicles = reply.data.List;
                        console.info(reply.data.List);
                    } else {
                        $.growl.error({ message: "Gagal mengambil data CFM Idle" })
                    }
                });
            }

        }


    }

})();