(function () {
    'use strict';

    angular.module("app").controller("CompCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'CompService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, CompService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = this;
        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles');
        vm.dataComplaint = [];
        vm.datasComplaint = [];
        vm.request_number = $stateParams.rq;
        vm.currentPage = 1;
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
        String.prototype.replaceAll = function (search, replacement) {
            var target = this;
            return target.replace(new RegExp(search, 'g'), replacement);
        };
        vm.isCalendarOpened = [false, false, false];
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }
        //DASHBOARD//
        vm.initDash = initDash;
        function initDash() {
            CompService.GetCoordinator(vm.coordinator, function (reply) {
                vm.coordinatorID = reply.data[0]['UserID'];
                console.info(vm.coordinatorID);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.loadComplaint(vm.currentPage);
        }
        $scope.promdate = null;
        $scope.compdate = null;
        $scope.category = "";
        $scope.policeNumber = "";
        $scope.vType = "";
        $scope.vUsage = "";
        $scope.employeeID = "";
        $scope.employeeName = "";
        vm.loadComplaint = loadComplaint;
        function loadComplaint(current) {
            $scope.promdateTemp = $scope.promdate;
            $scope.compdateTemp = $scope.compdate;
            if ($scope.promdateTemp == "") {
                $scope.promdateTemp = null;
            }
            else if ($scope.promdateTemp != "" && $scope.promdateTemp != null) {
                $scope.promdateTemp = vm.dateFormat($scope.promdateTemp);
            }
            if ($scope.compdateTemp == "") {
                $scope.compdateTemp = null;
            }
            else if ($scope.compdateTemp != "" && $scope.compdateTemp != null) {
                $scope.compdateTemp = vm.dateFormat($scope.compdateTemp);
            }
            var offset = (current * 10) - 10;
            var limit = 10;
            var data = $scope.promdateTemp + "|" + $scope.compdateTemp + "|" + $scope.category.toLowerCase() + "|" + $scope.policeNumber.toLowerCase() + "|" + $scope.vType.toLowerCase() + "|" + $scope.vUsage.toLowerCase() + "|" + $scope.employeeID.toLowerCase() + "|" + $scope.employeeName.toLowerCase() + "|" + offset + "|" + limit;
            console.info(data);
            //CompService.GetComplaint(data, function (reply) {
            //    UIControlService.unloadLoading();
            //    if (reply.status === 200) {
            //        vm.datasComplaint = reply.data.List;
            //        vm.totalRecords = reply.data.Count;
            //        console.info(vm.epaf);
            //    } else {
            //        $.growl.error({ message: "Failed To Get data ePaf" });
            //        UIControlService.unloadLoading();
            //    }
            //}, function (err) {
            //    console.info("error:" + JSON.stringify(err));
            //    UIControlService.unloadLoading();
            //});
        }
        vm.toExcel = toExcel;
        function toExcel (){
            alasql('SELECT  AS `PROMISED DATE`,  AS `COMPLAINT DATE`,  AS `CATEGORY`,  AS `PLICE NUMBER`, AS `VEHICLE TYPE`,  AS `VEHICLE USAGE`,  AS `EMPLOYEE ID`, AS `EMPLOYEE NAME`, AS `VEHICLE LOCATION`, AS `STATUS`, AS `PIC` INTO XLSX("caf.xlsx",{headers:true}) FROM ?', [vm.datasComplaint]);
        }

        //FORM COMPLAINT//
        var today = new Date();

        vm.request_date = vm.dateFormat(today);
        vm.coord = vm.coordinator;
        vm.init = init;
        function init() {
            CompService.GetCoordinator(vm.coordinator, function (reply) {
                vm.coordinatorID = reply.data[0]['UserID'];
                console.info(vm.coordinatorID);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            console.info(vm.request_number);
            if(vm.request_number==undefined){
                var modalInstance = $uibModal.open({
                    templateUrl: 'gs.html',
                    size: 'sm',
                    controller: function ($uibModalInstance, $scope) {
                        $scope.isCalendarOpened = [false, false, false];
                        $scope.getFleet = function () {
                        };
                        $scope.openCalendar = function (index) {
                            $scope.isCalendarOpened[index] = true;
                        }
                        $scope.batal = function () {
                            $uibModalInstance.dismiss('cancel');
                        };
                        $scope.simpan = function (param) {
                            vm.gs = param;
                            console.info(vm.gs);
                            $uibModalInstance.dismiss('cancel');
                        }

                    }
                });
            }
            else {

            }
            //CompService.GetComplaint(vm.request_number, function (reply) {
            //    vm.dataComplaint = reply.data;
                
            //}, function (err) {
            //    console.info("error:" + JSON.stringify(err));
            //});
        }
        vm.saveDraft = saveDraft;
        vm.sendDraft = sendDraft;
        function saveDraft(param) {
            console.info(param);
        }
        function sendDraft(param) {
            console.info(param);
        }
        vm.modalFollowUp = modalFollowUp;
        function modalFollowUp() {
            var modalInstance = $uibModal.open({
                templateUrl: 'followUp.html',
                size: 'modal-sm',
                //resolve: {
                //    test: function () {
                //        return csf;
                //    }
                //},
                controller: function ($uibModalInstance, $scope) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.getFleet = function () {
                    };
                    $scope.openCalendar = function (index) {
                        $scope.isCalendarOpened[index] = true;
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.simpan = function () {
                    }

                }
            });
        }
        vm.modalVehicle = modalVehicle;
        function modalVehicle() {
            var modalInstance = $uibModal.open({
                templateUrl: 'vehicle.html',
                size: 'modal-sm',
                //resolve: {
                //    test: function () {
                //        return csf;
                //    }
                //},
                controller: function ($uibModalInstance, $scope) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.getFleet = function () {
                    };
                    $scope.openCalendar = function (index) {
                        $scope.isCalendarOpened[index] = true;
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.simpan = function () {
                    }

                }
            });
        };
        vm.costCenter = costCenter;
        function costCenter(comp) {
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
                        CompService.GetKota(data, function (reply) {
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
                        vm.city = param1;
                        vm.address = param2
                        $uibModalInstance.dismiss('cancel');
                        console.info(comp);
                    };

                }
            });
        };
        vm.openEmployee = openEmployee;
        function openEmployee() {
            console.info();
            var modalInstance = $uibModal.open({
                templateUrl: 'employee.html',
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
                        CompService.GetEmployee(data, function (reply) {
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
                        
                            vm.employee_id = param.employee_id;
                            //comp.employee = param.employee_name;
                            vm.employee_name = param.employee_name;
                            vm.CC = param.cost_center;
                            vm.groupLevel = param.grup_level;

                        $uibModalInstance.dismiss('cancel');
                    };

                }
            });
        };
    }
})();