(function () {
    'use strict';

    angular.module("app").controller("AturVendorAanwijzingCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'AanwijzingService', '$state', 'UIControlService', 'UploadFileConfigService',
        'UploaderService', 'GlobalConstantService', '$uibModal', '$stateParams', 'item', '$uibModalInstance', ];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        AanwijzingService, $state, UIControlService, UploadFileConfigService,
        UploaderService, GlobalConstantService, $uibModal, $stateParams, item, $uibModalInstance) {
        var vm = this;
        vm.TenderStepID = item.TenderStepID;
        vm.TenderID = item.TenderID;
        vm.ProcPackType = item.ProcPackType;
        vm.TenderRefID = item.TenderRefID;
        vm.fileUpload;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("aanwijzing");
            loadDataTender();
        }

        function loadDataTender() {
            AanwijzingService.SelectVendorTender({
                StepID: vm.TenderStepID,
                ProcPackageType: vm.ProcPackType,
                TenderRefID: vm.TenderRefID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.register = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
        
        vm.save = save;
        function save() {
            vm.dataSimpan = [];
            for (var i = 0; i < vm.register.length; i++) {
                if (vm.register[i].IsSurvive === undefined) vm.register[i].IsSurvive = false;
                vm.dataSimpan.push({
                    TenderID: vm.TenderID,
                    TenderStepID: vm.TenderStepID,
                    VendorId: vm.register[i].VendorID,
                    IsCheck: vm.register[i].IsSurvive
                });
            }
            AanwijzingService.updateVendorAanwijzing(vm.dataSimpan, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil simpan pertanyaan aanwijzing");
                    $uibModalInstance.close();
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

        function processsave(url) {
			if(url === null){		
					url = " ";		
				}		
						
			var keterangan = vm.Remark.replace(/\n/g, '<br/>');
            var datasimpan = {
                TitleSummary: vm.title,
                Remark: keterangan,
                FileDocument: url,
                ID: vm.IDAwj
            }
            
        }
		
		vm.getListPertanyaan = getListPertanyaan;
		function getListPertanyaan(){
			
			vm.daftarQuest = vm.daftarPertanyaan.map(function (daftarPertanyaan){
				return {
					Judul : daftarPertanyaan.QuestionTitle,
					Pertanyaan : daftarPertanyaan.Question,
					NamaVendor : daftarPertanyaan.vendorPosted.VendorName
				}
			});
			vm.Remark = '';
			vm.daftarQuest.forEach(function(sub,key){
				var a = key+1;
				vm.Remark += a +'. Judul : '+ sub.Judul+'Oleh :'+sub.NamaVendor+ '\n';
				vm.Remark += 'Pertanyaan :'+sub.Pertanyaan;
				vm.Remark += '\n';
				vm.Remark += 'Jawaban :';
				vm.Remark += '\n';
				vm.Remark += '\n';
			});
			
			console.info(vm.Remark);
		}

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();