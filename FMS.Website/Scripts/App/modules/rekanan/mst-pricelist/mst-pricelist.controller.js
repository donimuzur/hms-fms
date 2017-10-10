(function () {
    'use strict';

    angular.module("app").controller("MSTPLCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'MstService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService,$timeout,$interval,DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.datapricelist = [];
        vm.totalRecords = 0;
        vm.roles = localStorage.getItem('roles');
        vm.man = "";
        vm.mod = "";
        vm.ser = "";
        vm.col = "";
        vm.zone = "";
        vm.vehyear = "";
        vm.price = "";
        vm.inshms = "";
        vm.insemp = "";
        vm.vehtype = "";
        vm.last = "";
        vm.modif = "";
        vm.off = 1;
        vm.vehusage = "";
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
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            var rep = vm.ser.replace(".", "~2b");
            var repl = rep.replace("/", "~2a");
            if (vm.format(vm.last) == "NaN-undefined-NaN") {
                var data = vm.vehyear + "|" + vm.last + "|" + vm.modif + "|" + vm.price + "|" + vm.inshms + "|" + vm.insemp + "|" + vm.man + "|" + vm.mod + "|" + repl + "|" + vm.col + "|" + vm.vehtype + "|" + vm.vehusage + "|" + vm.zone + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.vehyear + "|" + vm.format(vm.last) + "|" + vm.modif + "|" + vm.price + "|" + vm.inshms + "|" + vm.insemp + "|" + vm.man + "|" + vm.mod + "|" + repl + "|" + vm.col + "|" + vm.vehtype + "|" + vm.vehusage + "|" + vm.zone + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetpricelistExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.Getpricelist(data, function (reply) {
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
                    $scope.vcty;
                    $scope.vcus;
                    $scope.loc;
                    $scope.price;
                    $scope.ihms;
                    $scope.iemp;
                    $scope.vcy;
                    $scope.thn;
                    $scope.tha;
                    $scope.location = [];
                    $scope.address = [];
                    $scope.datavhct = [];
                    $scope.vuz;
                    $scope.vehspec = [];
                    $scope.datamodel = [];
                    $scope.mod;
                    $scope.series;
                    $scope.datamanufacturer = [];
                    $scope.datasem = [];
                    $scope.color;
                    $scope.dataseries = [];
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetZonePri(function (respon) {
                        $scope.location = respon.data;
                        console.log($scope.location)
                        console.log(respon)
                    })



                    MstService.GetVhcTPri(function (responnya) {
                        $scope.datavhct = responnya.data;
                        console.log($scope.datavhct);
                    })
                    $scope.cekvehty = function (param) {
                        MstService.cekvehty(param, function (respon) {
                            $scope.cekve = respon.data;
                            if (param == "WTC") {
                                $scope.vuz = respon.data[0].vehicle_type_id;

                            }
                            console.log($scope.vuz)
                            console.log(respon)
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        })
                    }

                    MstService.VehicleSpecsByManufacturer(function (resp) {
                        $scope.vehspec = resp.data;

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
                    $scope.cekseries = function (param) {
                        var rep = param.replace(".", "|");
                        var repl = rep.replace("/", "~2a");
                        MstService.cekseries(repl, function (re) {
                            $scope.dataseries = re.data;
                            console.log($scope.dataseries)
                        })
                    }
                    $scope.insert = function () {
                        console.log($scope.vuz)
                        if ($scope.vcus == "" || $scope.vcus == undefined) {
                            UIControlService.msg_growl("error", " Manufacturer Must Be Fill");
                        } else if ($scope.mod == "" || $scope.mod == undefined) {
                            UIControlService.msg_growl("error", " Model Must Be Fill");
                        } else if ($scope.series == "" || $scope.series == undefined) {
                            UIControlService.msg_growl("error", " Series Must Be Fill");
                        } else if ($scope.color == "" || $scope.color == undefined) {
                            UIControlService.msg_growl("error", " Color Must Be Fill");
                        } else if ($scope.thn == "" || $scope.thn == undefined) {
                            UIControlService.msg_growl("error", " Zone Must Be Fill");
                        } else if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", " Vehicle Type Must Be Fill");
                        } else if ($scope.vuz == "" || $scope.vuz == undefined) {
                            UIControlService.msg_growl("error", " Vehicle Usage Must Be Fill");
                        }
                        else if ($scope.vcy == "" || $scope.vcy == undefined) {
                            UIControlService.msg_growl("error", " Vehicle Year Must Be Fill");
                        } else if ($scope.price == "" || $scope.price == undefined) {
                            UIControlService.msg_growl("error", " Price Must Be Fill");
                        } else if ($scope.ihms == "" || $scope.ihms == undefined) {
                            UIControlService.msg_growl("error", " Installment HMS Must Be Fill");
                        } else {

                            var data = {
                                vehicle_id: $scope.color,
                                zone_id: $scope.thn,
                                vehicle_year: $scope.vcy,
                                vehicleTypeID: $scope.vuz,
                                pricelist: $scope.price,
                                installmentHMS: $scope.ihms,
                                installmentEMP: $scope.iemp,
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true

                            }
                            console.log(data)
                            $scope.datasem.push(data);
                            MstService.inspricelist($scope.datasem, function (reply) {
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", "Insert Success");
                                } else {
                                    $.growl.error({ message: "Gagal insert data vehicle spects" });
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
                var data = vm.vehyear + "|" + vm.last + "|" + vm.modif + "|" + vm.price + "|" + vm.inshms + "|" + vm.insemp + "|" + vm.man + "|" + vm.mod + "|" + repl + "|" + vm.col + "|" + vm.vehtype + "|" + vm.vehusage + "|" + vm.zone + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.vehyear + "|" + vm.format(vm.last) + "|" + vm.modif + "|" + vm.price + "|" + vm.inshms + "|" + vm.insemp + "|" + vm.man + "|" + vm.mod + "|" + repl + "|" + vm.col + "|" + vm.vehtype + "|" + vm.vehusage + "|" + vm.zone + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetpricelistExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.Getpricelist(data, function (reply) {
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
            var offset = (vm.off * 10) - 10;
            var rep = vm.ser.replace(".", "~2b");
            var repl = rep.replace("/", "~2a");
            if (vm.format(vm.last) == "NaN-undefined-NaN") {
                var data = vm.vehyear + "|" + vm.last + "|" + vm.modif + "|" + vm.price + "|" + vm.inshms + "|" + vm.insemp + "|" + vm.man + "|" + vm.mod + "|" + repl + "|" + vm.col + "|" + vm.vehtype + "|" + vm.vehusage + "|" + vm.zone + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.vehyear + "|" + vm.format(vm.last) + "|" + vm.modif + "|" + vm.price + "|" + vm.inshms + "|" + vm.insemp + "|" + vm.man + "|" + vm.mod + "|" + repl + "|" + vm.col + "|" + vm.vehtype + "|" + vm.vehusage + "|" + vm.zone + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.Getpricelist(data, function (reply) {
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
                    $scope.vcty = test.vehicle_type;
                    $scope.vcus = test.manufacturer;
                    $scope.specs_id = test.vehicle_id;
                    $scope.Zname = test.zone_id;
                    $scope.type_id = test.vehicle_type_id;
                    $scope.loc;
                    $scope.price = test.price;
                    $scope.ihms = test.installment_hms;
                    $scope.iemp = test.installment_emp;
                    $scope.vcy = test.vehicle_year;
                    $scope.thn = test.zone_name;
                    $scope.tha;
                    $scope.location = [];
                    $scope.address = [];
                    $scope.datavhct = [];
                    $scope.vuz = test.vehicle_usage;
                    $scope.vehspec = [];
                    $scope.datamodel = [];
                    $scope.mod = test.model;
                    $scope.series = test.series;
                    $scope.datamanufacturer = [];
                    $scope.datasem = [];
                    $scope.color = test.color;
                    $scope.dataseries = [];
                    $scope.check = test.is_active;
                    $scope.vehid;
                    $scope.vehtyid;
                    $scope.zoid;
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetZonePri(function (respon) {
                        $scope.location = respon.data;
                        $scope.cekvehty($scope.vcty);
                        $scope.cekseries($scope.series);
                        $scope.cekmodel($scope.mod);
                        $scope.cekmanufacturer($scope.vcus);
                        console.log($scope.location)
                        console.log(respon)
                    })
                    MstService.GetVhcTPri(function (responnya) {
                        $scope.datavhct = responnya.data;
                        console.log($scope.datavhct);
                    })
                    $scope.cekvehty = function (param) {
                        MstService.cekvehty(param, function (respon) {
                            $scope.cekve = respon.data;
                            if (param == "WTC") {
                                $scope.vuz = respon.data[0].vehicle_type_id;

                            }
                            console.log(respon)
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        })
                    }

                    MstService.VehicleSpecsByManufacturer(function (resp) {
                        $scope.vehspec = resp.data;

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
                    $scope.cekseries = function (param) {
                        var rep = param.replace(".", "|");
                        var repl = rep.replace("/", "~2a");
                        MstService.cekseries(repl, function (re) {
                            $scope.dataseries = re.data;
                            console.log($scope.dataseries)
                        })
                    }



                    $scope.update = function () {
                        var data;
                        if ($scope.vcus == "" || $scope.vcus == undefined) {
                            UIControlService.msg_growl("error", " Manufacturer Must Be Fill");
                        } else if ($scope.mod == "" || $scope.mod == undefined) {
                            UIControlService.msg_growl("error", " Model Must Be Fill");
                        } else if ($scope.series == "" || $scope.series == undefined) {
                            UIControlService.msg_growl("error", " Series Must Be Fill");
                        } else if ($scope.color == "" || $scope.color == undefined) {
                            UIControlService.msg_growl("error", " Color Must Be Fill");
                        } else if ($scope.thn == "" || $scope.thn == undefined) {
                            UIControlService.msg_growl("error", " Zone Must Be Fill");
                        } else if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", " Vehicle Type Must Be Fill");
                        } else if ($scope.vuz == "" || $scope.vuz == undefined) {
                            UIControlService.msg_growl("error", " Vehicle Usage Must Be Fill");
                        } else if ($scope.vcy == "" || $scope.vcy == undefined) {
                            UIControlService.msg_growl("error", " Vehicle Year Must Be Fill");
                        } else if ($scope.price == "" || $scope.price == undefined) {
                            UIControlService.msg_growl("error", " Price Must Be Fill");
                        } else if ($scope.ihms == "" || $scope.ihms == undefined) {
                            UIControlService.msg_growl("error", " Installment HMS Must Be Fill");
                        } else {
                            if ($('.tes').is(':checked') || $scope.check == 1) {
                                data = {
                                    price_list_id: test.pricelist_id,
                                    vehicle_id: $scope.specs_id,
                                    zone_id: $scope.Zname,
                                    vehicle_year: $scope.vcy,
                                    vehicleTypeID: $scope.type_id,
                                    pricelist: $scope.price,
                                    installmentHMS: $scope.ihms,
                                    installmentEMP: $scope.iemp,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    type: "upload",
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                            } else {
                                data = {
                                    price_list_id: test.pricelist_id,
                                    vehicle_id: $scope.specs_id,
                                    zone_id: $scope.Zname,
                                    vehicle_year: $scope.vcy,
                                    vehicleTypeID: $scope.type_id,
                                    pricelist: $scope.price,
                                    installmentHMS: $scope.ihms,
                                    installmentEMP: $scope.iemp,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: false

                                }
                            }

                            MstService.updpricelist(data, function (reply) {
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", "Update Success");
                                } else {
                                    $.growl.error({ message: "Gagal insert data vehicle spects" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }

                    }


                }
            })
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
                                if (sheet1[1].Column5 != "Color") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "Must be Color");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column6 != "Zone") {
                                    UIControlService.msg_growl("error", "Column 6 Name Is " + sheet1[1].Column6);
                                    UIControlService.msg_growl("error", "Must be Zone");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column7 != "Vehicle Year") {
                                    UIControlService.msg_growl("error", "Column 7 Name Is " + sheet1[1].Column7);
                                    UIControlService.msg_growl("error", "Must be Vehicle Year");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column8 != "Price") {
                                    UIControlService.msg_growl("error", "Column 8 Name Is " + sheet1[1].Column8);
                                    UIControlService.msg_growl("error", "Must be Price");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column9 != "Installment HMS") {
                                    UIControlService.msg_growl("error", "Column 9 Name Is " + sheet1[1].Column9);
                                    UIControlService.msg_growl("error", "Must be Installment HMS");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column10 != "Installment EMP") {
                                    UIControlService.msg_growl("error", "Column 10 Name Is " + sheet1[1].Column10);
                                    UIControlService.msg_growl("error", "Must be Installment EMP");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column11 != "Vehicle Type") {
                                    UIControlService.msg_growl("error", "Column 11 Name Is " + sheet1[1].Column11);
                                    UIControlService.msg_growl("error", "Must be Vehicle Type");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column12 != "Vehicle Usage") {
                                    UIControlService.msg_growl("error", "Column 12 Name Is " + sheet1[1].Column12);
                                    UIControlService.msg_growl("error", "Must be Vehicle Usage");
                                    $uibModalInstance.dismiss('cancel');
                                }

                                for (var i = 2; i < sheet1.length; i++) {
                               
                                    var manufacturer = sheet1[i].Column2,
                                        model = sheet1[i].Column3,
                                     series = sheet1[i].Column4,
                                    color = sheet1[i].Column5,
                                    zone = sheet1[i].Column6,
                                    vehicle_year = sheet1[i].Column7,
                                    price = sheet1[i].Column8,
                                    ihms = sheet1[i].Column9,
                                    iemp = sheet1[i].Column10,
                                    vehicle_type = sheet1[i].Column11,
                                    vehicle_usage = sheet1[i].Column12;

                                    if (vehicle_type == "WTC") {
                                        MstService.GetMsdVehWtc({ vehicle_type: vehicle_type }, function (resp) {
                                            $scope.dataveh.push(resp.data)
                                            console.log(resp)
                                        })
                                    } else {
                                        MstService.GetMSDVehID({ vehicle_type: vehicle_type, vehicle_usage: vehicle_usage }, function (response) {

                                            $scope.dataveh.push(response.data);
                                            console.log(response)


                                        }, function (err) {

                                            UIControlService.unloadLoading();
                                            $scope.data = [];
                                            
                                            UIControlService.msg_growl("error", "Pricelist Excel Column " + vehicle_type + "/"+vehicle_usage+" Undefined");
                                        })
                                    }

                                   

                                    MstService.GetZoneID(zone, function (resp) {

                                        if (resp.data.length == 0) {
                                            $scope.data = [];
                                            UIControlService.msg_growl("error", "Pricelist Excel Column " + zone + " Undefined");
                                        } else {
                                            $scope.datazone.push(resp.data)
                                            console.log(resp)
                                        }
                                      

                                    }, function (err) {
                                      console.log("coba lah")
                                        UIControlService.unloadLoading();
                                        $scope.data = [];
                                       
                                    })


                                    MstService.GetFourRole({ manufacturer: manufacturer, model: model, series: series, color: color }, function (ress) {
                                        console.log(ress)
                                        //if (ress.data.length > 1) {

                                        //    UIControlService.msg_growl("error", "Upload Excel Error");
                                        //    $scope.data = [];
                                        //    UIControlService.unloadLoadingModal();

                                        //} else {
                                        //    console.log("berhasil")
                                        //}
                                        if (ress.data.length == 0) {
                                            $scope.data = [];
                                            UIControlService.msg_growl("error", "Pricelist Excel Column " + manufacturer + " OR " + model + " OR " + series + " OR " + color + " Undefined");
                                        } else {
                                            $scope.datavehspec.push(ress.data[0]);
                                        }
                                       
                                    }, function (err) {
                                        console.log("COBA LAGI")
                                        UIControlService.unloadLoading();
                                       
                                    })

                                    var item = {
                                      
                                        manufacturer: sheet1[i].Column2,
                                        model: sheet1[i].Column3,
                                        series: sheet1[i].Column4,
                                        color: sheet1[i].Column5,
                                        zone: sheet1[i].Column6,
                                        vehicle_year: sheet1[i].Column7,
                                        pricelist: sheet1[i].Column8,
                                        installmentHMS: sheet1[i].Column9,
                                        installmentEMP: sheet1[i].Column10,
                                        vehicle_type: sheet1[i].Column11,
                                        vehicle_usage: sheet1[i].Column12,
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
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
                                    if (!item.color) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Color Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.zone) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Zone Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.vehicle_year) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Vehicle Year Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.pricelist) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Price Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.installmentHMS) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Installment HMS Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                   
                                    if (!item.installmentEMP) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Installment EMP Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.vehicle_type) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Vehicle Type Must be Fill");
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
                        $scope.check();

                        MstService.inspricelist($scope.semua, function (reply) {
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
                        console.log($scope.datazone);
                        console.log($scope.data);
                        console.log($scope.dataveh);
                        console.log($scope.datavehspec);
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
            alasql('SELECT manufacturer AS `Manufacturer`,model AS `Model`,series AS `Series`, color AS `Color`,zone_name AS `Zone Name`,vehicle_year AS `Vehicle Year`,price AS `Price`, installment_hms AS `Installment HMS` , installment_emp AS `Installment Employee`, vehicle_type AS `Vehicle Type`, vehicle_usage AS `Vehicle Usage`, datetime(last_modified) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-pricelist.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.delpricelist = delpricelist;
        function delpricelist(param) {
            UIControlService.loadLoading("loading");
            var data = {
                price_list_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delpricelist(data, function (reply) {
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