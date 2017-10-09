(function () {
    'use strict';

    angular.module("app").controller("CSFDashboardCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', '$state', '$stateParams', 'CSFDashboardService', '$filter'];

    function ctrl($http, $translate,$scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService,DTOptionsBuilder ,$state, $stateParams, CSFDashboardService, $filter) {
        var vm = this;

        vm.epaf = [];
        $scope.currentPage = 1;
        vm.fullSize = 10;
        vm.offset = (vm.currentPage * 10) - 10;
        vm.totalRecords = 0;
        vm.yes = 0;
        vm.coordinator = localStorage.getItem('username');
        vm.id_user;
        vm.roles = localStorage.getItem('roles');
        vm.init = init;
        $scope.field = {};
        vm.dateFormat = dateFormat;
        $scope.dtOptions = DTOptionsBuilder.newOptions()
        .withDisplayLength(10)
        .withOption('bLengthChange', false)
        .withOption('responsive', true)
        .withPaginationType('full_numbers');
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
        $scope.hoverIn = function (mouseEvent) {
            if (!mouseEvent) {
                mouseEvent = window.event;
            }
            $scope.field.left = mouseEvent.pageX + 'px';
            $scope.field.top = mouseEvent.pageY+20+'px';
            return this.hover = true;
        };

        $scope.hoverOut = function () {
            return this.hover = false;
        };
        function init() {
            if (vm.roles == 'SYSTEM.ROLE_HR') {
                UIControlService.loadLoading("Loading");
                vm.jLoad(1);
                console.info(vm.yes);
                console.info(vm.coordinator);
                CSFDashboardService.GetCoordinator(vm.coordinator, function (reply) {
                    vm.id_user = reply.data[0]['UserID'];
                    console.info(vm.id_user);
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                });
            }
            else {
                $state.go('home');

            }
            
        }

        function remove(list, obj) {
            var index = list.indexOf(obj);

            if (index >= 0) {
                list.splice(index, 1);
            } else {
                UIControlService.msg_growl('error', "ERRORS.OBJECT_NOT_FOUND");
            }
            return list;
        }
        vm.removeID = [];


        vm.changeStatus = changeStatus;
        vm.tampung = [];
        function CheckEpaf(obj, list) {
            var i;
            for (i = 0; i < list.length; i++) {
                if (list[i] === obj) {
                    return true;
                }
            }

            return false;
        }
        var counter = 0;
        function changeStatus(data) {
            console.info(data);
            var checking = CheckEpaf(data, vm.tampung);
            if (checking==false) {
                vm.tampung.push(data);
                counter = counter + 1;
                console.info(vm.tampung+counter);
            }
            else {
                vm.tampung = remove(vm.tampung, data);
                counter = counter - 1;
                console.info(vm.tampung+counter);
            }
        }
        vm.isCalendarOpened = [false, false, false];
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }
        vm.assign = assign;
        function assign() {
            if (vm.tampung.length > 0) {
                UIControlService.loadLoading("Loading");
                var i = 0;
                var epaf = [];
                for (var i = 0; i < vm.tampung.length; i++) {
                    var temp = {
                        epafID: vm.tampung[i].epaf_id,
                        effectiveDate: vm.tampung[i].effective_date,
                        employeeID : vm.tampung[i].employee_id,
                        employeeName : vm.tampung[i].employee_name,
                        costCenter: vm.tampung[i].cost_center,
                        groupLevel: vm.tampung[i].group_level,
                        epafNo: vm.tampung[i].epaf_number,
                        coordinator: vm.id_user,
                        action: vm.tampung[i].epaf_action,
                        role:vm.roles
                    };
                    epaf.push(temp);
                }
                console.info(epaf);
                CSFDashboardService.SaveCSF(epaf,function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        vm.init();
                        UIControlService.unloadLoading();
                    } else {
                        $.growl.error({ message: "Save Error" });
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                    UIControlService.unloadLoading();
                });
                vm.tampung = [];
                counter = 0;
                
            }
            else {
                UIControlService.msg_growl('warning', 'There Is No ePaf That Has Been Selected!');
            }
        }
        $scope.effdate = null;
        $scope.appdate = null;
        $scope.empName = "";
        $scope.eletter = "";
        $scope.act = "";
        $scope.epf = "";
        $scope.csf = "";
        $scope.stat = "";
        $scope.cop = "";
        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            //console.info("curr "+current)
            $scope.epfTemp = $scope.epf;
            $scope.csfTemp = $scope.csf;
            $scope.effdateTemp = $scope.effdate;
            if ($scope.effdateTemp == "") {
                $scope.effdateTemp = null;
            }
            else if ($scope.effdateTemp != "" && $scope.effdateTemp != null) {
                $scope.effdateTemp = vm.dateFormat($scope.effdateTemp);
            }
            $scope.appdateTemp = $scope.appdate;
            if ($scope.appdateTemp == "") {
                $scope.appdateTemp = null;
            }
            else if ($scope.appdateTemp != "" && $scope.appdateTemp != null) {
                $scope.appdateTemp = vm.dateFormat($scope.appdateTemp);
            }
            if ($scope.epfTemp.indexOf("/") !== -1) {
                // Code
                $scope.epfTemp = $scope.epfTemp.replaceAll("/", "~2f");
            }
            if ($scope.csfTemp.indexOf("/") !== -1) {
                // Code
                $scope.csfTemp = $scope.csfTemp.replaceAll("/", "~2f");
            }
            var offset = (current * 10) - 10;
            var limit = 15;
            var data = $scope.effdateTemp + "|" + $scope.appdateTemp + "|" + $scope.empName.toLowerCase() + "|" + $scope.eletter.toLowerCase() + "|" + $scope.act.toLowerCase() + "|" + $scope.epfTemp.toLowerCase() + "|" + $scope.csfTemp.toLowerCase() + "|" + $scope.stat.toLowerCase() + "|" + $scope.cop.toLowerCase() + "|" + offset + "|" + limit;
            console.info(data);
            CSFDashboardService.GetEpaf(data, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.epaf = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.info(vm.epaf);
                } else {
                    $.growl.error({ message: "Failed To Get data ePaf" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            });
        }
        vm.link = link;
        function link(param) {
            
            return param.replaceAll('/', 'A');
        };
        vm.toExcel = toExcel;
        function toExcel(tabel){
            console.info(tabel);
            alasql('SELECT effective_date AS `ePAF Effective Date`, approved_date AS `ePAF Approved Date`, employee_name AS `Employee Name`, eletter_sent AS `eLetter Sent`, epaf_action AS `Action`, epaf_number AS `ePAF No`, ref AS `CSF Number`,Sts AS `CSF Status`, COP_3 AS `COP < 3 Years` INTO XLSX("csf.xlsx",{headers:true}) FROM ?', [vm.epaf]);
        }
        vm.hapus = hapus;
        function hapus(data) {
            console.info(data);
            CSFDashboardService.deleteEpaf({
                csfID: data
            }, function (reply) {
                if (reply.status == 200) {
                    vm.init();
                    UIControlService.unloadLoading();
                }
                else {
                    $.growl.error({ message: "Assign Failed" });
                    vm.init();
                    UIControlService.unloadLoading();
                }
            },
            function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            }
            )
            vm.init();
        }
    }
})();
