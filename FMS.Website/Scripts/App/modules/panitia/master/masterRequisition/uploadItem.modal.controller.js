(function () {
    'use strict';
    angular.module("app")
    .controller("uploadItemController", ctrl);
    ctrl.$inject = ['$state', 'item', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'ExcelReaderService', 'RFQVHSService'];
    /* @ngInject */
    function ctrl($state, item, $scope, $http, $filter, $stateParams, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, UIControlService, ExcelReaderService, RFQVHSService) {

        var vm = this;
        
        vm.count = 0;
        vm.pageNumber = 1;
        vm.pageSize = 10;
        vm.searchText = "";
        vm.RFQVHSItems = [];

        vm.fileUpload;
        vm.allowEdit = item.allowEdit;
        vm.includeItems = item.includeItems;

        vm.onSearchSubmit = onSearchSubmit;
        function onSearchSubmit(searchText) {
            vm.searchText = searchText;
            vm.pageNumber = 1;
            vm.loadData();
        };

        vm.init = init;
        function init() {
            if (!item.rfqvhsId){
                vm.includeItems = true;
            }
            if (item.items.length > 0 && vm.includeItems) {
                vm.RFQVHSItems = item.items;
            }
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            vm.pagedItems = [];
            var index = -1;
            var offset = vm.pageSize * (vm.pageNumber - 1);
            var keyword = vm.searchText.toLowerCase();
            var pageItemCount = 0;
            
            if (vm.includeItems) {
                vm.count = 0;
                for (var i = 0; i < vm.RFQVHSItems.length; i++) {
                    var POLongText = String(vm.RFQVHSItems[i].POLongText);
                    if (POLongText != null && POLongText.toLowerCase().indexOf(keyword) >= 0) {
                        index += 1;
                        if (index >= offset && pageItemCount < vm.pageSize) {
                            vm.pagedItems.push(vm.RFQVHSItems[i]);
                            pageItemCount += 1;
                        }
                        vm.count++;
                    }
                };
            } else {
                RFQVHSService.getRFQVHSItems({
                    Parameter: item.rfqvhsId,
                    Offset: offset,
                    Limit: vm.pageSize,
                    Keyword: keyword,
                    Column: 1
                }, function (reply) {
                    if (reply.status === 200) {
                        vm.pagedItems = reply.data.List;
                        vm.count = reply.data.Count;
                        UIControlService.unloadLoading();
                    } else {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", 'NOTIFICATION.GETRFQ.ERROR', "NOTIFICATION.GETRFQ.TITLE");
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", 'NOTIFICATION.GETRFQ.ERROR', "NOTIFICATION.GETRFQ.TITLE");
                });
            }
        };

        vm.selectUpload = selectUpload;
        function selectUpload(fileUpload) {
            vm.fileUpload = fileUpload;
        }

        vm.uploadFile = uploadFile;
        function uploadFile() {
            if (validateFileType(vm.fileUpload)) {
                upload(vm.fileUpload);
            }
        }

        function validateFileType(file) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }
            /*
            var selectedFileType = file[0].type;
            var allowed = false;
            
            if (selectedFileType === "application/vnd.ms-excel") {
                allowed = true;
            }
            else if (selectedFileType === "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                allowed = true;
            }

            if (!allowed) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_INVALID_FILETYPE");
                return false;
            }
            */

            return true;
        }

        function upload(file) {
            UIControlService.loadLoadingModal("LOADING.UPLOADING");
            ExcelReaderService.readExcel(file,
                function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        vm.RFQVHSItems = [];
                        var excelContents = reply.data;
                        var sheet1 = excelContents[Object.keys(excelContents)[0]];
                        //var sheet2 = excelContents[Object.keys(excelContents)[1]];
                        //var sheet3 = dst...
                        var firstCurreny = "";
                        for (var i=1; i < sheet1.length; i++){
                            var item = {
                                Material: sheet1[i].Column2,
                                MaterialDescription: sheet1[i].Column3,
                                UnitOfMeasure: sheet1[i].Column4,
                                MaterialType: sheet1[i].Column5,
                                MaterialGroup: sheet1[i].Column6,
                                CreationDateString: sheet1[i].Column7,
                                Currency: sheet1[i].Column8,
                                ManufacturerName: sheet1[i].Column9,
                                PartNumber: sheet1[i].Column10,
                                PurchasingGroupName: sheet1[i].Column11,
                                POLongText: sheet1[i].Column12,
                                VendorName: sheet1[i].Column13,
                                CountryName: sheet1[i].Column14,
                                City: sheet1[i].Column15,
                                AnnualUsage: Number(sheet1[i].Column16) > 0 ? Number(sheet1[i].Column16) : 0
                            };
                            //validasi
                            if (!item.Material) {
                                UIControlService.msg_growl("error", "Item baris " + (i + 1) + " tidak valid:");
                                UIControlService.msg_growl("error", "Kode Material harus diisi");
                                vm.RFQVHSItems = [];
                                break;
                            }
                            if (!item.UnitOfMeasure) {
                                UIControlService.msg_growl("error", "Item baris " + (i + 1) + " tidak valid:");
                                UIControlService.msg_growl("error", "Unit of Measure harus diisi");
                                vm.RFQVHSItems = [];
                                break;
                            }
                            if (!item.Currency) {
                                UIControlService.msg_growl("error", "Item baris " + (i + 1) + " tidak valid:");
                                UIControlService.msg_growl("error", "Currency harus diisi");
                                vm.RFQVHSItems = [];
                                break;
                            }
                            if (!item.POLongText) {
                                UIControlService.msg_growl("error", "Item baris " + (i + 1) + " tidak valid:");
                                UIControlService.msg_growl("error", "PO Long Text harus diisi");
                                vm.RFQVHSItems = [];
                                break;
                            }
                            if (!item.AnnualUsage) {
                                UIControlService.msg_growl("error", "Item baris " + (i + 1) + " tidak valid:");
                                UIControlService.msg_growl("error", "Annual Usage harus lebih dari 0");
                                vm.RFQVHSItems = [];
                                break;
                            }
                            if (i == 1) {
                                firstCurreny = item.Currency;
                            } else {
                                if (firstCurreny !== item.Currency) {
                                    UIControlService.msg_growl("error", "Currency harus seragam");
                                    vm.RFQVHSItems = [];
                                    break;
                                }
                            }
                            vm.RFQVHSItems.push(item);
                        };
                        vm.count = vm.RFQVHSItems.length;
                        vm.pageNumber = 1;
                        vm.includeItems = true;
                        vm.loadData();
                    } else {
                        UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR");
                    }
                },
                function (error) {
                    UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR")
                    UIControlService.unloadLoadingModal();
                }
            );
        }

        vm.simpan = simpan;
        function simpan() {
            $uibModalInstance.close(vm.RFQVHSItems);
        };

        vm.back = back;
        function back() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();