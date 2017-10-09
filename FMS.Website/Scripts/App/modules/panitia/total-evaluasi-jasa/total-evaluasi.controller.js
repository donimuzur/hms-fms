(function () {
    'use strict';

    angular.module("app").controller("TotalEvaluasiJasaCtrl", ctrl);

    ctrl.$inject = ['$filter','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'TotalEvaluasiJasaService', '$state', 'UIControlService', 'UploadFileConfigService', 'UploaderService',
        'GlobalConstantService', '$uibModal', '$stateParams'];
    function ctrl($filter, $http, $translate, $translatePartialLoader, $location, SocketService,
        TEJService, $state, UIControlService, UploadFileConfigService, UploaderService,
        GlobalConstantService, $uibModal, $stateParams) {
        var vm = this;
        vm.IDTender = Number($stateParams.TenderRefID);
        var loadmsg = "MESSAGE.LOADING";
        vm.IDStepTender = Number($stateParams.StepID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.TenderName = '';
        vm.ProsentaseTechnical;
        vm.ProsentasePricing;
        vm.TenderID;
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("total-evaluasi-jasa");
            getLogin();
            loadDataTender();
            loadMethodEval(vm.IDTender);
        }
        vm.getLogin = getLogin;
        function getLogin() {
            TEJService.getLogin({
                TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.flagCheckList = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.addCheck = addCheck;
        function addCheck(flag, checklist){
            vm.flagCek = true;
            if(flag === 1){
                for (var i = 0; i < vm.listVendor.length; i++) {
                    vm.listVendor[i].IsCheck = checklist;
                }
                vm.cek = checklist;
            }

        }

        function loadDataTender() {
            TEJService.getDataStepTender({
                ID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.TenderName = data.tender.TenderName;
                    vm.StartDate = UIControlService.getStrDate(data.StartDate);
                    vm.EndDate = UIControlService.getStrDate(data.EndDate);
                    vm.nama_tahapan = data.step.TenderStepName;
                    vm.TenderID = data.TenderID;
                    
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
    
        function loadMethodEval(idtenderref) {
            TEJService.getMethodEvaluation({
                ID: idtenderref
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].DetailType === "Teknis") {
                            vm.ProsentaseTechnical = data[i].Weight;
                        }
                        else if (data[i].DetailType === "Harga") {
                            vm.ProsentasePricing = data[i].Weight
                        }
                    }
                    loadEvaTechPrice(vm.TenderID, vm.ProsentaseTechnical, vm.ProsentasePricing);
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.listDataEvalTechPrice = [];
        function loadEvaTechPrice(idtender, procenttech, procentprice) {
            TEJService.getDataEvaTechPrice({
                TenderID: idtender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listDataEvalTechPrice = data;
                    var newdata = [];
                    for (var i = 0; i < vm.listDataEvalTechPrice.length; i++) {
                        if(vm.listDataEvalTechPrice[i].score_technical === null){
                            vm.listDataEvalTechPrice[i].score_technical = 0;
                        }
                        if(vm.listDataEvalTechPrice[i].score_pricing === null){
                            vm.listDataEvalTechPrice[i].score_pricing = 0;
                        }
                        var tech = 0; var pric = 0;
                        tech = vm.listDataEvalTechPrice[i].score_technical * 2 * (procenttech/100);
                        pric = vm.listDataEvalTechPrice[i].score_pricing * 2 * (procentprice/100);
                        var aturdata = {
                            TenderStepID: vm.IDStepTender,
                            TenderID: vm.TenderID,
                            VendorID: vm.listDataEvalTechPrice[i].VendorID,
                            score_technical: parseFloat(tech),
                            score_pricing: parseFloat(pric),
                            score_total: parseFloat((tech + pric)),
                            rank: 0
                        }
                        newdata.push(aturdata);
                    }
                    sortRanking(newdata, 'score_total');
                    TEJService.InsertTotalEvaluation(newdata, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            loadDataTotalEval();
                        }
                    }, function (err) {
                        UIControlService.msg_growl("error", "MESSAGE.API");
                        UIControlService.unloadLoading();
                    });
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadDataTotalEval() {
            TEJService.getFinalTotalEval({
                TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listVendor = data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadDataTender() {
            TEJService.getDataStepTender({
                ID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.TenderName = data.tender.TenderName;
                    vm.StartDate = UIControlService.getStrDate(data.StartDate);
                    vm.EndDate = UIControlService.getStrDate(data.EndDate);
                    vm.nama_tahapan = data.step.TenderStepName;
                    vm.TenderID = data.TenderID;

                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function sortRanking(array, key) {
            function sortByKey(a, b) {
                var x = a[key];
                var y = b[key];
                return ((x < y) ? 1 : ((x > y) ? -1 : 0));
            }
            array.sort(sortByKey);
        }

        vm.backDetailTahapan = backDetailTahapan;
        function backDetailTahapan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
        }

        vm.sendToApproval = sendToApproval;
        function sendToApproval(dt) {
            console.info(vm.listVendor);
            vm.cr = [];
            for (var i = 0; i< vm.listVendor.length; i++) {
                vm.cr.push({
                    ID: vm.listVendor[i].ID,
                    TenderStepID: vm.listVendor[i].TenderStepID,
                    IsCheck: vm.listVendor[i].IsCheck
                });
            }
            bootbox.confirm($filter('translate')('CONFIRM.SEND_FOR_APPROVAL'), function (yes) {
                if (yes) {
                    UIControlService.loadLoading(loadmsg);
                    TEJService.sendToApproval(vm.cr, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SEND_TO_APPRV'));
                            init();
                        } else {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
                        }
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
                    });
                }
            });
        }


    }
})();