(function () {
    'use strict';

    angular.module("app")
    .controller("requisitionListTotEvalCtrl", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TotalEvaluasiJasaService',  'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, TEJService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.flagEmployee = {};
        vm.contractRequisition = [];
        vm.Keyword = '';
        vm.flagEmployee = 0;
        vm.statusLabels = [];
        vm.statusLabels["CR_PROCESS_3"] = 'STATUS.REVIEWED';
        vm.statusLabels["CR_REJECT_3"] = 'STATUS.PENDING';
        vm.statusLabels["CR_APPROVED"] = 'STATUS.APPROVED';

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('requisition-list');
            loadMethodEval(1);
        };
        function loadMethodEval(current) {
            vm.contracttotal = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            TEJService.getDataVerification({
                Offset: offset,
                Limit: vm.pageSize,
                Keyword: vm.Keyword

            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.list = reply.data;
                    for (var i = 0; i < vm.list.length; i++) {
                        if (vm.list[i].ApprovalStatusReff.Name !== "CR_PROCESS_1") vm.contracttotal.push(vm.list[i]);
                    }
                    if (vm.contracttotal.length != 0) {
                        vm.IDTender = vm.contracttotal[0].tender.TenderRefID;
                        vm.totalItems = vm.contracttotal.length;
                        TEJService.CekEmployee({
                            ID: vm.contracttotal[0].TotalEvaluationId

                        }, function (reply) {
                            UIControlService.unloadLoading();
                            if (reply.status === 200) {
                                vm.list == reply.data;

                            }
                        }, function (err) {
                            UIControlService.msg_growl("error", "MESSAGE.API");
                            UIControlService.unloadLoading();
                        });
                    }
                    
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

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


        vm.onSearchSubmit = function (searchText) {
            vm.Keyword = searchText;
            loadMethodEval(1);
        };

        vm.onFilterTypeChange = onFilterTypeChange;
        function onFilterTypeChange(column) {
            vm.column = column;
        }

        vm.loadContracts = loadContracts;
        function loadContracts() {
            UIControlService.loadLoading(loadmsg);
            TotalEvaluasiJasaService.SelectCR({
                Keyword: vm.keyword,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.column
            }, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.contractRequisition = reply.data.List;
                    vm.totalItems = reply.data.Count;
                } else {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
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

        vm.menujuDokumen = menujuDokumen;
        function menujuDokumen(dt) {
            $state.transitionTo('contract-requisition-docs-tc', { contractRequisitionId: dt.ContractRequisitionId });
        };

        vm.detailContract = detailContract;
        function detailContract(dt) {
            $state.transitionTo('detail-contract-requisition-tc', { contractRequisitionId: dt.ContractRequisitionId });
        };

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }

        vm.sendToApproval = sendToApproval;
        function sendToApproval(data) {
            var item = {
                ID: data.TotalEvaluationId,
                TenderId: data.tender.TenderRefID,
                Status: 0
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

            //var flagStatus = false;
            //for (var i = 0; i < vm.contracttotal.length; i++) {
            //    if (vm.contracttotal[i].Status == 1) {
            //        flagStatus = true;
            //    }
            //}
            //if (flagStatus == false || (flagStatus == true && data.ApprovalStatusReff.Value === 'Tender Commitee Process')) {
            //    var item = {
            //        ID: data.TotalEvaluationId,
            //        TenderId: data.tender.TenderRefID,
            //        Status: 0
            //    };
            //    var modalInstance = $uibModal.open({
            //        templateUrl: 'app/modules/panitia/total-evaluasi-jasa/detailApproval.modal.html',
            //        controller: 'detailApprovalEvalCtrl',
            //        controllerAs: 'detailApprovalEvalCtrl',
            //        resolve: { item: function () { return item; } }
            //    });
            //    modalInstance.result.then(function () {
            //        init();
            //    });
            //}
            //else {
            //    var dt = {
            //        ID: data.TotalEvaluationId,
            //        TenderReffId: data.tender.TenderRefID,
            //        Status: 0
            //    };
            //    bootbox.confirm($filter('translate')('CONFIRM.SEND_FOR_APPROVAL'), function (yes) {
            //        if (yes) {
            //            UIControlService.loadLoading(loadmsg);
            //            TEJService.SetApprovalStatus(dt, function (reply) {
            //                if (reply.status === 200) {
            //                    UIControlService.unloadLoading();
            //                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SEND_TO_APPRV'));
            //                    init();
            //                } else {
            //                    UIControlService.unloadLoading();
            //                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
            //                }
            //            }, function (error) {
            //                UIControlService.unloadLoading();
            //                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
            //            });
            //        }
            //    });
            //}
            
        }
    }
})();