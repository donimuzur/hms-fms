(function () {
    'use strict';

    angular.module("app")
    .controller("DataEvaluator", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationTechnicalService', '$state', 'UIControlService', '$uibModal', 'GlobalConstantService', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationTechnicalService,
        $state, UIControlService, $uibModal, GlobalConstantService, $stateParams) {
        var vm = this;
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.textSearch = '';
        vm.listPengumuman = [];
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.step = [];
        vm.init = init;
        function init() {
            loadStep();
            jLoad(1);
        }

        vm.loadStep = loadStep;
        function loadStep() {
           var step = {
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            EvaluationTechnicalService.selectByEval(step, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.step = reply.data;
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

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.list = [];
            EvaluationTechnicalService.selectByContract({
                TenderRefID: vm.TenderRefID,
                ProcPackageType: vm.ProcPackType
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.list = data;
                    console.info("data:" + JSON.stringify(vm.list));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Penilai" });
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
            $state.transitionTo('evaluation-technical', { TenderRefID: vm.TenderRefID, StepID:vm.step[0].ID, ProcPackType: vm.ProcPackType});
        };

        vm.simpan = simpan;
        function simpan(data) {
            vm.list = [];
            for (var i = 0; i < data.length; i++) {
                var dt = {
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
                                TenderStepDataID: data[i].evaluation.evaluation.evaluation.TenderStepDataID,
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
        }

        vm.viewSummary = viewSummary
        function viewSummary(data) {
            var data = {
                reff:vm.TenderRefID,
                proc: vm.ProcPackType,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/evaluasi-teknis/detail-data-evaluator.html',
                controller: 'DetailFormEvaluator',
                controllerAs: 'DetailFormEvaluator',
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
    }
})();;