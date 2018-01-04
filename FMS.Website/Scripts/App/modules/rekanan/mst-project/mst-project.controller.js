(function () {
    'use strict';

    angular.module("app").controller("MSTPRCtrl", ctrl);
     ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'MstService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.dataProject = [];
        vm.totalRecords = 0;
        vm.roles = localStorage.getItem('roles');
       
        vm.remark = "";
        vm.modified = "";
        vm.by = "";
        vm.aktive;
        vm.off = 1;
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
            vm.loadProject();
        }
        vm.dataexcel = [];
        vm.loadProject = loadProject;
        function loadProject() {
            UIControlService.unloadLoading();
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.remark + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.GetProjectExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetProject(data,function (reply) {
                if (reply.status === 200) {
                    vm.dataProject = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataProject)

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
                var data = vm.remark + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.GetProjectExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetProject(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataProject = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataProject)

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
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.remark + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.remark + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.GetProject(data, function (reply) {
                if (reply.status === 200) {
                    vm.dataProject = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log("Sukses")
                    console.log(vm.dataProject)

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
        }
        vm.insProject = insProject;
        function insProject() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.vcty;
                    $scope.data = [];
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.insert = function () {

                        if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", " Project Must be Fill");
                        } else {
                            var dat = {
                                project_name: $scope.vcty,
                                created_date: new Date(),
                                createdBy: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true

                            }
                            $scope.data.push(dat)
                            MstService.insProject($scope.data, function (reply) {
                                if (reply.data.Count > 0) {
                                    for (var i = 0; i < reply.data.Count; i++) {
                                        var words = [];
                                        words = reply.data.DataReturn.split(",");
                                        UIControlService.msg_growl("error", "Project <u><i><b>" + words[i] + "</u></i></b> Already In Database");
                                        $scope.data = [];
                                    }
                                }else{
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", "Insert Success");
                                } else {
                                    $.growl.error({ message: "Insert data Project Failed" });
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

        vm.updProject = updProject;
        function updProject(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.vcty;
                    $scope.check;

                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    MstService.GetProjectByID(param, function (reply) {
                        $scope.vcty = reply.data.project_name;
                        $scope.check = reply.data.is_active;
                        console.log(reply.data)
                    })

                    $scope.update = function () {
                        var data;
                        console.log($scope.check)
                        if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", " Project Must be Fill");
                            
                        } else {
                            if ($('.tes').is(':checked') || $scope.check == 1) {
                                data = {
                                    project_id: param,
                                    project_name: $scope.vcty,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                            } else {
                                data = {
                                    project_id: param,
                                    project_name: $scope.vcty,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: false

                                }
                            }
                           
                            MstService.updProject(data, function (reply) {
                                if (reply.status === 200) {
                                    $uibModalInstance.dismiss('cancel');
                                    init();
                                    UIControlService.msg_growl("success", " Update Success");
                                } else {
                                    $.growl.error({ message: "Insert data Project Failed" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }

                    }


                }
            })
        }



        vm.uploadPro= uploadPro;
        function uploadPro() {
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
                                if (sheet1[1].Column2 != "Project Name") {
                                    UIControlService.msg_growl("error", "Column 2 Name is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "Must be Project Name");
                                    $uibModalInstance.dismiss('cancel');
                                }

                                for (var i = 2; i < sheet1.length; i++) {
                                    var item = {
                                        project_name: sheet1[i].Column2,
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        type: "upload",
                                        is_active: true

                                    };
                                    console.log(item)
                                    if (!item.project_name) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + "  Not valid:");
                                        UIControlService.msg_growl("error", "Project Name Must be Fill");
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
                        MstService.insProject($scope.data, function (reply) {
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];
                                    words = reply.data.DataReturn.split(",");
                                    UIControlService.msg_growl("error", "Project <u><i><b>" + words[i] + "</u></i></b> Already In Database");
                                    $scope.data = [];
                                }
                            }
                            if (reply.status == 200) {
                                //$interval(function () {
                                //    vm.jLoad(1);
                                //}, 5000);
                                init();
                                UIControlService.msg_growl("success", " Upload Excel Success");
                            }
                            else {
                                UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR");
                            }
                        },
                            function (error) {
                                UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR")
                                UIControlService.unloadLoadingModal();
                                init()
                            });
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
        };

        vm.aktifs = aktifs
        function aktifs(param) {

            if (param == 1) {
                vm.aktive = 'Active'
            } else {
                vm.aktive = 'InActive'
            }
            return vm.aktive
        }


        vm.toExcel = toExcel;
        function toExcel(tabel) {
            alasql.fn.datetime = function (dateStr) {
                var date = vm.dateFormat(new Date(parseInt(dateStr.substring(6, 19))));
                return date.toLocaleString();
            };
            alasql('SELECT project_name AS `Project Name`, datetime(last_modified) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-project.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.delProject = delProject;
        function delProject(param) {
            UIControlService.loadLoading("loading");
            var data = {
                project_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delProject(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "InActive Success");
                    UIControlService.unloadLoading();
                    init();
                } else {
                    $.growl.error({ message: "Insert data Project Failed" });
                }
            })
        }


    }

})();