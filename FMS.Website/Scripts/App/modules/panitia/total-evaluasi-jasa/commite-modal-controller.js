(function () {
    'use strict';

    angular.module("app").controller("CommitteeModalCtrl", ctrl);
    ctrl.$inject = ['$filter','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'UIControlService', 'item', '$uibModalInstance', '$uibModal', 'GlobalConstantService', 'TotalEvaluasiJasaService'];
    /* @ngInject */
    function ctrl($filter,$http, $translate, $translatePartialLoader, $location, SocketService,
        UIControlService, item, $uibModalInstance, $uibModal, GlobalConstantService, TotalEvaluasiJasaService) {
        var vm = this;
        vm.flagEmp = item.dataTemp;
        vm.contractRequisitionId = item.contractRequisitionId;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.isAdd = item.act;
        vm.act = item.act;
        vm.datarekanan = [];
        vm.currentPage = 1;
        vm.fullSize = 10;
        vm.offset = (vm.currentPage * 10) - 10;
        vm.totalRecords = 0;
        vm.totalItems = 0;
        vm.user = '';
        vm.activator;
        vm.verificator;
        vm.menuhome = 0;
        vm.cmbStatus = 0;
        vm.rekanan_id = '';
        vm.flag = false;
        vm.pageSize = 10;
        vm.date = "";
        vm.datapegawai;
        vm.flagTemplate = false;
        vm.year = "";
        vm.datemonth = "";
        vm.project = "";
        vm.waktuMulai1 = (vm.year - 1) + '-' + vm.datemonth;
        vm.waktuMulai2 = vm.date;

        vm.sStatus = -1;
        vm.thisPage = 12;
        vm.verificationPage = 130;
        vm.verifikasi = {};
        vm.isCalendarOpened = [false, false, false, false];
        //functions
        vm.init = init;
        vm.selectedPosition1 = {};
        vm.addPegawai = {
            ContractRequisitionID: 0,
            position: {},
            employee: {},
            IsActive: ""
        };
        vm.empNonAct = [];
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');
            loadData();
            loadReferenceType();


        };

        vm.loadReferenceType = loadReferenceType;
        function loadReferenceType() {
            vm.tipeApp = [];
            UIControlService.loadLoading("Loading");
            TotalEvaluasiJasaService.getReferenceType(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.list = reply.data;
                    vm.tipeApp = vm.list.List;
                } else {
                    UIControlService.msg_growl("error", $filter('translate') ('MESSAGE.ERR_LOAD_APPROVERS'));
            }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate') ('MESSAGE.ERR_LOAD_APPROVERS'));
            });
        }
        vm.loadData = loadData;
        function loadData() {
            vm.crApps = [];
            vm.list = [];
            UIControlService.loadLoading("Loading");
            TotalEvaluasiJasaService.GetCRApprovals({
                Status: vm.contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.list = reply.data;
                    for (var i = 0; i < vm.list.length; i++) {
                        vm.crApps.push({
                            IsActive: vm.list[i].IsActive,
                            ID: vm.list[i].ID,
                            EmployeeID: vm.list[i].EmployeeID,
                            EmployeeFullName: vm.list[i].MstEmployee.FullName + ' ' + vm.list[i].MstEmployee.SurName,
                            EmployeePositionName: vm.list[i].MstEmployee.PositionName,
                            EmployeeDepartmentName: vm.list[i].MstEmployee.DepartmentName,
                            IsHighPriority: vm.list[i].IsHighPriority,
                            SysReference1: vm.list[i].SysReference1
                        });
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        vm.tambah = tambah;
        function tambah() {
            var data = {
                currentData: vm.crApps
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/total-evaluasi-jasa/aturApproval.modal.html',
                controller: 'selectApproverModalTotalEvalCtrl',
                controllerAs: 'selAppModalTotalEvalCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function (selected) {
                vm.datapegawai = selected;
                vm.Name = selected.FullName + ' ' + selected.SurName;
                //vm.crApps.push({
                //    ID: 0,
                //    EmployeeID: selected.EmployeeID,
                //    EmployeeFullName: selected.FullName + ' ' + selected.SurName,
                //    EmployeePositionName: selected.PositionName,
                //    EmployeeDepartmentName: selected.DepartmentName,
                //    IsHighPriority: false,
                //    IsActive: true
                //});
            });
        }

        vm.addTemplate = addTemplate;
        function addTemplate() {
            var data = {
                act: false
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/temporary/susunan-contract-engineer/form-template-employee.html',
                controller: 'FormCommitteeTemplateCtrl',
                controllerAs: 'FormCommitteeTemplateCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function (dataitem) {
                vm.list = vm.detail;
                vm.datapegawai = dataitem;
                if (vm.detail.length === 0) {
                    for (var i = 0; i < vm.datapegawai.length; i++) {
                        var data1 = {
                            ID: 0,
                            ContractRequisitionID: item.item.ContractRequisitionID,
                            position: vm.datapegawai[i].position,
                            employee:
                            {
                                ID: vm.datapegawai[i].employee.EmployeeID,
                                FullName: vm.datapegawai[i].employee.FullName,
                                PositionName: vm.datapegawai[i].employee.PositionName
                            },
                            IsActive: true
                        }
                        vm.detail.push(data1);

                    }
                }
                else {
                    for (var i = 0; i < vm.datapegawai.length; i++) {
                        for (var j = 0; j < vm.detail.length; j++) {
                            if (vm.datapegawai[i].position.PositionID === vm.detail[j].position.PositionID && vm.datapegawai[i].employee.EmployeeID === vm.detail[j].EmployeeID) {
                                vm.flagTemplate = true;
                            }
                        }
                        if (vm.flagTemplate == false) {
                            var data1 = {
                                ID: 0,
                                ContractRequisitionID: item.item.ContractRequisitionID,
                                position: vm.datapegawai[i].position,
                                employee:
                                {
                                    ID: vm.datapegawai[i].employee.EmployeeID,
                                    FullName: vm.datapegawai[i].employee.FullName,
                                    PositionName: vm.datapegawai[i].employee.PositionName
                                },
                                IsActive: true
                            }
                            vm.detail.push(data1);
                        }
                        vm.flagTemplate = false;
                    }
                    
                }
                //if (vm.datapegawai[0].ContractRequisitionID !== item.item.ContractRequisitionID) {
                //    for (var i = 0; i < vm.datapegawai.length; i++) {
                //        var data1 = {
                //            ID: 0,
                //            ContractRequisitionID: item.item.ContractRequisitionID,
                //            position: vm.datapegawai[i].position,
                //            employee:
                //            {
                //                ID: vm.datapegawai[i].employee.EmployeeID,
                //                FullName: vm.datapegawai[i].employee.FullName,
                //                PositionName: vm.datapegawai[i].employee.PositionName
                //            },
                //            IsActive: true
                //        }
                //        vm.detail.push(data1);

                //    }
                //}
            });
        }

        vm.addTemplate2 = addTemplate2;
        function addTemplate2() {
            var data = {
                act: false
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/temporary/susunan-contract-engineer/form-template-commite.html',
                controller: 'FormCommitteeTemplate2Ctrl',
                controllerAs: 'FormCommitteeTemplate2Ctrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function (dataitem) {
                vm.list = vm.detail;
                vm.datapegawai = dataitem;
                if (vm.detail.length === 0) {
                    for (var i = 0; i < vm.datapegawai.length; i++) {
                        var data1 = {
                            ID: 0,
                            ContractRequisitionID: item.item.ContractRequisitionID,
                            position: {
                                PositionID: vm.datapegawai[i].CommitteePositionID,
                                PositionName: vm.datapegawai[i].CommitteePositionName
                            },
                            employee:
                            {
                                ID: vm.datapegawai[i].EmployeeID,
                                FullName: vm.datapegawai[i].EmployeeName,
                                PositionName: vm.datapegawai[i].PositionName
                            },
                            IsActive: true
                        }
                        vm.detail.push(data1);
                    }
                }
                else {
                    for (var i = 0; i < vm.datapegawai.length; i++) {
                        for (var j = 0; j < vm.detail.length; j++) {
                            if (vm.datapegawai[i].CommitteePositionID === vm.detail[j].position.PositionID && vm.datapegawai[i].EmployeeID === vm.detail[j].EmployeeID) {
                                vm.flagTemplate = true;
                            }
                        }
                        if (vm.flagTemplate == false) {
                            var data1 = {
                                ID: 0,
                                ContractRequisitionID: item.item.ContractRequisitionID,
                                position: {
                                    PositionID: vm.datapegawai[i].CommitteePositionID,
                                    PositionName: vm.datapegawai[i].CommitteePositionName
                                },
                                employee:
                                {
                                    ID: vm.datapegawai[i].EmployeeID,
                                    FullName: vm.datapegawai[i].EmployeeName,
                                    PositionName: vm.datapegawai[i].PositionName
                                },
                                IsActive: true
                            }
                            vm.detail.push(data1);
                        }
                        vm.flagTemplate = false;
                    }
                }
            });
        }

        vm.load = load;
        function load(data) {
            console.info(JSON.stringify(data));
        }

        vm.addCommiteEmployee = addCommiteEmployee;
        function addCommiteEmployee() {
            console.info(vm.detail);
            if (vm.empNonAct.length !== 0) {
                for (var i = 0; i < vm.empNonAct.length; i++) {
                    vm.crApps.push(vm.empNonAct[i]);
                }
            }
            loadInsert();
        }

        vm.loadEmp = loadEmp;
        function loadEmp(data) {
            console.info(data);
            var data1 = {
                ID: 0,
                ContractRequisitionID: item.item.ContractRequisitionID,
                position: vm.selectedPosition,
                employee:
                {
                    ID: data.employeeID,
                    FullName: data.Name,
                    PositionName: data.PositionName
                },
                IsActive: true
            }
            vm.detail.push(data1);
        }

        vm.addEmployee = addEmployee;
        function addEmployee() {
            vm.addEmp = 0;
            vm.ListEmp = [];
            vm.act = true;
            if (vm.selectedPosition === undefined) {
                UIControlService.msg_growl("warning", "Tipe Approval belum di pilih"); return;
            }
            else if (vm.Name === undefined) {
                UIControlService.msg_growl("warning", "Pegawai Belum di pilih"); return;
            }
            else {
                vm.crApps.push({
                    ID: 0,
                    EmployeeID: vm.datapegawai.EmployeeID,
                    EmployeeFullName: vm.Name,
                    EmployeePositionName: vm.datapegawai.PositionName,
                    EmployeeDepartmentName: vm.datapegawai.DepartmentName,
                    IsHighPriority: false,
                    IsActive: true,
                    SysReference1: vm.selectedPosition
                });
                vm.Name = undefined;
}
        }

        vm.deleteRow = deleteRow;
        function deleteRow(data, index) {
            if (data.ID != 0) {
                data.IsActive = false;
                vm.empNonAct.push(data);
            }
            var idx = index;
            var _length = vm.crApps.length; // panjangSemula
            vm.crApps.splice(idx, 1);
        }

        vm.loadInsert = loadInsert;
        function loadInsert() {
            vm.list = [];
            for (var i = 0; i < vm.crApps.length; i++) {
                var dt = {
                    ID: vm.crApps[i].ID,
                    IsActive: vm.crApps[i].IsActive,
                    EmployeeID: vm.crApps[i].EmployeeID,
                    IsHighPriority: vm.crApps[i].IsHighPriority,
                    TotalEvaluation: vm.contractRequisitionId,
                    SysReference1: vm.crApps[i].SysReference1,
                    IsActive: true
                }
                vm.list.push(dt);
            }
            for (var i = 0; i < vm.empNonAct.length; i++) {
                var dt = {
                    ID: vm.empNonAct[i].ID,
                    IsActive: vm.empNonAct[i].IsActive,
                    EmployeeID: vm.empNonAct[i].EmployeeID,
                    IsHighPriority: vm.empNonAct[i].IsHighPriority,
                    TotalEvaluation: vm.contractRequisitionId,
                    SysReference1: vm.empNonAct[i].SysReference1,
                    IsActive: false
                }
                vm.list.push(dt);
            }
            UIControlService.loadLoading("Loading");
            TotalEvaluasiJasaService.SaveCRApprovals(vm.list, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_APPROVERS'));
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_APPROVERS'));
            });
        };

        vm.cekCR = cekCR;
        function cekCR() {
            ContractEngineerService.CekCR({
                Status: item.item.ContractRequisitionID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.flag = reply.data;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }
        function sendEmailVendor() {
            MailerService.getMailContent({
                Email: 'Pendaftaran Rekanan Baru',
                Username: vm.vendor.username,
                Name: vm.vendor.name
            }, function (response) {
                if (response.status == 200) {
                    var email = {
                        subject: response.data.Subject,
                        //mailContent: 'Terima kasih telah mendaftar di e-Procurement. Kami akan melakukan verifikasi terlebih dahulu terhadap data dan dokumen pendaftaran anda.',
                        mailContent: response.data.MailContent,
                        isHtml: true,
                        addresses: [vm.vendor.email]
                    };

                    UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
                    VendorRegistrationService.sendMail(email, function (response) {
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            UIControlService.msg_growl("notice", "Email Sent!");
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
