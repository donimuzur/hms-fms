(function () {
    'use strict';

    angular.module("app").controller("DetailApprovalCtrl", ctrl);
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'VerifikasiDataService', 'UIControlService', 'GlobalConstantService', '$state'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        VerifikasiDataService, UIControlService, GlobalConstantService, $state) {
        var vm = this;
        vm.init = init;
        vm.aktifasi = "";
        vm.Keyword = "";
        vm.pageSize = 10;
        function init() {
            $translatePartialLoader.addPart('verifikasi-data');
            jLoad(1);
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.listApproval = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            VerifikasiDataService.selectApproval({
                Offset: offset,
                Limit: vm.pageSize,
                Keyword: vm.Keyword
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listApproval = data.List;
                    console.info(vm.listApproval);
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ApprovalVendor" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


        vm.cancel = cancel;
        function cancel() {
            $state.transitionTo('verifikasi-data');
        }

        vm.save = save;
        function save(flag, data) {
            var IsActive = false;
            if (data.IsActive === false) {
                vm.Active = "mengaktifkan";
                IsActive = true;
            }
            else vm.Active = "mengaktifkan";


            if (flag == true) {
                bootbox.confirm('<h3 class="afta-font">' + "Apa anda yakin menyetujui untuk " + vm.Active + " vendor ini?"+ '</h3>', function (res) {
                    if (res) {
                        UIControlService.loadLoading("Silahkan Tunggu");
                        VerifikasiDataService.editApproval({
                            ID: data.ID,
                            flagApprove: flag,
                            VendorID: data.VendorID,
                            IsActive: IsActive
                        }, function (reply) {
                            UIControlService.unloadLoading();
                            if (reply.status === 200) {
                                UIControlService.msg_growl("success", "Data Berhasil di Approve ");
                                init();
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
                    else {
                        console.info("sorry");
                    }
                });
                
            }
            else if (flag == true) {
                bootbox.confirm('<h3 class="afta-font">' + "Anda yakin ingin membatalkan "+ vm.Active +" untuk vendor ini?" + '</h3>', function (res) {
                    if (res) {
                        UIControlService.loadLoading("Silahkan Tunggu");
                        VerifikasiDataService.editApproval({
                            ID: data.ID,
                            flagApprove: flag,
                            VendorID: data.VendorID,
                            IsActive: IsActive
                        }, function (reply) {
                            UIControlService.unloadLoading();
                            if (reply.status === 200) {
                                UIControlService.msg_growl("success", "Data Berhasil di Approve ");
                                init();
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
                    else {
                        console.info("sorry");
                    }
                });

            }
        }
    }
})();
