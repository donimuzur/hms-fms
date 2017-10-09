(function () {
    'use strict';

    angular.module("app").controller("CAFDashboardCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'CAFDashboardService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, CAFDashboardService,
        UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = $scope;
        vm.cafs = [];
        $scope.currentPage = 1;
        vm.fullSize = 10;
        vm.totalRecords = 0;
        vm.yes = 0;
        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles');
        vm.id_user;
        vm.init = init;
        String.prototype.replaceAll = function (search, replacement) {
            var target = this;
            return target.replace(new RegExp(search, 'g'), replacement);
        };
        function init() {
            UIControlService.loadLoading("Loading");
            vm.jLoad(1);
            CAFDashboardService.GetCoordinator(vm.coordinator, function (reply) {
                vm.coordinatorID = reply.data[0]['UserID'];
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
        }
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
        vm.hapus = hapus;
        function hapus() {
            console.info(hapus);
        }
        vm.isCalendarOpened = [false, false, false];
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }
        $scope.sirsno = "";
        $scope.po = "";
        $scope.empName = "";
        $scope.area = "";
        $scope.model = "";
        $scope.vendor = "";
        $scope.inDate = null;
        $scope.stat = "";
        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            $scope.sirsTemp = $scope.sirsno;
            if ($scope.sirsTemp.indexOf("/") !== -1) {
                // Code
                $scope.sirsTemp = $scope.sirsTemp.replaceAll("/", "~2f");
            }
            $scope.inDateTemp = $scope.inDate;
            if ($scope.inDateTemp == "") {
                $scope.inDateTemp = null;
            }
            else if ($scope.inDateTemp != "" && $scope.inDateTemp!=null) {
                $scope.inDateTemp = vm.dateFormat($scope.inDateTemp);
            }
            var offset = (current * 10) - 10;
            var limit = 15;
            var data = $scope.sirsno.toLowerCase() + "|" + $scope.po.toLowerCase() + "|" + $scope.empName.toLowerCase() + "|" + $scope.area.toLowerCase() + "|" + $scope.model.toLowerCase() + "|" + $scope.vendor.toLowerCase() + "|" + $scope.stat.toLowerCase() + "|" + $scope.inDateTemp + "|" + offset + "|" + limit;
            console.info(data);
            CAFDashboardService.GetCAF(data, function (reply) {
                if (reply.status === 200) {
                    vm.cafs = reply.data.List;
                    vm.totalrecords = reply.data.Count
                    console.info(vm.totalrecords);
                    UIControlService.unloadLoading();
                } else {
                    $.growl.error({ message: "Failed To Get Data CAF" });
                    UIControlService.unloadLoading();
                    
                }
            }, function (err) {
                console.info("error:" + json.stringify(err));
                UIControlService.unloadLoading();
            });
            UIControlService.unloadLoading();
        }
        vm.toExcel = toExcel;
        function toExcel(tabel) {
            console.info(tabel);
            alasql('SELECT sirsNo AS `SIRS No.`, policeNumber AS `Police No.`, employeeName AS `Employee Name`, area AS `Area`, vehicleModel AS `Vehicle Model`, vendor AS `vendor`, status AS `Status`,incidentDate AS `Incident Date` INTO XLSX("caf.xlsx",{headers:true}) FROM ?', [vm.cafs]);
            
        }
        vm.uploadCAF = uploadCAF;
        function uploadCAF() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadCAF.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.filecaf;
                    $scope.data = [];
                    $scope.select = function (file) {
                        $scope.filevehicle = file;
                    };
                    $scope.dateFormat = function (param) {
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
                    $scope.uploadFile = function () {
                        $scope.upload($scope.filevehicle);
                    };
                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file, function (reply) {
                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];
                                console.info(sheet1);
                                if (sheet1[1].Column2 != "SIRS No.") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "It Must Be SIRS No.");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "Input Month") {
                                    UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "It Must Be Month");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column4 != "Accident Month") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "It Must Be Accident Month");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column5 != "Nopol") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "It Must Be Body Nopol");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column6 != "supervisor") {
                                    UIControlService.msg_growl("error", "Column 6 Name Is " + sheet1[1].Column6);
                                    UIControlService.msg_growl("error", "It Must Be Supervisor");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column7 != "Crash/Incident") {
                                    UIControlService.msg_growl("error", "Column 7 Name Is " + sheet1[1].Column7);
                                    UIControlService.msg_growl("error", "It Must Be Crash/Incident");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column8 != "Outscope/Inscope") {
                                    UIControlService.msg_growl("error", "Column 8 Name Is " + sheet1[1].Column8);
                                    UIControlService.msg_growl("error", "It Must Be Outscope/Inscope");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column9 != "Accident Location") {
                                    UIControlService.msg_growl("error", "Column 9 Name Is " + sheet1[1].Column9);
                                    UIControlService.msg_growl("error", "It Must Be Accident Location");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column10 != "Description") {
                                    UIControlService.msg_growl("error", "Column 10 Name Is " + sheet1[1].Column10);
                                    UIControlService.msg_growl("error", "It Must Be Description");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                for (var i = 2; i < sheet1.length; i++) {
                                    var item = {
                                        sirsNo: sheet1[i].Column2,
                                        inputMonth: sheet1[i].Column3,
                                        accidentMonth: sheet1[i].Column4,
                                        nopol: sheet1[i].Column5,
                                        supervisor: sheet1[i].Column6,
                                        incident: sheet1[i].Column7,
                                        inscope: sheet1[i].Column8,
                                        location: sheet1[i].Column9,
                                        description: sheet1[i].Column10,
                                        role: vm.roles,
                                        coordinator:vm.coordinatorID
                                    };
                                    if (!item.sirsNo) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid");
                                        UIControlService.msg_growl("error", "SIRS No. Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.inputMonth) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Input Month Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.accidentMonth) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Accident Month Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.nopol) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Nopol Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.incident) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Crash/Incident Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.inscope) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Outscope/Inscope Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.location) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Accident Location Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.description) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i+1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Description Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
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
                    $scope.simpan = function () {
                        CAFDashboardService.SaveCAF($scope.data, function (reply) {
                            if (reply.status == 200) {
                                console.info($scope.data);
                                vm.init();
                            }
                            else {
                                UIControlService.msg_growl("error", "You Cant Insert Same Sirs Number");
                            }},
                            function (error) {
                                UIControlService.msg_growl("error", "Sorry, You Cant Insert Same Sirs Number")
                                UIControlService.unloadLoadingModal();
                        });
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
        };
        vm.cancel = cancel;
        function cancel(id) {
            console.info(id);
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size: 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        CAFDashboardService.GetRemark(function (reply) {
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
                                cafID: id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            CAFDashboardService.CancelCAF(data, function (reply) {
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
        vm.updateCAF = updateCAF;
        function updateCAF() {
            var modalInstance = $uibModal.open({
                templateUrl: 'updateCAF.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.fileUpdateCaf;
                    $scope.data = [];
                    $scope.dateFormat = function (param) {
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
                    $scope.select = function (file) {
                        $scope.fileupdate = file;
                    };
                    $scope.uploadFile = function () {
                        $scope.upload($scope.fileupdate);
                    };
                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file, function (reply) {
                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];
                                console.info(sheet1);
                                if (sheet1[1].Column2 != "No. SIRS") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "It Must Be No. SIRS");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "Police Number") {
                                    UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "It Must Be Police Number");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column4 != "Employee Name") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "It Must Be Employee Name");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column5 != "Progress Date") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "It Must Be Progress Date");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column6 != "Progress Update") {
                                    UIControlService.msg_growl("error", "Column 6 Name Is " + sheet1[1].Column6);
                                    UIControlService.msg_growl("error", "Progress Update");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column7 != "Remark") {
                                    UIControlService.msg_growl("error", "Column 7 Name Is " + sheet1[1].Column7);
                                    UIControlService.msg_growl("error", "It Must Be Remark");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column8 != "Estimation") {
                                    UIControlService.msg_growl("error", "Column 8 Name Is " + sheet1[1].Column8);
                                    UIControlService.msg_growl("error", "It Must Be Estimation");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[2].Column9 != "Actual") {
                                    UIControlService.msg_growl("error", "Column 9 Name Is " + sheet1[1].Column9);
                                    UIControlService.msg_growl("error", "It Must Be Actual");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                for (var i = 3; i < sheet1.length; i++) {
                                    var item = {
                                        sirsNo: sheet1[i].Column2,
                                        policeNumber: sheet1[i].Column3,
                                        employeeName: sheet1[i].Column4,
                                        progressDate: sheet1[i].Column5,
                                        progressUpdate: sheet1[i].Column6,
                                        remark: sheet1[i].Column7,
                                        estimation: sheet1[i].Column8,
                                        actual: sheet1[i].Column9,
                                        role:vm.roles
                                    };
                                    if (!item.sirsNo) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid");
                                        UIControlService.msg_growl("error", "No. SIRS Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.policeNumber) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Police Number Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.employeeName) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Employee Name Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.progressDate) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Progress Date Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.progressUpdate) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Progress Update Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.remark) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Remark Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.estimation) {
                                        UIControlService.msg_growl("error", "Item in Row " + (i + 1) + " Not Valid");
                                        UIControlService.msg_growl("error", "Estimation Location Must Be Filled");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.actual) {
                                        item.actual = 0;
                                    }
                                    CAFDashboardService.GetID({ sirs: sheet1[i].Column2 }, function (reply) {
                                        if (reply.status === 200) {
                                            var item = {
                                                sirs: reply.data.SIRS_number,
                                                status: reply.data.caf_status_id
                                            };
                                            $scope.data2.push(item);
                                        } else {
                                            $.growl.error({ message: "Wrong Status Progress" });
                                            UIControlService.unloadLoading();
                                        }
                                    }, function (err) {
                                        console.info("error:" + json.stringify(err));
                                        UIControlService.unloadLoading();
                                    });
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
                    $scope.data2=[];
                    $scope.simpan = function () {
                        $scope.check();
                        console.info($scope.Vcheck);
                        CAFDashboardService.UpdateCAF($scope.Vcheck, function (reply) {
                            if (reply.status === 200) {
                                UIControlService.msg_growl("notice", "Update Progress Success");
                                vm.init();
                            } else {
                                $.growl.error({ message: "Update Progress Failed" });
                                UIControlService.unloadLoading();
                            }
                        }, function (err) {
                            console.info("error:" + json.stringify(err));
                            UIControlService.unloadLoading();
                        });
                        var a = $scope.data.length-$scope.Vcheck.length
                        if (a != 0) {
                            UIControlService.msg_growl("error", a+" Data Not Saved");
                        }          
                        $uibModalInstance.dismiss('cancel');
                    };
                    //$scope.checkData=function (a){
                    //     for (var i = 0; i < a.length; i++) {
                    //         if (a[i] == false) return false;
                    //     } 
                    //}
                    $scope.Vcheck=[];
                    $scope.check = function () {
                        console.info($scope.data2);
                        console.info($scope.data);
                        for (var i = 0; i < $scope.data.length; i++) {
                            console.info($scope.data2[i].status + "--" + $scope.data[i].progressUpdate);
                            if ($scope.data2[i].status == 11) {
                                UIControlService.msg_growl("error", "SIRS " + $scope.data2[i].sirs + " Has Been Cancelled");
                                break;
                            }
                            else if ($scope.data2[i].status == 13 && $scope.data[i].progressUpdate == "Administrative") {
                                $scope.Vcheck.push($scope.data[i]);
                            }
                            else if (($scope.data2[i].status == 14&& $scope.data[i].progressUpdate == "Repairing")||($scope.data2[i].status == 14&&$scope.data[i].progressUpdate == "Administrative")) {
                                $scope.Vcheck.push($scope.data[i]);
                            }
                            else if (($scope.data2[i].status == 15 && $scope.data[i].progressUpdate == "Repairing") || ($scope.data2[i].status == 15 && $scope.data[i].progressUpdate == "Waiting for Spare Parts") || ($scope.data2[i].status == 15 && $scope.data[i].progressUpdate != "Delivery")) {
                                $scope.Vcheck.push($scope.data[i]);
                            }
                            else if (($scope.data2[i].status == 16 && $scope.data[i].progressUpdate == "Waiting for Spare Parts") || ($scope.data2[i].status == 16 && $scope.data[i].progressUpdate == "Delivery")) {
                                $scope.Vcheck.push($scope.data[i]);
                            }
                            else if (($scope.data2[i].status == 17 && $scope.data[i].progressUpdate == "Delivery") || ($scope.data2[i].status == 17 && $scope.data[i].progressUpdate == "Completed")) {
                                $scope.Vcheck.push($scope.data[i]);
                            }
                            else if ($scope.data2[i].status == 10) {
                                UIControlService.msg_growl("error", "Progress Update SIRS " + $scope.data2[i].sirs + " Not Valid");
                                break;
                            }
                            else {

                                UIControlService.msg_growl("error", "Progress Update SIRS " + $scope.data2[i].sirs + " Not Valid");
                                break;
                            }
                        };
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
        };
    }
})();
