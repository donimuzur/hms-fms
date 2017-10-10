(function () {
    'use strict';

    angular.module("app").controller("MonVHSCtrl", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataVHSService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataVHSService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.id = 0;
        vm.maxSize = 10;
        vm.listVHS = [];
        vm.searchBy = 0;
        vm.init = init;

        function init() {
            $translatePartialLoader.addPart('monitoring-vhs');
            loadPaket(1);
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataVHSService.Select({
                Keyword: vm.keyword,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.searchBy
            },
            function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listVHS = reply.data.List;
                    vm.VHSdata = data;
                    vm.totalItems = Number(data.Count);

                }
                else {
                    $.growl.error({ message: "MESSAGE.ERR_LOAD" });
                    UIControlService.unloadLoading();
                }
            },
                        function (error) {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
                        });
        };

        vm.cariPaket = cariPaket;
        function cariPaket(keyword) {
            vm.keyword = keyword;
            vm.currentPage = 1;
            loadPaket();
        };

        vm.insertBudget = insertBudget;
        function insertBudget(id) {
            UIControlService.loadLoading(loadmsg);
            DataVHSService.UpdateBudget({
                BudgetContract: vm.Budget_Val,
                ID: id,
            },
              function (reply) {
                  if (reply.status === 200) {
                      UIControlService.msg_growl("success", 'Sukses', "MESSAGE.TITLE_SUCCESS");
                      UIControlService.unloadLoading();
                      loadPaket();
                  }
                  else {
                      UIControlService.msg_growl("error", 'Gagal', "MESSAGE.TITLE_FAILED");
                      UIControlService.unloadLoading();
                  }
              },
        function (error) {
            UIControlService.unloadLoading();
            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
        });

        }
        vm.viewAddendum = viewAddendum;
        function viewAddendum(id) {
            $state.transitionTo('detail-data-adendum', { id: id });
        };

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }



    }
})();
