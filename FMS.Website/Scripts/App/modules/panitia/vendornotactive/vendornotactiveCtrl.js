(function () {
    'use strict';

    angular.module("app").controller("VendorNotActiveCtrl", ctrl);
    ctrl.$inject = ['$state','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'VendorNotActiveService', 'RoleService', 'UIControlService', '$uibModal'];
    function ctrl($state, $http, $translate, $translatePartialLoader, $location, SocketService, VendorNotActiveService,
        RoleService, UIControlService, $uibModal) {
        var vm = this;
        vm.init = init;
        vm.aktifasi = "";
        vm.Keyword = "";
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.currentPage = 1;
        function init() {
           // $translatePartialLoader.addPart('blacklist-data');
            jLoad(1);
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            vm.listVendor = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            VendorNotActiveService.SelectAll({
                Offset: offset,
                Limit: vm.pageSize,
                Keyword: vm.Keyword
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listVendor = data.List;
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan tidak aktif" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $state.go('homeadmin');
        }

        vm.openform = openform;
        function openform(act, data) {
            var data = {
                act: act,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/vendornotactive/formVendornotactive.html',
                controller: 'formVendornotactiveCtrl',
                controllerAs: 'formVendornotactiveCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }
       
    }
})();
