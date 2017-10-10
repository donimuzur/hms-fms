(function () {
    'use strict';

    angular.module("app").controller("DetailWarningLetterCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'WarningLetterService', 'RoleService', 'UIControlService', 'item', '$uibModal', '$uibModalInstance'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, WarningLetterService,
        RoleService, UIControlService, item, $uibModal, $uibModalInstance) {
        var vm = this;
        var page_id = 141;

        vm.isAdd = item.act;
        vm.Area;
        vm.LetterID;
        vm.Description="";
        vm.action = "";
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.kata = new Kata("");
        vm.init = init;
        vm.warningLetter = {};
        vm.isCalendarOpened = [false, false, false, false];

        function init() {
                vm.CreatedDate =  item.item.CreatedDate;
                vm.VendorName = item.item.Vendor.VendorName;
                vm.WarningType = item.item.SysReference.Name;
                vm.StartDate = item.item.StartDate;
                vm.EndDate = item.item.EndDate;
                vm.Area = item.item.Area;
                vm.Description = item.item.Description;
                vm.Reporter = item.item.department.department.DepartmentName;
                vm.Status = item.item.IsActive;

        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

        vm.tambah = tambah;
        function tambah(warningtype) {
            var data = {
                act: warningtype,
                data1: item.item,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/UbahTemplate.html',
                controller: 'UbahWarningLetterCtrl',
                controllerAs: 'UbahwarningLetterCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
            $uibModalInstance.close();
        }



    }
})();