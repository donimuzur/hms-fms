(function () {
    'use strict';

    angular.module("app").controller("TemporaryCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', 'TemporaryService', '$state', '$stateParams',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, TemporaryService, $state, $stateParams,
        UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = $scope;
        vm.request_number = $stateParams.id;
        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles');
        vm.reason;
        vm.reference;
        vm.start_date;
        vm.end_date;
        var today = new Date();
        vm.dateFormat = dateFormat;
        String.prototype.replaceAll = function (search, replacement) {
            var target = this;
            return target.replace(new RegExp(search, 'g'), replacement);
        };
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
        vm.dateFormatLog = dateFormatLog;
        function dateFormatLog(param) {
            var params = new Date(param);
            var dformat = vm.dateFormat(param) + ' ' +
                  [params.getHours().padLeft(),
                    params.getMinutes().padLeft(),
                    params.getSeconds().padLeft()].join(':');
            return dformat;
        }
        vm.temp_date=dateFormat(today);
        vm.employee_id;
        vm.cc;
        vm.groupLevel;
        vm.vehicle_type_id;
        vm.location_id;
        vm.expected_date;
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
        vm.insertVehicle = insertVehicle;
        function insertVehicle(param) {
            console.info(param);
            vm.selectedVehicle = param;
        };
        vm.init = init
        function init() {
            TemporaryService.GetCoordinator(vm.coordinator, function (reply) {
                vm.coordinatorID = reply.data[0]['UserID'];
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.loadTemp();
            vm.getProject();
        }

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }
        vm.setDate = setDate;
        function setDate(param) {
            console.info(param.start_rent);
            vm.start_date = new Date(param.start_rent);
            vm.end_date = new Date(param.end_rent);
        }
        vm.insertEmployee = insertEmployee;
        function insertEmployee(param){
            vm.employee_id = param.employee_id;
            vm.employee = param.employee_name;
            vm.cc = param.cost_center;
            if (param.group_level != null) {
                vm.groupLevel = param.group_level;
                vm.loadCategory(vm.groupLevel);
            }
            vm.Type = param.vehicle_type;
            
        }
        vm.getStatus = getStatus;
        function getStatus(idStatus) {
            TemporaryService.GetStatus(idStatus, function succ(reply) {
                if (reply.status == 200) {
                    vm.statusCSF = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Gagal mengambil data CSF" })
                }
            }
            );
        }
        vm.loadTemp = loadTemp;
        function loadTemp()
        {
            console.info(vm.request_number);
            vm.data = {};
            TemporaryService.GetTemporary(
                vm.request_number,
                function succ(reply) {
                    if (reply.status == 200) {
                        vm.dataTemp = reply.data;
                        vm.fleet_id = reply.data[0].fleet_id;
                        console.info(vm.dataTemp);
                    }
                    else {
                        UIControlService.unloadLoading();
                        vm.dataCSF=null;
                        $.growl.error({ message: "Gagal mengambil data CSF" })
                    }
                },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " +JSON.stringify(error));
                }
            );
            vm.getReason();
        }
        vm.cancel = cancel;
        function cancel(id) {
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size: 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        TemporaryService.GetRemark(function (reply) {
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
                                temporaryFormID: id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            TemporaryService.CancelTemporary(data, function (reply) {
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
        vm.loadCategory = loadCategory;
        vm.datakategori = [];
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
            }
            
            console.info(vm.datakategori);
        }
        vm.getLog = getLog;
        function getLog(idStatus) {
            TemporaryService.GetChangeLog(idStatus, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.log = reply.data;
                    //for (var i=0; i<reply.data.length; i++) {
                    //    vm.log = vm.log + "[" +reply.data[i].log_date+ "]" + " " + reply.data[i].user_name + " " + reply.data[i].status +'\n';
                    //}
                    console.info(vm.log);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Gagal mengambil data log CSF" })
                }
            },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " + JSON.stringify(error));
                }
            );
        }
        vm.getLocation = getLocation;
        function getLocation(id) {
            console.info(id);
            TemporaryService.GetKotaByID(id, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.location_id = id;
                    vm.kota = reply.data.city;
                    vm.alamat = reply.data.address;
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Gagal mengambil data CSF" })
                }
            }
             );
        }
        vm.getProject = getProject;
        vm.dataProject;
        vm.dataIsProject = [
            { value: 1, is: true, name: "Yes" },
            { value: 0, is: false, name: "No" }
        ];
        function getProject() {
            TemporaryService.GetProject(function (reply) {
                vm.dataProject = reply.data;
                console.info(vm.dataProject);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
        }
        vm.dataVtype = [
            {name:"Benefit"},
            {name:"WTC"}
        ]
        vm.getReason = getReason;
        function getReason () {
            TemporaryService.GetTempReason(function (reply) {
                if (reply.status === 200) {
                    vm.dataReason = reply.data;
                } else {
                    $.growl.error({ message: "Error" });
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });

        };
        vm.saveDraft = saveDraft;
        function saveDraft() {
            if (document.getElementById('tempID') == null) {
                console.info("Null");
                if (vm.reason==undefined) {
                    UIControlService.msg_growl('error', 'Reason Must Be Selected');
                }
                else if (vm.employee_id==undefined) {
                    UIControlService.msg_growl('error', 'Employee Must Be Selected');
                }
                else if(vm.Type==undefined){
                    UIControlService.msg_growl('error', 'Vehicle Type Must Be Selected');
                }
                else if(vm.location_id==undefined){
                    UIControlService.msg_growl('error', 'Location Must Be Selected');
                }
                else if (vm.selectedVehicle.length == 0) {
                    UIControlService.msg_growl('error', 'Vehicle Must Be Selected');
                }
                else if (vm.start_date==undefined) {
                    UIControlService.msg_growl('error', 'Start Date Must Be Selected');
                }
                else if (vm.end_date == undefined) {
                    UIControlService.msg_growl('error', 'End Date Must Be Selected');
                }
                else if(vm.end_date<=vm.start_date){
                    UIControlService.msg_growl('error', "End Date Can't Less Than Start Date");
                }
                else if (vm.Type == "Benefit" && vm.ActualGroupLevel == undefined) {
                    UIControlService.msg_growl('error', 'Actual Group Level Must Be Selected');
                }
                else if (vm.Type != "Benefit" && vm.isProject == undefined) {
                    UIControlService.msg_growl('error', 'Project Must Be Selected');
                }
                else if(vm.Type != "Benefit"&&vm.isProject==1&&vm.project == undefined){
                    UIControlService.msg_growl('error', 'Project Name Must Be Selected');
                }
                else {
                    if (vm.Type == "Benefit") {
                        vm.isProject = 0;
                    }
                    if (vm.isProject == 0) {
                        vm.project = null;
                    }
                    var data = {
                        temporaryID:0,
                        requestDate: vm.temp_date,
                        coordinator: vm.coordinatorID,
                        reason: vm.reason,
                        employeeID: vm.employee_id,
                        employeeName: vm.employee,
                        costCenter: vm.cc,
                        employeeGroup: vm.groupLevel,
                        actualGroup: vm.ActualGroupLevel,
                        vehicleType: vm.Type,
                        isProject: Number(vm.isProject),
                        project: Number(vm.project),
                        location: vm.location_id,
                        manufacturer: vm.selectedVehicle.manufacturer,
                        model: vm.selectedVehicle.model,
                        series: vm.selectedVehicle.series,
                        bodyType: vm.selectedVehicle.body_type,
                        color: vm.selectedVehicle.color,
                        startRent: vm.start_date,
                        endRent: vm.end_date,
                        supplyMethod: "Temporary",
                        role:vm.roles

                    }
                    TemporaryService.SaveDraft(data, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Save Temporary Success");
                            var link = reply.data.replaceAll('/', 'A');
                            $state.go('temporary', { id: link });
                        } else {
                            $.growl.error({ message: "Gagal mendapatkan data employee" });
                        }
                    }, function (err) {
                        $.growl.error({ message: err });
                        console.info("error:" + JSON.stringify(err));
                    });
                    console.info(data);
                }  
            }
            else {
                var tempID = document.getElementById('tempID').value;
                var reason, type, Isproject, project, actual;
                reason = document.getElementById('reason');
                vm.reason = reason.options[reason.selectedIndex].value;
                type = document.getElementById('Vtype');
                vm.Type = type.options[type.selectedIndex].value;
                if(vm.Type=="Benefit"){
                    actual = document.getElementById('actual');
                    vm.ActualGroupLevel = actual.options[actual.selectedIndex].value;
                }
                Isproject = document.getElementById('isProject');
                vm.isProject = Number(Isproject.options[Isproject.selectedIndex].value);
                if (vm.isProject == 1) {
                    project = document.getElementById('project');
                    vm.project = Number(project.options[project.selectedIndex].value);
                    console.info(vm.project);
                }
                if (vm.end_date <= vm.start_date) {
                    UIControlService.msg_growl('error', "End Date Can't Less Than Start Date");
                }
                else if (vm.Type == "Benefit" && vm.ActualGroupLevel == '? undefined:undefined ?') {
                    UIControlService.msg_growl('error', 'Actual Group Level Must Be Selected');
                }
                else if (vm.Type != "Benefit" && vm.isProject == undefined) {
                    UIControlService.msg_growl('error', 'Project Must Be Selected');
                }
                else if (vm.Type != "Benefit" && vm.isProject == 1 && Number.isNaN(vm.project)) {
                    UIControlService.msg_growl('error', 'Project Name Must Be Selected');
                }
                else {
                    var data = {
                        temporaryID: tempID,
                        requestDate: vm.temp_date,
                        coordinator: vm.coordinatorID,
                        reason: vm.reason,
                        employeeID: vm.employee_id,
                        employeeName: vm.employee,
                        costCenter: vm.cc,
                        employeeGroup: vm.groupLevel,
                        actualGroup: vm.ActualGroupLevel,
                        vehicleType: vm.Type,
                        isProject: Number(vm.isProject),
                        project: Number(vm.project),
                        location: vm.location_id,
                        manufacturer: vm.selectedVehicle.manufacturer,
                        model: vm.selectedVehicle.model,
                        series: vm.selectedVehicle.series,
                        bodyType: vm.selectedVehicle.body_type,
                        color: vm.selectedVehicle.color,
                        startRent: vm.start_date,
                        endRent: vm.end_date,
                        supplyMethod:"Temporary"
                    }
                    console.info(data);
                    TemporaryService.SaveDraft(data, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Save Temporary Success");
                            vm.init();
                        } else {
                            $.growl.error({ message: "Gagal mendapatkan data employee" });
                        }
                    }, function (err) {
                        $.growl.error({ message: err });
                        console.info("error:" + JSON.stringify(err));
                    });
                }
                
            }
            
        }
        vm.sendDraft = sendDraft;
        function sendDraft() {
            if (document.getElementById('tempID') == null) {
                console.info("Null");
                if (vm.reason == undefined) {
                    UIControlService.msg_growl('error', 'Reason Must Be Selected');
                }
                else if (vm.employee_id == undefined) {
                    UIControlService.msg_growl('error', 'Employee Must Be Selected');
                }
                else if (vm.Type == undefined) {
                    UIControlService.msg_growl('error', 'Vehicle Type Must Be Selected');
                }
                else if (vm.location_id == undefined) {
                    UIControlService.msg_growl('error', 'Location Must Be Selected');
                }
                else if (vm.selectedVehicle.length == 0) {
                    UIControlService.msg_growl('error', 'Vehicle Must Be Selected');
                }
                else if (vm.start_date == undefined) {
                    UIControlService.msg_growl('error', 'Start Date Must Be Selected');
                }
                else if (vm.end_date == undefined) {
                    UIControlService.msg_growl('error', 'End Date Must Be Selected');
                }
                else if (vm.end_date <= vm.start_date) {
                    UIControlService.msg_growl('error', "End Date Can't Less Than Start Date");
                }
                else if (vm.Type == "Benefit" && vm.ActualGroupLevel == undefined) {
                    UIControlService.msg_growl('error', 'Actual Group Level Must Be Selected');
                }
                else if (vm.Type != "Benefit" && vm.isProject == undefined) {
                    UIControlService.msg_growl('error', 'Project Must Be Selected');
                }
                else if (vm.Type != "Benefit" && vm.isProject == 1 && vm.project == undefined) {
                    UIControlService.msg_growl('error', 'Project Name Must Be Selected');
                }
                else {
                    if (vm.Type == "Benefit") {
                        vm.isProject = 0;
                    }
                    if (vm.isProject == 0) {
                        vm.project = null;
                    }
                    var data = {
                        temporaryID: 0,
                        requestDate: vm.temp_date,
                        coordinator: vm.coordinatorID,
                        reason: vm.reason,
                        employeeID: vm.employee_id,
                        employeeName: vm.employee,
                        costCenter: vm.cc,
                        employeeGroup: vm.groupLevel,
                        actualGroup: vm.ActualGroupLevel,
                        vehicleType: vm.Type,
                        isProject: Number(vm.isProject),
                        project: Number(vm.project),
                        location: vm.location_id,
                        manufacturer: vm.selectedVehicle.manufacturer,
                        model: vm.selectedVehicle.model,
                        series: vm.selectedVehicle.series,
                        bodyType: vm.selectedVehicle.body_type,
                        color: vm.selectedVehicle.color,
                        startRent: vm.start_date,
                        endRent: vm.end_date,
                        supplyMethod: "Temporary",
                        role:vm.roles
                    }
                    TemporaryService.SendDraft(data, function (reply) {
                        if (reply.status === 200) {
                            console.info(reply.data);
                            UIControlService.msg_growl("notice", "Send Temporary Success");
                            var link = reply.data.replaceAll('/', 'A');;
                            $state.go('temporary', { id: link });
                        } else {
                            $.growl.error({ message: "Gagal mendapatkan data employee" });
                        }
                    }, function (err) {
                        $.growl.error({ message: err });
                        console.info("error:" + JSON.stringify(err));
                    });
                    console.info(data);
                }
            }
            else {
                var tempID = document.getElementById('tempID').value;
                var reason, type, Isproject, project, actual;
                reason = document.getElementById('reason');
                vm.reason = reason.options[reason.selectedIndex].value;
                type = document.getElementById('Vtype');
                vm.Type = type.options[type.selectedIndex].value;
                if (vm.Type == "Benefit") {
                    actual = document.getElementById('actual');
                    vm.ActualGroupLevel = actual.options[actual.selectedIndex].value;
                }
                Isproject = document.getElementById('isProject');
                vm.isProject = Number(Isproject.options[Isproject.selectedIndex].value);
                if (vm.isProject == 1) {
                    project = document.getElementById('project');
                    vm.project = Number(project.options[project.selectedIndex].value);
                    console.info(vm.project);
                }
                if (vm.end_date <= vm.start_date) {
                    UIControlService.msg_growl('error', "End Date Can't Less Than Start Date");
                }
                else if (vm.Type == "Benefit" && vm.ActualGroupLevel == '? undefined:undefined ?') {
                    UIControlService.msg_growl('error', 'Actual Group Level Must Be Selected');
                }
                else if (vm.Type != "Benefit" && vm.isProject == undefined) {
                    UIControlService.msg_growl('error', 'Project Must Be Selected');
                }
                else if (vm.Type != "Benefit" && vm.isProject == 1 && Number.isNaN(vm.project)) {
                    UIControlService.msg_growl('error', 'Project Name Must Be Selected');
                }
                else {
                    var data = {
                        temporaryID: tempID,
                        requestDate: vm.temp_date,
                        coordinator: vm.coordinatorID,
                        reason: vm.reason,
                        employeeID: vm.employee_id,
                        employeeName: vm.employee,
                        costCenter: vm.cc,
                        employeeGroup: vm.groupLevel,
                        actualGroup: vm.ActualGroupLevel,
                        vehicleType: vm.Type,
                        isProject: Number(vm.isProject),
                        project: Number(vm.project),
                        location: vm.location_id,
                        manufacturer: vm.selectedVehicle.manufacturer,
                        model: vm.selectedVehicle.model,
                        series: vm.selectedVehicle.series,
                        bodyType: vm.selectedVehicle.body_type,
                        color: vm.selectedVehicle.color,
                        startRent: vm.start_date,
                        endRent: vm.end_date,
                        supplyMethod: "Temporary",
                        role: vm.roles
                    }
                    TemporaryService.SendDraft(data, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Send Temporary Success");
                            var link = vm.request_number.replace('Draft','');
                            $state.go('temporary', { id: link });
                        } else {
                            $.growl.error({ message: "Gagal mendapatkan data employee" });
                        }
                    }, function (err) {
                        $.growl.error({ message: err });
                        console.info("error:" + JSON.stringify(err));
                    });
                    console.info(data);
                }
            }
        }
        vm.employee;
        vm.openEmployee = openEmployee;
        function openEmployee() {
            var modalInstance = $uibModal.open({
                templateUrl: 'selectEmployee.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.EmployeeID = "";
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
                        TemporaryService.GetEmployee(data, function (reply) {
                            if (reply.status === 200) {
                                $scope.employees = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data employee" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                        console.info("adjh");
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.selectManager = function (param) {
                        console.info(param);
                        vm.employee_id = param.employee_id;
                        vm.employee = param.employee_name;
                        vm.cc = param.cost_center;
                        vm.groupLevel = param.grup_level;
                        vm.loadCategory(vm.groupLevel);
                        $uibModalInstance.dismiss('cancel');
                    };

                }
            });
        };
        vm.kota;
        vm.alamat;
        vm.openCity = openCity;
        function openCity() {
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
                        var data = $scope.city.toLowerCase() + "|" + $scope.address.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                        TemporaryService.GetKota(data, function (reply) {
                            if (reply.status === 200) {
                                $scope.CostCenter = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                                console.info($scope.CostCenter);
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data cc" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                    };
                    $scope.searchCity = function (param) {
                        if ($scope.city == undefined) {
                            $scope.getCC(1);
                        }
                        else {
                            console.info($scope.city);
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            TemporaryService.GetKotaByCity({
                                city: $scope.city,
                                Offset: $scope.offset,
                                Limit: $scope.pageSize
                            }, function (reply) {
                                $scope.CostCenter = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                    }
                    $scope.searchAddress = function (param) {
                        if ($scope.address == undefined) {
                            $scope.getCC(1);
                        }
                        else {
                            console.info($scope.city);
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            TemporaryService.GetKotaByAddress({
                                address: $scope.address,
                                Offset: $scope.offset,
                                Limit: $scope.pageSize
                            }, function (reply) {
                                $scope.CostCenter = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
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
        vm.selectVehicle = selectVehicle;
        function selectVehicle() {
            if (vm.groupLevel == undefined) {
                UIControlService.msg_growl("error", "Employee Must Be Selected");
            }
            else if(vm.Type==undefined){
                UIControlService.msg_growl("error", "Vehicle Type Must Be Selected");
            }
            else if (vm.Type == "Benefit" && vm.ActualGroupLevel == undefined) {
                UIControlService.msg_growl('error', 'Actual Group Level Must Be Selected');
            }
            else {
                var modalInstance = $uibModal.open({
                    templateUrl: 'selectVehicle.html',
                    controller: function ($uibModalInstance, $scope) {
                        $scope.manufacturer = '';
                        $scope.model = '';
                        $scope.series = '';
                        $scope.bodyType = '';
                        $scope.color = '';
                        $scope.currentPage = 1;
                        $scope.pageSize = 15;
                        $scope.totalRecords = 0;
                        $scope.car_group = vm.ActualGroupLevel;
                        if (vm.Type != "Benefit") {
                            $scope.car_group = 0;
                        }
                        $scope.loadVehicle = function (param) {
                            $scope.currentPage = param;
                            $scope.offset = (param * 10) - 10;
                            var data = vm.Type + "|" + $scope.manufacturer.toLowerCase() + "|" + $scope.car_group + "|" + $scope.model.toLowerCase() + "|" + $scope.serie.toLowerCase() + "|" + $scope.bodyType.toLowerCase() + "|" + $scope.color.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                            console.info(data);
                            TemporaryService.GetVehicle(data, function (reply) {
                                if (reply.status === 200) {
                                    $scope.DataVehicle = reply.data.List;
                                    $scope.totalRecords = reply.data.Count;
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
                            var data = vm.Type + "|" + $scope.manufacturer.toLowerCase() + "|" + $scope.car_group + "|" + $scope.model.toLowerCase() + "|" + $scope.serie.toLowerCase() + "|" + $scope.bodyType.toLowerCase() + "|" + $scope.color.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                            console.info(data);
                            TemporaryService.GetFleet(data, function (reply) {
                                if (reply.status === 200) {
                                    if (reply.data.Count > 0) {
                                        $scope.DataVehicle = reply.data.List;
                                        $scope.totalRecords = reply.data.Count;
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
                            $scope.serie = $scope.series
                            if ($scope.serie.indexOf(".") !== -1) {
                                // Code
                                $scope.serie = $scope.serie.replace(".", "~2a");
                            }
                            if ($scope.serie.indexOf("/") !== -1) {
                                // Code
                                $scope.serie = $scope.serie.replace("/", "~2f");
                            }
                            if (vm.Type == "Benefit") {
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
                        if (vm.fleet_id != null) {
                            TemporaryService.GetFleetTemp(vm.fleet_id, function (reply) {
                                if (reply.status === 200) {
                                    $scope.fleetcsf = reply.data;
                                    console.info($scope.fleetcsf);
                                }
                            });
                        }
                    };
                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file, function (reply) {
                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];
                                console.info(sheet1[2].Column2);
                                if (sheet1[2].Column2 != "Request Number") {
                                    UIControlService.msg_growl("error", "Column 2 Have Name " + sheet1[2].Column2);
                                    UIControlService.msg_growl("error", "It Should Request Number");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column3 != "Employee Name") {
                                    UIControlService.msg_growl("error", "Column 3 Have Name " + sheet1[2].Column3);
                                    UIControlService.msg_growl("error", "It Should Employee Name");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column4 != "Vendor") {
                                    UIControlService.msg_growl("error", "Column 4 Have Name " + sheet1[2].Column4);
                                    UIControlService.msg_growl("error", "It Should Vendor");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column5 != "Police Number") {
                                    UIControlService.msg_growl("error", "Column 5 Have Name " + sheet1[2].Column5);
                                    UIControlService.msg_growl("error", "It Should Police Number");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column6 != "Chasis Number") {
                                    UIControlService.msg_growl("error", "Column 6 Have Name " + sheet1[2].Column6);
                                    UIControlService.msg_growl("error", "It Should Chasis Number");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column7 != "Engine Number") {
                                    UIControlService.msg_growl("error", "Column 7 Have Name " + sheet1[2].Column7);
                                    UIControlService.msg_growl("error", "It Should Engine Number");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column8 != "Contract Start Date") {
                                    UIControlService.msg_growl("error", "Column 8 Have Name " + sheet1[2].Column8);
                                    UIControlService.msg_growl("error", "It Should Contract Start Date");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column9 != "Contract End Date") {
                                    UIControlService.msg_growl("error", "Column 9 Have Name " + sheet1[2].Column9);
                                    UIControlService.msg_growl("error", "It Should Contract End Date");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column10 != "AirBag") {
                                    UIControlService.msg_growl("error", "Column 10 Have Name " + sheet1[2].Column10);
                                    UIControlService.msg_growl("error", "It Should AirBag");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column11 != "Make") {
                                    UIControlService.msg_growl("error", "Column 11 Have Name " + sheet1[2].Column11);
                                    UIControlService.msg_growl("error", "It Should Make");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column12 != "Model") {
                                    UIControlService.msg_growl("error", "Column 12 Have Name " + sheet1[2].Column12);
                                    UIControlService.msg_growl("error", "It Should Model");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column13 != "Series") {
                                    UIControlService.msg_growl("error", "Column 13 Have Name " + sheet1[2].Column13);
                                    UIControlService.msg_growl("error", "It Should Series");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column14 != "Transmission") {
                                    UIControlService.msg_growl("error", "Column 14 Have Name " + sheet1[2].Column14);
                                    UIControlService.msg_growl("error", "It Should Transmission");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column15 != "Color") {
                                    UIControlService.msg_growl("error", "Column 15 Have Name " + sheet1[2].Column15);
                                    UIControlService.msg_growl("error", "It Should Color");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column16 != "Body type") {
                                    UIControlService.msg_growl("error", "Column 16 Have Name " + sheet1[2].Column16);
                                    UIControlService.msg_growl("error", "It Should Body type");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column17 != "Branding") {
                                    UIControlService.msg_growl("error", "Column 17 Have Name " + sheet1[2].Column17);
                                    UIControlService.msg_growl("error", "It Should Branding");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column18 != "Purpose") {
                                    UIControlService.msg_growl("error", "Column 18 Have Name " + sheet1[2].Column18);
                                    UIControlService.msg_growl("error", "It Should Purpose");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column19 != "Vehicle Year") {
                                    UIControlService.msg_growl("error", "Column 19 Have Name " + sheet1[2].Column19);
                                    UIControlService.msg_growl("error", "It Should Vehicle Year");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column20 != "PO") {
                                    UIControlService.msg_growl("error", "Column 20 Have Name " + sheet1[2].Column20);
                                    UIControlService.msg_growl("error", "It Should PO");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column21 != "PO Line") {
                                    UIControlService.msg_growl("error", "Column 21 Have Name " + sheet1[2].Column21);
                                    UIControlService.msg_growl("error", "It Should PO Line");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column22 != "Vat") {
                                    UIControlService.msg_growl("error", "Column 22 Have Name " + sheet1[2].Column22);
                                    UIControlService.msg_growl("error", "It Should Vat");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column23 != "Restituion") {
                                    UIControlService.msg_growl("error", "Column 23 Have Name " + sheet1[2].Column23);
                                    UIControlService.msg_growl("error", "It Should Restituion");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                for (var i = 3; i < sheet1.length; i++) {
                                    var item = {
                                        csf: sheet1[i].Column2,
                                        employeeName: sheet1[i].Column3,
                                        vendor: sheet1[i].Column4,
                                        policeNumber: sheet1[i].Column5,
                                        chasisNumber: sheet1[i].Column6,
                                        engineNumber: sheet1[i].Column7,
                                        contractStart: sheet1[i].Column8,
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
                                    if (typeof (item.vat) != "boolean" || !item.vat) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Vat Must be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (typeof (item.restituion) != "boolean" || !item.restituion) {
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
                        if ($scope.data.length > 0) {
                            TemporaryService.SaveFleet(
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
