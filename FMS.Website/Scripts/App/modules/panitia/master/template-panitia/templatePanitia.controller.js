(function () {
    'use strict';

    angular.module("app").controller("templatePanitiaCtrl", ctrl);

    ctrl.$inject = ['$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', '$filter', 'CommitteeTemplateService', 'UIControlService'];
    function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, $filter, CommitteeTemplateService, UIControlService) {

        var vm = this;
        vm.totalItems = 0;
        vm.templates = [];
        vm.textcari = "";
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.maxSize = 10;
        
        vm.init = init();
        function init() {
            $translatePartialLoader.addPart('master-template-panitia');
            loadData(1);
        };
        
        vm.caritemplate = caritemplate;
        function caritemplate(textcari) {
            vm.textcari = textcari;
            loadData(1);
        };

        vm.loadData = loadData;
        function loadData(page) {
            vm.currentPage = page;
            UIControlService.loadLoading("LOADING");
            CommitteeTemplateService.Select({
                Keyword: vm.textcari,
                column: 1,
                Offset: (vm.currentPage - 1) * vm.pageSize,
                Limit: vm.pageSize
            }, function (reply) {
                vm.templates = reply.data.List;
                vm.totalItems = reply.data.Count;
                UIControlService.unloadLoading();
            }, function (err) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
                UIControlService.unloadLoading();
            });
        };
        
        vm.tambahdata = tambahdata;
        function tambahdata(id) {
            ubahdata(0);
        };

        vm.ubahdata = ubahdata;
        function ubahdata(id) {
            var itemdata = {
                ID: id
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/template-panitia/formTemplatePanitia.html',
                controller: 'formTemplatePanitiaCtrl',
                controllerAs: "frmTempPanitiaCtrl",
                resolve: {
                    item: function() {
                        return itemdata;
                    }
                }
            });
            modalInstance.result.then(function() {
                loadData(1);
            });
        };

        vm.hapus = hapus;
        function hapus(id) {
            bootbox.confirm($filter('translate')('MESSAGE.CONFIRM_DEL'), function (yes) {
                if (yes) {
                    UIControlService.loadLoading("LOADING");
                    CommitteeTemplateService.Delete({
                        ID: id,
                    }, function (reply) {
                        loadData(vm.currentPage);
                        UIControlService.msg_growl("notice", 'MESSAGE.SUCC_DEL');
                        UIControlService.unloadLoading();
                    }, function (err) {
                        UIControlService.msg_growl("error", 'MESSAGE.ERR_DEL');
                        UIControlService.unloadLoading();
                    });
                }
            });           
        };
    }
})();
