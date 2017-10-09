(function () {
    'use strict';

    angular.module("app").controller("ubahAktaSahamCtrl", ctrl);

    ctrl.$inject = ['$http', '$uibModalInstance', 'item', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'AktaPendirianService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($http, $uibModalInstance, item, $filter, $translate, $translatePartialLoader, $location, SocketService, AktaPendirianService, UIControlService, GlobalConstantService) {

        var loadmsg = 'MESSAGE.LOADING';
        var vm = this;

        vm.allStocks = item.allStocks;
        var stocks = item.stocks;
        var legalDocId = item.legalDocId;
        vm.documentNo = item.documentNo;

        vm.init = init;
        function init(){
            for (var i = 0; i < vm.allStocks.length; i++) {
                vm.allStocks[i].checked = false;
                for (var j = 0; j < stocks.length; j++) {
                    if (vm.allStocks[i].StockID == stocks[j].StockID) {
                        vm.allStocks[i].checked = true;
                        break;
                    }
                }
            }
        }

        vm.save = save;
        function save() {

            var VendorStocks = [];
            vm.allStocks.forEach(function (s) {
                if (s.checked) {
                    VendorStocks.push(s);
                }
            });

            UIControlService.loadLoadingModal(loadmsg);
            AktaPendirianService.EditLegalStock({
                ID: legalDocId,
                VendorStocks: VendorStocks,
                VendorID: 0, //bypass validasi model
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", "MESSAGE.SUCC_SAVE_VSTOCK");
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE_VSTOCK");
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE_VSTOCK");
            });
        }

        vm.close = close;
        function close() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();