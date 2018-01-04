(function () {
    'use strict';

    angular.module("app")
    .controller("DetailCommercialEquipmentQualificationCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationQualificationService', '$state', 'UIControlService', '$uibModal','GlobalConstantService', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationQualificationService,
        $state, UIControlService, $uibModal, GlobalConstantService, $stateParams) {
        var vm = this;
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.VendorID = Number($stateParams.VendorID);
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.textSearch = '';
        vm.listPengumuman = [];
        vm.currentPage = 1;
        vm.pageSize = 10;

        vm.init = init;
        function init() {
            jLoad(1);
        }


        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.listqualification = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            EvaluationQualificationService.selectByCode({
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
                    vm.StepID = vm.listqualification[0].evaluation.evaluation.tender.ID;
                    console.info("data:" + JSON.stringify(data));
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

        vm.batal = batal;
        function batal() {
            //$uibModalInstance.dismiss('cancel');
            $uibModalInstance.close();
        };

         vm.simpan = simpan;
         function simpan(data) {
            vm.list = [];
            for (var i = 0; i < data.length; i++) {
                var dt = {
                    ID: data[i].ID,
                    EvaluationQualfDetailID: data[i].EvaluationQualfDetailID,
                    OfferEVCID: data[i].OfferEVCID,
                    IsValid: data[i].IsValid,
                    Remark: data[i].Remark,
                    evaluation: {
                        OfferEVDID : data[i].evaluation.OfferEVDID,
                        evaluation: {
                            VendorID: vm.VendorID,
                            TenderStepDataID: vm.StepID,
                            ServiceOEVID: data[i].evaluation.evaluation.ServiceOEVID
                        }
                    }
                };
                vm.list.push(dt);
            }
             EvaluationQualificationService.InsertChecklistDetail(vm.list,
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

        vm.kembali = kembali;
        function kembali() {
            $state.transitionTo('equipment-evaluation-qualification', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType, VendorID: vm.VendorID });
        }

        vm.gotoDetail = gotoDetail;
        function gotoDetail(data) {
            console.info(JSON.stringify(data.vendorOffer.DocumentType));
            if (data.vendorOffer.DocumentType === "FORM_DOCUMENT") {
                $state.transitionTo('detail-questionnaire', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType, VendorID: vm.VendorID });
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

    }
})();;