(function () {
    'use strict';

    angular.module("app").controller("GoodsAwardCtrl", ctrl);

    ctrl.$inject = ['$filter','Excel','$timeout','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'GoodsAwardService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($filter,Excel,$timeout, $http, $translate, $translatePartialLoader, $location, SocketService, GoodsAwardService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        var page_id = 141;
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.evalsafety = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.kata = new Kata("");
        vm.init = init;
        vm.exportHref;
        vm.detail = [];
        vm.jLoad = jLoad;
        vm.isCalendarOpened = [false, false, false, false];

        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);
            getUserLogin();

        }
        vm.jLoad = jLoad;
        function jLoad(current) {
            var tender = {
                column: vm.StepID,
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            GoodsAwardService.select(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detail = reply.data.List;
                    for (var i = 0; i < vm.detail.length; i++) {
                        vm.detail[i].PODate = new Date(Date.parse(vm.detail[i].PODate));
                        if (vm.detail[i].ApprovalStatusReff == null) {
                            vm.detail[i].Status = 'Draft';
                            vm.detail[i].flagTemp = 0;
                        }
                        else if (vm.detail[i].ApprovalStatusReff != null) {
                            cekEmployee(vm.detail[i].ID, vm.detail[i]);
                            vm.detail[i].Status = vm.detail[i].ApprovalStatusReff.Value;
                            vm.detail[i].flagTemp = 1;
                        }
                        vm.detail[i].StartContractDate = new Date(Date.parse(vm.detail[i].StartContractDate));
                    }
                    vm.count = reply.data.Count;
                    if (vm.count !== 0) {

                        loadExcelVendor();
                    }
                    console.info("data:" + JSON.stringify(reply.data));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Peneapan Pemenang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.cekEmployee = cekEmployee;
        function cekEmployee(Id, reff) {
            GoodsAwardService.CekEmployee({
                ID: Id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    reff.flagEmp = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString(data) { // TIMEZONE (-)
            if (data) {
                data = UIControlService.getStrDate(data);
            }
        };

        //supaya muncul di date picker saat awal load
        function convertToDate(data) {
            if (data) {
                data = new Date(Date.parse(data));
            }
        }

        vm.loadExcelVendor = loadExcelVendor;
        function loadExcelVendor() {
            vm.vendor = [];
            var tender = {
                column: vm.StepID,
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            GoodsAwardService.selectExcelVendor(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.vendor = reply.data;
                    console.info("data:" + JSON.stringify(vm.vendor));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Peneapan Pemenang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.save = save;
        function save(data) {
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
            GoodsAwardService.updateApproval(data, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Approval Data!!");
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.simpan = simpan;
        vm.List = [];
        function simpan() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            for (var i = 0; i < vm.detail.length; i++) {
                //if (vm.detail[i].NoPO === null) {
                //    UIControlService.msg_growl("warning", "No PO belum diisi !!");
                //    return;
                //}
                convertAllDateToString(vm.detail[i].PODate);

                if (vm.detail[i].PODate === null) {

                    UIControlService.msg_growl("warning", "Tanggal PO belum diisi !!");
                    return;
                }
                var dataGoods = {
                    ID: vm.detail[i].ID,
                    TenderStepID: vm.detail[i].TenderStepID,
                    VendorID: vm.detail[i].VendorID,
                    TotalNego: vm.detail[i].TotalNego,
                    NoPO: vm.detail[i].NoPO,
                    PODate: vm.detail[i].PODate
                }
                vm.List.push(dataGoods);
                if (i == vm.detail.length - 1 && vm.List.length === vm.detail.length) {
                    GoodsAwardService.update(vm.List, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            vm.ListDeal = [];
                            vm.flag = false;
                            UIControlService.msg_growl("success", "Berhasil Simpan Data PO!!");
                            window.location.reload();
                            
                        }
                        else {
                            UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                            return;
                        }
                    }, function (err) {
                        UIControlService.msg_growl("error", "Gagal Akses Api!!");
                        UIControlService.unloadLoadingModal();
                    });
                }
            }
            
        }

        vm.edit = edit;
        function edit(dataTabel) {
            var data = {
                item: dataTabel
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/penetapan-pemenang-barang/Detail.html',
                controller: 'DetailGoodsAwardCtrl',
                controllerAs: 'DetailGoodsAwardCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

        vm.Export = Export;
        function Export(tableId) {
            vm.exportHref = Excel.tableToExcel(tableId, 'sheet name');
            $timeout(function () { location.href = vm.exportHref; }, 100); // trigger download
        }

        function sendMail(data) {
            var email = {
                subject: 'Notifikasi Pemenang ',
                mailContent: 'Selamat Anda adalah pemendang ',
                isHtml: false,
                addresses: data
            };

            UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
            GoodsAwardService.sendMail(email,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                            loadNotDeal();
                            UIControlService.msg_growl("notice", "Email Telah dikirim ke vendor pemenang")
                    } else {
                        UIControlService.handleRequestError(response.data);
                    }
                },
                function (response) {
                    UIControlService.handleRequestError(response.data);
                    UIControlService.unloadLoading();
                });
        }

        vm.loadNotDeal = loadNotDeal;
        function loadNotDeal() {
            var model = {
                column: vm.StepID,
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            GoodsAwardService.selectVendorNotDeal(model, function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detailNotDeal = reply.data;
                    vm.ListNotDeal = [];
                    for (var j = 0; j < vm.detailNotDeal.length; j++) {
                        vm.ListNotDeal.push(vm.detailNotDeal[j].Email);
                    }
                    var email = {
                        subject: 'Notifikasi Pemenang Tender' + vm.TenderName,
                        mailContent: 'Maaf anda belum berhasil memenangkan Tender' + vm.TenderName,
                        isHtml: false,
                        addresses: vm.ListNotDeal
                    };

                    UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
                    GoodsAwardService.sendMail(email,
                        function (response) {
                            UIControlService.unloadLoading();
                            if (response.status == 200) {
                                UIControlService.msg_growl("notice", "Email Telah dikirim ke vendor kalah");
                                init();
                            } else {
                                UIControlService.handleRequestError(response.data);
                            }
                        },
                        function (response) {
                            UIControlService.handleRequestError(response.data);
                            UIControlService.unloadLoading();
                        });
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Peneapan Pemenang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.getUserLogin = getUserLogin;
        function getUserLogin() {
            GoodsAwardService.CekRequestor({
                TenderRefID: vm.TenderRefID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.flagSave = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.sendToApprove = sendToApprove;
        function sendToApprove(data) {
            bootbox.confirm($filter('translate')('Apakah anda ingin mengirim data approval ini untuk proses persetujuan?'), function (yes) {
                if (yes) {
                    UIControlService.loadLoading("MESSAGE.LOADING");
                    var dt = {
                        ID: data.ID,
                        TenderStepID: vm.StepID,
                        flagEmp: 1
                    };
                    GoodsAwardService.SendApproval(dt, function (reply) {
                        if (reply.status === 200) {
                            console.info(reply.data);
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SEND_TO_APPRV'));
                            init();
                        } else {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
                        }
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
                    });
                }
            });
        }
        vm.detailApproval = detailApproval;
        function detailApproval(dt, data) {
            var item = {
                ID: data.ID,
                flag: data.flagEmp,
                Status: dt
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/penetapan-pemenang-barang/detailApproval.modal.html',
                controller: 'detailApprovalSignOffCtrl',
                controllerAs: 'detailApprovalSignOffCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function () {
                init();
            });
        };

        vm.createExcel = createExcel;
        function createExcel() {
            var data = {
                StepID: vm.StepID,
                TenderRefID: vm.TenderRefID,
                ProcPackType: vm.ProcPackType
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/penetapan-pemenang-barang/SaveExcelNotif.html',
                controller: "SaveNotifExcel",
                controllerAs: "SaveNotifExcel",
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

    }
})();
//TODO

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

