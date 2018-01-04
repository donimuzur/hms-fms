(function () {
    'use strict';

    angular.module("app").controller("CRFDashCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', 'CRFDashboardService', '$state', '$stateParams',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$interval', '$timeout'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, CRFDashboardService, $state, $stateParams,
        UploadFileConfigService, ExcelReaderService, UploaderService, $interval, $timeout) {
            var vm = $scope;

            vm.epaf = [];
            $scope.currentPage = 1;
            vm.fullSize = 10;
            vm.offset = (vm.currentPage * 10) - 10;
            vm.totalRecords = 0;
            vm.yes = 0;
            vm.coordinator = localStorage.getItem('username');
            vm.id_user;
            vm.init = init;
            $scope.field = {};
            vm.dateFormat = dateFormat;
            vm.roles=localStorage.getItem('roles');
            String.prototype.replaceAll = function (search, replacement) {
                var target = this;
                return target.replace(new RegExp(search, 'g'), replacement);
            };
            function dateFormat(param) {
                var indexDepan = param.indexOf("(");
                var indexBelakang = param.indexOf(")");
                param = param.substring(indexDepan+1,indexBelakang);
                console.log(param);
                param = new Date(Number(param));
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
                $scope.field.top = mouseEvent.pageY + 20 + 'px';
                return this.hover = true;
            };
            
            console.info($stateParams);
            if ($stateParams.rq != "$s") {
                vm.admin = true;
            }
            else {
                vm.admin = false;
            }
            vm.isCalendarOpened = [false, false, false];
            vm.openCalendar = openCalendar;
            function openCalendar(index) {
                vm.isCalendarOpened[index] = true;
            }
            $scope.hoverOut = function () {
                return this.hover = false;
            };
            function init() {
                UIControlService.loadLoading("Loading");
                vm.jLoad(1);
                console.info(vm.yes);
                console.info(vm.coordinator);
                CRFDashboardService.GetCoordinator(vm.coordinator, function (reply) {
                    vm.id_user = reply.data[0]['UserID'];
                    console.info(vm.id_user);
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                });
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
            function CheckCRF(obj, list) {
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
                var checking = CheckCRF(data, vm.tampung);
                if (checking == false) {
                    vm.tampung.push(data);
                    counter = counter + 1;
                    console.info(vm.tampung + counter);
                }
                else {
                    vm.tampung = remove(vm.tampung, data);
                    counter = counter - 1;
                    console.info(vm.tampung + counter);
                }
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
                            employeeID: vm.tampung[i].employee_id,
                            employeeName: vm.tampung[i].employee_name,
                            costCenter: vm.tampung[i].cost_center,
                            groupLevel: vm.tampung[i].group_level,
                            currentAddres:vm.tampung[i].city,
                            currentLocation:vm.tampung[i].office_location,
                            newAddress:vm.tampung[i].to_city,
                            newLocation: vm.tampung[i].to_office_location,
                            fleetID: vm.tampung[i].fleet_id,
                            coordinator: vm.id_user,
                            relocationType:vm.tampung[i].vehicle_type,
                            role:vm.roles,
                            remark:0,
                            action: vm.tampung[i].epaf_action
                        };
                        epaf.push(temp);
                    }
                    console.info(epaf);
                    CRFDashboardService.SaveCRF(epaf, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            vm.init();
                        } else {
                            $.growl.error({ message: "Assign CRF Failed" });
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
                    UIControlService.msg_growl('warning', 'epaf belum dipilih!');
                }
            }
            vm.generateLink = generateLink;
            function generateLink(param) {
                var link = '#/crf/benefit/' + param.reference_number.replaceAll('/', 'A');
                return link;
            };
            $scope.epfdate = null;
            $scope.effdate = null;
            $scope.elet = ""
            $scope.empName = "";
            $scope.currloc = "";
            $scope.newloc = "";
            $scope.epf = "";
            $scope.cc = "";
            $scope.crf = "";
            $scope.rsn = "";
            vm.jLoad = jLoad;
            function jLoad(current) {
                $scope.epfTemp = $scope.epf;
                $scope.crfTemp = $scope.crf;

                if ($scope.epfTemp.indexOf("/") !== -1) {
                    // Code
                    $scope.epfTemp = $scope.epfTemp.replaceAll("/", "~2f");
                }
                if ($scope.crfTemp.indexOf("/") !== -1) {
                    // Code
                    $scope.crfTemp = $scope.crfTemp.replaceAll("/", "~2f");
                }
                $scope.epfdateTemp = $scope.epfdate;
                if ($scope.epfdateTemp == "") {
                    $scope.epfdateTemp = null;
                }
                else if ($scope.epfdateTemp != "" && $scope.epfdateTemp != null) {
                    $scope.epfdateTemp = vm.dateFormat($scope.epfdateTemp);
                }
                $scope.effdateTemp = $scope.effdate;
                if ($scope.effdateTemp == "") {
                    $scope.effdateTemp = null;
                }
                else if ($scope.effdateTemp != "" && $scope.effdateTemp != null) {
                    $scope.effdateTemp = vm.dateFormat($scope.effdateTemp);
                }
                var offset = (current * 10) - 10;
                var limit = 15;
                var data = $scope.epfdateTemp + "|" + $scope.effdateTemp + "|" + $scope.elet.toLowerCase() + "|" + $scope.empName.toLowerCase() + "|" + $scope.currloc.toLowerCase() + "|" + $scope.newloc.toLowerCase() + "|" + $scope.epfTemp.toLowerCase() + "|" + $scope.cc.toLowerCase() + "|" + $scope.crfTemp.toLowerCase() + "|" + $scope.rsn.toLowerCase() + "|" + offset + "|" + limit;
                console.info(data);
                CRFDashboardService.GetEpaf(data,function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        vm.epafs = reply.data.List;
                        $scope.totalRecords = reply.data.Count;
                        console.log(vm.epafs)
                    } else {
                        $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                    UIControlService.unloadLoading();
                });
                UIControlService.unloadLoading();
                $interval.cancel();
            }
            vm.toExcel = toExcel;
            function toExcel(tabel) {
                console.info(tabel);
                var exportHref;
                alasql('SELECT effective_date AS `ePAF Date`, approved_date AS `ePAF Approved Date`, eletter_sent AS `eLetter sent(s)`, employee_name AS `Employee Name`, office_location AS `Current Location`, to_office_location AS `Relocate Location`, epaf_number AS `ePAF No`,cost_center AS `Cost Center`, reference_number AS `CRF Number`, epaf_action AS `Reason` INTO XLSX("crf.xlsx",{headers:true}) FROM ?', [vm.epaf]);
                //exportHref = CRFDashboardService.tableToExcel(tabel, 'tabel epaf');
                //$timeout(function () { location.href = exportHref; }, 100);
            }
            vm.hapus = hapus;
            function hapus(data) {
                console.info(data);
                CRFDashboardService.deleteEpaf({
                    csfID: data
                }, function (reply) {
                    if (reply.status == 200) {
                        vm.init();
                        UIControlService.unloadLoading();
                    }
                    else {
                        $.growl.error({ message: "Gagal assign" });
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
