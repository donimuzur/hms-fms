(function () {
    'use strict';

    angular.module("app").controller("SuratPernyataanCtrl", ctrl);

    ctrl.$inject = ['$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', '$filter', 'VerifiedSendService', 'SocketService', 'SrtPernyataanService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, $filter, VerifiedSendService, SocketService, SrtPernyataanService, UIControlService, GlobalConstantService) {
        var vm = this;

        vm.data = [];
        vm.pos;
        vm.cek;
        vm.DocUrlAgreement;
        vm.DocUrlBConduct;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.DocType = null;
        vm.isApprovedCR = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('surat-pernyataan');
            loadVerifiedVendor();
            loadUrlLibrary(1);
            if (localStorage.getItem("currLang") === 'id') {
                vm.DocType = 4225;
            }
            else {
                vm.DocType = 4232;
            }
            loadUrlLibraryAgree(1);
        }

        if (localStorage.getItem("currLang") === 'id') {
            vm.DocType = 4225;
        }
        else {
            vm.DocType = 4232;
        }
       
        //ambil VendorID
        vm.loadVerifiedVendor = loadVerifiedVendor;
        function loadVerifiedVendor() {
            VerifiedSendService.selectVerifikasi(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.verified = reply.data;
                    vm.cekTemporary = vm.verified.IsTemporary;
                    vm.VendorID = vm.verified.VendorID;
                    jLoad(1);
                    loadUrlAgreement(1);
                    loadVendorContact();
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


        vm.loadVendorContact = loadVendorContact;
        function loadVendorContact(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectContactID({
                VendorID: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.ContactID = data[0].ContactID;
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
                    vm.StateName = data[0].Name;
                    loadCountry(1);
                    //console.info(vm.StateName);

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
                    vm.Country = data[0].Name;
                    console.info(vm.Country);
                    if (vm.Country === 'Indonesia') {
                        vm.cek = true;
                    }
                    else {
                        vm.cek = false;
                    }

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





        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.All({
                VendorId: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.data = data;
                    //console.info(vm.data);

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

        vm.loadUrlLibrary = loadUrlLibrary;
        function loadUrlLibrary(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectDocLibrary({
                DocType: vm.DocType
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.DocUrlBConduct = data[0].DocUrl;
                    console.info("bConduct" + vm.DocUrlBConduct);

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

        vm.loadUrlLibraryAgree = loadUrlLibraryAgree;
        function loadUrlLibraryAgree(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectDocLibrary({
                DocType: 4226
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.DocUrlAgreement = data[0].DocUrl;
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

        vm.loadUrlAgreement = loadUrlAgreement;
        function loadUrlAgreement(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectUrlAgree({
                VendorId: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    //getAddressID(1);
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

        vm.loadUrlBConduct = loadUrlBConduct;
        function loadUrlBConduct(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectUrlBConduct({
                VendorId: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.DOcUrlBConduct = data[0].AddressID;
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


        vm.loadUrlBConductEN = loadUrlBConductEN;
        function loadUrlBConductEN(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectBConductEN({
                VendorId: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    //vm.Address = data[0].AddressDetail;
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


        


        







        vm.baca = baca;
        function baca() {
            var data = {
                DocType: vm.DocType
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/data-perusahaan/suratPernyataan/suratPernyataan.modal.html',
                controller: 'SuratPernyataanModalCtrl',
                controllerAs: 'SrtPernyataanModalCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
        };

        vm.baca2 = baca2;
        function baca2() {
            var data = {
                item: data
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/data-perusahaan/suratPernyataan/suratPernyataan.modalBaca.html',
                controller: 'SuratPernyataanModalBacaCtrl',
                controllerAs: 'SrtPernyataanModalBacaCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
        };

        vm.upload = upload;
        function upload(data) {
            var data = {
                DocType: data,
                act : 1
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/data-perusahaan/suratPernyataan/suratPernyataan.modalUpload.html',
                controller: 'SuratPernyataanModalUploadCtrl',
                controllerAs: 'SrtPernyataanModalUpCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                window.location.reload();
            });
        };

        vm.upload2 = upload2;
        function upload2(data) {
            var data = {
                DocType: data,
                act: 0
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/data-perusahaan/suratPernyataan/suratPernyataan.modalUpload.html',
                controller: 'SuratPernyataanModalUploadCtrl',
                controllerAs: 'SrtPernyataanModalUpCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                window.location.reload();
            });
        };








    }
})();