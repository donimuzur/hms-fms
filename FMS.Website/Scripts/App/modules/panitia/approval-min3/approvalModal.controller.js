(function () {
    'use strict';

    angular.module("app").controller("ApprovalMin3ModalCtrl", ctrl);

    ctrl.$inject = ['item', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ApprovalForMin3Service', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService', '$uibModalInstance'];
    function ctrl(item, $http, $translate, $translatePartialLoader, $location, SocketService, ApprovalForMin3Service,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService, $uibModalInstance) {

        var vm = this;
        vm.init = init;
        vm.item = item.item;
        vm.act = item.act;
       // vm.jLoad = jLoad;

        function init() {
            console.info("item:" + JSON.stringify(vm.item));
            console.info("act:" + JSON.stringify(vm.act));
            UIControlService.loadLoading("Silahkan Tunggu...");
            loadDataApproval();

        }


        vm.loadDataApproval = loadDataApproval;
        function loadDataApproval() {
            vm.dataApproval = [];
            ApprovalForMin3Service.select({
                Keyword: null,
                column: 0,
                Offset: 0,
                Limit: 10
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataApproval = reply.data.List;
                    console.info("dataApproval:" + JSON.stringify(vm.dataApproval));
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
        
        vm.action= action;
        function action() {
            var data = {
                Id: vm.item.TenderStepDataId,
                ApprovalComment: vm.comment
            };
            ApprovalForMin3Service.reject(data,
               function (reply) {
                   UIControlService.unloadLoadingModal();
                   if (reply.status === 200) {
                       UIControlService.msg_growl("success", "Proses reject berhasil");
                       $uibModalInstance.close();

                   }
                   else {
                       UIControlService.msg_growl("error", "Proses reject gagal");
                       return;
                   }
               },
               function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses Api!!");
                   console.info("error:" + JSON.stringify(err));
                   UIControlService.unloadLoadingModal();
               }
           );
        }
        vm.batal = batal;
        function batal() {
            $uibModalInstance.close();
        }

    }
})();
//TODO


