(function () {
    'use strict';

    angular.module("app").controller("CommitteeModalCtrl", ctrl);
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PurchaseRequisitionService', 'UIControlService', 'item', '$uibModalInstance', '$uibModal', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        PurchReqService, UIControlService, item, $uibModalInstance, $uibModal, GlobalConstantService) {
        var vm = this;
        vm.flagEmp = item.dataTemp;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.act = item.act;
        vm.flagButton = item.act;
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
        vm.jLoad = jLoad;
        vm.selectedPosition1 = {};
        vm.addPegawai = {
            RFQGoodID: 0,
            position: {},
            employee: {},
            IsActive: ""
        };
        vm.RFQGoodID = item.RFQGoodID;
        vm.detail = [];
        vm.empNonAct = [];
        function init() {
            vm.project = item.ProjectTitle;
            if (item.RFQGoodID != 0) jLoad(1);
            else vm.detail = item.DetailCE;
            loadPosition();


        };

        function jLoad(current) {
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            PurchReqService.selectcommite({
                Offset: offset,
                Limit: 10,
                Status: vm.RFQGoodID,
                FilterType: vm.flagEmp
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detail = reply.data.List;
                    vm.totalItems = reply.data.Count;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadPosition = loadPosition;
        function loadPosition() {
            PurchReqService.selectposition(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detailPosition = reply.data;
                    console.info(vm.detailPosition.length);
                    if (vm.detailPosition.length === 1) {
                        vm.selectedPosition = vm.detailPosition[0];
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        vm.tambah = tambah;
        function tambah() {
            var data = {
                act: false
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/temporary/susunan-contract-engineer/form-commite-employee.html',
                controller: 'FormCommitteeEmployeeCtrl',
                controllerAs: 'FormCommitteeEmployeeCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function (dataitem) {
                vm.datapegawai = dataitem;
                for (var i = 0; i < vm.datapegawai.length; i++) {
                    vm.addPegawai.position = vm.datapegawai[i].position;
                    vm.addPegawai.employee = vm.datapegawai[i].employee;
                    vm.addPegawai.email = vm.datapegawai[i].employee.Email;
                    vm.detail.push(vm.addpegawai[i]);
                }
                console.info(vm.detail);
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
            if (vm.empNonAct.length !== 0) {
                for (var i = 0; i < vm.empNonAct.length; i++) {
                    vm.detail.push(vm.empNonAct[i]);
                }
            }
            loadInsert();
        }

        vm.loadEmp = loadEmp;
        function loadEmp(data) {
            console.info(data);
            var data1 = {
                ID: 0,
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
                UIControlService.msg_growl("warning", "Jabatan Panitia Belum di pilih"); return;
            }
            else if (vm.datapegawai === undefined) {
                UIControlService.msg_growl("warning", "Pegawai Belum di pilih"); return;
            }
            if (vm.detail.length === 0) {
                var data1 = {
                    ID: 0,
                    RFQGoodID: item.RFQGoodID,
                    position: vm.selectedPosition,
                    employee:
                    {
                        ID: vm.datapegawai.employeeID,
                        FullName: vm.datapegawai.Name,
                        PositionName: vm.datapegawai.PositionName
                    },
                    IsActive: true
                }
                vm.detail.push(data1);
            }
            else {
                for (var x = 0; x < vm.detail.length; x++) {
                    if (vm.detail[x].EmployeeID === vm.datapegawai.employeeID && vm.detail[x].position.PositionID === vm.selectedPosition.PositionID) {
                        vm.addEmp = 1;
                        UIControlService.msg_growl("warning", "Pegawai Telah pilih"); return;
                    }
                }
                var data1 = {
                    ID: 0,
                    position: vm.selectedPosition,
                    employee:
                    {
                        ID: vm.datapegawai.employeeID,
                        FullName: vm.datapegawai.Name,
                        PositionName: vm.datapegawai.PositionName
                    },
                    IsActive: true,
                    email: vm.datapegawai.Email
                }
                vm.detail.push(data1);
            }
        }

        vm.deleteRow = deleteRow;
        function deleteRow(data, index) {
            if (data.ID != 0) {
                var data = {
                    ID: data.ID,
                    IsActive: false
                }
                vm.empNonAct.push(data);
            }
            var idx = index - 1;
            var _length = vm.detail.length; // panjangSemula
            vm.detail.splice(idx, 1);
        }

        vm.loadInsert = loadInsert;
        function loadInsert() {
            vm.EmpEmail = "";
            vm.EmailVendor = [];
            $uibModalInstance.close(vm.detail);
        }

        vm.cekCR = cekCR;
        function cekCR() {
            ContractEngineerService.CekCR({
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

        vm.sendEmail = sendEmail;
        function sendEmail(data, i) {
            console.info(data);
            ContractEngineerService.getMailContent({
                EmailContent: 'Notifikasi Contract Engineer',
                TenderName: vm.project,
                VendorName: data.employee.FullName
            }, function (response) {
                if (response.status == 200) {
                    var email = {
                        subject: response.data.Subject,
                        mailContent: response.data.MailContent,
                        isHtml: true,
                        addresses: [data.email]
                    };

                    UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
                    ContractEngineerService.sendMail(email, function (response) {
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            UIControlService.msg_growl("notice", "Email Sent!");
                            if (vm.EmpEmail !== "" && i == vm.detail.length - 1) {
                                sendEmail1(vm.EmpEmail);
                            }
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

        vm.sendEmail1 = sendEmail1;
        function sendEmail1(dt) {
            ContractEngineerService.getMailContent1({
                EmailContent: 'Notifikasi Requestor pengadaan',
                TenderName: vm.project,
                VendorName: dt
            }, function (response) {
                if (response.status == 200) {
                    var email = {
                        subject: response.data.Subject,
                        mailContent: response.data.MailContent,
                        isHtml: true,
                        addresses: [vm.EmailRequestor]
                    };

                    UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
                    ContractEngineerService.sendMail(email, function (response) {
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            UIControlService.msg_growl("notice", "Email Sent!");
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
