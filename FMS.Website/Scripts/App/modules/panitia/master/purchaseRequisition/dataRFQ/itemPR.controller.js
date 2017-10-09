(function () {
    'use strict';

    angular.module("app")
    .controller("ItemPRCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PurchaseRequisitionService', '$state',
        'UIControlService', 'item', '$uibModalInstance'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PurchReqService, $state, UIControlService,
        item, $uibModalInstance) {
        var vm = this;
        //console.info(JSON.stringify(item));
        vm.maxSize = 10;
        vm.currentPage = 1;
        vm.totalItems = 0;
        vm.textDate = null;
        vm.textSearch = "";
        vm.listItemPR = [];
        vm.listCheckedPR = item.item;
        vm.CommodityID = item.CommodityID;
        vm.CommodityName = item.CommodityName;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("purchase-requisition");
            vm.jLoad(1);
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (vm.currentPage * vm.maxSize) - vm.maxSize;

            PurchReqService.getDataItemPR({
                ID : vm.CommodityID
            }, function (reply) {
                //console.info("dta:"+JSON.stringify(reply));
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listItemPR = data.List;                    
                    vm.totalItems = Number(data.Count);
                    if (vm.listItemPR.length > 0) {
                        for (var i = 0; i < vm.listItemPR.length; i++) {
                            var foundCheck = $.map(vm.listCheckedPR, function (val) {
                                return (val.ID == vm.listItemPR[i].ID) ? val : null;
                            });
                            if (foundCheck.length > 0) {
                                vm.listItemPR[i].IsUsed = true;
                            }
                            if (vm.listItemPR[i].InputDate === null) { vm.listItemPR[i].InputDate = "-"; }
                            else { vm.listItemPR[i].InputDate = UIControlService.getStrDate(vm.listItemPR[i].InputDate); }

                            if (vm.listItemPR[i].ApprovalDate === null) { vm.listItemPR[i].ApprovalDate = "-"; }
                            else { vm.listItemPR[i].ApprovalDate = UIControlService.getStrDate(vm.listItemPR[i].ApprovalDate); }

                            if (vm.listItemPR[i].DeliveryDate === null) { vm.listItemPR[i].DeliveryDate = "-"; }
                            else { vm.listItemPR[i].DeliveryDate = UIControlService.getStrDate(vm.listItemPR[i].DeliveryDate); }
                        }
                    }
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOADDATA");
                    UIControlService.unloadLoadingModal();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.choosePR = choosePR;
        function choosePR(row) {
            vm.listCheckedPR.push(row);
        }

        vm.checkAll = checkAll;
        vm.isCekAll;
        function checkAll() {
            console.info(vm.isCekAll);
            for (var i = 0; i < vm.listItemPR.length; i++) {
                if (vm.isCekAll === true) {
                    vm.listItemPR[i].IsUsed = true;
                    vm.listCheckedPR.push(vm.listItemPR[i]);
                }
                else {
                    vm.listItemPR[i].IsUsed = false;
                }
            }
            //vm.jLoad(1);
        }

        vm.simpanItem = simpanItem;
        function simpanItem(dataitem) {
            //console.info("x"+JSON.stringify(data));
            $uibModalInstance.close(dataitem);
        };

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();