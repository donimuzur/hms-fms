(function () {
    'use strict';

    angular.module("app").controller("CRFCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', 'RelocationService', '$state', '$stateParams',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, RelocationService, $state, $stateParams,
        UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = this;
        vm.request_number = $stateParams.id;
        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles');
        vm.reason;
        vm.reference;
        vm.start_date;
        vm.end_date;
        vm.cost_cc;
        vm.Vtype;
        var today = new Date();
        String.prototype.replaceAll = function (search, replacement) {
            var target = this;
            return target.replace(new RegExp(search, 'g'), replacement);
        };
        vm.dateFormat = dateFormat;
        function dateFormat(param) {
            param = new Date(param);
            var m_names = new Array("Jan", "Feb", "Mar",
 "Apr", "May", "Jun", "Jul", "Aug", "Sep",
 "Oct", "Nov", "Dec");
            var curr_date = param.getDate();
            var curr_month = param.getMonth();
            var curr_year = param.getFullYear();
            var date = curr_date + "-" + m_names[curr_month] + "-" + curr_year;
            return date;
        }
        vm.reType = [
            { value: 'Relocate Unit' },
            {value:'Change Unit'}
        ]
        vm.request_date = vm.dateFormat(today);
        vm.employee_id;
        vm.cc;
        vm.groupLevel;
        vm.vehicle_type_id;
        vm.location_id;
        vm.expected_date;
        vm.deliverable_date;
        vm.effective_date;
        vm.date;
        vm.Vtype;
        vm.Kategori;
        vm.Usage;
        vm.purpose = "test";
        vm.vat = "Test";
        vm.admin;
        vm.selectedVehicle = [];

        vm.isCalendarOpened = [false, false, false];
        console.info($stateParams);
        if($stateParams.id!="$s"){
          vm.admin=true;
        }
        else {
          vm.admin=false;
        }
        console.info(vm.admin);
        vm.init = init
        function init() {
            RelocationService.GetCoordinator(vm.coordinator, function (reply) {
                vm.coordinatorID = reply.data[0]['UserID'];
                console.info(vm.coordinatorID);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.selectedVehicle = [];
            vm.loadTemp();
        }
        vm.insertDataCRF = insertDataCRF
        function insertDataCRF(param) {
            console.log(param);
            vm.employee_id = param.fleetEmployeeID;
            vm.employee = param.fleetEmployeeName;
            vm.groupLevel = param.fleetGroupLevel;
            vm.cc = param.fleetCostCenter;
            vm.kota = param.fleetCurrentLocation;
            vm.newkota = param.new_location;
            vm.alamat = param.fleetCurrentAddress;
            vm.newalamat = param.new_address;
            vm.vUsage = param.fleetVehicleUsage;
            vm.fleetID=param.fleet_id;
            if (param.fleetActualGroup == param.fleetGroupLevel) {
                vm.actual = "As Entitled " + param.fleetGroupLevel;
            }
            if (param.fleetActualGroup != param.fleetGroupLevel) {
                vm.actual = "Flexi Benefit " + param.fleetGroupLevel;
            }
            vm.effective_date = new Date(param.effective_date);
            if (param.temporary_deliverable_date != null) {
                vm.deliverable_date = param.temporary_deliverable_date;
            }

            if (param.withd_datetime != null) {
                vm.date = param.withd_datetime;
            }
            var data;
            if(param.relocate_type=='Change Unit'){
                data = {
                    policeNumber: param.police_number,
                    manufacturer: param.manufacturer,
                    model: param.model,
                    series: param.Series,
                    body_type: param.body_type,
                    color: param.color,
                    vendor: param.vendor,
                    startContract: vm.dateFormat(param.start_period),
                    endContract: vm.dateFormat(param.end_period)
                };
            } else {
                data = {
                    policeNumber: param.fleetPoliceNumber,
                    manufacturer: param.fleetManufacturer,
                    model: param.fleetModel,
                    series: param.fleetSeries,
                    body_type: param.fleetBodyType,
                    color: param.fleetColor,
                    vendor: param.fleetVendorName,
                    startContract: vm.dateFormat(param.fleetStartPeriod),
                    endContract: vm.dateFormat(param.fleetEndPeriod)
                };
            }
            vm.selectedVehicle.push(data);

        };
        vm.coord;
        vm.getCoord = getCoord;
        function getCoord(id) {
            RelocationService.GetCoordinatorByID(id, function succ(reply) {
                if (reply.status == 200) {
                    vm.coord = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Coordinator ID" })
                }
            }
             );
        }
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }

        vm.loadTemp = loadTemp;
        function loadTemp()
        {
            console.info(vm.request_number);
            vm.data = {};
            RelocationService.GetCRF(
                vm.request_number,
                function succ(reply) {
                    if (reply.status == 200) {
                        
                        vm.dataCRF = reply.data;
                        if (vm.dataCRF.length == 0) {
                            console.info('asdad');
                            $state.go('crf', { id:null });
                        }
                        console.info(vm.dataCRF.length);
                    }
                    else {
                        UIControlService.unloadLoading();
                        vm.dataCRF=null;
                        $.growl.error({ message: "Gagal mengambil data CRF" })
                    }
                },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " +JSON.stringify(error));
                }
            );
        }
        vm.getStatus = getStatus;
        function getStatus(idStatus) {
            RelocationService.GetStatus(idStatus, function succ(reply) {
                if (reply.status == 200) {
                    vm.statusCRF = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Status CSF" })
                }
            }
            );
        }
        vm.getFleet = getFleet;
        function getFleet() {
            vm.dataFleetCFM = [];
            vm.dataFleetCFM.length;
        }
        vm.saveDraft = saveDraft;
        vm.sendDraft = sendDraft;
        vm.saveSubmitted = saveSubmitted;
        vm.sendSubmitted = sendSubmitted;
        vm.cancel = cancel;
        vm.declineHR = declineHR;
        vm.declineFleet = declineFleet;
        vm.acceptHR = acceptHR;
        vm.acceptFleet = acceptFleet;
        vm.saveFleet = saveFleet;
        function saveDraft() {
            console.info('save draft');
            if (document.getElementById('crfID') != null) {
                vm.crfID = document.getElementById('crfID').value;
            }
            if (document.getElementById('crfID') == null) {
                vm.crfID = 0;
            }
            if (vm.employee_id == undefined) {
                UIControlService.msg_growl('warning', 'Employee Not Selected');
            }
            else if (vm.newkota == undefined) {
                UIControlService.msg_growl('warning', 'New Location Not Selected');
            }
            else if (vm.effective_date == undefined) {
                UIControlService.msg_growl('warning', 'Effective Date Not Selected');
            }
            else {
                var data = {
                    crfID: vm.crfID,
                    coordinator: vm.coordinatorID,
                    requestDate: new Date(),
                    role: vm.roles,
                    remark: 0,
                    relocationType: 'Relocate Unit',
                    fleetID: vm.fleetID,
                    newCity: vm.newkota,
                    newAddress: vm.newalamat,
                    newCostcenter: vm.cost_cc,
                    effectiveDate: vm.effective_date
                };
                RelocationService.SaveDraftBenefit(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Save CSF Draft Success");
                        if (vm.crfID == 0) {
                            var link = reply.data.replaceAll('/', 'A');
                            $state.go('crf', { id: link });
                            console.info(reply);
                        }
                        else {
                            vm.init();
                        }
                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        $uibModalInstance.dismiss('cancel');
                    }
                    UIControlService.unloadLoading();
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "ERR_SAVE");
                });
                console.info(data);
            }
        };
        function sendDraft() {
            console.info('save draft');
            if (document.getElementById('crfID') != null) {
                vm.crfID = document.getElementById('crfID').value;
            }
            if (document.getElementById('crfID') == null) {
                vm.crfID = 0;
            }
            if (vm.employee_id == undefined) {
                UIControlService.msg_growl('warning', 'Employee Not Selected');
            }
            else if (vm.newkota == undefined) {
                UIControlService.msg_growl('warning', 'New Location Not Selected');
            }
            else if (vm.effective_date == undefined) {
                UIControlService.msg_growl('warning', 'Effective Date Not Selected');
            }
            else {
                var data = {
                    crfID: vm.crfID,
                    coordinator: vm.coordinatorID,
                    requestDate: new Date(),
                    role: vm.roles,
                    remark: 0,
                    relocationType: 'Relocate Unit',
                    fleetID: vm.fleetID,
                    newCity: vm.newkota,
                    newAddress: vm.newalamat,
                    newCostcenter: vm.cost_cc,
                    effectiveDate: vm.effective_date
                };
                RelocationService.SendDraftBenefit(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Send CSF Draft Success");
                        if (vm.crfID == 0) {
                            console.info(reply);
                            var link = reply.data.replaceAll('/', 'A');
                            $state.go('crf', { id: link });
                            console.info(reply);
                        }
                        else {
                            var link = vm.request_number.replaceAll('Draft', '');
                            $state.go('crf', { id: link });
                            console.info(reply);
                        }
                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        $uibModalInstance.dismiss('cancel');
                    }
                    UIControlService.unloadLoading();
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "ERR_SAVE");
                });
                console.info(data);
            }
        };
        function saveSubmitted(param) {
            console.info(param);
            if(vm.deliverable_date==undefined){
                UIControlService.msg_growl('warning', 'Deliverable Date Not Selected');
            }
            else if (param.withd_city==null) {
                UIControlService.msg_growl('warning', 'Withdraw Date Must Be Filled');
            }
            else if (param.withd_address == null) {
                UIControlService.msg_growl('warning', 'Withdraw City Must Be Filled');
            }
            else if (param.withd_pic == null) {
                UIControlService.msg_growl('warning', 'Withdraw PIC Name Must Be Filled');
            }
            else if (param.withd_phone == null) {
                UIControlService.msg_growl('warning', 'Withdraw Phone Must Be Filled');
            }
            else if (param.deliv_city == null) {
                UIControlService.msg_growl('warning', 'Deliver City Must Be Filled');
            }
            else if (param.deliv_address == null) {
                UIControlService.msg_growl('warning', 'Deliver Address Must Be Filled');
            }
            else if (param.deliv_pic == null) {
                UIControlService.msg_growl('warning', 'Deliver PIC Name Must Be Filled');
            }
            else if (param.deliv_phone == null) {
                UIControlService.msg_growl('warning', 'Deliver Phone Must Be Filled');
            }
            else if (vm.date==undefined) {
                UIControlService.msg_growl('warning', 'Withdraw Date Not Selected');
            }
            else {
                var data = {
                    crfID: param.crf_id,
                    delDate: vm.deliverable_date,
                    relocateType: param.relocation_type,
                    lapo: "save",
                    wDate: vm.date,
                    wCity: param.withd_city,
                    dCity: param.deliv_city,
                    wAddress: param.withd_address,
                    dAddress: param.deliv_address,
                    wPic: param.withd_pic,
                    dPic: param.deliv_pic,
                    wPhone: param.withd_phone,
                    dPhone: param.deliv_phone,
                    policeNumber: vm.selectedVehicle[0].policeNumber,
                    manfacturer: vm.selectedVehicle[0].manufacturer,
                    model: vm.selectedVehicle[0].model,
                    series: vm.selectedVehicle[0].series,
                    bodyType: vm.selectedVehicle[0].body_type,
                    vendor: vm.selectedVehicle[0].vendor,
                    startPeriod: vm.selectedVehicle[0].startContract,
                    endPeriod: vm.selectedVehicle[0].endContract,
                }
                RelocationService.SaveSendUser(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Send CSF Draft Success");
                        vm.init();
                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        $uibModalInstance.dismiss('cancel');
                    }
                    UIControlService.unloadLoading();
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "ERR_SAVE");
                });
            }
        };
        function sendSubmitted(param) {
            console.info(param);
            if (vm.deliverable_date == undefined) {
                UIControlService.msg_growl('warning', 'Deliverable Date Not Selected');
            }
            else if (param.withd_city == null) {
                UIControlService.msg_growl('warning', 'Withdraw Date Must Be Filled');
            }
            else if (param.withd_address == null) {
                UIControlService.msg_growl('warning', 'Withdraw City Must Be Filled');
            }
            else if (param.withd_pic == null) {
                UIControlService.msg_growl('warning', 'Withdraw PIC Name Must Be Filled');
            }
            else if (param.withd_phone == null) {
                UIControlService.msg_growl('warning', 'Withdraw Phone Must Be Filled');
            }
            else if (param.deliv_city == null) {
                UIControlService.msg_growl('warning', 'Deliver City Must Be Filled');
            }
            else if (param.deliv_address == null) {
                UIControlService.msg_growl('warning', 'Deliver Address Must Be Filled');
            }
            else if (param.deliv_pic == null) {
                UIControlService.msg_growl('warning', 'Deliver PIC Name Must Be Filled');
            }
            else if (param.deliv_phone == null) {
                UIControlService.msg_growl('warning', 'Deliver Phone Must Be Filled');
            }
            else if (vm.date == undefined) {
                UIControlService.msg_growl('warning', 'Withdraw Date Not Selected');
            }
            
            else {
                var data = {
                    crfID: param.crf_id,
                    delDate:vm.deliverable_date,
                    relocateType:param.relocation_type,
                    lapo:"Send",
                    wDate:vm.date,
                    wCity:param.withd_city,
                    dCity:param.deliv_city,
                    wAddress:param.withd_address,
                    dAddress:param.deliv_address,
                    wPic:param.withd_pic,
                    dPic:param.deliv_pic,
                    wPhone:param.withd_phone,
                    dPhone:param.deliv_phone,
                    policeNumber:vm.selectedVehicle[0].policeNumber,
                    manfacturer:vm.selectedVehicle[0].manufacturer,
                    model:vm.selectedVehicle[0].model,
                    series:vm.selectedVehicle[0].series,
                    bodyType:vm.selectedVehicle[0].body_type,
                    vendor:vm.selectedVehicle[0].vendor,
                    startPeriod:vm.selectedVehicle[0].startContract,
                    endPeriod: vm.selectedVehicle[0].endContract,
                }
                console.info(data);
                RelocationService.SaveSendUser(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Send CSF Draft Success");
                        vm.init();
                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        $uibModalInstance.dismiss('cancel');
                    }
                    UIControlService.unloadLoading();
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "ERR_SAVE");
                });
            }
        };
        function cancel(param) {
            console.info('cancel');
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size: 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        RelocationService.GetRemark(function (reply) {
                            if (reply.status === 200) {
                                $scope.dataRemark = reply.data;
                            } else {
                                $.growl.error({ message: "Failed To Get Remark" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                    };
                    $scope.simpan = function () {
                        if ($scope.remark == undefined) {
                            UIControlService.msg_growl("error", "Remark Must Be Selected");
                        }
                        else {
                            var data = {
                                crfID: param.crf_id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            RelocationService.CancelCRF(data, function (reply) {
                                if (reply.status === 200) {
                                    vm.init();
                                    UIControlService.msg_growl("error", "CRF Cancelled By" + vm.coordinator);
                                    $uibModalInstance.dismiss('cancel');
                                } else {
                                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                UIControlService.unloadLoading();
                            }, function (error) {
                                UIControlService.unloadLoading();
                                UIControlService.msg_growl("error", "ERR_SAVE");
                            });
                        }
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                }
            });
        }
        function declineHR(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size: 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        RelocationService.GetRemark(function (reply) {
                            if (reply.status === 200) {
                                $scope.dataRemark = reply.data;
                            } else {
                                $.growl.error({ message: "Failed To Get Remark" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                    };
                    $scope.simpan = function () {
                        if ($scope.remark == undefined) {
                            UIControlService.msg_growl("error", "Remark Must Be Selected");
                        }
                        else {
                            var data = {
                                crfID: param.crf_id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            RelocationService.DeclineCRFHR(data, function (reply) {
                                if (reply.status === 200) {
                                    vm.init();
                                    UIControlService.msg_growl("error", "CRF Rejected By HR");
                                    $uibModalInstance.dismiss('cancel');
                                } else {
                                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                UIControlService.unloadLoading();
                            }, function (error) {
                                UIControlService.unloadLoading();
                                UIControlService.msg_growl("error", "ERR_SAVE");
                            });
                        }
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                }
            });
        }
        function declineFleet(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size: 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        RelocationService.GetRemark(function (reply) {
                            if (reply.status === 200) {
                                $scope.dataRemark = reply.data;
                            } else {
                                $.growl.error({ message: "Failed To Get Remark" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                    };
                    $scope.simpan = function () {
                        if ($scope.remark == undefined) {
                            UIControlService.msg_growl("error", "Remark Must Be Selected");
                        }
                        else {
                            var data = {
                                crfID: param.crf_id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            RelocationService.DeclineCRFFleet(data, function (reply) {
                                if (reply.status === 200) {
                                    vm.init();
                                    UIControlService.msg_growl("error", "CRF Rejected By Fleet");
                                    $uibModalInstance.dismiss('cancel');
                                } else {
                                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                UIControlService.unloadLoading();
                            }, function (error) {
                                UIControlService.unloadLoading();
                                UIControlService.msg_growl("error", "ERR_SAVE");
                            });
                        }
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                }
            });
        };
        function acceptHR(param) {
            var data = {
                crfID: param.crf_id,
                role: vm.roles,
                remark: $scope.remark
            }
            RelocationService.ApproveCRFHR(data, function (reply) {
                if (reply.status === 200) {
                    vm.init();
                    UIControlService.msg_growl("notice", "CSF Approved By HR");
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                    $uibModalInstance.dismiss('cancel');
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
        };
        function acceptFleet(param) {
            var data = {
                crfID: param.crf_id,
                role: vm.roles,
                remark: $scope.remark
            }
            RelocationService.ApproveCRFFleet(data, function (reply) {
                if (reply.status === 200) {
                    vm.init();
                    UIControlService.msg_growl("notice", "CSF Approved By Fleet");
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                    $uibModalInstance.dismiss('cancel');
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
        };
        function saveFleet(param) {
            RelocationService.SaveByFleet(data, function (reply) {
                if (reply.status === 200) {
                    vm.init();
                    UIControlService.msg_growl("notice", "Save CSF In Progress Success");
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                    $uibModalInstance.dismiss('cancel');
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
        };
        vm.alamat;
        vm.costCenter = costCenter;
        function costCenter() {
            var modalInstance = $uibModal.open({
                templateUrl: 'csfSelectCity.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.pagingCSF = true;
                    $scope.pagingCSFCc = false;
                    $scope.currentPage = 1;
                    $scope.fullSize = 10;
                   
                    $scope.totalRecords = 0;
                    $scope.getCC = function (param) {
                        $scope.CostCenter = {};
                        $scope.currentPage = param;
                        $scope.offset = (param * 10) - 10;
                        RelocationService.GetCC({
                            Offset: $scope.offset,
                            Limit : $scope.fullSize
                        }, function (reply) {
                            if (reply.status === 200) {
                                $scope.CostCenter = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                                console.info($scope.CostCenter);
                               

                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data cc" });

                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                            console.log($scope.offset)
                        });
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.select = function(param)
                    {
                        
                            vm.cc_id = param.cc_id;
                            vm.cost_cc = param.cost_center1;
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.Kota = function (param, param1, param2) {
                        vm.location_id = param;
                        vm.kota = param1;
                        vm.alamat = param2
                        $uibModalInstance.dismiss('cancel');
                        console.info(vm.kota);
                    };

                }
            });
        };

        vm.employee;
        vm.openEmployee = openEmployee;
        function openEmployee() {
            var modalInstance = $uibModal.open({
                templateUrl: 'employee.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.pagingEmployee = true;
                    $scope.pagingEmployeeID = false;
                    $scope.pagingEmployeeName = false;
                    $scope.pagingEmployeeCost = false;
                    $scope.pagingEmployeeGrup = false;                   
                    $scope.pageSize = 10;
                    $scope.totalRecords = 0;
                    $scope.getEmployee = function (param) {
                        $scope.employees = {};
                        $scope.currentPage = param;
                        $scope.offset = (param * 10) - 10;
                        RelocationService.GetEmployee({
                            Offset: $scope.offset,
                            Limit: $scope.pageSize
                        }, function (reply) {
                            if (reply.status === 200) {
                                $scope.employees = reply.data;
                                $scope.totalRecords = reply.data.length;
                                //console.log(reply.data)
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data employee" });
                                console.log('tes')
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));

                        });
                        console.info("adjh");
                    };
                    $scope.searchID = function (param) {
                        if ($scope.employee_id == undefined) {
                            getEmployee(1);
                            $scope.pagingEmployee = true;
                            $scope.pagingEmployeeID = false;
                            $scope.pagingEmployeeName = false;
                            $scope.pagingEmployeeCost = false;
                            $scope.pagingEmployeeGrup = false;
                        }
                        else {
                            $scope.pagingEmployee = false;
                            $scope.pagingEmployeeID = true;
                            $scope.pagingEmployeeName = false;
                            $scope.pagingEmployeeCost = false;
                            $scope.pagingEmployeeGrup = false;
                            console.info($scope.employee_id);
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            RelocationService.SearchEmployee({
                                search: 'ID',
                                keyword: $scope.employee_id,
                                Offset: $scope.offset,
                                Limit: $scope.pageSize
                            }, function (reply) {
                                $scope.employees = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                    };
                    $scope.searchName = function (param) {
                        if ($scope.employee_name == undefined) {
                            getEmployee(1);
                            $scope.pagingEmployee = true;
                            $scope.pagingEmployeeID = false;
                            $scope.pagingEmployeeName = false;
                            $scope.pagingEmployeeCost = false;
                            $scope.pagingEmployeeGrup = false;
                        }
                        else {
                            $scope.pagingEmployee = false;
                            $scope.pagingEmployeeID = false;
                            $scope.pagingEmployeeName = true;
                            $scope.pagingEmployeeCost = false;
                            $scope.pagingEmployeeGrup = false;
                            console.info($scope.employee_name);
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            RelocationService.SearchEmployee({
                                search: 'nama',
                                keyword: $scope.employee_name,
                                Offset: $scope.offset,
                                Limit: $scope.pageSize
                            }, function (reply) {
                                $scope.employees = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                    };
                    $scope.searchCost = function (param) {
                        if ($scope.cost_center == undefined) {
                            getEmployee(1);
                            $scope.pagingEmployee = true;
                            $scope.pagingEmployeeID = false;
                            $scope.pagingEmployeeName = false;
                            $scope.pagingEmployeeCost = false;
                            $scope.pagingEmployeeGrup = false;
                        }
                        else {
                            $scope.pagingEmployee = false;
                            $scope.pagingEmployeeID = false;
                            $scope.pagingEmployeeName = false;
                            $scope.pagingEmployeeCost = true;
                            $scope.pagingEmployeeGrup = false;
                            console.info($scope.cost_center);
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            RelocationService.SearchEmployee({
                                search: 'cost',
                                keyword: $scope.cost_center,
                                Offset: $scope.offset,
                                Limit: $scope.pageSize
                            }, function (reply) {
                                $scope.employees = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                    }
                    $scope.searchGrup = function (param) {
                        if ($scope.group_level == undefined) {
                            getEmployee(1);
                            $scope.pagingEmployee = true;
                            $scope.pagingEmployeeID = false;
                            $scope.pagingEmployeeName = false;
                            $scope.pagingEmployeeCost = false;
                            $scope.pagingEmployeeGrup = false;
                        }
                        else {
                            $scope.pagingEmployee = false;
                            $scope.pagingEmployeeID = false;
                            $scope.pagingEmployeeName = false;
                            $scope.pagingEmployeeCost = false;
                            $scope.pagingEmployeeGrup = true;
                            console.info($scope.group_level);
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            RelocationService.SearchEmployee({
                                search: 'group',
                                keyword: $scope.group_level,
                                Offset: $scope.offset,
                                Limit: $scope.pageSize
                            }, function (reply) {
                                console.info(reply.data);
                                $scope.employees = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })

                        }
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.select = function (param) {
                        vm.employee_id = param.employee_id;
                        vm.employee = param.employee_name;
                        vm.position = param.title_name;
                        vm.groupLevel = param.group_level;
                        vm.cc = param.cost_center;
                        vm.vUsage = param.vehicle_usage;
                        vm.vType = param.vehicle_type;
                        vm.kota = param.city;
                        vm.alamat = param.address;
                        vm.fleetID = param.fleet_id;
                        if(param.actual_group==param.group_level){
                            vm.actual = "As Entitled "+param.group_level;
                        }
                        else vm.actual ="Flexi Benefit "+param.car_group_level;
                        $uibModalInstance.dismiss('cancel');
                    };

                }
            });
        };
        vm.dataTemporary = [];
        vm.openTemporary = openTemporary;
        function openTemporary() {
            var modalInstance = $uibModal.open({
                templateUrl: 'Temporary.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.start_date;
                    $scope.end_date;
                    $scope.reason
                    $scope.openCalendar = function (index) {
                        $scope.isCalendarOpened[index] = true;
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.simpan = function () {
                        if ($scope.reason == undefined) {
                            UIControlService.msg_growl("error", "Reason Belum Dipilih");
                        }
                        else if ($scope.start_date == undefined) {
                            UIControlService.msg_growl("error", "Start Date Belum Dipilih");
                        }
                        else if ($scope.end_date == undefined) {
                            UIControlService.msg_growl("error", "End Date Belum Dipilih");
                        }
                        else if ($scope.end_date < $scope.start_date) {
                            UIControlService.msg_growl("error", "End Date Tidak Boleh Kurang dari Start Date");
                        }
                        else {
                            vm.dataTemporary = [];
                            var item = {
                                startDate: $scope.start_date,
                                endDate: $scope.end_date,
                                reason: $scope.reason
                            }
                            vm.dataTemporary.push(item);
                            console.info(vm.dataTemporary);
                            $uibModalInstance.dismiss('cancel');
                        }
                    };

                }
            });
        }
        vm.kota;
        vm.alamat;
        vm.newlocation_id;
        vm.newkota;
        vm.newalamat;
        vm.openCity = openCity;
        function openCity(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'selectCity.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.pagingCity = true;
                    $scope.pagingLocationCity = false;
                    $scope.pagingLocationAddress = false;
                    $scope.currentPage = 1;
                    $scope.fullSize = 10;                   
                    $scope.totalRecords = 0;

                    $scope.getLocation = function (param) {
                        $scope.location = {};
                        $scope.currentPage = param;
                        $scope.offset = (param * 10) - 10;
                        RelocationService.GetKota({
                            Offset: $scope.offset,
                            Limit: $scope.fullSize
                        }, function (reply) {
                            if (reply.status === 200) {
                                $scope.location = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                               
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data lokasi" });
                              
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                        console.info("adjh");
                    };
                    $scope.searchCity = function (param) {
                        if ($scope.kota == undefined) {
                            getLocation(1)
                            $scope.pagingCity = true;
                            $scope.pagingLocationCity = false;
                            $scope.pagingLocationAddress = false;
                            console.log('kota undefined')
                        } else {
                            $scope.pagingCity = false;
                            $scope.pagingLocationCity = true;
                            $scope.pagingLocationAddress = false;
                            console.log($scope.kota);
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            RelocationService.GetLocationByCity({
                                search: 'Kota',
                                city: $scope.kota,
                                Offset: $scope.offset,
                                Limit: $scope.fullSize
                            }, function (reply) {
                                $scope.location = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                    }
                    $scope.searchAddress = function (param) {
                        if ($scope.alamat == undefined) {
                            getLocation(1)
                            $scope.pagingCity = true;
                            $scope.pagingLocationCity = false;
                            $scope.pagingLocationAddress = false;
                            console.log('alamat undefined')
                        } else {
                            $scope.pagingCity = false;
                            $scope.pagingLocationCity = false;
                            $scope.pagingLocationAddress = true;
                            console.log($scope.alamat);
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            RelocationService.GetLocationByAddress({
                                search: 'Alamat',
                                address: $scope.alamat,
                                Offset: $scope.offset,
                                Limit: $scope.fullSize
                            }, function (reply) {
                                $scope.location = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                                console.log(reply.data.List);
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    if (param == 1) {
                        $scope.select = function (param) {
                            
                                vm.location_id = param.location_id;
                                vm.kota = param.city;
                                vm.alamat = param.address;
                            $uibModalInstance.dismiss('cancel');

                        };
                    } else if (param == 2) {
                        $scope.select = function (param) {
                            
                                vm.newlocation_id = param.location_id;
                                vm.newkota = param.city;
                                vm.newalamat = param.address;
                            $uibModalInstance.dismiss('cancel');

                        };
                        console
                    } else {
                        console.log('ada yang salah')
                    }
                  

                }
            });
        };
        //vm.newKota;
        //vm.newAlamat
        vm.selectVehicle = selectVehicle;
        function selectVehicle() {
            if (vm.groupLevel == undefined) {
                UIControlService.msg_growl("error", "Employee Belum Dipilih");
            }
            else {
                var modalInstance = $uibModal.open({
                    templateUrl: 'selectVehicle.html',
                    controller: function ($uibModalInstance, $scope) {
                        $scope.getVehicle = function () {
                            $scope.DataVehicle = {};
                            RelocationService.GetVehicle(vm.groupLevel, function (reply) {
                                if (reply.status === 200) {
                                    $scope.DataVehicle = reply.data;
                                    console.info($scope.DataVehicle);
                                } else {
                                    $.growl.error({ message: "Gagal mendapatkan data cc" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            });
                        };
                        $scope.batal = function () {
                            $uibModalInstance.dismiss('cancel');
                        };
                        $scope.simpan = function (param) {
                            vm.selectedVehicle = [];
                            vm.selectedVehicle = param;
                            $uibModalInstance.dismiss('cancel');
                            console.info(vm.selectedVehicle);
                        };

                    }
                });
            }
        };
        vm.uploadDetailVehicle = uploadDetailVehicle;
        function uploadDetailVehicle() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadDetailVehicle.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.filevehicle;
                    $scope.data = [];
                    $scope.select = function (file) {
                        $scope.filevehicle = file;
                    };
                    $scope.uploadFile = function () {
                        $scope.upload($scope.filevehicle);
                    };
                    $scope.getfleet = function () {
                        CSFService.GetFleetCSF($stateParams.rq, function (reply) {
                            if (reply.status === 200) {
                                $scope.fleetcsf = reply.data;
                                console.info($scope.fleetcsf);
                            }
                        });
                    };
                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file, function (reply) {
                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];
                                console.info(sheet1[2].Column2);
                                
                                for (var i = 3; i < sheet1.length; i++) {
                                    var item = {
                                        csf: sheet1[i].Column2,
                                        employeeName: sheet1[i].Column3,
                                        vendor: sheet1[i].Column4,
                                        policeNumber: sheet1[i].Column5,
                                    };
                                    console.info(item.contractStart instanceof Date);
                                    //validasi
                                    //if (!item.csf) {
                                    //    UIControlService.msg_growl("error", "Item baris " + (i + 1) + " tidak valid:");
                                    //    UIControlService.msg_growl("error", "Request Number harus diisi");
                                    //    $scope.data = [];
                                    //    break;
                                    //}
                                    $scope.data.push(item);
                                };
                            } else {
                                UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR");
                            }
                        },
                            function (error) {
                                UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR")
                                UIControlService.unloadLoadingModal();
                            }
                        );
                    };
                    $scope.simpan = function () {
                        console.info($scope.data.length);
                        if ($scope.data.length > 0) {
                            CSFService.SaveFleet(
                                    $scope.data
                                , function (reply) {
                                    if (reply.status === 200) {
                                        UIControlService.msg_growl("notice", "Update Fleet Success");
                                        vm.init();
                                    } else {
                                        $.growl.error({ message: "" });
                                        vm.init();
                                    }
                                }, function (err) {
                                    console.info("error:" + JSON.stringify(err));
                                    vm.init();
                                });
                        }
                        else {
                            UIControlService.msg_growl("error", "Tidak Ada Data Yang Disimpan")
                            UIControlService.unloadLoadingModal();
                        }
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
        };
    }
})();
