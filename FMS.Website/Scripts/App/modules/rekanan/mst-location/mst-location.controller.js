(function () {
    'use strict';

    angular.module("app").controller("MSTLocCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
         'GlobalConstantService', '$state', '$stateParams', 'MstService',
     'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.dataLocation = [];
        vm.totalRecords = 0;
        vm.roles = localStorage.getItem('roles');
     
        vm.remark = "";
        vm.modified = "";
        vm.by = "";
        vm.aktive;
        vm.off = 1;
        vm.recat = "";
        vm.baset = "";
        vm.dataexcel = [];
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

        function init()
        {
            //UIControlService.loadLoading('Loading');
            MstService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                console.info(vm.id_coordinator);
                console.log('coba')
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.loadLocation();
        }

        vm.loadLocation = loadLocation;
        function loadLocation() {
            UIControlService.unloadLoading();
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.remark + "|" + vm.recat + "|" + vm.baset + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.recat + "|" + vm.baset + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }

            MstService.GetLocationExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetLocation(data,function (reply) {
                if (reply.status === 200) {
                    vm.dataLocation = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataLocation)

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
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.remark + "|" + vm.recat + "|" + vm.baset + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.recat + "|" + vm.baset + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }

           
            console.log(data)
            MstService.GetLocationExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetLocation(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataLocation = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataLocation)

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
            console.log(offset)
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.remark + "|" + vm.recat + "|" + vm.baset + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.recat + "|" + vm.baset + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }

            
            MstService.GetLocation(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataLocation = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataLocation)

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
        }

        vm.insLoc = insLoc;
        function insLoc() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.vcty;
                    $scope.vcus;
                    $scope.base;
                    $scope.data = [];
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.insert = function () {

                        if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", "City Must be Fill");
                        } else if ($scope.vcus == "" || $scope.vcus == undefined) {
                            UIControlService.msg_growl("error", "Address Must be Fill");
                        } else if ($scope.base == "" || $scope.base == undefined) {
                            UIControlService.msg_growl("error", "Base Town Must be Fill");
                        } else {
                            var dat = {
                                city: $scope.vcty,
                                address: $scope.vcus,
                                basetown: $scope.base,
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true

                            }
                            $scope.data.push(dat)
                            MstService.insLoc($scope.data, function (reply) {
                                if (reply.data.Count > 0) {
                                    for (var i = 0; i < reply.data.Count; i++) {
                                        var words = [];
                                        words = reply.data.DataReturn.split(",");
                                        UIControlService.msg_growl("error", "Location <u><b>" + words[i] + "</u></b> Already In Database");
                                        $scope.data = [];
                                    }
                                }else{
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", "Insert Success")
                                } else {
                                    $.growl.error({ message: "Insert Data Location Failed" });
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

        vm.delLoc = delLoc;
        function delLoc(param) {
            UIControlService.loadLoading("loading");
            var data = {
                location_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delLoc(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "InActive Success")
                    UIControlService.unloadLoading();
                    loadLocation();
                }
            })
        }

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            alasql.fn.datetime = function (dateStr) {
                var date = vm.dateFormat(new Date(parseInt(dateStr.substring(6, 19))));
                return date.toLocaleString();
            };
            alasql('SELECT  city AS `City`, address AS `Address`, basetown AS `Base Town`, datetime(last_modified) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-location.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.uploadLoc = uploadLoc;
        function uploadLoc() {
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
                                console.info(sheet1);
                                if (sheet1[1].Column2 != "City") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "Must be City");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "Address") {
                                    UIControlService.msg_growl("error", "Column 3 Name Is" + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "Must be Address");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column4 != "Base Town") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is" + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "Must be Base Town");
                                    $uibModalInstance.dismiss('cancel');
                                }

                                for (var i = 2; i < sheet1.length; i++) {
                                    var item = {
                                        city: sheet1[i].Column2,
                                        address: sheet1[i].Column3,
                                        basetown:sheet1[i].Column4,
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        type:"upload",
                                        is_active: true

                                    };
                                    console.log(item)
                                    if (!item.city) {
                                        UIControlService.msg_growl("error", "Item Line" + (i + 1) + " Not valid:");
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
                                    if (!item.basetown) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Base Town Must be Fill");
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
                        MstService.insLoc($scope.data, function (reply) {
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];
                                    words = reply.data.DataReturn.split(",");
                                    UIControlService.msg_growl("error", "Location <u><b>" + words[i] + "</u></b> Already In Database");
                                    $scope.data = [];
                                }
                            }
                            if (reply.status == 200) {
                                //$interval(function () {
                                //    vm.jLoad(1);
                                //}, 5000);
                                if (reply.data.Count == 1) {
                                    UIControlService.msg_growl("error", "Data Already Used")
                                } else {
                                    UIControlService.msg_growl("success", "Upload Excel Success")
                                    vm.init();
                                }
                              
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


        vm.updLoc = updLoc;
        function updLoc(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.vcty;
                    $scope.vcus;
                    $scope.base;
                    $scope.check;
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetLocByID(param, function (reply) {
                        $scope.vcty = reply.data.city;
                        $scope.vcus = reply.data.address;
                        $scope.check = reply.data.is_active;
                        $scope.base = reply.data.basetown;
                    })

                    $scope.update = function () {

                        if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", "City Must be Fill");
                        } else if ($scope.vcus == "" || $scope.vcus == undefined) {
                            UIControlService.msg_growl("error", "Address Must be Fill");
                        } else if ($scope.base == "" || $scope.base == undefined) {
                            UIControlService.msg_growl("error", "Base Town Must be Fill");
                        } else {
                            if ($('.tes').is(':checked') || $scope.check == 1) {
                                var data = {
                                    location_id: param,
                                    city: $scope.vcty,
                                    address: $scope.vcus,
                                    basetown:$scope.base,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                            } else {
                                var data = {
                                    location_id: param,
                                    city: $scope.vcty,
                                    address: $scope.vcus,
                                    basetown: $scope.base,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: false

                                }
                            }
                           
                            MstService.updLoc(data, function (reply) {
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();

                                    UIControlService.msg_growl("success", "Update Success")
                                } else {
                                    $.growl.error({ message: "Update Data Location Failed" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }

                    }


                }
            })
        }

    }

})();