(function () {
    'use strict';

    angular.module("app").controller("DetDistribusiCtrl", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataKontrakService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataKontrakService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.list = [];
        vm.init = init;
        vm.contractSignOff;

        var id = Number($stateParams.id);
        vm.id = Number($stateParams.id);

        vm.vendorid = Number($stateParams.vendorid);
        vm.tenderstepid = Number($stateParams.tenderstepid);

        function init() {
            $translatePartialLoader.addPart('detail-distribusi');
            loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataKontrakService.Distribusi({
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Status: id,
                VendorId: vm.vendorid,
                TenderStepId: vm.tenderstepid
            },
            function (reply) {
                console.info("data:" + JSON.stringify(reply.data));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.contractSignOff = data;
                    vm.list = reply.data.List;
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
        }


        vm.cariPaket = cariPaket;
        function cariPaket(keyword) {
            vm.keyword = keyword;
            vm.currentPage = 1;
            loadPaket();
        };

        vm.editDistribusi = editDistribusi;
        function editDistribusi(id, param) {
            $state.transitionTo('edit-distribusi', { id: id , param: param });
        };

        vm.addDistribusi = addDistribusi;
        function addDistribusi(id) {
            $state.transitionTo('add-distribusi', { id: id});
        };

        vm.getStrDate = getStrDate;
        function getStrDate(date) {
            date = new Date(Date.parse(date));
            return date.getFullYear();
        }

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
