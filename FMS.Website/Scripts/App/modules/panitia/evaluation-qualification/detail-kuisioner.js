(function () {
    'use strict';

    angular.module("app")
    .controller("DetailQuestionnaireQualificationCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationQualificationService', '$state', 'UIControlService', '$uibModal','GlobalConstantService', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationQualificationService,
        $state, UIControlService, $uibModal, GlobalConstantService, $stateParams) {
        var vm = this;
        vm.TenderStepID = Number($stateParams.TenderRefID);
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
            EvaluationQualificationService.selectByQuestionnaire({
                column: vm.VendorID,
                FilterType: vm.ProcPackType,
                Status: vm.TenderStepID,
                Offset: offset,
                Limit: vm.pageSize
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listqualification = data.List;
                    vm.totalItems = Number(data.Count);
                    vm.StepID = vm.listqualification[0].evaluation.evaluation.evaluation.tender.ID;
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
             EvaluationQualificationService.getStep({
                 FilterType: vm.ProcPackType,
                 Status: vm.TenderStepID
             }, function (reply) {
                 UIControlService.unloadLoading();
                 if (reply.status === 200) {
                     vm.step = reply.data; vm.list = [];
                     for (var i = 0; i < data.length; i++) {
                         var dt = {
                             TenderStepID: vm.step.ID,
                             ID: data[i].ID,
                             EvalChecklistDetailID: data[i].EvalChecklistDetailID,
                             OfferEVQID: data[i].OfferEVQID,
                             IsValid: data[i].IsValid,
                             Remark: data[i].Remark,
                             evaluation: {
                                 Remark: data[i].evaluation.Remark,
                                 IsValid: data[i].evaluation.IsValid,
                                 OfferEVCID: data[i].evaluation.OfferEVCID,
                                 evaluation: {
                                     ID: data[i].evaluation.evaluation.ID,
                                     OfferEVDID: data[i].evaluation.evaluation.OfferEVDID,
                                     evaluation: {
                                         VendorID: vm.VendorID,
                                         ServiceOEVID: data[i].evaluation.evaluation.evaluation.ServiceOEVID
                                     }
                                 }
                             }
                         };
                         vm.list.push(dt);
                     }
                     EvaluationQualificationService.InsertQuestionnaireDetail(vm.list,
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
            $state.transitionTo('detail-commercial-evaluation-qualification', { TenderRefID: vm.TenderStepID, ProcPackType: vm.ProcPackType, VendorID: vm.VendorID });
        }
    }
})();;