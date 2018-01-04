(function () {
    'use strict';

    angular.module("app").controller("MSTVhcSpCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
       'GlobalConstantService', '$state', '$stateParams', 'MstService',
   'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.dataVehicleS = [];
        vm.totalRecords = 0;
        vm.init = init;
        vm.roles = localStorage.getItem('roles');
        vm.manufacturer = "";
        vm.model = "";
        vm.series = "";
        vm.body_type = "";
        vm.color = "";
        vm.car_group_level = "";
        vm.image = "";
        vm.modified = "";
        vm.by = "";
        vm.aktif = "";
        vm.aktive;
        vm.off = 1;
        vm.dataexcel;
        vm.url; 
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
            vm.loadVehicleS();
        }

        vm.loadVehicleS = loadVehicleS;
        function loadVehicleS() {
            UIControlService.unloadLoading();
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.manufacturer + "|" + vm.model + "|" + vm.series + "|" + vm.body_type + "|" + vm.color + "|" + vm.car_group_level + "|" + vm.image + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.manufacturer + "|" + vm.model + "|" + vm.series + "|" + vm.body_type + "|" + vm.color + "|" + vm.car_group_level + "|" + vm.image + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetVehicleSpecsExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.SelectVehicleSpc(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataVehicleS = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses");
                    console.log(vm.dataVehicleS);
                    console.log(reply)
                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log("oke Gan");
        }
        vm.paging = paging;
        function paging(param) {
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.manufacturer + "|" + vm.model + "|" + vm.series + "|" + vm.body_type + "|" + vm.color + "|" + vm.car_group_level + "|" + vm.image + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.manufacturer + "|" + vm.model + "|" + vm.series + "|" + vm.body_type + "|" + vm.color + "|" + vm.car_group_level + "|" + vm.image + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetVehicleSpecsExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.SelectVehicleSpc(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataVehicleS = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses");
                    console.log(vm.dataVehicleS);
                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log(data)
        }

        vm.insVhcS = insVhcS;
        function insVhcS() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.manufacturer;
                    $scope.model;
                    $scope.series;
                    $scope.body_type;
                    $scope.color;
                    $scope.data = [];
                    $scope.car_group_level;
                    $scope.filevehicle;
                    $scope.image;
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.select = function (file) {
                        $scope.filevehicle = file;
                    };
                    $scope.insert = function () {
                        
                       console.log($scope.filevehicle)
                    
                        if ($scope.manufacturer == "" || $scope.manufacturer == undefined) {
                            UIControlService.msg_growl("error", " Manufacturer Must Be Fill");
                        } else if ($scope.model == "" || $scope.model == undefined) {
                            UIControlService.msg_growl("error", " Model Must Be Fill");
                        } else if ($scope.series == "" || $scope.series == undefined) {
                            UIControlService.msg_growl("error", " Series Must Be Fill");
                        } else if ($scope.body_type == "" || $scope.body_type == undefined) {
                            UIControlService.msg_growl("error", " Body Type Must Be Fill");
                        } else if ($scope.color == "" || $scope.color == undefined) {
                            UIControlService.msg_growl("error", " Color Must Be Fill");
                        } else if ($scope.filevehicle == "" || $scope.filevehicle == undefined) {
                            UIControlService.msg_growl("error", " Image Must Be Fill");
                        } else if ( $scope.car_group_level == undefined) {
                            UIControlService.msg_growl("error", " Car Group Level Must Be Fill");
                        } else {
                           
                            ExcelReaderService.upload($scope.filevehicle, function (reply) {
                                console.log(reply)
                                $scope.filename = reply.data.FileName;
                                var dat = {
                                    manufacturer: $scope.manufacturer,
                                    model: $scope.model,
                                    series: $scope.series,
                                    body_type: $scope.body_type,
                                    color: $scope.color,
                                    image: $scope.filename,
                                    car_group_level: $scope.car_group_level,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: localStorage.getItem('username'),
                                    modified_date: new Date(),
                                    is_active: true

                                }
                                $scope.data.push(dat);
                                MstService.insVhcS($scope.data, function (reply) {
                                    if (reply.data.Count > 0) {
                                        for (var i = 0; i < reply.data.Count; i++) {
                                            var words = [];
                                            words = reply.data.DataReturn.split(",");
                                            UIControlService.msg_growl("error", "Vendor <u><i><b>" + words[i] + "</u></i></b> Already In Database");
                                            $scope.data = [];
                                        }
                                    }else{
                                    if (reply.status === 200) {

                                        $uibModalInstance.dismiss('cancel');
                                        loadVehicleS();
                                        UIControlService.msg_growl("success", "Insert Success");
                                    } else {
                                        $.growl.error({ message: "Gagal insert data vehicle spects" });
                                    }
                                }
                                }, function (err) {
                                    console.info("error:" + JSON.stringify(err));
                                })
                            });
                        }
                    }                                                            
                }                            
            })
        }
        
        vm.delVhcS = delVhcS;
        function delVhcS(param) {
            UIControlService.loadLoading("loading");
            MstService.delVhcS({
                vehicleSpecsID: param,
                isActive: false,               
                ModifyDate: new Date(),
                LastDate: localStorage.getItem('username')
            }, function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", " InActive Success");
                    UIControlService.unloadLoading();
                    loadVehicleS();
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
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.manufacturer + "|" + vm.model + "|" + vm.series + "|" + vm.body_type + "|" + vm.color + "|" + vm.car_group_level + "|" + vm.image + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.manufacturer + "|" + vm.model + "|" + vm.series + "|" + vm.body_type + "|" + vm.color + "|" + vm.car_group_level + "|" + vm.image + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetVehicleSpecsExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.SelectVehicleSpc(data, function (reply) {
                vm.dataVehicleS = reply.data.List;
                vm.totalRecords = reply.data.Count;
            })
            console.log(data)
        }
        vm.uploadVhcS = uploadVhcS;
        function uploadVhcS() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadCAF.html',
                backdrop: 'static',
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
                                    UIControlService.msg_growl("error", "Must Be Manufacturer");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "Model") {
                                    UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "Must Be Model");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column4 != "Series") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "Must Be Series");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column5 != "Body Type") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "Must Be Body Type");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column6 != "Color") {
                                    UIControlService.msg_growl("error", "Column 6 Name Is " + sheet1[1].Column6);
                                    UIControlService.msg_growl("error", "Must Be Color");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column7 != "Car Group Level") {
                                    UIControlService.msg_growl("error", "Column 7 Name Is " + sheet1[1].Column7);
                                    UIControlService.msg_growl("error", "Must Be Car Group Level");
                                    $uibModalInstance.dismiss('cancel');
                                }


                                for (var i = 2; i < sheet1.length; i++) {
                                    var item = {
                                        manufacturer: sheet1[i].Column2,
                                        model: sheet1[i].Column3,
                                        series: sheet1[i].Column4,
                                        body_type: sheet1[i].Column5,
                                        color: sheet1[i].Column6,
                                        car_group_level: sheet1[i].Column7,
                                        type: "upload",
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: localStorage.getItem('username'),
                                        modified_by: new Date(),
                                        is_active: true

                                    };
                                    console.log(item)
                                    if (!item.manufacturer) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Manufacturer Must be Fill ");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.model) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Model Must be Fill ");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.series) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Series Must be Fill ");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.body_type) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "body Type Must be Fill ");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.color) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Color Must be Fill ");
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
                        MstService.insVhcS($scope.data, function (reply) {
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];
                                    words = reply.data.DataReturn.split(",");
                                    UIControlService.msg_growl("error", "Vehicle Specs <u><i><b>" + words[i] + "</u></i></b> Already In Database");
                                    $scope.data = [];
                                }
                            }
                            if (reply.status == 200) {
                                //$interval(function () {
                                //    vm.jLoad(1);
                                //}, 5000);
                                UIControlService.msg_growl("success", " Upload Excel Success");
                                vm.init();
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
                }
            });
        };

        vm.toExcel = toExcel;
        function toExcel(table) {
            alasql.fn.datetime = function (dateStr) {
                var date = vm.dateFormat(new Date(parseInt(dateStr.substring(6, 19))));
                return date.toLocaleString();
            };
            alasql('SELECT manufacturer As `Manufacturer`, model AS `Model`, series As `Series`, body_type AS `Body Type`, color As `Color`, car_group_level AS `Car Group Level`,image AS `Image`, datetime(modified_date) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-vhcspec.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }


        vm.updVhcS = updVhcS;
        function updVhcS(param){
            var modalInstance = $uibModal.open({
                templateUrl: 'updVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {                    
                    $scope.manufacturer2;
                    $scope.model2;
                    $scope.series2;
                    $scope.body_type2;
                    $scope.color2;
                    $scope.image;
                    $scope.car_group_level2;
                    $scope.filevehicle;
                    $scope.check;
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetVhcSByID(param, function (reply) {
                        $scope.manufacturer2 =reply.data.manufacturer;
                        $scope.model2 = reply.data.model;
                        $scope.series2=reply.data.series;
                        $scope.body_type2=reply.data.body_type;
                        $scope.color2=reply.data.color;
                        $scope.image=reply.data.image;
                        $scope.car_group_level2 = reply.data.car_group_level;
                        $scope.check = reply.data.is_active;
                    })
                    $scope.select = function (file) {
                        $scope.filevehicle = file;
                    };
                    $scope.update = function () {
                
             
                        if ($scope.manufacturer2 == "" || $scope.manufacturer2 == undefined) {
                            UIControlService.msg_growl("error", " Manufacturer Must Be Fill");
                        } else if ($scope.model2 == "" || $scope.model2 == undefined) {
                            UIControlService.msg_growl("error", " Model Must Be Fill");
                        } else if ($scope.series2 == "" || $scope.series2 == undefined) {
                            UIControlService.msg_growl("error", " Series Must Be Fill");
                        } else if ($scope.body_type2 == "" || $scope.body_type2 == undefined) {
                            UIControlService.msg_growl("error", " Body Type Must Be Fill");
                        } else if ($scope.color2 == "" || $scope.color2 == undefined) {
                            UIControlService.msg_growl("error", " Color Must Be Fill");
                        } else if ($scope.filevehicle == "" || $scope.filevehicle == undefined) {
                            UIControlService.msg_growl("error", " Image Must Be Fill");
                        } else if ($scope.car_group_level2 == undefined) {
                            UIControlService.msg_growl("error", " Car Group Level Must Be Fill");
                        } else {
                            ExcelReaderService.upload($scope.filevehicle, function (reply) {
                                console.log(reply)
                                $scope.filename = reply.data.FileName;
                                if ($('.tes').is(':checked') || $scope.check == 1) {
                                    var data = {
                                        vehicle_specs_id: param,
                                        manufacturer: $scope.manufacturer2,
                                        model: $scope.model2,
                                        series: $scope.series2,
                                        body_type: $scope.body_type2,
                                        color: $scope.color2,
                                        image: $scope.filename,
                                        car_group_level: $scope.car_group_level2,
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: localStorage.getItem('username'),
                                        modified_date: new Date(),
                                        is_active: true

                                    }
                                } else {
                                    var data = {
                                        vehicle_specs_id: param,
                                        manufacturer: $scope.manufacturer2,
                                        model: $scope.model2,
                                        series: $scope.series2,
                                        body_type: $scope.body_type2,
                                        color: $scope.color2,
                                        image: $scope.filename,
                                        car_group_level: $scope.car_group_level2,
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: localStorage.getItem('username'),
                                        modified_date: new Date(),
                                        is_active: false

                                    }
                                }

                                MstService.updVhcS(data, function (reply) {
                                    if (reply.status === 200) {

                                        UIControlService.msg_growl("success", " Update Success");
                                        $uibModalInstance.dismiss('cancel');
                                        loadVehicleS()
                                    } else {
                                        $.growl.error({ message: "Gagal insert data vehicle spects" });
                                    }
                                }, function (err) {
                                    console.info("error:" + JSON.stringify(err));
                                    UIControlService.unloadLoading();
                                })
                            });

                        }
                    }
                    
                    
                }
            })
        }
    }

})();