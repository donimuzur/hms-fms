(function () {
    'use strict';

    angular.module("app").controller("EquipmentQualificationCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationQualificationService', 'RoleService', 'UIControlService', '$uibModal', 'GlobalConstantService', '$stateParams', '$state'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationQualificationService,
        RoleService, UIControlService, $uibModal, GlobalConstantService, $stateParams, $state) {
       
        var vm = this;
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.VendorID = Number($stateParams.VendorID);
        var page_id = 141;
        vm.departemen = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        vm.jLoad = jLoad;

        function init() {
            jLoad(1);

        }
        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.listqualification = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            EvaluationQualificationService.selectByVendor({
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType,
                column: vm.VendorID,
                Offset: offset,
                Limit: vm.pageSize
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listqualification = data.List;
                    vm.totalItems = Number(data.Count);
                    console.info("data:" + JSON.stringify(data));
                    if (vm.listqualification[0].evaluation == null) {
                        vm.StepID = vm.listqualification[0].TenderStepDataID;
                    }
                    else {
                        vm.StepID = vm.listqualification[0].evaluation.tender.ID;
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Tahapan Evaluasi Kualifikasi" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.kembali = kembali;
        function kembali() {
            $state.transitionTo('evaluation-qualification', { TenderRefID: vm.TenderRefID, StepID:vm.StepID, ProcPackType:vm.ProcPackType });
        }

        vm.gotoDetail = gotoDetail;
        function gotoDetail(data) {
            console.info(JSON.stringify(data));
            if (data.vendorOffer.DocumentType === "FORM_DOCUMENT") {
                $state.transitionTo('detail-commercial-evaluation-qualification', { TenderRefID: data.evaluation.tender.tender.TenderRefID, ProcPackType: data.evaluation.tender.tender.ProcPackageType, VendorID: vm.VendorID });
            }
            else {
                var data = {
                    item: data
                }
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/modules/panitia/evaluation-qualification/detail-evaluation-qualification.html',
                    controller: 'DetailEquipmentQualificationCtrl',
                    controllerAs: 'DetailEquipmentQualificationCtrl',
                    resolve: {
                        item: function () {
                            return data;
                        }
                    }
                });
                modalInstance.result.then(function () {
                    vm.jLoad(1);
                });
            }
            
        }

        vm.simpan = simpan;
        function simpan(data) {
            vm.list = [];
            for (var i = 0; i < data.length; i++) {
                var dt = {
                    ID: data[i].ID,
                    EvaluationID: data[i].EvaluationID,
                    OfferEVDID: data[i].OfferEVDID,
                    IsValid: data[i].IsValid,
                    Remark: data[i].Remark,
                    evaluation:{
                        VendorID: vm.VendorID,
                        TenderStepDataID: vm.StepID,
                        ServiceOEVID: data[i].evaluation.ServiceOEVID
                    }
                };
                vm.list.push(dt);
            }
             EvaluationQualificationService.InsertDetail(vm.list,
                function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan data");
                        jLoad(1);
                    }
                    else {
                        UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                        return;
                    }
                },
                function (err) {
                    UIControlService.msg_growl("error", "Gagal Akses Api!!");
                    UIControlService.unloadLoadingModal();
                }
           );
        }
    }
})();

