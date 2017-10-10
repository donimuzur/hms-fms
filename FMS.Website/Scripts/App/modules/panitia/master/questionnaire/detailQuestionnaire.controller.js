(function () {
    'use strict';

    angular.module("app").controller("DetailQuestionnaireCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'QuestionnaireService',
        '$uibModal', '$stateParams', '$state', 'UIControlService', '$filter'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, QuestionnaireService,
        $uibModal, $stateParams, $state, UIControlService, $filter) {
        var vm = this;
        vm.idQuestionnaire = Number($stateParams.idQuestionnaire);
        vm.nama_questionnaire = "";
        vm.jenis_form = "";
        vm.listQuestionnaire = [];
        vm.listQuestion = [];

        vm.init = init;
        function init(){
            $translatePartialLoader.addPart('master-questionnaire');
            vm.jLoad();
        };

        vm.jLoad = jLoad;
        function jLoad() {
            UIControlService.loadLoading("Silahkan Tunggu..");
            QuestionnaireService.SelectQuestionByID({
                Status : vm.idQuestionnaire
            }, function (reply) {
                //console.info("data detail:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listQuestionnaire = data.List[0];
                    vm.nama_questionnaire = vm.listQuestionnaire.Name;
                    vm.jenis_form = vm.listQuestionnaire.formtype[0].Name;
                    vm.listQuestion = vm.listQuestionnaire.itemDetails;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('DETAIL.MESSAGE.ERR_SELECT'));
                    UIControlService.unloadLoading();
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
                UIControlService.unloadLoading();
                return;
            });
        }

        vm.openFormJawaban = openFormJawaban;
        function openFormJawaban(data,act) {
            var data = {
                act: true,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/questionnaire/formJawaban.html',
                controller: 'FormJawabanModalCtrl',
                controllerAs: 'jawabanModalCtrl',
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

        vm.back = back;
        function back() {
            $state.transitionTo('master-questionnaire');
        }   
    }
})();