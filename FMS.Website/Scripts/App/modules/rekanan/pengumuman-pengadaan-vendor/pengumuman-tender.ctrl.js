(function () {
    'use strict';

    angular.module("app")
    .controller("PengumumanTenderVendorCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PengumumanPengadaanService', 'PPVHSService', 'GoodOfferEntryService', 'PurchaseRequisitionService', '$state', 'UIControlService', '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PengumumanPengadaanService, PPVHSService, GoodOfferEntryService, PurchReqService,
        $state, UIControlService, $uibModal, $stateParams) {
        var vm = this;
        vm.TanggalHariIni = new Date();
        vm.srcText = '';
        vm.listPengumuman = [];
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.TenderID = Number($stateParams.TenderID);
       // vm.StepID = Number($stateParams.StepID);

        vm.init = init;
        function init() {
            console.info("tenderID" + vm.TenderID);
            loadSteps();
            loadDataTender();
            //loadDataTender();
            //$translatePartialLoader.addPart("pengumuman-pengadaaan-client");
            //loadDataOfferEntry();
        }


        function loadSteps() {
            PengumumanPengadaanService.GetSteps({
                ID: vm.TenderID
            }, function (reply) {
                console.info("step" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataSteps = reply.data;
                    vm.GoodsOrService = vm.dataSteps[0].step.GoodsOrService;
                    vm.TenderRefID = vm.dataSteps[0].tender.TenderRefID;
                    vm.ProcPackType = vm.dataSteps[0].tender.ProcPackageType;
                    for (var i = 1; i < vm.dataSteps.length; i++) {
                        if (vm.GoodsOrService === 1) {
                            vm.offerEntryType;
                            if (vm.dataSteps[i].step.FormTypeURL === 'pemasukan-penawaran-vhs') {
                                vm.offerEntryType = 'VHS';
                                vm.StepID = vm.dataSteps[i].ID;
                                i = vm.dataSteps.length;
                                GetRFQ();
                            }
                            else if (vm.dataSteps[i].step.FormTypeURL === 'pemasukan-penawaran-barang') {
                                vm.offerEntryType = 'GOODS';
                                vm.StepID = vm.dataSteps[i].ID;
                                i = vm.dataSteps.length;
                                loadGoodsOfferEntry();
                            }
                        }
                        else if (vm.GoodsOrService === 2) {
                            if (vm.dataSteps[i].step.FormTypeName === 'Pemasukan Penawaran Jasa') {
                                console.info("masuk");
                                vm.StepIDJasa = vm.dataSteps[i].ID;
                                console.info("stepIDjasa" + vm.StepIDJasa);
                                i = vm.dataSteps.length;
                                loadServiceOfferEntry();
                            }
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadDataTender() {
            PengumumanPengadaanService.getTenderReg({
                ID: vm.TenderID
            }, function (reply) {
                console.info("data tend" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataTender = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //barang
        vm.loadGoodsOfferEntry = loadGoodsOfferEntry;
        function loadGoodsOfferEntry() {
            var tender = {
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            GoodOfferEntryService.select(tender, function (reply) {
                console.info("goodsOffEntry:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.goods = reply.data;
                    for (var i = 0; i < vm.goods.length; i++) {
                        if (i == vm.goods.length - 1) {

                            vm.offer = vm.goods[i].goods.OfferTotalCost;
                            vm.Date = vm.goods[i].goods.CurrencyDate;
                        }
                    }
                    if (vm.goods[0].ID !== 0) {
                        loadOtherDoc(vm.goods[0].GoodsOEId);
                        vm.QLTime = vm.goods[0].goods.SupplierQLTime;
                        loadFreightCost2(vm.goods[0].goods.FreighCostId);
                    }
                    else {
                        loadFreightCost2();
                    }

                    loadCurrencies();
                    loadDeliveryTerms();
                    loadFreightCost();
                    loadRFQItem();
                    loadRFQGoods();
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Penawaran Barang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.changePrice = changePrice;
        function changePrice(data) {
            data.TotalPrice = data.UnitPrice * data.item.Quantity;
        }
        vm.selectedIncoTerms;
        vm.listIncoTerms = [];
        function loadIncoTerms() {
            PurchReqService.getIncoTerms(function (reply) {
                UIControlService.unloadLoading();
                vm.listIncoTerms = reply.data;
                for (var i = 0; i < vm.listIncoTerms.length; i++) {
                    if (vm.goods[0].goods.IncoId === vm.listIncoTerms[i].ID) {
                        vm.selectedIncoTerms = vm.listIncoTerms[i];
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.selectedState;
        vm.listState = [];
        function loadStateDelivery() {
            PurchReqService.getStateDelivery(function (reply) {
                UIControlService.unloadLoading();
                vm.listState = reply.data;
                for (var i = 0; i < vm.listState.length; i++) {
                    if (vm.goods[0].goods.StateID === vm.listState[i].StateID) {
                        vm.selectedState = vm.listState[i];
                        loadCityDelivery(vm.selectedState);
                        //changeState();
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.selectedFreightCost2;
        vm.listFreightCost2 = [];
        function loadFreightCost2(data) {
            GoodOfferEntryService.GetFreightCost(function (reply) {
                UIControlService.unloadLoading();
                vm.listFreightCost2 = reply.data;
                if (data !== undefined) {
                    for (var i = 0; i < vm.listFreightCost2.length; i++) {
                        if (vm.listFreightCost2[i].FreightCostID == data) {
                            vm.selectedFreightCost2 = vm.listFreightCost2[i];
                        }
                    }
                }

            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.changeState = changeState;
        function changeState(data) {
            loadCityDelivery(data.StateID);
        }

        //list City
        vm.selectedCity;
        vm.listCity = [];
        function loadCityDelivery(data) {
            PurchReqService.getCityDelivery({ StateID: data.StateID }, function (reply) {
                UIControlService.unloadLoading();
                //console.info("city:" + JSON.stringify(reply));
                vm.listCity = reply.data;
                for (var i = 0; i < vm.listCity.length; i++) {
                    if (vm.goods[0].goods.CityID === vm.listCity[i].CityID) {
                        vm.selectedCity = vm.listCity[i];
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadCurrencies = loadCurrencies;
        vm.selectedCurrencies;
        vm.listCurrencies = [];
        var CurrencyID;
        function loadCurrencies() {
            GoodOfferEntryService.getCurrencies(
            function (response) {
                if (response.status === 200) {
                    vm.listCurrencies = response.data;
                    for (var i = 0; i < vm.listCurrencies.length; i++) {
                        if (vm.goods[0].goods.RateID === vm.listCurrencies[i].CurrencyID) {
                            vm.selectedCurrencies = vm.listCurrencies[i];
                            changeCurrent();
                            break;
                        }
                    }
                }
                else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan list Currency");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                return;
            });
        }

        vm.selectedDeliveryTerms;
        vm.listDeliveryTerms = [];
        function loadDeliveryTerms() {
            PurchReqService.getDeliveryTerms(function (reply) {
                UIControlService.unloadLoading();
                vm.listDeliveryTerms = reply.data.List;
                for (var i = 0; i < vm.listDeliveryTerms.length; i++) {
                    if (vm.goods[0].goods.DeliveryTerms === vm.listDeliveryTerms[i].RefID) {
                        vm.selectedDeliveryTerms = vm.listDeliveryTerms[i];
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadRFQItem = loadRFQItem;
        function loadRFQItem() {
            var status = {
                Status: vm.TenderRefID
            }
            GoodOfferEntryService.GetRFQ(status, function (reply) {
                UIControlService.unloadLoading();
                vm.RfqID = reply.data;
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadRFQGoods = loadRFQGoods;
        function loadRFQGoods() {
            var status = {
                Status: vm.TenderRefID
            }
            GoodOfferEntryService.GetRFQGoods(status, function (reply) {
                UIControlService.unloadLoading();
                vm.RfqGoods = reply.data;
                vm.TenderType = reply.data[0].TenderType;
                loadReference(reply.data[0].DeliveryTerms);
                loadPaymentTerm(reply.data[0].PaymentTerm);
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.selectedFreightCost;
        vm.listFreightCost = [];
        function loadFreightCost() {
            GoodOfferEntryService.getTypeFreightCost(function (reply) {
                UIControlService.unloadLoading();
                vm.listFreightCost = reply.data.List;
                for (var i = 0; i < vm.listFreightCost.length; i++) {
                    if (vm.goods[0].goods.FreightCostType === vm.listFreightCost[i].RefID) {
                        vm.selectedFreightCost = vm.listFreightCost[i];
                        changeFreight(vm.selectedFreightCost.RefID);
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.changeFreight = changeFreight;
        vm.freight = [];
        vm.selectedFreightCostTime;
        function changeFreight(data) {
            GoodOfferEntryService.GetFreightTime({ Status: data }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.freight = reply.data;
                    for (var i = 0; i < vm.freight.length; i++) {
                        if (vm.goods[0].goods.FreightLeadTime === vm.freight[i].DeliveryTime) {
                            vm.selectedFreightCostTime = vm.freight[i];
                        }
                    }

                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Penawaran Barang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.selectedPaymentTerm;
        vm.listPaymentTerm = [];
        function loadPaymentTerm(data) {
            GoodOfferEntryService.getPaymentTerm(function (reply) {
                UIControlService.unloadLoading();
                vm.listPaymentTerm = reply.data;
                for (var i = 0; i < vm.listPaymentTerm.length; i++) {
                    if (data === vm.listPaymentTerm[i].Id) {
                        vm.selectedPaymentTerm = vm.listPaymentTerm[i];
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadReference = loadReference;
        function loadReference(data) {
            GoodOfferEntryService.DeliveryTerm({}, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.DeliveryTerm = reply.data.List;
                    for (var i = 0; i < vm.DeliveryTerm.length; i++) {
                        if (data == vm.DeliveryTerm[i].RefID) vm.selectedDeliveryTerms = vm.DeliveryTerm[i];
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data penawaran" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.changeCurrent = changeCurrent;
        function changeCurrent() {
            var data = {
                Keyword: vm.selectedCurrencies.Symbol
            }
            GoodOfferEntryService.GetExchangeRate(data, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.rate = reply.data;
                    vm.ExchangeRate = vm.rate.ExchangeRate;
                    vm.Date = vm.rate.ValidFrom;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Penawaran Barang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadOtherDoc = loadOtherDoc;
        function loadOtherDoc(data) {
            vm.listOtherDoc = [];
            GoodOfferEntryService.getOtherDoc({ Status: data }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.data = reply.data;
                    for (var i = 0; i < vm.data.length; i++) {
                        vm.listOtherDoc.push({
                            ID: vm.data[i].ID,
                            fileUpload: [{
                                name: vm.data[i].Filename
                            }],
                            Remark: vm.data[i].Remark,
                            DocUrl: vm.data[i].DocUrl
                        });
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Perusahaan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        //jasa
        function loadServiceOfferEntry() {
            UIControlService.loadLoading("Silahkan Tunggu");
            PengumumanPengadaanService.getKelengkapanDocVendor({
                TenderStepID: vm.StepIDJasa
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    console.info("penawaran:" + JSON.stringify(data));
                    vm.listKelengkapan = data.VendorDocuments;
                    for (var i = 0; i < vm.listKelengkapan.length; i++) {
                        if (!(vm.listKelengkapan[i].ApproveDate === null)) {
                            vm.listKelengkapan[i].ApproveDate = UIControlService.getStrDate(vm.listKelengkapan[i].ApproveDate);
                        }
                    }

                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.detailDokumen = detailDokumen;
        function detailDokumen(data) {
            //console.info(JSON.stringify(data));
            console.info("tendRef" + vm.TenderID);
            console.info("stepID" + vm.StepIDJasa);
            console.info("procPack" + vm.ProcPackType);
            console.info("dataOFFentry" + data.OfferEntryDocumentID);
            if (!(data.DocumentType === 'FORM_DOCUMENT')) {
                var dt = {
                    DocumentName: data.DocumentName,
                    FileType: data.FileType,
                    ApproveDate: data.ApproveDate,
                    DocumentURL: data.DocumentURL,
                    OfferEntryDocumentID: data.OfferEntryDocumentID,
                    OfferEntryVendorID: data.OfferEntryVendorID,
                    IsCheck: false
                }
                var modalInstance = $uibModal.open({
                    templateUrl: 'detail-dok-penawaran.html',
                    controller: 'DetDokPenawaranCtrl',
                    controllerAs: 'DetDokPenawaranCtrl',
                    resolve: {
                        item: function () {
                            return dt;
                        }
                    }
                });
                modalInstance.result.then(function () {
                    vm.init();
                });
            } else {
                $state.transitionTo('kelengkapan-tender-jasa-vendor', {
                    TenderRefID: vm.TenderID, StepID: vm.StepIDJasa, ProcPackType: vm.ProcPackType, DocID: data.OfferEntryDocumentID
                });
            }
        }

        //vhs
        vm.GetRFQ = GetRFQ;
        function GetRFQ() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            PPVHSService.selectRFQId({ Status: vm.TenderRefID }, function (reply) {
                console.info("rfqID:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.RFQId = reply.data;
                   loadVHSOfferEntry();
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evaluasi Teknis" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadRefDeliveryTerm = loadRefDeliveryTerm;
        function loadRefDeliveryTerm() {
            PPVHSService.DeliveryTerm({}, function (reply) {
                console.info("deliTerm:"+JSON.stringify(vm.RFQId.TenderType));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.DeliveryTerm = reply.data.List;
                    for (var i = 0; i < vm.DeliveryTerm.length; i++) {
                        if (vm.RFQId.TenderType == vm.DeliveryTerm[i].RefID) vm.selectedDeliveryTerms = vm.DeliveryTerm[i];
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data delivery term" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        function loadVHSOfferEntry() {
            vm.j = 0;
            PengumumanPengadaanService.loadTemplateOfferEntry({
                Status: vm.TenderRefID,
                column: vm.StepID
            }, function (reply) {
                //console.info("offEntry" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataOfferEntry = reply.data;
                    vm.QuotationNo = vm.dataOfferEntry[0].vhs.QuotationNo;
                    for (var i = 0; i < vm.dataOfferEntry.length; i++) {
                        if (vm.dataOfferEntry[i].DocName === "Surat Penawaran") {
                            vm.dataOfferEntry[i].DocName += ' ' + vm.RFQId.Limit + '%';
                        }
                    }

                    if (vm.dataOfferEntry[0].vhs.IncoId !== null) {

                        vm.VHSData = {
                            IncoTerm: vm.dataOfferEntry[0].vhs.IncoId,
                            FreightCostID: vm.dataOfferEntry[0].vhs.FreightCostDetailId,
                            BidderSelMethod: vm.RFQId.BidderSelMethod,
                            DeliveryTerms: vm.RFQId.DeliveryTerms
                        }
                        loadIncoTerms(vm.VHSData, 1);
                    }
                    else {

                        loadIncoTerms(vm.RFQId, 0);
                    }
                    for (var i = 0; i < vm.dataOfferEntry.length; i++) {
                        if (vm.j == 0 && vm.dataOfferEntry[i].ID !== 0) {
                            vm.QuotationNo = vm.dataOfferEntry[i].vhs.QuotationNo;
                            vm.j = 1;
                        }
                        if (i === vm.dataOfferEntry.length - 1 && vm.j == 0) {

                        }
                    }
                    loadRefDeliveryTerm();
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.selectedIncoTerms;
        vm.listIncoTerms = [];
        vm.loadIncoTerms = loadIncoTerms;
        function loadIncoTerms(data, flag) {
            vm.flagFreight = flag;
            PPVHSService.getIncoTerms({
                BidderSelMethod: data.BidderSelMethod,
                DeliveryTerms: data.DeliveryTerms
            }, function (reply) {
                UIControlService.unloadLoading();
                vm.listIncoTerms = reply.data;
                for (var i = 0; i < vm.listIncoTerms.length; i++) {
                    if (data.IncoTerm === vm.listIncoTerms[i].ID) {
                        vm.selectedIncoTerms = vm.listIncoTerms[i];
                        vm.loadFreight(data, flag);
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.selectedFreight;
        vm.listFreight = [];
        vm.loadFreight = loadFreight;
        function loadFreight(data, flag) {
            if (data) vm.Id = data.IncoTerm;
            else vm.Id = vm.selectedIncoTerms.ID;
            PPVHSService.selectFreight({
                Status: vm.Id
            }, function (reply) {
                UIControlService.unloadLoading();
                vm.listFreight = reply.data;
                console.info(vm.flagFreight);
                if (data) {
                    for (var i = 0; i < vm.listFreight.length; i++) {
                        if (vm.flagFreight == 0) {
                            if (data.FreightCostID === vm.listFreight[i].FreightCostId) {
                                vm.selectedState = vm.listFreight[i];
                                console.info(vm.selectedState);
                                break;
                            }
                        } else {
                            if (data.FreightCostID === vm.listFreight[i].ID) {
                                vm.selectedState = vm.listFreight[i];
                                console.info(vm.selectedState);
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


        //copy function disini

    }
})();

