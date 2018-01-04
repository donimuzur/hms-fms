(function () {
    'use strict';

    angular.module("app")
    .controller("formContractVarCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        var contractRequisitionVariationId = Number($stateParams.contractRequisitionVariationId);
        var action = $stateParams.action;

        var loadmsg = "";

        vm.contractRecVar = {};
        vm.isCalendarOpened = [false, false, false, false];
        vm.isTenderVerification = false;
        vm.contractReqVar = {};
        vm.reasonRefs = [];
        vm.contractValueVariationPercent = 0;
        vm.contractDateVariationPercent = 0;

        vm.breadcrumbs = []
        vm.breadcrumbs.push({ title: "BREADCRUMB.MASTER_REQUISITION", href: "" });
        vm.breadcrumbs.push({ title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" });
        if (contractRequisitionVariationId > 0) {
            vm.breadcrumbs.push({ title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/detail-contract-requisition/" + contractRequisitionId });
        }
        vm.breadcrumbs.push({ title: "BREADCRUMB.DATA_CONTRACT_VARIATION", href: "" });

        vm.dateRequestedOptions = {
            minDate: new Date()
        };

        vm.dateRequiredOptions = {
            minDate: new Date()
        };

        vm.planEndDateOptions = {
            minDate: new Date()
        };

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('contract-variation');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });

            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.IsRequestor({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
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
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_REQUESTOR'));
                $state.transitionTo('data-contract-requisition');
            });
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.GetCRVariationReasons(function (reply) {
                UIControlService.unloadLoading();
                vm.reasonRefs = reply.data;
                if (contractRequisitionVariationId > 0) {
                    loadDataForEdit();
                } else {
                    loadDataForCreate();
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_REASONS'));
            });
        };

        function loadDataForEdit() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.GetVariationByID({
                ID: contractRequisitionVariationId
            }, function (reply) {                
                vm.contractRecVar = reply.data;
                vm.contractRecVar.IsAnnualPlan = vm.contractRecVar.IsAnnualPlan ? "1" : "0";
                vm.contractRecVar.OriginalStartDate = new Date(Date.parse(vm.contractRecVar.OriginalStartDate));
                vm.contractRecVar.OriginalEndDate = new Date(Date.parse(vm.contractRecVar.OriginalEndDate));
                convertToDate();
                if (vm.contractRecVar.OriginalStartDate > new Date()) {
                    vm.planEndDateOptions.minDate = vm.contractRecVar.OriginalStartDate;
                }
                vm.isTenderVerification = vm.contractRecVar.StatusName !== 'CR_DRAFT' && vm.contractRecVar.StatusName.lastIndexOf('CR_REJECT_', 0) !== 0;
                if (vm.contractRecVar.ReasonRefs && vm.contractRecVar.ReasonRefs.length > 0) {
                    for (var i = 0; i < vm.reasonRefs.length; i++) {
                        vm.reasonRefs[i].isChecked = false;
                        for (var j = 0; j < vm.contractRecVar.ReasonRefs.length; j++) {
                            if (vm.reasonRefs[i].RefID === vm.contractRecVar.ReasonRefs[j].RefID) {
                                vm.reasonRefs[i].isChecked = true;
                                if (vm.reasonRefs[i].Name == 'CV_REASON_EXTENSION') {
                                    vm.enableDate = true;
                                } else if (vm.reasonRefs[i].Name == 'CV_REASON_RATE') {
                                    vm.enableValue = true;
                                }
                                break;
                            }
                        }
                    }
                }
                if (vm.contractRecVar.CurrencySymbol === 'IDR') {
                    vm.contractRecVar.OriginalValueInUSD = vm.contractRecVar.OriginalValue * vm.contractRecVar.RateIDRToUSD;
                    vm.contractRecVar.ContractCommitmentTotalValueInUSD = vm.contractRecVar.ContractCommitmentTotalValue * vm.contractRecVar.RateIDRToUSD;
                }
                calculateVariationPercent();
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
            });
        }

        function loadDataForCreate() {
            vm.DateRequested = new Date();
            vm.DateRequired = new Date();
            vm.contractRecVar.ContractRequisitionOriginId = contractRequisitionId;
            vm.contractRecVar.IsAnnualPlan = "0";
            vm.contractRecVar.ContractCommitmentTotalValue = 0;
            vm.contractRecVar.FurtherCommitmentTotalValue = 0;
            vm.contractRecVar.ReasonRefs = [];
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.GetCurrentVariantNumber({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                vm.contractRecVar.VariationNo = reply.data;
                DataContractRequisitionService.GetLastVariation({
                    ContractRequisitionId: contractRequisitionId
                }, function (reply) {
                    var lastVariation = reply.data;
                    if (lastVariation !== null) {
                        vm.CurrentEndDate = new Date(Date.parse(lastVariation.NewPlannedEndDate));
                        vm.contractRecVar.ContractCommitmentTotalValue = lastVariation.NewTotalValue;
                    }
                    DataContractRequisitionService.SelectById2({
                        ContractRequisitionId: contractRequisitionId
                    }, function (reply) {
                        var contractRec = reply.data;
                        vm.contractRecVar.ProjectTitle = contractRec.ProjectTitle;
                        vm.contractRecVar.OriginalValue = contractRec.TotalValue;
                        vm.contractRecVar.CurrencySymbol = contractRec.CurrencySymbol;
                        vm.contractRecVar.RateIDRToUSD = contractRec.RateIDRToUSD;
                        if (lastVariation === null) {
                            vm.contractRecVar.ContractCommitmentTotalValue = contractRec.TotalValue;
                        }
                        DataContractRequisitionService.GetContractByCRID({
                            ContractRequisitionId: contractRequisitionId
                        }, function (reply) {
                            UIControlService.unloadLoading();
                            var constractSO = reply.data;
                            vm.contractRecVar.ContractSignOffId = constractSO.ID;
                            vm.contractRecVar.OriginalStartDate = new Date(Date.parse(constractSO.ContractStartDate));
                            vm.contractRecVar.OriginalEndDate = new Date(Date.parse(constractSO.ContractEndDate));
                            if (lastVariation === null) {
                                vm.CurrentEndDate = new Date(Date.parse(constractSO.ContractEndDate));
                            }
                            vm.NewPlannedEndDate = vm.CurrentEndDate;
                            if (vm.contractRecVar.OriginalStartDate > new Date()) {
                                vm.planEndDateOptions.minDate = vm.contractRecVar.OriginalStartDate;
                            }
                            vm.contractRecVar.ContractNo = constractSO.ContractNo;
                            if (vm.contractRecVar.CurrencySymbol === 'IDR') {
                                vm.contractRecVar.OriginalValueInUSD = vm.contractRecVar.OriginalValue * vm.contractRecVar.RateIDRToUSD;
                                vm.contractRecVar.ContractCommitmentTotalValueInUSD = vm.contractRecVar.ContractCommitmentTotalValue * vm.contractRecVar.RateIDRToUSD;
                            }
                            calculateVariationPercent();
                        }, function (error) {
                            UIControlService.unloadLoading();
                            if (error == 'ERR_MULTIPLE_CONTRACT') {
                                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_MULTIPLE_CONTRACT'));
                                back();
                            }
                            else {
                                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_CR'));
                            }
                        });
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_CR'));
                    });
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_CR'));
                });
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_CR'));
            });
        };

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            if (vm.enableDate) {
                vm.isCalendarOpened[index] = true;
            }
        };

        vm.calculateVariationPercent = calculateVariationPercent;
        function calculateVariationPercent() {

            //Variasi nilai
            vm.contractRecVar.NewTotalValue =
                Number(vm.contractRecVar.ContractCommitmentTotalValue) + Number(vm.contractRecVar.FurtherCommitmentTotalValue);
            var originalValue = vm.contractRecVar.OriginalValue;
            var newValue = vm.contractRecVar.NewTotalValue;
            var percentageValue = originalValue > 0 ? (newValue * 100 / originalValue) - 100 : 0;
            vm.contractValueVariationPercent = Number(percentageValue.toFixed(2));
            if (vm.contractValueVariationPercent < 0) {
                vm.contractValueVariationPercent = 0;
            }

            if (vm.contractRecVar.CurrencySymbol === 'IDR') {
                vm.contractRecVar.FurtherCommitmentTotalValueInUSD = Number(vm.contractRecVar.FurtherCommitmentTotalValue) * vm.contractRecVar.RateIDRToUSD;
                vm.contractRecVar.NewTotalValueInUSD = vm.contractRecVar.NewTotalValue * vm.contractRecVar.RateIDRToUSD;
            }

            //variasi tanggal
            vm.contractDateVariationPercent = 0;
            if (vm.NewPlannedEndDate) {
                var originalDuration = (vm.contractRecVar.OriginalEndDate - vm.contractRecVar.OriginalStartDate);
                var newDuration = (vm.NewPlannedEndDate - vm.contractRecVar.OriginalStartDate);
                var percentageDuration = originalDuration > 0 ? (newDuration * 100 / originalDuration) - 100 : 0;
                vm.contractDateVariationPercent = Number(percentageDuration.toFixed(2));
            }
            if (vm.contractDateVariationPercent < 0) {
                vm.contractDateVariationPercent = 0;
            }

            //Ambil nilai terbesar
            vm.contractRecVar.VariationPercent = vm.contractValueVariationPercent > vm.contractDateVariationPercent ?
                vm.contractValueVariationPercent : vm.contractDateVariationPercent;
        };

        vm.onReasonCheck = onReasonCheck;
        function onReasonCheck(reasonRef) {
            switch (reasonRef.Name) {
                case 'CV_REASON_EXTENSION':
                    if (reasonRef.isChecked === true) {
                        vm.enableDate = true;
                    } else {
                        vm.NewPlannedEndDate = vm.CurrentEndDate;
                        vm.enableDate = false;
                    }
                    break;
                case 'CV_REASON_RATE':
                    if (reasonRef.isChecked === true) {
                        vm.enableValue = true;
                    } else {
                        vm.contractRecVar.FurtherCommitmentTotalValue = 0;
                        vm.contractRecVar.NewTotalValue = Number(vm.contractRecVar.ContractCommitmentTotalValue);
                        vm.enableValue = false;
                    }
                    break;
                default:
                    break;
            };
            vm.calculateVariationPercent();
        };

        vm.save = save;
        function save() {
            var checkField = checkRequiredField();
            if (!checkField) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_INCOMPLETE_FIELD'));
                return;
            }
            if (vm.DateRequired < vm.DateRequested) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_DATE_REQUIRED'));
                return;
            }

            vm.contractRecVar.IsAnnualPlan = vm.contractRecVar.IsAnnualPlan === "1";
            vm.contractRecVar.ReasonRefs = [];
            vm.reasonRefs.forEach(function (ref) {
                if (ref.isChecked === true) {
                    vm.contractRecVar.ReasonRefs.push({
                        RefID : ref.RefID
                    });
                }
            });
            convertAllDateToString();
            if (contractRequisitionVariationId > 0) {
                UIControlService.loadLoading(loadmsg);
                DataContractRequisitionService.EditVariant(vm.contractRecVar, function (reply) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE'));
                    vm.back();
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE'));
                });
            } else {
                DataContractRequisitionService.CreateVariant(vm.contractRecVar, function (reply) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE'));
                    vm.back();
                }, function (error) {
                    UIControlService.unloadLoading();
                    if (error.data === 'ERR_DUPLICATE_VARIATION_NO') {
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_DUPLICATE_VARIATION_NO'));
                        vm.contractRecVar.VariationNo += 1;
                    }
                    else {
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE'));
                    }
                });
            }
        };

        function checkRequiredField() {
            return vm.CurrentEndDate && vm.NewPlannedEndDate;
        }

        function convertAllDateToString() {
            if (vm.DateRequested) {
                vm.contractRecVar.DateRequested = UIControlService.getStrDate(vm.DateRequested);
            }
            if (vm.DateRequired) {
                vm.contractRecVar.DateRequired = UIControlService.getStrDate(vm.DateRequired);
            }
            if (vm.CurrentEndDate) {
                vm.contractRecVar.CurrentEndDate = UIControlService.getStrDate(vm.CurrentEndDate);
            }
            if (vm.NewPlannedEndDate) {
                vm.contractRecVar.NewPlannedEndDate = UIControlService.getStrDate(vm.NewPlannedEndDate);
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate(){
            if (vm.contractRecVar.DateRequested) {
                vm.DateRequested = new Date(Date.parse(vm.contractRecVar.DateRequested));
            }
            if (vm.contractRecVar.DateRequired) {
                vm.DateRequired = new Date(Date.parse(vm.contractRecVar.DateRequired));
            }
            if (vm.contractRecVar.CurrentEndDate) {
                vm.CurrentEndDate = new Date(Date.parse(vm.contractRecVar.CurrentEndDate));
            }
            if (vm.contractRecVar.NewPlannedEndDate) {
                vm.NewPlannedEndDate = new Date(Date.parse(vm.contractRecVar.NewPlannedEndDate));
            }
        }

        vm.back = back;
        function back() {
            if (contractRequisitionVariationId > 0) {
                $state.transitionTo('detail-contract-requisition', { contractRequisitionId: contractRequisitionId });
            } else {
                $state.transitionTo('data-contract-requisition');
            }
        };
    }
})();