(function () {
	'use strict';

	angular.module("app").controller("PurchaseRequisitionCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PurchaseRequisitionService', '$state', 'UIControlService', '$uibModal'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PurchReqService,
        $state, UIControlService, $uibModal) {
		var vm = this;
		vm.maxSize = 10;
		vm.currentPage = 1;
		vm.textKeyword = '';
		vm.colKeyword = 1;
		vm.PRList = [];
		vm.totalItems = 0;

		vm.rfqGoodsApprvls = [];
		vm.totalItemsApprvls = 0;

		vm.init = init();
		function init() {
			$translatePartialLoader.addPart("purchase-requisition");
			jLoad(vm.currentPage);
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			UIControlService.loadLoading("Silahkan Tunggu...");
			vm.currentPage = current;
			var offset = (current * 10) - 10;

			PurchReqService.selectDataPR({
				Offset: offset,
				Limit: vm.maxSize,
				Keyword: vm.textKeyword,
				column: Number(vm.colKeyword)
			}, function (reply) {
				///console.info("datane:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.PRList = data.List;
					vm.totalItems = Number(data.Count);
				} else {
					UIControlService.msg_growl("error", "Gagal Mendapatkan Data Freight");
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));
				UIControlService.msg_growl("error", "Gagal akses API");
				UIControlService.unloadLoading();
			});
		}

		vm.menujuApproval = menujuApproval;
		function menujuApproval() {
			$state.transitionTo('rfqGoods-draft-approval');
		}

		vm.formViewRFQ = formViewRFQ;
		function formViewRFQ(ID) {
			$state.transitionTo('purchase-requisition-formviewrfq', { RFQID: ID });
		}

		vm.formInputRFQ = formInputRFQ;
		function formInputRFQ(ID) {
			$state.transitionTo('purchase-requisition-formrfq', { RFQID: ID });
		}

		vm.detailRFQ = detailRFQ;
		function detailRFQ(ID) {
			$state.transitionTo('purchase-requisition-detailrfq', { RFQID: ID });
		}

		vm.formPROutStanding = formPROutStanding;
		function formPROutStanding() {
			$state.transitionTo('purcreq-pr-outstanding');
		}

		vm.getApprovalData = getApprovalData;
		function getApprovalData() {
			$translatePartialLoader.addPart("purchase-requisition");
			//UIControlService.loadLoading('LOADING.GETRFQ.MESSAGE');
			PurchReqService.getApprovalData({
				Offset: (vm.currentPage - 1) * vm.maxSize,
				Limit: vm.maxSize,
				Keyword: vm.keyword,
				Column: vm.column
			}, function (reply) {
				if (reply.status === 200) {
					vm.rfqGoodsApprvls = reply.data.List;
					vm.totalItemsApprvls = reply.data.Count;
					UIControlService.unloadLoading();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.GETRFQ.ERROR', "NOTIFICATION.GETRFQ.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.GETRFQ.ERROR', "NOTIFICATION.GETRFQ.TITLE");
			});
		}

		/* tambah vendor */
		vm.openFormDokumen = openFormDokumen;
		function openFormDokumen(isAdd, data) {
			var senddata = { data: data, isAdd: isAdd };
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/purchaseRequisition/dataRFQ/uploadDokumen.html',
				controller: 'UploadDokumenCtrl',
				controllerAs: 'uploadDokCtrl',
				resolve: {
					item: function () {
						return senddata;
					}
				}
			});
			modalInstance.result.then(function (dataVendor) {
				vm.jLoad(1);
			});
		}

		//send approval
		vm.sendApproval = sendApproval;
		function sendApproval(data) {
			PurchReqService.sendApproval({ ID: data.ID }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", "Berhasil Kirim Approval");
					vm.jLoad(1);
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

		//send approval
		vm.Publish = Publish;
		function Publish(data) {
		   //loadDataTender(data);
			PurchReqService.Publish({ ID: data.ID }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", "Berhasil Publish RFQ");
					loadDataTender(data);
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

		vm.DetailApproval = DetailApproval;
		function DetailApproval(data, flag) {
		    var data = {
		        RFQID: data,
		        Status: flag
		    }
		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/panitia/master/purchaseRequisition/dataRFQ/detailApproval.modal.html',
		        controller: 'detailApprovalCtrl',
		        controllerAs: 'detailApprovalCtrl',
		        resolve: {
		            item: function () {
		                return data;
		            }
		        }
		    });
		    modalInstance.result.then(function (dataVendor) {
		        window.location.reload();
		    });
		}

		function loadDataTender(data) {
		    vm.IdGoods = data.ID;
		    vm.Email = [];
		    PurchReqService.getDataGoods({
		        ID: data.ID
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            var data = reply.data;
		            vm.dataGoods = data;
		            console.info(vm.dataGoods);
		            if (vm.dataGoods.Emails != null) {
		                var dataemail = vm.dataGoods.Emails.split(',');
		                for (var i = 0; i < dataemail.length; i++) {
		                    vm.Email.push(dataemail[i]);
		                }
		            }
		            if (vm.dataGoods.contactVendorComm != null) {
		                for (var i = 0; i < vm.dataGoods.contactVendorComm.length; i++) {
		                    vm.Email.push(vm.dataGoods.contactVendorComm[i].Email);
		                    if (i == (vm.dataGoods.contactVendorComm.length - 1)) {
		                            sendEmail();
		                    }
		                }
		            }
		            else {
		                if (vm.dataGoods.contactVendor != null) {
		                    for (var i = 0; i < vm.dataGoods.contactVendor.length; i++) {
		                        vm.Email.push(vm.dataGoods.contactVendor[i].Email);
		                        if (i == (vm.dataGoods.contactVendor.length - 1)) {
		                            sendEmail();
		                        }
		                    }
		                }
		            }
		            
		           

		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.API");
		        UIControlService.unloadLoading();
		    });
		}

		vm.sendEmail = sendEmail;
		function sendEmail() {
		    PurchReqService.getMailContent({
		        EmailContent: 'Pengumuman Tender',
		        TenderName: vm.dataGoods.RFQName
		    }, function (response) {
		        if (response.status == 200) {
		            var email = {
		                subject: response.data.Subject,
		                mailContent: response.data.MailContent,
		                isHtml: true,
		                addresses: vm.Email
		            };

		            UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
		            PurchReqService.sendMail(email, function (response) {
		                UIControlService.unloadLoading();
		                if (response.status == 200) {
		                    UIControlService.msg_growl("notice", "Email Sent!");
		                    init();
		                    //GetApprovalGoods();
		                } else {
		                    UIControlService.handleRequestError(response.data);
		                }
		            }, function (response) {
		                UIControlService.handleRequestError(response.data);
		                UIControlService.unloadLoading();
		            });
		        } else {
		            UIControlService.handleRequestError(response.data);
		        }
		    }, function (response) {
		        UIControlService.handleRequestError(response.data);
		        UIControlService.unloadLoading();
		    });
		}

		vm.GetApprovalGoods = GetApprovalGoods;
		function GetApprovalGoods() {
		    vm.EmailReviewer = [];
		    vm.EmailRequestor = [];
		    PurchReqService.GetApproval({
		        ID: vm.IdGoods
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.list = reply.data;
		            if (vm.list.length != 0) {
		                vm.EmailRequestor.push(vm.list[0].requestor.Email);
		                for (var i = 0; i < vm.list.length; i++) {
		                    if (i == 0) vm.NameReviewer = vm.list[i].employee.FullName + ' ' + vm.list[i].employee.SurName;
		                    else vm.NameReviewer += ', ' + vm.list[i].employee.FullName + ' ' + vm.list[i].employee.SurName;
		                    vm.EmailReviewer.push(vm.list[i].employee.Email);
		                    if (i == (vm.list.length - 1)) {
		                        sendMailCE();
		                    }
		                }
		            }
		        } else {
		            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
		        }
		    }, function (error) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
		    });
		}

		vm.sendMailCE = sendMailCE;
		function sendMailCE() {
		    ContractEngineerService.getMailContent({
		        EmailContent: 'Notifikasi Contract Engineer',
		        TenderName: vm.dataGoods.RFQName,
		    }, function (response) {
		        if (response.status == 200) {
		            var email = {
		                subject: response.data.Subject,
		                mailContent: response.data.MailContent,
		                isHtml: true,
		                addresses: vm.EmailReviewer
		            };

		            UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
		            ContractEngineerService.sendMail(email, function (response) {
		                UIControlService.unloadLoading();
		                if (response.status == 200) {
		                    UIControlService.msg_growl("notice", "Email Sent!");
		                    sendEmailRequestor();
		                } else {
		                    UIControlService.handleRequestError(response.data);
		                }
		            }, function (response) {
		                UIControlService.handleRequestError(response.data);
		                UIControlService.unloadLoading();
		            });
		        } else {
		            UIControlService.handleRequestError(response.data);
		        }
		    }, function (response) {
		        UIControlService.handleRequestError(response.data);
		        UIControlService.unloadLoading();
		    });
		}

		vm.sendEmailRequestor = sendEmailRequestor;
		function sendEmailRequestor() {
		    ContractEngineerService.getMailContent1({
		        EmailContent: 'Notifikasi Requestor pengadaan',
		        TenderName: vm.project,
		        VendorName: vm.NameReviewer
		    }, function (response) {
		        if (response.status == 200) {
		            var email = {
		                subject: response.data.Subject,
		                mailContent: response.data.MailContent,
		                isHtml: true,
		                addresses: vm.EmailRequestor
		            };

		            UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
		            ContractEngineerService.sendMail(email, function (response) {
		                UIControlService.unloadLoading();
		                if (response.status == 200) {
		                    UIControlService.msg_growl("notice", "Email Sent!");
		                    init();
		                } else {
		                    UIControlService.handleRequestError(response.data);
		                }
		            }, function (response) {
		                UIControlService.handleRequestError(response.data);
		                UIControlService.unloadLoading();
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