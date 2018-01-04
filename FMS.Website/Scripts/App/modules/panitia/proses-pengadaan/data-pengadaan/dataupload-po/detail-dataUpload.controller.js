(function () {
    'use strict';

    angular.module("app").controller("detaildataUpload", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'dataUploadService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, dataUploadService, UIControlService) {


        var vm = this;

        vm.currentPage = 1;
        vm.fileUpload;
        vm.keyword = "";
        vm.maxSize = 10;
        vm.currentPage = 1;
        vm.listItemPO = [];
        vm.totalItems = 0;
        vm.ID = Number($stateParams.id);
        vm.column = 1;
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('detail-dataupload-po');
            loadData(1);
           
        }
        var loadmsg = "MESSAGE.LOADING";
        function loadData() {
            //console.info("curr "+current)
            UIControlService.loadLoading(loadmsg);
            dataUploadService.Detail({
                Offset: vm.maxSize * (vm.currentPage - 1),
                Limit: vm.maxSize,
                Parameter: vm.ID,
                Column: vm.column
            }, function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listItemPO = data.List;
                    for (var i = 0; i < vm.listItemPO.length; i++) {
                        if (vm.listItemPO[i].PODate === null) { vm.listItemPO[i].PODate = "-"; }
                        else { vm.listItemPO[i].PODate = UIControlService.getStrDate(vm.listItemPO[i].PODate); }

                        if (vm.listItemPO[i].PODueDate === null) { vm.listItemPO[i].PODueDate = "-"; }
                        else { vm.listItemPO[i].PODueDate = UIControlService.getStrDate(vm.listItemPO[i].PODueDate); }
                    }
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "MESSAGE.ERR_LOAD" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


    }
})();
