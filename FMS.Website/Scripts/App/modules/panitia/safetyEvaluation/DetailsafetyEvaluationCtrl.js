(function () {
    'use strict';

    angular.module("app").controller("DetailEvaluationSafetyCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvalSafetyService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvalSafetyService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        var page_id = 141;
        vm.StepID = Number($stateParams.StepID);
        vm.VendorID = Number($stateParams.VendorID);
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
        //vm.loadDepartemen = loadDepartemen;
        vm.cariEvalSafety = cariEvalSafety;
        vm.jLoad = jLoad;
        vm.ProcPackageType = 0;
        vm.TenderRefID = 0;

        vm.question_list =
        [
            {
                id: "Apakah diagram organisasi yang ditawarkan secara khusus untuk proyek yang sedang di tenderkan telah di lampiran?"
            },
            {
                id: "Apakah tugas dan tanggung jawab orang-orang yang terlibat dalam proyek di jabarkan dengan jelas?"
            },
            {
                id: "Apakah kontraktor telah menunjuk seorang penanggung jawab operasional yang bertanggung jawab terhadap implementasi kebijakan EHS?"
            },
            {
                id: "Apakah orang yang berperan sebagai penanggung jawab operasional memiliki sertifikat POP dan/atau dasar2 K3 Pertambangan?"
            },
            {
                id: "Apakah daftar alat sebagaimana yang di minta dalam document tender telah dicantumkan?"
            },
            {
                id: "Apakah pekerja kontraktor memiliki sertifikat kompetensi dari pemerintah sebagaimana yang diminta dalam dokument tender?"
            },
            {
                id: "Apakah equipment yang ditawarkan dalam proposal tender masih layak operasi?"
            }

        ];
        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            loadStep();

        }
        vm.loadStep = loadStep;
        function loadStep() {

            vm.questions = [];
            //console.info("curr "+current)
            vm.tender = [];
            var tender = {
                ID: vm.StepID
            };
            EvalSafetyService.getStep(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.tender = reply.data;
                    vm.TenderRefID = vm.tender.tender.TenderRefID;
                    vm.ProcPackageType = vm.tender.tender.ProcPackageType;
                    loadSafe();
                    jLoad(vm.TenderRefID, vm.ProcPackageType);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evalasi Safety" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.cariEvalSafety = cariEvalSafety;
        function cariEvalSafety() {
            vm.jLoad(1);
        }

        vm.loadSafe = loadSafe;
        function loadSafe() {
            //console.info("curr "+current)
            vm.safety = [];
            var tender = {
                ID: vm.StepID,
                tender: {
                    TenderRefID: vm.TenderRefID,
                    ProcPackageType: vm.ProcPackageType
                }
            }
            EvalSafetyService.select(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.safety = reply.data;
                    console.info("safe:" + JSON.stringify(vm.safety));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evalasi Safety" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.jLoad = jLoad;
        function jLoad(ref, type) {
            //console.info("curr "+current)
            console.info(JSON.stringify(vm.ProcPackageType));
            vm.evalsafety = [];
            var tender = {
                Status: ref,
                FilterType: type,
                column: vm.VendorID
            }
            EvalSafetyService.selectDetail(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.evalsafety = reply.data;
                    console.info(JSON.stringify(vm.evalsafety));
                    if(vm.evalsafety.length !== 0){
                        for (var i = 0; i < vm.question_list.length; i++) {
                            for (var y = 0; y < vm.evalsafety.length; y++) {
                                if (i === ((vm.evalsafety[y].Question) - 1))
                                    vm.questions.push({
                                        ID:vm.evalsafety[y].ID,
                                        EvaluationSafetyID: vm.evalsafety[y].EvaluationSafetyID,
                                        no: Number(i + 1),
                                        question: vm.question_list[i].id,
                                        IsValid: vm.evalsafety[i].IsValid,
                                        Description: vm.evalsafety[i].Description,
                                        safety: {
                                            Status: vm.evalsafety[y].safety.Status,
                                            TenderStepDataID: vm.StepID,
                                            VendorID: vm.VendorID
                                        }
                                    });
                            }

                        }
                    }
                    else{
                        for (var i = 0; i < vm.question_list.length; i++) {
                            vm.questions.push({
                                    EvaluationSafetyID:0,
                                    no: Number(i + 1),
                                    question: vm.question_list[i].id,
                                    IsValid: false,
                                    Description: "",
                                    safety: {
                                        Status: 0,
                                        TenderStepDataID: vm.StepID,
                                        VendorID: vm.VendorID
                                    }
                                });
                        }
                    }
                    console.info(JSON.stringify(vm.questions));
                    
                    
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evalasi Safety" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.simpan = simpan;
        function simpan(data) {

            vm.list = [];
            for (var i = 0; i < vm.questions.length; i++) {
                vm.list.push({
                    ID: vm.questions[i].ID,
                    EvaluationSafetyID: vm.questions[i].EvaluationSafetyID,
                    Question: i+1,
                    IsValid: vm.questions[i].IsValid,
                    Description: vm.questions[i].Description,
                    safety: {
                        Status: vm.questions[i].safety.Status,
                        TenderStepDataID: vm.StepID,
                        VendorID: vm.VendorID
                    }
                });
            }
            console.info(JSON.stringify(vm.list));
            EvalSafetyService.InsertDetail(vm.list,
               function (reply) {
                   UIControlService.unloadLoadingModal();
                   if (reply.status === 200) {
                       UIControlService.msg_growl("success", "Berhasil Simpan data");
                       init();
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

        vm.kembali = kembali;
        function kembali() {
            $state.transitionTo('evaluasi-safety', { TenderRefID: vm.TenderRefID, StepID: vm.StepID, ProcPackType: vm.ProcPackageType});
        }



    }
})();
//TODO

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

