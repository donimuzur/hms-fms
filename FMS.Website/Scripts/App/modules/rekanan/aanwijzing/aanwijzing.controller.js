(function () {
    'use strict';

    angular.module("app").controller("AanwijzingVendorCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'AanwijzingService', '$state', 'UIControlService', 'UploadFileConfigService',
        'UploaderService', 'GlobalConstantService', '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        AanwijzingService, $state, UIControlService, UploadFileConfigService,
        UploaderService, GlobalConstantService, $uibModal, $stateParams) {
        var vm = this;
        vm.IDTender = Number($stateParams.TenderRefID);
        vm.IDStepTender = Number($stateParams.StepID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.online = true;
        vm.TenderName = '';
        vm.TenderCode = '';
        vm.StartDate = null;
        vm.EndDate = null;
        vm.nama_tahapan = '';
        vm.is_created = false;
        vm.totalItems = 0;
        vm.maxSize = 5;
        vm.pertanyaan = [];
        vm.IsOpenAwj = false;
        vm.TypeAanwijzing = '';
        vm.listPertanyaan = [];
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.currentPage = 1;
        vm.today = UIControlService.getStrDate(new Date());

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("aanwijzing");
            loadAdminPost();
            loadDataTender();
            //jLoad(0);
        }

        function loadAdminPost() {
            AanwijzingService.getAdminPostByStepForVendor({
                ID: vm.IDStepTender
            }, function (reply) {
                if (reply.status === 200) {
                    vm.adminPost = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal mendapatkan data pesan admin");
            });
        }

        function loadDataTender() {
            AanwijzingService.getDataStepTender({
                ID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    console.info("tenderVend:" + JSON.stringify(data));
                    var startDate = data.StartDate;
                    var endDate = data.EndDate;
                    checkStepDate(startDate,endDate);
                    vm.TenderName = data.tender.TenderName;
                    vm.StartDate = UIControlService.getStrDate(data.StartDate);
                    vm.EndDate = UIControlService.getStrDate(data.EndDate);
                    vm.TenderID = data.TenderID;
                    vm.nama_tahapan = data.step.TenderStepName;
                    //console.info("tender::" + JSON.stringify(data));
                    loadDataAanwijzing();
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal mendapatkan data tender");
                UIControlService.unloadLoading();
            });
        }


        function checkStepDate(startDate, endDate) {
            var dateNow = new Date();
            var startDate = new Date(startDate);
            var endDate = new Date(endDate);
            vm.inProcess = false;
            if (dateNow >= startDate && dateNow <= endDate) {
                vm.inProcess = true;
            }
        }

        function loadDataAanwijzing() {
            AanwijzingService.getDataAanwijzingByTender({
                TenderID: vm.TenderID, TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    console.info("aanwijz vendor:" + JSON.stringify(data));
                    vm.dataAturAanwijzing = data;
                    if (!(data === null)) {
                        vm.IsOpenAwj = true;
                        vm.EntryStartDate = UIControlService.getStrDate(data.StartDateVendorEntry);
                        vm.EntryEndDate = UIControlService.getStrDate(data.EndDateVendorEntry);
                        vm.TypeAanwijzing = data.TypeAaanwijzing.Value;
                        vm.jLoad(1);
                    }
                    console.info(JSON.stringify(vm.dataAturAanwijzing));
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal mendapatkan data aanwijzing");
                UIControlService.unloadLoading();
            });
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * 5) - 5;

            AanwijzingService.getPostingByVendor({
                FilterType: vm.dataAturAanwijzing.ID, Offset : offset, Limit: vm.maxSize
            }, function (reply) {
                //console.info("datane:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listPertanyaan = data.List;
                    for (var i = 0; i < vm.listPertanyaan.length; i++) {
                        vm.listPertanyaan[i].QuestionDate = UIControlService.getStrDate(vm.listPertanyaan[i].QuestionDate);
                    }
                    vm.totalItems = data.Count;
                } else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan data Master Badan Usaha");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "Gagal mendapatkan data Master Badan Usaha");
                UIControlService.unloadLoading();
            });
        }

        vm.postingPertanyaan = postingPertanyaan;
        function postingPertanyaan() {
            if (vm.today > vm.dataAturAanwijzing.EndDateVendorEntry) {
                UIControlService.msg_growl("error", "Posting Aanwijzing sudah ditutup");
                return;
            }
            var data = {
                IDAwj: vm.dataAturAanwijzing.ID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/aanwijzing/postingAanwijzing.html',
                controller: 'PostingAanwijzingCtrl',
                controllerAs: 'PostAwjCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.init();
            });
        }
    }
})();