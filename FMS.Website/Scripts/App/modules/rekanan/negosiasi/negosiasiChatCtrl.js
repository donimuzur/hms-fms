(function () {
    'use strict';

    angular.module("app").controller("NegosiasiChatVendorCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.maxSize = 10;
        vm.init = init;
        vm.jLoad = jLoad;

        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
           loadTender();
            jLoad(1);

        }

        vm.loadTender = loadTender;
        function loadTender() {
            NegosiasiService.selectStep({ ID: vm.StepID }, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.step = reply.data;
                    vm.TenderRefID = vm.step.tender.TenderRefID;
                    vm.ProcPackType = vm.step.tender.ProcPackageType;
                    vm.TenderID = vm.step.TenderID;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Chatting" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }
        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.nego = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                Status: vm.StepID,
                Offset: offset,
                Limit: vm.pageSize
            }
            NegosiasiService.selectChatVendor(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.negoChat = reply.data.List;
                    vm.VendorName = reply.data.List[0].VendorName;
                    vm.TenderName = reply.data.List[0].TenderName;
                    vm.count = reply.data.Count;
                    console.info(JSON.stringify(vm.negoChat));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Chatting" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.detail = detail;
        function detail(flag) {
            $state.transitionTo('list-sub-nego-vendor', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('detail-tahapan-vendor', { TenderID: vm.TenderID });
        }
        
        vm.tulis = tulis;
        function tulis() {
            var data = {
                item: vm.negoChat[0],
                StepID: vm.StepID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/negosiasi/write-chat.html',
                controller: 'WriteChatVendorCtrl',
                controllerAs: 'WriteChatVendorCtrl',
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
//TODO


