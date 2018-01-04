(function () {
    'use strict';

    angular.module("app").controller("CSFWCtrl", ctrl);
    
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'CSFWTCService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService','$injector'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, CSFWTCService,
       UploadFileConfigService, ExcelReaderService, UploaderService, $injector) {
        var vm = this;
        vm.request_number = $stateParams.id;
        vm.coordinator = localStorage.getItem('username');
        vm.reason_id;
        vm.effective_date;
        vm.csf_date = new Date();
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
        $scope.queryOptions = {
            query: function (query) {
                var data = {
                    results: [
                        { id: "1", text: "A" },
                        { id: "2", text: "B" }
                    ]
                };

                query.callback(data);
            }
        };
        vm.dateFormatLog = dateFormatLog;
        function dateFormatLog(param) {
            var params = new Date(param);
            var dformat = vm.dateFormat(param) + ' ' +
                  [params.getHours().padLeft(),
                    params.getMinutes().padLeft(),
                    params.getSeconds().padLeft()].join(':');
            return dformat;
        }
        vm.setDate = setDate;
        function setDate(param) {
            if (param.expected_date!=null){
                vm.expected_date = new Date(param.expected_date);
            }
            vm.effective_date = new Date(param.effective_date);
        }
        vm.employee_id;
        vm.vehicle_type_id;
        vm.location_id;
        vm.expected_date;
        vm.Vtype;
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
        vm.vcategory
        vm.isCalendarOpened = [false, false, false];
        console.info($stateParams);
        vm.roles = localStorage.getItem('roles');
        vm.init = init
        function init() {
            CSFWTCService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                console.info(vm.id_coordinator);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.getSuply();
            vm.loadCSF();
            console.info(vm.dataIsProject);
        };
        vm.getSuply = getSuply;
        vm.dataSuply;
        function getSuply() {
            CSFWTCService.GetSupplyMethod(function (reply) {
                vm.dataSuply = reply.data;
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
        }
        vm.getProject = getProject;
        vm.dataProject;
        vm.dataIsProject = [
            { value: 1, is:true,name: "Yes" },
            {value:0, is:false,name:"No"}
        ];
        function getProject() {
            CSFWTCService.GetProject(function (reply) {
                vm.dataProject = reply.data;
                console.info(vm.dataProject);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
        }
        vm.tempLink = tempLink;
        function tempLink(param) {
            param = param.replaceAll('/', 'A');
            return param;

        }
        vm.insertVehicle = insertVehicle;
        function insertVehicle(param) {
            console.info(param);
            var data = {
                manufacturer: param.manufacturer,
                model: param.model,
                series: param.series,
                body_type: param.body_type,
                color: param.color,
            }
            vm.selectedVehicle.push(data);
        }
        vm.getTemporary = getTemporary;
        function getTemporary(param) {
            var data = {
                csfNumber: param
            }
            CSFWTCService.GetTemp(data, function succ(reply) {
                if (reply.status == 200) {
                    vm.dataTemporary = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Data Temporary" })
                }
            });
        }
        vm.getChild = getChild;
        function getChild(param) {
            if (param.manufacturer != null) {
                CSFWTCService.GetCSFChild(param.csf_id, function succ(reply) {
                    if (reply.status == 200) {
                        for (var i = 0;i<reply.data.length;i++){
                            vm.insertVehicle(reply.data[i]);
                        }
                    }
                    else {
                        UIControlService.unloadLoading();
                        $.growl.error({ message: "Failed To Get Data Child CSF" })
                    }
                });
            }
        }
        vm.getCoord = getCoord;
        function getCoord(id) {
            CSFWTCService.GetCoordinatorByID(id, function succ(reply) {
                if (reply.status == 200) {
                    vm.coordinator = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Coordinator CSF" })
                }
            }
             );
        }
        
        vm.getLocation = getLocation;
        function getLocation(id) {
            console.info(id);
            CSFWTCService.GetKotaByID(id, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.location_id = id;
                    vm.kota = reply.data.city;
                    vm.alamat = reply.data.address;
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Data Location" })
                }
            }
             );
        }
        vm.getStatus = getStatus;
        function getStatus(idStatus) {
            CSFWTCService.GetStatus(idStatus,function succ(reply) {
                if (reply.status == 200) {
                    vm.statusCSF = reply.data;
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Status CSF" })
                }
            },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " +JSON.stringify(error));
                }
            );
        }
        vm.getReasonCSF = getReasonCSF;
        function getReasonCSF() {
            CSFWTCService.GetReasonCSF(function succ(reply) {
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
        vm.getLog = getLog;
        function getLog(idStatus) {
            CSFWTCService.GetLog(idStatus, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.log = reply.data;
                    //for (var i = 0; i < reply.data.length; i++) {
                    //    vm.log = vm.log + "[" +reply.data[i].log_date+ "]" + " " + reply.data[i].user_name + " " + reply.data[i].status + '\n';
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
                        CSFWTCService.GetKota(data, function (reply) {
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
                        vm.selectedVehicle = [];
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
                                for (var i = 3; i < sheet1.length; i++) {
                                    var item = {
                                        manufacturer: sheet1[i].Column2,
                                        model: sheet1[i].Column3,
                                        series: sheet1[i].Column4,
                                        body_type: sheet1[i].Column5,
                                        color: sheet1[i].Column6,
                                    };
                                    if (!item.manufacturer) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Manufacturer Must Be Filled");
                                        $scope.data = [];
                                        vm.selectedVehicle = [];
                                    }
                                    if (!item.model) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Model Must Be Filled");
                                        $scope.data = [];
                                        vm.selectedVehicle = [];
                                    }
                                    if (!item.series) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Series Must Be Filled");
                                        $scope.data = [];
                                        vm.selectedVehicle = [];
                                    }
                                    if (!item.body_type) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Body Type Must Be Filled");
                                        $scope.data = [];
                                        vm.selectedVehicle = [];
                                    }
                                    if (!item.color) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Color Must Be Filled");
                                        $scope.data = [];
                                        vm.selectedVehicle = [];
                                    }
                                    vm.selectedVehicle.push(item);
                                    $scope.data.push(item);
                                }
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
                    $scope.pilih = function () {
                        console.info(vm.selectedVehicle);
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.batal = function () {
                        vm.selectedVehicle = [];
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
        };
        vm.getReason = getReason;
        function getReason(id) {
            CSFWTCService.GetReason(id, function succ(reply) {
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

        vm.loadCSF = loadCSF;
        String.prototype.replaceAll = function (search, replacement) {
            var target = this;
            return target.replace(new RegExp(search, 'g'), replacement);
        };
        function loadCSF()
        {
            console.info(vm.request_number);
            vm.dataCSF = {};
            CSFWTCService.GetCSF(
                vm.request_number,
                function succ(reply) {
                    if (reply.status == 200) {
                        if (reply.data[0].vehicle_type == "WTC") {
                            vm.dataCSF = reply.data;
                            console.info(vm.dataCSF);
                            vm.fleet_id = reply.data[0].fleet_id;
                            console.info(vm.fleet_id);
                        }
                        else {
                            var link = reply.data[0].request_number.replaceAll('/', 'A');
                            $state.go('CSF', { rq: link })
                        }
                        console.info(vm.dataCSF);
                    }
                    else {
                        UIControlService.unloadLoading();
                        vm.dataCSF = null;
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
        vm.ambil = 'no';
        vm.noCar = noCar;
        function noCar(param) {
            vm.ambil = param;
            console.info(vm.ambil);
        }
        vm.sendDraft = sendDraft;
        function sendDraft() {
            var data;
            var d = new Date();
            if (document.getElementById('csfID') == null) {
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
                        assignedTo: vm.assignedTo,
                        employeeName: vm.employee,
                        costCenter: vm.cc,
                        effectiveDate: vm.effective_date,
                        reasonID: vm.reason,
                        role: vm.roles,
                        remark:0
                    }
                    CSFWTCService.SaveMenu(data, function (reply) {
                        if (reply.status === 200) {
                            $state.go('dashboard-fleet');
                        } else {
                            $.growl.error({ message: "Failed To Send CSF Draft" });
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
                var assignTo = document.getElementById('assignTo').value;
                var effectiveDate = vm.dateFormat(vm.effective_date);
                var csfID = document.getElementById('csfID').value;
                var csfNumber = document.getElementById('csfNumber').value;
                var e = document.getElementById('reasonCSF');
                var reason = e.options[e.selectedIndex].value;
                if (employeeID == undefined) {
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
                        assignedTo: assignTo,
                        employeeName: employeeName,
                        costCenter: employeeCC,
                        effectiveDate: effectiveDate,
                        reasonID: reason,
                        role: vm.roles,
                        remark: 0
                    }
                    CSFWTCService.SaveMenu(data, function (reply) {
                        if (reply.status === 200) {
                            $state.go('dashboard-fleet');
                        } else {
                            $.growl.error({ message: "Failed To Send CSF Draft" });
                        }
                    }, function (err) {
                        console.info("error:" + JSON.stringify(err));
                    });
                    console.info(data);
                }
            }
        }
        vm.saveDraft = saveDraft;
        function saveDraft() {
            var data;
            var d = new Date();
            if (document.getElementById('csfID') == null) {
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
                        assignedTo: vm.assignedTo,
                        employeeName: vm.employee,
                        costCenter: vm.cc,
                        effectiveDate: vm.effective_date,
                        reasonID: vm.reason,
                        role: vm.roles,
                        remark: 0
                    }
                    CSFWTCService.SaveDraft(data, function (reply) {
                        if (reply.status === 200) {
                            $state.go('dashboard-fleet');
                        } else {
                            $.growl.error({ message: "Failed To Save CSF" });
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
                var assignTo = document.getElementById('assignTo').value;
                var effectiveDate = dateFormat(vm.effective_date);
                var csfID = document.getElementById('csfID').value;
                var csfNumber = document.getElementById('csfNumber').value;
                var e = document.getElementById('reasonCSF');
                var reason = e.options[e.selectedIndex].value;
                if (employeeID == undefined) {
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
                        assignedTo: assignTo,
                        employeeName: employeeName,
                        costCenter: employeeCC,
                        effectiveDate: effectiveDate,
                        reasonID: reason,
                        role: vm.roles,
                        remark: 0
                    }
                    CSFWTCService.SaveDraft(data, function (reply) {
                        if (reply.status === 200) {
                            $state.go('dashboard-fleet');
                        } else {
                            $.growl.error({ message: "Failed To Save Draft CSF" });
                        }
                    }, function (err) {
                        console.info("error:" + JSON.stringify(err));
                    });
                    console.info(data);
                }
            }
        }
        vm.sendAssigned = sendAssigned;
        function sendAssigned(param) {
            var e = document.getElementById('reasonCSF');
            var reason = e.options[e.selectedIndex].value;
            if (vm.selectedVehicle.length == 0) {
                UIControlService.msg_growl('warning', 'Vehicle Not Selected');
            }
            else {
                var data = {
                    csfID: param.csf_id,
                    locationID: vm.location_id,
                    coordinator: vm.id_coordinator,
                    employeeID: Number(document.getElementById('empID').value),
                    assignedTo: document.getElementById('assignTo').value,
                    employeeName: document.getElementById('empName').value,
                    costCenter: Number(document.getElementById('empCC').value),
                    effectiveDate: vm.dateFormat(vm.effective_date),
                    reasonID: reason,
                    dataVehicle: vm.selectedVehicle,
                    role: vm.roles,
                    remark: 0
                }
                CSFWTCService.SendAssigned(data, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Save CSF Success");
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
        }
        vm.saveAssigned = saveAssigned;
        function saveAssigned(param) {
            var e = document.getElementById('reasonCSF');
            var reason = e.options[e.selectedIndex].value;
            if (vm.selectedVehicle.length == 0) {
                UIControlService.msg_growl('warning', 'Vehicle Not Selected');
            }
            else {
                var data = {
                    csfID: param.csf_id,
                    locationID: vm.location_id,
                    coordinator: vm.id_coordinator,
                    employeeID: Number(document.getElementById('empID').value),
                    assignedTo: document.getElementById('assignTo').value,
                    employeeName: document.getElementById('empName').value,
                    costCenter: Number(document.getElementById('empCC').value),
                    effectiveDate: vm.dateFormat(vm.effective_date),
                    reasonID: reason,
                    dataVehicle: vm.selectedVehicle,
                    role: vm.roles,
                    remark: 0
                }
                    CSFWTCService.SaveAssigned(data, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Save CSF Success");
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
          
        }
        vm.saveFleet = saveFleet;
        function saveFleet(param) {
            var id;
            if (Number(param.Is_project) != 0) {
                id=Number(param.project_id)
            }
            else {
                id = "";
            }
            var data = {
                csfID: param.csf_id,
                expectedDate: vm.dateFormat(vm.expected_date),
                isProject: Number(param.Is_project),
                supplyMethod: param.supply_method,
                projectID: id,
            }
            CSFWTCService.SaveCSFFleet(data, function (reply) {
                if (reply.status === 200) {
                    vm.init();
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
            console.info(data);
        }
        vm.suplyMethod;
        vm.declineFleet = declineFleet;
        vm.progress = progress;
        vm.cancel = cancel;
        function cancel(id) {
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
                            CSFWTCService.CSFCancel(data, function (reply) {
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
                            CSFWTCService.CSFRejectedByFleet(data, function (reply) {
                                if (reply.status === 200) {
                                    UIControlService.msg_growl("notice", "CSF Rejected By Fleet");
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
        function progress(id) {
            CSFWTCService.SendMail();
            CSFWTCService.CSFInprogress({
                csfID: id,
                role: vm.roles,
                coordinator2: vm.id_coordinator
            }
            , function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", "CSF Approved By Fleet");
                    vm.init();
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
                        CSFWTCService.GetEmployee(data, function (reply) {
                            if (reply.status === 200) {
                                $scope.employees = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            } else {
                                $.growl.error({ message: "Failed To Get Data Employee" });
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
                        console.info();
                        if (document.getElementById('csfID') != null) {
                            console.info(document.getElementById('csfID').value);
                            document.getElementById('empID').value = param.employee_id;
                            document.getElementById('empName').value = param.employee_name;
                        }
                        else {
                            vm.employee_id = param.employee_id;
                            vm.employee = param.employee_name;
                        }
                        
                        $uibModalInstance.dismiss('cancel');
                    };

                }
            });
        };
        vm.assignedTo;
        vm.openEmployee2 = openEmployee2;
        function openEmployee2() {
            var modalInstance = $uibModal.open({
                templateUrl: 'selectEmployee2.html',
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
                        CSFWTCService.GetEmployee(data, function (reply) {
                            if (reply.status === 200) {
                                $scope.employees = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                            } else {
                                $.growl.error({ message: "Failed To Get Data Employee" });
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
                        console.info(param);
                        if (document.getElementById('csfID') != null) {
                            console.info(document.getElementById('assignTo'));
                            document.getElementById('assignTo').value = param.employee_name
                        }
                        else {
                            vm.assignedTo = param.employee_name;
                        }
                        $uibModalInstance.dismiss('cancel');
                    };

                }
            });
        };
        vm.cc;
        vm.openCC = openCC;
        function openCC() {
            var modalInstance = $uibModal.open({
                templateUrl: 'selectCC.html',
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
                        CSFWTCService.GetCC(data,function (reply) {
                            if (reply.status === 200) {
                                $scope.cc = reply.data.List;
                                $scope.totalRecords = reply.data.Count;
                                console.info(reply.data);
                            } else {
                                $.growl.error({ message: "Failed To Get Data Cost Center" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                        console.info("CC");
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.selectCC = function (param) {
                        if (document.getElementById('csfID') != null) {
                            console.info(document.getElementById('csfID').value);
                            document.getElementById('empCC').value = param.cost_center1;
                        }
                        else {
                            vm.cc = param.cost_center1;
                            vm.cc_id = param.cc_id;
                        }
                        
                        $uibModalInstance.dismiss('cancel');
                    };

                }
            });
        };
        vm.alamat;
        vm.selectVehicle = selectVehicle;
        //vm.selectVehicleUser = selectVehicleUser;
        //function selectVehicleUser(csf) {
        //    var modalInstance = $uibModal.open({
        //        templateUrl: 'csfSelectVehicle.html',
        //        controller: function ($uibModalInstance, $scope) {
        //            $scope.currentPage = 1;
        //            $scope.pageSize = 15;
        //            $scope.totalRecords = 0;
        //            $scope.loadFleet = function (param) {
        //                $scope.currentPage = param;
        //                $scope.offset = (param * 10) - 10;
        //                CSFWTCService.GetFleet(function (reply) {
        //                    if (reply.status == 200) {
        //                        console.info(reply.data);
        //                        $scope.DataVehicle = reply.data
        //                        console.info($scope.DataVehicle);
        //                        $scope.totalRecords = reply.data.length;
        //                    } else {
        //                        $.growl.error({ message: "Failed To Get Data Vehicle" });
        //                    }
        //                }, function (err) {
        //                    console.info("error:" + JSON.stringify(err));
        //                });
        //            }
        //            $scope.batal = function () {
        //                $uibModalInstance.dismiss('cancel');
        //            };
        //            $scope.simpan = function (param) {
        //                vm.selectedVehicle = [];
        //                var item = {
        //                    manufacturer: param.manufacturer,
        //                    model: param.model,
        //                    series: param.series,
        //                    body_type: param.body_type,
        //                    color: param.color,
        //                    vendor: param.vendor_name
        //                };
        //                csf.manufacturer = param.manufacturer;
        //                csf.model = param.model;
        //                csf.series = param.series;
        //                csf.color = param.color;
        //                csf.body_type = param.body_type;
        //                csf.vehicle_id = param.vehicle_specs_id;
        //                csf.vcategory = param.car_group_level;
        //                vm.selectedVehicle.push(item);
        //                $uibModalInstance.dismiss('cancel');
        //                console.info(vm.selectedVehicle);
        //            };
        //        }
        //    });
        //};
        function selectVehicle() {
            var modalInstance = $uibModal.open({
                templateUrl: 'csfSelectVehicle.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.manufacturer = '';
                    $scope.model = '';
                    $scope.series = '';
                    $scope.bodyType = '';
                    $scope.color = '';
                    $scope.currentPage = 1;
                    $scope.pageSize = 15;
                    $scope.totalRecords = 0;
                    $scope.loadFleet = function (param) {
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
                        var data = "WTC" + "|" + $scope.manufacturer.toLowerCase() + "|0" + "|" + $scope.model.toLowerCase() + "|" + $scope.serie.toLowerCase() + "|" + $scope.bodyType.toLowerCase() + "|" + $scope.color.toLowerCase() + "|" + $scope.offset + "|" + $scope.pageSize;
                        CSFWTCService.GetFleet(data,function (reply) {
                            if (reply.status == 200) {
                                console.info(reply.data);
                                $scope.DataVehicle = reply.data.List
                                console.info($scope.DataVehicle);
                                $scope.totalRecords = reply.data.Count;
                            } else {
                                $.growl.error({ message: "Failed To Get Data Vehicle" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.simpan = function (param) {
                        vm.selectedVehicle = [];
                        var item = {
                            manufacturer: param.manufacturer,
                            model: param.model,
                            series: param.series,
                            body_type: param.body_type,
                            color: param.color,
                            vendor:param.vendor_name
                        };
                        vm.selectedVehicle.push(item);
                        $uibModalInstance.dismiss('cancel');
                        console.info(vm.selectedVehicle);
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
                controller: function ($uibModalInstance, $scope, test) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.start_date;
                    $scope.end_date;
                    $scope.reason;
                    $scope.csf = test;
                    $scope.getReason = function () {
                        CSFWTCService.GetTempReason(function (reply) {
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
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.simpan = function () {
                        if ($scope.reason == undefined) {
                            UIControlService.msg_growl("error", "Reason Must Be Selected");
                        }
                        else if ($scope.start_date == undefined) {
                            UIControlService.msg_growl("error", "Start Date Must Be Selected");
                        }
                        else if ($scope.end_date == undefined) {
                            UIControlService.msg_growl("error", "End Date Must Be Selected");
                        }
                        else if ($scope.end_date < $scope.start_date) {
                            UIControlService.msg_growl("error", "End Date Can't Less Than Start Date");
                        }
                        else {
                            var data = {
                                csfNumber: $scope.csf.request_number,
                                startDate: $scope.start_date,
                                endDate: $scope.end_date,
                                reason: Number($scope.reason),
                                coordinator: vm.id_coordinator,
                                suplyMethod: "Temporary",
                                locationId: $scope.csf.location_id,
                                employeeName: $scope.csf.employee_name,
                                actualGroup: 0,
                                costCenter: $scope.csf.cost_center,
                                GroupLevel: 0,
                                project: false,
                                vehicleType: "Benefit",
                                employeeId: $scope.csf.employee_id,
                                role: vm.roles
                            }
                            $scope.csf.Is_project = 0;
                            $scope.csf.supply_method = "Temporary";
                            console.info(data);
                            vm.saveFleet($scope.csf);
                            CSFWTCService.SaveTemporary(data, function (reply) {
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
                        CSFWTCService.GetFleetCSF(vm.fleet_id, function (reply) {
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
                                if (sheet1[2].Column2 != "Request Number") {
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
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Request Number Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.employeeName) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Employee Name Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.vendor) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Vendor Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.policeNumber) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Police Number Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.chasisNumber) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Chasis Number Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.engineNumber) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Engine Number Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.contractStart) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Contract Start Date diisi");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.contractEnd) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Contract End Date Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (typeof (item.airbag) != "boolean" || !item.airbag) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Airbag Must Be Filled / Format Data Wrong");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.make) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Make Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.model) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Model Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.series) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Series Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.transmission) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Transmission Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.color) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Color Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.bodyType) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Body Type Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.branding) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Branding Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.purpose) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Purpose Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.vehicleYear) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Vehicle Year Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.po) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "PO Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.poLine) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "PO Line Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (typeof (item.vat) != "boolean" || !item.vat) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Vat Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (typeof (item.restituion) != "boolean" || !item.restituion) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid:");
                                        UIControlService.msg_growl("error", "Restituion Must Be Filled");
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
                            CSFWTCService.SaveFleet(
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
                            UIControlService.msg_growl("error", "There Is No Data To Be Stored")
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
