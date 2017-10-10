(function () {
    'use strict';

    angular.module("app").controller("QuestionnaireCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'QuestionnaireService', '$state',
        'UIControlService', '$filter'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, QuestionnaireService, $state,
        UIControlService, $filter) {

        var vm = this;

        vm.nama_pr = "Pengadaan Kendaraan";
        vm.listQuestionnaire = [];
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.idQuestionnaire = 0;
        vm.txtSearch = "";
        vm.totalItems = 0;

        vm.init = init;
        $translatePartialLoader.addPart('master-questionnaire');
        function init(){
            
            jLoad(vm.currentPage);
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu..");
            vm.listQuestionnaire = [];
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			QuestionnaireService.SelectQuestion({
				Offset: offset,
				Limit: vm.maxSize,
				Keyword: vm.txtSearch
			}, function (reply) {
			    //console.info("data:"+JSON.stringify(reply));
			    UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.listQuestionnaire  = data.List;
					vm.totalItems = Number(data.Count);
				} else {
				    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.FAIL_SELECT'));
				    UIControlService.unloadLoading();
				    return;
				}
			}, function (err) {
			    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
			    UIControlService.unloadLoading();
			    return;
			});
        }

        vm.searchData = searchData;
        function searchData() {
            jLoad(1);
        }

        vm.formQuest = formQuest;
        function formQuest(idQuest) {
            $state.transitionTo('master-questionnaire-form', { idQuestionnaire: idQuest });
        }

        vm.detailQuest = detailQuest;
        function detailQuest(idQuest) {
            $state.transitionTo('master-questionnaire-detail', { idQuestionnaire: idQuest });
        }

        vm.editActive = editActive;
        function editActive(id, active) {
            UIControlService.loadLoading("Silahkan Tunggu");
            //console.info("ada:"+JSON.stringify(data))
            QuestionnaireService.EditActive({
                QuestionareID: id, IsActive: active
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {                    
                    if (active === false) { UIControlService.msg_growl("success", "MESSAGE.SUC_EDITACTIVE"); }
                    if (active === true) { UIControlService.msg_growl("success", "MESSAGE.SUC_EDITNONACTIVE"); }
                    jLoad(1);
                }
                else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_EDITACTIVE ");
                    return;
                }
            }, function (err) {

                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
    }
})();