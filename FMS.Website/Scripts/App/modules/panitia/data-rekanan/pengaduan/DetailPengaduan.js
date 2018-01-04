(function () {
    'use strict';

    angular.module("app").controller("DetailPengaduanCtrl", ctrl);

    ctrl.$inject = ['$http', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ComplaintService', 'RoleService', 'UIControlService', '$uibModal', '$uibModalInstance'];
    function ctrl($http,item, $translate, $translatePartialLoader, $location, SocketService, ComplaintService,
        RoleService, UIControlService, $uibModal, $uibModalInstance) {
        var vm = this;
        vm.search = "";
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;

        function init() {
            $translatePartialLoader.addPart('master-complaint');
            vm.name = item.item.ReporterName;
            vm.departement = item.item.department.DepartmentName;
            vm.vendor = item.item.vendor.VendorName;
            vm.date = item.item.Date;
            vm.description = item.item.Description;
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

    }
})();