(function () {
	'use strict';

	angular.module("app").controller("PengurusPerusahaanController", ctrl);

	ctrl.$inject = ['$uibModal', '$filter', '$translatePartialLoader', 'PengurusPerusahaanService', 'CommonEngineService', 'UIControlService'];
	/* @ngInject */
	function ctrl($uibModal, $filter, $translatePartialLoader, PengurusPerusahaanService, CommonEngineService, UIControlService) {
		var vm = this;
		var loadmsg = 'MESSAGE.LOADING';

		vm.compPersons = [];
		vm.vendorName;
		vm.vendorID;
		vm.bisaMengubahData;
		vm.isChangeData = false;
		vm.isApprovedCR = false;
		vm.finalApproveBy = null;
		vm.isEditedByVendor = false;

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('pengurus-perusahaan');
			UIControlService.loadLoading(loadmsg);
			/*
			CommonEngineService.GetLoggedVendor(function (reply) {
				if (reply.status === 200) {
					console.info("get logged vendor" + JSON.stringify(reply.data));
					vm.vendorName = reply.data.Name;
					vm.vendorID = reply.data.VendorID;
					//TODO : cek permintaan ubah data
					vm.bisaMengubahData = true;
					loadData();
					loadCheckCR();
					//loadContactCompany();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_USER');
				}
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_USER');
			});*/
			chekcIsVerified();
		};
		vm.loadContactCompany = loadContactCompany;
		function loadContactCompany() {
			PengurusPerusahaanService.selectContact({ VendorID: vm.vendorID }, function (reply) {
				if (reply.status == 200) {
					vm.contact = reply.data;
					vm.vendorLocation = [];
					console.info("kontak" + JSON.stringify(vm.contact));
					for (var i = 0; i < vm.contact.length; i++) {
						if (vm.contact[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY') {
							console.info("this" + JSON.stringify(vm.contact[i]));
							vm.vendorLocation = vm.contact[i].Contact.Address.State.Country.Code;
							break;
						}
					}
					console.info("vendor location" + JSON.stringify(vm.vendorLocation));
				} else {
					UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
				return;
			});
		}

		vm.submitCP = submitCP;
		function submitCP() {
			bootbox.confirm('<h3 class="afta-font center-block">' + $filter('translate')('MESSAGE.CONFIRM_SUBMIT') + '<h3>', function (reply) {
				if (reply) {
					UIControlService.loadLoading(loadmsg);
					PengurusPerusahaanService.Submit({ OpsiCode: 'OC_COMPANYPERSON' }, function (reply2) {
						UIControlService.unloadLoading();
						if (reply2.status === 200) {
							UIControlService.msg_growl('notice', 'MESSAGE.SUCC_SUBMIT');
							loadData();
						} else
							UIControlService.msg_growl('error', 'MESSAGE.ERR_SUBMIT');
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl('error', 'MESSAGE.ERR_SUBMIT');
					});
				}
			});
		}

		function loadCheckCR() {
			UIControlService.loadLoading("Silahkan Tunggu");
			PengurusPerusahaanService.getCRbyVendor({ CRName: 'OC_COMPANYPERSON' }, function (reply) {
				//PermintaanUbahDataService.getCRbyVendor(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					console.info("CR:" + JSON.stringify(reply));

					if (reply.data === true) { //has data
						vm.isApprovedCR = true;
						//reply.data[0].ChangeRequestDataDetails[0].IsEditedByVendor == null ? vm.isEditedByVendor = false : vm.isEditedByVendor = reply.data[0].ChangeRequestDataDetails[0].IsEditedByVendor;
						//vm.finalApproveBy = reply.data[0].ChangeRequestDataDetails[0].FinalApproveBy;
					} else {
						vm.isApprovedCR = false;
					}

					//if (!(reply.data === null) && reply.data.ApproveBy === 1) {
					//	vm.isApprovedCR = true;
					//}
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		function chekcIsVerified() {
			PengurusPerusahaanService.isVerified(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					console.info(JSON.stringify(reply));
					var data = reply.data;
					vm.vendorID = data.VendorID;
					if (!(data.Isverified === null)) {
						vm.isChangeData = true;
					}
					loadContactCompany();
					loadData();
					loadCheckCR();
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		function loadData() {
			PengurusPerusahaanService.GetByVendor({
				VendorID: vm.vendorID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.compPersons = reply.data;
					vm.compPersons.forEach(function (cp) {
						cp.DateOfBirthConverted = UIControlService.convertDate(cp.DateOfBirth);
					});
					checkMandatoryPositions();
					console.info("peng>> " + JSON.stringify(reply));
				} else {
					UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD');
					UIControlService.unloadLoading();
				}
			}, function (err) {
				UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD');
				UIControlService.unloadLoading();
			});
		}

		function checkMandatoryPositions() {
			vm.noPresDir = true;
			vm.noFinDir = true;
			vm.noOperDir = true;

			vm.compPersons.forEach(function (cp) {
				if (cp.PositionRef === 'PRESIDENT_DIRECTOR') {
					vm.noPresDir = false;
				} else if (cp.PositionRef === 'DIRECTOR_OF_FINANCE') {
					vm.noFinDir = false;
				} else if (cp.PositionRef === 'DIRECTOR_OF_OPERATIONS') {
					vm.noOperDir = false;
				}
			});
		}

		vm.addCP = addCP;
		function addCP() {
			var iscr = false;
			if (vm.isApprovedCR === true && vm.isChangeData === true) {
				iscr = true;
			}
			var lempar = {
				compPerson: {
					VendorID: vm.vendorID,
					PositionRef: 'OTHERS',
					Address: {},
					IsCR: iscr,
					Location: vm.vendorLocation
				},
                action: 'add'
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/pengurus-perusahaan/pengurus-perusahaan.formModal.html',
				controller: 'formPengurusPerusahaanCtrl',
				controllerAs: 'formPPCtrl',
				resolve: {
					item: function () {
						return lempar;
					}
				}
			});
			modalInstance.result.then(function () {
			    window.location.reload();
			});
		};

		vm.editCP = editCP;
		function editCP(cp) {
			var lempar = {
				compPerson: {
					ID: cp.ID,
					VendorID: vm.vendorID,
					PersonName: cp.PersonName,
					DateOfBirth: new Date(Date.parse(cp.DateOfBirth)),
					NoID: cp.NoID,
					IDUrl: cp.IDUrl,
					PersonAddress: cp.PersonAddress,
					PositionRef: cp.PositionRef,
					ServiceStartDate: new Date(Date.parse(cp.ServiceStartDate)),
					ServiceEndDate: new Date(Date.parse(cp.ServiceEndDate)),
					Address: cp.Address,
                    CompanyPosition: cp.CompanyPosition
				},
                action:'edit'
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/pengurus-perusahaan/pengurus-perusahaan.formModal.html',
				controller: 'formPengurusPerusahaanCtrl',
				controllerAs: 'formPPCtrl',
				resolve: {
					item: function () {
						return lempar;
					}
				}
			});
			modalInstance.result.then(function () {
				loadData();
			});
		};

		vm.viewCP = viewCP;
		function viewCP(cp) {
			var lempar = {
				compPerson: {
					ID: cp.ID,
					VendorID: vm.vendorID,
					PersonName: cp.PersonName,
					DateOfBirth: UIControlService.convertDate(cp.DateOfBirth),
					NoID: cp.NoID,
					IDUrl: cp.IDUrl,
					PersonAddress: cp.PersonAddress,
					PositionRef: cp.PositionRef,
					ServiceStartDate: UIControlService.convertDate(cp.ServiceStartDate),
					ServiceEndDate: UIControlService.convertDate(cp.ServiceEndDate),
					Address: {
						AddressInfo: cp.Address.AddressInfo,
						AddressDetail: cp.Address.AddressDetail,
					},
					Country: cp.Address.State.Country.Name,
                    CompanyPosition: cp.CompanyPosition
				}
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/pengurus-perusahaan/pengurus-perusahaan.viewModal.html',
				controller: 'viewPengurusPerusahaanCtrl',
				controllerAs: 'viewPPCtrl',
				resolve: {
					item: function () {
						return lempar;
					}
				}
			});
		};

		vm.deleteCP = deleteCP;
		function deleteCP(cp) {
			bootbox.confirm('<h3 class="afta-font center-block">' + $filter('translate')('MESSAGE.CONFIRM_DEL') + '<h3>', function (reply) {
				if (reply) {
					UIControlService.loadLoading(loadmsg);
					PengurusPerusahaanService.Delete({
						ID: cp.ID
					}, function (reply2) {
						UIControlService.unloadLoading();
						if (reply2.status === 200) {
							UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
							window.location.reload();
						} else
							UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
					});
				}
			});
		};
	}
})();