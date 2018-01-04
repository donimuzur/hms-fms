(function () {
    'use strict';

    angular.module("app").controller("DashHRCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', '$uibModal', '$state', '$stateParams', 'DTOptionsBuilder', 'DTColumnBuilder',
        'GlobalConstantService','UIControlService','DashService','UploadFileConfigService', 'ExcelReaderService', 'UploaderService'];

    function ctrl($http, $scope, $translate, $window, $uibModal, $state, $stateParams, DTOptionsBuilder,DTColumnBuilder,
        GlobalConstantService, UIControlService, DashService,UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = this;
        vm.init = init;
        vm.user=localStorage.getItem('username');
        vm.user_id;
        vm.dataCSF;
        vm.roles = localStorage.getItem('roles');
        $scope.dtOptions = DTOptionsBuilder.newOptions()
        .withDisplayLength(10)
        .withOption('bLengthChange', false)
        .withOption('responsive', true)
        .withPaginationType('full_numbers');
        String.prototype.replaceAll = function (search, replacement) {
            var target = this;
            return target.replace(new RegExp(search, 'g'), replacement);
        };
        
        vm.dateFormat = dateFormat;
        function dateFormat(param) {
            param = new Date(param);
            var m_names = new Array("Jan", "Feb", "Mar",
 "Apr", "May", "Jun", "Jul", "Aug", "Sep",
 "Oct", "Nov", "Dec");
            var curr_date = param.getDate();
            var curr_month = param.getMonth();
            var curr_year = param.getFullYear();
            var date = curr_date + "-" + m_names[curr_month] + "-" + curr_year;
            return date;
        }

        function init() {
            if (vm.roles != "SYSTEM.ROLE_HR") {
                $state.go('home');
            }
            else {
                DashService.GetCoordinator(vm.user, function (reply) {
                    vm.user_id = reply.data[0]['UserID'];
                    console.info(vm.user_id);
                    vm.getCSF(vm.user_id);

                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                });
                console.info('init');
            }
        };
        vm.go = go;
        function go(param) {
            param = param.replaceAll("/", "A");
            var url = $state.href('CSF', { rq: param })
            window.open(url, "blank");
        }
        vm.getCSF = getCSF;
        function getCSF(param) {
            console.info(param);
            DashService.GetCSF(param, function (reply) {
                vm.dataCSF = reply.data;
                console.info(vm.dataCSF);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
        };
    }

})();
