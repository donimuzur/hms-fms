(function () {
    'use strict';

    angular.module("app").controller("SuratPernyataanModalCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$stateParams', '$location', 'SocketService', 'VerifiedSendService', 'SrtPernyataanService',
        '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item', 'UploadFileConfigService', 'UploaderService'];
    function ctrl($http, $translate, $translatePartialLoader, $stateParams, $location, SocketService, VerifiedSendService, SrtPernyataanService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item, UploadFileConfigService, UploaderService) {
        var vm = this;

        vm.true = 1;
        vm.agree;
        vm.vendorName;
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
        vm.Country;
        vm.StateName;
        vm.Position;
        vm.noID;
        vm.PersonAddress;
        vm.tglSekarang = UIControlService.getDateNow2("-");
        vm.ID;
        vm.DocType1 = item.DocType;
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('surat-pernyataan');
            loadVerifiedVendor(1);
            CekVendor();
            var dateNow = new Date();
            vm.convertedDateNow = UIControlService.getStrDate(dateNow);
            console.info("convertedDateNow:"+JSON.stringify(vm.convertedDateNow));
            //console.info(vm.tglSekarang);
            //convertToDate();
        }
        /*function convertToDate() {
            vm.tglSekarang.StartDate = UIControlService.getStrDate(vm.tglSekarang.StartDate);
        }*/

        vm.CekVendor = CekVendor;
        function CekVendor() {
            SrtPernyataanService.CekAgree({DocType: vm.DocType1}, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.flag = reply.data;
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

        vm.loadVerifiedVendor = loadVerifiedVendor;
        function loadVerifiedVendor() {
            VerifiedSendService.selectVerifikasi(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.verified = reply.data;
                    vm.cekTemporary = vm.verified.IsTemporary;
                    vm.VendorID = vm.verified.VendorID;
                    vm.vendorName = vm.verified.VendorName;
                    //loadCompanyPerson(1);
                    //loadLegalDoc(1);
                    loadVendorContact(1);
                    loadCompanyPerson(1);
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

        vm.setuju = setuju;
        function setuju() {
            cek();
        }

        vm.cek = cek;
        function cek() {
            VerifiedSendService.selectVerifikasi(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.verified = reply.data;
                    vm.cekTemporary = vm.verified.IsTemporary;
                    vm.VendorID = vm.verified.VendorID;
                    vm.vendorName = vm.verified.VendorName;
                    //loadCompanyPerson(1);
                    //loadLegalDoc(1);
                    Cekcek(1);
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
        vm.Cekcek = Cekcek;
        function Cekcek(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectCek({
                VendorId: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200 && reply.data.length > 0) {
                    var data = reply.data;
                    //vm.Nama = data;
                    vm.ID = data[0].ID;
                    vm.DocType = data[0].DocType;
                    vm.UploadDate = data[0].UploadDate;
                    vm.DocUrl = data[0].DocumentUrl;
                    //console.info(vm.ID);
                    SrtPernyataanService.updateAgree({
                        ID: vm.ID,
                        VendorId: vm.VendorID,
                        DocType: vm.DocType,
                        UploadDate: vm.UploadDate,
                        DocumentUrl: vm.DocUrl,
                        IsAgree: 1,
                        AgreementDate: vm.tglSekarang,
                        IsActive: 1
                    },
                                    function (reply) {
                                        //console.info("reply" + JSON.stringify(reply))
                                        UIControlService.unloadLoadingModal();
                                        if (reply.status === 200) {
                                            UIControlService.msg_growl("success", "Berhasil Setuju!");
                                            $uibModalInstance.close();

                                        }
                                        else {
                                            UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                            return;
                                        }
                                    },
                                    function (err) {
                                        console.info(err);
                                        UIControlService.msg_growl("error", "Gagal Akses Api!!");
                                        UIControlService.unloadLoadingModal();
                                    }
                                );
                 
                }
                else if (reply.status === 200 && reply.data.length <= 0) {
                    agree();
                }
                else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
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
                    vm.noID = generateFilterStrings2(reply.data);
                    vm.PersonAddress = data[0].PersonAddress;
                    //console.info(vm.NamaDir);
                    //console.info(vm.noID);
                    //console.info(vm.PersonAddress);
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
                    //console.info(vm.Country);

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

        function generateFilterStrings2(nama) {
            var namaDir = "";
            for (var i = 0; i < nama.length; i++) {
                namaDir += nama[i].NoID + ",";
            }
            return namaDir.substring(0, namaDir.length - 1);
        }

        //console.info(vm.tglSekarang);

        vm.agree = agree;
        function agree() {
            SrtPernyataanService.insertAgree({
                VendorId: vm.VendorID,
                IsAgree: 1,
                AgreementDate: vm.tglSekarang,
                DocType: vm.DocType1,
                IsActive: 1
            },
                                    function (reply) {
                                        //console.info("reply" + JSON.stringify(reply))
                                        UIControlService.unloadLoadingModal();
                                        if (reply.status === 200) {
                                            UIControlService.msg_growl("success", "Berhasil Setuju!");
                                            $uibModalInstance.close();

                                        }
                                        else {
                                            UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                            return;
                                        }
                                    },
                                    function (err) {
                                        console.info(err);
                                        UIControlService.msg_growl("error", "Gagal Akses Api!!");
                                        UIControlService.unloadLoadingModal();
                                    }
                                );

        }

        vm.update = update;
        function update() {
            SrtPernyataanService.updateAgree({
                ID : vm.ID,
                VendorId: vm.VendorID,
                IsAgree: 1,
                AgreementDate: vm.tglSekarang,
                IsActive: 1,
                DocType: vm.DocType1
            },
                                    function (reply) {
                                        UIControlService.unloadLoadingModal();
                                        if (reply.status === 200) {
                                            UIControlService.msg_growl("success", "Berhasil Setuju!");
                                            $uibModalInstance.close();

                                        }
                                        else {
                                            UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                            return;
                                        }
                                    },
                                    function (err) {
                                        console.info(err);
                                        UIControlService.msg_growl("error", "Gagal Akses Api!!");
                                        UIControlService.unloadLoadingModal();
                                    }
                                );

        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };


    }

    
})();