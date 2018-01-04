(function () {
    'use strict';

    angular.module("app")
    .controller("aturSubCostEstimateCtrlFormCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'item'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, item) {

        var vm = this;
        var loadmsg = "";

        vm.ceSub = item.ceSub;
        vm.ceSubName = item.ceSub.Name;
        vm.noAddLine = item.noAddLine;
        vm.count;
        vm.pageNumber = 1;
        vm.pageSize = 10;
        vm.searchText = "";
        vm.ceLines = [];
        vm.ceLinesPaged = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            if (vm.noAddLine) {
                vm.ceLines = [];
                vm.ceLinesPaged = [];
                vm.count = 0;
            } else {
                UIControlService.loadLoadingModal(loadmsg);
                DataContractRequisitionService.SelectAvailableCELine(vm.ceSub, function (reply) {
                    if (reply.status === 200) {
                        vm.ceLines = reply.data;
                        vm.count = vm.ceLines.length;
                        vm.pageCELines();
                        DataContractRequisitionService.SelectCESubDet(vm.ceSub, function (reply) {
                            if (reply.status === 200) {
                                var ceSubDets = reply.data;
                                for (var i = 0; i < vm.ceLines.length; i++) {
                                    vm.ceLines[i].checked = false;
                                    for (var j = 0; j < ceSubDets.length; j++) {
                                        if (vm.ceLines[i].ContractRequisitionCELineID === ceSubDets[j].ContractRequisitionCELineID) {
                                            vm.ceLines[i].checked = true;
                                            break;
                                        }
                                    }
                                }
                            } else {
                                UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_CELINE'));
                            }
                            UIControlService.unloadLoadingModal();
                        }, function (error) {
                            UIControlService.unloadLoadingModal();
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                        });
                    } else {
                        UIControlService.unloadLoadingModal();
                        UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_CELINE'));
                    }
                }, function (error) {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                });
            }
        };

        //Belum dipakai
        vm.onSearchSubmit = onSearchSubmit;
        function onSearchSubmit(searchText) {
            vm.searchText = searchText;
            vm.pageNumber = 1;
            vm.pageCELines();
        };

        vm.pageCELines = pageCELines;
        function pageCELines() {
            vm.ceLinesPaged = [];
            var index = -1;
            var offset = vm.pageSize * (vm.pageNumber - 1);
            var keyword = vm.searchText.toLowerCase();
            var pageItemCount = 0;
            vm.count = 0;
            for (var i = 0; i < vm.ceLines.length; i++) {
                if (vm.ceLines[i].Name.toLowerCase().indexOf(keyword) >= 0) {
                    index += 1;
                    if (index >= offset && pageItemCount < vm.pageSize) {
                        vm.ceLinesPaged.push(vm.ceLines[i]);
                        pageItemCount += 1;
                    }
                    vm.count++;
                }
            };
        };
        
        vm.simpan = simpan;
        function simpan() {

            if (!vm.ceSubName) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_CESUB_NONAME'));
                return;
            }

            vm.ceSub.ContractRequisitionCESubDetails = [];
            vm.ceLines.forEach(function (line) {
                if (line.checked) {
                    vm.ceSub.ContractRequisitionCESubDetails.push({
                        ContractRequisitionCELineID: line.ContractRequisitionCELineID,
                        ContractRequisitionCESubID: vm.ceSub.ContractRequisitionCESubID,
                        IsActive: true
                    });
                }
            });

            var oldName = vm.ceSub.Name;
            vm.ceSub.Name = vm.ceSubName;
            UIControlService.loadLoadingModal(loadmsg);
            DataContractRequisitionService.SaveCESub(vm.ceSub, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_CE_SUB'));
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_CE_SUB'));
                    vm.ceSub.Name = oldName;
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                vm.ceSub.Name = oldName;
            });
        };
        vm.batal = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();