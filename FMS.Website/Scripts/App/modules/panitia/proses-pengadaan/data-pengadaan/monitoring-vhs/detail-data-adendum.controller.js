(function () {
    'use strict';

    angular.module("app").controller("DetVHSCtrl", ctrl);

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
        vm.maxSize = 10;
        vm.VHSdata;
        vm.listVHS = [];
        vm.init = init;
        var id = Number($stateParams.id);
        vm.id = Number($stateParams.id);
        function init() {
            $translatePartialLoader.addPart('detail-data-adendum');
            loadPaket();
        };
        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataVHSService.Addendum({
                Offset: vm.pageSize * (vm.currentPage - 1),
                Status: vm.id,
                Limit: vm.pageSize,
                Column: vm.column
            },
            function (reply) {
                console.info("data:" + JSON.stringify(reply.data));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    console.info("LIST:" + JSON.stringify(data));
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

        vm.addAdendum = addAdendum;
        function addAdendum(id) {
            $state.transitionTo('add-adendum', {id : id});
        };

        vm.editAddendum = editAddendum;
        function editAddendum(id) {
            $state.transitionTo('edit-adendum', { id: id });
        };
        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
