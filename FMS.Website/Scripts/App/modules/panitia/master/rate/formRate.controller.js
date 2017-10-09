(function () {
    'use strict';

    angular.module("app")
            .controller("FormRateCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'RateService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'GlobalConstantService', 'ExcelReaderService', '$state'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, RateService, UploadFileConfigService,
        UIControlService, UploaderService, GlobalConstantService, ExcelReaderService, $state) {
        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api')+"/";
        vm.pathFile;
        vm.tglSekarang = UIControlService.getDateNow("");
        vm.message = "Silahkan Tunggu";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("master-rate");
            //console.info(vm.folderFile);
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.MASTER.RATE", function (response) {
                UIControlService.unloadLoading();
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
                UIControlService.unloadLoading();
                return;
            });
        }

        vm.selectUpload = selectUpload;
        //vm.fileUpload;
        function selectUpload() {
            console.info((vm.fileUpload));
        }
        /*start upload */
        vm.uploadFile = uploadFile;
        function uploadFile() {
            UIControlService.loadLoading(vm.message);
            ExcelReaderService.readExcel(vm.fileUpload,
                function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        var excelContents = reply.data;
                        var Sheet1 = excelContents[Object.keys(excelContents)[0]]; /*untuk baca nama sheet*/
                        var countproperty = numAttrs(Sheet1[0]);
                        vm.colFromCurr;
                        vm.colFromFact;
                        vm.colToCurr;
                        vm.colToFact;
                        vm.colValid;
                        vm.colExchRate;

                        for (var i = 1; i <= countproperty ; i++) {
                            if (Sheet1[0]['Column'+i] === 'FROM_CURR') {
                                vm.colFromCurr = i;
                            }
                            if (Sheet1[0]['Column' + i] === 'FROM_FACTOR') {
                                vm.colFromFact = i;
                            }
                            if (Sheet1[0]['Column' + i] === 'TO_CURRNCY') {
                                vm.colToCurr = i;
                            }
                            if (Sheet1[0]['Column' + i] === 'TO_FACTOR') {
                                vm.colToFact = i;
                            }
                            if (Sheet1[0]['Column' + i] === 'VALID_FROM') {
                                vm.colValid = i;
                            }
                            if (Sheet1[0]['Column' + i] === 'EXCH_RATE') {
                                vm.colExchRate = i;
                            }

                            console.info('*Column' + i + " " + vm.colFromCurr + vm.colFromFact);
                            if (!(vm.colFromCurr === undefined) && !(vm.colFromFact === undefined) &&
                                !(vm.colToCurr === undefined) && !(vm.colToFact === undefined) &&
                                !(vm.colValid === undefined) && !(vm.colExchRate === undefined)) {
                                break;
                            }
                        }
                        //cek jika ada kolom excel tidak sesuai format
                        if ((vm.colFromCurr === undefined) || (vm.colFromFact === undefined) ||
                                (vm.colToCurr === undefined) || (vm.colToFact === undefined) ||
                                (vm.colValid === undefined) || (vm.colExchRate === undefined)) {
                            UIControlService.msg_growl("warning", "MESSAGE.ERR_EXCEL");
                            return;
                        }
                        vm.newExcel = []
                        for (var a = 1; a < Sheet1.length; a++) {
                            var objExcel = {
                                FromCurr: Sheet1[a]['Column' + vm.colFromCurr],
                                FromFactor: Sheet1[a]['Column' + vm.colFromFact],
                                ToCurr: Sheet1[a]['Column' + vm.colToCurr],
                                ToFactor: Sheet1[a]['Column' + vm.colToFact],
                                ExchangeRate: Sheet1[a]['Column' + vm.colExchRate],
                                ValidFrom: UIControlService.convertDateEx(Sheet1[a]['Column' + vm.colValid])
                            }
                            vm.newExcel.push(objExcel)
                        }
                        //console.info(">head:"+JSON.stringify(vm.newExcel));
                    }
                });            
        }

        /*count of property object*/
        function numAttrs(obj) {
            var count = 0;
            for (var key in obj) {
                if (obj.hasOwnProperty(key)) {
                    ++count;
                }
            }
            return count;
        }

        vm.uploadSave = uploadSave;
        function uploadSave() {
            UIControlService.loadLoading(vm.message);
            RateService.Insert(vm.newExcel, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "FORM.MSG_SUC_SAVE");
                    if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                        upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.tglSekarang);
                    }
                    $state.transitionTo('master-rate');
                }
                else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });            
        }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
            //console.info("tipefile: " + selectedFileType);
            if (selectedFileType === "vnd.ms-excel") {
                selectedFileType = "xls";
            }
            else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                selectedFileType = "xlsx";
            }
            else {
                selectedFileType = selectedFileType;
            }
            //console.info("filenew:" + selectedFileType);
            //jika excel
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
        
        vm.upload = upload;
        function upload(file, config, filters, dates, callback) {
            //console.info(file);
            var size = config.Size;
            var unit = config.SizeUnitName;
            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
            }

            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFile(file,"UPLOAD_DIRECTORIES_RATE",size,filters, dates,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = vm.folderFile + url;
                        UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");

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
        
    }
})();