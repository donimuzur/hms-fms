(function () {
    'use strict';

    angular.module("app").controller("MSTVJCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'MstService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval,DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.dataVenjob = [];
        vm.totalRecords = 0;
        vm.roles = localStorage.getItem('roles');
      
        vm.vendor = "";
        vm.zone = "";
        vm.year = "";
        vm.vhty = "";
        vm.vhus = "";
        vm.modified = "";
        vm.by = "";
        vm.off = 1;
        vm.nalias = "";
        vm.dataexcel;
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
        vm.init = init;
        function init() {
            UIControlService.loadLoading("loading");
            MstService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                console.info(vm.id_coordinator);
                console.log('coba')
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.loadVenjob();
        }

        vm.loadVenjob = loadVenjob;
        function loadVenjob() {
            UIControlService.unloadLoading();
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
           
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.vendor + "|" + vm.nalias + "|" + vm.zone + "|" + vm.vhty + "|" + vm.vhus + "|" + vm.year + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.vendor + "|" + vm.nalias + "|" + vm.zone + "|" + vm.vhty + "|" + vm.vhus + "|" + vm.year + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetVenjobExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetVenjob(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataVenjob = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("berhasil Gan")
                    console.log(vm.dataVenjob)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };

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
        vm.paging = paging;
        function paging(param) {
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (param * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.vendor + "|" + vm.nalias + "|" + vm.zone + "|" + vm.vhty + "|" + vm.vhus + "|" + vm.year + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.vendor + "|" + vm.nalias + "|" + vm.zone + "|" + vm.vhty + "|" + vm.vhus + "|" + vm.year + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetVenjobExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetVenjob(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataVenjob = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("berhasil Gan")
                    console.log(vm.dataVenjob)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
        }

        vm.updVenjob = updVenjob;
        function updVenjob(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updVen.html',
                backdrop: 'static',
                resolve: {
                    test: function () {
                        return param;
                    }
                },
                controller: function ($uibModalInstance, $scope,test) {
                    $scope.vendor_id = test.vendor_id;
                    $scope.vehtype = test.vehicle_type;
                    $scope.zones = test.zone_id;
                    $scope.check = test.is_active;
                    $scope.short_name = test.nameAlias;
                    $scope.vendor2 = test.vendor2;
                    $scope.zone_na = test.zone;
                    $scope.year = test.year;
                
                    $scope.Vname;
                    $scope.Sname = test.vendor_id;
                    $scope.Zname= test.zone_id;
                    $scope.years;
                    $scope.vcty;
                    $scope.vcus;
                    $scope.data = [];
                    $scope.vendor = [];
                    $scope.datazone = [];
                    $scope.datavhct = [];
                  
                    $scope.vendor;
                   
                   
                    $scope.Zid;
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                   
                   
                   
                    MstService.GetVendorjob(function (reply) {
                        $scope.vendor = reply.data;
                        $scope.cekvendor($scope.vendor2)
                        console.log(reply)
                    })
                    MstService.GetZonePri(function (respon) {
                        $scope.datazone = respon.data;
                        console.log($scope.datazone)
                    })
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetVhcTPri(function (responnya) {
                        $scope.datavhct = responnya.data;
                     
                     
                        console.log($scope.datavhct);
                    })
                    $scope.cekvendor = function (param) {
                        MstService.cekvendor(param, function (response) {
                            $scope.hasilVen = response.data;
                            //$scope.Sname = response.data[0].name_alias;
                            console.log(response)
                            console.log($scope.Sname)
                            console.log($scope.hasilVen)
                        })
                    }
                   
                    $scope.cekvehty = function (param) {
                        MstService.cekvehty(param, function (respon) {
                            $scope.cekve = respon.data;

                            console.log(respon)
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        })
                    }

                    $scope.update = function () {
                        var data;
                        if ($scope.vendor2 == "" || $scope.vendor2 == undefined) {
                            UIControlService.msg_growl("error", " Vendor Name Must be Fill");
                        } else if ($scope.Sname == "" || $scope.Sname == undefined) {
                            UIControlService.msg_growl("error", " Short Name Must be Fill")
                        } else if ($scope.Zname == "" || $scope.Zname == undefined) {
                            UIControlService.msg_growl("error", " Zone Name Must be Fill");
                        } else if ($scope.year == "" || $scope.year == undefined) {
                            UIControlService.msg_growl("error", " Years Must be Fill");
                        } else if ($scope.vehtype == "" || $scope.vehtype == undefined) {
                            UIControlService.msg_growl("error", " Vehicle Type Must be Fill");
                        }  else {
                            if ($('.tes').is(':checked') || $scope.check == 1) {
                                data = {
                                    vendor_area_id: test.vendor_area_id,
                                    vendor_id: $scope.Sname,
                                    zone_id: $scope.Zname,
                                    year: $scope.year,
                                    vehicleTypeID: $scope.vehtype,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                            } else {
                                data = {
                                    vendor_area_id: test.vendor_area_id,
                                    vendor_id: $scope.Sname,
                                    zone_id: $scope.Zname,
                                    year: $scope.year,
                                    vehicleTypeID: $scope.vehtype,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: false

                                }
                            }
                            
                            MstService.updVenjob(data, function (reply) {
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", " Update Success");
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

        vm.delVenjob = delVenjob;
        function delVenjob(param) {

            UIControlService.loadLoading("loading");
            var data = {
                vendor_area_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delVenjob(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", " InActive Success");
                    UIControlService.unloadLoading();
                    init();
                } else {
                    $.growl.error({ message: "Gagal Delete Data" });
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
                    $scope.cekvehty = function (param) {
                        MstService.cekvehty(param, function (respon) {
                            $scope.cekve = respon.data;

                            console.log(respon)
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        })
                    }
                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file, function (reply) {
                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];
                                console.info(sheet1);
                                if (sheet1[1].Column2 != "Vendor Name") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "Must be Vendor Name");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "Short Name") {
                                    UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "Must be Short Name");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column4 != "Zone Name") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "Must be Zone Name");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column5 != "Year") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "Must be Year");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column6 != "Vehicle Type") {
                                    UIControlService.msg_growl("error", "Column 6 Name Is " + sheet1[1].Column6);
                                    UIControlService.msg_growl("error", "Must be Vehicle Type");
                                    $uibModalInstance.dismiss('cancel');
                                }
                               

                                for (var i = 2; i < sheet1.length; i++) {
                                    var vendor_name = sheet1[i].Column2,
                                        short_name = sheet1[i].Column3,
                                     zone_name = sheet1[i].Column4,
                                    vehicle_type = sheet1[i].Column6,
                                    vehicle_usage = sheet1[i].Column7;

                          
                                 
                                    var item = {
                                       
                                        Vendor_Name: sheet1[i].Column2,
                                        Name_alias: sheet1[i].Column3,
                                        year: sheet1[i].Column5,
                                        Zone_Name: sheet1[i].Column4,
                                        vehicleTypeID: sheet1[i].Column6,
                                        type: "upload",
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        is_active: true

                                    };
                                    console.log(item)
                                    if (!item.Vendor_Name) {
                                        UIControlService.msg_growl("error", "Item Line " + i + " Not valid:");
                                        UIControlService.msg_growl("error", "Vendor Name Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.Name_alias) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Short Name Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.year) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Short Name Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.Zone_Name) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Zone Name Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.vehicleTypeID) {
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
                        
                        
                        MstService.insVenjob($scope.data, function (reply) {
                            var word = [];
                            var wordkoma = [];
                            if (reply.data == "") {
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", " Insert Success");
                                } else {
                                    $.growl.error({ message: "Gagal insert data vehicle spects" });
                                }
                            } else {
                                word = reply.data.DataReturn.split("|");
                                for (var i = 0; i < word.length ; i++) {
                                    if (word[i] == "") {
                                        console.log("tes aja bos")
                                    } else {
                                        wordkoma = word[i].split(",");
                                        for (var i = 0; i < wordkoma.length ; i++) {
                                            UIControlService.msg_growl("error", "<u><i><b>" + wordkoma[i] + "</u></i></b>  Already In Database");
                                        }
                                       
                                        $scope.data = [];
                                        console.log("coba aja")
                                    }

                                }
                                console.log(word)
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
                    //$scope.check = function () {
                    //    console.log($scope.datazone);
                    //    console.log($scope.data);
                    //    console.log($scope.dataveh);
                    //    console.log($scope.datavendor);
                    //    for (var i = 0; i < $scope.data.length; i++) {
                    //        var item = {
                    //            vendor_id: $scope.datavendor[i].vendor_id,
                    //            zone_id: $scope.datazone[i].zone_id,
                    //            vehicleTypeID: $scope.data[i].vehicle_type,
                    //            year:$scope.data[i].year,
                    //            created_date: new Date(),
                    //            created_by: localStorage.getItem('username'),
                    //            last_modified: new Date(),
                    //            modified_by: localStorage.getItem('username'),
                    //            is_active: true
                    //        }
                    //        $scope.semua.push(item)
                    //    }
                    //}
                    $scope.off = 1;
                    $scope.lim = 5;
                    $scope.paging = function (param) {
                        $scope.totalRecords =($scope.data.length)
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
            alasql('SELECT vendor2 AS `Vendor Name`,nameAlias AS `Name Alias`, zone AS `Zone Name`, year AS `Year`, vehicle_type AS `Vehicle type`,datetime(modified_date) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-venjob.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.insVenjob = insVenjob;
        function insVenjob() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVen.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.Vname;
                    $scope.Sname;
                    $scope.Zname;
                    $scope.years;
                    $scope.vcty;
                    $scope.vcus;
                    $scope.data = [];
                    $scope.vendor = [];
                    $scope.datazone = [];
                    $scope.datavhct = [];
                    MstService.GetVendorjob(function (reply) {
                        $scope.vendor = reply.data;
                        console.log(reply)
                    })
                    MstService.GetZonePri(function (respon) {
                        $scope.datazone = respon.data;
                        console.log($scope.datazone)
                    })
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetVhcTPri(function (responnya) {
                        $scope.datavhct = responnya.data;
                        console.log($scope.datavhct);
                    })
                    $scope.cekvendor = function (param) {
                        MstService.cekvendor(param, function (response) {
                            $scope.hasilVen = response.data;
                            console.log($scope.hasilVen)
                        })
                    }
                    $scope.cekvehty = function (param) {
                        MstService.cekvehty(param, function (respon) {
                            $scope.cekve = respon.data;
                            
                            console.log(respon)
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        })
                    }
                    $scope.insert = function () {

                        if ($scope.Vname == "" || $scope.Vname == undefined) {
                            UIControlService.msg_growl("error", " Vendor Name Must be Fill");
                        } else if ($scope.Sname== "" || $scope.Sname== undefined) {
                            UIControlService.msg_growl("error", " Short Name Must be Fill")
                        } else if ($scope.Zname== "" || $scope.Zname== undefined) {
                            UIControlService.msg_growl("error", " Zone Name Must be Fill");
                        } else if ($scope.years == "" || $scope.years == undefined) {
                            UIControlService.msg_growl("error", " Years Must be Fill");
                        } else if ($scope.vcty == "" || $scope.vcty== undefined) {
                            UIControlService.msg_growl("error", " Vehicle Type Must be Fill");
                        } 
                        else {
                            var dat = {
                                Vendor_Name: $scope.Vname,
                                Name_alias:$scope.Sname,
                                Zone_Name: $scope.Zname,
                                year: $scope.years,
                                vehicleTypeID: $scope.vcty,
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true
                            }
                            $scope.data.push(dat)
                            MstService.insVenjob($scope.data, function (reply) {
                                console.log(reply)
                                var word = [];
                                if (reply.data == "") {
                                    if (reply.status === 200) {
                                        $uibModalInstance.dismiss('cancel');
                                        init();
                                        UIControlService.msg_growl("success", " Insert Success");
                                    } else {
                                        $.growl.error({ message: "Gagal insert data vehicle spects" });
                                    }
                                } else {
                                    word = reply.data.DataReturn.split("|");
                                    for (var i = 0; i < word.length ; i++) {
                                        if (word[i] == "") {
                                            console.log("tes aja bos")
                                        } else {
                                            UIControlService.msg_growl("error", "<u><i><b>" + word[i] + "</u></i></b>  Already In Database");
                                            $scope.data = [];
                                            console.log("coba aja")
                                        }
                                       
                                    }
                                    console.log(word)
                                }
                             
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                    }
                }
            });
        }
    }
})();