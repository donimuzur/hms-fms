(function () {
    'use strict';

    angular.module("app").controller("BalanceVendorCtrl", ctrl);

    ctrl.$inject = ['AuthService', '$scope', '$state', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'BalanceVendorService', 'RoleService', 'UIControlService', 'GlobalConstantService', '$uibModal'];
    function ctrl( AuthService, $scope, $state, $http, $translate, $translatePartialLoader, $location, SocketService, BalanceVendorService,
        RoleService, UIControlService, GlobalConstantService, $uibModal) {

        var vm = this;
        var page_id = 141;
        vm.departemen = [];
        var asset = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        //vm.isApprovedCR;
        vm.balanceDocUrl = "";

        vm.IsApprovedCR = false;
        vm.initialize = initialize;
        vm.changeDataPermission = false;
        function initialize() {
            $translatePartialLoader.addPart('vendor-balance');
            loadVendor();
            jLoad(1);
            loadCheckCR();
            //loadUnit();
            checkIsVerified();
            loadBalanceDocUrl();
        }

        function loadCheckCR() {
            UIControlService.loadLoading("Silahkan Tunggu");
            BalanceVendorService.getCRbyVendor({ CRName: 'OC_VENDORBALANCE'},function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    console.info("checkCR:" + JSON.stringify(reply));
                    if (reply.data === true) {
                        vm.IsApprovedCR = true;
                    } else {
                    	vm.IsApprovedCR = false;
                    }
                    
                    //console.info("sentcr" + JSON.stringify(vm.isSentCR));
                    //console.info("isApprove?" + JSON.stringify(vm.isApprovedCR));
                   // changePermission();
                }

            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadBalanceDocUrl() {
            BalanceVendorService.balanceDocUrl(function (reply) {
                if (reply.status === 200) {
                    vm.balanceDocUrl = reply.data.DocUrl;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
            });
        }

        function checkIsVerified() {
            BalanceVendorService.isVerified(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    console.info("check"+JSON.stringify(reply));
                    var data = reply.data;
                    vm.verified = data.Isverified;
                    console.info("isver" + JSON.stringify(vm.verified));
                    //if (vm.verified === 1) {
                        //loadCheckCR();
                   // }
                   // changePermission();

                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function changePermission() {
            if (vm.verified !== null || (vm.verified === 1 && vm.isApprovedCR === true)) {
                vm.changeDataPermission = true;
            }
            //if (vm.isSentCR===true && vm.isApprovedCR===fal)
        }
        vm.loadVendor = loadVendor;
        function loadVendor() {
            BalanceVendorService.selectVendor(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.Vendor = reply.data;
                    console.info(vm.Vendor.VerifiedSendDate);
                    if (vm.Vendor.VerifiedSendDate === null && vm.Vendor.VerifiedDate===null) {
                        vm.IsApprovedCR = true;
                    }
                    //console.info("vendor" + JSON.stringify(vm.isApprovedCR));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Neraca Perusahaan" });
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
            vm.asset = 0;
            vm.hutang = 0;
            vm.modal = 0;
            //console.info("curr "+current)
            vm.vendorbalance = [];
            vm.currentPage = current;
            BalanceVendorService.select({VendorID: 0 }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.vendorbalance = reply.data;
                    console.info("curr " + JSON.stringify(vm.vendorbalance));
                    for (var i = 0; i < vm.vendorbalance.length; i++) {
                        if (vm.vendorbalance[i].WealthType.Name == "WEALTH_TYPE_ASSET") {
                            vm.listAsset = vm.vendorbalance[i];
                        }
                        if (vm.vendorbalance[i].WealthType.Name == "WEALTH_TYPE_DEBTH") {
                            vm.listDebth = vm.vendorbalance[i];
                        }
                    }
                    for (var i = 0; i < vm.vendorbalance.length; i++) {
                        for (var j = 0; j < vm.vendorbalance[i].subWealth.length; j++) {
                            if (vm.vendorbalance[i].subWealth[j].subCategory.length === 0) {
                                if (vm.vendorbalance[i].WealthType.RefID === 3097 && vm.vendorbalance[i].subWealth[j].IsActive === true) {
                                    if (vm.asset === 0) {
                                        vm.asset = vm.vendorbalance[i].subWealth[j].nominal;
                                        console.info(vm.asset);
                                    }
                                    else
                                        vm.asset = +vm.asset + +vm.vendorbalance[i].subWealth[j].nominal;
                                    console.info(vm.asset);

                                }
                                else if (vm.vendorbalance[i].WealthType.RefID === 3099 && vm.vendorbalance[i].subWealth[j].IsActive === true) {
                                    if (vm.hutang === 0) {
                                        vm.hutang = vm.vendorbalance[i].subWealth[j].nominal;
                                        console.info(vm.hutang);
                                    }
                                    else {
                                        vm.hutang = +vm.hutang + +vm.vendorbalance[i].subWealth[j].nominal;
                                        console.info(vm.hutang);
                                    }


                                }
                            }
                            else {
                                for (var k = 0; k < vm.vendorbalance[i].subWealth[j].subCategory.length; k++) {
                                    console.info(vm.vendorbalance[i].subWealth[j].subCategory[k]);
                                    if (vm.vendorbalance[i].subWealth[j].subCategory[k].Wealth.RefID === 3097 && vm.vendorbalance[i].subWealth[j].subCategory[k].IsActive === true) {
                                        if (vm.asset === 0) {
                                            vm.asset = vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
                                            console.info(vm.asset);
                                        }
                                        else
                                            vm.asset = +vm.asset + +vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
                                        console.info(vm.asset);

                                    }
                                    else if (vm.vendorbalance[i].subWealth[j].subCategory[k].WealthType === 3099 && vm.vendorbalance[i].subWealth[j].subCategory[k].IsActive === true) {
                                        if (vm.hutang === 0) {
                                            vm.hutang = vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
                                            console.info(vm.hutang);
                                        }
                                        else {
                                            vm.hutang = +vm.hutang + +vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
                                            console.info(vm.hutang);
                                        }


                                    }
                                }
                            }
                            
                        }
                    }
                    vm.modal = +vm.asset - +vm.hutang;
                                
                    console.info(JSON.stringify(vm.modal));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Neraca Perusahaan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.ubah_aktif = ubah_aktif;
        function ubah_aktif(data, active) {
            UIControlService.loadLoading("Silahkan Tunggu");
            console.info("ada:"+JSON.stringify(data))
            BalanceVendorService.editActive({
                BalanceID: data.BalanceID,
                IsActive: active,
                isApprovedCR: vm.isApprovedCR
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var msg = "";
                    if (active === false) msg = "Non Aktifkan ";
                    if (active === true) msg = "Aktifkan ";
                    UIControlService.msg_growl("success", "Data Berhasil di " + msg);
                    jLoad(1);
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
                    return;
                }
            }, function (err) {

                UIControlService.msg_growl("error", "Gagal Akses API ");
                UIControlService.unloadLoading();
            });

        }

        vm.tambah = tambah;
        function tambah() {
            console.info("masuk form add/edit");
            var data = {
                act: true
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/data-perusahaan/neraca/formNeraca.html',
                controller: 'formNeracaCtrl',
                controllerAs: 'formNeracaCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                window.location.reload();
            });
        }

        vm.edit = edit;
        function edit(data, flag) {
            console.info("masuk form add/edit");
            if (flag == 1) {
                var data = {
                    act: false,
                    item: data
                }
            }
            if (flag != 1) {

                var data = {
                    act: false,
                    item:
                    {
                        BalanceID: flag.BalanceID,
                        Wealth: data,
                        COA: flag.COAType,
                        Unit: flag.Unit,
                        Amount: flag.Amount,
                        DocUrl: flag.DocUrl,
                        Nominal: flag.nominal
                    }
                }
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/data-perusahaan/neraca/formNeraca.html',
                controller: 'formNeracaCtrl',
                controllerAs: 'formNeracaCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
        }

        vm.upload = upload;
        function upload() {
            console.info("masuk form add/edit");
            var data = {
                act: true
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/data-perusahaan/neraca/formUploadNeraca.html',
                controller: 'frmUploadNeracaCtrl',
                controllerAs: 'frmUploadNeracaCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                window.location.reload();
                //$scope.main.loadMenus();
                //AuthService.getUserLogin(function (reply) {
                //    if (reply.status === 200) {
                //        $scope.main.getCurrUser();
                //        //console.info(JSON.stringify(reply));
                //        AuthService.getRoleUserLogin({ Keyword: reply.data.CurrentUsername }, function (reply1) {
                //            if (reply1.status === 200 && reply1.data.List.length > 0) {
                //                var role = reply1.data.List[0].RoleName;
                //                UIControlService.msg_growl("notice", 'NOTIFICATION.LOGIN.SUCCESS.MESSAGE', "NOTIFICATION.LOGIN.SUCCESS.TITLE");
                //                if (role === 'APPLICATION.ROLE_VENDOR') {
                //                    $state.go('vendor-balance');
                //                } else if (role === 'APPLICATION.ROLE_VENDOR_INTERNATIONAL') {
                //                    $state.go('vendor-balance');
                //                } else {
                //                    $state.go('homeadmin');
                //                }
                //            } else {
                //                UIControlService.msg_growl("error", "User Tidak Berhak Login");
                //                $state.go('home');
                //            }
                //        }, function (err1) {
                //            UIControlService.msg_growl("error", "MESSAGE.API");
                //        });
                //    }
                //}, function (err) {
                //    UIControlService.msg_growl("error", "MESSAGE.API");
                //});
            });
        }
    }
})();
