(function () {
    'use strict';

    angular.module("app")
    .controller("detailPRCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PurchaseRequisitionService', '$state',
        'UIControlService', '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PurchReqService, $state, UIControlService,
        $uibModal, $stateParams) {
        var vm = this;
        vm.RFQID = Number($stateParams.RFQID);

        vm.maxSize = 10;
        vm.currentPage = 0;
        vm.listStepTender = [];
        vm.listComodity = [];
        vm.listClasification = [];
        vm.listTypeTender = [];
        vm.listOptionsTender = [];
        vm.checkAreaKomoditi = false;
        vm.checkDaftarVendor = false;
        vm.isAdd;
        vm.dataForm = {};       
        
        vm.init = init();
        function init() {
            $translatePartialLoader.addPart("purchase-requisition");
            if (vm.RFQID > 0) {
                PurchReqService.selectPRRFQByID({ ID: vm.RFQID }, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        var data = reply.data;
                        //console.info(JSON.stringify(data));
                        vm.dataForm = data;
                        if (vm.dataForm.IsByArea === true) { vm.checkAreaKomoditi = true; }
                        if (vm.dataForm.IsByVendor === true) { vm.checkDaftarVendor = true; }
                        vm.listStepTender = vm.dataForm.RFQGoodsSteps;
                        //set data list vendor
                        for (var a = 0; a < vm.listStepTender.length; a++) {
                            vm.listStepTender[a]['TenderStepName'] = vm.listStepTender[a].MstTenderStep.FormTypeName;
                            vm.listStepTender[a].StartDate = new Date(Date.parse(vm.listStepTender[a].StartDate));
                            vm.listStepTender[a].EndDate = new Date(Date.parse(vm.listStepTender[a].EndDate));
                        }
                        vm.listVendor = vm.dataForm.Vendors;
                        //set data itemPR
                        for (var b = 0; b < vm.dataForm.RFQGoodsItemPRs.length; b++) {
                            vm.itemPRChecked.push(vm.dataForm.RFQGoodsItemPRs[b].ItemPR);
                        }
                        console.info("itemPR:: " + JSON.stringify(vm.itemPRChecked));
                        itemcombo();
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "MESSAGE.API");
                    UIControlService.unloadLoading();
                });
            } else {
                vm.dataForm = new RFQField(0, '', '', '', 0, 0, null, null, null, null, '', 0, 0, 0, 0, 0, null, null, null,
                    null, null, 0, null, 0, 0, 0, 0);
                itemcombo();
            }
            
        }

        function itemcombo() {
            loadKomoditi();
            loadKlasifikasi();
            loadBidderMethod();
            loadDeliveryTerms();
            loadIncoTerms();
            loadSchemaTender();
            loadTypeTender();
            loadOptionsTender();
            loadStateDelivery();
            loadEvalMethod();
        }

        function selectByID(ID) {
            PurchReqService.selectPRRFQByID({ ID: ID },function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.dataForm = data;
                    if (vm.dataForm.IsByArea === true) { vm.checkAreaKomoditi = true; }
                    if (vm.dataForm.IsByVendor === true) { vm.checkDaftarVendor = true; }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.openCalendar = openCalendar;
        vm.isCalendarOpened = [false, false, false, false];
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        vm.openCalendar2 = openCalendar2;
        vm.isCalendarOpened2 = [false, false, false, false];
        function openCalendar2(index) {
            vm.isCalendarOpened2[index] = true;
        };

        //list combo komoditas
        vm.selectedComodity;
        function loadKomoditi() {
            PurchReqService.getCommodity(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.listComodity = reply.data;
                    if (vm.RFQID > 0) {
                        //console.info(vm.dataForm.CommodityID + "..."+JSON.stringify(vm.listComodity));
                        for (var i = 0; i < vm.listComodity.length; i++) {
                            if (vm.dataForm.CommodityID === vm.listComodity[i].ID) {
                                vm.selectedComodity = vm.listComodity[i];
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

        //list combo klasifikasi
        vm.selectedClasification;
        function loadKlasifikasi() {
            PurchReqService.getClasification(function (reply) {
                UIControlService.unloadLoading();
                vm.listClasification = reply.data.List;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listClasification.length; i++) {
                        if (vm.dataForm.CompScale === vm.listClasification[i].RefID) {
                            vm.selectedClasification = vm.listClasification[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //list combo jenis penawaran (type tender)
        vm.selectedTypeTender;
        function loadTypeTender() {
            PurchReqService.getTypeTender(function (reply) {
                UIControlService.unloadLoading();
                vm.listTypeTender = reply.data.List;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listTypeTender.length; i++) {
                        if (vm.dataForm.TenderType === vm.listTypeTender[i].RefID) {
                            vm.selectedTypeTender = vm.listTypeTender[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //list combo tipe penawaran (options tender)
        vm.selectedOptionsTender;
        function loadOptionsTender() {
            PurchReqService.getOptionsTender(function (reply) {
                UIControlService.unloadLoading();
                vm.listOptionsTender = reply.data.List;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listOptionsTender.length; i++) {
                        if (vm.dataForm.TenderOption === vm.listOptionsTender[i].RefID) {
                            vm.selectedOptionsTender = vm.listOptionsTender[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
        
        //list options delivery terms
        vm.selectedDeliveryTerms;
        vm.listDeliveryTerms = [];
        function loadDeliveryTerms() {
            PurchReqService.getDeliveryTerms(function (reply) {
                UIControlService.unloadLoading();
                vm.listDeliveryTerms = reply.data.List;
                
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //list combo Inco terms
        vm.selectedIncoTerms;
        vm.listIncoTerms = [];
        function loadIncoTerms() {
            PurchReqService.getIncoTerms(function (reply) {
                UIControlService.unloadLoading();
                vm.listIncoTerms = reply.data;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listIncoTerms.length; i++) {
                        if (vm.dataForm.IncoTerm === vm.listIncoTerms[i].ID) {
                            vm.selectedIncoTerms = vm.listIncoTerms[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //list combo metode bidder
        vm.selectedBidderMethod;
        vm.listBidderMethod = [];
        function loadBidderMethod() {
            PurchReqService.getBidderMethod(function (reply) {
                UIControlService.unloadLoading();
                vm.listBidderMethod = reply.data.List;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listBidderMethod.length; i++) {
                        if (vm.dataForm.BidderSelMethod === vm.listBidderMethod[i].RefID) {
                            vm.selectedBidderMethod = vm.listBidderMethod[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //list combo skema Tender
        vm.selectedSchemaTender;
        vm.listSchemaTender = [];
        function loadSchemaTender() {
            PurchReqService.getProcMethods(function (reply) {
                UIControlService.unloadLoading();
                vm.listSchemaTender = reply.data;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listSchemaTender.length; i++) {
                        if (vm.dataForm.ProcMethod === vm.listSchemaTender[i].MethodID) {
                            vm.selectedSchemaTender = vm.listSchemaTender[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //list combo evaluasi method
        vm.selectedEvalMethod;
        vm.listEvalMethod = [];
        function loadEvalMethod() {
            PurchReqService.getEvalMethod(function (reply) {
                UIControlService.unloadLoading();
                vm.listEvalMethod = reply.data;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listEvalMethod.length; i++) {
                        if (vm.dataForm.EvalMethod === vm.listEvalMethod[i].EvaluationMethodId) {
                            vm.selectedEvalMethod = vm.listEvalMethod[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //list provinsi
        vm.selectedState;
        vm.listState = [];
        function loadStateDelivery() {
            PurchReqService.getStateDelivery(function (reply) {
                UIControlService.unloadLoading();
                vm.listState = reply.data;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listState.length; i++) {
                        if (vm.dataForm.DeliveryLocationState === vm.listState[i].StateID) {
                            vm.selectedState = vm.listState[i];
                            changeState();
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.changeState = changeState;
        function changeState() {
            loadCityDelivery(vm.selectedState.StateID);
        }

        //list City
        vm.selectedCity;
        vm.listCity = [];
        function loadCityDelivery(IDState) {
            //console.info("state:" + IDState);
            PurchReqService.getCityDelivery({ StateID: IDState }, function (reply) {
                UIControlService.unloadLoading();
                //console.info("city:" + JSON.stringify(reply));
                vm.listCity = reply.data;
                if (vm.RFQID > 0) {
                    for (var i = 0; i < vm.listCity.length; i++) {
                        if (vm.dataForm.DeliveryLocationCity === vm.listCity[i].CityID) {
                            vm.selectedCity = vm.listCity[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //tampilkan tahapan
        vm.getSteps = getSteps;
        function getSteps() {
            if (vm.selectedSchemaTender === undefined) {
                UIControlService.msg_growl("error", 'Skema Tender Harus Dipilih Terlebih Dahulu');
                return;
            }
            else {
                //console.info("eval:" + JSON.stringify(vm.selectedSchemaTender));
                UIControlService.loadLoadingModal('Loading Tampilkan Tahapan');
                PurchReqService.getSteps({ ProcMethod: vm.selectedSchemaTender.MethodID }, function (reply) {
                    if (reply.status === 200) {
                        vm.tenderSteps = reply.data;
                        //console.info("steps:" + JSON.stringify(vm.tenderSteps));
                        for (var i = 0; i < vm.tenderSteps.length; i++) {
                            var step = {
                                RFQGoodsID: 0,
                                TenderStepID: vm.tenderSteps[i].TenderStepID,
                                TenderStepName: vm.tenderSteps[i].TenderStepName,
                                StartDate: new Date(),
                                EndDate: new Date(),
                                Duration: vm.tenderSteps[i].Duration
                            }
                            vm.listStepTender.push(step);
                        }
                        UIControlService.unloadLoadingModal();
                    } else {
                        UIControlService.unloadLoadingModal();
                        UIControlService.msg_growl("error", 'NOTIFICATION.GET.STEPS.ERROR', "NOTIFICATION.GET.STEPS.TITLE");
                    }
                }, function (err) {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", 'NOTIFICATION.GET.STEPS.ERROR', "NOTIFICATION.GET.STEPS.TITLE");
                })
            }            
        }

        /*open form pilih item PR*/
        vm.openItemPR = openItemPR;
        vm.itemPRChecked = []
        function openItemPR(data) {
            if (vm.selectedComodity === undefined) {
                UIControlService.msg_growl("error", 'Komoditi harus dipilih terlebih dahulu');
                return;
            }
            var data = {
                item: vm.itemPRChecked,
                CommodityID : vm.selectedComodity.ID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/purchaseRequisition/dataRFQ/detailitemPR.html',
                controller: 'DetailItemPRCtrl',
                controllerAs: 'DetailItemPRCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function (dataitem) {
                //console.info(JSON.stringify(dataitem));
                vm.itemPRChecked = dataitem;
            });
        }

        /*tampilkan vendor*/
        vm.viewVendor = viewVendor;
        function viewVendor() {
            if (vm.selectedComodity === undefined) {
                UIControlService.msg_growl("warning", "FORMINPUTRFQ.MSG.ERR_CMB_COMODITY");
                return;
            }
            if (vm.selectedClasification === undefined) {
                UIControlService.msg_growl("warning", "FORMINPUTRFQ.MSG.ERR_CLASIFICATION");
                return;
            }
            if ((vm.dataForm.IsLocalVendor === undefined || vm.dataForm.IsLocalVendor === false) &&
                (vm.dataForm.IsNational === undefined || vm.dataForm.IsNational === false) &&
                (vm.dataForm.IsInternational === undefined || vm.dataForm.IsInternational === false)) {
                UIControlService.msg_growl("warning", "FORMINPUTRFQ.MSG.ERR_AREA");
                return;
            }
            vm.viewVendorModel = {
                CommodityID: vm.selectedComodity.ID,
                IsLocalVendor: vm.dataForm.IsLocalVendor,
                IsNational: vm.dataForm.IsNational,
                IsInternational: vm.dataForm.IsInternational,
                CompScale: vm.selectedClasification.RefID
            };

            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/purchaseRequisition/dataRFQ/viewVendorRFQ.html',
                controller: 'viewVendorRFQCtrl',
                controllerAs: 'viewVendorRFQCtrl',
                resolve: {
                    model: function () {
                        return vm.viewVendorModel;
                    },
                }
            });
        }

        /*get vendor  by area ,komoditi, klasifikasi*/
        vm.getVendorByArea = getVendorByArea;
        function getVendorByArea() {
            UIControlService.loadLoading('Loading Data Vendor');
            PurchReqService.viewVendor({
                CommodityID: vm.selectedComodity.ID,
                IsLocalVendor: vm.dataForm.IsLocalVendor,
                IsNational: vm.dataForm.IsNational,
                IsInternational: vm.dataForm.IsInternational,
                CompScale: vm.selectedClasification.RefID
            }, function (reply) {
                UIControlService.unloadLoading();
                console.info("ven:" + JSON.stringify(reply));
                if (reply.status === 200) {
                    vm.listVendor = reply.data;
                } else {
                    UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
                    return;
                }
            }, function (err) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
            });
        }

        /* tambah vendor */
        vm.openDataVendor = openDataVendor;
        vm.listVendor = [];
        function openDataVendor() {
            var data = {};
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/purchaseRequisition/dataRFQ/DataVendor.html',
                controller: 'DataVendorCtrl',
                controllerAs: 'DataVendorCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function (dataVendor) {
                //cek vendor sudah ada belum
                var foundby = $.map(vm.listVendor, function (val) {
                    return val.VendorID === dataVendor.VendorID ? val : null;
                });
                if (foundby.length > 0) {
                    UIControlService.msg_growl("error", "Vendor " + dataVendor.Name + " sudah ditambahkan!!");
                    return;
                }
                else {
                    vm.listVendor.push(dataVendor);
                }                
               
            });
        }

        vm.hapusVendor = hapusVendor;
        function hapusVendor(data) {
            for (var i = 0; i < vm.listVendor.length; i++) {
                vm.listVendor.splice(i, 1);
            }
        }
        /* end tambah vendor */

        /* untuk cek form disabled enabled */
        vm.cekForm = cekForm;
        function cekForm(action) {
            if (action === 'area') {
                vm.checkAreaKomoditi = true;
                vm.checkDaftarVendor = false;
            }
            else if (action === 'vendor') {
                vm.checkAreaKomoditi = false;
                vm.checkDaftarVendor = true;
            }
            else {
                vm.checkAreaKomoditi = false;
                vm.checkDaftarVendor = false;
            }
        }

        /*proses simpan*/
        vm.prosesSimpan = prosesSimpan;
        function prosesSimpan() {
            if (vm.checkAreaKomoditi === false && vm.checkDaftarVendor === false) {
                UIControlService.msg_growl("warning", "FORMINPUTRFQ.MSG.ERR_VENDOR");
                return;
            }
            //console.info(JSON.stringify(vm.dataForm));
            if (vm.dataForm.DeliveryTerms === null) {
                UIControlService.msg_growl("warning", "LB_DELIVERYTERMS belum di pilih!");
                return;
            }

            /*cek jika area checked*/
            if (vm.checkAreaKomoditi === true || vm.checkDaftarVendor === false) {
                if (vm.dataForm.IsLocal === undefined) { vm.dataForm.IsLocal = false; }
                if (vm.dataForm.IsNational === undefined) { vm.dataForm.IsNational = false; }
                if (vm.dataForm.IsInternational === undefined) { vm.dataForm.IsInternational = false; }
                if (vm.dataForm.IsVendorEmails === undefined) { vm.dataForm.IsVendorEmails = false; }
                vm.dataForm.CommodityID = vm.selectedComodity.ID;
                vm.dataForm.CompScale = vm.selectedClasification.RefID;
                vm.dataForm.IsByArea = true;
                vm.dataForm.IsByVendor = false;

            }
            else if (vm.checkAreaKomoditi === false || vm.checkDaftarVendor === true) {
                vm.dataForm.CommodityID = null;
                vm.dataForm.IsLocal = null;
                vm.dataForm.IsNational = null;
                vm.dataForm.IsInternational = null;
                vm.dataForm.CompScale = null;
                vm.IsVendorEmails = null;
                vm.Emails = null;
                vm.IsByVendor = true;
                vm.IsByArea = false;
            }

            vm.dataForm.TenderType = vm.selectedTypeTender.RefID;
            vm.dataForm.TenderOption = vm.selectedOptionsTender.RefID;
            vm.dataForm.IncoTerm = vm.selectedIncoTerms.ID;
            vm.dataForm.DeliveryTerms = Number(vm.dataForm.DeliveryTerms);
            vm.dataForm.DeliveryLocationState = vm.selectedState.StateID;
            vm.dataForm.DeliveryLocationCity = vm.selectedCity.CityID;
            
            vm.dataForm.BidderSelMethod = vm.selectedBidderMethod.RefID;
            vm.dataForm.ProcMethod = vm.selectedSchemaTender.MethodID;
            vm.dataForm.EvalMethod = vm.selectedEvalMethod.EvaluationMethodId;
            /*set tahapan tender*/
            var stepTenders = [];
            for (var a = 0; a < vm.listStepTender.length; a++) {
                var step = {
                    TenderStepID: vm.listStepTender[a].TenderStepID,
                    StartDate: vm.listStepTender[a].StartDate,
                    EndDate: vm.listStepTender[a].EndDate,
                    Duration: Number(vm.listStepTender[a].Duration)
                }
                stepTenders.push(step);
            }
            /*set daftar vendor*/
            var vendorsList = [];
            for (var b = 0; b < vm.listVendor.length; b++) {
                var dataVen = { VendorID: vm.listVendor[b].VendorID };
                vendorsList.push(dataVen);
            }
            /*set item PR ID*/
            var itemPRs = [];
            //console.info("items" + JSON.stringify(vm.itemPRChecked));
            for(var c = 0; c < vm.itemPRChecked.length; c++){
                var item = { ItemPRID: vm.itemPRChecked[c].ID }
                itemPRs.push(item);
            }

            //console.info("vendor:: " + JSON.stringify(vendorsList));
            //console.info("step:: " + JSON.stringify(stepTenders));
            //console.info("item:: " + JSON.stringify(itemPRs));
            vm.dataForm['Vendors'] = vendorsList;
            vm.dataForm['RFQGoodsSteps'] = stepTenders;
            vm.dataForm['RFQGoodsItemPRs'] = itemPRs;
            //console.info("finalgoods:: " + JSON.stringify(vm.dataForm));
            /*proses simpan */
            if (vm.RFQID > 0) {
                PurchReqService.updatePRRFQ(vm.dataForm, function (reply) {
                    UIControlService.unloadLoading();
                    //console.info("prq:: " + JSON.stringify(reply));
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Data");
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
            else {
                PurchReqService.insertPRRFQ(vm.dataForm, function (reply) {
                    UIControlService.unloadLoading();
                    //console.info("prq:: " + JSON.stringify(reply));
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Data");
                        $state.tra
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
        }

        vm.back = back;
        function back() {
            $state.transitionTo('purchase-requisition');
        }


        /*model form*/
        function RFQField(ID, RFQCode, RFQName, NoticeText, VendorSelectBy, CommodityID, IsLocal, IsNational, IsInternational, IsVendorEmails
            , Emails, CompScale, DeliveryTerms, IncoTerm, DeliveryLocationState, DeliveryLocationCity, BidderSelMethod, ProcMethod, EvalMethod, Status, IsByArea,
            Limit, ExpiredDay, IsOpen, IsByVendor, TenderType, TenderOption) {
            vm.self = this;
            self.RFQCode = RFQCode;
            self.RFQName = RFQName;
            self.NoticeText = NoticeText;
            self.VendorSelectBy = VendorSelectBy;
            self.CommodityID = CommodityID;
            self.IsLocal = IsLocal;
            self.IsNational = IsNational;
            self.IsInternational = IsInternational;
            self.IsVendorEmails = IsVendorEmails;
            self.Emails = Emails;
            self.CompScale = CompScale;
            self.DeliveryTerms = DeliveryTerms;
            self.IncoTerm = IncoTerm;
            self.DeliveryLocationState = DeliveryLocationState;
            self.DeliveryLocationCity = DeliveryLocationCity;
            self.BidderSelMethod = BidderSelMethod;
            self.ProcMethod = ProcMethod;
            self.EvalMethod = EvalMethod;
            self.IsByArea = IsByArea;
            self.Limit = Limit;
            self.ExpiredDay = ExpiredDay;
            self.IsOpen = IsOpen;
            self.IsByVendor = IsByVendor;
            self.TenderType = TenderType;
            self.TenderOption = TenderOption;
        }

    }
})();