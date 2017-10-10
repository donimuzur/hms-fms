(function () {
	'use strict';

	angular.module("app").controller("ProvinsiCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ProvinsiService', 'RoleService', 'UIControlService', '$uibModal' ];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, ProvinsiService,
        RoleService, UIControlService, $uibModal) {

		var vm = this;
		var page_id = 141;

		vm.provinsiList = [];
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.pageSize = 10;
		vm.txtSearch= "";
		vm.init = init;

		function init() {
		    $translatePartialLoader.addPart('master-provinsi');
			//UIControlService.loadLoading("Silahkan Tunggu...");
			//jLoad(1);
			getRegion();
		}

		vm.listRegions = [];
		vm.selectedRegions;
		function getRegion() {
		   ProvinsiService.getRegion(
               function (response) {
                   vm.listRegions = response.data;
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
		    //console.info("idreg:" + vm.selectedRegions.ContinentID);
		    ProvinsiService.getCountries(vm.selectedRegions.ContinentID,
               function (response) {
                   //console.info("neg>" + JSON.stringify(response));
                   vm.listCountry = response.data;
               },
           function (response) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		//vm.changeCountry = changeCountry;
		//vm.listProvince = [];
		//vm.selectedProvince;
		//function changeCountry() {
		//    //console.info("idneg:" + JSON.stringify(vm.selectedCountry));
		//    ProvinsiService.getStates(vm.selectedCountry.CountryID,
        //       function (response) {
        //           //console.info("pro>" + JSON.stringify(response));
        //           vm.listProvince = response.data;
        //       },
        //   function (response) {
        //       UIControlService.msg_growl("error", "Gagal Akses API");
        //       return;
        //   });
		//}

		vm.showData = showData;
		function showData() {

		        vm.jLoad(1);
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			vm.provinsiList = [];
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			ProvinsiService.select({
				Offset: offset,
				Limit: vm.pageSize,
				Status: vm.selectedCountry.CountryID
			}, function (reply) {
			    //console.info("data:"+JSON.stringify(reply));
			    UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.provinsiList = data.List;
					vm.totalItems = Number(data.Count);
				} else {
				    UIControlService.msg_growl("error", "Gagal Mendapatkan Data Provinsi");
				    UIControlService.unloadLoading();
				    return;
				}
			}, function (err) {
			    UIControlService.msg_growl("error", "Gagal Akses API!! ");
			    UIControlService.unloadLoading();
			    return;
			});
		}
        
		vm.ubah_aktif = ubah_aktif;
		function ubah_aktif(data, active) {
		    UIControlService.loadLoading("Silahkan Tunggu");  
            //console.info("ada:"+JSON.stringify(data))
		    ProvinsiService.editActive({ StateID: data.StateID
                , IsActive: active }
                , function (reply) {
			    UIControlService.unloadLoading();
			    if (reply.status === 200) {
			        var msg = "";
			        if(active === false) msg = " NonAktifkan ";
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

		vm.openForm = openForm;
		function openForm(data, isAdd) {
		    //console.info("masuk form add/edit"+isAdd);
		    var data = {
		        act: isAdd,
		        item: data
		    }
		    var modalInstance = $uibModal.open({
			    templateUrl: 'app/modules/panitia/master/provinsi/formProvinsi.html',
			    controller: 'provinsiModalCtrl',
			    controllerAs: 'provinsiModalCtrl',
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
	}
})();

