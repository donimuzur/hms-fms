(function () {
    'use strict';

    angular.module("app").controller("MSTVendorCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
          'GlobalConstantService', '$state', '$stateParams', 'MstService',
      'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.dataVendor = [];
        vm.totalRecords = 0;
        vm.roles = localStorage.getItem('roles');
        vm.remark = "";
        vm.nalias = "";
        vm.modified = "";
        vm.by = "";
        vm.aktive;
        vm.off = 1;
        vm.recat = "";
        vm.rename = "";
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
            vm.loadVendor();
        }
        vm.dataexcel = [];
        vm.loadVendor = loadVendor;
        function loadVendor() {
            UIControlService.unloadLoading();
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            var email = vm.rename.replace(".", "~2b");
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.remark + "|" + vm.recat + "|" + email + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.recat + "|" + email + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.GetVendorExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetVendor(data,function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataVendor = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("berhasil Gan")
                    console.log(vm.dataVendor)
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
        vm.search = search;
        function search() {
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            var email = vm.rename.replace(".", "~2b");
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.remark + "|" + vm.recat + "|" + email + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.recat + "|" + email + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.GetVendorExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetVendor(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataVendor = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("berhasil Gan")
                    console.log(vm.dataVendor)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
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
            var email = vm.rename.replace(".", "~2b");
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.remark + "|" + vm.recat + "|" + email + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.recat + "|" + email + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.GetVendor(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataVendor = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("berhasil Gan")
                    console.log(vm.dataVendor)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
        }
        vm.updVendor = updVendor;
        function updVendor(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updVen.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.Vname;
                    $scope.Vshort;
                    $scope.Vemail;
                    $scope.check;
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetMSDVendorID(param, function (reply) {
                        $scope.Vname=reply.data.vendor_name;
                        $scope.Vshort =reply.data.name_alias;
                        $scope.Vemail = reply.data.email_address;
                        $scope.check = reply.data.is_active;
                        console.log(reply.data)
                    })
                    $scope.email = function (param) {
                        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                        return re.test(param);
                    }
                    $scope.update = function () {

                        if ($scope.Vname == "" || $scope.Vname == undefined) {
                            UIControlService.msg_growl("error", " Vendor Name Must be Fill");
                        } else if ($scope.Vshort == "" || $scope.Vshort == undefined) {
                            UIControlService.msg_growl("error", " Short Name Must be Fill")
                        } else if ($scope.Vemail == "" || $scope.Vemail == undefined) {
                            UIControlService.msg_growl("error", " Email Address Must be Fill Or Wrong Email Format");
                        } else if ($scope.email($scope.Vemail) != true) {
                            UIControlService.msg_growl("error", " Wrong Email Format");
                        } else {

                            if ($('.tes').is(':checked') || $scope.check == 1) {
                                var data = {
                                    vendor_id: param,
                                    vendor_name: $scope.Vname,
                                    name_alias: $scope.Vshort,
                                    email_address: $scope.Vemail,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                            } else {
                                var data = {
                                    vendor_id: param,
                                    vendor_name: $scope.Vname,
                                    name_alias: $scope.Vshort,
                                    email_address: $scope.Vemail,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: false

                                }
                            }

                           
                            MstService.updVen(data, function (reply) {
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", "Update Success")
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

        vm.delVendor = delVendor;
        function delVendor(param) {
            UIControlService.loadLoading("loading");
            var data = {
                vendor_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delVen(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "InActive Success")
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
                    $scope.email = function (param) {
                        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                        return re.test(param);
                    }
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
                                if (sheet1[1].Column4 != "Email Address") {
                                    UIControlService.msg_growl("error", "Column 4 Name Is " + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "Must be Email Address");
                                    $uibModalInstance.dismiss('cancel');
                                }

                                for (var i = 2; i < sheet1.length; i++) {
                                    var item = {
                                        vendor_name: sheet1[i].Column2,
                                        name_alias: sheet1[i].Column3,
                                        email_address: sheet1[i].Column4,
                                        type: "upload",
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        is_active: true

                                    };
                                    console.log(item)
                                    if (!item.vendor_name) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Vendor Name Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.name_alias) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Short Name Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.email_address) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Email Address Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if ($scope.email(item.email_address)!=true) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Wrong Format Email Address");
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
                        MstService.insVen($scope.data, function (reply) {
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];
                                    words = reply.data.DataReturn.split(",");
                                    UIControlService.msg_growl("error", "Vendor <u><i><b>" + words[i] + "</u></i></b> Already In Database");
                                    $scope.data = [];
                                }
                            }
                            if (reply.status == 200) {
                                //$interval(function () {
                                //    vm.jLoad(1);
                                //}, 5000);
                                vm.init();
                                UIControlService.msg_growl("success", "Upload Excel Success")
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
        function toExcel(tabel) {
            alasql.fn.datetime = function (dateStr) {
                var date = vm.dateFormat(new Date(parseInt(dateStr.substring(6, 19))));
                return date.toLocaleString();
            };
            alasql('SELECT vendor_name AS `Vendor Name`,name_alias AS `Short Name`,email_address AS `Email Address`, datetime(last_modified) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-vendor.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.insVen = insVen;
        function insVen() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVen.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.Vname;
                    $scope.Vshort;
                    $scope.Vemail;
                    $scope.data = [];
                  
                    $scope.email = function (param) {
                        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                        return re.test(param);
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.insert = function () {

                        if ($scope.Vname == "" || $scope.Vname == undefined) {
                            UIControlService.msg_growl("error", " Vendor Name Must be Fill");
                        } else if ($scope.Vshort == "" || $scope.Vshort == undefined) {
                            UIControlService.msg_growl("error", " Short Name Must be Fill")
                        } else if ($scope.Vemail == "" || $scope.Vemail == undefined) {
                            UIControlService.msg_growl("error", " Email Address Must be Fill Or Wrong Email Format");
                        } else if ($scope.email($scope.Vemail) != true) {
                            UIControlService.msg_growl("error", " Wrong Email Format");
                        }else {
                            var dat = {
                                vendor_name: $scope.Vname,
                                name_alias: $scope.Vshort,
                                email_address: $scope.Vemail,                               
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true

                            }
                            $scope.data.push(dat)
                            MstService.insVen($scope.data, function (reply) {
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
                                    init();
                                    UIControlService.msg_growl("success", "Insert Success")
                                } else {
                                    $.growl.error({ message: "Insert Vendor Failed" });
                                }
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