(function () {
    'use strict';

    angular.module("app")
    .controller("detailPPVendorController", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PengumumanPengadaanService', '$state', 'UIControlService', 'item', '$uibModalInstance',
         'UploaderService', '$uibModal', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PengumumanPengadaanService,
        $state, UIControlService, item, $uibModalInstance, UploaderService, $uibModal, GlobalConstantService) {
        console.info(">>" + JSON.stringify(item));
        var vm = this;
        var item = item;
        
        vm.init = init;
        function init(){
            vm.TenderCode = item.TenderStepData.tender.TenderCode;
            vm.TenderName = item.TenderStepData.tender.TenderName;
            vm.IsLocal = item.IsLokal;
            vm.IsNational = item.IsNational;
            vm.IsInternational = item.IsInternational;
            vm.IsVendorEmails = item.IsVendorEmails;
            vm.IsOpen = item.IsOpen;
            vm.Emails = item.Emails;
            vm.Description = item.Description;
            if (!(item.CommodityID === null)) {
                vm.selectedComodity = item.MstCommodity;
            }
            if (!(item.CompanyScaleID === null)) {
                vm.selectedClasification = item.SysReference;
            }
            if (!(item.TechnicalID === null)) {
                vm.selectedTechnical = item.Technical;
            }
            
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

    }
})();