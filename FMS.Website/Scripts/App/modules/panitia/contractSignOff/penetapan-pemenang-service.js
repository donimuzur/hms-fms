(function () {
    'use strict';

    angular.module("app").controller("ContractSignOffController", ctrl);

    ctrl.$inject = ['$state','$uibModal', '$filter', 'UploadFileConfigService', 'UIControlService', '$translatePartialLoader', 'ContractSignOffService', 'UploaderService', '$stateParams', 'GlobalConstantService'];

    function ctrl($state, $uibModal, $filter, UploadFileConfigService, UIControlService, $translatePartialLoader, ContractSignOffService, UploaderService, $stateParams, GlobalConstantService) {
        var vm = this;

        vm.IDTender = Number($stateParams.TenderRefID);
        vm.IDStepTender = Number($stateParams.StepID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.isStartDateOpened = false;
        vm.isEndDateOpened = false;
        vm.filePath;
        vm.flagEmp = 0;
        vm.isCalendarOpened = [false, false, false, false];
        vm.isCalendarOpened1 = [false, false, false, false];

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };
        vm.openCalendar1 = openCalendar1;
        function openCalendar1(index) {
            vm.isCalendarOpened1[index] = true;
        };


        vm.init = init;
        function init() {
            loadLogin();
            loadTender();
            vm.flagTemp = 0;
            loadTypeSizeFile();
            UIControlService.loadLoading("Silahkan Tunggu");
            ContractSignOffService.getSignOff({
                TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detail = reply.data;
                    for (var i = 0; i < vm.detail.length; i++) {
                        if (vm.detail[i].ApprovalStatusReff == null) {
                            vm.detail[i].Status = 'Draft';
                            vm.detail[i].flagTemp = 0;
                        }
                        else if (vm.detail[i].ApprovalStatusReff != null) {
                            cekEmployee(vm.detail[i].ID, vm.detail[i]);
                            vm.detail[i].Status = vm.detail[i].ApprovalStatusReff.Value;
                            vm.detail[i].flagTemp = 1;
                        }
                        vm.detail[i].ContractStartDate = new Date(Date.parse(vm.detail[i].ContractStartDate));
                        vm.detail[i].ContractEndDate = new Date(Date.parse(vm.detail[i].ContractEndDate));
                    }
                    
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadTender = loadTender;
        function loadTender() {
            ContractSignOffService.Step({
                TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.step = reply.data;
                    console.info(vm.step);
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }


        vm.loadLogin = loadLogin;
        function loadLogin() {
            ContractSignOffService.getLogin({
                TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.login = reply.data;
                    console.info(vm.login);
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
        function uploadFile(data) {
            console.info(data.filePath);
            console.info(data.UploadURL);
            var folder = "SIGNOFF_" + vm.IDStepTender + data.VendorName;

            if (data.filePath === undefined) { //null
                if (data.UploadURL != null) {
                    saveSignOff(data);
                }
                else if (data.UploadURL == null) {
                    UIControlService.msg_growl("error", "File tidak boleh kosong");
                    return;
                }
            }
            else if (data.filePath !== undefined) {
                var tipefileupload = typefile(data.filePath, vm.idUploadConfigs);
                if (validateFileType(data.filePath, vm.idUploadConfigs)) {
                    upload(data, vm.idFileSize, vm.idFileTypes, folder, tipefileupload);
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
            var tipesize_file = tipefile + " / " + Math.floor(file.filePath[0].size / 1024) + " KB";
            console.info(tipesize_file);

            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFile(file.filePath, "UPLOAD_DIRECTORIES_ADMIN", size, filters, folder,
		        function (response) {
		            UIControlService.unloadLoading();
		            if (response.status == 200) {
		                var url = response.data.Url;
		                UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
		                file.UploadURL = url;
		                saveSignOff(file);
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
        function saveSignOff(data) {
            UIControlService.loadLoading("Silahkan Tunggu");
            ContractSignOffService.save({
                VendorID: data.VendorID,
                ContractEndDate: data.ContractEndDate,
                ContractNo: data.ContractNo,
                ContractStartDate: data.ContractStartDate,
                Summary: data.Summary,
                UploadURL: data.UploadURL,
                TenderStepID: vm.IDStepTender,
                ID: data.ID
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
        function sendToApprove(data) {
            bootbox.confirm($filter('translate')('Apakah anda ingin mengirim data approval ini untuk proses persetujuan?'), function (yes) {
                if (yes) {
                    UIControlService.loadLoading("MESSAGE.LOADING");
                    var dt = {
                        ID: data.ID,
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
        function cekEmployee(Id, reff) {
            ContractSignOffService.CekEmployee({
                ID: Id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    reff.flagEmp = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.detailApproval = detailApproval;
        function detailApproval(dt, data) {
            var item = {
                ID: data.ID,
                flag: data.flagEmp,
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

        vm.back = back;
        function back() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.step.tender.TenderRefID, ProcPackType: vm.step.tender.ProcPackageType });
        }
    }
})();