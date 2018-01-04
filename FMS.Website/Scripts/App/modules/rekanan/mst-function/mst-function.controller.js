(function () {
    'use strict';

    angular.module("app").controller("MSTFCCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
         'GlobalConstantService', '$state', '$stateParams', 'MstService',
     'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = this;
        vm.coordinator = localStorage.getItem('username');
        vm.dataRg = [];
        vm.totalRecords = 0;
        vm.ids;
        vm.iddd = [];
        vm.dateFormat = dateFormat;
        vm.tha;
        vm.fun = "";
        vm.gro = "";
        vm.date = "";
       
        vm.modified = "";
        vm.by = "";
        vm.off = 1;
        vm.roles = localStorage.getItem('roles');
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
            vm.loadRg();
        }

        vm.loadRg = loadRg;
        function loadRg() {
            UIControlService.unloadLoading();
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN" || vm.format(vm.date) == "NaN-undefined-NaN") {
                var data = vm.fun + "|" + vm.gro + "|" + vm.date + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else if (vm.format(vm.date) == "NaN-undefined-NaN") {
                var data = vm.fun + "|" + vm.gro + "|" + vm.date + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.fun + "|" + vm.gro + "|"+ vm.format(vm.date) + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }
            UIControlService.unloadLoading();
            MstService.GetFunctionExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetFunction(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataRg = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataRg)

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
            var offset = (param * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN" || vm.format(vm.date) == "NaN-undefined-NaN") {
                var data = vm.fun + "|" + vm.gro + "|" + vm.date + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else if (vm.format(vm.date) == "NaN-undefined-NaN") {
                var data = vm.fun + "|" + vm.gro + "|" + vm.date + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.fun + "|" + vm.gro + "|" + vm.format(vm.date) + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }

            UIControlService.unloadLoading();
            MstService.GetFunctionExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetFunction(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataRg = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataRg)

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

        vm.insRg = insRg;
        function insRg() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.isCalendarOpened = [false, false, false];
                    $scope.vcty;
                    $scope.thi;
                    $scope.thn;
                  
                    $scope.effective_date;
                   
                    $scope.functioncost = [];

                   
                    $scope.openCalendar = function (index) {
                        $scope.isCalendarOpened[index] = true;
                    }
                    $scope.data = [];
                    $scope.datargm = [];
                    $scope.coba = function (param) {
                        return param;
                    }
                    MstService.Getfunctioncost(function (reply) {
                        $scope.functioncost = reply.data;
                    })
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.insert = function () {
                      
                        if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", " Regional Group Must be Fill");
                        } else if ($scope.thn == "" || $scope.thn == undefined) {
                            UIControlService.msg_growl("error", " City Must be Fill")
                        } else if ($scope.tha == "" || $scope.tha == undefined) {
                            UIControlService.msg_growl("error", " Address Must be Fill");
                        } else if ($scope.thi == "" || $scope.thi == undefined) {
                            UIControlService.msg_growl("error", " Effective Date Must be Fill");
                        } else {
                            MstService.CekLocation({ city: $scope.thn, address: $scope.tha }, function (respon) {
                                $scope.loca_id = respon.data.location_id;
                                console.log($scope.loca_id)
                                var dat = {
                                    regional_group: $scope.vcty,
                                    location_id: $scope.loca_id,
                                    effective_date: vm.dateFormat($scope.thi),

                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                                $scope.data.push(dat)


                                MstService.insRg($scope.data, function (reply) {
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
                                            $uibModalInstance.dismiss('cancel');

                                            init();
                                            UIControlService.msg_growl("success", " Insert Success");
                                        } else {
                                            $.growl.error({ message: "Insert data Failed vehicle spects" });
                                        }
                                    }
                                }, function (err) {
                                    console.info("error:" + JSON.stringify(err));
                                })

                            })

                        }
                    }
                }
            })
        }

        vm.updRg = updRg;
        function updRg(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updVhcs.html',
                backdrop: 'static',
                resolve: {
                    test: function () {
                        return param;
                    }
                },
                controller: function ($uibModalInstance, $scope, test) {
                    $scope.isCalendarOpened = [false, false, false];

                    $scope.city;
                    $scope.add;
                    $scope.loc;
                    $scope.rgroup = test.regional_group;
                    $scope.location = [];
                    $scope.datargm = [];
                    $scope.rm = [];
                    $scope.vcty = test.RegionalName;
                    $scope.thn = test.address;
                    $scope.cty = test.city;

                    $scope.thi = new Date(test.effective_date);
                    $scope.check = test.is_active;

                    MstService.GetRegionalMap(function (re) {
                        $scope.datargm = re.data;
                        console.log(re.data)

                    })
                    MstService.GetMSDLocationGetCity(function (respon) {
                        $scope.location = respon.data;

                        console.log($scope.location)
                        console.log(respon.data.City)
                    })
                    $scope.openCalendar = function (index) {
                        $scope.isCalendarOpened[index] = true;
                    }
                    $scope.cekcitygan = function (param) {

                        console.log(param)
                        MstService.cekCity(param, function (response) {
                            $scope.address = response.data;
                            console.log($scope.address)
                        })

                    }
                    $scope.cekcitygan($scope.cty);

                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }


                    $scope.update = function () {
                        var data;
                        if ($scope.rgroup == "" || $scope.rgroup == undefined) {
                            UIControlService.msg_growl("error", " Regional Group Must be Fill");
                        } else if ($scope.cty == "" || $scope.cty == undefined) {
                            UIControlService.msg_growl("error", " City Must be Fill")
                        } else if ($scope.thn == "" || $scope.thn == undefined) {
                            UIControlService.msg_growl("error", " Address Must be Fill")
                        } else if ($scope.thi == "" || $scope.thi == undefined) {
                            UIControlService.msg_growl("error", " Effective Date Must be Fill");
                        } else {
                            MstService.CekLocation({ city: $scope.cty, address: $scope.thn }, function (respon) {

                                if ($('.tes').is(':checked') || $scope.check == 1) {
                                    data = {
                                        regionalmap_id: test.regionalmap_id,
                                        regional_group: $scope.rgroup,
                                        location_id: respon.data.location_id,
                                        effective_date: vm.dateFormat($scope.thi),
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        is_active: true

                                    }
                                } else {
                                    data = {
                                        regionalmap_id: test.regionalmap_id,
                                        regional_group: $scope.rgroup,
                                        location_id: respon.data.location_id,
                                        effective_date: vm.dateFormat($scope.thi),
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        is_active: false

                                    }
                                }


                                MstService.updRg(data, function (reply) {
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

                            })


                        }

                    }
                }
            })
        }



        vm.uploadRg = uploadRg;
        function uploadRg() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadCAF.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.filecaf;
                    $scope.data = [];
                    $scope.id = [[]];
                    $scope.data2 = []
                    $scope.semua = [];
                    $scope.data3 = [];
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

                                if (sheet1[1].Column2 != "RegionalGroup") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "Must Be RegionalGroup");
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
                                if (sheet1[1].Column5 != "EffectiveDate") {
                                    UIControlService.msg_growl("error", "Column 5 Name Is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "Must Be EffectiveDate");
                                    $uibModalInstance.dismiss('cancel');
                                }

                                for (var i = 2; i < sheet1.length; i++) {
                                    var regional_name = sheet1[i].Column2;
                                    var city = sheet1[i].Column3;

                                    var item = {
                                        regional_group: sheet1[i].Column2,

                                        effective_date: vm.dateFormat(sheet1[i].Column5),
                                        city: sheet1[i].Column3,
                                        address: sheet1[i].Column4,
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        type: "upload",
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        is_active: true

                                    };
                                    console.log(item)
                                    if (!item.regional_group) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Regional Group Must be Fill");
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
                                    if (!item.effective_date) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Effective Date Must be Fill");
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
                    //$scope.chekreg = function (param) {
                    //    console.log(param)
                    //    MstService.CekRegionalName(param, function (response) {
                    //        if (response.status === 200) {
                    //            var item = {
                    //                regional_id: response.data.ragional_id
                    //            }
                    //            console.log(item)
                    //            console.log(response)
                    //            $scope.data3.push(item)
                    //            console.log($scope.data3)
                    //        } else {
                    //            $.growl.error({ message: "Failed Get Location" });
                    //            UIControlService.unloadLoading();
                    //        }
                    //    }, function (err) {


                    //        $scope.data = [];
                    //        UIControlService.msg_growl("error", "RegionalMapping Excel Column " + regional_name + " Undefined");

                    //    })
                    //}
                    //$scope.chekloc = function (city, address) {
                    //    console.log(city + " " + address)
                    //    MstService.CekLocation({ city: city, address: address }, function (response) {
                    //        if (response.status === 200) {
                    //            var item = {
                    //                location_id: response.data.location_id
                    //            }
                    //            console.log(item)
                    //            console.log(response)
                    //            $scope.data2.push(item)
                    //            console.log($scope.data2)
                    //        } else {
                    //            $.growl.error({ message: "Failed Get Location" });
                    //            UIControlService.unloadLoading();

                    //        }
                    //    }, function (err) {

                    //        UIControlService.unloadLoading();
                    //        $scope.data = [];
                    //        UIControlService.msg_growl("error", "RegionalMapping Excel Column " + city + "/" + address + " Undefined");

                    //    })
                    //}
                    $scope.simpan = function () {
                        //var a = $scope.check();
                        MstService.insRg($scope.data, function (reply) {
                            console.log(reply)
                            var word = [];
                            var word2 = [];
                            word = reply.data.DataReturnCadangan.split(",");
                            word2 = reply.data.DataReturnCadangan2.split(",");
                            console.log(word);
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];

                                    words = reply.data.DataReturn.split(",");


                                    UIControlService.msg_growl("error", "<u><i><b>" + words[i] + "</u></i></b>  Already In Database");

                                }

                            }
                            if (word.length > 0) {
                                for (var c = 0; c < word.length ; c++) {
                                    UIControlService.msg_growl("error", "<u><i><b>" + word[c] + "</u></i></b>  Not Found In Database");

                                }
                            }

                            if (reply.status == 200) {
                                //$interval(function () {
                                //    vm.jLoad(1);
                                //}, 5000);
                                UIControlService.msg_growl("success", "Upload Excel Success")
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

                    $scope.check = function () {
                        console.info($scope.data2[0])
                        console.info($scope.data.length)
                        console.info($scope.data3)
                        for (var i = $scope.data.length - 1; i >= 0; i--) {
                            console.log(i)
                            console.log($scope.data[i].effective_date)
                            console.log($scope.data2[i])
                            console.log($scope.data3[i])
                            var item = {
                                regional_group: $scope.data3[i].regional_id,
                                location_id: $scope.data2[i].location_id,
                                effective_date: new Date($scope.data[i].effective_date),
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true

                            }
                            $scope.semua.push(item)
                        }

                        console.log($scope.semua);

                    }

                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
        };

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            alasql('SELECT RegionalName AS `Regional Name`,city AS `City`, address AS `Address`, effective_date AS `Effective Date`, last_modified AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-regionalmapping.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.delRg = delRg;
        function delRg(param) {
            UIControlService.loadLoading("loading");
            var data = {
                regionalmap_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delRg(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "InActive Success")
                    UIControlService.unloadLoading();
                    init();
                } else {
                    $.growl.error({ message: "Insert Data Failed vehicle spects" });
                }
            })
        }


    }

})();