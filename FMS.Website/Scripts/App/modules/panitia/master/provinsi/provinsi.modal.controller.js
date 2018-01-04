(function () {
	'use strict';

	angular.module("app").controller("provinsiModalCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ProvinsiService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance'];

	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, ProvinsiService,
		RoleService, UIControlService, item, $uibModalInstance) {
		var vm = this;
		//console.info("masuuk modal : " + JSON.stringify(item));
		vm.isAdd = item.act;
		vm.action = "";
		vm.namaProvinsi = "";
		vm.timezone = "";
		vm.countryID = 0;
		vm.stateID = 0;
		vm.data;
		
		vm.init = init;
		function init() {
		    $translatePartialLoader.addPart('master-provinsi');
		    if (vm.isAdd === true) {
		        vm.action = "Tambah ";
		    }
		    else {
		        vm.action = "Ubah ";
		        vm.data = item.item;
		        vm.namaProvinsi = vm.data.Name;
		        vm.stateID = vm.data.StateID;
		    }
		    getRegion();
		}

		vm.batal = batal;
		function batal() {
		    $uibModalInstance.dismiss('cancel');
		};

		vm.listRegions = [];
		vm.selectedRegions;
		function getRegion() {
		    ProvinsiService.getRegion(
                function (response) {
                    vm.listRegions = response.data;
                    if (vm.isAdd === false) {
                        for (var i = 0; i < vm.listRegions.length; i++) {
                            if (vm.data.Country.Continent.ContinentID === vm.listRegions[i].ContinentID) {
                                vm.selectedRegions = vm.listRegions[i];
                                vm.changeRegion();
                                break;
                            }
                        }
                    }
                },
            function (response) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                return;
            });
		}

		vm.changeRegion = changeRegion;
		vm.listCountry = [];
		vm.selectedCountry;
		function changeRegion() {
		    ProvinsiService.getCountries(vm.selectedRegions.ContinentID,
               function (response) {
                   //console.info("neg>" + JSON.stringify(response));
                   vm.listCountry = response.data;
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listCountry.length; i++) {
                           if (vm.data.CountryID === vm.listCountry[i].CountryID) {
                              // console.info("masuuuk")
                               vm.selectedCountry = vm.listCountry[i];
                               changeCountry();
                               break;
                           }
                       }
                   }
               },
           function (response) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.changeCountry = changeCountry;
		vm.listProvince = [];
		vm.selectedProvince;
		function changeCountry() {
		    //console.info("idreg:" + JSON.stringify(vm.selectedRegions));
		    //console.info("idneg:" + JSON.stringify(vm.selectedCountry));
		    vm.timezone = vm.selectedRegions.Name + "/" + vm.selectedCountry.Name;
		    vm.countryID = vm.selectedCountry.CountryID;
		}

		vm.prosesSimpan = prosesSimpan;
		function prosesSimpan() {
		    
		    if (vm.selectedRegions === undefined) {
		        UIControlService.msg_growl("warning", "Nama Benua belum dipilih!!");
		        return;
		    }

		    if (vm.selectedCountry === undefined) {
		        UIControlService.msg_growl("warning", "Nama Negara belum dipilih!!");
		        return;
		    }
		    //proses simpan data
		    UIControlService.loadLoadingModal("Silahkan Tunggu");
		    if (vm.isAdd === true) {
		        ProvinsiService.insert({
		            Name: vm.namaProvinsi, Timezone: vm.timezone, CountryID: vm.countryID
		        }, function (reply) {
		            UIControlService.unloadLoadingModal();
		            //console.info("ins: " + JSON.stringify(reply));
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
		        ProvinsiService.update({
		            StateID: vm.stateID, Name: vm.namaProvinsi, Timezone: vm.timezone, CountryID: vm.countryID
		        }, function (reply) {
		            UIControlService.unloadLoadingModal();
		            //console.info("ins: " + JSON.stringify(reply));
		            if (reply.status === 200) {
		                UIControlService.msg_growl("notice", "Berhasil Simpan Perubahan Data Master Asosiasi!!");
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

	}
})();