(function () {
    'use strict';

    angular.module("app").controller("FormJawabanModalCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
         'RoleService', 'UIControlService', 'item', '$uibModal', 'QuestionnaireService', '$uibModalInstance'];

    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, 
		RoleService, UIControlService, item, $uibModal, QuestionnaireService, $uibModalInstance) {
        var vm = this;
        vm.data = item.item;
        vm.idQuestionnaire = 0;
        vm.tipejawaban;
        vm.opsi;
        vm.bobot;
        vm.listJawaban = [];
        vm.isUpdate;
        vm.opsi_yes;
        vm.opsi_no;
        vm.bobot_yes;
        vm.bobot_no;
        //console.info(JSON.stringify(item));

        vm.init = init;
        function init() {
            getData();          
           
        }

        vm.getData = getData;
        function getData() {
            QuestionnaireService.SelectAnswerByID({
                QuestionID: vm.data.QuestionID,
                QuestionareID: vm.data.QuestionaireID
            },
            function (response) {
                console.info(JSON.stringify(response));
                if (response.status === 200 && response.data.List.length > 0) {
                    vm.listJawaban = response.data.List;
                    vm.tipejawaban = vm.listJawaban[0].optiontype;
                    vm.isUpdate = true;
                    getOption();
                }
                else if (response.status === 200 && response.data.List.length < 1) {
                    vm.isUpdate = false;
                    vm.listJawaban = [];
                    getOption();
                }
                else {
                    UIControlService.msg_growl("error", "FORMJAWABAN.MESSAGE.ERR_GETOPTION");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                return;
            });
        }

        vm.getOption = getOption;
        vm.listOption = [];
        function getOption() {
            //console.info("oo"+JSON.stringify(vm.isUpdate));
            QuestionnaireService.SelectOption(
               function (response) {
                   //console.info(JSON.stringify(response));
                   if (response.status === 200) {
                       vm.listOption = response.data;
                       if (vm.isUpdate === true) {
                           for (var i = 0; i < vm.listOption.length; i++) {
                               console.info(i + vm.listOption[i].RefID + ":" + vm.tipejawaban.RefID);
                               if (vm.listOption[i].RefID === vm.tipejawaban.RefID) {
                                   vm.tipejawaban = vm.listOption[i];
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "FORMJAWABAN.MESSAGE.ERR_GETOPTION");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "MESSAGE.API");
                   return;
               });
        }

        vm.changeType = changeType;
        function changeType() {
            //console.info(JSON.stringify(vm.tipejawaban));
            vm.listJawaban = [];
        }

        vm.addOpsi = addOpsi;
        function addOpsi() {
            if (vm.tipejawaban.RefID === 28 && vm.listJawaban.length === 2) {
                UIControlService.msg_growl("warning", "FORMJAWABAN.MESSAGE.WAR_ADDOPTION");
                return;
            }
            var data = {
                QuestionID: vm.data.QuestionID,
                QuestionareID: vm.data.QuestionaireID,
                OptionTypeID: vm.tipejawaban.RefID,
                Description: null,
                Value: vm.opsi,
                WeightFactor: vm.bobot
            }
            vm.listJawaban.push(data);
            console.info(JSON.stringify(vm.listJawaban));
        }

        vm.deleteRow = deleteRow;
        function deleteRow(index) {
            console.info("hapus");
            var idx = index - 1;
            var _length = vm.listJawaban.length; // panjangSemula
           vm.listJawaban.splice( idx, 1 );   
        };

        vm.simpanJawaban = simpanJawaban;
        function simpanJawaban() {
            if (vm.isUpdate === false ) {
                prosessimpan();
            }
            else {
                var dataheader = {
                    QuestionID: vm.data.QuestionID,
                    QuestionareID: vm.data.QuestionaireID
                }
                var header = [];
                header.push(dataheader);
                QuestionnaireService.UpdateAnswer({
                    itemOption: header,
                    itemOptionInsert: vm.listJawaban
                }, function (reply) {
                    console.info("quest: " + JSON.stringify(reply));
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "FORM.MESSAGE.SUC_SAVE");
                        return;
                    }
                    else {
                        UIControlService.msg_growl("error", "FORM.MESSAGE.ERR_SAVE");
                        return;
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", 'MESSAGE.API');
                });
            }
                /*
            else if (vm.isUpdate === false && vm.tipejawaban.RefID === 28) {
                var dataYes = {
                    QuestionID: vm.data.QuestionID,
                    QuestionareID: vm.data.QuestionaireID,
                    OptionTypeID: vm.tipejawaban.RefID,
                    Description: null,
                    Value: vm.opsi_yes,
                    WeightFactor: vm.bobot_yes
                }
                var dataNo = {
                    QuestionID: vm.data.QuestionID,
                    QuestionareID: vm.data.QuestionaireID,
                    OptionTypeID: vm.tipejawaban.RefID,
                    Description: null,
                    Value: vm.opsi_no,
                    WeightFactor: vm.bobot_no
                }
                vm.listJawaban.push(dataYes);
                vm.listJawaban.push(dataNo);
                prosessimpan();
            }
            */

        }

        function prosessimpan() {
            QuestionnaireService.InsertAnswer({
                itemOption: vm.listJawaban
                }, function (reply) {
                    //console.info("quest: " + JSON.stringify(reply));
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "FORM.MESSAGE.SUC_SAVE");
                        return;
                    }
                    else {
                        UIControlService.msg_growl("error", "FORM.MESSAGE.ERR_SAVE");
                        return;
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
                });
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();