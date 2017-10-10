(function () {
    'use strict';

    angular.module("app")
    .controller("DetailFormEvaluator", ctrl);

    ctrl.$inject = ['item','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationTechnicalService', '$state', 'UIControlService', '$uibModal', 'GlobalConstantService', '$stateParams', '$uibModalInstance'];
    function ctrl(item, $http, $translate, $translatePartialLoader, $location, SocketService, EvaluationTechnicalService,
        $state, UIControlService, $uibModal, GlobalConstantService, $stateParams, $uibModalInstance) {
        var vm = this;
        vm.detail = item.item;
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
            vm.list = [];
            EvaluationTechnicalService.selectByEmployee({
                Status: item.reff,
                FilterType: item.proc,
                Keyword: vm.detail.EmployeeID
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
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/temporary/evaluasi-teknis/detail-data-evaluator.html',
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