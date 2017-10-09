(function () {
	'use strict';

	angular.module("app").controller("formTenagaAhliCtrl", ctrl);

	ctrl.$inject = ['TenagaAhliService', 'UIControlService', 'item', '$uibModalInstance'];

	function ctrl(TenagaAhliService, UIControlService, item, $uibModalInstance) {
		var vm = this;
		vm.act = item.act;
		vm.item = item.item;
		vm.Gender = "";
		vm.tenagaahli = {};
		vm.isCalendarOpened = [false, false, false, false];
		vm.addresses = { AddressInfo: "" };
		vm.countrys = { Name: "" };
		vm.statuss = { Name: "" };
		vm.radio = {
			tipeM: "M",
			tipeF: "F",
			StatusK: "CONTRACT",
			StatusI: "INTERNSHIP",
			StatusP: "PERMANENT",
		}
		vm.nationalities = ["Indonesia"];

		vm.init = init;
		function init() {
			var tomorrow = new Date();
			console.info("Act:" + vm.act);
			tomorrow.setDate(tomorrow.getDate() + 1);
			var afterTomorrow = new Date(tomorrow);
			afterTomorrow.setDate(tomorrow.getDate() + 30);
			console.info(afterTomorrow);
			UIControlService.loadLoadingModal("Silahkan Tunggu");

			TenagaAhliService.GetAllNationalities(function (reply) {
				UIControlService.unloadLoadingModal();
				if (reply.status === 200) {
					vm.nationalities = reply.data;
				} else {
					UIControlService.msg_growl("error", "Gagal mendapat daftar negara");
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal mendapat daftar negara");
				UIControlService.unloadLoadingModal();
			});

			if (vm.act === false) {
				vm.Name = vm.item.Name;
				vm.tenagaahli.BirthDate = vm.item.DateOfBirth;
				if (vm.item.Gender === "M") {
					vm.Gender = "M";
				} else if (vm.item.Gender === "F") {
					vm.Gender = "F";
				}
				vm.address = vm.item.address.AddressInfo;
				vm.Education = vm.item.Education;
				vm.Nationality = vm.item.country.Name;
				vm.Position = vm.item.Position;
				vm.YearOfExperience = vm.item.YearOfExperience;
				vm.Email = vm.item.Email;

				if (vm.item.Statusperson.Name === "CONTRACT") {
					vm.Status = "CONTRACT";
				} else if (vm.item.Statusperson.Name === "INTERNSHIP") {
					vm.Status = "INTERNSHIP";
				} else if (vm.item.Statusperson.Name === "PERMANENT") {
					vm.Status = "PERMANENT";
				}

				vm.Expertise = vm.item.Expertise;
			}

			convertToDate();
		}


		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		};

		function convertAllDateToString() { // TIMEZONE (-)
			if (vm.tenagaahli.BirthDate) {
				vm.tenagaahli.BirthDate = UIControlService.getStrDate(vm.tenagaahli.BirthDate);
			}
		};

		//Supaya muncul di date picker saat awal load
		function convertToDate() {
			if (vm.tenagaahli.BirthDate) {
				vm.tenagaahli.BirthDate = new Date(Date.parse(vm.tenagaahli.BirthDate));
			}
		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		};

		vm.update = update;
		vm.vendor = {};
		function update() {
			convertAllDateToString();
			UIControlService.loadLoadingModal("Silahkan Tunggu");
			if (vm.act === false) {
				vm.addresses.AddressInfo = vm.address;
				vm.countrys.Name = vm.Nationality;
				vm.statuss.Value = vm.Status;
				vm.vendor = {
					ID: vm.item.ID,
					Name: vm.Name,
					DateOfBirth: vm.tenagaahli.BirthDate,
					Gender: vm.Gender,
					address: vm.addresses,
					Education: vm.Education,
					country: vm.countrys,
					Position: vm.Position,
					YearOfExperience: vm.YearOfExperience,
					Email: '',
					Statusperson: vm.statuss,
					Expertise: vm.Expertise
				};
				TenagaAhliService.update(vm.vendor, function (reply) {
					UIControlService.unloadLoadingModal();
					if (reply.status === 200) {
						UIControlService.msg_growl("success", "Data Tenaga Ahli Berhasil di update");
						$uibModalInstance.close();
					} else {
						UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
						return;
					}
				}, function (err) {
					UIControlService.msg_growl("error", "Data Tenaga Ahli gagal di update");
					UIControlService.unloadLoadingModal();
				});
			} else if (vm.act === true) {
				vm.addresses.AddressInfo = vm.address;
				vm.countrys.Name = vm.Nationality;
				vm.statuss.Value = vm.Status;
				vm.vendor = {
					Name: vm.Name,
					DateOfBirth: vm.tenagaahli.BirthDate,
					Gender: vm.Gender,
					address: vm.addresses,
					Education: vm.Education,
					country: vm.countrys,
					Position: vm.Position,
					Email: '',
					Statusperson: vm.statuss,
					Expertise: vm.Expertise
				};
				TenagaAhliService.insert(vm.vendor, function (reply) {
					UIControlService.unloadLoadingModal();
					if (reply.status === 200) {
						UIControlService.msg_growl("success", "Data Tenaga Ahli Berhasil di update");
						$uibModalInstance.close();
					} else {
						UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
						return;
					}
				}, function (err) {
					UIControlService.msg_growl("error", "Data Tenaga Ahli gagal di update");
					UIControlService.unloadLoadingModal();
				});
			}
		}

		vm.addToList = addToList;
		function addToList() {
			vm.addresses.AddressInfo = vm.address;
			vm.countrys.Name = vm.Nationality;
			vm.statuss.Name = vm.Status;
			vm.vendor = {
				Name: vm.Name,
				DateOfBirth: vm.tenagaahli.BirthDate,
				Gender: vm.Gender,
				address: vm.addresses,
				Education: vm.Education,
				country: vm.countrys,
				Position: vm.Position,
				YearOfExperience: vm.YearOfExperience,
				Email: '',
				Statusperson: vm.statuss,
				Expertise: vm.Expertise
			};
		}

		vm.add = add;
		function add(data) {
			console.info(JSON.stringify(data));
		}
	}
})();