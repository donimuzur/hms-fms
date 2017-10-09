(function () {
    'use strict';

    angular.module("app").controller("DetailApprovalBlacklistCtrl", ctrl);
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'BlacklistService', 'UIControlService', 'GlobalConstantService', '$state', '$uibModal'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        BlacklistService, UIControlService, GlobalConstantService, $state, $uibModal) {
        var vm = this;
        vm.init = init;
        vm.aktifasi = "";
        vm.Keyword = "";
        vm.pageSize = 10;
        function init() {
            $translatePartialLoader.addPart('blacklist-data');
            jLoad(1);
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.listApproval = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            BlacklistService.selectApproval({
                Offset: offset,
                Limit: vm.pageSize,
                Keyword: vm.Keyword
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listApproval = data.List;
                    console.info(vm.listApproval);
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ApprovalVendor" });
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
            $state.transitionTo('blacklist-data');
        }

        vm.save = save;
        function save(data) {
            UIControlService.loadLoading("Silahkan Tunggu");
            var data = {
                act: false,
                ID: data.ID,
                LType: data.LType,
                BlacklistId: data.BlacklistId,
                BlacklistType: data.BlacklistType,
                VendorID: data.VendorID,
                VendorName: data.VendorName
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/blacklist/DetailBlacklistRejectVendorCtrl.html',
                controller: 'FormWhitelistCtrl',
                controllerAs: 'FormWhitelistCtrl',
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
