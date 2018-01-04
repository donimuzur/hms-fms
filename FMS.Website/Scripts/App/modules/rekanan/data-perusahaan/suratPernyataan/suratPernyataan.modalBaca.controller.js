(function () {
    'use strict';

    angular.module("app").controller("SuratPernyataanModalBacaCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$stateParams', '$location', 'SocketService', 'VerifiedSendService', 'SrtPernyataanService',
        '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item', 'UploadFileConfigService', 'UploaderService'];
    function ctrl($http, $translate, $translatePartialLoader, $stateParams, $location, SocketService, VerifiedSendService, SrtPernyataanService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item, UploadFileConfigService, UploaderService) {
        var vm = this;

        vm.VendorID;
        //vm.Nama = [];
        //vm.Document = [];
        vm.NamaDir;
        vm.NamaNotaris;
        vm.DocNo;
        vm.DocDate;
        vm.DocDateSub;
        vm.ContactID;
        vm.AddressID;
        vm.positionRef;
        vm.vendorName;
        vm.StateID;
        vm.CountryID;


        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('surat-pernyataan');
            loadVerifiedVendor();
        };

        //ambil VendorID
        vm.loadVerifiedVendor = loadVerifiedVendor;
        function loadVerifiedVendor() {
            VerifiedSendService.selectVerifikasi(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.verified = reply.data;
                    console.info("datamodal" + JSON.stringify(vm.verified));
                    vm.cekTemporary = vm.verified.IsTemporary;
                    vm.VendorID = vm.verified.VendorID;
                    vm.vendorName = vm.verified.VendorName;
                    loadCompanyPerson(1);
                    loadLegalDoc(1);
                    loadVendorContact(1);
                    //console.info(JSON.stringify(vm.verified.VendorID));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Perusahaan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadCompanyPerson = loadCompanyPerson;
        function loadCompanyPerson(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectNamaDir({
                VendorID: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    //vm.Nama = data;
                    vm.NamaDir = generateFilterStrings(reply.data);
                    vm.positionRef = data[0].PositionRef;
                    //console.info(vm.NamaDir);

                } else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadLegalDoc = loadLegalDoc;
        function loadLegalDoc(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectLegalDoc({
                VendorID: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.NamaNotaris = data[0].NotaryName;
                    vm.DocNo = data[0].DocumentNo;
                    vm.DocDate = data[0].DocumentDate;
                    vm.DocDateSub = vm.DocDate.substring(0,4);
                    //console.info(vm.DocDateSub);

                } else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


        vm.loadVendorContact = loadVendorContact;
        function loadVendorContact(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectContactID({
                VendorID: vm.VendorID
            }, function (reply) {
                console.info("kontak:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.ContactID = data[0].ContactID;
                    vm.businessName = data[0].Vendor.businessName;
                    vm.countrycode = data[0].Contact.Address.State.Country.Code;
                    console.info(vm.countrycode);
                    getAddressID(1);
                    //console.info(vm.ContactID);
                
                } else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.getAddressID = getAddressID;
        function getAddressID(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectAddressID({
                ContactID: vm.ContactID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.AddressID = data[0].AddressID;
                    loadAddress(1);
                    //console.info(vm.AddressID);

                } else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


        vm.loadAddress = loadAddress;
        function loadAddress(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectAddress({
                AddressID: vm.AddressID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.Address = data[0].AddressDetail;
                    vm.StateID = data[0].StateID;
                    loadCountryID(1);
                    //console.info(vm.Address);

                } else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadCountryID = loadCountryID;
        function loadCountryID(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectCountryID({
                StateID: vm.StateID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.CountryID = data[0].CountryID;
                    loadCountry(1);
                    //console.info(vm.CountryID);

                } else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadCountry = loadCountry;
        function loadCountry(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectCountry({
                CountryID: vm.CountryID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    //vm.Address = data[0].;
                    //console.info(reply.data);

                } else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


        function generateFilterStrings(nama) {
            var namaDir = "";
            for (var i = 0; i < nama.length; i++) {
                namaDir += nama[i].PersonName + ",";
            }
            return namaDir.substring(0, namaDir.length - 1);
        }




        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };


    }
})();