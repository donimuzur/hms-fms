(function () {
    'use strict';

    angular.module("app").controller("ApprovalMin3Ctrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ApprovalForMin3Service', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, ApprovalForMin3Service,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.init = init;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;

        function init() {
            $translatePartialLoader.addPart('tender-step-approval');
            UIControlService.loadLoading("Silahkan Tunggu...");
            loadDataApproval(1);
        }

        vm.loadDataApproval = loadDataApproval;
        function loadDataApproval(current) {
            vm.dataApproval = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            ApprovalForMin3Service.select({
                Keyword: null,
                column: 0,
                Offset: offset,
                Limit:vm.pageSize
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataApproval = reply.data.List;
                    vm.totalItems = reply.data.Count;
                    vm.maxSize = vm.totalItems;
                    console.info("dataApproval:" + JSON.stringify(vm.dataApproval));
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.approve = approve;
        function approve(data) {
            console.info("data:" + JSON.stringify(data));
            ApprovalForMin3Service.approve({
                Id:data.TenderStepDataId,
                ApprovalComment:"approve"
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil approve");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal approve");
                UIControlService.unloadLoading();
            });
        }

        vm.reject = reject;
        function reject(data) {
            console.info("data:" + JSON.stringify(data));
            ApprovalForMin3Service.reject({
                Id: data.TenderStepDataId,
                ApprovalComment: "reject"
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil reject");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal reject");
                UIControlService.unloadLoading();
            });
        }

        vm.approvalModal = approvalModal;
        function approvalModal(list, flag) {
            console.info("flag:" + flag);
            console.info("list:" + JSON.stringify(list));
            
            var data = {
                item: list,
                act: flag
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/approval-min3/approvalModal.html',
                controller: 'ApprovalMin3ModalCtrl',
                controllerAs: 'ApprovalMin3ModalCtrl',
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

        vm.backpengadaan = backpengadaan;
        function backpengadaan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
        }



    }
})();


