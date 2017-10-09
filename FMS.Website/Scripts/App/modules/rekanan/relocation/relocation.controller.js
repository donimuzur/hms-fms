(function () {
    'use strict';

    angular.module("app").controller("CRFCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', 'RelocationService', '$state', '$stateParams',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, RelocationService, $state, $stateParams,
        UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = $scope;
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
        Number.prototype.padLeft = function (base, chr) {
            var len = (String(base || 10).length - String(this).length) + 1;
            return len > 0 ? new Array(len).join(chr || '0') + this : this;
        }
        vm.dateFormatLog = dateFormatLog;
        function dateFormatLog(param) {
            var params = new Date(param);
            var dformat = vm.dateFormat(param) +' '+
                  [params.getHours().padLeft(),
                    params.getMinutes().padLeft(),
                    params.getSeconds().padLeft()].join(':');
            return dformat;
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
        function init(param) {
            RelocationService.GetCoordinator(vm.coordinator, function (reply) {
                vm.coordinatorID = reply.data[0]['UserID'];
                console.info(vm.coordinatorID);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.vType = param;
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
            vm.cost_cc = param.new_cost_center;
            vm.kota = param.fleetCurrentLocation;
            vm.newkota = param.new_location;
            vm.alamat = param.fleetCurrentAddress;
            vm.newalamat = param.new_address;
            vm.vUsage = param.fleetVehicleUsage;
            vm.fleetID = param.fleet_id;
            vm.actualLevel = param.fleetActualGroup;
            if (param.fleetActualGroup == param.fleetGroupLevel) {
                vm.actual = "As Entitled " + param.fleetActualGroup;
            }
            if (param.fleetActualGroup != param.fleetGroupLevel) {
                vm.actual = "Flexi Benefit " + param.fleetActualGroup;
            }
            if (param.effective_date != null) {
                vm.effective_date = new Date(param.effective_date);
            }
            if (param.temporary_deliverable_date != null) {
                vm.deliverable_date = new Date(param.temporary_deliverable_date);
            }

            if (param.withd_datetime != null) {
                vm.date = new Date(param.withd_datetime);
            }
            if (param.expected_date != null) {
                vm.expected_date = new Date(param.expected_date);
            }
            var data;
            if(param.relocation_type=='Change Unit'){
                data = {
                    policeNumber: param.police_number,
                    manufacturer: param.manufacturer,
                    model: param.model,
                    series: param.series,
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
        vm.getTemporary = getTemporary;
        function getTemporary(param) {
            console.info(param);
            var data = {
                csfNumber: param
            };
            RelocationService.GetTemp(data, function succ(reply) {
                if (reply.status == 200) {
                    vm.dataTemporary = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Data Temporary" })
                }
            });
        };
        vm.tempLink = tempLink;
        function tempLink(param) {
            param = param.replaceAll('/', 'A');
            return param;

        }
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
                        console.info(reply.data[0].fleetVehicleType);
                        if (vm.dataCRF.length == 0) {
                            console.info('asdad');
                            $state.go('crf', { id:null });
                        }
                        if (reply.data[0].fleetVehicleType != vm.vType) {
                            console.info(vm.vType);
                            if (vm.vType != "WTC") {
                                $state.go('crf-wtc', { id: vm.request_number });
                            }
                            else {
                                $state.go('crf', { id: vm.request_number });
                            }
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
        vm.getLog = getLog;
        function getLog(param) {
            console.info(param);
            RelocationService.GetLog(param, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.log = reply.data;
                    console.info(vm.log);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Data Log CSF" })
                }
            },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " + JSON.stringify(error));
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
            });
        }
        vm.getFleet = getFleet;
        function getFleet(param) {
            vm.dataFleetCFM = [];
            var data = "|" + vm.newkota + "|" + vm.actualLevel + "|||||||0|10";
            RelocationService.GetIdleFleet(data, function succ(reply) {
                if (reply.status == 200) {
                    vm.dataFleetCFM = reply.data.List;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Status CSF" })
                }
            });
        }
        vm.dataIsChange = [
            { value: 1, is: true, name: "Yes" },
            { value: 0, is: false, name: "No" }
        ];
        vm.saveDraft = saveDraft;
        vm.sendDraft = sendDraft;
        vm.saveSubmitted = saveSubmitted;
        vm.sendSubmitted = sendSubmitted;
        vm.saveDraftWTC = saveDraftWTC;
        vm.sendDraftWTC = sendDraftWTC;
        vm.saveSubmittedWTC = saveSubmittedWTC;
        vm.sendSubmittedWTC = sendSubmittedWTC;
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
                    newCostCenter: vm.cost_cc,
                    effectiveDate: vm.effective_date
                };
                RelocationService.SaveDraftBenefit(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Save CRF Draft Success");
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
                    newCostCenter: vm.cost_cc,
                    effectiveDate: vm.effective_date
                };
                RelocationService.SendDraftBenefit(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Send CRF Draft Success");
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
            console.info(vm.date);
            if(param.withd_city==null) {
                UIControlService.msg_growl('warning', 'Withdraw City Must Be Filled');
            }
            else if (param.change_police_number == null) {
                UIControlService.msg_growl('warning', 'Change Police Number City Must Be Filled');
            }
            else if (param.withd_address == null) {
                UIControlService.msg_growl('warning', 'Withdraw Address Must Be Filled');
            }
            else if (param.withd_pic == null) {
                UIControlService.msg_growl('warning', 'Withdraw PIC Name Must Be Filled');
            }
            else if (param.withd_phone == "") {
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
            else if (param.deliv_phone == "") {
                UIControlService.msg_growl('warning', 'Deliver Phone Must Be Filled');
            }
            else if (vm.date == undefined) {
                UIControlService.msg_growl('warning', 'Withdraw Date Not Selected');
            }
            else {
                
                var data = {
                    crfID: param.crf_id,
                    delDate: vm.deliverable_date,
                    relocateType: param.relocation_type,
                    vehicleUsage: param.fleetVehicleUsage,
                    isChange:Number(param.change_police_number),
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
                    manufacturer: vm.selectedVehicle[0].manufacturer,
                    model: vm.selectedVehicle[0].model,
                    series: vm.selectedVehicle[0].series,
                    bodyType: vm.selectedVehicle[0].body_type,
                    vendor: vm.selectedVehicle[0].vendor,
                    startPeriod: vm.selectedVehicle[0].startContract,
                    endPeriod: vm.selectedVehicle[0].endContract,
                }
                console.info(data);
                RelocationService.SaveSendUser(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Save CRF Assigned Success");
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
            if (param.withd_city == null) {
                UIControlService.msg_growl('warning', 'Withdraw City Must Be Filled');
            }
            else if (param.change_police_number == null) {
                UIControlService.msg_growl('warning', 'Change Police Number City Must Be Filled');
            }
            else if (param.withd_address == null) {
                UIControlService.msg_growl('warning', 'Withdraw City Must Be Filled');
            }
            else if (param.withd_pic == null) {
                UIControlService.msg_growl('warning', 'Withdraw PIC Name Must Be Filled');
            }
            else if (param.withd_phone == "") {
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
            else if (param.deliv_phone == "") {
                UIControlService.msg_growl('warning', 'Deliver Phone Must Be Filled');
            }
            else if (vm.date == undefined) {
                UIControlService.msg_growl('warning', 'Withdraw Date Not Selected');
            }
            
            else {
                if (param.change_police_number == null) {
                    param.change_police_number = 0;
                }
                var data = {
                    crfID: param.crf_id,
                    delDate:vm.deliverable_date,
                    relocateType: param.relocation_type,
                    vehicleUsage: param.fleetVehicleUsage,
                    isChange: Number(param.change_police_number),
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
                    manufacturer:vm.selectedVehicle[0].manufacturer,
                    model:vm.selectedVehicle[0].model,
                    series:vm.selectedVehicle[0].series,
                    bodyType:vm.selectedVehicle[0].body_type,
                    vendor:vm.selectedVehicle[0].vendor,
                    startPeriod:vm.selectedVehicle[0].startContract,
                    endPeriod: vm.selectedVehicle[0].endContract,
                    role:vm.roles
                }
                console.info(data);
                RelocationService.SaveSendUser(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Send CRF Assigned Success");
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
        function saveDraftWTC() {
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
            else {
                var data = {
                    crfID: vm.crfID,
                    coordinator: vm.coordinatorID,
                    requestDate: new Date(),
                    role: vm.roles,
                    remark: 0,
                    relocationType: 'Relocate Unit',
                    fleetID: vm.fleetID,
                };
                RelocationService.SaveDraftWTC(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Save CRF Draft Success");
                        if (vm.crfID == 0) {
                            var link = reply.data.replaceAll('/', 'A');
                            $state.go('crf-wtc', { id: link });
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
        function sendDraftWTC() {
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
            else {
                var data = {
                    crfID: vm.crfID,
                    coordinator: vm.coordinatorID,
                    requestDate: new Date(),
                    role: vm.roles,
                    remark: 0,
                    relocationType: 'Relocate Unit',
                    fleetID: vm.fleetID,
                };
                RelocationService.SendDraftWTC(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Save CRF Draft Success");
                        if (vm.crfID == 0) {
                            var link = reply.data.replaceAll('/', 'A');
                            $state.go('crf-wtc', { id: link });
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
        function saveSubmittedWTC(param) {
            console.info(param);
            if (vm.effective_date == undefined) {
                UIControlService.msg_growl('warning', 'Effective Date Not Selected');
            }
            else if (vm.cost_cc == undefined) {
                UIControlService.msg_growl('warning', 'Cost Center Not Selected');
            }
            else if (param.change_police_number == null) {
                UIControlService.msg_growl('warning', 'Change Police Number Not Selected');
            }
            else if (param.withd_pic == null) {
                UIControlService.msg_growl('warning', 'Withdraw PIC Name Must Be Filled');
            }
            else if (param.withd_phone == "") {
                UIControlService.msg_growl('warning', 'Withdraw Phone Must Be Filled');
            }
            else if (vm.newkota == undefined) {
                UIControlService.msg_growl('warning', 'Deliver City Must Be Filled');
            }
            else if (vm.newalamat == undefined) {
                UIControlService.msg_growl('warning', 'Deliver Address Must Be Filled');
            }
            else if (param.deliv_pic == null) {
                UIControlService.msg_growl('warning', 'Deliver PIC Name Must Be Filled');
            }
            else if (param.deliv_phone == "") {
                UIControlService.msg_growl('warning', 'Deliver Phone Must Be Filled');
            }
            else if (vm.date == undefined) {
                UIControlService.msg_growl('warning', 'Withdraw Date Not Selected');
            }
            else {
                var data = {
                    crfID: param.crf_id,
                    delDate: vm.deliverable_date,
                    effectiveDate: vm.effective_date,
                    costCenter: vm.cost_cc,
                    isChange: Number(param.change_police_number),
                    lapo: "save",
                    wDate: vm.date,
                    wCity: vm.kota,
                    dCity: vm.newkota,
                    wAddress: vm.alamat,
                    dAddress: vm.newalamat,
                    wPic: param.withd_pic,
                    dPic: param.deliv_pic,
                    wPhone: param.withd_phone,
                    dPhone: param.deliv_phone,
                    role:vm.roles
                }
                console.info(data);
                RelocationService.SaveSendWTCUser(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Save CRF Assigned Success");
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
        function sendSubmittedWTC(param) {
            if (vm.effective_date == undefined) {
                UIControlService.msg_growl('warning', 'Effective Date Not Selected');
            }
            else if (vm.cost_cc == undefined) {
                UIControlService.msg_growl('warning', 'Cost Center Not Selected');
            }
            else if (param.change_police_number == null) {
                UIControlService.msg_growl('warning', 'Change Police Number Not Selected');
            }
            else if (param.withd_pic == null) {
                UIControlService.msg_growl('warning', 'Withdraw PIC Name Must Be Filled');
            }
            else if (param.withd_phone == "") {
                UIControlService.msg_growl('warning', 'Withdraw Phone Must Be Filled');
            }
            else if (vm.newkota == undefined) {
                UIControlService.msg_growl('warning', 'Deliver City Must Be Filled');
            }
            else if (vm.newalamat == undefined) {
                UIControlService.msg_growl('warning', 'Deliver Address Must Be Filled');
            }
            else if (param.deliv_pic == null) {
                UIControlService.msg_growl('warning', 'Deliver PIC Name Must Be Filled');
            }
            else if (param.deliv_phone == "") {
                UIControlService.msg_growl('warning', 'Deliver Phone Must Be Filled');
            }
            else if (vm.date == undefined) {
                UIControlService.msg_growl('warning', 'Withdraw Date Not Selected');
            }
            else {
                var data = {
                    crfID: param.crf_id,
                    delDate: vm.deliverable_date,
                    effectiveDate: vm.effective_date,
                    costCenter: vm.cost_cc,
                    isChange: Number(param.change_police_number),
                    lapo: "Send",
                    wDate: vm.date,
                    wCity: vm.kota,
                    dCity: vm.newkota,
                    wAddress: vm.alamat,
                    dAddress: vm.newalamat,
                    wPic: param.withd_pic,
                    dPic: param.deliv_pic,
                    wPhone: param.withd_phone,
                    dPhone: param.deliv_phone,
                    role: vm.roles
                }
                console.info(data);
                RelocationService.SaveSendWTCUser(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Send CRF Assigned Success");
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
                                crfID: param,
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
                                crfID: param,
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
                                crfID: param,
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
                crfID: param,
                role: vm.roles,
                remark: 0
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
                crfID: param,
                role: vm.roles,
                remark: 0,
                coordinator2:vm.coordinatorID
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
            if (param.new_police_number == null) {
                param.new_police_number = "";
            }
            if (param.relocate_po_number == null) {
                param.relocate_po_number = "";
            }
            if (param.relocate_cost == null) {
                param.relocate_cost = 0;
            }
            if (param.relocate_po_line == "") {
                param.relocate_po_line = 0;
            }
            var data = {
                crfID:param.crf_id,
                expectedDate: vm.expected_date,
                newPolice: param.new_police_number,
                rePOLine: param.relocate_po_line,
                rePONumber: param.relocate_po_number,
                rePOPrice:param.relocate_cost
            }
            RelocationService.SaveByFleet(data, function (reply) {
                if (reply.status === 200) {
                    vm.init();
                    UIControlService.msg_growl("notice", "Save CRF In Progress Success");
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
                    $scope.costCenter = "";
                    $scope.description = "";
                    $scope.function = "";
                    $scope.currentPage = 1;
                    $scope.pageSize = 15;
                    $scope.totalRecords = 0;
                    $scope.getCC = function (param) {
                        $scope.currentPage = param;
                        $scope.offset = (param * 10) - 10;
                        var data = $scope.costCenter.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize + "|" + $scope.function + "|" + $scope.description;
                        RelocationService.GetCC(data, function (reply) {
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
                    $scope.pageSize = 15;
                    $scope.totalRecords = 0;
                    $scope.getEmployee = function (param) {
                        $scope.employees = {};
                        $scope.currentPage = param;
                        $scope.offset = (param * 10) - 10;
                        RelocationService.GetEmployee(vm.vType, function (reply) {
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
        function openTemporary(param) {
            var crf = param;
            var modalInstance = $uibModal.open({
                templateUrl: 'temporary.html',
                resolve: {
                    test: function () {
                        return crf;
                    }
                },
                controller: function ($uibModalInstance, $scope, test) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.start_date;
                    $scope.end_date;
                    $scope.reason;
                    $scope.crf = test;
                    $scope.getReason = function () {
                        console.info($scope.crf);
                        RelocationService.GetTempReason(function (reply) {
                            if (reply.status === 200) {
                                $scope.datareason = reply.data;
                            } else {
                                $.growl.error({ message: "Error" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });

                    };
                    $scope.openCalendar = function (index) {
                        $scope.isCalendarOpened[index] = true;
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.simpan = function () {
                        if ($scope.reason == undefined) {
                            UIControlService.msg_growl("error", "Reason Must be Selected");
                        }
                        else if ($scope.start_date == undefined) {
                            UIControlService.msg_growl("error", "Start Date Must be Selected");
                        }
                        else if ($scope.end_date == undefined) {
                            UIControlService.msg_growl("error", "End Date Must be Selected");
                        }
                        else if ($scope.end_date < $scope.start_date) {
                            UIControlService.msg_growl("error", "End Date Can't Less Than Start Date");
                        }
                        else {
                            if ($scope.crf.fleetVehicleType == "BENEFIT") {
                                $scope.crf.fleetVehicleType = "Benefit";
                            }
                            if ($scope.crf.fleetGroupLevel == null) {
                                $scope.crf.fleetGroupLevel = "";
                            }
                            if ($scope.crf.new_cost_center == null) {
                                $scope.cost_center = $scope.crf.fleetCostCenter;
                            }
                            else {
                                $scope.cost_center = $scope.crf.new_cost_center;
                            }
                            var data = {
                                crfNumber: $scope.crf.request_number,
                                startDate: $scope.start_date,
                                endDate: $scope.end_date,
                                reason: $scope.reason,
                                coordinator: vm.coordinatorID,
                                suplyMethod: "Temporary",
                                city: $scope.crf.new_location,
                                address: $scope.crf.new_address,
                                employeeName: $scope.crf.fleetEmployeeName,
                                actualGroup: $scope.crf.fleetActualGroup,
                                costCenter: $scope.cost_center,
                                GroupLevel: $scope.crf.fleetGroupLevel,
                                project: false,
                                vehicleType: $scope.crf.fleetVehicleType,
                                employeeId: $scope.crf.fleetEmployeeID,
                                role: vm.roles
                            }
                            console.info(data);
                            RelocationService.SaveTemporary(data, function (reply) {
                                if (reply.status === 200) {
                                    vm.saveFleet($scope.crf);
                                    UIControlService.msg_growl("notice", "Save  Temporary Success");
                                    $uibModalInstance.dismiss('cancel');
                                    vm.init();
                                } else {
                                    $.growl.error({ message: "Error" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            });
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
                    $scope.city = "";
                    $scope.address = "";
                    $scope.currentPage = 1;
                    $scope.pageSize = 15;
                    $scope.totalRecords = 0;
                    $scope.getCC = function (param) {
                        $scope.currentPage = param;
                        $scope.offset = (param * 10) - 10;
                        $scope.CostCenter = {};
                        var data = $scope.city.toLowerCase() + "|" + $scope.address.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                        RelocationService.GetKota(data, function (reply) {
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
                        $scope.manufacturer = '';
                        $scope.model = '';
                        $scope.series = '';
                        $scope.bodyType = '';
                        $scope.vendor = '';
                        $scope.color = '';
                        $scope.currentPage = 1;
                        $scope.pageSize = 15;
                        $scope.totalRecords = 0;
                        $scope.getVehicle = function (param) {
                            $scope.serie = $scope.series
                            if ($scope.serie.indexOf(".") !== -1) {
                                // Code
                                $scope.serie = $scope.serie.replace(".", "~2a");
                            }
                            if ($scope.serie.indexOf("/") !== -1) {
                                // Code
                                $scope.serie = $scope.serie.replace("/", "~2f");
                            }
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            $scope.DataVehicle = {};
                            var data = "Benefit" + "|" + vm.newkota + "|" + vm.actualLevel +"|"+ $scope.manufacturer.toLowerCase() +"|" + $scope.model.toLowerCase() + "|" + $scope.serie.toLowerCase() + "|" + $scope.bodyType.toLowerCase() + "|" + $scope.vendor.toLowerCase() + "|" + $scope.color.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                            RelocationService.GetIdleFleet(data, function succ(reply) {
                                if (reply.status == 200) {
                                    $scope.DataVehicle = reply.data.List;
                                    $scope.totalRecords = reply.data.Count;
                                    console.info(reply.data);
                                }
                                else {
                                    UIControlService.unloadLoading();
                                    $.growl.error({ message: "Failed To Get Status CSF" })
                                }
                            });
                        };
                        $scope.batal = function () {
                            $uibModalInstance.dismiss('cancel');
                        };
                        $scope.simpan = function (param) {
                            vm.selectedVehicle = [];
                            var data = {
                                policeNumber: param.police_number,
                                manufacturer: param.manufacturer,
                                model: param.model,
                                series: param.series,
                                body_type: param.body_type,
                                color: param.color,
                                vendor: param.vendor_name,
                                startContract: vm.dateFormat(param.start_date),
                                endContract: vm.dateFormat(param.end_date)
                            };
                            vm.selectedVehicle.push(data);
                            $uibModalInstance.dismiss('cancel');
                            console.info(param);
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
