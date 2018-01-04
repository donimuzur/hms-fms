(function () {
    'use strict';

    angular.module("app").controller("MSTZMCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'MstService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.dataZoneMap = [];
        vm.totalRecords = 0;
      
        vm.Zna = "";
        vm.cit = "";
        vm.ads = "";
        vm.eff = "";
        vm.lasmod = "";
        vm.modby = "";
        vm.off = 1;
        vm.roles = localStorage.getItem('roles');
      
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
            vm.loadZoneMap();
        }
        vm.dataexcel;
        vm.loadZoneMap = loadZoneMap;
        function loadZoneMap() {
            UIControlService.unloadLoading();
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            
            if (vm.format(vm.lasmod) == "NaN-undefined-NaN") {
                var data = vm.eff + "|" + vm.Zna + "|" + vm.cit + "|" + vm.ads + "|" + vm.lasmod + "|" + vm.modby + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.eff + "|" + vm.Zna + "|" + vm.cit + "|" + vm.ads + "|" + vm.format(vm.lasmod) + "|" + vm.modby + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetZonemapExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetZoneMap(data,function (reply) {
                if (reply.status === 200) {
                    vm.dataZoneMap = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataZoneMap)

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log("oke Gan");
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
            if (vm.format(vm.lasmod) == "NaN-undefined-NaN") {
                var data = vm.eff + "|" + vm.Zna + "|" + vm.cit + "|" + vm.ads + "|" +vm.lasmod + "|" + vm.modby + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.eff + "|" + vm.Zna + "|" + vm.cit + "|" + vm.ads + "|" + vm.format(vm.lasmod) + "|" + vm.modby + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetZonemapExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetZoneMap(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataZoneMap = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataZoneMap)

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log(data)
        }
        vm.paging = paging;
        function paging(param) {
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;

            if (vm.format(vm.lasmod) == "NaN-undefined-NaN") {
                var data = vm.eff + "|" + vm.Zna + "|" + vm.cit + "|" + vm.ads + "|" + vm.lasmod + "|" + vm.modby + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.eff + "|" + vm.Zna + "|" + vm.cit + "|" + vm.ads + "|" + vm.format(vm.lasmod) + "|" + vm.modby + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetZoneMap(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataZoneMap = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataZoneMap)

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log(data)
        }
        vm.insZoneMap = insZoneMap;
        function insZoneMap() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.vcty;
                    $scope.thi;
                    $scope.thn;
                    $scope.tha;
                    $scope.start_effective_date;
                    $scope.datazone = [];
                    $scope.openCalendar = function (index) {
                        $scope.isCalendarOpened[index] = true;
                    }
                    MstService.GetZonePri(function (respon) {
                        $scope.datazone = respon.data;
                        console.log($scope.datazone)
                    })
                    $scope.data = [];
                    $scope.location = [];
                    MstService.GetMSDLocationGetCity(function (respon) {
                        $scope.location = respon.data;
                        console.log($scope.location)
                    })
                    $scope.cekcitygan = function (param) {

                        console.log(param)
                        MstService.cekCity(param, function (response) {
                            $scope.address = response.data;
                            console.log($scope.address)
                        })
                        
                    }

                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.insert = function () {

                        if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", " Zone Must be Fill");
                        } else if ($scope.thn == "" || $scope.thn == undefined) {
                            UIControlService.msg_growl("error", " City Must be Fill")
                        } else if ($scope.thi == "" || $scope.thi == undefined) {
                            UIControlService.msg_growl("error", " Address Must be Fill");
                        } else if ($scope.start_effective_date == "" || $scope.start_effective_date == undefined) {
                            UIControlService.msg_growl("error", " Start Effective Year Must be Fill");
                        } else {
                            var dat = {
                                zone: $scope.vcty,
                                city: $scope.thn,
                                address:$scope.thi,
                                start_effective_year: $scope.start_effective_date,
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true

                            }
                           
                            $scope.data.push(dat)
                            MstService.insZoneMap($scope.data, function (reply) {
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

        vm.updZoneMap = updZoneMap;
        function updZoneMap(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.vcty;
                    $scope.thn;
                    $scope.thi;
                    $scope.zoneid;
                    $scope.start_effective_date;
                    $scope.add;
                    $scope.check;
                    $scope.openCalendar = function (index) {
                        $scope.isCalendarOpened[index] = true;
                    }
                    $scope.datazone = [];
                   
                    MstService.GetZonePri(function (respon) {
                        $scope.datazone = respon.data;
                        console.log($scope.datazone)
                    })

                    
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetMSDZoneMapID(param, function (reply) {
                        $scope.vcty = reply.data[0].zone;
                        $scope.thn = reply.data[0].city;
                        $scope.cekcitygan($scope.thn);
                        
                        $scope.start_effective_date = reply.data[0].start_effective_year;
                        $scope.thi = reply.data[0].address;
                        $scope.check = reply.data[0].is_active;
                        console.log(reply.data)
                        console.log(reply)
                    })
                  
                    $scope.location = [];
                    MstService.GetMSDLocationGetCity(function (respon) {
                        $scope.location = respon.data;
                      
                        console.log($scope.location)
                        
                    })
                    $scope.cekcitygan = function (param) {
                       
                        console.log(param)
                        MstService.cekCity(param, function (response) {
                            $scope.address = response.data;
                            console.log($scope.address)
                        })

                    }

                    $scope.update = function () {
                       
                        var data;
                        console.log($('#zone').val())
                        if ($('#zone').val() == "" || $('#zone').val() == undefined) {
                            UIControlService.msg_growl("error", " Zone Name Must be Fill");
                        } else if ($('#city').val() == "" || $('#city').val() == undefined) {
                            UIControlService.msg_growl("error", " City Must be Fill")
                        } else if ($('#address').val() == "" || $('#address').val() == undefined) {
                            UIControlService.msg_growl("error", " Address Must be Fill")
                        } else if ($scope.start_effective_date == "" || $scope.start_effective_date == undefined) {
                            UIControlService.msg_growl("error", " Start Effective Year Must be Fill");
                        } else {
                            if ($('.tes').is(':checked') || $scope.check == 1) {
                                data = {
                                    zonemap_id: param,
                                    zone_id: $('#zone').val(),
                                    location_id: $('#address').val(),
                                    start_effective_year: $scope.start_effective_date,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                            } else {
                                data = {
                                    zonemap_id: param,
                                    zone_id: $('#zone').val(),
                                    location_id: $('#address').val(),
                                    start_effective_year: $scope.start_effective_date,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: false

                                }
                            }

                            
                            MstService.updZoneMap(data, function (reply) {
                                if (reply.status === 200) {
                                    UIControlService.msg_growl("success", " Update Success");
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    console.log("Berhasil Insert");
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



        vm.uploadZM = uploadZM;
        function uploadZM() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadCAF.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.filecaf;
                    $scope.data = [];
                    $scope.id = [[]];
                    $scope.data2 = []
                    $scope.datazn = [];
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
                    $scope.uploadFile = function () {
                        $scope.upload($scope.filevehicle);
                    };
                    $scope.dtOpt = DTOptionsBuilder.newOptions()
   .withDisplayLength(5)
   .withOption('bLengthChange', false)
   .withOption('responsive', true)
   .withPaginationType('full_numbers');
                    $scope.upload = function (file) {

                        ExcelReaderService.readExcel(file, function (reply) {

                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];

                                if (sheet1[1].Column2 != "Zone") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "Must Be Zone");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "City") {
                                    UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "Must Be City");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column4 != "Address") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "Must Be Address");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column5 != "StartEffectiveYear") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "Must Be StartEffectiveYear");
                                    $uibModalInstance.dismiss('cancel');
                                }

                                for (var i = 2; i < sheet1.length; i++) {
                                    var zone = sheet1[i].Column2;
                                    var city = sheet1[i].Column3;
                                    var address = sheet1[i].Column4;
                                    var item = {
                                        zone: sheet1[i].Column2,
                                        start_effective_year: sheet1[i].Column5,
                                        city: sheet1[i].Column3,
                                        address: sheet1[i].Column4,
                                        created_date: new Date(),
                                        type: "upload",
                                        created_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        is_active: true

                                    };
                                    console.log(item)
                                    if (!item.zone) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Zone Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.city) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "City Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.address) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Address Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.start_effective_year) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Start Effective Year Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    //MstService.CekLocation({ city: city, address: address }, function (response) {
                                    //    if (response.status === 200) {
                                    //        var item = {
                                    //            location_id: response.data.location_id
                                    //        }
                                    //        $scope.data2.push(item)
                                    //    } else {
                                    //        $.growl.error({ message: "Failed Get Location" });
                                    //        UIControlService.unloadLoading();
                                    //    }

                                    //}, function (err) {
                                       
                                    //    UIControlService.unloadLoading();
                                    //    $scope.data = [];
                                    //    UIControlService.msg_growl("error", "Location Excel Column " + city + "/" + address + " Undefined");
                                    //})

                                    //MstService.GetZoneID(zone, function (respon) {
                                    //    if (respon.status === 200) {
                                    //        var item = {
                                    //            zone_id: respon.data.zone_id
                                    //        }
                                    //        $scope.datazn.push(item)
                                    //        console.log($scope.datazn)
                                    //    } else {
                                    //        $.growl.error({ message: "Failed Get Location" });
                                    //        UIControlService.unloadLoading();
                                           
                                    //    }

                                    //}, function (err) {
                                  
                                    //    UIControlService.unloadLoading();
                                    //    $scope.data = [];
                                    //    UIControlService.msg_growl("error", "ZoneMapping Excel Column " + zone + " Undefined");
                                    //})
                                    
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
                        //var a = $scope.check();
                     
                        MstService.insZoneMap($scope.data, function (reply) {
                            console.log(reply)
                            var word2 = [];
                            var word = [];
                           
                            console.log(word);
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];
                                 
                                    word = reply.data.DataReturnCadangan.split(",");
                                    word2 = reply.data.DataReturnCadangan2.split(",");
                                    words = reply.data.DataReturn.split(",");


                                    UIControlService.msg_growl("error", "<u><i><b>" + words[i] + "</u></i></b>  Already In Database");

                                }

                            }
                            if (word.length > 0) {
                                for (var c = 0; c < word.length ; c++) {
                                    if (word[c] == "") {
                                        console.log("do nothing")
                                    } else {
                                        UIControlService.msg_growl("error", "<u><i><b>" + word[c] + "</u></i></b>  Not Found In Database");
                                    }

                                }
                            }
                            if (word2.length > 0) {
                                for (var c = 0; c < word2.length ; c++) {
                                    if (word2[c] == "") {
                                        console.log("do nothing")
                                    } else {
                                        UIControlService.msg_growl("error", "<u><i><b>" + word2[c] + "</u></i></b>  Not Found In Database");
                                    }

                                }
                            }
                            if (reply.status == 200) {
                                //$interval(function () {
                                //    vm.jLoad(1);
                                //}, 5000);
                                vm.init();
                                UIControlService.msg_growl("success", " Upload Excel Success");
                            }
                            else {
                                UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR");
                            }
                        },function (error) {
                                UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR")
                                UIControlService.unloadLoadingModal();
                            });
                        $uibModalInstance.dismiss('cancel');
                    };

                    //$scope.check = function () {
                    //    console.info($scope.data2[0])
                    //    console.info($scope.data.length)
                    //    console.info($scope.datazn)

                    //    for (var i = 0; i < $scope.data.length; i++) {
                    //        var item = {
                    //            zone_id: $scope.datazn[i].zone_id,
                    //            location_id: $scope.data2[i].location_id,
                    //            start_effective_year: $scope.data[i].start_effective_year,
                    //            created_date: new Date(),
                    //            created_by: localStorage.getItem('username'),
                    //            last_modified: new Date(),
                    //            modified_by: localStorage.getItem('username'),
                    //            is_active: true

                    //        }
                    //        $scope.semua.push(item)
                    //    }

                    //    console.log($scope.semua);

                    //}
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
        };
        vm.toExcel = toExcel;
        function toExcel(tabel) {
            alasql.fn.datetime = function (dateStr) {
                var date = vm.dateFormat(new Date(parseInt(dateStr.substring(6, 19))));
                return date.toLocaleString();
            };
            alasql('SELECT zone AS `Zone Name`, city AS `City`,address AS `Address`, start_effective_year AS `Start Effective Date`, datetime(modified_date) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-zonemapping.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.delZoneMap = delZoneMap;
        function delZoneMap(param) {
            UIControlService.loadLoading("loading");
            var data = {
                zonemap_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delZoneMap(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", " InActive Success");
                    UIControlService.unloadLoading();
                    init();
                } else {
                    $.growl.error({ message: "Gagal insert data vehicle spects" });
                }
            })
        }


    }

})();