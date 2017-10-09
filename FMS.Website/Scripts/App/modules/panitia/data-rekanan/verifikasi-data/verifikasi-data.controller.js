(function () {
    'use strict';

    angular.module("app").controller("VerifikasiDataCtrl", ctrl);
    ctrl.$inject = ['$http',  '$translate', '$translatePartialLoader', '$location', 'SocketService', 
        'VerifikasiDataService','UIControlService', '$uibModal', '$state'];
    /* @ngInject */
    function ctrl($http,  $translate, $translatePartialLoader, $location, SocketService,
        VerifikasiDataService, UIControlService, $uibModal, $state) {
        var vm = this;
        vm.datarekanan = [];
        vm.currentPage = 1;
        vm.fullSize = 10;
        vm.offset = (vm.currentPage * 10) - 10;
        vm.totalRecords = 0;
        vm.nCompany = '';
        vm.activator;
        vm.verificator;
        vm.menuhome = 0;
        vm.cmbStatus = 0;
        vm.rekanan_id = '';
        vm.flag = 0;
        
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
        vm.openCalendar = openCalendar;
        vm.show = show;
        vm.add = add;
        vm.addVerifikasi = addVerifikasi;
        vm.listExpired = listExpired;
        
        function init() {
            vm.listDropdown =
            [
                { Value: 0, Name: "SELECT.ALL" },
                { Value: 1, Name: "SELECT.NOT_ACTIVED" },
                { Value: 2, Name: "SELECT.ACTIVED" },
                { Value: 6, Name: "SELECT.END_ACTIVED" },
                { Value: 3, Name: "SELECT.NOT_VERIFIED" },
                { Value: 4, Name: "SELECT.VERIFIED" },
                { Value: 8, Name: "SELECT.END_VERIFIED" },
                { Value: 10, Name: "SELECT.NOT_ACTIVE" },
                
            ]
            $translatePartialLoader.addPart('verifikasi-data');
            jLoad(1);
            convertToDate();
            getUserLogin();
        };

        vm.getUserLogin = getUserLogin;
        function getUserLogin() {
            VerifikasiDataService.getUserLogin(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.typelogin = reply.data;
                    //console.info("tipe login" + JSON.stringify(vm.typelogin));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data " });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.verifyEndDate = verifyEndDate;
        function verifyEndDate(selectedEndDate,selectedStartDate) {
            var convertedEndDate = UIControlService.getStrDate(selectedEndDate);
            var convertedStartDate = UIControlService.getStrDate(selectedStartDate);
            //console.info("selected end date" + JSON.stringify(convertedEndDate));
            //console.info("selected start date" + JSON.stringify(convertedStartDate));
            if (convertedEndDate < convertedStartDate) {
                UIControlService.msg_growl("warning", "Tanggal batas akhir tidak boleh sebelum batas awal");
                vm.verifikasi.EndDate = " ";
            }
            //else {
              //  console.info("masak");
            //}
        }

        function show() {
            console.info(vm.Status);
            if (vm.verifikasi.StartDate === undefined && vm.verifikasi.EndDate !== undefined) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_DATE1");
                return;
            }
            if (vm.verifikasi.EndDate === undefined && vm.verifikasi.StartDate !== undefined) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_DATE2");
                return;
            }
            if (vm.Status === undefined) {
                vm.cmbStatus = 0;
            }
            else {
                vm.cmbStatus = vm.Status.Value;
            }
            jLoad(1);
        }

        function jLoad(current) {
            vm.verifikasidata = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            VerifikasiDataService.all({
                Offset: offset,
                Limit: vm.fullSize,
                Keyword: vm.nCompany,
                Status: vm.cmbStatus,
                Date1: UIControlService.getStrDate(vm.verifikasi.StartDate),
                Date2: UIControlService.getStrDate(vm.verifikasi.EndDate)
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.verifikasidata = data.List;
                    vm.totalItems = Number(data.Count);
                    vm.flag = vm.cmbStatus;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString() { // TIMEZONE (-)
            if (vm.verifikasi.StartDate) {
                vm.verifikasi.StartDate = UIControlService.getStrDate(vm.verifikasi.StartDate);
            }
            if (vm.verifikasi.EndDate) {
                vm.verifikasi.EndDate = UIControlService.getStrDate(vm.verifikasi.EndDate);
            }
        };

        function convertToDate() {
            if (vm.verifikasi.StartDate) {
                vm.verifikasi.StartDate = new Date(Date.parse(vm.verifikasi.StartDate));
            }
            if (vm.verifikasi.EndDate) {
                vm.verifikasi.EndDate = new Date(Date.parse(vm.verifikasi.EndDate));
            }
        }
       
        function add(act, data, name) {
            var data = {
                TenderName : name, 
                act: act,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/DetailActified.html',
                controller: 'FormActivedVendorCtrl',
                controllerAs: 'FrmActivedVendorCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(vm.currentPage);
            });
        }

        function addVerifikasi(data) {
            $state.transitionTo('proses-verifikasi', { id: data.VendorID });
        }

        function listExpired() {
            var data = {
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/expired-license.html',
                controller: 'ExpiredLicenseController',
                controllerAs: 'ExpiredLicenseController',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(vm.currentPage);
            });
        }

        vm.editActive = editActive;
        function editActive(active, data) {
            var data = {
                act: active,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/DetailAktivasiVendor.html',
                controller: 'FormActiveVendorCtrl',
                controllerAs: 'FrmActiveVendorCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(vm.currentPage);
            });

        }

        vm.approval = approval;
        function approval() {
            $state.transitionTo('detail-approval-vendor');
        }

        vm.Detailapproval = Detailapproval;
        function Detailapproval(data) {
            var data = {
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/DetailModalApproval.html',
                controller: 'DetailModalApprovalCtrl',
                controllerAs: 'DetailModalApprovalCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(vm.currentPage);
            });

        }

        vm.saveSAP = saveSAP;
        function saveSAP() {
            console.info("sss");
            VerifikasiDataService.saveSAPCode(vm.verifikasidata, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    init();
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }
    }
})();
