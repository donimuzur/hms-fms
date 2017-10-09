(function () {
    'use strict';

    angular.module("app")
    .controller("FormEvaluator", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService','UploaderService','UploadFileConfigService',
        'EvaluationTechnicalService', '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,UploaderService, UploadFileConfigService, EvaluationTechnicalService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item) {
        var vm = this;
        vm.list = [];
        vm.list = item.item;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.isCalendarOpened = [false];
        vm.flag = item.act;
        vm.init = init;
        vm.eval = [];
        vm.com = [];
        function init() {
            vm.StepID = item.tenderStepData.ID;
            vm.TenderRefID = item.tenderStepData.tender.TenderRefID;
            vm.ProcPackType = item.tenderStepData.tender.ProcPackageType;
            //vm.EvaluatorDate = item.tenderStepData.EvaluationDate;
            vm.DocumentDate = item.tenderStepData.DocumentDate;
            vm.DocumentUrl = item.tenderStepData.DocumentUrl;
            vm.Summary = item.tenderStepData.Summary;
            //loadVendor();
            UIControlService.loadLoadingModal("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.EVALUATION.TECHNICAL", function (response) {
                UIControlService.unloadLoadingModal();
                if (response.status == 200) {
                    vm.name = response.data.name;
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];

                } else {
                    UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_TYPEFILE");
                UIControlService.unloadLoadingModal();
            });
            if (vm.list && vm.list.length > 0) {
               
            }
            convertToDate();
        }

        vm.calculateSummaryScore = calculateSummaryScore;
        function calculateSummaryScore() {
            //console.info("curr "+current)
            UIControlService.loadLoadingModal("MESSAGE.LOADING");
            EvaluationTechnicalService.calculateSummaryScore({
                ID: vm.StepID
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.list = reply.data;
                } else {
                    UIControlService.msg_growl("error", "Gagal mendapat skor evaluasi");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal mendapat skor evaluasi");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
            //$uibModalInstance.close();
        };

        vm.viewMustHave = viewMustHave;
        function viewMustHave(data) {
            var data = {
                VendorName: data.VendorName,
                VendorID: data.VendorID,
                TenderStepDataID: vm.StepID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/evaluasi-teknis/viewBelowMustHave.modal.html',
                controller: 'viewBelowMustHaveModal',
                controllerAs: 'mustHaveCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

        vm.save = save;
        function save(url) {
            /*
            if (vm.detail.length != vm.list.length) {
                alert("Ada rekanan yang belum dipilih");
                return;
            }
            */
            //convertAllDateToString();
            var tenderStepData = {
                ID: vm.StepID,
                Summary: vm.Summary,
                DocumentDate: UIControlService.getStrDate(vm.DocumentDate),
                DocumentUrl: url
            }
            //console.info(JSON.stringify(vm.eval));
            EvaluationTechnicalService.saveSummaryScore({
                TenderStepData: tenderStepData,
                EvaluationTechnicalScoreSummaries: vm.list
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Simpan Data!!");
                    $uibModalInstance.close();
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.add = add;
        function add(data) {
            vm.flag1 = data;
        }
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        /*
        function convertAllDateToString() { // TIMEZONE (-)
            if (vm.UploadDate) {
                vm.UploadDate = UIControlService.getStrDate(vm.UploadDate);
            }
        };
        */

        /*
        vm.tambah = tambah;
        function tambah() {
            console.info("ss");
            if (vm.Rating > vm.evaltechnical.length) {
                alert("Peringkat melebihi maksimal jumlah rekanan");
                return;
            }
            
            for (var i = 0; i < vm.evaltechnical.length; i++) {
                if (vm.Rating == vm.evaltechnical[i].Ratings) {
                    alert("Peringkat telah dipilih rekanan lain");
                    return;
                }
            }
            for (var i = 0; i < vm.evaltechnical.length; i++) {
                if (vm.evaltechnical[i].VendorID === vm.selectedTypeVendor.VendorID) {
                    var data = {
                        ID: vm.evaltechnical[i].ID,
                        VendorID: vm.selectedTypeVendor.VendorID,
                        VendorName: vm.selectedTypeVendor.VendorName,
                        Ratings: vm.Rating,
                        Score: vm.score,
                        Status: vm.selectedType,
                        TenderStepDataID: vm.evaltechnical[i].TenderStepDataID
                    };
                    vm.list.push(data);
                }
            }
            //console.info(JSON.stringify(vm.list));
            
        }
        */

        /*
        vm.deleteRow = deleteRow;
        function deleteRow(index) {
            var idx = index - 1;
            var _length = vm.list.length; // panjangSemula
            vm.list.splice(idx, 1);
        };
        */

        //Supaya muncul di date picker saat awal load
        function convertToDate() {
            if (!vm.DocumentDate) {
                vm.DocumentDate = new Date();
            }
            vm.DocumentDate = new Date(Date.parse(vm.DocumentDate));
        }

        //get tipe dan max.size file - 2
        function generateFilterStrings(allowedTypes) {
            //console.info(allowedTypes);
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.selectUpload = selectUpload;
        //vm.fileUpload;
        function selectUpload() {
            //console.info((vm.fileUpload));
        }
        /*start upload */
        vm.uploadFile = uploadFile;
        function uploadFile() {
            //console.info((vm.fileUpload));
            if (!vm.fileUpload) {
                save(vm.DocumentUrl);
            }
            else {
                if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                    upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, "");
                }
            }
        }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            /*
            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);

            if (selectedFileType === "vnd.ms-excel") {
                selectedFileType = "xls";
            }
            else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                selectedFileType = "xlsx";
            }
            else {
                selectedFileType = selectedFileType;
            }
            vm.type = selectedFileType;
            console.info("filenew:" + selectedFileType);
            //jika excel
            if (selectedFileType === "vnd.ms-excel")
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
            */
            return true;
        }

        vm.upload = upload;
        function upload(file, config, filters, callback) {

            var size = config.Size;
            var unit = config.SizeUnitName;

            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
                vm.flag = 0;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
                vm.flag = 1;
            }

            UIControlService.loadLoadingModal("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFileLibrary(file, size, filters,
                function (response) {
                    UIControlService.unloadLoadingModal();
                    //console.info("response:" + JSON.stringify(response));
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;
                        if (vm.flag == 0) {
                            vm.size = Math.floor(s)
                        }
                        if (vm.flag == 1) {
                            vm.size = Math.floor(s / (1024));
                        }
                        save(url);
                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                        return;
                    }
                },
                function (response) {
                    UIControlService.msg_growl("error", "MESSAGE.API")
                    UIControlService.unloadLoadingModal();
                }
            );
        }
    }
})();;