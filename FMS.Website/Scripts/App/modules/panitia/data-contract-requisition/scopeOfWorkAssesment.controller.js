(function () {
    'use strict';

    angular.module("app")
    .controller("scopeOfWorkAssesmentCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";
        var sowaQuestions = [];        

        vm.questionnaires = [];
        vm.totalScore;
        vm.titleScore = 'Beginner';
        vm.newSOWA;
        vm.isTenderVerification = false;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/detail-contract-requisition/" + $stateParams.contractRequisitionId},
            { title: "BREADCRUMB.SOWA", href: "" }
        ];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.IsRequestor({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        loadCRInfo();
                        vm.loadData();
                    } else {
                        UIControlService.msg_growl("warning", $filter('translate')('MESSAGE.ERR_NOT_REQUESTOR'));
                        $state.transitionTo('data-contract-requisition');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_REQUESTOR'));
                    $state.transitionTo('data-contract-requisition');
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                $state.transitionTo('data-contract-requisition');
            });
        };

        function loadCRInfo() {
            DataContractRequisitionService.SelectById({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.isTenderVerification = reply.data.StatusName !== 'CR_DRAFT' && reply.data.StatusName.lastIndexOf('CR_REJECT_', 0) !== 0;
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
                    } else {
                        vm.newSOWA = true;
                        DataContractRequisitionService.SelectNewSOWAQuestion({}, function (reply) {
                            if (reply.status === 200) {
                                if (reply.data != null) {
                                    arrangeNewSOWAForm(reply.data);
                                    UIControlService.unloadLoading();
                                } else {
                                    UIControlService.unloadLoading();
                                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_NO_NEW_SOWA'));
                                }
                            } else {
                                UIControlService.unloadLoading();
                                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_SOWA'));
                            }
                        }, function (error) {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                        });
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

        function arrangeNewSOWAForm(questionnaireData) {
            vm.questionnaires = [];
            vm.totalScore = 0;
            var categoryNumber = 1;
            for (var i = 0; i < questionnaireData.length; i++) {
                var questionnaire = {};
                questionnaire.Name = questionnaireData[i].Name;
                questionnaire.categories = [];
                for (var j = 0; j < questionnaireData[i].question.length; j++) {
                    questionnaireData[i].question[j].ContractRequisitionSOWAOptionId = null;
                    questionnaireData[i].question[j].ContractRequisitionSOWAOptions = questionnaireData[i].question[j].Opts;
                    if (questionnaireData[i].question[j].ContractRequisitionSOWAOptions.length > 0) {
                        questionnaireData[i].question[j].ContractRequisitionSOWAOptionId = questionnaireData[i].question[j].ContractRequisitionSOWAOptions[0].OptionID;
                        vm.totalScore += questionnaireData[i].question[j].ContractRequisitionSOWAOptions[0].WeightFactor;
                    }
                    questionnaireData[i].question[j].ContractRequisitionId = contractRequisitionId;
                    sowaQuestions.push(questionnaireData[i].question[j]);

                    var needNewCategory = true;
                    for (var k = 0; k < questionnaire.categories.length; k++) {
                        if (questionnaire.categories[k].Category === questionnaireData[i].question[j].Category) {
                            questionnaire.categories[k].Questions.push(questionnaireData[i].question[j]);
                            needNewCategory = false;
                            break;
                        }
                    }
                    if (needNewCategory) {
                        var newCategory = {
                            Category: questionnaireData[i].question[j].Category,
                            Number: categoryNumber++,
                            Questions: []
                        };
                        newCategory.Questions.push(questionnaireData[i].question[j]);
                        questionnaire.categories.push(newCategory);
                    }
                }
                vm.questionnaires.push(questionnaire);
            }
        };

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

        vm.save = save;
        function save() {
            UIControlService.loadLoading(loadmsg);
            var saveSOWA = vm.newSOWA ? DataContractRequisitionService.InsertSOWA : DataContractRequisitionService.UpdateSOWA;
            saveSOWA(sowaQuestions, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_SOWA'));
                    vm.back();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_SOWA'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.back = back;
        function back() {
            $state.transitionTo('detail-contract-requisition', { contractRequisitionId: contractRequisitionId });
        };
    }
})();