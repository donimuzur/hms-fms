(function () {
    'use strict';

    angular.module("app").controller("pegawaiModalCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PegawaiService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance'];

    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PegawaiService,
		RoleService, UIControlService, item, $uibModalInstance) {
        var vm = this;
        console.info("masuuk modal : " + JSON.stringify(item));
        
        //vm.action = "";
        
        //vm.Kode = item.item.PgrCode;
        
        vm.Nama = item.item.Name;
        vm.PosCode = item.item.subdepartemen;
        vm.ID = item.item.employeeID;
        vm.a = "camelia";
        vm.jLoad3 = jLoad3;
        vm.Pegawais2=[];
        vm.currentPage = 0;
        vm.maxSize = 10;
        vm.totalItems = 0;
        vm.Kode=[];

        vm.init = init;
        function init() {
            jLoad3(1);
            }

        vm.jLoad3 = jLoad3;
        function jLoad3(current) {
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            PegawaiService.Select2({
                Offset: offset,
                Limit: vm.maxSize,
                Keyword: vm.ID
            }, function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                //console.info(JSON.stringify(reply.data.List));
                    var data = reply.data;
                    vm.Pegawais2 = data.List[0];
                    vm.Kode = vm.Pegawais2.PgrCode;
                    vm.totalItems = Number(data.Count);
                }
                else {
                    $.growl.error({ message: "Gagal mendapatkan data Master Pegawai" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


        //vm.Nama = item.item.Name;
        





        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

        vm.simpan = simpan;
        function simpan() {
            console.info(vm.Nama + " - " +  vm.Kode);
            if (vm.Kode === "" || vm.Kode === null) {
                alert("Kode PGR belum diisi!!");
                
            }
            else {
                prosesSimpan();
            }

        }

        //proses simpan
        vm.prosesSimpan = prosesSimpan;
        function prosesSimpan() {
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
            PegawaiService.update2({
                EmployeeID : vm.ID, PgrCode : vm.Kode
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    console.info("data:" + JSON.stringify(reply));
                    UIControlService.msg_growl("success", "Berhasil Simpan Data Pegawai!!");
                    $uibModalInstance.close();
                }
                else {
                    //console.info("Gagal!");
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                    return;
                }
            }, function (err) {
                //Console.info("Error!");
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.cekKodeNama = cekKodeNama;
        function cekKodeNama() {
            //pengecekkan kode atau nama sudah ada belum?
            DepartemenService.cekData({
                column: 1, Keyword: vm.kode_depart
            }, function (reply) {
                //console.info("cek1:" + JSON.stringify(reply));
                if (reply.status === 200 && reply.data.length > 0) {
                    UIControlService.msg_growl("warning", "Kode sudah ada, silahkan masukkan kode yg lain!!");
                    return;
                }
                else if (reply.status === 200 && reply.data.length <= 0) {
                    DepartemenService.cekData({
                        column: 2, Keyword: vm.nama_depart
                    }, function (reply2) {
                        //console.info("cek2:" + JSON.stringify(reply2));
                        if (reply2.status === 200 && reply2.data.length > 0) {
                            UIControlService.msg_growl("warning", "Nama sudah ada, silahkan masukkan kode yg lain!!");
                            return;
                        }
                        else if (reply2.status === 200 && reply2.data.length <= 0) {
                            prosesSimpan();
                        }
                        else {
                            UIControlService.msg_growl("error", "Gagal melakukan pengecekkan data");
                            return;
                        }
                    }, function (err) {
                        UIControlService.msg_growl("error", "Gagal Akses API!!");
                        UIControlService.unloadLoading();
                    });
                }
                else {
                    UIControlService.msg_growl("error", "Gagal melakukan pengecekkan data");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                UIControlService.unloadLoading();
            });
        }

    }
})();