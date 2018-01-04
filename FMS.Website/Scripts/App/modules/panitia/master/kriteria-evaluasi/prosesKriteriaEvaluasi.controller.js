(function () {
    'use strict';

    angular.module("app")
    .controller("formKriteriaEvaluasiCtrl", ctrl);
    
    ctrl.$inject = ['$http', '$uibModal', '$filter', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'KriteriaEvaluasiService', 'item', 'UIControlService'];
    /* @ngInject */
    function ctrl($http, $uibModal, $filter, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, KriteriaEvaluasiService, item, UIControlService) {
        var vm = this;
        vm.placeHolder = item.level === 1 ? 'MODAL.MASTER_KRITERIA' : 'MODAL.SUB_KRITERIA';
        vm.isEdit = false;
        vm.title = "MODAL.TAMBAH";
        vm.nama = "";
        vm.options = [];
        vm.isOptionScoreFixed = false;
        var loadingMessage = '';
        
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('master-kriteria-evaluasi');
            $translate.refresh().then(function () {
                loadingMessage = $filter('translate')('MESSAGE.LOADING');
            });
            if (item.criteriaId) {
                vm.isEdit = true;
                vm.title = "MODAL.UBAH";
                vm.nama = item.criteriaName;
                vm.isOptionScoreFixed = item.isOptionScoreFixed;

                KriteriaEvaluasiService.getOptions({
                    criteriaId: item.criteriaId
                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        vm.options = reply.data;
                    } else {
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_OPTS'));
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_OPTS'));
                    UIControlService.unloadLoadingModal();
                });
            }
        };

        vm.addOption = addOption;
        function addOption() {
            var prevMaxScore = 0;
            if (vm.options.length > 0) {
                prevMaxScore = vm.options[vm.options.length - 1].MaxScore;
            }
            vm.options.push({
                MinScore: prevMaxScore,
                MaxScore: 100
            });
        }

        vm.deleteOption = deleteOption;
        function deleteOption(index) {
            if (index != vm.options.length - 1) {
                vm.options[index + 1].MinScore = vm.options[index].MinScore;
            }
            vm.options.splice(index, 1);
        }

        vm.maxScoreChange = maxScoreChange;
        function maxScoreChange(index) {
            if (!vm.options[index].MaxScore) {
                vm.options[index].MaxScore = 0;
            }
            var prevMaxScore = 0; //memastikan nilai maksimum tidak kurang dari nilai maksimum opsi sebelumnya
            if (index > 0) {
                prevMaxScore = vm.options[index - 1].MaxScore;
            }
            if (vm.options[index].MaxScore < prevMaxScore) {
                vm.options[index].MaxScore = prevMaxScore;
            }

            var nextMaxScore = 100; //memastikan nilai maksimum tidak melebihi nilai maksimum opsi selanjutnya
            if (index != vm.options.length - 1) {
                nextMaxScore = vm.options[index + 1].MaxScore;
            }
            if (vm.options[index].MaxScore > nextMaxScore) {
                vm.options[index].MaxScore = nextMaxScore;
            }

            if (index != vm.options.length - 1) { //menjadikan nilai maksimum sebagai nilai minimum opsi selanjutnya
                vm.options[index + 1].MinScore = vm.options[index].MaxScore;
            }
        }

        vm.cancel = cancel;
        function cancel () {
            $uibModalInstance.dismiss('cancel');
        };
        
        vm.save = save;
        function save() {
            if (!vm.nama) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NO_NAME");
                return;
            }
            for (var i = 0; i < vm.options.length; i++) {
                if (!vm.options[i].OptionName) {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_NO_DESCRIPTION");
                    return;
                }
            }

            UIControlService.loadLoadingModal(loadingMessage);
            if (vm.isEdit) {
                KriteriaEvaluasiService.updateCriteria({
                    CriteriaId: item.criteriaId,
                    CriteriaName: vm.nama,
                    EvaluationCriteriaOptions: vm.options,
                    IsOptionScoreFixed: vm.isOptionScoreFixed
                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE'));
                        $uibModalInstance.close();
                    } else {
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE'));
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE'));
                    UIControlService.unloadLoadingModal();
                });
            } else {
                KriteriaEvaluasiService.insertCriteria({
                    CriteriaName: vm.nama,
                    Level: item.level,
                    ParentId: item.parentId,
                    EvaluationCriteriaOptions: vm.options,
                    IsOptionScoreFixed: vm.isOptionScoreFixed
                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE'));
                        $uibModalInstance.close();
                    } else {
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE'));
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE'));
                    UIControlService.unloadLoadingModal();
                });
            }
        }
    }
})();