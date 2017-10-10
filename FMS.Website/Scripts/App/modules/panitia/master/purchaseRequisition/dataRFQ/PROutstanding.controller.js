(function () {
    'use strict';

    angular.module("app")
    .controller("PROutstandingCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PurchaseRequisitionService', '$state',
        'UIControlService', '$uibModal'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PurchReqService, $state,
        UIControlService, $uibModal) {
        var vm = this;
        vm.maxSize = 10;
        vm.currentPage = 1;
        vm.totalItems = 0;
        vm.textSearch = "";
        vm.listItemPR = [];
        vm.commodities = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("purchase-requisition");
            loadCommodities();
        }

        vm.onSearchSubmit = onSearchSubmit;
        function onSearchSubmit(textSearch) {
            vm.textSearch = textSearch;
            jLoad(1);
        };

        function loadCommodities(){
            UIControlService.loadLoading("Silahkan Tunggu...");
            PurchReqService.getCommodity(
                function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        vm.commodities = reply.data;
                        jLoad(1);                   
                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_LOADDATA");
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOADDATA");
                    UIControlService.unloadLoading();
                });
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (vm.currentPage * vm.maxSize) - vm.maxSize;

            PurchReqService.getDataPROutstanding({
                column : 4, Keyword: vm.textSearch, Offset: offset, Limit: vm.maxSize
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listItemPR = data.List;                    
                    vm.totalItems = Number(data.Count);
                    for (var i = 0; i < vm.listItemPR.length; i++) {
                        if (vm.listItemPR[i].InputDate === null) { vm.listItemPR[i].InputDate = "-"; }
                        else { vm.listItemPR[i].InputDate = UIControlService.getStrDate(vm.listItemPR[i].InputDate); }

                        if (vm.listItemPR[i].ApprovalDate === null) { vm.listItemPR[i].ApprovalDate = "-"; }
                        else { vm.listItemPR[i].ApprovalDate = UIControlService.getStrDate(vm.listItemPR[i].ApprovalDate); }

                        if (vm.listItemPR[i].DeliveryDate === null) { vm.listItemPR[i].DeliveryDate = "-"; }
                        else { vm.listItemPR[i].DeliveryDate = UIControlService.getStrDate(vm.listItemPR[i].DeliveryDate); }
                    }
                   
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOADDATA");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_LOADDATA");
                UIControlService.unloadLoading();
            });
        }

        /*open form pilih item PR*/
        /*
        vm.openCommodity = openCommodity;
        function openCommodity(data) {
            var modalInstance = $uibModal.open({
                templateUrl: 'addCommodity.html',
                controller: 'PRCommodityCtrl',
                controllerAs: 'PRCommodityCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function (dataitem) {
                updateMaterial(dataitem);
            });
        }

        function updateMaterial(data) {
            UIControlService.loadLoading("Loading Update Komoditi");
            PurchReqService.updateMaterial(data, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Update Material");
                    vm.jLoad(vm.currentPage);
                }
                else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.MSG_ERR_SAVE");
                UIControlService.unloadLoadingModal();
            });
        }
        */

        vm.updateCommodity = updateCommodity;
        function updateCommodity(id, commodity) {
            UIControlService.loadLoading("Loading Update Komoditi");
            PurchReqService.updateCommodity([{
                ID: id,
                Commodity: commodity
            }], function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil megubah Komoditas");
                    vm.jLoad(vm.currentPage);
                }
                else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.MSG_ERR_SAVE");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.back = back;
        function back() {
            //console.info("back");
            $state.transitionTo('purchase-requisition');
        }

    }
})();