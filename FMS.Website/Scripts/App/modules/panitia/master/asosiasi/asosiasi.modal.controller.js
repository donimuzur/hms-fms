(function () {
    'use strict';

    angular.module("app").controller("FormAsosiasiCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'AsosiasiService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance'];

    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, AsosiasiService,
		RoleService, UIControlService, item, $uibModalInstance) {
        //console.info("masuk modal");
        var vm = this;

        vm.associatonFld = new AsosiasiField(0, "", "", "", "", "", "", "","");
        vm.isAdd = item.act;
        vm.data = item.item;
        vm.action = "";
        vm.listProvinsi = [];
        vm.selectedProvince;
        vm.listKabupaten = [];
        vm.selectedKabupaten;
        vm.listKecamatan = [];
        vm.selectedKecamatan;
        vm.temp_name = "";

        vm.init = init;
        function init() {
            if(vm.isAdd === true) {
                vm.action = "Tambah ";
                getProvinsi();
		    }
            else {
                
                vm.action = "Ubah ";
                vm.associatonFld = new AsosiasiField(vm.data.AssosiationID, vm.data.AssosiationName, vm.data.Address[0].Address.AddressInfo,
                    vm.data.Address[0].Phone, vm.data.Address[0].Email, vm.data.AssociationInfo, vm.data.Address[0].Address.DistrictID,
                    vm.data.Address[0].Address.CityID, vm.data.Address[0].Address.StateID);
                vm.selectedProvince = vm.data.Address[0].Address.State;
                vm.temp_name = vm.data.AssosiationName;
                getProvinsi();
            }
        }

        function getProvinsi() {
            AsosiasiService.getProvince(360,
               function (response) {
                   vm.listProvinsi = response.data;
                   //console.info(">> " + JSON.stringify(vm.listProvinsi));
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listProvinsi.length; i++) {
                           if (vm.associatonFld.StateID === vm.listProvinsi[i].StateID) {
                               vm.selectedProvince = vm.listProvinsi[i];
                               vm.changeProvince();
                               break;                               
                           }
                       }
                   }

               },
           function (response) {
               handleError(response, "State");
           });
        }

        vm.changeProvince = changeProvince;
        function changeProvince() {
            var id;
            if (vm.isAdd === false) { id = vm.associatonFld.StateID; }
            else { id = vm.selectedProvince.StateID; }
            AsosiasiService.getCities(id,
               function (response) {
                   vm.listKabupaten = response.data;
                   vm.associatonFld.StateID = vm.selectedProvince.StateID;
                   
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listKabupaten.length; i++) {
                           if (vm.associatonFld.CityID === vm.listKabupaten[i].CityID) {
                               vm.selectedKabupaten = vm.listKabupaten[i];
                               vm.changeCities();
                               break;
                           }
                       }
                   }
                   
               },
           function (response) {
               handleError(response, "Cities");
           });
           
        }

        vm.changeCities = changeCities;
        function changeCities() {
            vm.associatonFld.CityID = vm.selectedKabupaten.CityID;
            var id;
            if (vm.isAdd === false) { id = vm.associatonFld.CityID; }
            else { id = vm.selectedKabupaten.CityID; }

            AsosiasiService.getDistrict(id,
               function (response) {
                   vm.listKecamatan = response.data;
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listKecamatan.length; i++) {
                           if (vm.associatonFld.DistrictID === vm.listKecamatan[i].DistrictID) {
                               vm.selectedKecamatan = vm.listKecamatan[i];
                               break;
                           }
                       }
                   }
               },
           function (response) {
               handleError(response, "District");
           });
           
        }

        vm.changeDistrict = changeDistrict;
        function changeDistrict() {
            //console.info("dis: " + JSON.stringify(vm.selectedKecamatan));
            vm.associatonFld.DistrictID = vm.selectedKecamatan.DistrictID;
        }

        vm.batal = batal;
		function batal() {
		    $uibModalInstance.dismiss('cancel');
        };

        vm.simpan = simpan;
        function simpan() {
            //console.info("kirim:" + JSON.stringify(vm.associatonFld));
            if (vm.associatonFld.AssociationName === "" || vm.associatonFld.AssociationName === null) {
                UIControlService.msg_growl("warning", "Nama Asosiasi belum diisi!!");
		        return;
            }
            if (vm.associatonFld.AddressInfo === "" || vm.associatonFld.AddressInfo === null) {
                UIControlService.msg_growl("warning", "Alamat Asosiasi belum diisi!!");
                return;
            }
            if (vm.associatonFld.StateID === "" || vm.associatonFld.StateID === null) {
                UIControlService.msg_growl("warning", "Provinsi Belum dipilih!!");
                return;
            }
            if (vm.associatonFld.CityID === "" || vm.associatonFld.CityID === null) {
                UIControlService.msg_growl("warning", "Kabupaten Belum dipilih!!");
                return;
            }
            if (vm.associatonFld.DistrictID=== "" || vm.associatonFld.DistrictID === null) {
                UIControlService.msg_growl("warning", "Kecamatan Belum dipilih!!");
                return;
            }
            if (vm.associatonFld.Phone === "" || vm.associatonFld.Phone === null) {
                UIControlService.msg_growl("warning", "Nomor Telepon Asosiasi belum diisi!!");
                return;
            }
            if (vm.associatonFld.Email === "" || vm.associatonFld.Email === null) {
                UIControlService.msg_growl("warning", "Alamat Email Asosiasi belum diisi!!");
                return;
            }
            console.info(vm.associatonFld.AssociationName + " : " + vm.temp_name + "=" + vm.isAdd);
            if (vm.isAdd === true) {
                vm.cekNama(vm.associatonFld.AssociationName);
            }
            else if (vm.isAdd === false && !(vm.associatonFld.AssociationName === vm.temp_name)) {
                vm.cekNama(vm.associatonFld.AssociationName);
            }
            else {
                prosesSimpan();
            }
        }

        vm.cekNama = cekNama;
        function cekNama(key) {
            //pengecekkan kode atau nama sudah ada belum?
            AsosiasiService.cekData({
               Keyword: key
            }, function (reply) {
                if (reply.status === 200 && reply.data.length > 0) {
                    UIControlService.msg_growl("warning", "Nama sudah ada, silahkan masukkan Nama yg lain!!");
		            return;
                }
                else if (reply.status === 200 && reply.data.length <= 0) { 
                    vm.prosesSimpan();
                }
                else {
                    UIControlService.msg_growl("error", "Gagal melakukan pengecekkan data!!");
		            return;
                }
		    }, function(err) { 
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
		        UIControlService.unloadLoading();
		    });
	    }

        vm.prosesSimpan = prosesSimpan;
        function prosesSimpan() {
            UIControlService.loadLoadingModal("Silahkan Tunggu");
            if (vm.isAdd === true) {
                
		        AsosiasiService.insert(vm.associatonFld, function (reply) {
		            UIControlService.unloadLoadingModal();
		            console.info("ins: " + JSON.stringify(reply));
		            if (reply.status === 200) {
		                UIControlService.msg_growl("notice", "Berhasil Simpan Data Master Asosiasi!!");
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
            else {
                console.info("masuk edit:"+JSON.stringify(vm.associatonFld));
		        AsosiasiService.update( vm.associatonFld, function (reply) {
		            UIControlService.unloadLoadingModal();
		            if (reply.status === 200) {
		                UIControlService.msg_growl("notice", "Berhasil Simpan Data Master Asosiasi!!");
		                $uibModalInstance.close();
		            }
		            else {
		                UIControlService.msg_growl("error", "Gagal menyimpan data!!");
		                return;
		            }
		        }, function (err) {
		            //console.info("error:" + JSON.stringify(err));
		            UIControlService.msg_growl("error", "Gagal Akses Api!!");
		            UIControlService.unloadLoadingModal();
		        });
		    }
        }

    }

})();

function AsosiasiField(AssosiationID, AssociationName, AddressInfo, Phone, Email, AssociationInfo,
    DistrictID, CityID, StateID) {
    var self = this;
    self.AssosiationID = AssosiationID;
    self.AssociationName = AssociationName;
    self.AddressInfo = AddressInfo;
    self.Phone = Phone;
    self.Email = Email;
    self.AssociationInfo = AssociationInfo;
    self.DistrictID = DistrictID;
    self.CityID = CityID;
    self.StateID = StateID;
}