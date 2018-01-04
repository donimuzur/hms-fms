(function () {
    'use strict';

    angular.module("app").controller("PPVHSVendorCtrl", ctrl);

    ctrl.$inject = ['Excel', '$timeout', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PPVHSService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl(Excel, $timeout, $http, $translate, $translatePartialLoader, $location, SocketService, PPVHSService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        vm.NegoId = 0;
        vm.jLoad = jLoad;

        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            GetRFQ();
            loadTender();

        }
        vm.loadReference = loadReference;
        function loadReference() {
            PPVHSService.DeliveryTerm({}, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.DeliveryTerm = reply.data.List;
                    for (var i = 0; i < vm.DeliveryTerm.length; i++) {
                        if (vm.RFQId.TenderType == vm.DeliveryTerm[i].RefID) vm.selectedDeliveryTerms = vm.DeliveryTerm[i];
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


        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.vhs = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                Status: vm.TenderRefID,
                column: vm.StepID
            }
            PPVHSService.select(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.vhs = reply.data;
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

        vm.loadTemplate = loadTemplate;
        function loadTemplate() {
            vm.j =0;
            var tender = {
                Status: vm.TenderRefID,
                column: 190
            }
            PPVHSService.select(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.Template = reply.data;
                    for (var i = 0; i < vm.Template.length; i++) {
                        if (vm.Template[i].DocName === "Surat Penawaran") {
                            vm.Template[i].DocName += ' ' + vm.RFQId.Limit + '%';
                        }
                    }
                    
                    if (vm.Template[0].vhs.IncoId !== null) {
                        vm.QuotationNo = vm.Template[0].vhs.QuotationNo;
                        vm.VHSData = {
                            IncoTerm: vm.Template[0].vhs.IncoId,
                            FreightCostID: vm.Template[0].vhs.FreightCostDetailId,
                            BidderSelMethod: vm.RFQId.BidderSelMethod,
                            DeliveryTerms: vm.RFQId.DeliveryTerms
                        }
                        loadIncoTerms(vm.VHSData, 1);
                    }
                    else {

                        loadIncoTerms(vm.RFQId, 0);
                    }
                    for (var i = 0; i < vm.Template.length; i++) {
                        if (vm.j == 0 && vm.Template[i].ID !== 0) {
                            vm.QuotationNo = vm.Template[i].vhs.QuotationNo;
                            vm.j = 1;
                        }
                        if (i === vm.Template.length - 1 && vm.j == 0) {
                            
                        }
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadTender = loadTender;
        function loadTender() {
            PPVHSService.selectStep({ ID: vm.StepID }, function (reply) {
                console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.step = reply.data;
                    var startDate = vm.step.StartDate;
                    var endDate = vm.step.EndDate;
                    checkStepDate(startDate, endDate);
                    vm.TenderRefID = vm.step.tender.TenderRefID;
                    vm.ProcPackType = vm.step.tender.ProcPackageType;
                    vm.TenderID = vm.step.TenderID;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Chatting" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        function checkStepDate(startDate, endDate) {
            var dateNow = new Date();
            var startDate = new Date(startDate);
            var endDate = new Date(endDate);
            vm.inProcess = false;
            if (dateNow > startDate && dateNow < endDate) {
                vm.inProcess = true;
            }
        }

        vm.upload = upload;
        function upload(data) {
            $state.transitionTo('upload-file-vhs', { DocTypeID: data.TenderDocTypeID, StepID: vm.StepID, TenderRefID: vm.TenderRefID
        });
           
        }

        vm.backpengadaan = backpengadaan;
        function backpengadaan() {

            $state.transitionTo('detail-tahapan-vendor', { TenderID: vm.TenderID  });
        }

        vm.Insert = Insert;
        function Insert() {
            console.info(vm.selectedState);
            var data = {
                QuotationNo: vm.QuotationNo,
                TenderStepID: vm.StepID,
                SupplierLeadTime:vm.RFQId.LeadTime,
                ExpiredDay: vm.RFQId.LeadTime,
                VHSOfferEntryDetails: vm.Template
            }
            if (vm.selectedIncoTerms == null) data.IncoId = null;
            else data.IncoId = vm.selectedIncoTerms.ID;
            if (vm.selectedState == null) data.FreightCostDetailId = null;
            else data.FreightCostDetailId = vm.selectedState.ID;
            PPVHSService.InsertAll(data,
              function (reply) {
                  if (reply.status === 200) {
                      UIControlService.msg_growl("success", "Berhasil Simpan data");
                      window.location.reload();
                     // $state.transitionTo('detail-tahapan-vendor', { TenderID: vm.TenderID });
                  }
                  else {
                      UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                      return;
                  }
              },
              function (err) {
                  UIControlService.msg_growl("error", "Gagal Akses Api!!");
              }
         );
        }

        vm.uploadDet = uploadDet;
        function uploadDet(data, flag) {
            var data = {
                TenderStepID: vm.StepID,
                act: flag,
                item: data,
                inProcess: vm.inProcess
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/pemasukkan-penawaran-vhs/formDoc.html',
                controller: "frmDocCtrl",
                controllerAs: "frmDocCtrl",
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

        vm.uploadDetBC = uploadDetBC;
        function uploadDetBC(data, flag) {
            console.info("inProc:" + vm.inProcess);
            var data = {
                TenderStepID: vm.StepID,
                act: flag,
                item: data,
                Limit: vm.RFQId.Limit,
                inProcess:vm.inProcess
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/pemasukkan-penawaran-vhs/formBC.html',
                controller: "frmBCCtrl",
                controllerAs: "frmBCCtrl",
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

        vm.downloadExcel = downloadExcel;
        function downloadExcel() {
            var data = {
                TenderRefID: vm.TenderRefID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/pemasukkan-penawaran-vhs/SaveExcelNotif.html',
                controller: "SaveNotifExcel",
                controllerAs: "SaveNotifExcel",
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

        vm.viewDetail = viewDetail;
        function viewDetail() {
            var data = {
                StepID: vm.StepID,
                DocTypeID: 21
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/pemasukkan-penawaran-vhs/DetailPenawaran.html',
                controller: "DetailPenawaranCtrl",
                controllerAs: "DetailPenawaranCtrl",
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


        vm.selectedDeliveryTerms;
        vm.listDeliveryTerms = [];
        vm.loadDeliveryTerms = loadDeliveryTerms;
        function loadDeliveryTerms(data) {
            PPVHSService.getDeliveryTerms(function (reply) {
                UIControlService.unloadLoading();
                vm.listDeliveryTerms = reply.data.List;

                for (var i = 0; i < vm.listDeliveryTerms.length; i++) {
                    if (data.DeliveryTerms === vm.listDeliveryTerms[i].RefID) {
                        vm.selectedDeliveryTerms = vm.listDeliveryTerms[i];
                        break;
                    }
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
                        vm.changeFreightDetai(data, flag);
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
        vm.changeFreightDetai = changeFreightDetai;
        function changeFreightDetai(data, flag) {
            if(data) vm.Id = data.IncoTerm;
            else vm.Id = vm.selectedIncoTerms.ID;
            PPVHSService.selectFreight({
                Status:vm.Id
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

        vm.GetRFQ = GetRFQ;
        function GetRFQ() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            PPVHSService.selectRFQId({ Status: vm.TenderRefID }, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.RFQId = reply.data;
                    loadTemplate();
                    loadReference();
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

        vm.Freight = Freight;
        function Freight(data) {
            console.info(data);
            PPVHSService.selectFreight({ Status: vm.TenderRefID }, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.FreightCost = reply.data;
                    if (data !== undefined) {
                        for (var i = 0; i<vm.FreightCost.length; i++) {
                            if (vm.FreightCost[i].FreightCostID === data.DeliveryTransport) {
                                vm.selectedState = vm.FreightCost[i];
                                vm.selectedFreightCostTime = vm.FreightCost[i];
                                changeTransport(vm.selectedState, 0);
                            }
                        }
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.changeState = changeState;
        function changeState(data) {
            vm.selectedTransport = {};
            vm.selectedFreightCostTime = {};
            PPVHSService.selectFreight({ Keyword: data.DeliveryState }, function (reply) {
                
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.Transport = reply.data;
                    if (vm.Transport.length === 1) {
                        vm.selectedTransport = vm.Transport[0];
                        changeTransport(vm.selectedTransport, vm.selectedState);
                    }
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

        vm.changeTransport = changeTransport;
        function changeTransport(data1, flag) {
            PPVHSService.selectTransport({ Status: data1.DeliveryTransportID,  Keyword: data1.DeliveryState }, function (reply) {

                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.Time = reply.data;
                    if(flag != undefined){
                        for (var i = 0; i < vm.Time.length; i++) {
                            if(vm.Time[i].FreightCostID == data1.DeliveryTransport){
                                vm.selectedFreightCostTime = vm.Time[i];
                            }
                    }
                    }
                    if (vm.Time.length === 1) {
                        vm.selectedFreightCostTime = vm.Time[0];
                    }
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

        

       

    }
})();
//TODO


