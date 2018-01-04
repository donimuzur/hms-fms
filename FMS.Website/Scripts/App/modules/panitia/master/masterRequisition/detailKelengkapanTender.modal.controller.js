(function () {
    'use strict';

    angular.module("app")
    .controller("detailKelengkapanTenderController", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, UIControlService, GlobalConstantService) {

        var vm = this;
        var stepDoc = item.RFQStepDocuments != null ? item.RFQStepDocuments : [];
        var loadmsg = "MESSAGE.LOADING";

        vm.tenderDocTypes = item.tenderDocTypes;
        vm.editable = item.editable;

        vm.init = init;
        function init() {
            vm.tenderDocTypes.forEach(function (doc) {
                doc.checked = false;
                for(var i = 0; i < stepDoc.length; i++){
                    if (doc.ID === stepDoc[i].TenderDocTypeID) {
                        doc.Information = stepDoc[i].Information;
                        doc.checked = true;
                        break;
                    }
                }
            });
        };

        vm.loadOpenAll = loadOpenAll;
        function loadOpenAll() {
            console.info("openall?" + JSON.stringify(vm.IsOpenAll));
            if (vm.IsOpenAll === true) {
                console.info("masuk");
                for (var i = 0; i < vm.tenderDocTypes.length; i++) {
                    vm.tenderDocTypes[i].checked = true;
                }
                    
            }
            else if (vm.IsOpenAll === false) {
                console.info("masuk");
                for (var i = 0; i < vm.tenderDocTypes.length; i++) {
                    vm.tenderDocTypes[i].checked = false;
                }
            }
        }

        vm.save = save;
        function save() {
            var result = [];
            vm.tenderDocTypes.forEach(function (doc) {
                if (doc.checked === true) {
                    result.push({
                        TenderDocTypeID: doc.ID,
                        Information: doc.Information
                    });
                }
            });
            $uibModalInstance.close(result);
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();