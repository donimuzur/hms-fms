(function () {
    'use strict';

    angular.module("app").controller("MSTPNCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'MstService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.datapricelist = [];
        vm.totalRecords = 0;
        vm.roles = localStorage.getItem('roles');
        vm.man = "";
        vm.mod = "";
        vm.ser = "";
        vm.vehyear = "";
        vm.monthstart = "";
        vm.monthend = "";
        vm.penalty = "";
        vm.vcty = "";
        vm.last = "";
        vm.modif = "";
        vm.logic = "";
        vm.vendor = "";
        vm.off = 1;
        vm.dataexcel;
        vm.init = init;
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
        vm.dates = dates
        function dates(param) {

            vm.tes = new Date(parseInt(param));
            return vm.tes;
            console.log(param)
        }
        function init() {
            UIControlService.loadLoading("loading");
            MstService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                console.info(vm.id_coordinator);
                console.log('coba')
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.loadpricelist();
        }

        vm.loadpricelist = loadpricelist;
        function loadpricelist() {
            UIControlService.unloadLoading();            
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var rep = vm.ser.replace(".", "~2b");
            var repl = rep.replace("/", "~2a");
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            if (vm.format(vm.last) == "NaN-undefined-NaN") {
                var data = vm.man + "|" + vm.mod + "|" + repl + "|" + vm.vehyear + "|" + vm.monthstart + "|" + vm.monthend + "|" + vm.penalty + "|" + vm.vcty + "|"+vm.vendor+"|"+vm.logic+"|" + vm.last + "|" + vm.modif + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.man + "|" + vm.mod + "|" + repl + "|" + vm.vehyear + "|" + vm.monthstart + "|" + vm.monthend + "|" + vm.penalty + "|" + vm.vcty + "|" + vm.vendor + "|"+vm.logic+"|" + vm.format(vm.last) + "|" + vm.modif + "|" + offset + "|" + pageSize + "|" + newss;
            }
            console.log(data)
            MstService.GetPenaltyExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetPenalty(data, function (reply) {
                if (reply.status === 200) {
                    vm.datapricelist = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.datapricelist)
                    console.log(reply)

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log("oke Gan");
        }

        vm.inspricelist = inspricelist;
        function inspricelist() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    //$scope.data = [];
                    $scope.manufacturer;
                    $scope.model;
                    $scope.series;
                    $scope.vehyear;
                    $scope.months;
                    $scope.monthe;
                    $scope.penalty;
                    $scope.vcty;
                    $scope.datasem = [];
                    $scope.datavhct = [];
                    $scope.datamodel = [];
                    $scope.datamanufacturer = [];
                    $scope.dataseries = [];
                    $scope.namealias = [];
                    $scope.logics = [];
                    $scope.logic;
                    MstService.GetVhcTPri(function (responnya) {
                        $scope.datavhct = responnya.data;
                        console.log($scope.datavhct);
                    })
                    MstService.VehicleSpecsByManufacturer(function (resp) {
                        $scope.vehspec = resp.data;

                    })
                    MstService.Getvendor(function (respon) {
                        $scope.namealias = respon.data;
                    })
                    $scope.cekmodel = function (param) {
                        MstService.cekmodel(param, function (respo) {
                            $scope.datamodel = respo.data;
                            console.log($scope.datamodel)
                        })
                    }
                    $scope.cekmanufacturer = function (param) {
                        MstService.cekmanufacturer(param, function (res) {
                            $scope.datamanufacturer = res.data;
                            console.log($scope.datamanufacturer)
                        })
                    }
                    MstService.GetPenaltyLogic(function (reply) {
                        $scope.logics = reply.data;
                    })
                    $scope.cekseries = function (param) {
                        var rep = param.replace(".", "|");
                        var repl = rep.replace("/", "~2a");
                        MstService.cekseries(repl, function (re) {
                            $scope.dataseries = re.data;
                            console.log($scope.dataseries)
                        })
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.insert = function () {

                        if ($scope.vcus == "" || $scope.vcus == undefined) {
                            UIControlService.msg_growl("error", "Manufacturer Must Be Fill");
                        } else if ($scope.mod == "" || $scope.mod == undefined) {
                            UIControlService.msg_growl("error", "Model Must Be Fill");
                        }else if ($scope.series == "" || $scope.series == undefined) {
                            UIControlService.msg_growl("error", "Series Must Be Fill");
                        }else if ($scope.vehyear == "" || $scope.vehyear == undefined) {
                            UIControlService.msg_growl("error", "Vehicle Year Must Be Fill");
                        }else if ($scope.months == "" || $scope.months == undefined) {
                            UIControlService.msg_growl("error", "Month Start Must Be Fill");
                        }else if ($scope.monthe == "" || $scope.monthe == undefined) {
                            UIControlService.msg_growl("error", "Month End Must Be Fill");
                        }else if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", "Vehicle Type Must Be Fill");
                        } else if ($scope.vendor == "" || $scope.vendor == undefined) {
                            UIControlService.msg_growl("error", "Vendor Must Be Fill");
                        } else if ($scope.logic == "" || $scope.logic == undefined) {
                            UIControlService.msg_growl("error", "Logic Must Be Fill");
                        }
                        else {
                        var data = {
                            manufacturer: $scope.vcus,
                            model: $scope.mod,
                            series: $scope.series,
                            vehicle_year: $scope.vehyear,
                            month_start: $scope.months,
                            month_end: $scope.monthe,
                            //penalty: $scope.penalty,
                            vehicle_type: $scope.vcty,
                            vendor: $scope.vendor,
                            logic_id:$scope.logic,
                            created_date: new Date(),
                            create_by: localStorage.getItem('username'),
                            modified_date: new Date(),
                            modified_by: localStorage.getItem('username'),
                            is_active: true

                        }
                        console.log(data)
                        $scope.datasem.push(data);
                        MstService.insertPenalty($scope.datasem, function (reply) {

                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];
                                    var word = [];
                                    words = reply.data.DataReturn.split(",");
                                    word = reply.data.DataReturnCadangan.split(",");
                                    console.log(word);
                                    UIControlService.msg_growl("error", "<u><i><b>" + words[i] + "</u></i></b> Already In Database");
                                    $scope.data = [];
                                }
                            }
                            else {
                                if (reply.status === 200) {
                                    UIControlService.msg_growl("success", " insert Success");
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    console.log("Berhasil Insert");
                                } else {
                                    $.growl.error({ message: "Gagal insert data vehicle spects" });
                                }
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        })
                    }
                    }
                }
            })
        }
        vm.isCalendarOpened = [false, false, false];
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }
        vm.format = format;
        function format(param) {
            param = new Date(param);
            var m_names = new Array("01", "02", "03",
  "04", "05", "06", "07", "08", "09",
  "10", "11", "12");

            //if (param.getDate().length == 1) {
            //    var curr_date = '0' + param.getDate();
            //}
            var tes = "" + param.getDate()
            if (tes.length == 1) {
                var curr_date = "0" + param.getDate();
            } else {
                var curr_date = param.getDate();
            }
            console.log(tes.length)

            var curr_month = param.getMonth();
            var curr_year = param.getFullYear();
            var date = curr_year + "-" + m_names[curr_month] + "-" + curr_date;
            if (date == "1970-01-01") {
                return "";
            } else {

                return date;
            }
        }
        vm.search = search;
        function search() {
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            var rep = vm.ser.replace(".", "~2b");
            var repl = rep.replace("/", "~2a");
            if (vm.format(vm.last) == "NaN-undefined-NaN") {
                var data = vm.man + "|" + vm.mod + "|" + repl + "|" + vm.vehyear + "|" + vm.monthstart + "|" + vm.monthend + "|" + vm.penalty + "|" + vm.vcty + "|" + vm.vendor + "|" + vm.logic + "|" + vm.last + "|" + vm.modif + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.man + "|" + vm.mod + "|" + repl + "|" + vm.vehyear + "|" + vm.monthstart + "|" + vm.monthend + "|" + vm.penalty + "|" + vm.vcty + "|" + vm.vendor + "|" + vm.logic + "|" + vm.format(vm.last) + "|" + vm.modif + "|" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.GetPenaltyExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetPenalty(data, function (reply) {
                if (reply.status === 200) {
                    vm.datapricelist = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.datapricelist)
                    console.log(reply)

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
        }
        vm.paging = paging;
        function paging(param) {
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (param * 10) - 10;
            var rep = vm.ser.replace(".", "~2b");
            var repl = rep.replace("/", "~2a");
            if (vm.format(vm.last) == "NaN-undefined-NaN") {
                var data = vm.man + "|" + vm.mod + "|" + repl + "|" + vm.vehyear + "|" + vm.monthstart + "|" + vm.monthend + "|" + vm.penalty + "|" + vm.vcty + "|" + vm.vendor + "|" + vm.logic + "|" + vm.last + "|" + vm.modif + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.man + "|" + vm.mod + "|" + repl + "|" + vm.vehyear + "|" + vm.monthstart + "|" + vm.monthend + "|" + vm.penalty + "|" + vm.vcty + "|" + vm.vendor + "|" + vm.logic + "|" + vm.format(vm.last) + "|" + vm.modif + "|" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.GetPenalty(data, function (reply) {
                if (reply.status === 200) {
                    vm.datapricelist = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.datapricelist)
                    console.log(reply)

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log(data)
        }

        vm.updpricelist = updpricelist;
        function updpricelist(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updVhcs.html',
                backdrop: 'static',
                resolve: {
                    test: function () {
                        return param;
                    }
                },
                controller: function ($uibModalInstance, $scope, test) {
                   
                    $scope.manufacturer;
                    $scope.model;
                    $scope.series;
                    $scope.vehyear = test.vehicle_year;
                    $scope.months = test.month_start;
                    $scope.check = test.is_active;
                    $scope.monthe = test.month_end;
                    $scope.penalty = test.penalty1;
                    $scope.vcty = test.vehicle_type;
                    $scope.vcus = test.manufacturer;
                    $scope.mod = test.model;
                    $scope.series = test.series;
                    $scope.datavhct = [];
                    $scope.datamodel = [];
                    $scope.datamanufacturer = [];
                    $scope.dataseries = [];
                    $scope.vendor = test.vendor;
                    $scope.logic = test.logic;
                    $scope.logics = [];
                    MstService.GetVhcTPri(function (responnya) {
                        $scope.datavhct = responnya.data;
                        console.log($scope.datavhct);
                    })
                    MstService.VehicleSpecsByManufacturer(function (resp) {
                        $scope.vehspec = resp.data;
                        $scope.cekmanufacturer($scope.vcus);
                        $scope.cekmodel($scope.mod)
                        $scope.cekseries($scope.series)
                    })

                    $scope.cekmodel = function (param) {
                        MstService.cekmodel(param, function (respo) {
                            $scope.datamodel = respo.data;
                            console.log($scope.datamodel)
                        })
                    }
                    console.log($scope.logic)
                    MstService.GetPenaltyLogic(function (reply) {
                        $scope.logics = reply.data;
                    })
                    $scope.cekmanufacturer = function (param) {
                        MstService.cekmanufacturer(param, function (res) {
                            $scope.datamanufacturer = res.data;
                            console.log($scope.datamanufacturer)
                        })
                    }
                    $scope.cekseries = function (param) {
                        var rep = param.replace(".", "|");
                        var repl = rep.replace("/", "~2a");
                        MstService.cekseries(repl, function (re) {
                            $scope.dataseries = re.data;
                            console.log($scope.dataseries)
                        })
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.Getvendor(function (respon) {
                        $scope.namealias = respon.data;
                    })
                  

                    $scope.update = function () {
                        var data;
                        if ($scope.vcus == "" || $scope.vcus == undefined) {
                            UIControlService.msg_growl("error", "Manufacturer Must Be Fill");
                        } else if ($scope.mod == "" || $scope.mod == undefined) {
                            UIControlService.msg_growl("error", "Model Must Be Fill");
                        } else if ($scope.series == "" || $scope.series == undefined) {
                            UIControlService.msg_growl("error", "Series Must Be Fill");
                        } else if ($scope.vehyear == "" || $scope.vehyear == undefined) {
                            UIControlService.msg_growl("error", "Vehicle Year Must Be Fill");
                        } else if ($scope.months == "" || $scope.months == undefined) {
                            UIControlService.msg_growl("error", "Month Start Must Be Fill");
                        } else if ($scope.monthe == "" || $scope.monthe == undefined) {
                            UIControlService.msg_growl("error", "Month End Must Be Fill");
                        } else if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", "Vehicle Type Must Be Fill");
                        } else if ($scope.vendor == "" || $scope.vendor == undefined) {
                            UIControlService.msg_growl("error", "Vendor Must Be Fill");
                        } else if ($scope.logic == "" || $scope.logic == undefined) {
                            UIControlService.msg_growl("error", "Logic Must Be Fill");
                        }
                        else {
                            if ($('.tes').is(':checked') || $scope.check == 1) {
                                data = {
                                    penalty_id:test.penalty_id,
                                    manufacturer: $scope.vcus,
                                    model: $scope.mod,
                                    series: $scope.series,
                                    vehicle_year: $scope.vehyear,
                                    month_start: $scope.months,
                                    month_end: $scope.monthe,
                                    //penalty: $scope.penalty,
                                    vehicle_type: $scope.vcty,
                                    vendor: $scope.vendor,
                                    logic_id: $scope.logic,
                                    created_date: new Date(),
                                    create_by: localStorage.getItem('username'),
                                    modified_date: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                            } else {
                                data = {
                                    penalty_id:test.penalty_id,
                                    manufacturer: $scope.manufacturer,
                                    model: $scope.model,
                                    series: $scope.series,
                                    vehicle_year: $scope.vehyear,
                                    month_start: $scope.months,
                                    month_end: $scope.monthe,
                                    //penalty: $scope.penalty,
                                    vehicle_type: $scope.vcty,
                                    vendor: $scope.vendor,
                                    logic_id: $scope.logic,
                                    created_date: new Date(),
                                    create_by: localStorage.getItem('username'),
                                    modified_date: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: false

                                }
                            }

                            MstService.updPenalty(data, function (reply) {
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", "Update Success");
                                } else {
                                    $.growl.error({ message: "Update Data Failed" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }

                    }


                }
        })
        console.log("lll");
        }

        vm.uploadVen = uploadVen;
        function uploadVen() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadCAF.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.filecaf;
                    $scope.data = [];
                    $scope.datazone = [];
                    $scope.dataveh = [];
                    $scope.datavendor = [];
                    $scope.semua = [];
                    $scope.datavehspec = [];
                    $scope.dataveh = [];
                    $scope.datazone = [];
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
                    $scope.dtOpt = DTOptionsBuilder.newOptions()
       .withDisplayLength(5)
       .withOption('bLengthChange', false)
       .withOption('responsive', true)
       .withPaginationType('full_numbers');
                    $scope.uploadFile = function () {
                        $scope.upload($scope.filevehicle);
                    };

                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file, function (reply) {
                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];
                                console.info(sheet1);
                               
                                if (sheet1[1].Column2 != "Manufacturer") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "Must be Manufacturer");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "Model") {
                                    UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "Must be Model");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column4 != "Series") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "Must be Series");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column5 != "Vehicle Year") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "Must be Vehicle Year");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column6 != "Month Start") {
                                    UIControlService.msg_growl("error", "Column 6 Name Is " + sheet1[1].Column6);
                                    UIControlService.msg_growl("error", "Must be Month Start");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column7 != "Month End") {
                                    UIControlService.msg_growl("error", "Column 7 Name Is " + sheet1[1].Column7);
                                    UIControlService.msg_growl("error", "Must be Month End");
                                    $uibModalInstance.dismiss('cancel');
                                }
                              
                                if (sheet1[1].Column8 != "Vehicle Type") {
                                    UIControlService.msg_growl("error", "Column 9 Name Is " + sheet1[1].Column9);
                                    UIControlService.msg_growl("error", "Must be Vehicle Type");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column9 != "Vendor") {
                                    UIControlService.msg_growl("error", "Column 10 Name Is " + sheet1[1].Column10);
                                    UIControlService.msg_growl("error", "Must be Penalty");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column10 != "Logic") {
                                    UIControlService.msg_growl("error", "Column 11 Name Is " + sheet1[1].Column11);
                                    UIControlService.msg_growl("error", "Must be Logic");
                                    $uibModalInstance.dismiss('cancel');
                                }

                               

                                for (var i = 2; i < sheet1.length; i++) {
                                    var item = {

                                        manufacturer: sheet1[i].Column2,
                                        model: sheet1[i].Column3,
                                        series: sheet1[i].Column4,
                                        vehicle_year: sheet1[i].Column5,
                                        month_start: sheet1[i].Column6,
                                        month_end: sheet1[i].Column7,
                                        //penalty: sheet1[i].Column8,
                                        vehicle_type: sheet1[i].Column8,
                                        vendor: sheet1[i].Column9,
                                        logic_id:sheet1[i].Column10,
                                        type:"upload",
                                        created_date: new Date(),
                                        create_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        is_active: true

                                    }
                                    //var item = {

                                    //    vendor_name: sheet1[i].Column2,
                                    //    short_name: sheet1[i].Column3,
                                    //    year: sheet1[i].Column5,
                                    //    zone_name: sheet1[i].Column4,
                                    //    vehicle_type: sheet1[i].Column6,
                                    //    vehicle_usage: sheet1[i].Column7,
                                    //    vehicle_type_id: "1",
                                    //    created_date: new Date(),
                                    //    created_by: localStorage.getItem('username'),
                                    //    last_modified: new Date(),
                                    //    modified_by: localStorage.getItem('username'),
                                    //    is_active: true

                                    //};
                                    var item = {

                                        manufacturer: sheet1[i].Column2,
                                        model: sheet1[i].Column3,
                                        series: sheet1[i].Column4,
                                        vehicle_year: sheet1[i].Column5,
                                        month_start: sheet1[i].Column6,
                                        month_end: sheet1[i].Column7,
                                        //penalty: sheet1[i].Column8,
                                        vehicle_type: sheet1[i].Column9,
                                        vendor: sheet1[i].Column10,
                                        logic_id:sheet1[i].Column11,
                                        created_date: new Date(),
                                        create_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        is_active: true

                                    }

                                    console.log(item)
                                    if (!item.manufacturer) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Manufacturer Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.model) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Model Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.series) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Series Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.vehicle_year) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Vehicle Year Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.month_start) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Month Start Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.month_end) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Month End Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                   
                                    if (!item.vehicle_type) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Vehicle Type Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.vendor) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Vendor Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.logic_id) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Logic Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                  

                                    $scope.data.push(item);
                                    console.log($scope.data)
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
                        MstService.insertPenalty($scope.data, function (reply) {
                            console.log(reply)
                            var word2 = [];
                            var word = [];

                            console.log(word);

                            word = reply.data.DataReturnCadangan.split(",");
                            word2 = reply.data.DataReturnCadangan2.split(",");
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];

                                    words = reply.data.DataReturn.split(",");


                                    UIControlService.msg_growl("error", "<u><i><b>" + words[i] + "</u></i></b>  Already In Database");
                                    console.log("wow")
                                }

                            }
                            if (word.length > 0) {
                                for (var c = 0; c < word.length ; c++) {
                                    if (word[c] == "") {
                                        console.log("do nothing")
                                    } else {
                                        UIControlService.msg_growl("error", "<u><i><b>" + word[c] + "</u></i></b>  Not Found In Database");
                                        console.log("wow")
                                    }

                                }
                            }
                            if (word2.length > 0) {
                                for (var c = 0; c < word2.length ; c++) {
                                    if (word2[c] == "") {
                                        console.log("do nothing")
                                    } else {
                                        UIControlService.msg_growl("error", "<u><i><b>" + word2[c] + "</u></i></b>  Not Found In Database");
                                        console.log("wow")
                                    }

                                }
                            }
                            if (reply.status == 200) {
                                //$interval(function () {
                                //    vm.jLoad(1);
                                //}, 5000);
                                vm.init();
                                UIControlService.msg_growl("success", "Upload Excel Success");
                            }
                            else {
                                UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR");
                            }
                        },
                            function (error) {
                                UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR")
                                UIControlService.unloadLoadingModal();
                            });
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.check = function () {
                     
                        for (var i = 0; i < $scope.data.length; i++) {
                            var item = {
                                vehicle_id: $scope.datavehspec[i].vehicle_specs_id,
                                zone_id: $scope.datazone[i].zone_id,
                                vehicleTypeID: $scope.dataveh[i].vehicle_type_id,
                                vehicle_year: $scope.data[i].vehicle_year,
                                pricelist: $scope.data[i].pricelist,
                                installmentHMS: $scope.data[i].installmentHMS,
                                installmentEMP: $scope.data[i].installmentEMP,
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true
                            }
                            $scope.semua.push(item)
                        }
                    }
                    $scope.off = 1;
                    $scope.lim = 5;
                    $scope.paging = function (param) {
                        $scope.totalRecords = ($scope.data.length)
                    }
                }
            });
        };

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            alasql.fn.datetime = function (dateStr) {
                var date = vm.dateFormat(new Date(parseInt(dateStr.substring(6, 19))));
                return date.toLocaleString();
            };
            alasql('SELECT manufacturer AS `Manufacturer`,model AS `Model`,series AS `Series`, vehicle_year AS `Vehicle Year`,month_start AS `Month Start`,month_end AS `Month End`,penalty1 AS `Penalty`, vehicle_type AS `Vehicle Type`, datetime(modified_date) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-penalty.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.delpricelist = delpricelist;
        function delpricelist(param) {
            UIControlService.loadLoading("loading");
            var data = {
                penalty_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delPenalty(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "InActive Success");
                    UIControlService.unloadLoading();
                    init();
                } else {
                    $.growl.error({ message: "Gagal insert data vehicle spects" });
                }
            })
        }


    }

})();