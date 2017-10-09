(function () {
    'use strict';

    angular.module("app")
    .controller("aturSubCostEstimateCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";
        vm.isTenderVerification = false;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.DETAIL_COST_ESTIMATE", href: "#/detail-cost-estimate/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.ATUR_SUB_COST_ESTIMATE", href: "" }
        ];

        vm.userBisaMengatur = false;
        vm.ceSub = [];
        vm.totalHPS = 0;
        vm.searchText = '';

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            loadCRInfo();
            vm.loadData();
        };

        function loadCRInfo() {
            DataContractRequisitionService.SelectById({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.currencySymbol = reply.data.CurrencySymbol;
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.isTenderVerification = reply.data.StatusName !== 'CR_DRAFT' && reply.data.StatusName.lastIndexOf('CR_REJECT_', 0) !== 0;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        vm.onSearchSubmit = function (searchText) {
            vm.searchText = searchText;
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SelectCESub({
                ContractRequisitionId: contractRequisitionId,
                Name: vm.searchText
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.ceSub = reply.data;
                    vm.totalHPS = 0;
                    vm.ceSub.forEach(function (sub) {
                        vm.totalHPS += sub.TotalLineCost;
                    })
                }
                else {
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_CELINE'));
                }
            }, function (err){
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        vm.romanize = romanize;
        function romanize(num) {
            if (!+num)
                return false;
            var digits = String(+num).split(""),
                    key = ["", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM",
                        "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC",
                        "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"],
                    roman = "",
                    i = 3;
            while (i--)
                roman = (key[+digits.pop() + (i * 10)] || "") + roman;
            return Array(+digits.join("") + 1).join("M") + roman;
        }

        vm.tambahSubPekerjaan = tambahSubPekerjaan;
        function tambahSubPekerjaan() {
            var lempar = {
                ceSub: {
                    ContractRequisitionCESubID: 0,
                    ContractRequisitionId: contractRequisitionId
                },
                noAddLine: false
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/aturSubCostEstimate.modal.html',
                controller: 'aturSubCostEstimateCtrlFormCtrl',
                controllerAs: 'aturSubCEFormCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadData();
            });
        };

        vm.tambahSubUntukSub = tambahSubUntukSub;
        function tambahSubUntukSub(ceSub) {

            if (ceSub.TotalLineCost > 0 && (!ceSub.Children || ceSub.Children.length === 0)) {
                UIControlService.msg_growl("error", $filter('translate')('CE.NO_ADD_SUB_COST'));
                return;
            }

            var lempar = {
                ceSub: {
                    ContractRequisitionCESubID: 0,
                    ContractRequisitionId: contractRequisitionId,
                    Parent: ceSub.ContractRequisitionCESubID,
                },
                noAddLine: false
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/aturSubCostEstimate.modal.html',
                controller: 'aturSubCostEstimateCtrlFormCtrl',
                controllerAs: 'aturSubCEFormCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadData();
            });
        };

        vm.ubah = ubah;
        function ubah(ceSub) {

            var noAddLine = false;
            if (ceSub.Children && ceSub.Children.length > 0) {
                noAddLine = true;
            }

            var lempar = {
                ceSub: ceSub,
                noAddLine: noAddLine
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/aturSubCostEstimate.modal.html',
                controller: 'aturSubCostEstimateCtrlFormCtrl',
                controllerAs: 'aturSubCEFormCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadData();
            });
        };

        vm.detail = detail;
        function detail(ContractRequisitionCESubID) {
            var lempar = {
                contractRequisitionCESubID: ContractRequisitionCESubID
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/aturSubCostEstimate.modalDet.html',
                controller: 'aturSubCostEstimateDetCtrl',
                controllerAs: 'aturSubCEDetCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
        };

        vm.hapus = hapus;
        function hapus(ceSub) {

            bootbox.confirm($filter('translate')('CE.CONFIRM_DEL_CESUB'), function (yes) {
                if (yes) {
                    UIControlService.loadLoading(loadmsg);
                    DataContractRequisitionService.DelCESub(ceSub, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_DEL_CE_SUB'));
                            vm.loadData();
                        } else {
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_DEL_CE_SUB'));
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                    });
                }
            });
        };

        vm.back = back;
        function back() {
            $state.transitionTo('detail-cost-estimate', { contractRequisitionId: contractRequisitionId });
        };
    }
})();