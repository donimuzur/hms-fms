(function () {
    'use strict';

    angular.module("app")
    .controller("detailEvaluasiPenawaranVHSController", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'EvaluasiPenawaranVHSService', 'DataPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, EvaluasiPenawaranVHSService, DataPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.stepID = Number($stateParams.StepID);
        vm.tenderRefID = Number($stateParams.TenderRefID);
        vm.procPackType = Number($stateParams.ProcPackType);

        vm.tenderStepData = {};
        vm.itemPRs = [];
        vm.offerEntries = [];
        vm.evaluation = [];
        vm.evaluationVendors = [];
        vm.scoreDetails = [];
        vm.repeater = [];
        vm.rfqvhs = {};
        
        vm.keyword = "";
        vm.column = 1;
        vm.pageNumber = 1;
        vm.pageSize = 10;
        vm.count = 0;

        vm.isProcess = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('evaluasi-penawaran-barang');
            loadData();
        };
        
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            EvaluasiPenawaranVHSService.getrfqvhs({
                ID: vm.stepID
            }, function (reply) {
                vm.rfqvhs = reply.data;
                vm.isFPA = (vm.rfqvhs.RFQType === 2);
                DataPengadaanService.GetStepByID({
                    ID: vm.stepID
                }, function (reply) {
                    vm.tenderStepData = reply.data;
                    vm.isProcess = vm.tenderStepData.StatusName === "PROCUREMENT_TYPE_PROCESS";
                        EvaluasiPenawaranVHSService.getOfferEntries({
                            ID: vm.tenderStepData.TenderID
                        }, function (reply) {
                            vm.offerEntries = reply.data;
                            vm.offerEntries.forEach(function (entry) {
                                if (!entry.ExchangeRateToUSD) {
                                    entry.ExchangeRateToUSD = 1;
                                };
                                entry.OfferTotalCostInUSD = entry.OfferTotalCost * entry.ExchangeRateToUSD;
                                entry.OfferTotalCostInUSD = entry.OfferTotalCostInUSD.toFixed(2);
                                /*
                                entry.VHSOfferEntryDetails[0].detail.forEach(function (det) {
                                    det.TotalPriceInUSD = det.TotalPrice * entry.ExchangeRateToUSD;
                                    det.TotalPriceInUSD = det.TotalPriceInUSD.toFixed(2);
                                });
                                */
                                //entry.FreightCostPercent = entry.FreightDeliveryCost;
                            });
                            calculateAllTotalPurchaseValue();
                            var length = vm.offerEntries.length;
                            vm.repeater = [];
                            for (var i = 0; i < length * 3; i++) {
                                vm.repeater.push(i);
                            }
                            UIControlService.unloadLoading();
                            loadPagedOEDetail();
                            loadEvaluationData();
                        }, function (error) {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_OFFERS');
                        });
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
                });
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };

        vm.loadPagedOEDetail = loadPagedOEDetail;
        function loadPagedOEDetail() {
            EvaluasiPenawaranVHSService.getPagedItemPRs({
                Parameter: vm.tenderStepData.TenderID,
                Keyword: vm.keyword,
                column: vm.column,
                Offset: (vm.pageNumber - 1) * vm.pageSize,
                Limit: vm.pageSize
            }, function (reply) {
                vm.itemPRs = reply.data.List;
                vm.count = reply.data.Count;
                vm.offerEntries.forEach(function (entry) {
                    EvaluasiPenawaranVHSService.getPagedOEDetail({
                        Parameter: entry.ID,
                        Keyword: vm.keyword,
                        column: vm.column,
                        Offset: (vm.pageNumber - 1) * vm.pageSize,
                        Limit: vm.pageSize
                    }, function (reply) {
                        var oeDetail = reply.data;
                        entry.VHSOfferEntryDetails[0].detail = oeDetail;
                        entry.VHSOfferEntryDetails[0].detail.forEach(function (det) {
                            det.TotalPriceInUSD = det.TotalPrice * entry.ExchangeRateToUSD;
                            det.TotalPriceInUSD = det.TotalPriceInUSD.toFixed(2);
                        });
                    }, function (error) {
                        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_OFFERS');
                    });
                });
            }, function (error) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_ITEM_PRS');
            });
        }

        function loadEvaluationData() {
            UIControlService.loadLoading(loadmsg);
            EvaluasiPenawaranVHSService.getByTenderStepData({
                ID: vm.stepID,
                TenderID: vm.tenderStepData.TenderID
            }, function (reply) {
                vm.evaluation = reply.data;
                vm.evaluationVendors = vm.evaluation.VHSOfferEvaluationVendors;
                if (vm.evaluation.ID > 0) { //Jika sudah pernah dilakukan evaluasi
                    EvaluasiPenawaranVHSService.getCriterias({
                        ID: vm.evaluation.ID
                    }, function (reply) {
                        vm.criterias = reply.data;
                        sumCriteriaWeights();
                        vm.costChangeAll();
                        setFreightDeliveryCostAndTimes();
                        calculateLeadTimeScores();
                        calculatePriceScores();
                        vm.evaluationVendors.forEach(function (ev) {
                            var scoreDetIndex = 0;
                            ev.VHSOEvaluationVendorScoreDetails.forEach(function (scoreDet) {
                                var weight = vm.criterias[scoreDetIndex++].Weight;
                                scoreDet.CalculatedScore = scoreDet.Score * weight / 100;
                                scoreDet.CalculatedScore = scoreDet.CalculatedScore.toFixed(2);
                            })
                        })
                        calculateTotalScores();
                        UIControlService.unloadLoading();
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_CRITERIA');
                    });
                } else { //Jika belum
                    vm.evaluation.TenderStepDataID = vm.stepID;
                    vm.evaluationVendors = [];
                    EvaluasiPenawaranVHSService.getCriteriasFromEMDC({
                        ID: vm.tenderStepData.TenderID
                    }, function (reply) {
                        vm.criterias = reply.data;
                        sumCriteriaWeights();
                        var vendorIndex = 0;
                        EvaluasiPenawaranVHSService.getVerifiedDocScore(vm.offerEntries, function (reply) {
                            var verifiedDocDetails = reply.data;
                            vm.offerEntries.forEach(function (entry) {
                                var evaluationVendor = {
                                    VHSOfferEntryID: entry.ID,
                                    Score: 0,
                                    VHSOEvaluationVendorScoreDetails: []
                                };
                                vm.evaluationVendors.push(evaluationVendor);
                                var scoreDetIndex = 0;
                                vm.criterias.forEach(function (criteria) {
                                    var scoreDet = {
                                        CriteriaID: criteria.CriteriaID,
                                        Score: 0,
                                        SelectedECOptionID: null,
                                        SelectedECOptionName: null,
                                    };
                                    verifiedDocDetails.forEach(function (docDet) {
                                        if (docDet.CriteriaId == criteria.CriteriaID && docDet.VHSOfferEntry.VHSOEid == entry.ID) {
                                            scoreDet.Score = docDet.Score;
                                            scoreDet.SelectedECOptionID = docDet.SelectedECOptionID;
                                        }
                                    });
                                    evaluationVendor.VHSOEvaluationVendorScoreDetails.push(scoreDet);
                                    optionChange(vendorIndex, scoreDetIndex);
                                    scoreDetIndex++;
                                });
                                vendorIndex++;
                            });
                            setFreightDeliveryCostAndTimes();
                            calculateLeadTimeScores();
                            calculatePriceScores();
                            calculateTotalScores();
                            UIControlService.unloadLoading();
                        }, function (error) {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_VERIFICATION_SCORE');
                        });
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_CRITERIA');
                    });
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        }

        vm.costChangeAll = costChangeAll;
        function costChangeAll() {
            calculateAllTotalPurchaseValue();
            calculatePriceScores();
            calculateTotalScores();
        }

        vm.costChange = costChange;
        function costChange(index) {
            calculateTotalPurchaseValue(index);
            calculatePriceScores();
            calculateTotalScores();
        }

        function calculateAllTotalPurchaseValue() {
            for (var i = 0; i < vm.offerEntries.length;i++) {
                calculateTotalPurchaseValue(i);
            }
        }

        function calculateTotalPurchaseValue(index) {
            vm.offerEntries[index].TotalPurchaseValue = vm.offerEntries[index].OfferTotalCost;

            if (vm.evaluation.FreightCostUnitName === 'COST_UNIT_PERCENT') {
                vm.offerEntries[index].FreightCostPercentValue = vm.offerEntries[index].FreightCostPercent ?
                    vm.offerEntries[index].OfferTotalCost * vm.offerEntries[index].FreightCostPercent / 100 : 0;
                vm.offerEntries[index].TotalPurchaseValue += vm.offerEntries[index].FreightCostPercentValue;

                vm.offerEntries[index].FreightCostPercentValueInUSD = vm.offerEntries[index].FreightCostPercentValue * vm.offerEntries[index].ExchangeRateToUSD;
                vm.offerEntries[index].FreightCostPercentValueInUSD = vm.offerEntries[index].FreightCostPercentValueInUSD.toFixed(2);

            } else if (vm.evaluation.FreightCostUnitName === 'COST_UNIT_CURRENCY'){
                vm.offerEntries[index].TotalPurchaseValue += Number(vm.offerEntries[index].FreightCost);

                vm.offerEntries[index].FreightCostInUSD = Number(vm.offerEntries[index].FreightCost) * vm.offerEntries[index].ExchangeRateToUSD;
                vm.offerEntries[index].FreightCostInUSD = vm.offerEntries[index].FreightCostInUSD.toFixed(2);
            }

            vm.offerEntries[index].CustomsDutyCostPercentValue = vm.offerEntries[index].CustomsDutyCostPercent ?
                    vm.offerEntries[index].OfferTotalCost * vm.offerEntries[index].CustomsDutyCostPercent / 100 : 0;
            vm.offerEntries[index].TotalPurchaseValue += vm.offerEntries[index].CustomsDutyCostPercentValue;

            vm.offerEntries[index].CustomsDutyCostPercentValueInUSD = vm.offerEntries[index].CustomsDutyCostPercentValue * vm.offerEntries[index].ExchangeRateToUSD;
            vm.offerEntries[index].CustomsDutyCostPercentValueInUSD = vm.offerEntries[index].CustomsDutyCostPercentValueInUSD.toFixed(2);

            if (vm.evaluation.OtherCostUnitName === 'COST_UNIT_PERCENT') {
                vm.offerEntries[index].OtherCostPercentValue = vm.offerEntries[index].OtherCostPercent ?
                    vm.offerEntries[index].OfferTotalCost * vm.offerEntries[index].OtherCostPercent / 100 : 0;
                vm.offerEntries[index].TotalPurchaseValue += vm.offerEntries[index].OtherCostPercentValue;

                vm.offerEntries[index].OtherCostPercentValueInUSD = vm.offerEntries[index].OtherCostPercentValue * vm.offerEntries[index].ExchangeRateToUSD;
                vm.offerEntries[index].OtherCostPercentValueInUSD = vm.offerEntries[index].OtherCostPercentValueInUSD.toFixed(2);
            } else if (vm.evaluation.OtherCostUnitName === 'COST_UNIT_CURRENCY') {
                vm.offerEntries[index].TotalPurchaseValue += Number(vm.offerEntries[index].OtherCost);

                vm.offerEntries[index].OtherCostInUSD = Number(vm.offerEntries[index].OtherCost) * vm.offerEntries[index].ExchangeRateToUSD;
                vm.offerEntries[index].OtherCostInUSD = vm.offerEntries[index].OtherCostInUSD.toFixed(2);
            }

            vm.offerEntries[index].TotalPurchaseValueInUSD = vm.offerEntries[index].TotalPurchaseValue * vm.offerEntries[index].ExchangeRateToUSD;
            vm.offerEntries[index].TotalPurchaseValueInUSD = vm.offerEntries[index].TotalPurchaseValueInUSD.toFixed(2);
        }

        vm.optionChange = optionChange;
        function optionChange(vendorIndex, scoreDetIndex) {
            var scoreDet = vm.evaluationVendors[vendorIndex].VHSOEvaluationVendorScoreDetails[scoreDetIndex];
            var optionID = scoreDet.SelectedECOptionID;
            var optionData = getOptionById(vm.criterias[scoreDetIndex].CriteriaOptions, optionID);
            scoreDet.MaxScore = optionData.MaxScore;
            scoreDet.MinScore = optionData.MinScore;

            //Langsung masukkan nilai skor Max apabila opsi kriteria berupa nilai fix, bukan range
            if (vm.criterias[scoreDetIndex].IsOptionScoreFixed && optionID > 0) {
                scoreDet.Score = scoreDet.MaxScore;
            };

            //Memastikan skor berada dalam range yang benar
            scoreChange(vendorIndex, scoreDetIndex);
        };

        vm.scoreChange = scoreChange;
        function scoreChange(vendorIndex, scoreDetIndex) {
            var scoreDet = vm.evaluationVendors[vendorIndex].VHSOEvaluationVendorScoreDetails[scoreDetIndex];
            //Memastikan skor berada dalam range yang benar
            if (scoreDet.Score > scoreDet.MaxScore){
                scoreDet.Score = scoreDet.MaxScore
            }
            if (scoreDet.Score < scoreDet.MinScore){
                scoreDet.Score = scoreDet.MinScore
            }
            //Kalkulasi skor + bobot
            var weight = vm.criterias[scoreDetIndex].Weight;
            scoreDet.CalculatedScore = scoreDet.Score * weight / 100;
            scoreDet.CalculatedScore = scoreDet.CalculatedScore.toFixed(2);
            calculateTotalScores();
        };

        function getOptionById(options, id) { //untuk mengambil besar min/max score
            if (options && id) {
                for (var i = 0; i < options.length; i++) {
                    if (options[i].ID === id) {
                        return options[i];
                    }
                }
            }
            else {
                return {
                    ID: null,
                    MinScore: 0,
                    MaxScore: 100
                };
            }
        };

        vm.freightTypeChange = freightTypeChange;
        function freightTypeChange() {
            setFreightDeliveryCostAndTimes();
            calculateAllTotalPurchaseValue();
            calculateLeadTimeScores();
            calculatePriceScores();
            calculateTotalScores();
        }

        function setFreightDeliveryCostAndTimes() {
            vm.offerEntries.forEach(function (entry) {
                switch (vm.evaluation.FreightTypeName) {
                    case "Air":
                        entry.FreightDeliveryTime = entry.FreightDeliveryTimeAir;
                        entry.FreightCostPercent = entry.FreightDeliveryCostAir;
                        break;
                    case "Land":
                        entry.FreightDeliveryTime = entry.FreightDeliveryTimeLand;
                        entry.FreightCostPercent = entry.FreightDeliveryCostLand;
                        break;
                    case "Sea":
                        entry.FreightDeliveryTime = entry.FreightDeliveryTimeSea;
                        entry.FreightCostPercent = entry.FreightDeliveryCostSea;
                        break;
                    default:
                        entry.FreightDeliveryTime = 0;
                        entry.FreightCostPercent = 0;
                        break;
                }
            });
        }

        function calculateLeadTimeScores() {
            var minLeadTime;
            vm.offerEntries.forEach(function (entry) {
                var totalLeadTime = entry.SupplierQLTime + entry.FreightDeliveryTime;
                if (minLeadTime === undefined || totalLeadTime < minLeadTime) {
                    minLeadTime = totalLeadTime;
                }
            });
            var vendorIndex = 0;
            vm.evaluationVendors.forEach(function (ev) {
                var offerEntry = vm.offerEntries[vendorIndex++];
                var totalLeadTime = offerEntry.SupplierQLTime + offerEntry.FreightDeliveryTime;
                ev.VHSOEvaluationVendorScoreDetails[1].Score = totalLeadTime > 0 ? minLeadTime * 100 / totalLeadTime : 100;
                var leadTimeWeight = vm.criterias[1].Weight;
                ev.VHSOEvaluationVendorScoreDetails[1].CalculatedScore =
                    ev.VHSOEvaluationVendorScoreDetails[1].Score * leadTimeWeight / 100;
                ev.VHSOEvaluationVendorScoreDetails[1].Score =
                    ev.VHSOEvaluationVendorScoreDetails[1].Score.toFixed(2);
                ev.VHSOEvaluationVendorScoreDetails[1].CalculatedScore =
                    ev.VHSOEvaluationVendorScoreDetails[1].CalculatedScore.toFixed(2);
            });
        };
        

        function calculatePriceScores() {
            var minPrice;
            vm.offerEntries.forEach(function (entry) {
                var totalPrice = Number(entry.TotalPurchaseValueInUSD);
                if (minPrice === undefined || totalPrice < minPrice) {
                    minPrice = totalPrice;
                }
            });
            var vendorIndex = 0;
            vm.evaluationVendors.forEach(function (ev) {
                var offerEntry = vm.offerEntries[vendorIndex++];
                var totalPrice = Number(offerEntry.TotalPurchaseValueInUSD);
                ev.VHSOEvaluationVendorScoreDetails[0].Score = minPrice * 100 / totalPrice;
                var priceWeight = vm.criterias[0].Weight;
                ev.VHSOEvaluationVendorScoreDetails[0].CalculatedScore =
                    ev.VHSOEvaluationVendorScoreDetails[0].Score * priceWeight / 100;
                ev.VHSOEvaluationVendorScoreDetails[0].Score =
                    ev.VHSOEvaluationVendorScoreDetails[0].Score.toFixed(2);
                ev.VHSOEvaluationVendorScoreDetails[0].CalculatedScore =
                    ev.VHSOEvaluationVendorScoreDetails[0].CalculatedScore.toFixed(2);
            });
        };

        function calculateTotalScores() {
            vm.evaluationVendors.forEach(function (ev) {
                ev.Score = 0;
                ev.VHSOEvaluationVendorScoreDetails.forEach(function (det) {
                    ev.Score += Number(det.CalculatedScore);
                });
                ev.Score = ev.Score.toFixed(2);
            });
        };

        function sumCriteriaWeights() {
            vm.totalWeight = 0;
            vm.criterias.forEach(function (crit) {
                vm.totalWeight += crit.Weight;
            })
        };

        vm.totalOfferTooltip = totalOfferTooltip;
        function totalOfferTooltip(offerEntryIndex, costType, typeIndex) {
            var vendorName = vm.offerEntries[offerEntryIndex].VendorName;
            var currency = vm.offerEntries[offerEntryIndex].CurrencySymbol;
            var typeText = "";
            switch (typeIndex) {
                case 0: return "";
                case 1: typeText = "(" + currency + ")"; break;
                case 2: typeText = "(USD)"; break;
            }
            return "Vendor : " + vendorName + "\n" + costType + " " + typeText;
        }

        vm.leadTimeTooltip = leadTimeTooltip;
        function leadTimeTooltip(offerEntryIndex, leadTimeType, typeIndex) {
            if (typeIndex !== 0) {
                return "";
            }
            var vendorName = vm.offerEntries[offerEntryIndex].VendorName;
            return "Vendor : " + vendorName + "\n" + leadTimeType;
        }

        vm.criteriaEvaluationTooltip = criteriaEvaluationTooltip;
        function criteriaEvaluationTooltip(offerEntryIndex, criteriaIndex, typeIndex) {
            var vendorName = vm.offerEntries[offerEntryIndex].VendorName;
            var criteriaName = "";
            switch(criteriaIndex){
                case 0: criteriaName = "Price"; break;
                case 1: criteriaName = "Leadtime"; break;
                default: criteriaName = vm.criterias[criteriaIndex].CriteriaName; break;
            }
            var typeText = "";
            switch (typeIndex) {
                case 1: typeText = "(Score)"; break;
                case 2: typeText = "(Weighted Result)"; break;
            }
            return "Vendor : " + vendorName + "\n" + criteriaName + " " + typeText;
        }

        vm.sortByScore = sortByScore;
        function sortByScore() {

            //Urutkan evaluation vendor
            vm.evaluationVendors.sort(function (a, b) {
                return b.Score - a.Score;
            });

            //Urutkan offer entry berdasarkan urutan evaluation vendor
            var orderedOfferEntries = [];
            vm.evaluationVendors.forEach(function (ev) {
                var offerEntryId = ev.VHSOfferEntryID;
                for (var i = 0; i < vm.offerEntries.length; i++) {
                    if (vm.offerEntries[i].ID === offerEntryId) {
                        orderedOfferEntries.push(vm.offerEntries[i]);
                        vm.offerEntries.splice(i, 0);
                        break;
                    }
                }
            });
            vm.offerEntries = orderedOfferEntries;
        };

        vm.save = save;
        function save() {
            vm.evaluation.FreightCostUnitRef = {
                Name: vm.evaluation.FreightCostUnitName
            };
            vm.evaluation.OtherCostUnitRef = {
                Name: vm.evaluation.OtherCostUnitName
            };
            vm.evaluation.FreightTypeRef = {
                Name: vm.evaluation.FreightTypeName
            };
            vm.evaluationVendors.forEach(function (ev) {
                ev.VHSOEvaluationVendorScoreDetails.forEach(function (det) {
                    det.VHSOfferEvaluationCriteria = {
                        CriteriaID: det.CriteriaID
                    };
                });
            })
            vm.evaluation.VHSOfferEvaluationCriterias = vm.criterias;
            vm.evaluation.VHSOfferEvaluationVendors = vm.evaluationVendors;

            UIControlService.loadLoading(loadmsg);
            EvaluasiPenawaranVHSService.saveScoring({
                VHSOfferEntries: vm.offerEntries,
                VHSOfferEvaluation: vm.evaluation
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", 'MESSAGE.SUCC_SAVE_SCORE');
                    //loadData();
                    $state.transitionTo("evaluasi-penawaran-vhs", { TenderRefID: vm.tenderRefID, StepID: vm.stepID, ProcPackType: vm.procPackType });
                } else {
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE_SCORE');
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE_SCORE');
            });
        }
    }
})();