(function () {
    'use strict';

    angular.module("app")
            .controller("DetailPenawaranCtrl", ctrl);

    ctrl.$inject = ['$timeout','Excel','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PPVHSService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($timeout,Excel, $http, $translate, $translatePartialLoader, $location, SocketService, PPVHSService, UploadFileConfigService,
        UIControlService, UploaderService, item, $uibModalInstance, GlobalConstantService) {

        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.DocTypeID = item.DocTypeID ;
        vm.StepID = item.StepID;
        vm.init = init;
        function init() {
            loadAwal();
            
        }

        vm.loadAwal = loadAwal;
        function loadAwal() {
            PPVHSService.selectDetail({
                TenderDocTypeID: vm.DocTypeID,
                vhs:
                {
                    TenderStepID: vm.StepID
                }
            }, function (reply) {
                UIControlService.unloadLoading();
                vm.newExcel = [];
                if (reply.status === 200) {
                    vm.det = reply.data;
                    console.info(reply.data);
                    for (var i = 0; i < vm.det.length; i++) {

                        var objExcel = {
                            MaterialCode: vm.det[i].MaterialCode,
                            Estimate: vm.det[i].Estimate,
                            Unit: vm.det[i].Unit,
                            ItemDescrip: vm.det[i].ItemDescrip,
                            Manufacture: vm.det[i].Manufacture,
                            PartNo: vm.det[i].PartNo,
                            Currency: vm.det[i].Currency,
                            PriceIDR: vm.det[i].PriceIDR,
                            LeadTime: vm.det[i].LeadTime,
                            CountryOfOrigin: vm.det[i].CountryOfOrigin,
                            Remark: vm.det[i].Remark
                        }

                        vm.newExcel.push(objExcel);
                        if (i == vm.det.length - 1) {
                            console.info(vm.newExcel);
                        }

                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evaluasi Teknis" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

       
    }
})();