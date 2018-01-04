(function () {
    'use strict';

    angular.module("app").controller("FormActivedVendorCtrl", ctrl);
    ctrl.$inject = ['$http',  '$translate', '$translatePartialLoader', '$location', 'SocketService', 
        'VerifikasiDataService', 'UIControlService', 'item', '$uibModalInstance', '$uibModal', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($http,  $translate, $translatePartialLoader, $location, SocketService,
        VerifikasiDataService, UIControlService, item, $uibModalInstance, $uibModal, GlobalConstantService) {
        var vm = this;
        //console.info("act:" + JSON.stringify(item));
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.isAdd = item.item;
        vm.act = item.act;
        vm.TenderName = item.TenderName;
        vm.datarekanan = [];
        vm.addressBranch = '';
        vm.addressMain = '';

        vm.currentPage = 1;
        vm.fullSize = 10;
        vm.offset = (vm.currentPage * 10) - 10;
        vm.totalRecords = 0;
        vm.user = '';
        vm.activator;
        vm.verificator;
        vm.menuhome = 0;
        vm.cmbStatus = 0;
        vm.rekanan_id = '';
        vm.flag = false;
        
        vm.date = "";
        vm.year = "";
        vm.datemonth = "";

        vm.waktuMulai1 = (vm.year - 1) + '-' + vm.datemonth;
        vm.waktuMulai2 = vm.date;

        vm.sStatus = -1;
        vm.thisPage = 12;
        vm.verificationPage = 130;
        vm.verifikasi = {};
        vm.isCalendarOpened = [false, false, false, false];
        //functions
        vm.init = init;
        vm.jLoad = jLoad;
        vm.detail = [];
        
        function init() {
            $translatePartialLoader.addPart('verifikasi-data');
            jLoad();
           
            
        };

        function jLoad() {
            vm.addressBranch = '';
            vm.addressMain = '';
            VerifikasiDataService.allcontact({
                VendorID: item.item.VendorID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detail = reply.data;
                    for (var i = 0; i < vm.detail.length ; i++) {
                        if (vm.detail[i].VendorContactType.Value === "VENDOR_CONTACT_TYPE_COMPANY") {
                            vm.State = vm.detail[i].Contact.Address.State.Country.Code;
                            if (vm.detail[i].Contact.Fax !== null) {
                                vm.ld = vm.detail[i].Contact.Fax;
                            }
                        }
                        if (vm.detail[i].VendorContactType.Value === "VENDOR_OFFICE_TYPE_BRANCH") {
                            vm.addressBranch = vm.detail[i].Contact.Address.AddressInfo + vm.detail[i].Contact.Address.AddressDetail;
                        }
                        if (vm.detail[i].VendorContactType.Value === "VENDOR_OFFICE_TYPE_MAIN") {
                            vm.addressMain = vm.detail[i].Contact.Address.AddressInfo + vm.detail[i].Contact.Address.AddressDetail;
                        }
                    }
                    console.info(JSON.stringify(vm.detail));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        vm.tolakactived = tolakactived;
        function tolakactived() {
            console.info("masuk form add/edit");
            var data = {
                act: false,
                item: vm.isAdd,
                emailAddress: vm.detail[0].Contact.Email
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/ActivedRejected.html',
                controller: 'FormActivedRejectedCtrl',
                controllerAs: 'FrmActivedRejectedCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
           });
            modalInstance.result.then(function () {
                $uibModalInstance.close();
            });
        }

        vm.acceptactived = acceptactived;
        function acceptactived(){
            console.info("masuk form add/edit");
            var data = {
                act: true,
                item: vm.isAdd,
                emailAddress: vm.detail[0].Contact.Email
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/ActivedRejected.html',
                controller: 'FormActivedRejectedCtrl',
                controllerAs: 'FrmActivedRejectedCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                $uibModalInstance.close();
            });
        }
    }
})();
