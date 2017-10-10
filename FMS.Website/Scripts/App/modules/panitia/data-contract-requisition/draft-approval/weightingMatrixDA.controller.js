(function () {
    'use strict';

    angular.module("app")
    .controller("weightingMatrixDACtrl", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'MetodeEvaluasiService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, metodeEvaluasiService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";
        vm.isTenderVerification = true;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.APPROVAL", href: "#/data-contract-requisition/approval" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/data-contract-requisition/approval/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.WEIGHTING_MATRIX", href: "" }
        ];

        vm.ProjectTitle = "";
        vm.selectedEvaluationMethod;

        var Administrasi = [];
        var AdministrasiLevel1 = [];
        var AdministrasiLevel2 = [];
        var AdministrasiLevel3 = [];
        var Teknis = [];
        var TeknisLevel1 = [];
        var TeknisLevel2 = [];
        var TeknisLevel3 = [];
        var Harga = [];
        var HargaLevel1 = [];
        var HargaLevel2 = [];
        var HargaLevel3 = [];
        vm.Administrasi;
        vm.bobotAdministrasi;
        vm.Teknis;
        vm.bobotTeknis;
        vm.Harga;
        vm.bobotHarga;
        vm.kategori;
        vm.nama;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            UIControlService.unloadLoading(loadmsg);
            DataContractRequisitionService.IsApprover({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        vm.loadData();
                    } else {
                        UIControlService.msg_growl("warning", $filter('translate')('MESSAGE.ERR_NOT_APPROVER'));
                        $state.transitionTo('contract-requisition-draft-approval');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_APPROVER'));
                    $state.transitionTo('contract-requisition-draft-approval');
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_APPROVER'));
                $sta$state.transitionTo('contract-requisition-draft-approval');
            });
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SelectById({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                if (reply.status === 200) {
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.selectedEvaluationMethod = String(reply.data.EvaluationMethodId);
                    vm.loadMEDetail();
                } else {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
            });
        }
        
        vm.loadMEDetail = loadMEDetail;
        function loadMEDetail() {
            UIControlService.loadLoading(loadmsg);
            metodeEvaluasiService.selectById({
                EvaluationMethodId: Number(vm.selectedEvaluationMethod)
            }, function (reply) {
                if (reply.status === 200) {
                    vm.kategori = reply.data.EvaluationMethodDetails;
                    for (var i = 0; i < vm.kategori.length; i++) {
                        if (vm.kategori[i].DetailType === 'Administrasi')
                            vm.bobotAdministrasi = vm.kategori[i].Weight;
                        else if (vm.kategori[i].DetailType === 'Teknis')
                            vm.bobotTeknis = vm.kategori[i].Weight;
                        else if (vm.kategori[i].DetailType === 'Harga')
                            vm.bobotHarga = vm.kategori[i].Weight;
                    }
                    vm.nama = reply.data.EvaluationMethodName;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_ME_DET'));
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_ME_DET'));
            });

            metodeEvaluasiService.selectDCByMethod({
                EvaluationMethodId: Number(vm.selectedEvaluationMethod)
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var hasil = reply.data;
                    Administrasi = [];
                    AdministrasiLevel1 = [];
                    AdministrasiLevel2 = [];
                    AdministrasiLevel3 = [];
                    Teknis = [];
                    TeknisLevel1 = [];
                    TeknisLevel2 = [];
                    TeknisLevel3 = [];
                    Harga = [];
                    HargaLevel1 = [];
                    HargaLevel2 = [];
                    HargaLevel3 = [];
                    for (var i = 0; i < hasil.length; i++) {
                        if (hasil[i].DetailType === 'Administrasi') {
                            Administrasi.push(hasil[i]);
                        }
                        else if (hasil[i].DetailType === 'Teknis') {
                            Teknis.push(hasil[i]);
                        }
                        else if (hasil[i].DetailType === 'Harga') {
                            Harga.push(hasil[i]);
                        }
                    }
                    for (var i = 0; i < Administrasi.length; i++) {
                        if (Administrasi[i].Level === 1) {
                            AdministrasiLevel1.push(Administrasi[i]);
                        }
                        else if (Administrasi[i].Level === 2) {
                            AdministrasiLevel2.push(Administrasi[i]);
                        }
                        else if (Administrasi[i].Level === 3) {
                            AdministrasiLevel3.push(Administrasi[i]);
                        }
                    }
                    for (var i = 0; i < AdministrasiLevel2.length; i++) {
                        AdministrasiLevel2[i].sub = [];
                        for (var j = 0; j < AdministrasiLevel3.length; j++) {
                            if (AdministrasiLevel3[j].Parent === AdministrasiLevel2[i].CriteriaId) {
                                AdministrasiLevel2[i].sub.push(AdministrasiLevel3[j]);
                            }
                        }
                    }
                    for (var i = 0; i < AdministrasiLevel1.length; i++) {
                        AdministrasiLevel1[i].sub = [];
                        for (var j = 0; j < AdministrasiLevel2.length; j++) {
                            if (AdministrasiLevel2[j].Parent === AdministrasiLevel1[i].CriteriaId) {
                                AdministrasiLevel1[i].sub.push(AdministrasiLevel2[j]);
                            }
                        }

                    }

                    for (var i = 0; i < Teknis.length; i++) {
                        if (Teknis[i].Level === 1) {
                            TeknisLevel1.push(Teknis[i]);
                        }
                        else if (Teknis[i].Level === 2) {
                            TeknisLevel2.push(Teknis[i]);
                        }
                        else if (Teknis[i].Level === 3) {
                            TeknisLevel3.push(Teknis[i]);
                        }
                    }
                    for (var i = 0; i < TeknisLevel2.length; i++) {
                        TeknisLevel2[i].sub = [];
                        for (var j = 0; j < TeknisLevel3.length; j++) {
                            if (TeknisLevel3[j].Parent === TeknisLevel2[i].CriteriaId) {
                                TeknisLevel2[i].sub.push(TeknisLevel3[j]);
                            }
                        }
                    }
                    for (var i = 0; i < TeknisLevel1.length; i++) {
                        TeknisLevel1[i].sub = [];
                        for (var j = 0; j < TeknisLevel2.length; j++) {
                            if (TeknisLevel2[j].Parent === TeknisLevel1[i].CriteriaId) {
                                TeknisLevel1[i].sub.push(TeknisLevel2[j]);
                            }
                        }
                    }

                    for (var i = 0; i < Harga.length; i++) {
                        if (Harga[i].Level === 1) {
                            HargaLevel1.push(Harga[i]);
                        }
                        else if (Harga[i].Level === 2) {
                            HargaLevel2.push(Harga[i]);
                        }
                        else if (Harga[i].Level === 3) {
                            HargaLevel3.push(Harga[i]);
                        }
                    }
                    for (var i = 0; i < HargaLevel2.length; i++) {
                        HargaLevel2[i].sub = [];
                        for (var j = 0; j < HargaLevel3.length; j++) {
                            if (HargaLevel3[j].Parent === HargaLevel2[i].CriteriaId) {
                                HargaLevel2[i].sub.push(HargaLevel3[j]);
                            }
                        }
                    }
                    for (var i = 0; i < HargaLevel1.length; i++) {
                        HargaLevel1[i].sub = [];
                        for (var j = 0; j < HargaLevel2.length; j++) {
                            if (HargaLevel2[j].Parent === HargaLevel1[i].CriteriaId) {
                                HargaLevel1[i].sub.push(HargaLevel2[j]);
                            }
                        }
                    }
                    vm.Administrasi = AdministrasiLevel1;
                    vm.Teknis = TeknisLevel1;
                    vm.Harga = HargaLevel1;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_ME_DET'));
                }
            }, function (err) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_ME_DET'));
            });
        };
        
        vm.cancel = cancel;
        function cancel() {
            $state.transitionTo('detail-contract-requisition-da', { contractRequisitionId: contractRequisitionId });
        };
    }
})();