(function () {
    'use strict';

    angular.module("app")
    .controller("evaluasiHargaJasaModalController", ctrl);

    ctrl.$inject = ['$state', 'item', '$http', '$filter', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'EvaluasiHargaJasaService', 'DataPengadaanService', 'UIControlService', 'UploaderService', 'UploadFileConfigService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, item, $http, $filter, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, EvaluasiHargaJasaService, DataPengadaanService, UIControlService, UploaderService, UploadFileConfigService, GlobalConstantService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        var tenderRefID = item.tenderRefID;
        var procPackageType = item.procPackageType;
        var tenderID = item.tenderID;
        var evaluasiID = item.evaluasiID;
        vm.evaluationVendor = item.evaluationVendor;
        vm.tenderStepData = item.tenderStepData;
        vm.totalCostEstimate = 0;
        vm.offerLimit = 0;
        vm.fileUpload;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";

        vm.documentDate = item.tenderStepData.DocumentDate ? new Date(item.tenderStepData.DocumentDate) : new Date();
        vm.isCalendarOpen = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('evaluasi-harga-jasa');
            loadData();
            loadUploadFileConfig();
        };

        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            EvaluasiHargaJasaService.getTotalCostEstimate({
                TenderRefID: tenderRefID,
            }, function (reply) {
                vm.totalCostEstimate = reply.data;
                EvaluasiHargaJasaService.getOfferPriceLimit({
                    TenderRefID: tenderRefID,
                    ProcPackageType: procPackageType
                }, function (reply) {
                    vm.offerLimit = reply.data;
                    var limit = vm.totalCostEstimate * vm.offerLimit / 100;
                    vm.upperlimit = vm.totalCostEstimate + limit;
                    vm.lowerlimit = vm.totalCostEstimate - limit;
                    if (vm.evaluationVendor.length === 0) {
                        EvaluasiHargaJasaService.getOfferEntryVendor({
                            ID: tenderID
                        }, function (reply) {
                            UIControlService.unloadLoadingModal();
                            vm.evaluationVendor = reply.data;
                        }, function (error) {
                            UIControlService.unloadLoadingModal();
                            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
                        });
                    } else {
                        UIControlService.unloadLoadingModal();
                    }
                }, function (error) {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_LIMIT');
                });
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_TOTAL_CE');
            });
        };

        vm.generateSkor = generateSkor;
        function generateSkor() {
            var minCost = null;
            vm.evaluationVendor.forEach(function (ev) {
                if (ev.OfferTotalCost >= vm.lowerlimit && ev.OfferTotalCost <= vm.upperlimit) {
                    if (minCost === null || minCost > ev.OfferTotalCost) {
                        minCost = ev.OfferTotalCost;
                    }
                }
            });
            vm.evaluationVendor.forEach(function (ev) {
                ev.Score = 0;
                if (ev.OfferTotalCost >= vm.lowerlimit && ev.OfferTotalCost <= vm.upperlimit) {
                    ev.Score = minCost * 5 / ev.OfferTotalCost;
                }
            });
        }

        vm.openCalendar = openCalendar;
        function openCalendar() {
            vm.isCalendarOpen = true;
        }

        function loadUploadFileConfig() {
            UploadFileConfigService.getByPageName("PAGE.ADMIN.EVALUATION.SOPRICE", function (response) {
                if (response.status == 200) {
                    vm.uploadConfigs = response.data;
                    vm.fileTypes = generateFilterStrings(response.data);
                    vm.fileSize = vm.uploadConfigs[0];
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
            });
        }

        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.selectUpload = selectUpload;
        function selectUpload(file) {
            vm.fileUpload = file;
        }

        vm.simpan = simpan;
        function simpan() {
            if (vm.fileUpload){
                uploadFile();
            } else {
                saveEvaluation();
            }
        }

        function uploadFile() {
            if (validateFileType()) {
                upload();
            }
        }

        function validateFileType() {
            if (!vm.fileUpload || vm.fileUpload == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }
            return true;
        }

        function upload() {
            var size = vm.fileSize.Size;
            var unit = vm.fileSize.SizeUnitName;
            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
            }
            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
            }
            UIControlService.loadLoadingModal(loadmsg);
            UploaderService.uploadSingleFileSOEvaluation(tenderID, vm.fileUpload, size, vm.fileTypes,
            function (reply) {
                if (reply.status == 200) {
                    UIControlService.unloadLoadingModal();
                    var url = reply.data.Url;
                    vm.tenderStepData.DocumentUrl = url;
                    saveEvaluation();
                } else {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_UPLOAD');
                }
            }, function (err) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
            });
        };

        function saveEvaluation() {
            vm.tenderStepData.DocumentDate = UIControlService.getStrDate(vm.documentDate);
            var evaluasi = {
                ID: evaluasiID,
                ServiceOfferEntryEvaluationVendors: vm.evaluationVendor,
                TenderStepData: vm.tenderStepData
            }

            UIControlService.loadLoadingModal(loadmsg);
            EvaluasiHargaJasaService.saveEvaluation(evaluasi, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status == 200) {
                    UIControlService.msg_growl("notice", 'MESSAGE.SUCC_SAVE');
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE');
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE');
            });
        };

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();