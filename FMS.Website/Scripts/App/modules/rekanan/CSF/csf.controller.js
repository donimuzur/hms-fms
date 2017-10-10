(function () {
    'use strict';

    angular.module("app").controller("CSFCtrl", ctrl);

    ctrl.$inject = ['$http', '$rootScope', '$scope', '$translate','$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'CSFService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService'];

    function ctrl($http, $rootScope, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, CSFService, 
        UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = this;
        vm.CC;
        vm.reasonCSF
        vm.groupLevel;
        vm.request_number = $stateParams.rq;
        vm.coordinator = localStorage.getItem('username');
        vm.id_coordinator;
        vm.reason_id;
        vm.reason;
        vm.effective_date;
        vm.expected_date;
        vm.endRent_date;
        vm.project = 'no';
        vm.roles = localStorage.getItem('roles');
        var today = new Date();
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
            var dformat = vm.dateFormat(param) + ' ' +
                  [params.getHours().padLeft(),
                    params.getMinutes().padLeft(),
                    params.getSeconds().padLeft()].join(':');
            return dformat;
        }
        vm.csf_date = dateFormat(today);
        vm.employee_id;
        vm.vehicle_type_id;
        vm.location_id;
        vm.expected_date;
        vm.Vtype = "Benefit";
        vm.statusCSF
        vm.Kategori;
        vm.Usage;
        vm.purpose = "test";
        vm.vat = "Test";
        vm.admin;
        vm.selectedVehicle = [];
        vm.manufacturer;
        vm.model;
        vm.series;
        vm.color;
        vm.body_type;
        vm.vcategory;
        vm.dataVehicleType;
        vm.getVehicleType = getVehicleType;
        function getVehicleType() {
            CSFService.GetVehicleType(function (reply) {
                console.info(reply.data);
                vm.dataVehicleType = reply.data;
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
        }
        vm.isCalendarOpened = [false, false, false];
        console.info($stateParams);
        if($stateParams.rq!="$s"){
            vm.admin=true;
        }
        else {
            vm.admin=false;
        }
        console.info(vm.admin);
        vm.init = init;
        function init() {
            CSFService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                console.info(vm.id_coordinator);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.getRequestNumber();
            vm.loadCSF();
            vm.getVehicleType();
        }
        vm.Temp = Temp
        function Temp(param) {
            var expected = vm.expected_date;
            var effective = new Date(param.effective_date);
            if (expected > effective) {
                vm.temp = true;
            }
            else {
                vm.temp = false;
            }
            console.info(vm.temp);
            
        }
        vm.tempLink = tempLink;
        function tempLink(param) {
            param = param.replaceAll('/', 'A');
            return param;

        }
        vm.getRequestNumber = getRequestNumber;
        function getRequestNumber() {
            CSFService.GetRequestNumber(function succ(reply) {
                if (reply.status == 200) {
                    vm.dataReference = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Data Temporary" })
                }
            });
        };
        vm.getTemporary = getTemporary;
        function getTemporary(param) {
            var data = {
                csfNumber: param
            }
            CSFService.GetTemp(data, function succ(reply) {
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
        vm.getCoord = getCoord;
        function getCoord(id) {
            CSFService.GetCoordinatorByID(id, function succ(reply) {
                    if (reply.status == 200) {
                        vm.coordinator = reply.data;
                        console.info(reply.data);
                    }
                    else {
                        UIControlService.unloadLoading();
                        $.growl.error({ message: "Failed To Get Coordinator ID" })
                    }
                }
             );
        }
        vm.getLocation = getLocation;
        function getLocation(id) {
            vm.location_id = id;
            console.info(id);
            CSFService.GetKotaByID(id, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.kota = reply.data.city;
                    vm.alamat = reply.data.address;
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Location Employee" })
                }
            }
             );
        }
        vm.setDate = setDate;
        function setDate(param) {
            if (param.expected_date != null) {
                vm.expected_date = new Date(param.expected_date);
            }
            vm.effective_date = new Date(param.effective_date);
        }
        vm.getStatus = getStatus;
        function getStatus(idStatus) {
            CSFService.GetStatus(idStatus,function succ(reply) {
                if (reply.status == 200) {
                    vm.statusCSF = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Status CSF" })
                }
            }
            );
        }
        vm.getReasonCSF = getReasonCSF;
        function getReasonCSF(){
            CSFService.GetReasonCSF(function succ(reply){
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.reasonCSF = reply.data;
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Data Reason CSF" })
                }
            },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " + JSON.stringify(error));
                }
            );
        }
        vm.loadCategory = loadCategory;
        vm.datakategori=[];
        function loadCategory(param) {
            console.info(param);
            var data;
            var i = param;
            vm.datakategori = [];
            if (param != 0) {
                do {
                    if (i == param) {
                        data = {
                            value: i,
                            category: "As Entitled : Car Group " + i,
                        }
                    }
                    else if (i != 0) {
                        data = {
                            value: i,
                            category: "FlexiBenefit : Car Group " + i,
                        }
                    }
                    vm.datakategori.push(data);
                    i--;
                }
                while (i != 0);
                data = {
                    value: 0,
                    category: "No Car",
                };
                vm.datakategori.push(data);
            }
        }
        vm.getLog = getLog;
        function getLog(idStatus) {
            CSFService.GetLog(idStatus, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.log=reply.data;
                    //for (var i=0; i<reply.data.length; i++) {
                    //    vm.log = vm.log + "[" +reply.data[i].log_date+ "]" + " " + reply.data[i].user_name + " " + reply.data[i].status +'\n';
                    //}
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

        vm.getReason = getReason;
        function getReason(id) {
            CSFService.GetReason(id, function succ(reply) {
                if (reply.status == 200) {
                    vm.reasonCSF = reply.data[0]['reason1'];
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Reason CSF" })
                }
            },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " + JSON.stringify(error));
                }
            );
        }
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }
        String.prototype.replaceAll = function (search, replacement) {
            var target = this;
            return target.replace(new RegExp(search, 'g'), replacement);
        };
        vm.loadCSF = loadCSF;
        function loadCSF()
        {
            console.info(vm.request_number);
            vm.dataCSF = {};
            CSFService.GetCSF(
                vm.request_number,
                function succ(reply) {
                    if (reply.status == 200) {
                        if (reply.data[0].vehicle_type == "Benefit") {
                            vm.dataCSF = reply.data;
                            console.info(vm.dataCSF);
                            vm.fleet_id = reply.data[0].fleet_id;
                            console.info(vm.fleet_id);
                        }
                        else {
                            var link = reply.data[0].request_number.replaceAll('/', 'A');
                            $state.go('CSF-WTC', { id: link });
                        }
                    }
                    else {
                        UIControlService.unloadLoading();
                        vm.dataCSF=null;
                        $.growl.error({ message: "Failed To Get Data CSF" })
                    }
                },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " + JSON.stringify(error));
                    vm.dataCSF = null;
                }
            );
        }
        vm.ambil = 0;
        vm.noCar = noCar;
        function noCar(param) {
            vm.ambil = param;
            if (vm.ambil == 0) {
                vm.selectedVehicle = [];
            }
            console.info(vm.ambil);
        }
        vm.noProject = noProject;
        function noProject(param) {
            console.info(param);
            vm.project = param;
        }
        vm.sendRequest = sendRequest;
        function sendRequest(param) {
            if (param.vehicle_category == null) {
                console.info();
                if (vm.Kategori == undefined) {
                    UIControlService.msg_growl('warning', 'Vehicle Category Not Selected');
                }
                else if (vm.Kategori != 0 && vm.Usage == undefined) {
                    UIControlService.msg_growl('warning', 'Vehicle Usage Not Selected');
                }
                else if (vm.Kategori != 0 && vm.selectedVehicle.length == 0) {
                    UIControlService.msg_growl('warning', 'Vehicle Not Selected');
                }
                else if (vm.Kategori != 0 && vm.location_id == undefined) {
                    UIControlService.msg_growl('warning', 'Location Not Selected');
                }
                else {
                    var data = {
                        csfID: param.csf_id,
                        vehicleType: vm.Vtype,
                        vehicleUsage: vm.Usage,
                        locationID: vm.location_id,
                        vehicleCategory: vm.ambil,
                        manufacturer: vm.manufacturer,
                        series: vm.series,
                        bodyType: vm.body_type,
                        color: vm.color,
                        model: vm.model,
                        role: vm.roles,
                        remark: 0
                    }
                    CSFService.UpdateCSF(data, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Send CSF Assigned Success");
                            var link = vm.request_number;
                            $state.reload();
                        } else {
                            UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        }
                        UIControlService.unloadLoading();
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", "ERR_SAVE");
                    });
                }
            }
            else {
                if (Number(param.vehicle_category) != 0 && vm.location_id == undefined) {
                    vm.location_id = param.location_id
                }
                if (Number(param.vehicle_category) != 0 && param.vehicle_usage == null) {
                    UIControlService.msg_growl('warning', 'Vehicle Usage Not Selected');
                }
                else if (Number(param.vehicle_category) != 0 && param.manufacturer == null) {
                    UIControlService.msg_growl('warning', 'Vehicle Not Selected');
                }
                else if (Number(param.vehicle_category) != 0 && vm.location_id == null) {
                    UIControlService.msg_growl('warning', 'Location Not Selected');
                }
                else {
                    console.info(param.vehicle_usage);

                    var data = {
                        csfID: param.csf_id,
                        vehicleUsage: param.vehicle_usage,
                        locationID: vm.location_id,
                        vehicleCategory: Number(param.vehicle_category),
                        manufacturer: param.manufacturer,
                        series: param.series,
                        bodyType: param.body_type,
                        color: param.color,
                        model: param.model,
                        role: vm.roles,
                        remark: 0
                    }
                    CSFService.UpdateCSF(data, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Save  CSF Success");
                            $state.reload();
                        } else {
                            UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        }
                        UIControlService.unloadLoading();
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", "ERR_SAVE");
                    });
                }
            }
            
        }
        vm.accept = accept;
        vm.declineHR = declineHR;
        vm.declineFleet = declineFleet;
        vm.progress = progress;
        vm.cancel = cancel;
        function cancel(id) {
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size : 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        CSFService.GetRemark(function (reply) {
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
                        if ($scope.remark==undefined) {
                            UIControlService.msg_growl("error", "Remark Must Be Selected");
                        }
                        else {
                            var data = {
                                csfID: id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            CSFService.CSFCancel(data, function (reply) {
                                if (reply.status === 200) {
                                    vm.init();
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
        function accept(csf) {
            //CSFService.SendMail();
            console.info(csf.vehicle_category);
            CSFService.CSFApproved({
                csfID: csf.csf_id,
                role: vm.roles,
                category: csf.vehicle_category
            }
            , function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", "CSF Approved");
                    vm.init();
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
        }
        function declineHR(id) {
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size: 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        CSFService.GetRemark(function (reply) {
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
                                csfID: id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            CSFService.CSFRejectedByHR(data, function (reply) {
                                if (reply.status === 200) {
                                    UIControlService.msg_growl("notice", "CSF Rejected By HR");
                                    vm.init();
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
        function declineFleet(id) {
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size: 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        CSFService.GetRemark(function (reply) {
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
                                csfID: id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            CSFService.CSFRejectedByFleet(data, function (reply) {
                                if (reply.status === 200) {
                                    UIControlService.msg_growl("notice", "CSF Rejected By Fleet");
                                    var link = vm.request_number;
                                    $state.go('csf', { id: link });
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
        function progress(id) {
            //CSFService.SendMail();
            CSFService.CSFInprogress({
                csfID: id,
                role: vm.roles,
                coordinator2: vm.id_coordinator
            }
            , function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", "CSF Approved By Fleet");
                    $state.reload();
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
            console.info(id);
        }
        vm.save = save;
        function save(param) {
            console.info(param);
            if (param.vehicle_category == null) {
                
                if (vm.Kategori == undefined) {
                    UIControlService.msg_growl('warning', 'Vehicle Category Not Selected');
                }
                else if (vm.Kategori != 0 && vm.Usage == undefined) {
                    UIControlService.msg_growl('warning', 'Vehicle Usage Not Selected');
                }
                else if (vm.Kategori != 0 && vm.selectedVehicle.length == 0 ) {
                    UIControlService.msg_growl('warning', 'Vehicle Not Selected');
                }
                else if(vm.Kategori != 0&&vm.location_id==undefined){
                    UIControlService.msg_growl('warning', 'Location Not Selected');
                }
                else {
                    var data={
                        csfID: param.csf_id,
                        vehicleType: vm.Vtype,
                        vehicleUsage: vm.Usage,
                        locationID: vm.location_id,
                        vehicleCategory: vm.Kategori,
                        manufacturer: vm.manufacturer,
                        series: vm.series,
                        bodyType: vm.body_type,
                        color: vm.color,
                        model: vm.model,
                        role: vm.roles,
                        remark: 0
                    }
                    console.info(data);
                    CSFService.SaveCSFByUser(data, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Save CSF Assigned Success");
                            $state.reload();
                        } else {
                            UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        }
                        UIControlService.unloadLoading();
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", "ERR_SAVE");
                    });
                }
            }
            else {
                console.info(param.vehicle_category);
                if (Number(param.vehicle_category) != 0 && vm.location_id == undefined) {
                    vm.location_id = param.location_id
                }
                if (Number(param.vehicle_category) != 0 && param.vehicle_usage == null) {
                    UIControlService.msg_growl('warning', 'Vehicle Usage Not Selected');
                }
                else if (Number(param.vehicle_category) != 0 && param.manufacturer == null) {
                    UIControlService.msg_growl('warning', 'Vehicle Not Selected');
                }
                else if (Number(param.vehicle_category) != 0 && vm.location_id == null) {
                    UIControlService.msg_growl('warning', 'Location Not Selected');
                }
                else {
                    var data = {
                        csfID: param.csf_id,
                        vehicleUsage: param.vehicle_usage,
                        locationID: vm.location_id,
                        vehicleCategory: Number(param.vehicle_category),
                        manufacturer: param.manufacturer,
                        series: param.series,
                        bodyType: param.body_type,
                        color: param.color,
                        model: param.model,
                        role: vm.roles,
                        remark: 0
                    }
                    console.info(data);
                    CSFService.SaveCSFByUser(data, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Save CSF Assigned Success");
                            $state.reload();
                        } else {
                            UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        }
                        UIControlService.unloadLoading();
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", "ERR_SAVE");
                    });
                }
            }
            
        }
        vm.sendDraft = sendDraft;
        function sendDraft() {
            var data;
            var d = new Date();
            if (document.getElementById('csfID') == null) {
                if (vm.referenceNumber == undefined) {
                    vm.referenceNumber = "";
                }
                if (vm.employee_id == undefined) {
                    UIControlService.msg_growl("error", "Employee Not Selected");
                }
                else if (vm.effective_date == undefined) {
                    UIControlService.msg_growl("error", "Effective Date Not Selected");
                }
                else if (vm.reason == undefined) {
                    UIControlService.msg_growl("error", "Reason Not Selected");
                }
                else {
                    data = {
                        csfID: "",
                        csfNumber: "",
                        statusID: 5,
                        coordinator: vm.id_coordinator,
                        employeeID: vm.employee_id,
                        employeeName: vm.employee,
                        costCenter: vm.CC,
                        groupLevel: vm.groupLevel,
                        effectiveDate: vm.effective_date,
                        reasonID: vm.reason,
                        role: vm.roles,
                        remark: 0,
                        referenceNumber: vm.referenceNumber,
                        receiverName:vm.employee
                    };
                    CSFService.SaveMenu(data, function (reply) {
                        if (reply.status === 200) {
                            CSFService.SendMail(data);
                            $state.go('dashboard-hr');
                        } else {
                            $.growl.error({ message: "Send CSF Draft Failed" });
                        }
                    }, function (err) {
                        console.info("error:" + JSON.stringify(err));
                    });
                }
            }
            else {
                var employeeID = document.getElementById('empID').value;
                var employeeName = document.getElementById('empName').value;
                var employeeCC = document.getElementById('empCC').value;
                var employeeGroup = document.getElementById('empGroup').value;
                var effectiveDate = vm.dateFormat(vm.effective_date);
                var csfID = document.getElementById('csfID').value;
                var csfNumber = document.getElementById('csfNumber').value;
                var e = document.getElementById('reasonCSF');
                var reason = e.options[e.selectedIndex].value;
                if (employeeID==undefined) {
                    UIControlService.msg_growl("error", "Employee Not Selected");
                }
                else if (reason == undefined) {
                    UIControlService.msg_growl("error", "Reason Not Selected");
                }
                else {
                    data = {
                        csfID: csfID,
                        csfNumber: csfNumber,
                        statusID: 5,
                        coordinator: vm.id_coordinator,
                        employeeID: employeeID,
                        employeeName: employeeName,
                        costCenter: employeeCC,
                        groupLevel: employeeGroup,
                        effectiveDate: effectiveDate,
                        reasonID: reason,
                        role: vm.roles,
                        remark: 0,
                        receiverName: vm.employeeame

                    }
                    CSFService.SaveMenu(data, function (reply) {
                        if (reply.status === 200) {
                            CSFService.SendMail(data);
                            $state.go('dashboard-hr');
                        } else {
                            $.growl.error({ message: "Send CSF Draft Failed" });
                        }
                    }, function (err) {
                        console.info("error:" + JSON.stringify(err));
                    });
                }
            }
        }
        vm.SaveReference = SaveReference;
        function SaveReference(param) {
            var data = {
                csfID: param.csf_id,
                csfNumber: param.request_number,
                referenceNumber:param.reference_number
            }
            CSFService.SaveReference(data, function (reply) {
                if (reply.status === 200) {
                    vm.init();
                } else {
                    $.growl.error({ message: "Save CSF Draft Failed" });
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            console.info(param.reference_number);
        }
        vm.saveDraft = saveDraft;
        function saveDraft() {
            var data;
            var d = new Date();
            if (document.getElementById('csfID') == null) {
                if(vm.referenceNumber==undefined){
                    vm.referenceNumber = "";
                }
                if (vm.employee_id == undefined) {
                    UIControlService.msg_growl("error", "Employee Not Selected");
                }
                else if (vm.effective_date == undefined) {
                    UIControlService.msg_growl("error", "Effective Date Not Selected");
                }
                else if (vm.reason == undefined) {
                    UIControlService.msg_growl("error", "Reason Not Selected");
                }
                else {
                    data = {
                        csfID: "",
                        csfNumber: "",
                        statusID: 4,
                        coordinator: vm.id_coordinator,
                        employeeID: vm.employee_id,
                        employeeName: vm.employee,
                        costCenter: vm.CC,
                        groupLevel: vm.groupLevel,
                        effectiveDate: vm.effective_date,
                        reasonID: vm.reason,
                        remark: 0,
                        referenceNumber:vm.referenceNumber
                    }
                    console.info(data);
                    CSFService.SaveDraft(data, function (reply) {
                        if (reply.status === 200) {
                            $state.go('dashboard-hr');
                        } else {
                            $.growl.error({ message: "Save CSF Draft Failed" });
                        }
                    }, function (err) {
                        console.info("error:" + JSON.stringify(err));
                    });
                    console.info(data);
                }
            }
            else {
                var employeeID = document.getElementById('empID').value;
                var employeeName = document.getElementById('empName').value;
                var employeeCC = document.getElementById('empCC').value;
                var employeeGroup = document.getElementById('empGroup').value;
                var csfID = document.getElementById('csfID').value;
                var csfNumber = document.getElementById('csfNumber').value;
                var e = document.getElementById('reasonCSF');
                var reason = e.options[e.selectedIndex].value;
                if (employeeID==undefined) {
                    UIControlService.msg_growl("error", "Employee Not Selected");
                }
                else if (reason == undefined) {
                    UIControlService.msg_growl("error", "Reason Not Selected");
                }
                else {
                    data = {
                        csfID: csfID,
                        csfNumber: csfNumber,
                        statusID: 4,
                        coordinator: vm.id_coordinator,
                        employeeID: employeeID,
                        employeeName: employeeName,
                        costCenter: employeeCC,
                        groupLevel: employeeGroup,
                        effectiveDate: vm.dateFormat(vm.effective_date),
                        reasonID: reason,
                        remark: 0
                    }
                    CSFService.SaveDraft(data, function (reply) {
                        if (reply.status === 200) {
                            $state.go('dashboard-hr');
                        } else {
                            $.growl.error({ message: "Save CSF Draft Failed" });
                        }
                    }, function (err) {
                        console.info("error:" + JSON.stringify(err));
                    });
                    console.info(data);
                }
            }
        }
        vm.suplyMethod;
        vm.sendFleet = sendFleet;
        function sendFleet(param) {;
            console.info(param);
            if (vm.expected_date == null) {
                UIControlService.msg_growl("Error", "Expected Date Not Selected");
            }
            else {
                var data = {
                    csfID: param.csf_id,
                    expectedDate:vm.dateFormat(vm.expected_date)
                };
                CSFService.SaveExpected(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Save Expected date Success");
                        vm.init();
                    } else {
                        $.growl.error({ message: "Error" });
                    }
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                });
            }
        }
        vm.employee;
        vm.openEmployee = openEmployee;
        function openEmployee() {
            console.info("asdd");
            var modalInstance = $uibModal.open({
                templateUrl: 'employee.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.EmployeeID="";
                    $scope.EmployeeName = "";
                    $scope.EmployeeCost = "";
                    $scope.EmployeeGroup = "";
                    $scope.EmployeePosition = "";
                    $scope.currentPage = 1;
                    $scope.pageSize = 15;
                    $scope.totalRecords = 0;
                    $scope.getEmployee = function (param) {
                        $scope.employees = {};
                        $scope.currentPage = param;
                        $scope.offset = (param * 10) - 10;
                        var data = $scope.EmployeeID.toLowerCase() + "|" + $scope.EmployeeName.toLowerCase() + "|" + $scope.EmployeeCost.toLowerCase() + "|" + $scope.EmployeeGroup + "|" + $scope.EmployeePosition.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                        CSFService.GetEmployee(data,function (reply) {
                            if (reply.status === 200) {
                                $scope.employees = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            } else {
                                $.growl.error({ message: "Failed To Get Employee" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                        console.info();
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.select = function (param) {
                        if (document.getElementById('csfID') != null) {
                            document.getElementById('empID').value = param.employee_id;
                            document.getElementById('empName').value = param.employee_name;
                            document.getElementById('empCC').value = param.cost_center;
                            document.getElementById('empGroup').value = param.grup_level
                        }
                        else {
                            vm.employee_id = param.employee_id;
                            vm.employee = param.employee_name;
                            vm.CC = param.cost_center;
                            vm.groupLevel = param.grup_level;

                        }
                        $uibModalInstance.dismiss('cancel');
                    };

                }
            });
        };
        vm.uploadVehicle = uploadVehicle;
        function uploadVehicle() {
            var modalInstance = $uibModal.open({
                templateUrl: 'csfUploadVehicle.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.filevehicle;
                    $scope.data = [];
                    $scope.select = function (file) {
                        $scope.filevehicle = file;
                    };
                    $scope.uploadFile = function () {
                        $scope.upload($scope.filevehicle);
                    };
                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file, function (reply) {
                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];
                                console.info(sheet1);
                                if (sheet1[2].Column2 != "Manufacturer") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[2].Column2);
                                    UIControlService.msg_growl("error", "It Must Be Manufacturer");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column3 != "Model") {
                                    UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[2].Column3);
                                    UIControlService.msg_growl("error", "It Must Be Model");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column4 != "Series") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[2].Column4);
                                    UIControlService.msg_growl("error", "It Must Be Series");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column5 != "Body Type") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[2].Column5);
                                    UIControlService.msg_growl("error", "It Must Be Body Type");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column6 != "Color") {
                                    UIControlService.msg_growl("error", "Column 6 Name Is " + sheet1[2].Column6);
                                    UIControlService.msg_growl("error", "It Must Be Color");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                var item = {
                                    manufacturer: sheet1[3].Column2,
                                    model: sheet1[3].Column3,
                                    series: sheet1[3].Column4,
                                    body_type: sheet1[3].Column5,
                                    color: sheet1[3].Column6,
                                };
                                if (!item.manufacturer) {
                                    UIControlService.msg_growl("error", "Item Line " + (4) + " Not valid:");
                                    UIControlService.msg_growl("error", "Manufacturer Must be Filled");
                                    $scope.data = [];
                                }
                                if (!item.model) {
                                    UIControlService.msg_growl("error", "Item Line " + (4) + " Not valid:");
                                    UIControlService.msg_growl("error", "Model Must be Filled");
                                    $scope.data = [];
                                }
                                if (!item.series) {
                                    UIControlService.msg_growl("error", "Item Line " + (4) + " Not valid:");
                                    UIControlService.msg_growl("error", "Series Must be Filled");
                                    $scope.data = [];
                                }
                                if (!item.body_type) {
                                    UIControlService.msg_growl("error", "Item Line " + (4) + " Not valid:");
                                    UIControlService.msg_growl("error", "Body Type Must be Filled");
                                    $scope.data = [];
                                }
                                if (!item.color) {
                                    UIControlService.msg_growl("error", "Item Line " + (4) + " Not valid:");
                                    UIControlService.msg_growl("error", "Color Must be Filled");
                                    $scope.data = [];
                                }
                                $scope.data.push(item);
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
                    $scope.pilih = function (param) {
                        console.info(param);
                        vm.selectedVehicle = [];
                        vm.selectedVehicle = param;
                        vm.manufacturer = param.manufacturer;
                        vm.model = param.model;
                        vm.series = param.series;
                        vm.color = param.color;
                        vm.body_type = param.body_type;
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
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
                        if (vm.fleet_id != null) {
                            CSFService.GetFleetCSF(vm.fleet_id, function (reply) {
                                if (reply.status === 200) {
                                    $scope.fleetcsf = reply.data;
                                    console.info($scope.fleetcsf);
                                }
                            });
                        }
                    };
                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file,function (reply) {
                        if (reply.status === 200) {
                            var excelContents = reply.data;
                            var sheet1 = excelContents[Object.keys(excelContents)[0]];
                            console.info(sheet1[2].Column2);
                            if (sheet1[2].Column2!="Request Number") {
                                UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[2].Column2);
                                UIControlService.msg_growl("error", "It Must Be Request Number");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column3 != "Employee Name") {
                                UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[2].Column3);
                                UIControlService.msg_growl("error", "It Must Be Employee Name");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column4 != "Vendor") {
                                UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[2].Column4);
                                UIControlService.msg_growl("error", "It Must Be Vendor");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column5 != "Police Number") {
                                UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[2].Column5);
                                UIControlService.msg_growl("error", "It Must Be Police Number");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column6 != "Chasis Number") {
                                UIControlService.msg_growl("error", "Column 6 Name Is " + sheet1[2].Column6);
                                UIControlService.msg_growl("error", "It Must Be Chasis Number");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column7 != "Engine Number") {
                                UIControlService.msg_growl("error", "Column 7 Name Is " + sheet1[2].Column7);
                                UIControlService.msg_growl("error", "It Must Be Engine Number");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column8 != "Contract Start Date") {
                                UIControlService.msg_growl("error", "Column 8 Name Is " + sheet1[2].Column8);
                                UIControlService.msg_growl("error", "It Must Be Contract Start Date");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column9 != "Contract End Date") {
                                UIControlService.msg_growl("error", "Column 9 Name Is " + sheet1[2].Column9);
                                UIControlService.msg_growl("error", "It Must Be Contract End Date");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column10 != "AirBag") {
                                UIControlService.msg_growl("error", "Column 10 Name Is " + sheet1[2].Column10);
                                UIControlService.msg_growl("error", "It Must Be AirBag");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column11 != "Make") {
                                UIControlService.msg_growl("error", "Column 11 Name Is " + sheet1[2].Column11);
                                UIControlService.msg_growl("error", "It Must Be Make");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column12 != "Model") {
                                UIControlService.msg_growl("error", "Column 12 Name Is " + sheet1[2].Column12);
                                UIControlService.msg_growl("error", "It Must Be Model");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column13 != "Series") {
                                UIControlService.msg_growl("error", "Column 13 Name Is " + sheet1[2].Column13);
                                UIControlService.msg_growl("error", "It Must Be Series");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column14 != "Transmission") {
                                UIControlService.msg_growl("error", "Column 14 Name Is " + sheet1[2].Column14);
                                UIControlService.msg_growl("error", "It Must Be Transmission");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column15 != "Color") {
                                UIControlService.msg_growl("error", "Column 15 Name Is " + sheet1[2].Column15);
                                UIControlService.msg_growl("error", "It Must Be Color");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column16 != "Body type") {
                                UIControlService.msg_growl("error", "Column 16 Name Is " + sheet1[2].Column16);
                                UIControlService.msg_growl("error", "It Must Be Body type");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column17 != "Branding") {
                                UIControlService.msg_growl("error", "Column 17 Name Is " + sheet1[2].Column17);
                                UIControlService.msg_growl("error", "It Must Be Branding");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column18 != "Purpose") {
                                UIControlService.msg_growl("error", "Column 18 Name Is " + sheet1[2].Column18);
                                UIControlService.msg_growl("error", "It Must Be Purpose");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column19 != "Vehicle Year") {
                                UIControlService.msg_growl("error", "Column 19 Name Is " + sheet1[2].Column19);
                                UIControlService.msg_growl("error", "It Must Be Vehicle Year");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column20 != "PO") {
                                UIControlService.msg_growl("error", "Column 20 Name Is " + sheet1[2].Column20);
                                UIControlService.msg_growl("error", "It Must Be PO");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column21 != "PO Line") {
                                UIControlService.msg_growl("error", "Column 21 Name Is " + sheet1[2].Column21);
                                UIControlService.msg_growl("error", "It Must Be PO Line");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column22 != "Vat") {
                                UIControlService.msg_growl("error", "Column 22 Name Is " + sheet1[2].Column22);
                                UIControlService.msg_growl("error", "It Must Be Vat");
                                $uibModalInstance.dismiss('cancel');
                            }
                            if (sheet1[2].Column23 != "Restituion") {
                                UIControlService.msg_growl("error", "Column 23 Name Is " + sheet1[2].Column23);
                                UIControlService.msg_growl("error", "It Must Be Restituion");
                                $uibModalInstance.dismiss('cancel');
                            }
                            for (var i = 3; i < sheet1.length; i++) {
                                var item = {
                                    csf: sheet1[i].Column2,
                                    employeeName : sheet1[i].Column3,
                                    vendor: sheet1[i].Column4,
                                    policeNumber: sheet1[i].Column5,
                                    chasisNumber: sheet1[i].Column6,
                                    engineNumber: sheet1[i].Column7,
                                    contractStart:sheet1[i].Column8,
                                    contractEnd: sheet1[i].Column9,
                                    airbag: sheet1[i].Column10,
                                    make: sheet1[i].Column11,
                                    model: sheet1[i].Column12,
                                    series: sheet1[i].Column13,
                                    transmission: sheet1[i].Column14,
                                    color: sheet1[i].Column15,
                                    bodyType: sheet1[i].Column16,
                                    branding: sheet1[i].Column17,
                                    purpose: sheet1[i].Column18,
                                    vehicleYear: sheet1[i].Column19,
                                    po: sheet1[i].Column20,
                                    poLine: sheet1[i].Column21,
                                    vat: sheet1[i].Column22,
                                    restituion: sheet1[i].Column23,
                                };
                                console.info(item.contractStart instanceof Date);
                                //validasi
                                if (!item.csf) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Request Number Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.employeeName) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Employee Name Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.vendor) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Vendor Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.policeNumber) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Police Number Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.chasisNumber) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Chasis Number Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.engineNumber) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Engine Number Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.contractStart) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Contract Start Date diisi");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.contractEnd) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Contract End Date Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (typeof (item.airbag) != "boolean" || !item.airbag) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Airbag Must be Filled/Format Salah");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.make) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Make Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.model) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Model Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.series) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Series Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.transmission) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Transmission Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.color) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Color Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.bodyType) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Body Type Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.branding) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Branding Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.purpose) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Purpose Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.vehicleYear) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Vehicle Year Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.po) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "PO Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (!item.poLine) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "PO Line Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (typeof (item.vat) != "boolean"||!item.vat) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Vat Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
                                if (typeof (item.restituion) != "boolean"||!item.restituion) {
                                    UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                    UIControlService.msg_growl("error", "Restituion Must be Filled");
                                    $scope.data = [];
                                    break;
                                }
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
                        if ($scope.data.length>0) {
                            CSFService.SaveFleet( 
                                    $scope.data
                                ,function (reply) {
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
                            UIControlService.msg_growl("error", "There Is No Data To Stored")
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
        vm.alamat;
        vm.costCenter = costCenter;
        function costCenter() {
            var modalInstance = $uibModal.open({
                templateUrl: 'csfSelectCity.html',
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
                        var data = $scope.city.toLowerCase() + "|" + $scope.address.toLowerCase() + "|"+ $scope.offset + "|" + $scope.pageSize;
                        CSFService.GetKota(data, function (reply) {
                            if (reply.status === 200) {
                                $scope.CostCenter = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                                console.info($scope.CostCenter);
                            } else {
                                $.growl.error({ message: "Failed To Get Location" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });  
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.Kota = function (param,param1,param2) {
                        vm.location_id = param;
                        vm.kota = param1;
                        vm.alamat = param2
                        $uibModalInstance.dismiss('cancel');
                        console.info(vm.kota);
                    };

                }
            });
        };
        vm.dataTemporary = [];
        vm.openTemporary = openTemporary;
        function openTemporary(param) {
            var csf = param;
            var modalInstance = $uibModal.open({
                templateUrl: 'temporary.html',
                resolve: {
                    test: function () {
                        return csf;
                    }
                },
                controller: function ($uibModalInstance, $scope,test) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.start_date;
                    $scope.end_date;
                    $scope.reason;
                    $scope.csf = test;
                    $scope.getReason = function () {
                        CSFService.GetTempReason(function (reply) {
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
                        if($scope.reason==undefined){
                            UIControlService.msg_growl("error", "Reason Must be Selected");
                        }
                        else if ($scope.start_date==undefined) {
                            UIControlService.msg_growl("error", "Start Date Must be Selected");
                        }
                        else if ($scope.end_date == undefined) {
                            UIControlService.msg_growl("error", "End Date Must be Selected");
                        }
                        else if ($scope.end_date < $scope.start_date) {
                            UIControlService.msg_growl("error", "End Date Can't Less Than Start Date");
                        }
                        else {
                            var data = {
                                csfNumber:$scope.csf.request_number,
                                startDate: $scope.start_date,
                                endDate:$scope.end_date,
                                reason: Number($scope.reason),
                                coordinator:vm.id_coordinator,
                                suplyMethod:"Temporary",
                                locationId:$scope.csf.location_id,
                                employeeName : $scope.csf.employee_name,
                                actualGroup : $scope.csf.vehicle_category,
                                costCenter: $scope.csf.cost_center,
                                GroupLevel:$scope.csf.group_level,
                                project: false,
                                vehicleType:"Benefit",
		                        employeeId: $scope.csf.employee_id,
                                role: vm.roles
                            }
                            vm.sendFleet($scope.csf);
                            console.info(data);
                            CSFService.SaveTemporary(data, function (reply) {
                                if (reply.status === 200) {
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
        vm.selectVehicle = selectVehicle;
        //vm.selectVehicleUser = selectVehicleUser;
        //function selectVehicleUser(csf) {
        //    console.info(csf.group_level);
        //    if (csf.vehicle_usage == null) {
        //        UIControlService.msg_growl("error", "Vehicle Usage Must be Selected");
        //    }
        //    else {
        //        var modalInstance = $uibModal.open({
        //            templateUrl: 'csfSelectVehicle.html',
        //            controller: function ($uibModalInstance, $scope) {
        //                $scope.manufacturer = '';
        //                $scope.model = '';
        //                $scope.series = '';
        //                $scope.bodyType = '';
        //                $scope.color = '';
        //                $scope.currentPage = 1;
        //                $scope.pageSize = 15;
        //                $scope.totalRecords = 0;
        //                $scope.loadVehicle = function (param) {
        //                    $scope.currentPage = param;
        //                    $scope.offset = (param * 10) - 10;
        //                    $scope.DataVehicle = {};
        //                    var data = "Benefit" + "|" + $scope.manufacturer.toLowerCase() + "|" + $scope.car_group_level + "|" + $scope.model.toLowerCase() + "|" + $scope.series.toLowerCase() + "|" + $scope.bodyType.toLowerCase() + "|" + $scope.color.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
        //                    console.info(data);
        //                    CSFService.GetVehicle(data, function (reply) {
        //                        if (reply.status === 200) {
        //                            $scope.DataVehicle = reply.data.List;
        //                            $scope.totalRecords = reply.data.Count;
        //                        } else {
        //                            $.growl.error({ message: "Failed To Get Data Vehicle" });
        //                        }
        //                    }, function (err) {
        //                        console.info("error:" + JSON.stringify(err));
        //                    });
        //                };
        //                $scope.loadFleet = function (param) {
        //                    $scope.currentPage = param;
        //                    $scope.offset = (param * 10) - 10;
        //                    $scope.DataVehicle = {};
        //                    var data = "Benefit" + "||" + csf.group_level + "|" + $scope.manufacturer.toLowerCase() + "|" + $scope.model.toLowerCase() + "|" + $scope.series.toLowerCase() + "|" + $scope.bodyType.toLowerCase() + "|" + $scope.color.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
        //                    console.info(data);
        //                    CSFService.GetFleet(data, function (reply) {
        //                        if (reply.status === 200) {
        //                            if (reply.data.Count > 0) {
        //                                $scope.DataVehicle = reply.data.List;
        //                                $scope.totalRecords = reply.data.Count;
        //                            }
        //                            else {
        //                                $scope.loadVehicle(param);
        //                            }
                                    
        //                        } else {
        //                            $.growl.error({ message: "Failed To Get Data Vehicle" });
        //                        }
        //                    }, function (err) {
        //                        console.info("error:" + JSON.stringify(err));
        //                    });
        //                }
        //                $scope.getVehicle = function (param) {
        //                    if (csf.vehicle_usage === "CFM") {
        //                        $scope.loadFleet(param);
        //                    }
        //                    else {
        //                        $scope.loadVehicle(param);
        //                    }
        //                };
        //                $scope.batal = function () {
        //                    $scope.DataVehicle = {};
        //                    $uibModalInstance.dismiss('cancel');
        //                };
        //                $scope.simpan = function (param) {
        //                    vm.selectedVehicle = [];
        //                    vm.selectedVehicle = param;
        //                    vm.manufacturer = param.manufacturer;
        //                    vm.model = param.model;
        //                    vm.series = param.series;
        //                    vm.color = param.color;
        //                    vm.body_type = param.body_type;
        //                    vm.vehicle_id = param.vehicle_specs_id;
        //                    vm.vcategory = param.car_group_level;
        //                    csf.manufacturer = param.manufacturer;
        //                    csf.model = param.model;
        //                    csf.series = param.series;
        //                    csf.color = param.color;
        //                    csf.body_type = param.body_type;
        //                    csf.vehicle_id = param.vehicle_specs_id;
        //                    csf.vcategory = param.car_group_level;
        //                    $uibModalInstance.dismiss('cancel');
        //                    console.info(vm.selectedVehicle);
        //                };
        //            }
        //        });
        //    }
        //};
        function selectVehicle(csf) {
            if (csf.vehicle_usage == null && vm.Usage == undefined) {
                UIControlService.msg_growl("error", "Vehicle Usage Must be Selected");
            }
            else {
                var modalInstance = $uibModal.open({
                    templateUrl: 'csfSelectVehicle.html',
                    resolve: {
                        test: function () {
                            return csf;
                        }
                    },
                    controller: function ($uibModalInstance, $scope,test) {
                        $scope.manufacturer = '';
                        $scope.model = '';
                        $scope.series = '';
                        $scope.bodyType = '';
                        $scope.color = '';
                        $scope.currentPage = 1;
                        $scope.pageSize = 15;
                        $scope.totalRecords = 0;
                        $scope.car_group = csf.vehicle_category;
                        if (csf.vehicle_category == null) {
                            $scope.car_group=vm.Kategori;
                        }
                        $scope.loadVehicle = function (param) {
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            $scope.DataVehicle = {};
                            var data = "Benefit" + "|" + $scope.manufacturer.toLowerCase() + "|" + $scope.car_group + "|" + $scope.model.toLowerCase() + "|" + $scope.serie.toLowerCase() + "|" + $scope.bodyType.toLowerCase() + "|" + $scope.color.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                            console.info(data);
                            CSFService.GetVehicle(data, function (reply) {
                                if (reply.status === 200) {
                                    $scope.DataVehicle = reply.data.List;
                                    $scope.totalRecords = reply.data.Count;
                                    $scope.isFleet = false;
                                } else {
                                    $.growl.error({ message: "Failed To Get Data Vehicle" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            });
                        };
                        $scope.loadFleet = function (param) {
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            $scope.DataVehicle = {};
                            var data = "Benefit" + "|" + $scope.manufacturer.toLowerCase() + "|" + $scope.car_group + "|" + $scope.model.toLowerCase() + "|" + $scope.serie.toLowerCase() + "|" + $scope.bodyType.toLowerCase() + "|" + $scope.color.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                            console.info(data);
                            CSFService.GetFleet(data, function (reply) {
                                if (reply.status === 200) {
                                    if (reply.data.Count > 0) {
                                        $scope.DataVehicle = reply.data.List;
                                        $scope.totalRecords = reply.data.Count;
                                        $scope.isFleet = true;
                                    }
                                    else {
                                        $scope.loadVehicle(param);
                                    }

                                } else {
                                    $.growl.error({ message: "Failed To Get Data Vehicle" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            });
                        }
                        $scope.getVehicle = function (param) {
                            $scope.serie=$scope.series
                            if ($scope.serie.indexOf(".") !== -1) {
                                // Code
                                $scope.serie = $scope.serie.replace(".", "~2a");
                            }
                            if ($scope.serie.indexOf("/") !== -1) {
                                // Code
                                $scope.serie = $scope.serie.replace("/", "~2f");
                            }
                            if (vm.Usage === "CFM"||csf.vehicle_usage==="CFM") {
                                $scope.loadFleet(param);
                            }
                            else {
                                $scope.loadVehicle(param);
                            }
                        };
                        $scope.batal = function () {
                            $scope.DataVehicle = {};
                            $uibModalInstance.dismiss('cancel');
                        };
                        $scope.simpan = function (param) {
                            vm.selectedVehicle = [];
                            vm.selectedVehicle = param;
                            vm.manufacturer = param.manufacturer;
                            vm.model = param.model;
                            vm.series = param.series;
                            vm.color = param.color;
                            vm.body_type = param.body_type;
                            vm.vehicle_id = param.vehicle_specs_id;
                            vm.vcategory = param.car_group_level;
                            csf.manufacturer = param.manufacturer;
                            csf.model = param.model;
                            csf.series = param.series;
                            csf.color = param.color;
                            csf.body_type = param.body_type;
                            csf.vehicle_id = param.vehicle_specs_id;
                            csf.vcategory = param.car_group_level;
                            if ($scope.isFleet) {
                                vm.vendor = param.vendor_name;
                                csf.vendor = param.vendor_name;
                            }
                            else{
                                vm.vendor="ASSA"
                                csf.vendor="ASSA"
                            }
                            $uibModalInstance.dismiss('cancel');
                            console.info(vm.selectedVehicle);
                        };
                    }
                });
            }
        };
    }
    
})();
