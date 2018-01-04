(function () {
    'use strict';

    angular.module("app").controller("UbahWarningLetterCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'WarningLetterService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance', '$sce'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, WarningLetterService,
        RoleService, UIControlService, item, $uibModalInstance, $sce) {
        var vm = this;

        vm.departemen = [];
        var page_id = 141;
        vm.one;
        vm.letternomor = "";
        vm.isAdd = item.act;
        vm.Area;
        vm.isActive = false;
        vm.LetterID;
        vm.Description = "";
        vm.action = "";
        vm.init = init;
        vm.warningLetter = {};
        vm.isCalendarOpened = [false, false, false, false];

        function init() {
            vm.warningLetter.CreatedDate = item.data1.CreatedDate;
            vm.selectedWarningType = item.data1.SysReference.RefID;
            vm.warningLetter.StartDate = item.data1.StartDate;
            vm.warningLetter.EndDate = item.data1.EndDate;
            vm.location = item.data1.Area;
            getWarningType();
			vm.trusteddepartemen = "";
			convertToDate();
			
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
                               if (vm.selectedWarningType === vm.listWarningType[i].RefID) {
                                   vm.selectedWarningType = vm.listWarningType[i];
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list Jenis Peringatan");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses API");
                   return;
               });
        }


        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            vm.isActive = true;
            //console.info("curr "+current)
            console.info(JSON.stringify(vm.selectedWarningType));
            WarningLetterService.SelectTemplate({
                Status: vm.selectedWarningType.RefID
            }, function (reply) {
                console.info(JSON.stringify());
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.departemen = data.List[0].TemplateDescription;
                    convertAllDateToString();
                    console.info(JSON.stringify(vm.warningLetter.CreatedDate));
                    replace();

                    vm.trusteddepartemen = $sce.trustAsHtml(vm.departemen);

                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Template Warning Letter" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString() { // TIMEZONE (-)
            //if (item.data1.RequestedDate) {
            //    item.data1.RequestedDate = UIControlService.getStrDate(item.data1.RequestedDate);
            //}
            if (vm.warningLetter.CreatedDate) {
                vm.warningLetter.CreatedDate = UIControlService.getStrDate(vm.warningLetter.CreatedDate);
            }
            if (vm.warningLetter.StartDate) {
                vm.warningLetter.StartDate = UIControlService.getStrDate(vm.warningLetter.StartDate);
            }
            if (vm.warningLetter.EndDate) {
                vm.warningLetter.EndDate = UIControlService.getStrDate(vm.warningLetter.EndDate);
            }
        };

        function replace() {
            //var RequestedDate = vm.warningLetter.RequestedDate.split('-');
            var CreatedDate = vm.warningLetter.CreatedDate.split('-');
            var StartDate = vm.warningLetter.StartDate.split('-');
            var EndDate = vm.warningLetter.EndDate.split('-');
            if (vm.selectedWarningType.Name === "WARNING_TYPE_ADMONITION-1") {
                vm.departemen = vm.departemen
                                           .replace(/#nomor_surat/g, vm.letternomor)
                                           .replace(/#lokasi/g, vm.location)
                                           .replace(/#tgl_surat/g, (CreatedDate[2] + '-' + CreatedDate[1] + '-' + CreatedDate[0]))
                                           .replace(/#nama_vendor/g, item.data1.Vendor.VendorName)
                                           //.replace(/#alamat_vendor/g, item.data1.Vendor.)
                                           //.replace(/#alamat_kec_vendor/g, isiTemplate.nama_paket)
                                           //.replace(/#alamat_kab_vendor/g, (stu[2] + '-' + stu[1] + '-' + stu[0]))
                                           //.replace(/#email_vendor/g, toWords(rekanan.length))
                                           //.replace(/#phone_vendor/g, rekanan.length)
                                           //.replace(/#nama_direktur_vendor/g, rekanan_string)
                                           .replace(/#admonition_ke/g, "Admonition-01")
                                           //.replace(/#admonition_contract/g, panitia_list)
                                           //.replace(/#dear_direktur_vendor/g, (stu[2] + '-' + stu[1] + '-' + stu[0]))
                                           .replace(/#tgl_admonition/g, (CreatedDate[2] + '-' + CreatedDate[1] + '-' + CreatedDate[0]))
                                           .replace(/#solusi_vendor/g, vm.solution)
                                           .replace(/#admonition_ke_angka/g, "1")
                                           .replace(/#admonition_ke_huruf/g, "satu")
                                           .replace(/#tgl_berlaku_peringatan/g, (StartDate[2] + '-' + StartDate[1] + '-' + StartDate[0]))
                                           .replace(/#tgl_akhir_peringatan/g, (EndDate[2] + '-' + EndDate[1] + '-' + EndDate[0]))
                                           .replace(/#tgl_admonition/g, (CreatedDate[2] + '-' + CreatedDate[1] + '-' + CreatedDate[0]))
                //.replace(/#nama_approver/g, toWords(rekanan.length)) //login
                //.replace(/#jabatan_approver/g, rekanan.length)
                //.replace(/#nama_xc/g, rekanan_string);//inputan
            }

            else if (vm.selectedWarningType.Name === "WARNING_TYPE_ADMONITION-2") {
                vm.departemen = vm.departemen
                                            .replace(/#nomor_surat/g, vm.letternomor)
                                            .replace(/#lokasi/g, vm.location)
                                            .replace(/#tgl_surat/g, (CreatedDate[2] + '-' + CreatedDate[1] + '-' + CreatedDate[0]))
                                            .replace(/#nama_vendor/g, item.data1.Vendor.VendorName)
                                            //.replace(/#alamat_vendor/g, item.data1.Vendor.)
                                            //.replace(/#alamat_kec_vendor/g, isiTemplate.nama_paket)
                                            //.replace(/#alamat_kab_vendor/g, (stu[2] + '-' + stu[1] + '-' + stu[0]))
                                            //.replace(/#email_vendor/g, toWords(rekanan.length))
                                            //.replace(/#phone_vendor/g, rekanan.length)
                                            //.replace(/#nama_direktur_vendor/g, rekanan_string)
                                            .replace(/#admonition_ke/g, "Admonition-02")
                                            //.replace(/#admonition_contract/g, panitia_list)
                                            //.replace(/#dear_direktur_vendor/g, (stu[2] + '-' + stu[1] + '-' + stu[0]))
                                            .replace(/#tgl_admonition/g, (CreatedDate[2] + '-' + CreatedDate[1] + '-' + CreatedDate[0]))
                                            .replace(/#solusi_vendor/g, vm.solution)
                                            .replace(/#admonition_ke_angka/g, "2")
                                            .replace(/#admonition_ke_huruf/g, "dua")
                                            .replace(/#tgl_berlaku_peringatan/g, (StartDate[2] + '-' + StartDate[1] + '-' + StartDate[0]))
                                            .replace(/#tgl_akhir_peringatan/g, (EndDate[2] + '-' + EndDate[1] + '-' + EndDate[0]))
                                            .replace(/#tgl_admonition/g, (CreatedDate[2] + '-' + CreatedDate[1] + '-' + CreatedDate[0]))
                //.replace(/#nama_approver/g, toWords(rekanan.length)) //login
                //.replace(/#jabatan_approver/g, rekanan.length)
                //.replace(/#nama_xc/g, rekanan_string);//inputan
            }
            
            else if (vm.selectedWarningType.Name === "WARNING_TYPE_NOTIFICATION") {
                vm.departemen = vm.departemen
                                            .replace(/#nomor_surat/g, vm.letternomor)
                                            .replace(/#lokasi/g, vm.location)
                                            .replace(/#tgl_surat/g, (CreatedDate[2] + '-' + CreatedDate[1] + '-' + CreatedDate[0]))
                                            .replace(/#nama_vendor/g, item.data1.Vendor.VendorName)
                                            //.replace(/#alamat_vendor/g, item.data1.Vendor.)
                                            //.replace(/#alamat_kec_vendor/g, isiTemplate.nama_paket)
                                            //.replace(/#alamat_kab_vendor/g, (stu[2] + '-' + stu[1] + '-' + stu[0]))
                                            //.replace(/#email_vendor/g, toWords(rekanan.length))
                                            //.replace(/#phone_vendor/g, rekanan.length)
                                            //.replace(/#nama_direktur_vendor/g, rekanan_string)
                                            .replace(/#nama_vendor/g, vm.solution)
                                            .replace(/#ket_pelanggaran/g, vm.solution)
                                            .replace(/#ket_pasal_pelanggaran/g, vm.solution)
                                            .replace(/#sanksi_pelanggaran/g, vm.solution)
                                            .replace(/#tgl_berlaku_peringatan/g, (StartDate[2] + '-' + StartDate[1] + '-' + StartDate[0]))
                                            .replace(/#tgl_akhir_peringatan/g, (EndDate[2] + '-' + EndDate[1] + '-' + EndDate[0]))
                                            .replace(/#tgl_admonition/g, (CreatedDate[2] + '-' + CreatedDate[1] + '-' + CreatedDate[0]))
                //.replace(/#nama_approver/g, toWords(rekanan.length)) //login
                //.replace(/#jabatan_approver/g, rekanan.length)
                //.replace(/#nama_xc/g, rekanan_string);//inputan
            }

        };

        //Supaya muncul di date picker saat awal load
        function convertToDate() {
            if (vm.warningLetter.CreatedDate) {
                vm.warningLetter.CreatedDate = new Date(Date.parse(vm.warningLetter.CreatedDate));
            }
            if (item.data1.CreatedDateDate) {
                item.data1.CreatedDate = new Date(Date.parse(item.data1.CreatedDate));
            }
            if (vm.warningLetter.StartDate) {
                vm.warningLetter.StartDate = new Date(Date.parse(vm.warningLetter.StartDate));
            }
            if (vm.warningLetter.EndDate) {
                vm.warningLetter.EndDate = new Date(Date.parse(vm.warningLetter.EndDate));
            }
        }

        vm.SimpanClick = SimpanClick;
        function SimpanClick() {
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
                WarningLetterService.UpdateTemplate({
                    LetterID: item.data1.LetterID,
                    TemplateDoc: vm.departemen,

                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Data Warning Letter!!");
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
})();