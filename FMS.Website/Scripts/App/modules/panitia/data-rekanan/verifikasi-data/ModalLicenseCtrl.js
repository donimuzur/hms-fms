(function () {
    'use strict';

    angular.module("app").controller("ModalLicenseCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'RoleService', 'UIControlService', 'item', '$uibModalInstance', '$state', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, 
        RoleService, UIControlService, item, $uibModalInstance, $state, GlobalConstantService) {
        var vm = this;
        var page_id = 141;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.data = item.item;
        vm.isAdd = item.flag;
        vm.countryID = 0;
        vm.lokasidetail = '';
        
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("data-izinusaha");
            if (vm.isAdd === 4) {
                vm.countryID = vm.data.StateLocationRef.CountryID;
                console.info(JSON.stringify(vm.countryID));
                if (vm.countryID === 360) {
                    vm.lokasidetail = vm.data.CityLocation.Name + "," + vm.data.StateLocationRef.Name + "," + vm.data.StateLocationRef.Country.Name + "," + vm.data.StateLocationRef.Country.Continent.Name;
                } else {
                    vm.lokasidetail = vm.data.StateLocationRef.Name + "," + vm.data.StateLocationRef.Country.Name + "," + vm.data.StateLocationRef.Country.Continent.Name;

                    console.info(JSON.stringify(vm.lokasidetail));
                }
            }

        }
        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

    }
})();