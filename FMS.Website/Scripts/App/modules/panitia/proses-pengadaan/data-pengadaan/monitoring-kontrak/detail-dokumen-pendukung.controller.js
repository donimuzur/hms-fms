(function () {
    'use strict';

    angular.module("app").controller("DetDokCtrl", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$uibModal', '$http', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataKontrakService', 'UIControlService', 'GlobalConstantService', 'UploaderService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $uibModal,  $http, $filter, $translate, $translatePartialLoader, $location, SocketService, DataKontrakService, UIControlService, GlobalConstantService, UploaderService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.currentPage = 1;
        vm.DocName = "";
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.list = [];
        vm.init = init;
        vm.contractSignOff;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        var id = Number($stateParams.id);
        vm.id = Number($stateParams.id);
        function init() {
            $translatePartialLoader.addPart('detail-dokumen-pendukung');
            loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataKontrakService.DetailDok({
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Status: id,
                Column: vm.column
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
        vm.uploadDok = uploadDok;
        function uploadDok() {
            var lempar = {
                datalempar: {
                    ContractSignOffId: id,
                    id: id,
                    DocName: "",
                    DocUrl: "",
                    Size: 0,
                }
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/proses-pengadaan/data-pengadaan/monitoring-kontrak/upload-doc-pendukung.html',
                controller: 'uploadDokCtrl',
                controllerAs: 'uploadDokCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadPaket();
            });
           // $state.transitionTo('upload-dokumen-pendukung', { id: id });
        };

        vm.cariPaket = cariPaket;
        function cariPaket(keyword) {
            vm.keyword = keyword;
            vm.currentPage = 1;
            loadPaket();
        };

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
