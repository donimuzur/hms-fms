(function () {
    'use strict';

    angular.module("app").controller("DetailContactCtrl", ctrl);
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'VerifikasiDataService', 'UIControlService', 'item', '$uibModalInstance', '$uibModal', 'GlobalConstantService', 'VendorRegistrationService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        VerifikasiDataService, UIControlService, item, $uibModalInstance, $uibModal, GlobalConstantService, VendorRegistrationService) {
        var vm = this;
        //console.info("act:" + JSON.stringify(item));
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.isAdd = item.item;
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
        vm.savedata = savedata;
        vm.detail = [];

        function init() {
            vm.Name = vm.isAdd.Contact.Name;
            vm.Email = vm.isAdd.Contact.Email;
            vm.Telp = vm.isAdd.Contact.Phone;
            

        };
       
        vm.loadPhoneCodes = loadPhoneCodes;
        function loadPhoneCodes(data) {
            UIControlService.loadLoading("Loading");
            VendorRegistrationService.getCountries(
              function (response) {
                  vm.phoneCodeList = response.data;
                  for (var i = 0; i < vm.phoneCodeList.length; i++) {
                      if (vm.phoneCodeList[i].PhonePrefix === data) {
                          vm.phoneCode = vm.phoneCodeList[i];
                      }
                  }
                  UIControlService.unloadLoading();
              }, function (err) {
                  $.growl.error({ message: "Gagal Akses API >" + err });
                  UIControlService.unloadLoading();
              });
        }

        function savedata() {
            var data = {
                ContactID: vm.isAdd.ContactID,
                Name: vm.Name,
                Email: vm.Email,
                Phone: vm.Telp
            }
            $uibModalInstance.close(data);
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

       
    }
})();
