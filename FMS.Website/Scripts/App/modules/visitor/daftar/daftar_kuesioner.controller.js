(function () {
    'use strict';

    angular.module("app")
    .controller("DaftarKuesionerCtrl", ctrl);

    ctrl.$inject = ['$timeout', '$uibModal', '$http', '$translate', '$translatePartialLoader', '$location', '$state', 'SocketService', 'UIControlService', 'VendorRegistrationService', 'MailerService'];
    /* @ngInject */
    function ctrl($timeout, $uibModal, $http, $translate, $translatePartialLoader, $location, $state, SocketService, UIControlService, VendorRegistrationService, MailerService) {
        var vm = this;
        var input;
        vm.questions = [];
        vm.answers = [];
        vm.currentLang;
        vm.answerquestion4 = {};
        vm.flag = 0;
        vm.initialize = initialize;
        vm.save = save;
        vm.Init = false;
        vm.tglSekarang = new Date();

        function initialize(button2, printableArea, button, divInfo, divInfo1) {

           
            vm.currentLang = $translate.use();
            $translatePartialLoader.addPart('daftar');
            loadVendor();
            loadCek(button2, printableArea, button, divInfo, divInfo1);
        }

        vm.loadVendor = loadVendor();
        function loadVendor() {
            VendorRegistrationService.selectVendor(
                {
                    VendorID: Number(localStorage.getItem('vendor_reg_id'))
            },function (reply) {
                    //console.info("data:"+JSON.stringify(reply));
                   UIControlService.unloadLoading();
                   if (reply.status === 200) {
                       vm.vendor = reply.data;
                       for (var i = 0; i < vm.vendor.length; i++) {
                           if (vm.vendor[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN') {
                               vm.address = vm.vendor[i].Contact.Address.AddressInfo + vm.vendor[i].Contact.Address.AddressDetail;
                               vm.VendorName = vm.vendor[i].Vendor.VendorName;
                               vm.npwp = vm.vendor[i].Vendor.Npwp;
                           }
                           else if (vm.vendor[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY') {
                               vm.email = vm.vendor[i].Contact.Email;
                               vm.username = vm.vendor[i].Vendor.user.Username;
                               vm.VendorName = vm.vendor[i].Vendor.VendorName;
                           }
                           else if (vm.vendor[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_PERSONAL') {
                              
                               if (vm.telp == undefined) {
                                   vm.name = vm.vendor[i].Contact.Name;
                                   vm.telp = vm.vendor[i].Contact.Phone;
                               } 
                           } 
                   }
                       } else {
                       $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
                       UIControlService.unloadLoading();
                       }
                       }, function (err) {
                   console.info("error:" +JSON.stringify(err));
                       //$.growl.error({ message: "Gagal Akses API >" + err });
                       UIControlService.unloadLoading();
               });
        }

        vm.change = change;
        function change(data, index) {
            for (var i = 0; i < vm.data.length; i++) {
                for (var i = 0; i < vm.data.length; i++) {

                }
            }
            console.info(data);
        }

        vm.printDiv = printDiv;
        vm.FlagValue = 0;
        function printDiv(areaID, button, divInfo, divInfo1) {
            for (var i = 0; i < vm.data.length; i++) {
                if (vm.data[i].Value == 0) {
                    vm.FlagValue = 1;
                    UIControlService.msg_growl('error', "ERRORS.VALUE_NOT_FOUND");
                    
                }
                if (vm.FlagValue === 0 && i === (vm.data.length - 1)) {
                    var modalInstance = $uibModal.open({
                        templateUrl: 'app/modules/visitor/daftar/formQuestionnaire.html',
                        controller: "FormSureQuestionnaireCtrl",
                        controllerAs: "FormSureQuestionnaireCtrl"
                    });
                    modalInstance.result.then(function () {
                        console.info("coba");
                        vm.save();
                    });
                    
                }
            }
            vm.FlagValue = 0;
        }

        vm.jload = jload;
        function jload(button2, printableArea, button, divInfo, divInfo1) {
            vm.button = button;
            vm.divInfo = divInfo;
            vm.divInfo1 = divInfo1;
            vm.button2 = button2;
            console.info(vm.button2);

            UIControlService.loadLoading("LOADERS.LOADING");
            VendorRegistrationService.selectQuestionnaire({Keyword: vm.currentLang},
               function (reply) {
                   vm.data = [];
                   vm.type = [];
                   //console.info("data:"+JSON.stringify(reply));
                   UIControlService.unloadLoading();
                   if (reply.status === 200) {
                       vm.list = reply.data;
                       if (vm.vendorQues.length !== 0) {
                           vm.flag = 1;
                           for (var i = 0; i < vm.vendorQues.length; i++) {
                               
                               for (var j = 0; j < vm.list[i].type.length; j++) {
                                   var calldateType = {
                                       ID: vm.list[i].type[j].ID,
                                       question: vm.list[i].type[j].question
                                   }
                                   vm.type.push(calldateType);
                               }
                               vm.calldata = {
                                   ID: vm.list[i].ID,
                                   question: vm.list[i].question,
                                   DetailId: vm.list[i].DetailId,

                                   Value: vm.vendorQues[i].VendQuesDetailId,
                                   Description: vm.vendorQues[i].Description,
                                   type: vm.type
                               }
                               vm.data.push(vm.calldata);
                               vm.type = []; 
                               if (i == vm.vendorQues.length - 1) {
                                   var reenterButton = document.getElementById(vm.button);
                                   var reenterButton2 = document.getElementById(vm.button2);
                                   var divInfo = document.getElementById(vm.divInfo);
                                   var divInfo1 = document.getElementById(vm.divInfo1);
                                   //divInfo1.style.visibility = 'visible';
                                  // reenterButton.style.visibility = 'hidden';
                                   //divInfo.style.visibility = 'visible';
                                   $timeout(function () {
                                       window.print();
                                       vm.flag = 2;
                                   }, 3000);

                                   reenterButton.style.visibility = 'visible';
                                   divInfo.style.visibility = 'hidden';
                                   divInfo1.style.visibility = 'hidden';
                               }
                           }
                       }
                       else {
                           var divInfo = document.getElementById(vm.divInfo);
                           divInfo.style.visibility = 'hidden';
                           var divInfo1 = document.getElementById(vm.divInfo1);
                           divInfo1.style.visibility = 'hidden';

                           vm.data = vm.list;
                       }
                   } else {
                       $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
                       UIControlService.unloadLoading();
                   }
               }, function (err) {
                   console.info("error:" + JSON.stringify(err));
                   //$.growl.error({ message: "Gagal Akses API >" + err });
                   UIControlService.unloadLoading();
               });
            }

        vm.save = save;
        function save() {
            var questionaire = [];
            console.info(vm.data);
            for (var i = 0; i < vm.data.length; i++) {
                var data = {
                    Value: vm.data[i].Value,
                    VendorID: Number(localStorage.getItem('vendor_reg_id')),
                    Description: vm.data[i].Description
                }
                questionaire.push(data);
            }
            
          console.info(JSON.stringify(questionaire));
            UIControlService.loadLoading("LOADERS.LOADING_SAVE_QUESTIONAIRE");
            VendorRegistrationService.saveQuestionaire(questionaire,
                function (response) {
                    UIControlService.unloadLoading();
                    vm.flag = 2;
                    UIControlService.msg_growl("notice", 'KUESIONER.SAVE');
                    window.location.reload();
                    
                    // $state.go('login-rekanan');
                },
                function (response) {
                    UIControlService.handleRequestError(response.data);
                    UIControlService.unloadLoading();
                    //$state.go('login-rekanan');
                });
        }

        vm.upload = upload;
        function upload() {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/visitor/daftar/formUploadQuestionnaire.html',
                controller: "FormQuestionnaireCtrl",
                controllerAs: "FormQuestionnaireCtrl"
            });
            modalInstance.result.then(function () {
                sendEmail();
            });
               
        }
        
        vm.loadCek = loadCek;
        function loadCek(button2, printableArea, button, divInfo, divInfo1) {
            VendorRegistrationService.CekVendor(
                {
                    VendorID: Number(localStorage.getItem('vendor_reg_id'))
                }, function (reply) {
                    //console.info("data:"+JSON.stringify(reply));
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        vm.vendorQues = reply.data;
                        vm.jload(button2, printableArea, button, divInfo, divInfo1);
                    } else {
                        $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                    //$.growl.error({ message: "Gagal Akses API >" + err });
                    UIControlService.unloadLoading();
                });
        }

        function sendEmail() {
            MailerService.getMailContent({
                Email: 'Pendaftaran Rekanan Baru',
                Username: vm.username,
                Name: vm.VendorName
            }, function (response) {
                if (response.status == 200) {
                    var email = {
                        subject: response.data.Subject,
                        //mailContent: 'Terima kasih telah mendaftar di e-Procurement. Kami akan melakukan verifikasi terlebih dahulu terhadap data dan dokumen pendaftaran anda.',
                        mailContent: response.data.MailContent,
                        isHtml: true,
                        addresses: [vm.email]
                    };

                    UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
                    VendorRegistrationService.sendMail(email, function (response) {
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            UIControlService.msg_growl("notice", "Email Sent!");
                            $state.go('login-panitia');
                        } else {
                            UIControlService.handleRequestError(response.data);
                        }
                        
                    }, function (response) {
                        UIControlService.handleRequestError(response.data);
                        UIControlService.unloadLoading();
                        //$state.go('daftar_kuesioner');
                    });
                } else {
                    UIControlService.handleRequestError(response.data);
                }
            }, function (response) {
                UIControlService.handleRequestError(response.data);
                UIControlService.unloadLoading();
            });
        }

    }
})();
