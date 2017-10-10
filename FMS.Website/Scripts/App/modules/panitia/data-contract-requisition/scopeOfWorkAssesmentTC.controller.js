(function () {
    'use strict';

    angular.module("app")
    .controller("scopeOfWorkAssesmentTCCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'RequisitionListService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, RequisitionListService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";
        var sowaQuestions = [];        
        vm.isTenderVerification = true;

        vm.questionnaires = [];
        vm.totalScore;
        vm.titleScore = 'Beginner';
        vm.newSOWA;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.PROSES_PENGADAAN", href: "" },
            { title: "BREADCRUMB.REQUISITION_LIST", href: "#/requisition-list" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/requisition-list/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.SOWA", href: "" }
        ];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            UIControlService.unloadLoading(loadmsg);
            RequisitionListService.IsApprover({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        loadCRInfo();
                        vm.loadData();
                    } else {
                        UIControlService.msg_growl("warning", $filter('translate')('MESSAGE.ERR_NOT_APPROVER'));
                        $state.transitionTo('requisition-list');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_APPROVER'));
                    $state.transitionTo('requisition-list');
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                $sta$state.transitionTo('requisition-list');
            });
        };

        function loadCRInfo() {
            DataContractRequisitionService.SelectById({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.ProjectTitle = reply.data.ProjectTitle;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SelectSOWAQuestion({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                if (reply.status === 200) {
                    if (reply.data != null) {
                        sowaQuestions = reply.data;
                        arrangeSOWAForm(sowaQuestions);
                        UIControlService.unloadLoading();
                        vm.newSOWA = false;
                    }
                } else {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_SOWA'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        function arrangeSOWAForm(questions) {
            vm.questionnaires = [];
            vm.totalScore = 0;
            var categoryNumber = 1;
            for (var i = 0; i < questions.length; i++) {
                vm.totalScore += questions[i].SOWAOptionWeightFactor;

                var needNewQuestionnaire = true;
                for (var j = 0; j < vm.questionnaires.length; j++) {
                    if (vm.questionnaires[j].Name === questions[i].QuestionnaireName) {
                        var needNewCategory = true;
                        for (var k = 0; k < vm.questionnaires[j].categories.length; k++) {
                            if (vm.questionnaires[j].categories[k].Category === questions[i].QuestionCategory) {
                                vm.questionnaires[j].categories[k].Questions.push(questions[i]);
                                needNewCategory = false;
                                break;
                            }
                        }
                        if (needNewCategory) {
                            var newCategory = {
                                Category: questions[i].QuestionCategory,
                                Number: categoryNumber++,
                                Questions: []
                            };
                            newCategory.Questions.push(questions[i]);
                            vm.questionnaires[j].categories.push(newCategory);
                        }
                        needNewQuestionnaire = false;
                        break;
                    }
                }
                if (needNewQuestionnaire) {
                    var newQuestionnaire = {
                        Name: questions[i].QuestionnaireName,
                        categories: []
                    }
                    var newCategory = {
                        Category: questions[i].QuestionCategory,
                        Number: categoryNumber++,
                        Questions: []
                    };
                    newCategory.Questions.push(questions[i]);
                    newQuestionnaire.categories.push(newCategory);
                    vm.questionnaires.push(newQuestionnaire);
                }
            }
            recalculateTitleScore();
        }        

        vm.recalculateTotalValue = recalculateTotalValue;
        function recalculateTotalValue() {
            vm.totalScore = 0;
            sowaQuestions.forEach(function(sq) {
                var WeightFactor = 0;
                if (vm.newSOWA) {
                    sq.ContractRequisitionSOWAOptions.forEach(function (opt) {
                        if (Number(sq.ContractRequisitionSOWAOptionId) === opt.OptionID) {
                            WeightFactor = opt.WeightFactor;
                        }
                    });
                } else {
                    sq.ContractRequisitionSOWAOptions.forEach(function (opt) {
                        if (Number(sq.ContractRequisitionSOWAOptionId) === opt.ContractRequisitionSOWAOptionId) {
                            WeightFactor = opt.WeightFactor;
                        }
                    });
                }                
                vm.totalScore += WeightFactor;
            });
            recalculateTitleScore();
        }

        function recalculateTitleScore() {
            if (vm.totalScore > 110) {
                vm.titleScore = 'National Contractor';
            } else if (vm.totalScore > 66) {
                vm.titleScore = 'Advance';
            } else {
                vm.titleScore = 'Beginner';
            }
        }

        vm.back = back;
        function back() {
            $state.transitionTo('detail-contract-requisition-tc', { contractRequisitionId: contractRequisitionId });
        };
    }
})();