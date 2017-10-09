(function () {
    'use strict';

    angular.module("app")
    .controller("DetailItemPRCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PurchaseRequisitionService', '$state',
        'UIControlService', 'item', '$uibModalInstance'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PurchReqService, $state, UIControlService,
        item, $uibModalInstance) {
        var vm = this;
        console.info("det>>"+JSON.stringify(item));
        vm.maxSize = 10;
        vm.currentPage = 1;
        vm.totalItems = 0;
        vm.textDate = null;
        vm.textSearch = "";
        vm.listItemPR = [];
        vm.listCheckedPR = item.item;
        vm.CommodityID = item.CommodityID;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("purchase-requisition");
            vm.listItemPR = item.item;
            for (var i = 0; i < vm.listItemPR.length; i++) {
                vm.listItemPR[i].InputDate = UIControlService.getStrDate(vm.listItemPR[i].InputDate);
                vm.listItemPR[i].ApprovalDate = UIControlService.getStrDate(vm.listItemPR[i].ApprovalDate);
                vm.listItemPR[i].DeliveryDate = UIControlService.getStrDate(vm.listItemPR[i].DeliveryDate);
            }
            //vm.jLoad(1);
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();