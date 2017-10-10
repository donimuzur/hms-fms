(function () {
    'use strict';

    angular.module("app").controller("PRCommodityCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PurchaseRequisitionService', '$state', 'UIControlService', 'item', '$uibModalInstance'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PurchReqService,
        $state, UIControlService, item, $uibModalInstance) {
        var vm = this;
        vm.textSearch = "";
        vm.colSearch = 1;
        vm.maxSize = 10;
        //console.info("item:" + JSON.stringify(item));
        vm.dataPR = item;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("purchase-requisition");
            vm.loadCommodity(1);
        }

        vm.onSearchSubmit = onSearchSubmit;
        function onSearchSubmit(textSearch) {
            vm.textSearch = textSearch;
            vm.loadCommodity(1);
        };

        vm.listMaterial = [];
        vm.totalItems = 0;
        vm.loadCommodity = loadCommodity;
        function loadCommodity(current) {
            UIControlService.loadLoadingModal('LOADING.VIEW_VENDOR');
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            PurchReqService.selectMaterial({
                Keyword: vm.textSearch, Column: vm.colSearch, Limit: vm.maxSize, Offset: offset
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                //console.info("material:" + JSON.stringify(reply));
                if (reply.status === 200) {
                    vm.listMaterial = reply.data.List;
                    vm.totalItems = reply.data.Count;
                } else {
                    UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
                }
            }, function (err) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
            });
        }

        vm.pilihCommodity = pilihCommodity;
        vm.senddata = [];
        function pilihCommodity(data) {
            var data = {
                ID: vm.dataPR.ID,
                Material: data.Code
            };
            vm.senddata.push(data);
           $uibModalInstance.close(vm.senddata);
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();