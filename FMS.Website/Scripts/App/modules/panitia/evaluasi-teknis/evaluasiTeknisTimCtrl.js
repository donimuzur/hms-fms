(function () {
    'use strict';

    angular.module("app").controller("EvaluationTechnicalTimCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationTechnicalService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationTechnicalService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        var page_id = 141;
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.evalsafety = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.kata = new Kata("");
        vm.init = init;

        vm.jLoad = jLoad;

        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            EvaluationTechnicalService.isEvaluator({
                TenderRefID: vm.TenderRefID
            }, function (reply) {
                if (reply.data === true) {
                    jLoad(1);
                } else {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl('error', "Anda bukan evaluator untuk pengadaan ini");
                    vm.back();
                }
            }, function (err) {
                UIControlService.msg_growl('error', "Anda bukan evaluator untuk pengadaan ini");
                UIControlService.unloadLoading();
            });
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            console.info("curr "+current)
            vm.evaltechnical = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                ID: vm.StepID,
                tender: {
                    TenderRefID: vm.TenderRefID,
                    ProcPackageType: vm.ProcPackType
                }
            }
            EvaluationTechnicalService.select(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.evaltechnical = reply.data;
                    console.info("data:" + JSON.stringify(vm.evaltechnical));
                } else {
                    UIControlService.msg_growl('error',"Gagal mendapatkan data Evaluasi Teknis");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl('error', "Gagal mendapatkan data Evaluasi Teknis");
                UIControlService.unloadLoading();
            });
        }

        vm.detail = detail;
        function detail(data, flag) {
            //console.info(JSON.stringify(data));
            if (flag === 2) {
                $state.transitionTo('skoring-criteria', { TenderRefID: vm.TenderRefID, ID: data.VendorID, ProcPackType: vm.ProcPackType });
            }
            else if (flag === 3) {
                $state.transitionTo('equipment-evaluation-qualf-teknis', { TenderRefID: vm.TenderRefID, VendorID: data.VendorID, ProcPackType: vm.ProcPackType });
            }
        }

        vm.back = back;
        function back() {
            $state.transitionTo('evaluation-technical', { TenderRefID: vm.TenderRefID, StepID: vm.StepID, ProcPackType: vm.ProcPackType });
        }

        vm.Approval = Approval;
        function Approval() {
            var data = {
                item: vm.TenderRefID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/safetyEvaluation/DetailApproval.html',
                controller: 'DetailApprovalCtrl',
                controllerAs: 'DetailApprovalCtrl',
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
})();
//TODO

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

