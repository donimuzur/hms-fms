(function () {
	'use strict';

	angular.module("app").controller("ContractSignOffController", ctrl);

	ctrl.$inject = ['$uibModal','$filter', 'UploadFileConfigService', 'UIControlService', '$translatePartialLoader', 'ContractSignOffService', 'UploaderService', '$stateParams', 'GlobalConstantService'];

	function ctrl($uibModal, $filter, UploadFileConfigService, UIControlService, $translatePartialLoader, ContractSignOffService, UploaderService, $stateParams, GlobalConstantService) {
		var vm = this;

		vm.IDTender = Number($stateParams.TenderRefID);
		vm.IDStepTender = Number($stateParams.StepID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.isStartDateOpened = false;
		vm.isEndDateOpened = false;
		vm.filePath;
		vm.flagEmp = 0;

		vm.init = init;
		function init() {
		    vm.flagTemp = 0;
			loadTypeSizeFile();
			UIControlService.loadLoading("Silahkan Tunggu");
			ContractSignOffService.getSignOff({
				TenderStepID: vm.IDStepTender
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    if (reply.data.ApprovalStatusReff == null) { vm.Status = 'Draft'; vm.flagTemp = 0; }
				    else if (reply.data.ApprovalStatusReff != null) {
				        cekEmployee(reply.data.ID);
				        vm.Status = reply.data.ApprovalStatusReff.Value;
				        vm.flagTemp = 1;
				    }
					vm.vendName = reply.data.VendorName;
					vm.ContractNo = reply.data.ContractNo;
					vm.ContractStartDate = new Date(Date.parse(reply.data.ContractStartDate));
					vm.ContractEndDate = new Date(Date.parse(reply.data.ContractEndDate));
					vm.UploadURL = reply.data.UploadURL;
					vm.Summary = reply.data.Summary;
					vm.ID = reply.data.ID;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.openStartDate = openStartDate;
		function openStartDate() {
			if (vm.isStartDateOpened) vm.isStartDateOpened = false;
			else vm.isStartDateOpened = true;
		}

		vm.openEndDate = openEndDate;
		function openEndDate() {
			if (vm.isEndDateOpened) vm.isEndDateOpened = false;
			else vm.isEndDateOpened = true;
		}

		vm.selectUpload = selectUpload;
		function selectUpload() {
			console.info(vm.filePath);
			//	//var test = vm.pathUpload;
		}

		function validateFileType(file, allowedFileTypes) {
			//console.info(JSON.stringify(allowedFileTypes));
			var selectedFileType = typefile(file, allowedFileTypes);
			var allowed = false;
			for (var i = 0; i < allowedFileTypes.length; i++) {
				if (allowedFileTypes[i].Name == selectedFileType) {
					allowed = true;
					return allowed;
				}
			}

			if (!allowed) {
				UIControlService.msg_growl("warning", "MESSAGE.ERR_INVALID_FILETYPE");
				return false;
			}
		}

		function typefile(file, allowedFileTypes) {
			if (!file || file.length == 0) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return false;
			}

			var selectedFileType = file[0].type;
			selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
			//console.info("tipefile: " + selectedFileType);
			if (selectedFileType === "application/pdf") {
				selectedFileType = "pdf";
			} else if (selectedFileType === "application/msword") {
				selectedFileType = "doc";
			} else if (selectedFileType === "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || selectedFileType == "vnd.openxmlformats-officedocument.wordprocessingml.document") {
				selectedFileType = "docx";
			} else {
				selectedFileType = selectedFileType;
			}
			return selectedFileType;
			//console.info("file:" + selectedFileType);
		}

		vm.uploadFile = uploadFile;
		function uploadFile() {
		    console.info(vm.filePath);
		    console.info(vm.UploadURL);
			var folder = "SIGNOFF_" + vm.IDStepTender;
			
			if (vm.filePath === undefined) { //null
			    if (vm.UploadURL != null) {
			        saveSignOff();
			    }
			   else if (vm.UploadURL == null) {
			       UIControlService.msg_growl("error", "File tidak boleh kosong");
			       return;
			    }
			}
			else if (vm.filePath !== undefined) {
			    var tipefileupload = typefile(vm.filePath, vm.idUploadConfigs);
			    if (validateFileType(vm.filePath, vm.idUploadConfigs)) {
			        upload(vm.filePath, vm.idFileSize, vm.idFileTypes, folder, tipefileupload);
			    }
			}
		}

		function upload(file, config, filters, folder, tipefile, callback) {
			var size = config.Size;
			var unit = config.SizeUnitName;
			if (unit == 'SIZE_UNIT_KB') {
				size *= 1024;
			} else if (unit == 'SIZE_UNIT_MB') {
				size *= (1024 * 1024);
			}

			//console.info(file[0].size + ":" + file[0].type);
			var tipesize_file = tipefile + " / " + Math.floor(file[0].size / 1024) + " KB";
			console.info(tipesize_file);

			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
			UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_ADMIN", size, filters, folder,
		        function (response) {
		        	UIControlService.unloadLoading();
		        	if (response.status == 200) {
		        		var url = response.data.Url;
		        		UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
		        		vm.UploadURL = url;
		        		saveSignOff();
		        	} else {
		        		UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
		        		return;
		        	}
		        },
		        function (response) {
		        	UIControlService.msg_growl("error", "MESSAGE.API")
		        	UIControlService.unloadLoading();
		        });

		}

		function loadTypeSizeFile() {
			UIControlService.loadLoadingModal("MESSAGE.LOADING");
			//get tipe dan max.size file - 1
			UploadFileConfigService.getByPageName("PAGE.ADMIN.TENDER.CONTRACTSIGNOFF", function (response) {
				UIControlService.unloadLoadingModal();
				if (response.status == 200) {
					vm.idUploadConfigs = response.data;
					vm.idFileTypes = UIControlService.generateFilterStrings(response.data);
					vm.idFileSize = vm.idUploadConfigs[0];
				} else {
					UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoadingModal();
				return;
			});
		}

		//vm.saveSignOff = saveSignOff;
		function saveSignOff() {
			UIControlService.loadLoading("Silahkan Tunggu");
			ContractSignOffService.save({
				ContractEndDate: vm.ContractEndDate,
				ContractNo: vm.ContractNo,
				ContractStartDate: vm.ContractStartDate,
				Summary: vm.Summary,
				UploadURL: vm.UploadURL,
				TenderStepID: vm.IDStepTender,
				ID: vm.ID
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("success", 'NOTIFICATION.SAVE.SUCCESS.MESSAGE', "NOTIFICATION.SAVE.SUCCESS.TITLE");
					window.location.reload();
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.sendToApprove = sendToApprove;
		function sendToApprove() {
		    bootbox.confirm($filter('translate')('Apakah anda ingin mengirim data approval ini untuk proses persetujuan?'), function (yes) {
		        if (yes) {
		            UIControlService.loadLoading("MESSAGE.LOADING");
		            var dt = {
		                ID: vm.ID,
		                TenderStepID: vm.IDStepTender,
		                flagEmp: 1
		            };
		            ContractSignOffService.SendApproval(dt, function (reply) {
		                if (reply.status === 200) {
		                    UIControlService.unloadLoading();
		                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SEND_TO_APPRV'));
		                    init();
		                } else {
		                    UIControlService.unloadLoading();
		                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
		                }
		            }, function (error) {
		                UIControlService.unloadLoading();
		                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
		            });
		        }
		    });
		}

		vm.cekEmployee = cekEmployee;
		function cekEmployee(Id, reff){
		    ContractSignOffService.CekEmployee({
		        ID: Id
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.flagEmp = reply.data;
		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.API");
		        UIControlService.unloadLoading();
		    });
		}

		vm.detailApproval = detailApproval;
		function detailApproval(dt) {
		    var item = {
		        ID: vm.ID,
		        flag: vm.flagEmp,
                Status: dt
		    };
		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/panitia/contractSignOff/detailApproval.modal.html',
		        controller: 'detailApprovalSignOffCtrl',
		        controllerAs: 'detailApprovalSignOffCtrl',
		        resolve: { item: function () { return item; } }
		    });
		    modalInstance.result.then(function () {
		        init();
		    });
		};
	}
})();