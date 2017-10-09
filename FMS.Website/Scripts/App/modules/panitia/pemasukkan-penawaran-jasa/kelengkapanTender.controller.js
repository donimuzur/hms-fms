(function () {
    'use strict';

    angular.module("app").controller("KelengkapanTenderCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'OfferEntryService', '$state', 'UIControlService', '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, OEService,
        $state, UIControlService, $uibModal, $stateParams) {
        var vm = this;
        vm.IDTender = Number($stateParams.TenderRefID);
        vm.IDStepTender = Number($stateParams.StepID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.listKelengkapan = {};
        vm.TenderName = '';
        vm.StartDate = null;
        vm.EndDate = null;
        vm.listKelengkapanKomersial = [];

        vm.init = init;
        function init() {
            UIControlService.loadLoading("Silahkan Tunggu");
            OEService.getKelengkapanTender({
                TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    //console.info(">>:" + JSON.stringify(data));
                    vm.listKelengkapan = data.ServiceOfferEntryDocuments;
                    vm.listKelengkapanKomersial = data.ServiceOfferEntryChecklists;
                    vm.TenderName = data.TenderName;
                    vm.StartDate = data.StartDate;
                    vm.EndDate = data.EndDate;
                    saveKelengkapan(data);
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
        function saveKelengkapan(data) {
            OEService.saveKelengkapanTender(data, function (reply) {
                if (reply.status === 200) {
                	//UIControlService.msg_growl("success", "Update Data");
                	OEService.getKelengkapanTender({
                		TenderStepID: vm.IDStepTender
                	}, function (reply) {
                		UIControlService.unloadLoading();
                		if (reply.status === 200) {
                			var data = reply.data;
                			//console.info(">>:" + JSON.stringify(data));
                			vm.listKelengkapan = data.ServiceOfferEntryDocuments;
                			vm.listKelengkapanKomersial = data.ServiceOfferEntryChecklists;
                		}
                	}, function (err) {
                		UIControlService.msg_growl("error", "MESSAGE.API");
                		UIControlService.unloadLoading();
                	});
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
            });
        }

        vm.backDetailTahapan = backDetailTahapan;
        function backDetailTahapan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
        }

        vm.backTahapan = backTahapan;
        function backTahapan() {
            $state.transitionTo('pemasukkan-penawaran-jasa', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType, StepID: vm.IDStepTender });
        }

        vm.detailKelengkapan = detailKelengkapan;
        function detailKelengkapan(data) {
            var ischeck = false;
            if (data.DocumentType === 'FORM_DOCUMENT') {
                ischeck = true;
            }
            var datasend = {
                IDTender: vm.IDTender,
                TenderName: vm.TenderName,
                StartDate: vm.StartDate,
                EndDate: vm.EndDate,
                dataDoc: data,
                listCheck: vm.listKelengkapanKomersial,
                ischeck: ischeck
            }
            if (!(data.DocumentType === 'FORM_DOCUMENT')) {
                var modalInstance = $uibModal.open({
                    templateUrl: 'upload-dokumen.html',
                    controller: 'UploadDokumenJasaCtrl',
                    controllerAs: 'uploadDokJasaCtrl',
                    resolve: {
                        item: function () {
                            return datasend;
                        }
                    }
                });
                modalInstance.result.then(function () {
                    vm.init();
                });
            } else {
                //hanya untuk komersial
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/modules/panitia/pemasukkan-penawaran-jasa/kelengkapan-datakomersial.html',
                    controller: 'KelengkapanDataKomersialCtrl',
                    controllerAs: 'KDKomersialCtrl',
                    resolve: {
                        item: function () {
                            return datasend;
                        }
                    }
                });
                modalInstance.result.then(function () {
                    vm.init();
                });
            }
        }

        vm.updateChecklist = updateChecklist;
        function updateChecklist(data){             
            var savedata = {
                ID: data.ID,
                IsRequired: data.IsRequired,
                DocumentURL: data.DocumentURL
            }
            OEService.updateDocs(savedata, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Update Data");
                    vm.init();
                }
                else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

    }
})();