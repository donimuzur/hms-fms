
(function () {
	'use strict';

	angular.module("app").controller("FormNonBuildingController", ctrl);

	ctrl.$inject = ['$scope', '$http', '$uibModalInstance', '$cookieStore', '$state', '$stateParams', '$rootScope',
        '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataPerlengkapanService', 'item',
        'ProvinsiService', 'UIControlService'];
	/* @ngInject */
	function ctrl($scope, $http, $uibModalInstance, $cookieStore, $state, $stateParams, $rootScope,
        $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataPerlengkapanService, item,
        ProvinsiService, UIControlService) {
		var vm = this;
		console.info("send:" + JSON.stringify(item));
		vm.dataForm = {};
		vm.isAdd = item.isForm;
		vm.isCategory = item.type;
		vm.title = "";
		vm.ID;
		vm.tipeform = item.type;
		var categoryID; //sesuai id ref
		vm.isCalendarOpened = [false, false, false, false];
		vm.tipeform = item.type;
		vm.dateconvert;
		//vm.IsCR = item.IsCR;
		//vm.IsTemporary;
		//vm.Action;
		//if (vm.isAdd === false) {
			//vm.cekTemporary = item.data.IsTemporary;
		//}
		//console.info("cek:: " + vm.cekTemporary);

		vm.init = init;
		function init() {
			//cek vehicle atau equipment
			vm.datenow = new Date();
			vm.datepickeroptions = {
				minMode: 'year',
				maxDate: vm.datenow
			}
			if (vm.isCategory === "vehicle") {
				vm.title = "Data Kendaraan";
				categoryID = 3136;
			}
			if (vm.isCategory === "equipment") {
				vm.title = "Data Peralatan";
				categoryID = 3137;
			}

			if (vm.isAdd === true) {
				vm.dataForm = new Field("", "", categoryID, 0, null, "", 0, "", "");
				vm.ID = 0;
			}
			else {
				vm.dataForm = new Field(item.data.Brand, item.data.Capacity, categoryID, item.data.Condition,
                    new Date(Date.parse(item.data.MfgDate)), item.data.Name, item.data.Ownership, item.data.SerialNo, item.data.Type);
				vm.ID = item.data.ID;
				vm.dateconvert = UIControlService.getStrDate(vm.dataForm.MfgDate);
			}
			loadConditional();
			loadKepemilikan();
		}

		vm.listConditional = [];
		vm.selectedConditional;
		function loadConditional() {
			UIControlService.loadLoading("Loading Data Kondisi");
			DataPerlengkapanService.getConditionEq(
            function (reply) {
            	UIControlService.unloadLoading();
            	vm.listConditional = reply.data.List;
            	if (vm.isAdd === false) {
            		for (var i = 0; i < vm.listConditional.length; i++) {
            			if (item.data.Condition === vm.listConditional[i].RefID) {
            				vm.selectedConditional = vm.listConditional[i];
            				break;
            			}
            		}
            	}
            }, function (err) {
            	UIControlService.msg_growl("error", "MESSAGE.API");
            	UIControlService.unloadLoading();
            });
		}

		vm.listKepemilikan = [];
		vm.selectedKepemilikan;
		function loadKepemilikan() {
			UIControlService.loadLoading("Loading Data Kepemilikan");
			DataPerlengkapanService.getOwnership(
            function (reply) {
            	UIControlService.unloadLoading();
            	vm.listKepemilikan = reply.data.List;
            	if (vm.isAdd === false) {
            		for (var i = 0; i < vm.listKepemilikan.length; i++) {
            			if (item.data.Ownership === vm.listKepemilikan[i].RefID) {
            				vm.selectedKepemilikan = vm.listKepemilikan[i];
            				break;
            			}
            		}
            	}
            }, function (err) {
            	UIControlService.msg_growl("error", "MESSAGE.API");
            	UIControlService.unloadLoading();
            });
		}

		/*open form date*/
		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		};

		vm.saveData = saveData;
		function saveData() {
			//console.info(JSON.stringify(vm.dataForm));
			//console.info(JSON.stringify(vm.selectedConditional));
			//console.info(JSON.stringify(vm.selectedKepemilikan));

			if (vm.selectedConditional === undefined) {
				UIControlService.msg_growl("warning", "MESSAGE.NO_CONDITIONAL");
				return;
			}
			if (vm.selectedKepemilikan === undefined) {
				UIControlService.msg_growl("error", "MESSAGE.NO_OWNERSHIP");
				return;
			}

			//if (vm.IsCR === false) { vm.IsTemporary = false; }
			//else if (vm.IsCR === true) { vm.IsTemporary = true; }

			//if (vm.isAdd === true) { vm.Action = 0; }
			//if (vm.isAdd === false) { vm.Action = 1; }

			var senddata = {
				ID: vm.ID,
				Brand: vm.dataForm.Brand,
				Capacity: vm.dataForm.Capacity,
				Category: categoryID,
				Condition: vm.selectedConditional.RefID,
				MfgDate: vm.dataForm.MfgDate,
				Name: vm.dataForm.Name,
				Ownership: vm.selectedKepemilikan.RefID,
				SerialNo: vm.dataForm.SerialNo,
				Type: vm.dataForm.Brand
			}
			//console.info(JSON.stringify(senddata.Capacity));
			var checkCapacity = angular.isString(senddata.Capacity);
			//console.info("isAdd?" + vm.isAdd);
			//console.info("isCR?" + vm.IsCR);
			//console.info("cekTemporary?" + vm.cekTemporary);
			if (vm.isAdd === true) {
				DataPerlengkapanService.insertNonBuilding(senddata, function (reply) {
					if (reply.status === 200) {
						//if (vm.IsCR === true && vm.isAdd === false) {
						//	editActiveNonBuilding(senddata, false, vm.action);
						//} else {
						UIControlService.msg_growl("success", "MESSAGE.SUCCES_SAVE_NONBUILDING");
						$uibModalInstance.close();
						//}
					}
				}, function (err) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", "MESSAGE.API");
				});
			} else if (vm.isAdd === false /*|| (!(vm.IsCR === false) && !(vm.cekTemporary === false))*/) {
				DataPerlengkapanService.updateNonBuilding(senddata, function (reply) {
					if (reply.status === 200) {
						UIControlService.msg_growl("success", "MESSAGE.SUCCES_UPDATE_NONBUILDING");
						$uibModalInstance.close();
					}
				}, function (err) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", "MESSAGE.API");
				});
			}
		}

		function editActiveNonBuilding(data, active, action) {
			DataPerlengkapanService.editActiveNonBulding({
				ID: data.ID,
				IsActive: active,
				//Action: action
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", "MESSAGE.SUCCES_SAVE_NONBUILDING");
					$uibModalInstance.close();
				} else {
					UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});
		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		}

		function Field(Brand, Capacity, Category, Condition, MfgDate, Name, Ownership, SerialNo, Type) {
			var self = this;
			self.Brand = Brand;
			self.Capacity = Capacity;
			self.Category = Category;
			self.Condition = Condition;
			self.MfgDate = MfgDate;
			self.Name = Name;
			self.Ownership = Ownership;
			self.SerialNo = SerialNo;
			self.Type = Type;
		}
	}
})();