(function () {
    'use strict';

    angular.module("app").controller("FormQuestionnaireCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'QuestionnaireService',
        '$stateParams', 'UIControlService', '$filter', '$state'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, QuestionnaireService, $stateParams,
        UIControlService, $filter, $state) {

        var vm = this;
        vm.idQuestionnaire = Number($stateParams.idQuestionnaire);
        vm.action = "";
        vm.nama_quest = "";
        vm.kategori_p;
        vm.pertanyaan_p;
        vm.listQuestionnaire = [];
        vm.dataUp = [];
        vm.listQuestionnaire_temp = [];
        vm.listQuestionnaire_new = [];
        vm.selectedForm;

        vm.init = init;
        function init(){
            $translatePartialLoader.addPart('master-questionnaire');
            if (vm.idQuestionnaire === 0) {
                vm.nama_quest = "";
                vm.getJenisForm();
            }
            else {
                vm.nama_quest = "Ubah";
                getByID(vm.idQuestionnaire);
                //console.info("masuk: " + JSON.stringify(vm.dataUp));
                
            }
            
        };

        vm.getByID = getByID;
        function getByID(id) {
            QuestionnaireService.SelectQuestionByID({
                Status: vm.idQuestionnaire
            }, function (reply) {
                //console.info("data detail:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataUp = reply.data.List[0];
                    vm.nama_quest = vm.dataUp.Name;
                    //vm.selectedForm = vm.dataUp.formtype[0];
                    
                    vm.getJenisForm();
                    vm.listQuestionnaire_temp = reply.data.List[0].itemDetails;
                    vm.listQuestionnaire = vm.dataUp.itemDetails;                   
                    //console.info(JSON.stringify(vm.listQuestionnaire ));
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
        
        vm.getJenisForm = getJenisForm;
        vm.listJenisForm = [];
        
        function getJenisForm() {
            QuestionnaireService.SelectTypeForm(
               function (response) {
                   //console.info("out: " + JSON.stringify(response));
                   if (response.status === 200) {
                       vm.listJenisForm = response.data;
                       if (vm.idQuestionnaire > 0) {
                           //console.info("masuk>sel: " + JSON.stringify(vm.selectedForm));
                           for (var i = 0; i < vm.listJenisForm.length; i++) {
                               if (vm.listJenisForm[i].FormTypeID === vm.dataUp.formtype[0].FormTypeID) {
                                   vm.selectedForm = vm.listJenisForm[i];
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", 'FORM.MESSAGE.ERR_TYPEFORM');
                       return;
                   }
                   
               },
           function (response) {
               UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
               return;
           });
        }

        //proses input detail        
        vm.tambahList = tambahList;
        function tambahList() {
            if (vm.pertanyaan_p === '' || vm.pertanyaan_p === null || vm.pertanyaan_p === undefined) {
                UIControlService.msg_growl("warning", 'FORM.MESSAGE.CEKFQUESTION');
                return;
            }
            var list;
            var category = "";
            if (vm.kategori_p === '' || vm.kategori_p === null) {
                category = ("Uncategorized");
            }
            else {
                category = (vm.kategori_p).split(' ').join('_');;
            }
            list = {
                Category: category.toUpperCase(),
                subCategory: []
            }
            console.info(category.toUpperCase() + "::::");
            var subkategori = { QuestionID: 0, Description: vm.pertanyaan_p, IsActive:true }
            if (vm.listQuestionnaire.length > 0 && (vm.kategori_p !== '' || vm.kategori_p !== null)) {
                for (var i = 0; i < vm.listQuestionnaire.length; i++) {
                    console.info( "::::" + vm.listQuestionnaire[i].Category);
                    if (category.toUpperCase() === vm.listQuestionnaire[i].Category) {
                        vm.listQuestionnaire[i].subCategory.push(subkategori);
                        break;
                    } else {
                        list.subCategory.push(subkategori);
                        vm.listQuestionnaire.push(list);
                        break;
                    }
                }
            }
            else {
                list.subCategory.push(subkategori);
                vm.listQuestionnaire.push(list);
            }          
            
            vm.pertanyaan_p = "";
            console.info(JSON.stringify(vm.listQuestionnaire));
            //console.info(">>"+JSON.stringify(vm.listQuestionnaire_temp));
        }

        vm.prosesSimpan = prosesSimpan;
        function prosesSimpan() {
            if (vm.nama_quest === '' || vm.nama_quest === null || vm.nama_quest === undefined) {
                UIControlService.msg_growl("warning", 'FORM.MESSAGE.FNAMA_QUEST');
                return;
            }
            if (vm.selectedForm === undefined) {
                UIControlService.msg_growl("warning", 'FORM.MESSAGE.FJENIS_FORM');
                return;
            }
            UIControlService.loadLoading("Silahkan Tunggu..");
            if (vm.idQuestionnaire === 0) {
                QuestionnaireService.InsertQuestion({
                        Name: vm.nama_quest,
                        FormTypeID: vm.selectedForm.FormTypeID,
                        itemDetails: vm.listQuestionnaire
                    },
                    function (reply) {
                        //console.info("quest: " + JSON.stringify(reply));
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "FORM.MESSAGE.SUC_SAVE");
                            vm.nama_quest = "";
                            vm.listQuestionnaire = [];
                            vm.kategori_p = "";
                            return;
                        }
                        else {
                            UIControlService.msg_growl("error", "FORM.MESSAGE.ERR_SAVE");
                            return;
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
                    }
                );
            }
            else {
                //console.info("old: " + JSON.stringify(vm.listQuestionnaire_temp));
                //console.info("new: " + JSON.stringify(vm.listQuestionnaire));
                
                /*
                for (var i = 0; i < vm.listQuestionnaire.length; i++) {
                    var foundNew = $.map(vm.listQuestionnaire_temp, function (val) {
                        return (val.pr_subpekerjaan_id == $scope.subPekerjaan[i].pr_subpekerjaan_id && val.peringkat == "1") ? val : null;
                    });
                }
                */
                /*
                QuestionnaireService.UpdateQuestion({
                    Name: vm.nama_quest,
                    FormTypeID: vm.selectedForm.FormTypeID,
                    itemDetails: vm.listQuestionnaire
                },
                    function (reply) {
                        //console.info("quest: " + JSON.stringify(reply));
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "FORM.MESSAGE.SUC_SAVE");
                            return;
                            vm.nama_quest = "";
                            vm.listQuestionnaire = [];
                            vm.kategori_p = "";
                        }
                        else {
                            UIControlService.msg_growl("error", "FORM.MESSAGE.ERR_SAVE");
                            return;
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
                    }
                );
                */
            }
        }

        vm.hapusItem = hapusItem;
        function hapusItem(data) {
            for (var i = 0; i < vm.listQuestionnaire.length; i++) {
                for (var c = 0; c < vm.listQuestionnaire[i].subCategory.length; c++) {
                    if (data === vm.listQuestionnaire[i].subCategory[c]) {
                        vm.listQuestionnaire[i].subCategory.splice(c,1);
                        break;
                    }
                }
            }
            //console.info("new:" + JSON.stringify(vm.listQuestionnaire));
        }

        vm.hapusUpdate = hapusUpdate;
        function hapusUpdate(data) {
            console.info(JSON.stringify(data));
            for (var i = 0; i < vm.listQuestionnaire_temp.length; i++) {
                for (var a = 0; a < vm.listQuestionnaire_temp[i].subCategory.length; a++) {
                    if (data === vm.listQuestionnaire_temp[i].subCategory[a] ) {
                        vm.listQuestionnaire_temp[i].subCategory[a].IsActive = false;
                        break;
                    }
                }
            }
            //console.info("temp:"+JSON.stringify(vm.listQuestionnaire_temp));
            
        }

        vm.back = back;
        function back() {
            $state.transitionTo('master-questionnaire');
        }
    }
})();