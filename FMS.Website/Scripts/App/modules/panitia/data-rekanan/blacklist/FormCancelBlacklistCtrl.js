(function () {
    'use strict';

    angular.module("app").controller("FormCancelBlacklistCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'BlacklistService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance', '$state', '$uibModal'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, BlacklistService,
        RoleService, UIControlService, item, $uibModalInstance, $state, $uibModal) {
        var vm = this;
        var page_id = 141;

        vm.isAdd = item.act;
        vm.Area;
        vm.LetterID;
        vm.Description = "";
        vm.action = "";
        vm.actionblacklst = "";
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.init = init;
        vm.warningLetter = {};
        vm.isCalendarOpened = [false, false, false, false];

        function init() {
            console.info(JSON.stringify(item));
            if (item.act === true) {
                vm.action = "Tambah";
                vm.actionblacklist = "BLACKLIST";
            }
            else if (item.act === false){
            vm.action = "Pembatalan";
            vm.actionblacklist = "WHITELIST";
        }

        }

        vm.blacklist = blacklist;
        function blacklist() {
            BlacklistService.InsertBlacklist({
                VendorID: item.VendorID,
                ReferenceNo: item.ReferenceNo,
                BlacklistDescription: item.BlacklistDescription,
                DocUrl: item.DocUrl,
                VendorStock: item.vendorstock,
                CompPers: item.employee,
                DetailPerson: item.DetailPerson
            },
                             function (reply) {
                                 UIControlService.unloadLoadingModal();
                                 if (reply.status === 200) {
                                     UIControlService.msg_growl("success", "Berhasil Kirim Approval!!");
                                     $uibModalInstance.close(true);
                                 }
                                 else {
                                     UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                     return;
                                 }
                             },
                             function (err) {
                                 UIControlService.msg_growl("error", "Gagal Akses Api!!");
                                 UIControlService.unloadLoadingModal();
                             }
                         );


            //for (var i = 0; i < item.data.length; i++) {
            //    for (var y = 0; y < item.data[i].length; y++) {
            //        if (i == 0 && item.data[i][y].check == true) {
            //            BlacklistService.insert({
            //                VendorID: item.VendorID,
            //                VendorStockID: null,
            //                EmployeeVendorID: item.data[i][y].EmployeeVendorID,
            //                Description: item.data[i][y].Description
            //            },
            //                  function (reply) {
            //                      UIControlService.unloadLoadingModal();
            //                      if (reply.status === 200) {

            //                      }
            //                      else {
            //                          UIControlService.msg_growl("error", "Gagal menyimpan data!!");
            //                          return;
            //                      }
            //                  },
            //                  function (err) {
            //                      UIControlService.msg_growl("error", "Gagal Akses Api!!");
            //                      UIControlService.unloadLoadingModal();
            //                  }
            //              );
            //        }
            //        else if (i == 1 && item.data[i][y].check == true) {
            //            BlacklistService.insert({
            //                VendorID: item.VendorID,
            //                VendorStockID: item.data[i][y].StockID,
            //                EmployeeVendorID: null,
            //                Description: item.data[i][y].Description
            //            },
            //                  function (reply) {
            //                      UIControlService.unloadLoadingModal();
            //                      if (reply.status === 200) {

            //                      }
            //                      else {
            //                          UIControlService.msg_growl("error", "Gagal menyimpan data!!");
            //                          return;
            //                      }
            //                  },
            //                  function (err) {
            //                      UIControlService.msg_growl("error", "Gagal Akses Api!!");
            //                      UIControlService.unloadLoadingModal();
            //                  }
            //              );
            //        }
            //    }
            //    if (i == ((item.data.length) - 1) && item.data.length != 0) {
            //        BlacklistService.insertDetail({
            //            VendorID: item.VendorID,
            //            DetailPerson: item.DetailPerson
            //        },
            //            function (reply) {
            //                UIControlService.unloadLoadingModal();
            //                if (reply.status === 200) {
            //                    UIControlService.msg_growl("success", "Berhasil Simpan Data Blacklist !!");
            //                    $uibModalInstance.close(true);


            //                }
            //                else {
            //                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
            //                    return;
            //                }
            //            },
            //            function (err) {
            //                UIControlService.msg_growl("error", "Gagal Akses Api!!");
            //                UIControlService.unloadLoadingModal();
            //            }
            //        );
            //    }

            //}
        }

        vm.batalkan = batalkan;
        function batalkan() {
            BlacklistService.editBlacklist({
                VendorID: item.data.VendorID,
                BlacklistID: item.data.BlacklistID,
                BlacklistTypeID: item.data.BlacklistTypeID,
                ReferenceNo: item.ReferenceNo,
                DocUrl: item.DocUrl

            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Data Berhasil di batalkan blacklist");
                    $uibModalInstance.close(true);

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

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }
    }
})();