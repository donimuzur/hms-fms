(function () {
    'use strict';

    angular.module('app').filter('monthYear', function () {
        return function (input, defaultValue) {
            return getMonth(input);
        }
    });

    angular.module('app').filter('ifEmpty', function () {
        return function (input, defaultValue) {
            if (angular.isUndefined(input) || input === null || input === '') {
                return defaultValue;
            }

            return input;
        }
    });

    angular.module("app").filter('slugify', function () {
        return function (input) {
            if (!input)
                return;

            // make lower case and trim
            var slug = input.toLowerCase().trim();

            // replace invalid chars with spaces
            slug = slug.replace(/[^a-z0-9\s-]/g, '');

            // replace multiple spaces or hyphens with a single hyphen
            slug = slug.replace(/[\s-]+/g, '-');

            return slug;
        };
    });

    angular.module("app").controller("SUMDashboardCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', 'CFMService', 'CSFService', 'SUMService', '$state', '$stateParams', '$filter'];
	
    function ctrl($http, $translate, $scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, DTOptionsBuilder, CFMService, CSFService, SUMService, $state, $stateParams, $filter) {
        var vm = $scope;

        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles');

        var arrMonth = [];
        var chartValue = [];

        var date = new Date();
        var months = [],
            monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
            mlist = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        for (var i = 0; i < 12; i++) {
            months.push({ value: (date.getMonth() + 1), name: monthNames[date.getMonth()] + ' ' + date.getFullYear() });
            date.setMonth(date.getMonth() - 1);
        }

        $scope.arrMonth = months;

        var date = new Date();
        $scope.thisMonth = monthNames[date.getMonth()] + '-' + date.getFullYear();

        //$scope.selectedMonth = date.getMonth();
        //$scope.selectedMonthFilter = function (element) {
        //    if (!$scope.selectedMonth) return true;
        //    return element.created.getMonth() == $scope.selectedMonth;
        //}

        vm.update = function (m) {
            //$window.alert(m);
            jLoad(m);
        }

        vm.init = init;
        function init() {
            if (vm.roles == 'SYSTEM.ROLE_HR' || vm.roles == 'SYSTEM.ROLE_FLEET') {

                vm.jLoad(1);
                $scope.thisMonth = monthNames[(1 - 1)] + '-' + date.getFullYear();

            } else {
                $state.go('home');
            }
        }

        /*
            Executive Summary
            - Report 2
        */

        vm.jLoad = jLoad;
        function jLoad(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
            }

            var data = {
                DateFilter: year + '-' + month
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();

            /*
                REPORT VEHICLES
            */

            CFMService.SUMPtdMonthlyVehicle(data, function (reply) {
                //console.info(reply.data);

                //$scope.VehiclebyFunction = reply.data;
                //$scope.getTotal = function (int) {
                //    var total = 0;
                //    angular.forEach($scope.VehiclebyFunction, function (el) {
                //        total += el[int];
                //    });
                //    return total;
                //};

                var objData = reply.data;
                if (0 != chartdiv_4.dataProvider.length) {
                    chartdiv_4.dataProvider = [];
                }

                var newData = [];
                //for (var i = 0; i < objData.length; i++) {
                //    var obj = objData[i];
                //    for (var key in obj) {
                //        newData.push({
                //            "label": key,
                //            "value": obj[key]
                //        });
                //    }
                //}

                angular.forEach(objData, function (filterObj) {
                    //console.info(filterObj);
                    delete filterObj.report;
                    for (var key in filterObj) {
                        //newData.push({
                        //    "label": key,
                        //    "value": filterObj[key]
                        //});

                        chartdiv_4.dataProvider.push({
                            "label": key,
                            "value": filterObj[key]
                        });
                    }
                });

                //AmCharts.checkEmptyData(chartdiv_4);
                //console.info(newData);
                chartdiv_4.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });


            /*
                REPORT ACCIDENT / INCIDENT
            */
            CFMService.AccidentAll(data, function (reply) {
                //console.info(reply.data);

                var objData = reply.data;

                //if (objData.length == 0) {
                //    angular.element(document.querySelector("#chartdiv")).parent().parent().parent().addClass("hide");
                //} else {
                //    angular.element(document.querySelector("#chartdiv")).parent().parent().parent().removeClass("hide");
                //}

                if (0 != chartdiv.dataProvider.length) {
                    chartdiv.dataProvider = [];
                }

                angular.forEach(objData, function (fObj) {
                    //console.info(filterObj);
                    fObj.label = fObj.function;
                    //delete fObj.function;
                    fObj.value = fObj.Jan_17;
                    //delete fObj.Jan_17;
                    chartdiv.dataProvider.push(fObj);
                });

                chartdiv.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });

            /*
                REPORT FUEL COST (mio IDR) VS LEASING COST (mio IDR)
            */
            CFMService.Lease_FuelMonthly(data, function (reply) {
                //console.info(reply.data);

                var objData = reply.data;
                if (0 != chartdiv_2.dataProvider.length || 0 != chartdiv_8.dataProvider.length) {
                    chartdiv_2.dataProvider = [];
                    chartdiv_8.dataProvider = [];
                }

                angular.forEach(objData, function (filterObj) {
                    //console.info(filterObj);

                    //if (filterObj.FUEL_COST != null) {
                        filterObj.fuel = filterObj.FUEL_COST;
                        delete filterObj.FUEL_COST;

                        //filterObj.leasing = 0; //filterObj.LEASE;
                        //delete filterObj.LEASE;
                        chartdiv_2.dataProvider.push(filterObj);
                    //}

                });

                chartdiv_2.validateData();

                angular.forEach(objData, function (filterObj) {
                    //console.info(filterObj);

                    if (filterObj.LEASE != null) {
                        filterObj.leasing = filterObj.LEASE;
                        delete filterObj.LEASE;

                        //filterObj.leasing = 0; //filterObj.LEASE;
                        //delete filterObj.LEASE;
                        chartdiv_8.dataProvider.push(filterObj);
                    }

                });

                chartdiv_8.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });


            /*
                REPORT FUEL PURCHASED (LITERS) VS KM DRIVEN
            */
            CFMService.KMD_FuelMonthly(data, function (reply) {
                //console.info(reply.data);

                var objData = reply.data;
                if (0 != chartdiv_3.dataProvider.length || 0 != chartdiv_9.dataProvider.length) {
                    chartdiv_3.dataProvider = [];
                    chartdiv_9.dataProvider = [];
                }

                angular.forEach(objData, function (filterObj) {
                    //console.info(filterObj);

                    if (filterObj.Fuel_Purchase != null) {
                        filterObj.liters = filterObj.Fuel_Purchase.toFixed();
                        delete filterObj.Fuel_Purchase;

                        //filterObj.leasing = 0; //filterObj.LEASE;
                        //delete filterObj.LEASE;
                        chartdiv_3.dataProvider.push(filterObj);
                    }

                });

                chartdiv_3.validateData();

                angular.forEach(objData, function (filterObj) {
                    //console.info(filterObj);

                    if (filterObj.KM_Driven != null) {
                        filterObj.km = Math.round(filterObj.KM_Driven);
                        delete filterObj.KM_Driven;

                        //filterObj.leasing = 0; //filterObj.LEASE;
                        //delete filterObj.LEASE;
                        chartdiv_9.dataProvider.push(filterObj);
                    }

                });

                chartdiv_9.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });


            /*
                REPORT KM'S DRIVEN/VEHICLE
            */
            CFMService.KMD_VehicleMonthly(data, function (reply) {
                //console.info(reply.data);

                var objData = reply.data;
                if (0 != chartdiv_5.dataProvider.length) {
                    chartdiv_5.dataProvider = [];
                }

                angular.forEach(objData, function (filterObj) {
                    //console.info(filterObj);

                    if (filterObj.KMperVehicle != null) {
                        filterObj.km = Math.round(filterObj.KMperVehicle);
                        //delete filterObj.Fuel_Purchase;

                        //filterObj.leasing = 0; //filterObj.LEASE;
                        //delete filterObj.LEASE;
                        chartdiv_5.dataProvider.push(filterObj);
                    }

                });

                chartdiv_5.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });

            /*
                REPORT ACCIDENT INDEX (PER 1,000,000 KM)
            */
            CFMService.AccidentIndexMonthly(data, function (reply) {
                //console.info(reply.data);

                var objData = reply.data;
                if (0 != chartdiv_6.dataProvider.length) {
                    chartdiv_6.dataProvider = [];
                }

                angular.forEach(objData, function (filterObj) {
                    //console.info(filterObj);

                    if (filterObj.AccidentIndex != null) {
                        filterObj.AccidentIndex = Math.round(filterObj.AccidentIndex);
                        //delete filterObj.Fuel_Purchase;

                        //filterObj.leasing = 0; //filterObj.LEASE;
                        //delete filterObj.LEASE;
                        chartdiv_6.dataProvider.push(filterObj);
                    }

                });

                chartdiv_6.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });


            /*
                REPORT OPERATIONAL COST PER KM (IN IDR)
            */
            CFMService.OperationalCostperKMMonthly(data, function (reply) {
                //console.info(reply.data);

                var objData = reply.data;
                if (0 != chartdiv_7.dataProvider.length) {
                    chartdiv_7.dataProvider = [];
                }

                angular.forEach(objData, function (filterObj) {
                    //console.info(filterObj);

                    //if (filterObj.OperationalCostperKM != null) {
                        filterObj.cost = Math.round(filterObj.OperationalCostperKM);
                        delete filterObj.OperationalCostperKM;

                        //filterObj.leasing = 0; //filterObj.LEASE;
                        //delete filterObj.LEASE;
                        chartdiv_7.dataProvider.push(filterObj);
                    //}

                });

                chartdiv_7.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //SumPTD
        vm.SumPTD = SumPTD;
        function SumPTD(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }

            CFMService.SumPTD(data, function (reply) {
                //console.info(reply.data);

                var newData = [];
                angular.forEach(reply.data, function (filterObj) {
                    filterObj.function = filterObj.report;
                    delete filterObj.report;
                    newData.push(filterObj);
                });

                $scope.SumPTD = newData;
                $scope.getTotalSumPTD = function (int) {
                    var total = 0;
                    angular.forEach($scope.SumPtdYtd, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    filterObj.HR = Math.round(filterObj.HR);
                    filterObj.IS = Math.round(filterObj.IS);
                    filterObj.MANAGEMENT = Math.round(filterObj.MANAGEMENT);
                    filterObj.MARKETING = Math.round(filterObj.MARKETING);
                    filterObj.OPERATIONS = Math.round(filterObj.OPERATIONS);
                    filterObj.SALES = Math.round(filterObj.SALES);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });

        }

        //SumPtdYtd
        vm.SumPtdYtd = SumPtdYtd;
        function SumPtdYtd(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }
            //console.info(data);

            //CFMService.SumPtdYtd(function (reply) {
            CFMService.SumPtdYtd(data, function (reply) {
                //console.info(reply.data);

                var newData = [];
                angular.forEach(reply.data, function (filterObj) {
                    filterObj.function = filterObj.report;
                    delete filterObj.report;
                    newData.push(filterObj);
                });

                $scope.SumPtdYtd = newData;
                $scope.getTotalPtdYtd = function (int) {
                    var total = 0;
                    angular.forEach($scope.SumPtdYtd, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_r.dataProvider.length) {
                    chart_r.dataProvider = [];
                }

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    filterObj.HR = Math.round(filterObj.HR);
                    filterObj.IS = Math.round(filterObj.IS);
                    filterObj.MANAGEMENT = Math.round(filterObj.MANAGEMENT);
                    filterObj.MARKETING = Math.round(filterObj.MARKETING);
                    filterObj.OPERATIONS = Math.round(filterObj.OPERATIONS);
                    filterObj.SALES = Math.round(filterObj.SALES);
                    chart_r.dataProvider.push(filterObj);
                });
                chart_r.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //Sum Quarter
        vm.SumQuarter = SumQuarter;
        function SumQuarter() {

            //var date = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            var quarter = Math.floor((date.getMonth() + 3) / 3);
            //$window.alert("Quarter:" + quarter);
            $scope.period = quarter;

            var year = date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }
            //console.info(data);

            CFMService.SumQuarter1(data, function (reply) {
                //console.info(reply.data);

                $scope.period = quarter;
                var newData = [];
                angular.forEach(reply.data, function (filterObj) {
                    filterObj.function = filterObj.report;
                    delete filterObj.report;
                    newData.push(filterObj);
                });

                $scope.SumQuarter = newData;
                $scope.getTotalQuarter = function (int) {
                    var total = 0;
                    angular.forEach($scope.SumQuarter, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    filterObj.HR = Math.round(filterObj.HR);
                    filterObj.IS = Math.round(filterObj.IS);
                    filterObj.MANAGEMENT = Math.round(filterObj.MANAGEMENT);
                    filterObj.MARKETING = Math.round(filterObj.MARKETING);
                    filterObj.OPERATIONS = Math.round(filterObj.OPERATIONS);
                    filterObj.SALES = Math.round(filterObj.SALES);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });



            CFMService.SumQuarter2(data, function (reply) {
                //console.info(reply.data);

                $scope.period = quarter;
                var newData = [];
                angular.forEach(reply.data, function (filterObj) {
                    filterObj.function = filterObj.report;
                    delete filterObj.report;
                    newData.push(filterObj);
                });

                $scope.SumQuarter = newData;
                $scope.getTotalQuarter = function (int) {
                    var total = 0;
                    angular.forEach($scope.SumQuarter, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    filterObj.HR = Math.round(filterObj.HR);
                    filterObj.IS = Math.round(filterObj.IS);
                    filterObj.MANAGEMENT = Math.round(filterObj.MANAGEMENT);
                    filterObj.MARKETING = Math.round(filterObj.MARKETING);
                    filterObj.OPERATIONS = Math.round(filterObj.OPERATIONS);
                    filterObj.SALES = Math.round(filterObj.SALES);
                    chart_2.dataProvider.push(filterObj);
                });
                chart_2.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });



            CFMService.SumQuarter3(data, function (reply) {
                //console.info(reply.data);

                $scope.period = quarter;
                var newData = [];
                angular.forEach(reply.data, function (filterObj) {
                    filterObj.function = filterObj.report;
                    delete filterObj.report;
                    newData.push(filterObj);
                });

                $scope.SumQuarter = newData;
                $scope.getTotalQuarter = function (int) {
                    var total = 0;
                    angular.forEach($scope.SumQuarter, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    filterObj.HR = Math.round(filterObj.HR);
                    filterObj.IS = Math.round(filterObj.IS);
                    filterObj.MANAGEMENT = Math.round(filterObj.MANAGEMENT);
                    filterObj.MARKETING = Math.round(filterObj.MARKETING);
                    filterObj.OPERATIONS = Math.round(filterObj.OPERATIONS);
                    filterObj.SALES = Math.round(filterObj.SALES);
                    chart_3.dataProvider.push(filterObj);
                });
                chart_3.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });



            CFMService.SumQuarter4(data, function (reply) {
                //console.info(reply.data);

                $scope.period = quarter;
                var newData = [];
                angular.forEach(reply.data, function (filterObj) {
                    filterObj.function = filterObj.report;
                    delete filterObj.report;
                    newData.push(filterObj);
                });

                $scope.SumQuarter = newData;
                $scope.getTotalQuarter = function (int) {
                    var total = 0;
                    angular.forEach($scope.SumQuarter, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    filterObj.HR = Math.round(filterObj.HR);
                    filterObj.IS = Math.round(filterObj.IS);
                    filterObj.MANAGEMENT = Math.round(filterObj.MANAGEMENT);
                    filterObj.MARKETING = Math.round(filterObj.MARKETING);
                    filterObj.OPERATIONS = Math.round(filterObj.OPERATIONS);
                    filterObj.SALES = Math.round(filterObj.SALES);
                    chart_4.dataProvider.push(filterObj);
                });
                chart_4.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //VehiclebyFunction
        vm.VehiclebyFunction = VehiclebyFunction;
        function VehiclebyFunction(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                VehiclebyPurposes(m);

                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }
            //console.info(data);

            CFMService.NumberOfVehicleByFunction(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    //filterObj.function = filterObj.by_purposes;
                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = filterObj[key];
                                newdata.push(filterObj);
                            }
                        }
                    }
                    
                });
                
                $scope.VehiclebyFunction = newdata;
                $scope.getTotal = function (int) {
                    var total = 0;
                    angular.forEach($scope.VehiclebyFunction, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);                    
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //VehiclebyPurposes
        vm.VehiclebyPurposes = VehiclebyPurposes;
        function VehiclebyPurposes(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }
            console.info(data);

            CFMService.NumberOfVehicleByPurpose(data, function (reply) {
                //console.info(reply.data);

                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    //filterObj.function = filterObj.by_purposes;
                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = filterObj[key];
                                newdata.push(filterObj);
                            }
                        }
                    }
                });

                $scope.VehiclebyPurposes = newdata;
                $scope.getTotalPurposes = function (int) {
                    var total = 0;
                    angular.forEach($scope.VehiclebyPurposes, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_r.dataProvider.length) {
                    chart_r.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    chart_r.dataProvider.push(filterObj);
                });
                chart_r.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //VehiclebyMarketing
        vm.VehiclebyMarketing = VehiclebyMarketing;
        function VehiclebyMarketing() {
            var data = {
                start: '01',
                end: '03',
                year: '2017'
            }

            CFMService.NumberOfVehicleByPurpose(data, function (reply) {
                //console.info(reply.data);

                $scope.VehiclebyPurposes = reply.data;
                $scope.getTotalPurposes = function (int) {
                    var total = 0;
                    angular.forEach($scope.VehiclebyPurposes, function (el) {
                        total += el[int];
                    });
                    return total;
                };

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //NumberOfWTCVehicleByMarketing
        vm.WTCVehicleByMarketing = WTCVehicleByMarketing;
        function WTCVehicleByMarketing(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                WTCVehicleBySales(m);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }

            CFMService.NumberOfWTCVehicleByMarketing(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {

                    for (key in filterObj) {
                        if (key != 'city') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = filterObj[key];
                                newdata.push(filterObj);
                            }
                        }
                    }

                });

                $scope.WTCVehicleByMarketing = newdata;
                $scope.getTotalWTCMarketing = function (int) {
                    var total = 0;
                    angular.forEach($scope.WTCVehicleByMarketing, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    //filterObj.function = filterObj.city;
                    //delete filterObj.city;
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //WTCVehicleBySales
        vm.WTCVehicleBySales = WTCVehicleBySales;
        function WTCVehicleBySales(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }

            CFMService.NumberOfWTCVehicleBySales(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {

                    for (key in filterObj) {
                        if (key != 'city') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = filterObj[key];
                                newdata.push(filterObj);
                            }
                        }
                    }
                    //console.info(newdata);

                });

                $scope.WTCVehicleBySales = newdata;
                $scope.getTotalWTCSales = function (int) {
                    var total = 0;
                    angular.forEach($scope.WTCVehicleBySales, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_r.dataProvider.length) {
                    chart_r.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    chart_r.dataProvider.push(filterObj);
                });
                chart_r.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //NumberVehicleByMake
        vm.NumberVehicleByMake = NumberVehicleByMake;
        function NumberVehicleByMake(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                NumberOfVehicleByBodyType(m);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }

            CFMService.NumberVehicleByMake(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    filterObj.function = filterObj.manufacturer;
                    delete filterObj.manufacturer;

                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = filterObj[key];
                                newdata.push(filterObj);
                            }
                        }
                    }

                });

                $scope.NumberVehicleByMake = newdata;
                $scope.getTotalByMake = function (int) {
                    var total = 0;
                    angular.forEach($scope.NumberVehicleByMake, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //NumberOfVehicleByBodyType
        vm.NumberOfVehicleByBodyType = NumberOfVehicleByBodyType;
        function NumberOfVehicleByBodyType(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }

            CFMService.NumberOfVehicleByBodyType(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    filterObj.function = filterObj.manufacturer;
                    delete filterObj.manufacturer;

                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = filterObj[key];
                                newdata.push(filterObj);
                            }
                        }
                    }
                });

                $scope.NumberOfVehicleByBodyType = newdata;
                $scope.getTotalBodyType = function (int) {
                    var total = 0;
                    angular.forEach($scope.NumberOfVehicleByBodyType, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_r.dataProvider.length) {
                    chart_r.dataProvider = [];
                }

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    chart_r.dataProvider.push(filterObj);
                });
                chart_r.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //OdometerReport
        vm.OdometerReport = OdometerReport;
        function OdometerReport(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                OdometerReportBySalesMkt(m);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month
            }

            CFMService.OdometerReport(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = Math.round(filterObj[key]);
                                newdata.push(filterObj);
                            }
                        }
                    }
                });

                $scope.OdometerReport = reply.data;
                $scope.getTotalOdometer = function (int) {
                    var total = 0;
                    angular.forEach($scope.OdometerReport, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(reply.data, function (filterObj) {
                    //console.info(filterObj);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //OdometerReportBySalesMkt
        vm.OdometerReportBySalesMkt = OdometerReportBySalesMkt;
        function OdometerReportBySalesMkt(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                //DateFilter: year + '-' + month,
                city: 'Jakarta',
                role: vm.roles
            }

            CFMService.OdometerReportBySalesMkt(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = Math.round(filterObj[key]);
                                newdata.push(filterObj);
                            }
                        }
                    }
                });

                $scope.OdometerReportBySalesMkt = newdata;
                $scope.getTotalOdoSalesMkt = function (int) {
                    var total = 0;
                    angular.forEach($scope.OdometerReportBySalesMkt, function (el) {
                        total += el[int];
                    });
                    return total;
                };
            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //LiterByFunction
        vm.LiterByFunction = LiterByFunction;
        function LiterByFunction(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month,
            }

            CFMService.LiterByFunction(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = Math.round(filterObj[key]);
                                newdata.push(filterObj);
                            }
                        }
                    }
                });

                $scope.LiterByFunction = newdata;
                $scope.getTotalLiter = function (int) {
                    var total = 0;
                    angular.forEach($scope.LiterByFunction, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //FuelCostByFunction
        vm.FuelCostByFunction = FuelCostByFunction;
        function FuelCostByFunction(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
                //FuelCostBySalesMkt
                FuelCostBySalesMkt(m);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month,
            }

            CFMService.FuelCostByFunction(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = Math.round(filterObj[key]);
                                newdata.push(filterObj);
                            }
                        }
                    }
                });

                $scope.FuelCostByFunction = newdata;
                $scope.getTotalFuelCost = function (int) {
                    var total = 0;
                    angular.forEach($scope.FuelCostByFunction, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //FuelCostBySalesMkt
        vm.FuelCostBySalesMkt = FuelCostBySalesMkt;
        function FuelCostBySalesMkt(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month,
            }

            CFMService.FuelCostBySalesMkt(data, function (reply) {
                //console.info(reply.data);
                var newdata = [];
                angular.forEach(reply.data, function (filterObj) {
                    for (key in filterObj) {
                        if (key != 'function') {
                            //console.info(chartValue + " => " + key + " = " + filterObj[key]);
                            if (key == chartValue) {
                                filterObj.Jan_17 = Math.round(filterObj[key]);
                                newdata.push(filterObj);
                            }
                        }
                    }
                });

                $scope.FuelCostBySalesMkt = newdata;
                $scope.getTotalFuelCostSalesMkt = function (int) {
                    var total = 0;
                    angular.forEach($scope.FuelCostBySalesMkt, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });

        }

        //Lease Cost by Function
        vm.LeaseCostbyFunction = LeaseCostbyFunction;
        function LeaseCostbyFunction(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month,
            }

            CFMService.LeaseCost(data, function (reply) {
                //console.info(reply.data);
                var newdata = reply.data;
                //angular.forEach(reply.data, function (filterObj) {
                //    newdata.push(filterObj);
                //});

                $scope.LeaseCostbyFunction = newdata;
                $scope.getTotalLeaseCost = function (int) {
                    var total = 0;
                    angular.forEach($scope.LeaseCostbyFunction, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                $scope.getTotal = function () {
                    var total = 0;
                    angular.forEach($scope.LeaseCostbyFunction, function (item) {
                        total += (item.Lease + item.short);
                    });
                    return total;
                }

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
            
        }

        //AC vs OB
        vm.ACOBTotalMonthly = ACOBTotalMonthly;
        function ACOBTotalMonthly(m) {
            //var d = new Date();

            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                //chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month,
            }
            console.info(data);

            CFMService.ACOBTotalMonthly(data, function (reply) {
                //console.info(data);

                var newdata = reply.data;
                $scope.ACOBTotalMonthly = newdata;
                $scope.getTotalACOB = function (int) {
                    var total = 0;
                    angular.forEach($scope.ACOBTotalMonthly, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    //delete filterObj.dua;
                    //delete filterObj.tiga;

                    //console.info(filterObj);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //Sales by Regions
        vm.SalesByRegionPerStik = SalesByRegionPerStik;
        function SalesByRegionPerStik(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month,
            }

            CFMService.SalesByRegionPerStik(data, function (reply) {
                //console.info(reply.data);
                var newdata = reply.data;
                $scope.SalesByRegionPerStik = newdata;
                $scope.getTotalSalesStik = function (int) {
                    var total = 0;
                    angular.forEach($scope.SalesByRegionPerStik, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(newdata, function (filterObj) {
                    //console.info(filterObj);
                    delete filterObj.dua;
                    delete filterObj.tiga;

                    //console.info(filterObj);
                    chart_l.dataProvider.push(filterObj);
                });
                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });

        }

        vm.SalesByRegionPerkm = SalesByRegionPerkm;
        function SalesByRegionPerkm(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            $scope.thisMonth = monthNames[(month - 1)] + '-' + date.getFullYear();
            var data = {
                DateFilter: year + '-' + month,
            }

            CFMService.SalesByRegionPerkm(data, function (reply) {
                //console.info(reply.data);

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });

        }

        //Accident
        vm.AccidentAll = AccidentAll;
        function AccidentAll(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if(m!=null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);

                //AccidentBenefit
                AccidentBenefit(m);
            }

            var data = {
                DateFilter: year + '-' + month
            };

            CFMService.AccidentAll(data, function (reply) {
                //console.info(reply.data);

                $scope.AccidentAll = reply.data;
                $scope.getTotalAccident = function (int) {
                    var total = 0;
                    angular.forEach($scope.AccidentAll, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                var objData = reply.data;
                
                if (0 != chart_l.dataProvider.length) {
                    chart_l.dataProvider = [];
                }

                angular.forEach(objData, function (fObj) {
                    //console.info(filterObj);
                    fObj.label = fObj.function;
                    //delete fObj.function;
                    fObj.value = fObj.Jan_17;
                    //delete fObj.Jan_17;
                    chart_l.dataProvider.push(fObj);
                });

                chart_l.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        //AccidentBenefit
        vm.AccidentBenefit = AccidentBenefit;
        function AccidentBenefit(m) {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            if (m != null) {
                month = ('0' + (m)).slice(-2);
                chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            }

            var data = {
                DateFilter: year + '-' + month
            };

            CFMService.AccidentBenefit(data, function (reply) {
                //console.info(reply.data);

                $scope.AccidentBenefit = reply.data;
                $scope.getTotalAccidentB = function (int) {
                    var total = 0;
                    angular.forEach($scope.AccidentBenefit, function (el) {
                        total += el[int];
                    });
                    return total;
                };

                var objData = reply.data;

                if (0 != chart_r.dataProvider.length) {
                    chart_r.dataProvider = [];
                }

                angular.forEach(objData, function (fObj) {
                    //console.info(filterObj);
                    fObj.label = fObj.function;
                    //delete fObj.function;
                    fObj.value = fObj.Jan_17;
                    //delete fObj.Jan_17;
                    chart_r.dataProvider.push(fObj);
                });

                chart_r.validateData();

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }


        /*
         * Executive Summary Report by Region
         */

        //SUMbyRegion
        vm.SUMbyRegion = SUMbyRegion;
        function SUMbyRegion() {

            //var d = new Date();
            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            //if (m != null) {
            //    month = ('0' + (m)).slice(-2);
            //    chartValue = mlist[(month - 1)] + '_' + date.getFullYear().toString().substring(2);
            //}

            var data = {
                DateFilter: year
            };

            CFMService.SUMbyRegion(data, function (reply) {
                //console.info(reply.data);

            }, function (err) {
                $window.alert("error:" + JSON.stringify(err));
            });
        }

        /*
         * End Report by Region
         */

        function getMonth() {
            var d = new Date();
            var month = d.getMonth() + 1;
            return month < 10 ? '0' + month : '' + month; // ('' + month) for string result
        }

        function format(date) {
            date = new Date(date);

            var day = ('0' + date.getDate()).slice(-2);
            var month = ('0' + (date.getMonth() + 1)).slice(-2);
            var year = date.getFullYear();

            return year + '-' + month + '-' + day;
        }

        //var month_name = function (dt) {
        //    mlist = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        //    return mlist[dt.getMonth() + '_' + dt.getFullYear()];
        //};
		
	}

})();