(function () {
    'use strict';

    angular.module("app").controller("tenderVerTotEvalCtrl", ctrl);

    ctrl.$inject = ['$filter', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'TotalEvaluasiJasaService', '$state', 'UIControlService', 'UploadFileConfigService', 'UploaderService',
        'GlobalConstantService', '$uibModal', '$stateParams'];
    function ctrl($filter, $http, $translate, $translatePartialLoader, $location, SocketService,
        TEJService, $state, UIControlService, UploadFileConfigService, UploaderService,
        GlobalConstantService, $uibModal, $stateParams) {
        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.TenderName = '';
        vm.ProsentaseTechnical;
        vm.ProsentasePricing;
        vm.TenderID;
        vm.pageSize = 10;
        vm.Keyword = '';
        vm.init = init;
        function init() {
            getLoginCP();
            $translatePartialLoader.addPart('verifikasi-tender');
            loadMethodEval(1);
        }

        vm.getLoginCP = getLoginCP;
        function getLoginCP() {
            TEJService.getLoginCP(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.flagCheckList = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }


        function loadDataTender() {
            TEJService.getDataStepTender({
                ID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.TenderName = data.tender.TenderName;
                    vm.StartDate = UIControlService.getStrDate(data.StartDate);
                    vm.EndDate = UIControlService.getStrDate(data.EndDate);
                    vm.nama_tahapan = data.step.TenderStepName;
                    vm.TenderID = data.TenderID;

                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadMethodEval(current) {
            vm.currentPage = current;
			var offset = (current * 10) -10;
            TEJService.getDataVerification({
                Offset: offset,
                Limit: vm.pageSize,
                Keyword: vm.Keyword

            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.contracttotal = reply.data;
                    for (var i = 0; i < vm.contracttotal.length; i++) {
                        if (vm.contracttotal[i].ApprovalStatusReff.Name == "CR_APPROVED" || vm.contracttotal[i].ApprovalStatusReff.Name == "APPROVAL_VENDOR_REJECTED") {
                            console.info(vm.contracttotal[i]);
                            vm.contracttotal[i].flagButton = false;

                        }
                        else vm.contracttotal[i].flagButton = true;
                    }
                    vm.totalItems = vm.contracttotal.length;
                    if(vm.contracttotal.length != 0)
                    vm.IDTender = vm.contracttotal[0].tender.TenderRefID;
            }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.onSearchSubmit = function (searchText) {
            vm.Keyword = searchText;
            loadMethodEval(1);
        };

        vm.detailtotal = detailtotal;
        function detailtotal(detail) {
            var item = {
                TenderStepID: detail.TenderStepID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/total-evaluasi-jasa/detailTotalEvaluasi.html',
                controller: 'detTotalEvaluasiCtrl',
                controllerAs: 'detTotalEvaluasiCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

        vm.backDetailTahapan = backDetailTahapan;
        function backDetailTahapan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
        }
        vm.aturApproval = aturApproval;
        function aturApproval(dt) {
           // $state.transitionTo('verifikasi-totalEval-atur-app', { contractRequisitionId: dt.tender.TenderRefID});
          //  console.info(data);
            var data = {
                act: 0,
                contractRequisitionId: dt.tender.TenderRefID
                //,
                //item: data,
                //dataTemp: data.flag
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/total-evaluasi-jasa/commite-modal.html',
                controller: 'CommitteeModalCtrl',
                controllerAs: 'CommitteeModalCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });

        };

        vm.detailApproval = detailApproval;
        function detailApproval(dt) {
            var item = {
                TenderId: dt.tender.TenderRefID,
                Status: 1
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/total-evaluasi-jasa/detailApproval.modal.html',
                controller: 'detailApprovalEvalCtrl',
                controllerAs: 'detailApprovalEvalCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function () {
                init();
            });
        };

       
    }
})();