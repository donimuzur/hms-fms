(function () {
	'use strict';

	angular.module("app").controller("addRFQCtrl", ctrl);

	ctrl.$inject = ['$uibModalInstance', 'item', '$uibModal', 'UIControlService', 'RFQVHSService'];

	function ctrl($uibModalInstance, item, $uibModal, UIControlService, RFQVHSService) {
		var vm = this;

		vm.formData = {};
		vm.formData.RFQVHSItems = [];
		vm.formData.Vendors = [];
		vm.formData.RFQSteps = [];
		vm.paymentTerms = [];

		vm.rfqStep = {};
		vm.isCalendarOpened = [];
		vm.addedVendor = [];
		vm.IsDeliveryTermFix = false;
		vm.allowEdit = item.allowEdit;

		function getDefaultTemplate() {
			UIControlService.loadLoadingModal('LOADING.GET.TEMPLATE');
			RFQVHSService.getDefaultTemplate(function (reply) {
				if (reply.status === 200) {
					vm.formData.RFQName = reply.data[0];
					vm.formData.NoticeText = reply.data[1];
					UIControlService.unloadLoadingModal();
				} else {
				}
			}, function (err) {
			});
		}

		function getCommodityByItem() {
			UIControlService.loadLoadingModal('LOADING.GET.COMMODITIES');
			var items = [];
			/*
		    vm.formData.RFQVHSItems.forEach(function (item) {
		        items.push({
		            Material: Number(item.Material)
		        });
		    });
            */
			if (vm.formData.RFQVHSItems.length > 0) {
				items.push({
					Material: Number(vm.formData.RFQVHSItems[0].Material)
				});
			}
			RFQVHSService.getCommoditiesByItem(items, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoadingModal();
					vm.commodities = reply.data;
					if (vm.commodities != null && vm.commodities.length > 0) {
						vm.formData.CommodityID = vm.commodities[0].ID;
					}
				} else {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.GET.COMMODITIES.ERROR', "NOTIFICATION.GET.COMMODITIES.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.COMMODITIES.ERROR', "NOTIFICATION.GET.COMMODITIES.TITLE");
			});
		};

		RFQVHSService.getCompScale(function (reply) {
			//UIControlService.loadLoadingModal('LOADING.GET.COMPSCALE');
			if (reply.status === 200) {
				vm.compScales = reply.data;
				//UIControlService.unloadLoadingModal();
			} else {
				//UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.COMPSCALE.ERROR', "NOTIFICATION.GET.COMPSCALE.TITLE");
			}
		}, function (err) {
			//UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.COMPSCALE.ERROR', "NOTIFICATION.GET.COMPSCALE.TITLE");
		});


		RFQVHSService.getFixCustom(function (reply) {
			//UIControlService.loadLoadingModal('LOADING.GET.FIXCUSTOM');
			if (reply.status === 200) {
				vm.deliveryTerms = reply.data;
				//UIControlService.unloadLoadingModal();
			} else {
				//UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.FIXCUSTOM.ERROR', "NOTIFICATION.GET.FIXCUSTOM.TITLE");
			}
		}, function (err) {
			//UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.FIXCUSTOM.ERROR', "NOTIFICATION.GET.FIXCUSTOM.TITLE");
		});

		function getIncoTerms() {
			RFQVHSService.getIncoTerms({
				BidderSelMethod: vm.formData.BidderSelMethod,
				DeliveryTerms: vm.formData.DeliveryTerms
			}, function (reply) {
				//UIControlService.loadLoadingModal('LOADING.GET.INCOTERMS');
				if (reply.status === 200) {
					vm.incoTerms = reply.data;
					//UIControlService.unloadLoadingModal();
				} else {
					//UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.GET.INCOTERMS.ERROR', "NOTIFICATION.GET.INCOTERMS.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.INCOTERMS.ERROR', "NOTIFICATION.GET.INCOTERMS.TITLE");
			});
		}

		RFQVHSService.getLocation(function (reply) {
			//UIControlService.loadLoadingModal('LOADING.GET.LOCATION');
			if (reply.status === 200) {
				vm.locations = reply.data;
				//UIControlService.unloadLoadingModal();
			} else {
				//UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
			}
		}, function (err) {
			//UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
		});

		RFQVHSService.getProcMethods(function (reply) {
			//UIControlService.loadLoadingModal('LOADING.GET.PROCMETHOD');
			if (reply.status === 200) {
				vm.procMethods = reply.data;
				//UIControlService.unloadLoadingModal();
			} else {
				//UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.PROCMETHOD.ERROR', "NOTIFICATION.GET.PROCMETHOD.TITLE");
			}
		}, function (err) {
			//UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.PROCMETHOD.ERROR', "NOTIFICATION.GET.PROCMETHOD.TITLE");
		});

		RFQVHSService.getPymentTerm(function (reply) {
			if (reply.status === 200) {
				vm.paymentTerms = reply.data;
			} else {
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.PAYMENTTERM.ERROR', "NOTIFICATION.GET.PAYMENTTERM.TITLE");
			}
		}, function (err) {
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.PAYMENTTERM.ERROR', "NOTIFICATION.GET.PAYMENTTERM.TITLE");
		});

		RFQVHSService.getTypeTender(function (reply) {
			if (reply.status === 200) {
				vm.listTypeTender = reply.data.List;
			} else {
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.TENDERTYPE.ERROR', "NOTIFICATION.GET.TENDERTYPE.TITLE");
			}
		}, function (err) {
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.TENDERTYPE.ERROR', "NOTIFICATION.GET.TENDERTYPE.TITLE");
		});

		RFQVHSService.getOptionsTender(function (reply) {
			if (reply.status === 200) {
				vm.listOptionsTender = reply.data.List;
			} else {
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.TENDEROPTION.ERROR', "NOTIFICATION.GET.TENDEROPTION.TITLE");
			}
		}, function (err) {
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.TENDEROPTION.ERROR', "NOTIFICATION.GET.TENDEROPTION.TITLE");
		});

		RFQVHSService.getBidderMethod(function (reply) {
			if (reply.status === 200) {
				vm.listBidderMethod = reply.data.List;
			} else {
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.BIDDERMETHOD.ERROR', "NOTIFICATION.GET.BIDDERMETHOD.TITLE");
			}
		}, function (err) {
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.BIDDERMETHOD.ERROR', "NOTIFICATION.GET.BIDDERMETHOD.TITLE");
		});

		RFQVHSService.getStateDelivery(function (reply) {
			if (reply.status === 200) {
				vm.listState = reply.data;
			} else {
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.LISTSTATE.ERROR', "NOTIFICATION.GET.LISTSTATE.TITLE");
			}
		}, function (err) {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.LISTSTATE.ERROR', "NOTIFICATION.GET.LISTSTATE.TITLE");
		});

		RFQVHSService.getDocTypes(function (reply) {
			if (reply.status === 200) {
				vm.tenderDocTypes = reply.data;
			} else {
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.DOCTYPES.ERROR', "NOTIFICATION.GET.DOCTYPES.TITLE");
			}
		}, function (err) {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.DOCTYPES.ERROR', "NOTIFICATION.GET.DOCTYPES.TITLE");
		});

		vm.changeState = changeState;
		function changeState() {
			vm.formData.DeliveryLocation = null;
			loadState();
		}

		function loadState() {
			UIControlService.loadLoadingModal('LOADING.GET.CITIES');
			RFQVHSService.getCityDelivery({
				StateID: vm.formData.DeliveryLocationState
			}, function (reply) {
				if (reply.status === 200) {
					vm.listCity = reply.data;
					UIControlService.unloadLoadingModal();
				} else {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.GET.LISTCITY.ERROR', "NOTIFICATION.GET.LISTCITY.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.LISTCITY.ERROR', "NOTIFICATION.GET.LISTCITY.TITLE");
			});
		}

		vm.setReviewer = setReviewer;
		function setReviewer(data) {
			if (vm.formData.Commitees == null)
				vm.formData.Commitees = [];

			var item = {
				//item: data
				item: vm.formData
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/setVHSFPAReviewer.html',
				controller: 'CommitteeModalCtrl',
				controllerAs: 'CommitteeModalCtrl',
				resolve: { item: function () { return item; } }
			});

			modalInstance.result.then(function (detail) {
				vm.formData.Commitees = detail;
				//loadRFQVHS();
			});
		}

		vm.bidderChange = bidderChange;
		function bidderChange() {
			getIncoTerms();
		}

		vm.changeType = changeType;
		function changeType() {
			UIControlService.loadLoadingModal('LOADING.GET.EVALMETHOD');

			if (vm.formData.RFQType !== '1') { //not VHS
				vm.formData.LeadTime = null;
			}
			getEvalMethod();
		}

		function getEvalMethod() {
			RFQVHSService.getEvalMethod({
				RFQType: vm.formData.RFQType
			}, function (reply) {
				if (reply.status === 200) {
					vm.evalMethods = reply.data;
					UIControlService.unloadLoadingModal();
					if (!item.ID) generateCode();
				} else {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.GET.EVALMETHOD.ERROR', "NOTIFICATION.GET.EVALMETHOD.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.EVALMETHOD.ERROR', "NOTIFICATION.GET.EVALMETHOD.TITLE");
			});
		}

		function generateCode() {
			UIControlService.loadLoadingModal('LOADING.GET.GENERATECODE');
			RFQVHSService.generateCode({
				RFQType: vm.formData.RFQType
			}, function (reply) {
				if (reply.status === 200) {
					vm.formData.RFQCode = reply.data;
					UIControlService.unloadLoadingModal();
				} else {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.GET.GENERATECODE.ERROR', "NOTIFICATION.GET.GENERATECODE.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.GENERATECODE.ERROR', "NOTIFICATION.GET.GENERATECODE.TITLE");
			});
		}

		vm.vendorSelectByChange = vendorSelectByChange;
		function vendorSelectByChange() {
			if (Number(vm.formData.VendorSelectBy) === 2) {
				vm.formData.IsLocal = false;
				vm.formData.IsNational = false;
				vm.formData.IsInternational = false;
				vm.formData.CommodityID = null;
				vm.formData.CompScale = null;
				vm.formData.IsVendorEmails = false;
				vm.formData.Emails = null;
			}
			if (Number(vm.formData.VendorSelectBy) === 1) {
				vm.addedVendor = [];
			}
		}

		vm.init = init;
		function init() {
			vm.today = new Date();
			vm.datepickeroptions = {
				minDate: vm.today,
			}
			if (item.ID) {
				UIControlService.loadLoadingModal('LOADING.LOAD.RFQ');
				RFQVHSService.loadRFQVHS({ ID: item.ID }, function (reply) {
					UIControlService.unloadLoadingModal();
					if (reply.status === 200) {
						vm.formData = reply.data;
						vm.formData.Commitees.forEach(function (temp) {
							temp.position = {};
							temp.employee = {};
							temp.position.PositionCode = temp.PositionCode;
							temp.position.PositionName = temp.PositionName;
							temp.position.PositionID = temp.PositionCommiteeId;
							temp.employee.ID = temp.EmployeeId;
							temp.employee.FullName = temp.EmployeeName;
							temp.employee.PositionName = temp.PositionName;
						});
						getCommodityByItem();
						vm.deliveryTermChange(vm.formData.DeliveryTerms);
						if (vm.formData.DeliveryLocationState > 0) {
							loadState();
						}
						vm.addedVendor = [];
						vm.formData.Vendors.forEach(function (vendor) {
							vm.addedVendor.push({
								VendorID: vendor.VendorID,
								Name: vendor.contact.Name,
								Email: vendor.contact.Email,
							});
						});
						vm.tenderSteps = vm.formData.RFQSteps;
						if (vm.tenderSteps.length > 0) {
							vm.tenderSteps[0].StartDate = new Date(vm.tenderSteps[0].StartDate);
						}
						getEvalMethod();
					} else {
						UIControlService.msg_growl("error", 'NOTIFICATION.LOAD.RFQ.ERROR', "NOTIFICATION.LOAD.RFQ.TITLE");
					}
				}, function (err) {
					UIControlService.msg_growl("error", 'NOTIFICATION.LOAD.RFQ.ERROR', "NOTIFICATION.LOAD.RFQ.TITLE");
					UIControlService.unloadLoadingModal();
				});
			} else {
				vm.formData = {};
				vm.formData.RFQVHSItems = [];
				vm.formData.Vendors = [];
				vm.formData.RFQSteps = [];
				getCommodityByItem();
				getDefaultTemplate();
			}
		}

		vm.chkBidder = chkBidder;
		function chkBidder() {
			UIControlService.loadLoadingModal('LOADING.VIEW.VENDOR');
			RFQVHSService.viewVendor({
				CommodityID: vm.formData.CommodityID,
				IsLocal: vm.formData.IsLocal,
				IsNational: vm.formData.IsNational,
				IsInternational: vm.formData.IsInternational,
				CompScale: vm.formData.CompScale
			}, function (reply) {
				if (reply.status === 200) {
					var result = reply.data;
					var arrEmail = vm.formData.Emails.split(',');
					for (var i = 0; i < result.length; i++) {
						for (var j = 0; j < arrEmail.length; j++) {
							if (result[i].Email.trim() == arrEmail[j].trim()) {
								arrEmail.splice(j, 1);
								vm.formData.Emails = arrEmail.join();
							}
						}
					}
					UIControlService.unloadLoadingModal();
				} else {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.VIEW.VENDOR.ERROR', "NOTIFICATION.VIEW.VENDOR.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.VIEW.VENDOR.ERROR', "NOTIFICATION.VIEW.VENDOR.TITLE");
			});
		}

		vm.getSteps = getSteps;
		function getSteps() {
			UIControlService.loadLoadingModal('LOADING.GET.STEPS');
			RFQVHSService.getSteps({
				ProcMethod: vm.formData.ProcMethod
			}, function (reply) {
				if (reply.status === 200) {
					vm.tenderSteps = reply.data;
					if (vm.tenderSteps.length > 0) {
						vm.tenderSteps[0].StartDate = new Date();
					}
					vm.tenderSteps.forEach(function (step) {
						step.Duration = 0;
					});
					getEndDate(0);
					UIControlService.unloadLoadingModal();
				} else {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.GET.STEPS.ERROR', "NOTIFICATION.GET.STEPS.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.STEPS.ERROR', "NOTIFICATION.GET.STEPS.TITLE");
			});
		}

		function convert_datetime(param) {
			return UIControlService.convertDateTime(param);
		};

		vm.getEndDate = getEndDate;
		function getEndDate(elementAt) {
			for (var i = elementAt; i < vm.tenderSteps.length; i++) {
				var dat = new Date(vm.tenderSteps[i].StartDate);
				if (vm.tenderSteps[i].Duration === null) {
					dat.setDate(dat.getDate() + 0);
				}
				else {
					dat.setDate(dat.getDate() + parseInt(vm.tenderSteps[i].Duration));
				}
				vm.tenderSteps[i].EndDate = UIControlService.getStrDate(dat);
				//vm.tenderSteps[i].EndDate = UIControlService.getStrDate(UIControlService.getEndDateByWeekday(vm.tenderSteps[i].StartDate, parseInt(vm.tenderSteps[i].Duration)));
				if (vm.tenderSteps[1 + i]) {
					dat.setDate(dat.getDate() + 1);
					vm.tenderSteps[1 + i].StartDate = UIControlService.getStrDate(dat);
				}
			}
		}

		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			if (vm.allowEdit) {
				vm.isCalendarOpened[index] = true;
			}
		};

		vm.uploadItem = uploadItem;
		function uploadItem() {
			var items = [];
			if (vm.formData.RFQVHSItems) {
				vm.formData.RFQVHSItems.forEach(function (item) {
					items.push(item);
				});
			}
			var item = {
				rfqvhsId: vm.formData.ID,
				items: items,
				allowEdit: vm.allowEdit,
				includeItems: vm.formData.IncludeItems
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/uploadItem.modal.html',
				controller: 'uploadItemController',
				controllerAs: 'uploadItemCtrl',
				resolve: { item: function () { return item; } }
			});
			modalInstance.result.then(function (result) {
				vm.formData.IncludeItems = true;
				vm.formData.RFQVHSItems = result;
				getCommodityByItem();
			});
		}

		vm.addVendor = addVendor;
		function addVendor() {
			var modalInstance = $uibModal.open({
				templateUrl: 'addVendorRFQ.html',
				controller: 'addVendorRFQCtrl',
				controllerAs: 'addVendorRFQCtrl',
				resolve: { items: function () { return vm.addedVendor; } }
			});

			modalInstance.result.then(function (addedVendor) {
				vm.addedVendor.push(addedVendor);
			});
		}

		vm.deleteVendor = deleteVendor;
		function deleteVendor(index) {
			vm.addedVendor.splice(index, 1);
		}

		vm.viewVendor = viewVendor;
		function viewVendor() {
			vm.viewVendorModel = {
				CommodityID: vm.formData.CommodityID,
				IsLocal: vm.formData.IsLocal,
				IsNational: vm.formData.IsNational,
				IsInternational: vm.formData.IsInternational,
				CompScale: vm.formData.CompScale
			};

			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/viewVendorRFQ.html',
				controller: 'viewVendorRFQCtrl',
				controllerAs: 'viewVendorRFQCtrl',
				resolve: { model: function () { return vm.viewVendorModel; }, }
			});
		}

		vm.deliveryTermChange = deliveryTermChange;
		function deliveryTermChange(id) {
			vm.IsDeliveryTermFix = false;
			vm.deliveryTerms.forEach(function (dt) {
				if (dt.RefID === Number(id) && dt.Name === 'RFQ_FIX') {
					vm.IsDeliveryTermFix = true;
				};
			});
			if (vm.IsDeliveryTermFix === false) {
				vm.formData.FreightCostID = null;
				vm.formData.IncoTerm = null;
				vm.formData.DeliveryLocation = null;
				vm.formData.DeliveryLocationState = null;
			}

			getIncoTerms();
		}

		vm.createRFQ = createRFQ;
		function createRFQ() {
			vm.formData.RFQSteps = [];
			for (var i = 0; i < vm.tenderSteps.length; i++) {
				vm.rfqStep = {
					TenderStepID: vm.tenderSteps[i].TenderStepID,
					Duration: vm.tenderSteps[i].Duration,
					StartDate: i === 0 ? UIControlService.getStrDate(vm.tenderSteps[i].StartDate) : vm.tenderSteps[i].StartDate,
					EndDate: vm.tenderSteps[i].EndDate,
					RFQStepDocuments: vm.tenderSteps[i].RFQStepDocuments,
				};
				vm.formData.RFQSteps.push(vm.rfqStep);
			}
			vm.formData.Vendors = vm.addedVendor;

			if (vm.formData.ID > 0) {
				UIControlService.loadLoadingModal('LOADING.UPDATE.MESSAGE');
				RFQVHSService.update(vm.formData, function (reply) {
					if (reply.status === 200) {
						UIControlService.unloadLoadingModal();
						UIControlService.msg_growl("info", 'NOTIFICATION.UPDATE.SUCCESS.MESSAGE', "NOTIFICATION.UPDATE.SUCCESS.TITLE");
						$uibModalInstance.close();
					} else {
						UIControlService.unloadLoadingModal();
						UIControlService.msg_growl("error", 'NOTIFICATION.UPDATE.ERROR.MESSAGE', "NOTIFICATION.UPDATE.ERROR.TITLE");
					}
				}, function (err) {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.UPDATE.ERROR.MESSAGE', "NOTIFICATION.UPDATE.ERROR.TITLE");
				});
			} else {
				UIControlService.loadLoadingModal('LOADING.CREATE.MESSAGE');
				RFQVHSService.create(vm.formData, function (reply) {
					if (reply.status === 200) {
						UIControlService.unloadLoadingModal();
						UIControlService.msg_growl("info", 'NOTIFICATION.CREATE.SUCCESS.MESSAGE', "NOTIFICATION.CREATE.SUCCESS.TITLE");
						$uibModalInstance.close();
					} else {
						UIControlService.unloadLoadingModal();
						UIControlService.msg_growl("error", 'NOTIFICATION.CREATE.ERROR.MESSAGE', "NOTIFICATION.CREATE.ERROR.TITLE");
						generateCode();
					}
				}, function (err) {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.CREATE.ERROR.MESSAGE', "NOTIFICATION.CREATE.ERROR.TITLE");
					generateCode();
				});
			}
		}

		vm.aturDokumen = aturDokumen;
		function aturDokumen(dt) {
			var item = {
				RFQStepDocuments: dt.RFQStepDocuments,
				tenderDocTypes: vm.tenderDocTypes,
				editable: vm.formData.IsPublished !== true
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/detailKelengkapanTender.modal.html',
				controller: 'detailKelengkapanTenderController',
				controllerAs: 'detKelTenderCtrl',
				resolve: { item: function () { return item; } }
			});
			modalInstance.result.then(function (result) {
				dt.RFQStepDocuments = result
			});
		}

		vm.closeModal = closeModal;
		function closeModal() {
			$uibModalInstance.close();
		}
	}
})();