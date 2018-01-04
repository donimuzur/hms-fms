(function () {
    'use strict';

    angular.module("app")
            .controller("ListDOACtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'ListDOAService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, ListDOAService, UploadFileConfigService,
        UIControlService, UploaderService, GlobalConstantService) {

        var vm = this;
        vm.listData = [];
        vm.idUploadConfigs = [];
        vm.selectUpload = selectUpload;
        vm.textSearch = "";
        vm.maxSize = 10;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('master-DOA');
            vm.jLoad(0);
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * vm.maxSize) - vm.maxSize;
            ListDOAService.select({
                Offset: offset,
                Limit: vm.maxSize,
                Keyword: vm.textSearch
            }, function (reply) {
                //console.info("doa:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listData = data.List;
                    vm.totalItems = Number(data.Count);
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOADDATA");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoading();
            });
        }

        function selectUpload() {

        }


    }
})();