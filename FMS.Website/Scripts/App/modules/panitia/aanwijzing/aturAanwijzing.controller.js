(function () {
	'use strict';

	angular.module("app").controller("AturAanwijzingCtrl", ctrl);

	ctrl.$inject = ['SocketService', 'AanwijzingService', 'UIControlService', 'UploaderService', '$uibModal', '$stateParams', 'item', '$uibModalInstance', ];

	function ctrl(SocketService, AanwijzingService, UIControlService, UploaderService, $uibModal, $stateParams, item, $uibModalInstance) {
		//console.info("atur: "+JSON.stringify(item));
		var vm = this;
		vm.IsAtur = item.IsAtur;
		vm.TenderID = item.TenderID;
		vm.TenderStepID = item.TenderStepID;
		vm.StartDateStep = item.StartDateStep;
		vm.EndDateStep = item.EndDateStep;
		vm.TenderName = item.TenderName;
		vm.isCalendarOpened = [false, false, false, false];
		vm.selectedTypeAwj;
		vm.listAanwijzingType = [];
		vm.dateNow = new Date();
		vm.timezone = vm.dateNow.getTimezoneOffset();
		vm.timezoneClient = vm.timezone / 60;

		//console.info("atur: "+JSON.stringify(item));
		vm.init = init;
		function init() {
		    //angular detectBrowser
		    vm.isOpera = !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0; // Opera 8.0+ (UA detection to detect Blink/v8-powered Opera)
		    vm.isFirefox = typeof InstallTrigger !== 'undefined'; // Firefox 1.0+
		    vm.isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0; // At least Safari 3+: "[object HTMLElementConstructor]"
		    vm.isChrome = !!window.chrome && !vm.isOpera; // Chrome 1+
		    vm.isIE = /*@cc_on!@*/ false || !!document.documentMode; // At least IE6

		    console.info("item:" + JSON.stringify(item));
		    console.info("tenderID" + vm.TenderID);
			loadTypeAanwijzing();
			if (vm.IsAtur === false) {
			    vm.frm = new form(0, vm.TenderStepID, vm.TenderID, null, null, null, new Date(Date.parse(vm.EndDateStep)), null);
				console.info("frm aanwijzing:" + JSON.stringify(vm.frm));
			} else {
				vm.ID = item.data.ID;
				vm.VendorStartDate = new Date(Date.parse(item.data.StartDateVendorEntry));
				vm.VendorEndDate = new Date(Date.parse(item.data.EndDateVendorEntry));
				vm.AnswerStart = new Date(Date.parse(item.data.AnswerStartDate));
				vm.AnswerEnd = new Date(Date.parse(item.data.AnswerEndDate));
				vm.offset = vm.VendorStartDate.getTimezoneOffset();
				console.info("timezoneClient" + vm.timezoneClient);
				if (vm.isChrome === true) {
				    var objappVersion = navigator.appVersion; var objAgent = navigator.userAgent; var objbrowserName = navigator.appName; var objfullVersion = '' + parseFloat(navigator.appVersion); var objBrMajorVersion = parseInt(navigator.appVersion, 10); var objOffsetName, objOffsetVersion, ix;
				    if ((objOffsetVersion = objAgent.indexOf("Chrome")) != -1) { objbrowserName = "Chrome"; objfullVersion = objAgent.substring(objOffsetVersion + 7); }
				    if ((ix = objfullVersion.indexOf(";")) != -1) objfullVersion = objfullVersion.substring(0, ix);
				    if ((ix = objfullVersion.indexOf(" ")) != -1) objfullVersion = objfullVersion.substring(0, ix); objBrMajorVersion = parseInt('' + objfullVersion, 10);
				    if (isNaN(objBrMajorVersion)) { objfullVersion = '' + parseFloat(navigator.appVersion); objBrMajorVersion = parseInt(navigator.appVersion, 10); };
				    if (objBrMajorVersion >= 58) {
				        vm.VendorStartEntryDate = new Date(vm.VendorStartDate.setHours(vm.VendorStartDate.getHours() + 0));
				        vm.VendorEndEntryDate = new Date(vm.VendorEndDate.setHours(vm.VendorEndDate.getHours() + 0));
				        vm.AnswerStartDate = new Date(vm.AnswerStart.setHours(vm.AnswerStart.getHours() + 0));
				        vm.AnswerEndDate = new Date(vm.AnswerEnd.setHours(vm.AnswerEnd.getHours() + 0));
				    }
				    else if (objBrMajorVersion <= 57) {
				        vm.VendorStartEntryDate = new Date(vm.VendorStartDate.setHours(vm.VendorStartDate.getHours() + vm.timezoneClient));
				        vm.VendorEndEntryDate = new Date(vm.VendorEndDate.setHours(vm.VendorEndDate.getHours() + vm.timezoneClient));
				        vm.AnswerStartDate = new Date(vm.AnswerStart.setHours(vm.AnswerStart.getHours() + vm.timezoneClient));
				        vm.AnswerEndDate = new Date(vm.AnswerEnd.setHours(vm.AnswerEnd.getHours() + vm.timezoneClient));
				    }
				}
				else {
				    vm.VendorStartEntryDate = new Date(vm.VendorStartDate.setHours(vm.VendorStartDate.getHours() - 0));
				    vm.VendorEndEntryDate = new Date(vm.VendorEndDate.setHours(vm.VendorEndDate.getHours() - 0));
				    vm.AnswerStartDate = new Date(vm.AnswerStart.setHours(vm.AnswerStart.getHours() - 0));
				    vm.AnswerEndDate = new Date(vm.AnswerEnd.setHours(vm.AnswerEnd.getHours() - 0));
				}
				vm.frm = new form(vm.ID, vm.TenderStepID, vm.TenderID, vm.VendorStartEntryDate, vm.VendorEndEntryDate, vm.AnswerStartDate, vm.AnswerEndDate, item.data.AanwijzingType);
				console.info("frm aanwijzing:" + JSON.stringify(vm.frm));
			}
		}

		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		};

		vm.changeStartDateVendor = changeStartDateVendor;
		function changeStartDateVendor() {
			if (UIControlService.getStrDate(vm.frm.StartDateVendorEntry) < UIControlService.getStrDate(vm.StartDateStep)) {
				UIControlService.msg_growl("warning", "Tanggal Mulai Input Pertanyaan Vendor harus lebih besar dari Tanggal Mulai Tahapan Aanwijzing");
				vm.frm.StartDateVendorEntry = null;
				return;
			}
			if (UIControlService.getStrDate(vm.frm.StartDateVendorEntry) > UIControlService.getStrDate(vm.EndDateStep)) {
				UIControlService.msg_growl("warning", "Tanggal Mulai Input Pertanyaan Vendor harus lebih Kecil dari Tanggal Selesai Tahapan Aanwijzing");
				vm.frm.StartDateVendorEntry = null;
				return;
			}
		}

		vm.changeEndDateVendor = changeEndDateVendor;
		function changeEndDateVendor() {
			if (UIControlService.getStrDate(vm.frm.EndDateVendorEntry) < UIControlService.getStrDate(vm.StartDateStep)) {
				UIControlService.msg_growl("warning", "Tanggal Selesai Input Pertanyaan Vendor harus lebih besar dari Tanggal Mulai Tahapan Aanwijzing");
				vm.frm.EndDateVendorEntry = null;
				return;
			}
			if (UIControlService.getStrDate(vm.frm.EndDateVendorEntry) > UIControlService.getStrDate(vm.EndDateStep)) {
				UIControlService.msg_growl("warning", "Tanggal Selesai Input Pertanyaan Vendor harus lebih Kecil dari Tanggal Selesai Tahapan Aanwijzing");
				vm.frm.EndDateVendorEntry = null;
				return;
			}
			console.info("endDatevendor:" + vm.frm.EndDateVendorEntry);
		}

		vm.changeEntryDateAnswer = changeEntryDateAnswer;
		function changeEntryDateAnswer() {
			console.info("answerStartDate:" + vm.frm.AnswerStartDate);
			if (vm.frm.EndDateVendorEntry === null) {
				UIControlService.msg_growl("warning", "Tanggal Selesai Input Pertanyaan Vendor harus diisi terlebih dahulu");
				vm.frm.AnswerStartDate = null;
				return;
			}
			if (UIControlService.getStrDate(vm.frm.AnswerStartDate) < UIControlService.getStrDate(vm.frm.EndDateVendorEntry)) {
				UIControlService.msg_growl("warning", "Tanggal input jawaban peserta harus setelah tanggal selesai input pertanyaan vendor");
				vm.enddateHours = vm.frm.EndDateVendorEntry.getHours();
				vm.minHours = vm.enddateHours + 1;
				console.info("minHours" + vm.minHours);
				vm.frm.AnswerStartDate = vm.frm.EndDateVendorEntry;
				vm.frm.AnswerStartDate.setHours(vm.minHours);
				vm.frm.AnswerStartDate.setMinutes(0);
				return;
			}
		}

		vm.changeEntryTimeAnswer = changeEntryTimeAnswer;
		function changeEntryTimeAnswer() {
            /*waktu answerDate (inputan)*/
		    console.info("datetime answerDate>" + vm.frm.AnswerStartDate);
		    vm.answerHours = vm.frm.AnswerStartDate.getHours();
		    console.info("time answerDate>" + vm.answerHours);

		    /*waktu endDate vendor entry*/
		    vm.minnHours = vm.minHours-1;
		    console.info("endDate>" + vm.frm.EndDateVendorEntry);
		    console.info("minHours endDate>" + vm.minnHours);
		    if (UIControlService.getStrDate(vm.frm.AnswerStartDate) === UIControlService.getStrDate(vm.frm.EndDateVendorEntry)) {
		        if (vm.answerHours <= vm.minnHours) {
		            vm.frm.AnswerStartDate = vm.frm.EndDateVendorEntry;
		            vm.frm.AnswerStartDate.setHours(vm.minHours);
		            vm.frm.AnswerStartDate.setMinutes(0);
		        }
		    }
		}

		//function loadTypeAanwijzing() {
		//	AanwijzingService.getTypeAanwijzing(function (reply) {
		//		UIControlService.unloadLoading();
		//		if (reply.status === 200) {
		//			vm.listAanwijzingType = reply.data.List;
		//			if (vm.IsAtur === true) {
		//				//console.info(vm.dataForm.CommodityID + "..."+JSON.stringify(vm.listComodity));
		//				for (var i = 0; i < vm.listAanwijzingType.length; i++) {
		//					if (vm.frm.AanwijzingType === vm.listAanwijzingType[i].RefID) {
		//						vm.selectedTypeAwj = vm.listAanwijzingType[i];
		//						break;
		//					}
		//				}
		//			}
		//		}
		//	}, function (err) {
		//		UIControlService.msg_growl("error", "MESSAGE.API");
		//		UIControlService.unloadLoading();
		//	});
		//}


		vm.selectedTypeAwj;
		vm.listAanwijzingType = [];
		function loadTypeAanwijzing() {
			AanwijzingService.getTypeAanwijzing(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.listAanwijzingType = reply.data.List;
					if (vm.IsAtur === true) {
						//console.info(vm.dataForm.CommodityID + "..."+JSON.stringify(vm.listComodity));
						for (var i = 0; i < vm.listAanwijzingType.length; i++) {
							if (vm.frm.AanwijzingType === vm.listAanwijzingType[i].RefID) {
								vm.selectedTypeAwj = vm.listAanwijzingType[i];
								break;
							}
						}
					}
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		};
		vm.save = save;
		function save() {
		    var action = true;
			console.info("startDate" + JSON.stringify(vm.frm.StartDateVendorEntry));
		    //vm.frm.StartDateVendorEntry = new Date(vm.frm.StartDateVendorEntry.setHours(vm.stepItem.StartDate.getHours() + 8));
			if (vm.selectedTypeAwj === undefined) {
				UIControlService.msg_growl("warning", "Tipe Aanwijzing belum dipilih!!");
				return;
			}
			vm.frm.AanwijzingType = vm.selectedTypeAwj.RefID;
			console.info(JSON.stringify(vm.frm));
			if (UIControlService.getStrDate(vm.frm.EndDateVendorEntry) === UIControlService.getStrDate(vm.frm.AnswerStartDate)) {
			    console.info("tgl sama");
			    vm.jamEndDate = vm.frm.EndDateVendorEntry.getHours();
			    vm.jamAnswerStart = vm.frm.AnswerStartDate.getHours();
			    vm.selisih = vm.jamAnswerStart - vm.jamEndDate;
			    if (vm.selisih < 1) {
			        action = false;
			    }
			}
			console.info("action" + action);
			if (action === true) {
			    if (vm.IsAtur === false) {
			        console.info("tambah");
			        console.info("frm:" + JSON.stringify(vm.frm));
			        AanwijzingService.createAanwijzing(vm.frm, function (reply) {
			            UIControlService.unloadLoading();
			            //console.info("prq:: " + JSON.stringify(reply));
			            if (reply.status === 200) {
			                UIControlService.msg_growl("success", "Berhasil Simpan Data");
			                $uibModalInstance.close();
			            } else {
			                UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
			                return;
			            }
			        }, function (err) {
			            UIControlService.msg_growl("error", "MESSAGE.ERR_API");
			            UIControlService.unloadLoadingModal();
			        });
			    } else { //ini sementara saja, biar g error waktu edit --Luvia
			        //console.info("isFirefox?" + vm.isFirefox);
			       // if (vm.isFirefox === true) {
			            vm.frm.StartDateVendorEntry = new Date(vm.frm.StartDateVendorEntry.setHours(vm.frm.StartDateVendorEntry.getHours() + 7));
			           vm.frm.EndDateVendorEntry = new Date(vm.frm.EndDateVendorEntry.setHours(vm.frm.EndDateVendorEntry.getHours() + 7));
			           vm.frm.AnswerStartDate = new Date(vm.frm.AnswerStartDate.setHours(vm.frm.AnswerStartDate.getHours() + 7));
			            vm.frm.AnswerEndDate = new Date(vm.frm.AnswerEndDate.setHours(vm.frm.AnswerEndDate.getHours() + 7));
			       // }
			        console.info("frm:" + JSON.stringify(vm.frm));
                    
			        AanwijzingService.updateAanwijzing(vm.frm, function (reply) {
			            UIControlService.unloadLoading();
			            //console.info("prq:: " + JSON.stringify(reply));
			            if (reply.status === 200) {
			                UIControlService.msg_growl("success", "Berhasil Simpan Data");
			                $uibModalInstance.close();
			            } else {
			                UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
			                return;
			            }
			        }, function (err) {
			            UIControlService.msg_growl("error", "MESSAGE.ERR_API");
			            UIControlService.unloadLoadingModal();
			        });
			    }
			}
			else if (action===false){
			    UIControlService.msg_growl("warning", "Batas minimum input jawaban panitia harus satu jam setelah vendor mengajukan pertanyaan");
			}
		}

		/*
                        vm.batal = batal;
                        function batal() {
                            $uibModalInstance.dismiss('cancel');
                        };*/

		function form(ID, TenderStepID, TenderID, StartDateVendorEntry, EndDateVendorEntry, AnswerStartDate, AnswerEndDate, AanwijzingType) {
			var self = this;
			self.ID = ID;
			self.TenderStepID = TenderStepID;
			self.TenderID = TenderID;
			self.StartDateVendorEntry = StartDateVendorEntry;
			self.EndDateVendorEntry = EndDateVendorEntry;
			self.AnswerStartDate = AnswerStartDate;
			self.AnswerEndDate = AnswerEndDate;
			self.AanwijzingType = AanwijzingType;
		}
	}
})();