(function () {
    'use strict';

    angular.module("app").controller("MSTCCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'MstService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
        var vm = $scope;
        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles');
        vm.dataComplaint = [];
        vm.datasComplaint = [];
        vm.dataexcel = [];
        vm.totalRecords = 0;
        vm.role="";
        vm.complaint="";
        vm.modified="";
        vm.by = "";
        vm.aktif = "";
        vm.aktive;
        vm.off = 1;
       
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
            vm.loadComplaint();
        }

        vm.loadComplaint = loadComplaint;
        function loadComplaint() {
            UIControlService.unloadLoading();
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.role + "|" + vm.complaint + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.role + "|" + vm.complaint + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetComplaintExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetComplaint(data,function (reply) {
                if (reply.status === 200) {
                    vm.dataComplaint = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                   console.log(vm.dataComplaint)
                    console.log("Sukses")
                    console.log(reply)
                  

                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log("oke Gan");
        }
        vm.aktifs = aktifs
        function aktifs(param) {
            
            if (param == 1) {
                vm.aktive = 'Active'
            } else {
                vm.aktive = 'InActive'
            }
            return vm.aktive
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
            var coba;
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.off * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.role + "|" + vm.complaint + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.role + "|" + vm.complaint + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;

            }
            console.log(data)
            MstService.GetComplaintExcel(data, function (reply) {
                vm.dataexcel = reply.data.List;
            })
            MstService.GetComplaint(data, function (response) {
                vm.dataComplaint = response.data.List;
                vm.totalRecords = response.data.Count;
            })
            console.log(data)
        }
        vm.paging = paging;
        function paging(param) {
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var tes = param;
            
            var offset = (tes * 10) - 10;
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.role + "|" + vm.complaint + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.role + "|" + vm.complaint + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;

            }
            MstService.GetComplaint(data, function (response) {
                vm.dataComplaint = response.data.List;
                vm.totalRecords = response.data.Count;
            })
            console.log(data)
        }
        vm.insComplaint = insComplaint;
        function insComplaint() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insVhcs.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.vcty;
                    $scope.vcus;
                    $scope.data = [];
                    $scope.datasComplaint = [];
                    MstService.GetroleCom(function (reply) {
                        if (reply.status === 200) {
                            
                            $scope.datasComplaint = reply.data;
                            console.log($scope.datasComplaint)

                        }
                    }), function (err) {
                        console.info("error:" + JSON.stringify(err));
                        UIControlService.unloadLoading();
                    };
                    console.log($scope.datasComplaint)
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    $scope.insert = function () {
                        console.log($scope.vcus)
                        console.log($scope.vcty)
                        if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", "Complaint Must Be Fill");
                        } else if ($scope.vcus == "" || $scope.vcus == undefined) {
                            UIControlService.msg_growl("error", "Role Must Be Fill");
                        } else {
                            var dat = {
                               
                                role_id: $scope.vcus,
                                complaintCategory: $scope.vcty,
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true

                            }
                            $scope.data.push(dat)
                            MstService.insComplaint($scope.data, function (reply) {
                                console.log(reply)
                          
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
                                        UIControlService.msg_growl("success", "Insert Success");
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

        vm.uploadCom = uploadCom;
        function uploadCom() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadCAF.html',
                backdrop:"static",
                controller: function ($uibModalInstance, $scope) {
                    $scope.filecaf;
                    $scope.data = [];
                    $scope.data2 = [];
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
                                console.info(sheet1);

                                if (sheet1[1].Column2 != "RoleName") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "Must Be RoleName");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "ComplaintCategory") {
                                    UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "Must Be ComplaintCategory");
                                    $uibModalInstance.dismiss('cancel');
                                }
                               


                                for (var i = 2; i < sheet1.length; i++) {

                                    var rolename = sheet1[i].Column2;
                                    console.log(rolename)
                                 
                                    var item = {
                                        role_id:sheet1[i].Column2,
                                        complaintCategory: sheet1[i].Column3,
                                        created_date: new Date(),
                                        created_by: localStorage.getItem('username'),
                                        last_modified: new Date(),
                                        modified_by: localStorage.getItem('username'),
                                        type:"upload",
                                        is_active: true

                                    };
                                    console.log(item)
                                    if (!item.role_id) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Role Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.complaintCategory) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Complaint Category Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    

                                    $scope.data.push(item);
                                    console.log($scope.data)
                                    console.log($scope.data2)
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
                    //$scope.chek = function (param) {
                    //    console.log(param)
                    //    MstService.GetMSDRoleName(param, function (response) {
                    //        var item = {
                    //            role_id: response.data.role_id
                    //        }
                    //        console.log(response)
                    //        $scope.data2.push(item)
                    //        console.log($scope.data2)
                    //    }, function (err) {
                    //        console.info("error:" + JSON.stringify(err));
                    //        $scope.data = [];
                    //        UIControlService.msg_growl("error", "Complaint Excel Column " + rolename + " Undefined");
                    //    })
                    //}

                    $scope.simpan = function () {
                     
                        MstService.insComplaint($scope.data, function (reply) {
                            console.log(reply)
                            var word = [];
                            word = reply.data.DataReturnCadangan.split(",");

                            console.log(word);
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];
                                  
                                    words = reply.data.DataReturn.split(",");
                                  
                                  
                                    UIControlService.msg_growl("error", "<u><i><b>" + words[i] + "</u></i></b>  Already In Database");

                                }
                              
                            }
                            if(word.length >0){
                                for (var c = 0; c < word.length ; c++) {
                                    if (word[c] == "") {
                                        console.log("do nothing")
                                    } else {
                                        UIControlService.msg_growl("error", "<u><i><b>" + word[c] + "</u></i></b>  Not Found In Database");
                                    }
                                  }
                            }
                            if (reply.status == 200) {
                                //$interval(function () {
                                //    vm.jLoad(1);
                                //}, 5000);
                                UIControlService.msg_growl("success", "Upload Excel Success");
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
                    $scope.check = function (respon) {
                        
                        for (var i = $scope.data.length-1; i >= 0; i--) {
                            console.log(i)
                            var item = {
                                role_id: $scope.data2[i].role_id,
                                complaintCategory: $scope.data[i].complaintcategory,
                                created_date: new Date(),
                                created_by: localStorage.getItem('username'),
                                last_modified: new Date(),
                                modified_by: localStorage.getItem('username'),
                                is_active: true

                            };
                            $scope.semua.push(item)
                        }
                        console.log($scope.semua)
                    }
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
            alasql('SELECT  complaint_categorys AS `Category Complaint`,role_name AS `Role`, datetime(modified_date) AS `Last Modified`, modified_by AS `Modified By` INTO XLSX("mst-complaint.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
        }

        vm.updComplaint = updComplaint;
        function updComplaint(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updVhcs.html',
                backdrop: 'static',
                resolve: {
                    test: function () {
                        return param;
                    }
                },
                controller: function ($uibModalInstance, $scope,test) {
                    $scope.vcty = test.complaint_categorys;
                    $scope.vcus = test.role_id;
                    $scope.vcys = test.role_name;
                    $scope.check = test.is_active;
                    //MstService.GetComplaintByID(param, function (respon) {
                    //    $scope.vcty = respon.data[0].complaint_categorys;
                    //    $scope.vcus = respon.data[0].role_id;
                    //    $scope.check=respon.data[0].is_active;
                    //    $scope.vcys = respon.data[0].role_name;
                    //    console.log(respon.data)


                    //}, function (err) {
                    //    console.info("error:" + JSON.stringify(err));
                    //    UIControlService.unloadLoading();
                    //})
                    $scope.datasComplaint = [];
                    MstService.GetroleCom(function (reply) {
                        if (reply.status === 200) {

                            $scope.datasComplaint = reply.data;
                            console.log($scope.datasComplaint)

                        }
                    }), function (err) {
                        console.info("error:" + JSON.stringify(err));
                        UIControlService.unloadLoading();
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                    
                    
                    $scope.update = function () {
                        var data;
                        if ($scope.vcty == "" || $scope.vcty == undefined) {
                            UIControlService.msg_growl("error", "Complaint Must Be Fill");
                        } else if ($scope.vcus == "" || $scope.vcus == undefined) {
                            UIControlService.msg_growl("error", "Role Must Be Fill");
                        } else {
                            if ($('.tes').is(':checked')|| $scope.check == 1) {
                                var data = {
                                    comcat_id: test.compcat_id,
                                    role_id: $scope.vcus,
                                    complaintCategory: $scope.vcty,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: true

                                }
                            } else {
                                var data = {
                                    comcat_id: test.compcat_id,
                                    role_id: $scope.vcus,
                                    complaintCategory: $scope.vcty,
                                    created_date: new Date(),
                                    created_by: localStorage.getItem('username'),
                                    last_modified: new Date(),
                                    modified_by: localStorage.getItem('username'),
                                    is_active: false

                                }
                            }
                            
                            MstService.updComplaint(data, function (reply) {
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

        vm.delComplaint = delComplaint;
        function delComplaint(param) {
            UIControlService.loadLoading("loading");
            var data = {
                comcat_id: param,
                isActive: false,
                last_modified: new Date(),
                ModifyBy: localStorage.getItem('username')
            };
            console.log(data)
            MstService.delComplaint(data, function (reply) {
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