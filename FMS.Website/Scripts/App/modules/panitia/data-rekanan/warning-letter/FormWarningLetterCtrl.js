(function () {
    'use strict';

    angular.module("app").controller("FormWarningLetterCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'WarningLetterService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance', '$state'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, WarningLetterService,
        RoleService, UIControlService, item, $uibModalInstance, $state) {
        var vm = this;
        var page_id = 141;

        vm.isAdd = item.act;
        vm.Area;
        vm.LetterID;
        vm.Description="";
        vm.action = "";
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
            if (vm.isAdd === true) {
                vm.action = "Tambah";
                
            }
            else {
                vm.action = "Ubah";
                vm.warningLetter.RequestedDate = item.item.Date;
                vm.ReporterName = item.item.ReporterName;
                vm.Description = item.item.Description;
            }

            getEmployeeName();
            getVendorName();
            getWarning();
            convertToDate();

        }

        vm.getEmployeeName = getEmployeeName;
        vm.selectedReporter;
        vm.listReporter = [];
        function getEmployeeName() {
            WarningLetterService.SelectEmployeeName(
               function (response) {
                   if (response.status === 200) {
                       vm.listReporter = response.data;
                       if (vm.isAdd === false) {
                           for (var i = 0; i < vm.listReporter.length; i++) {
                               if (item.item.department.DepartmentID === vm.listReporter[i].DepartmentID) {
                                   vm.selectedReporter = vm.listReporter[i];

                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list tipe wilayah");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses API");
                   return;
               });
        }

        vm.getVendorName = getVendorName;
        vm.selectedVendorName;
        vm.listVendorName = [];
        function getVendorName() {
            WarningLetterService.SelectVendorName(
               function (response) {
                   if (response.status === 200) {
                       vm.listVendorName = response.data;
                       if (vm.isAdd === false) {
                           for (var i = 0; i < vm.listVendorName.length; i++) {
                               if (item.item.vendor.VendorID === vm.listVendorName[i].VendorID) {
                                   vm.selectedVendorName = vm.listVendorName[i];

                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list tipe wilayah");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses API");
                   return;
               });
        }


        vm.getWarningType = getWarningType;
        vm.selectedWarningType;
        vm.listWarningType = [];
        function getWarningType() {
            WarningLetterService.SelectWarningType(
               function (response) {
                   if (response.status === 200) {
                       vm.listWarningType = response.data;
                       if (vm.isAdd === false) {
                           for (var i = 0; i < vm.listWarningType.length; i++) {
                               if (item.item.WarningID === vm.listWarningType[i].RefID) {
                                   vm.selectedWarningType = vm.listWarningType[i];
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list tipe wilayah");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses API");
                   return;
               });
        }

        vm.getWarning = getWarning;
        vm.selectedWarning;
        vm.listWarning = [];
        function getWarning() {
            WarningLetterService.SelectWarning(
               function (response) {
                   if (response.status === 200) {
                       vm.listWarning = response.data;
                       if (vm.isAdd === false) {
                           for (var i = 0; i < vm.listWarning.length; i++) {
                               if (item.item.WarningID === vm.listWarning[i].RefID) {
                                   vm.selectedWarning = vm.listWarning[i];
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list tipe wilayah");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses API");
                   return;
               });
        }

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString() { // TIMEZONE (-)
            if (vm.warningLetter.RequestedDate) {
                vm.warningLetter.RequestedDate = UIControlService.getStrDate(vm.warningLetter.RequestedDate);
            }
            if (vm.warningLetter.StartDate) {
                vm.warningLetter.StartDate = UIControlService.getStrDate(vm.warningLetter.StartDate);
            }
            if (vm.warningLetter.EndDate) {
                vm.warningLetter.EndDate = UIControlService.getStrDate(vm.warningLetter.EndDate);
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate() {
            if (vm.warningLetter.RequestedDate) {
                vm.warningLetter.RequestedDate = new Date(Date.parse(vm.warningLetter.RequestedDate));
            }
            if (vm.warningLetter.StartDate) {
                vm.warningLetter.StartDate = new Date(Date.parse(vm.warningLetter.StartDate));
            }
            if (vm.warningLetter.EndDate) {
                vm.warningLetter.EndDate = new Date(Date.parse(vm.warningLetter.EndDate));
            }
        }

        vm.simpan = simpan;
        function simpan() {
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
            if (vm.action === "Tambah") {
                convertAllDateToString();

                console.info(JSON.stringify(vm.selectedReporter.EmployeeID));
                WarningLetterService.insert({
                    Date: vm.warningLetter.RequestedDate,
                    ReporterName: vm.ReporterName,
                    ReporterID: vm.selectedReporter.DepartmentID,
                    VendorID: vm.selectedVendorName.VendorID,
                    WarningID: vm.selectedWarning.RefID,
                    Description: vm.Description
                    
                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Data PengaduanDepartment!!");
                        $uibModalInstance.close();
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
            else if (vm.action === "Ubah") {
                WarningLetterService.update({
                    //LetterID: vm.LetterID,
                    //CreatedDate: vm.warningLetter.RequestedDate,
                    //VendorID: vm.selectedVendorName.VendorID,
                    //WarningTypeID: vm.selectedWarningType.RefID,
                    //StartDate: vm.warningLetter.StartDate,
                    //EndDate: vm.warningLetter.EndDate,
                    //Area: vm.Area,
                    //Description: vm.Description,
                    //IsActive: item.item.IsActive,
                    //ReporterID: vm.selectedReporter.ID
                    ComplaintID: item.item.ComplaintID,
                    IsActive: true,
                        Date: vm.warningLetter.RequestedDate,
                        ReporterName: vm.ReporterName,
                        ReporterID: vm.selectedReporter.DepartmentID,
                        VendorID: vm.selectedVendorName.VendorID,
                        WarningID: vm.selectedWarning.RefID,
                        Description: vm.Description
                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Update Data Warning Letter!!");
                        $uibModalInstance.close();
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

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

        vm.tambah = tambah;
        function tambah(data, act) {
            console.info("masuk form add/edit");
            var data = {
                act: act,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/FormWarningLetter.html',
                controller: 'FormWarningLetterCtrl',
                controllerAs: 'frmWarningLetterCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                $state.transitionTo('master-warningLetter');
            });
        }

        }
})();