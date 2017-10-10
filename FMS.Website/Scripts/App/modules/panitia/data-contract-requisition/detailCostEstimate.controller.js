(function () {
    'use strict';

    angular.module("app")
    .controller("detailCostEstimateCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'ExcelReaderService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, ExcelReaderService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.isTenderVerification = false;
        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.DETAIL_COST_ESTIMATE", href: "" }
        ];

        vm.count = 0;
        vm.pageNumber = 1;
        vm.pageSize = 10;
        vm.searchText = "";
        vm.ceLines = [];

        vm.isViewingXL = false;
        vm.ceLinesFromXL = [];

        vm.fileUpload;

        vm.onSearchSubmit = function (searchText) {
            vm.searchText = searchText;
            vm.pageNumber = 1;
            vm.loadData();
        };

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');

            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.IsRequestor({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        loadCRInfo();
                        vm.loadData();
                    } else {
                        UIControlService.msg_growl("warning", $filter('translate')('MESSAGE.ERR_NOT_REQUESTOR'));
                        $state.transitionTo('data-contract-requisition');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_REQUESTOR'));
                    $state.transitionTo('data-contract-requisition');
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                $state.transitionTo('data-contract-requisition');
            });
        };

        function loadCRInfo() {
            DataContractRequisitionService.SelectById({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.currencySymbol = reply.data.CurrencySymbol;
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.isTenderVerification = reply.data.StatusName !== 'CR_DRAFT' && reply.data.StatusName.lastIndexOf('CR_REJECT_', 0) !== 0;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        vm.loadData = loadData;
        function loadData() {
            if (vm.isViewingXL === false) {
                UIControlService.loadLoading(loadmsg);
                DataContractRequisitionService.SelectCELine({
                    Parameter: contractRequisitionId,
                    Keyword: vm.searchText,
                    Limit: vm.pageSize,
                    Offset: (vm.pageNumber - 1) * vm.pageSize,
                    column: 1,
                }, function (reply) {
                    if (reply.status === 200) {
                        vm.ceLines = reply.data.List;
                        vm.count = reply.data.Count;
                    } else {
                        UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_CELINE'));
                    }
                    UIControlService.unloadLoading();
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                });
            } else {
                vm.ceLines = [];
                var index = -1;
                var offset = vm.pageSize * (vm.pageNumber - 1);
                var keyword = vm.searchText.toLowerCase();
                var pageItemCount = 0;

                //vm.count = 0;
                for (var i = 0; i < vm.ceLinesFromXL.length; i++) {
                    if (vm.ceLinesFromXL[i].Name.toLowerCase().indexOf(keyword) >= 0) {
                        index += 1;
                        if (index >= offset && pageItemCount < vm.pageSize) {
                            vm.ceLines.push(vm.ceLinesFromXL[i]);
                            pageItemCount += 1;
                        }
                        //vm.count++;
                    }
                };
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
            UIControlService.loadLoading(loadmsg);
            ExcelReaderService.readExcel(file,
                function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        vm.isViewingXL = true;
                        vm.ceLinesFromXL = [];
                        var excelContents = reply.data;
                        var sheet1 = excelContents[Object.keys(excelContents)[0]];
                        //var sheet2 = excelContents[Object.keys(excelContents)[1]];
                        //var sheet3 = dst...
                        var currentSubID = 1;
                        var currentSubName = vm.ProjectTitle;
                        for (var i = 1; i < sheet1.length; i++) {
                            var line = {
                                ContractRequisitionId: contractRequisitionId,
                                Name: sheet1[i].Column2,
                                Quantity: Number(sheet1[i].Column3),
                                OrderUnit: sheet1[i].Column4,
                                UnitCost: Number(sheet1[i].Column5),
                                LineCost: Number(sheet1[i].Column6),
                                //Tax: Number(sheet1[i].Column7)
                                Tax: 0
                            };
                            if (line.Name) {
                                if (!line.Quantity && !line.OrderUnit && !line.UnitCost && !line.LineCost) {
                                    currentSubID += 1;
                                    currentSubName = line.Name;
                                } else {
                                    line.SubID = currentSubID;
                                    line.SubName = currentSubName;
                                    vm.ceLinesFromXL.push(line);
                                }
                            }
                        };

                        vm.count = vm.ceLinesFromXL.length;
                        vm.pageNumber = 1;
                        vm.loadData();
                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                    }
                },
                function (error) {
                    UIControlService.msg_growl("error", "MESSAGE.MESSAGE.ERR_API")
                    UIControlService.unloadLoading();
                }
            );
        }

        vm.simpan = simpan;
        function simpan() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SaveCELines(vm.ceLinesFromXL, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_CE_LINE'));
                    vm.isViewingXL = false;
                    vm.loadData();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_CE_LINE'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_CE_LINE'));
            });
        };

        vm.back = back;
        function back() {
            $state.transitionTo('detail-contract-requisition', { contractRequisitionId: contractRequisitionId });
        };

        vm.subCost = subCost;
        function subCost() {
            $state.transitionTo('atur-subcost-estimate', { contractRequisitionId: contractRequisitionId });
        };
    }
})();