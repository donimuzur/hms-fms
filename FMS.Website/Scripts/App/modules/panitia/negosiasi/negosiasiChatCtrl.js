(function () {
    'use strict';

    angular.module("app").controller("NegosiasiChatCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.StepID = Number($stateParams.StepID);
        vm.NegoId = Number($stateParams.NegoId);
        vm.VendorID = Number($stateParams.VendorID);
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.maxSize = 10;
        vm.init = init;
        vm.TenderRefID = 0;
        vm.ProcPackType = 0;
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
                    console.info(JSON.stringify(vm.step));
                    loadVendor();
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

        vm.loadVendor = loadVendor;
        function loadVendor() {
            NegosiasiService.selectVendorName({VendorID: vm.VendorID}, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.Vendor = reply.data;
                    console.info(JSON.stringify(vm.nego));
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
                column: vm.VendorID,
                Offset: offset,
                Limit: vm.pageSize
            }
            NegosiasiService.selectChat(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.negoChat = reply.data.List;
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
            $state.transitionTo('list-sub-nego', { TenderRefID: vm.TenderRefID, VendorID: vm.VendorID, ProcPackType: vm.ProcPackType });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('negosiasi', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType, StepID: vm.StepID });
        }
        
        vm.tulis = tulis;
        function tulis() {
            var data = {
                NegoId: vm.NegoId,
                VendorID: vm.VendorID,
                StepID: vm.StepID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/negosiasi/write-chat.html',
                controller: 'WriteChatCtrl',
                controllerAs: 'WriteChatCtrl',
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


