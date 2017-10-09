(function () {
	'use strict';

	angular.module("app").controller("AanwijzingCtrl", ctrl);

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
		vm.aanwijzingStepId = 0;
		//console.info("ten:" + vm.IDTender + ":" + vm.IDStepTender);
		vm.online = true;
		vm.TenderID;
		vm.TenderName = '';
		vm.TenderCode = '';
		vm.StartDate = null;
		vm.EndDate = null;
		vm.nama_tahapan = '';
		vm.is_created = false;
		vm.totalItems = 0;
		vm.maxSize = 10;
		vm.pertanyaan = [];
		vm.IsAtur = false;
		//vm.AnswerStartDate = null;
		//vm.AnswerEndDate = null;
		vm.TypeAanwijzing = '';
		vm.TimeToAnswer = false;
		vm.dataAturAanwijzing = null;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.adminPost = null;

		vm.init = init;
		function init() {
		    $translatePartialLoader.addPart("aanwijzing");
		    loadAdminPost();
			loadDataTender();
		}

		function loadAdminPost() {
		    AanwijzingService.getAdminPostByStep({
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
		    UIControlService.loadLoading("Silahkan Tunggu...");
			AanwijzingService.getDataStepTender({
				ID: vm.IDStepTender
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.TenderName = data.tender.TenderName;
					vm.StartDate = UIControlService.getStrDate(data.StartDate);
					vm.EndDate = UIControlService.getStrDate(data.EndDate);
					vm.nama_tahapan = data.step.TenderStepName;
					vm.TenderID = data.TenderID;
					//console.info("tenderID:" + vm.TenderID);
					loadDataAanwijzing();
					//console.info("tender::" + JSON.stringify(data));
				}
			}, function (err) {
			    UIControlService.msg_growl("error", "Gagal mendapatkan data tender");
				UIControlService.unloadLoading();
			});
		}

		function loadDataAanwijzing() {
		    UIControlService.loadLoading("Silahkan Tunggu...");
			AanwijzingService.getDataAanwijzingByTender({
				TenderID: vm.TenderID, TenderStepID: vm.IDStepTender
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.dataAturAanwijzing = data;
					//console.info("aan: " + JSON.stringify(data));
					if (!(data === null)) {
						vm.IsAtur = true;
						vm.AnswerStartDate = data.AnswerStartDate;
						vm.AnswerEndDate = data.AnswerEndDate;
						vm.TypeAanwijzing = data.TypeAaanwijzing.Value;
						//console.info(new Date(Date.parse(vm.AnswerStartDate)) + ">>" + "==" + UIControlService.getStrDate(new Date()));
						if (new Date(Date.parse(vm.AnswerStartDate)) == new Date()) {
							vm.TimeToAnswer === true;
						}
						if (!(vm.dataAturAanwijzing.PostedDate === null)) {
							vm.dataAturAanwijzing.PostedDate = UIControlService.getStrDate(vm.dataAturAanwijzing.PostedDate);
						}
						vm.jLoad(1);
					}
				}
			}, function (err) {
			    UIControlService.msg_growl("error", "Gagal mendapatkan data pre-bid");
				UIControlService.unloadLoading();
			});
		}


		vm.backpengadaan = backpengadaan;
		function backpengadaan() {
			$state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
		}

		vm.listPertanyaan = [];
		vm.jLoad = jLoad;
		function jLoad(current) {
			UIControlService.loadLoading("Silahkan Tunggu...");
			vm.currentPage = current;
			var offset = (current * vm.maxSize) - vm.maxSize;
			//console.info("idnee:" + vm.dataAturAanwijzing.ID);
			AanwijzingService.getDataQuestions({
				FilterType: vm.dataAturAanwijzing.ID, Offset: offset, Limit: vm.maxSize
			}, function (reply) {
				//console.info("dataQue:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.aanwijzingStepId = data.List[0].AanwijzingStepID;
					vm.listPertanyaan = data.List;
					for (var i = 0; i < vm.listPertanyaan.length; i++) {
						vm.listPertanyaan[i].QuestionDate = UIControlService.getStrDate(vm.listPertanyaan[i].QuestionDate);
					}
					vm.totalItems = data.Count;
					vm.loadAllQuestion();
				} else {
					UIControlService.msg_growl("error", "Gagal mendapatkan data Pertanyaan");
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));
			    UIControlService.msg_growl("error", "Gagal mendapatkan data Pertanyaan");
				UIControlService.unloadLoading();
			});
		}

		vm.daftarPertanyaan = [];
		vm.loadAllQuestion = loadAllQuestion;
		function loadAllQuestion() {
			AanwijzingService.getDataQuestions({
				FilterType: vm.dataAturAanwijzing.ID, Offset: 0, Limit: vm.totalItems
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.daftarPertanyaan = data.List;
					for (var i = 0; i < vm.daftarPertanyaan.length; i++) {
						vm.daftarPertanyaan[i].QuestionDate = UIControlService.getStrDate(vm.daftarPertanyaan[i].QuestionDate);
					}
				} else {
					UIControlService.msg_growl("error", "Gagal mendapatkan data Pertanyaan");
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));		
			    UIControlService.msg_growl("error", "Gagal mendapatkan data Pertanyaan");
				UIControlService.unloadLoading();
			});
		}

		vm.postingInformasi = postingInformasi;
		function postingInformasi() {

		    var item = null;
		    if (vm.adminPost) {
		        item = vm.adminPost;
		    } else {
		        item = {
		            Title: "",
		            Post: "",
		            UploadURL: "",
		            AanwijzingStepID: vm.dataAturAanwijzing.ID,
		        };
		    }

		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/panitia/aanwijzing/postingInfo.html',
		        controller: 'PostingInfoCtrl',
		        controllerAs: 'PostInfoCtrl',
		        resolve: {
		            item: function () {
		                return item;
		            }
		        }
		    });
		    modalInstance.result.then(function () {
		        vm.init();
		    });
		}

		vm.postingPertanyaan = postingPertanyaan;
		function postingPertanyaan(data) {
			var data = {
				daftarPertanyaan: vm.daftarPertanyaan,
				ID: vm.dataAturAanwijzing.ID
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/aanwijzing/postingAanwijzing.html',
				controller: 'PostingSummaryAanwijzingCtrl',
				controllerAs: 'PostSummaryAwjCtrl',
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

		vm.aturAanwijzing = aturAanwijzing;
		function aturAanwijzing(data, isAtur) {
			//console.info(vm.TenderName + vm.TenderID + vm.IDStepTender);
			var senddata = {
				TenderName: vm.TenderName,
				AanwijzingStepID: vm.aanwijzingStepId,
				TenderID: vm.TenderID,
				TenderStepID: vm.IDStepTender,
				StartDateStep: vm.StartDate,
				EndDateStep: vm.EndDate,
				IsAtur: isAtur,
				data: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/aanwijzing/aturAanwijzing.html',
				controller: 'AturAanwijzingCtrl',
				controllerAs: 'AturAwjCtrl',
				resolve: { item: function () { return senddata; } }
			});
			modalInstance.result.then(function () {
				vm.init();
			});
		}

		vm.aturVendorAanwijzing = aturVendorAanwijzing;
		function aturVendorAanwijzing() {
		    var senddata = {
		        TenderRefID: vm.IDTender,
                ProcPackType: vm.ProcPackType,
		        TenderID: vm.TenderID,
		        TenderStepID: vm.IDStepTender
		    }
		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/panitia/aanwijzing/aturVendorAanwijzing.html',
		        controller: 'AturVendorAanwijzingCtrl',
		        controllerAs: 'AturVendorAanwijzingCtrl',
		        resolve: { item: function () { return senddata; } }
		    });
		    modalInstance.result.then(function () {
		        vm.init();
		    });
		}
	}
})();