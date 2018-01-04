(function () {
    'use strict';

    angular.module("app").controller("MSTEmpCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
          'GlobalConstantService', '$state', '$stateParams', 'MstService',
      'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder', 'AuthService'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder, AuthService) {
        var vm = $scope;
        vm.employee_id;
        vm.employee_name;
        vm.title_name;
        vm.grup_lvl;
        vm.position;
        vm.cc;
        vm.division;
        vm.directorate;
        vm.city;
        vm.off_loc;
        vm.company;
        vm.emp_id = "";
        vm.emp_name = "";
        vm.grup = "";
        vm.title = "";
        vm.cost = "";
        vm.div = "";
        vm.dir = "";
        vm.city = "";
        vm.off = "";
        vm.com = "";
        vm.modified = "";
        vm.by = "";
        vm.offs = 1;
        vm.email = "";
        vm.dataexcel;
        vm.datasemployee;
        var today = new Date();
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
        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles')
        vm.dataEmployee = [];
        vm.totalRecords = 0;
        
        vm.init = init;
        function init() {
            
            MstService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                
                console.info(vm.id_coordinator);
                console.log(reply.data)
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.loadEmployee();
        }
      
        vm.loadEmployee = loadEmployee;
        function loadEmployee()
        {
            UIControlService.loadLoading("loading");
            var news = vm.roles.replace("_", " ")
            var newss = news.replace(".", "z")
            var pageSize = 10;
            var offset = (vm.offs * 10) - 10;
            var email = vm.email.replace(".", "~2b");
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.emp_id + "|" + vm.emp_name + "|" + vm.grup + "|" + vm.title + "|" + email + "|" + vm.cost + "|" + vm.div + "|" + vm.dir + "|" + vm.city + "|" + vm.off + "|" + vm.com + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.emp_id + "|" + vm.emp_name + "|" + vm.grup + "|" + vm.title + "|" + email + "|" + vm.cost + "|" + vm.div + "|" + vm.dir + "|" + vm.city + "|" + vm.off + "|" + vm.com + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            }
            console.log(data)
            
            MstService.SelectEmployee2(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.datasEmployee = reply.data.List;
                   
                    console.log('Berhasil')
                    console.log(vm.dataEmployee)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
                }

            }),function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };               
            
            MstService.SelectEmployee(data,function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataEmployee = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    vm.ganti = reply.data.List[0].last_modified.substring(6,19);
                    vm.coba = vm.ganti.replace('/', '')
                    vm.tanggal = new Date(parseInt(vm.ganti));
                    vm.tes = vm.coba;
                    console.log(vm.tes)
                    console.log(vm.tanggal)
                    console.log(vm.coba)
                    console.log(vm.ganti)
                    console.log('Berhasil')
                    console.log(vm.dataEmployee)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
                }

            }),function (err) {
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
            var offset = (vm.offs * 10) - 10;
            var email = vm.email.replace(".", "~2b");
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.emp_id + "|" + vm.emp_name + "|" + vm.grup + "|" + vm.title + "|"+email+"|" + vm.cost + "|" + vm.div + "|" + vm.dir + "|" + vm.city + "|" + vm.off + "|" + vm.com + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.emp_id + "|" + vm.emp_name + "|" + vm.grup + "|" + vm.title + "|"+email+"|" + vm.cost + "|" + vm.div + "|" + vm.dir + "|" + vm.city + "|" + vm.off + "|" + vm.com + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.SelectEmployee2(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.datasEmployee = reply.data.List;

                    console.log('Berhasil')
                    console.log(vm.dataEmployee)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
                }

            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
console.log(vm.datasEmployee)
            MstService.SelectEmployee(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataEmployee = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log('Berhasil')
                    console.log(vm.dataEmployee)
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
            var email = vm.email.replace(".", "~2b");
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.emp_id + "|" + vm.emp_name + "|" + vm.grup + "|" + vm.title + "|" + email + "|" + vm.cost + "|" + vm.div + "|" + vm.dir + "|" + vm.city + "|" + vm.off + "|" + vm.com + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.emp_id + "|" + vm.emp_name + "|" + vm.grup + "|" + vm.title + "|" + email + "|" + vm.cost + "|" + vm.div + "|" + vm.dir + "|" + vm.city + "|" + vm.off + "|" + vm.com + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            }
            
            MstService.SelectEmployee(data, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataEmployee = reply.data.List;
                    vm.totalRecords = reply.data.Count;
                    console.log('Berhasil')
                    console.log(vm.dataEmployee)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
                }

            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
        }

        vm.updateEmp = updateEmp;
        function updateEmp(param) {
            var modalInstance = $uibModal.open({
                templateUrl: 'updEmp.html',
                backdrop: 'static',
                resolve: {
                    test: function () {
                        return param;
                    }
                },
                controller: function ($uibModalInstance, $scope) {
                    $scope.table_emp_id = param.table_emp_id;
                    $scope.dataUp;
                    $scope.employee_id = param.employee_id;
                    $scope.employee_name = param.employee_name;
                    $scope.title_name = param.title_name;
                    $scope.grup_lvl = param.grup_level;                   
                    $scope.cc = param.costcenter;
                    $scope.division = param.division;
                    $scope.directorate = param.directorate;
                    $scope.city = param.city;
                    $scope.off_loc = param.office_location;
                    $scope.company = param.company;
                    $scope.createDate = dateFormat(new Date(param.created_date));
                    $scope.today = dateFormat(new Date());
                    console.log(param)
                    $scope.employee_name2=param.employee_name;
                    $scope.title_name2 = param.title_name;
                    $scope.grup_lvl2 = param.grup_level;
                    $scope.cc2 = param.costcenter;
                    $scope.division2 = param.division;
                    $scope.directorate2 = param.directorate;
                    $scope.city2 = param.city;
                    $scope.off_loc2 = param.office_location;
                    $scope.company2 = param.company;
                    $scope.dataHis = [];
                    $scope.check = param.is_active;
                    $scope.Vemail = param.email;
                    $scope.Vemail2 = param.email;
                    $scope.employee_id2 = param.employee_id;
                    //MstService.GetEmp(param, function (reply) {
                        
                    //    $scope.dataUp = reply.data;
                    //    $scope.table_emp_id = reply.data.table_emp_id;
                    //    $scope.employee_id = reply.data.employee_id;
                    //    $scope.employee_id2 = reply.data.employee_id;
                    //    $scope.employee_name = reply.data.employee_name;
                    //    $scope.Vemail = reply.data.email;
                    //    $scope.Vemail2 = reply.data.email;
                    //    $scope.title_name = reply.data.title_name;
                    //    $scope.grup_lvl = reply.data.grup_level;                       
                    //    $scope.cc = reply.data.cost_center;
                    //    $scope.division = reply.data.division;
                    //    $scope.directorate = reply.data.directorate;
                    //    $scope.city = reply.data.city;
                    //    $scope.off_loc = reply.data.office_location;
                    //    $scope.company = reply.data.company;
                    //    $scope.employee_name2 = reply.data.employee_name;
                    //    $scope.title_name2 = reply.data.title_name;
                    //    $scope.grup_lvl2 =reply.data.grup_level;
                    //    $scope.cc2 = reply.data.cost_center;
                    //    $scope.division2 = reply.data.division;
                    //    $scope.directorate2 = reply.data.directorate;
                    //    $scope.city2 = reply.data.city;
                    //    $scope.off_loc2 = reply.data.office_location;
                    //    $scope.company2 = reply.data.company;
                    //    $scope.check = reply.data.is_active;
                    //    $scope.data = [];
                    //    console.log($scope.dataUp)
                    //})
                    console.log(param)
                    $scope.location = [];
                    MstService.GetMSDLocationGetCity(function (respon) {
                        $scope.location = respon.data;
                        console.log($scope.location)
                        $scope.cekcitygan($scope.city)
                    })
                    $scope.cekcitygan = function (param) {

                        console.log(param)
                        MstService.cekCity(param, function (response) {
                            $scope.address = response.data;
                            console.log($scope.address)
                        })

                    }
                    
                    $scope.email = function (param) {
                        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                        return re.test(param);
                    }

                    $scope.update = function () {
                        console.log($scope.employee_id)
                        if ($scope.employee_id == "" || $scope.employee_id == undefined) {
                            UIControlService.msg_growl("error", " Employee ID Must be Fill");
                        }else if ($scope.employee_name == "" || $scope.employee_name == undefined) {
                            UIControlService.msg_growl("error", " Employee Name Must be Fill");
                        } else if ($scope.title_name == "" || $scope.title_name == undefined) {
                            UIControlService.msg_growl("error", " Position Must be Fill");
                        } else if ($scope.email($scope.Vemail) != true) {
                            UIControlService.msg_growl("error", " Wrong Email Format");
                        } else if ($scope.grup_lvl == undefined) {
                            UIControlService.msg_growl("error", " Group Level Must be Fill");
                        } else if ($scope.cc == "" || $scope.cc == undefined) {
                            UIControlService.msg_growl("error", " Cost Center Must be Fill");
                        } else if ($scope.division == "" || $scope.division == undefined) {
                            UIControlService.msg_growl("error", " Division Must be Fill");
                        } else if ($scope.directorate == "" || $scope.directorate == undefined) {
                            UIControlService.msg_growl("error", " Directorate Must be Fill");
                        } else if ($scope.city == "" || $scope.city == undefined) {
                            UIControlService.msg_growl("error", " City Must be Fill");
                        } else if ($scope.off_loc == "" || $scope.off_loc == undefined) {
                            UIControlService.msg_growl("error", " Office Location Must be Fill");
                        } else if ($scope.company == "" || $scope.company == undefined) {
                            UIControlService.msg_growl("error", " Company Must be Fill");
                        //} else if ($scope.employee_id == $scope.employee_id2&&$scope.employee_name == $scope.employee_name2 && $scope.title_name == $scope.title_name2 && $scope.grup_lvl == $scope.grup_lvl2 && $scope.cc == $scope.cc2 && $scope.division == $scope.division2 && $scope.directorate == $scope.directorate2 && $scope.city == $scope.city2 && $scope.off_loc == $scope.off_loc2 && $scope.company == $scope.company2 && $scope.Vemail == $scope.Vemail2) {
                        //    UIControlService.msg_growl("error", "No Changes ");
                        }else {

                            if ($('.tes').is(':checked') || $scope.check == 1) {
                                var data = {
                                    tableEmpID: $scope.table_emp_id,
                                    employeeID: $scope.employee_id,
                                    employeeName: $scope.employee_name,
                                    titleName: $scope.title_name,
                                    costCenter: $scope.cc,
                                    city: $scope.city,
                                    office_location: $scope.off_loc,
                                    division: $scope.division,
                                    directorate: $scope.directorate,
                                    company: $scope.company,
                                    gruplevel: $scope.grup_lvl,
                                    isActive: true,
                                    flag: false,
                                    createBy: localStorage.getItem('username'),
                                    createDate: new Date(),
                                    ModifyBy: localStorage.getItem('username'),
                                    LastDate: new Date(),
                                    email : $scope.Vemail
                                    

                                }
                            } else {
                                var data = {
                                    tableEmpID: $scope.table_emp_id,
                                    employeeID: $scope.employee_id,
                                    employeeName: $scope.employee_name,
                                    titleName: $scope.title_name,
                                    costCenter: $scope.cc,
                                    city: $scope.city,
                                    office_location: $scope.off_loc,
                                    division: $scope.division,
                                    directorate: $scope.directorate,
                                    company: $scope.company,
                                    gruplevel: $scope.grup_lvl,
                                    isActive: false,
                                    flag: false,
                                    createBy: localStorage.getItem('username'),
                                    createDate: new Date(),
                                    ModifyBy: localStorage.getItem('username'),
                                    LastDate: new Date(),
                                    email: $scope.Vemail

                                }
                            }
                           
                            var data2 = {
                                employee_id: $scope.employee_id2,
                                email:$scope.Vemail2,
                                employee_name: $scope.employee_name2,
                                tittle_name: $scope.title_name2,
                                cost_center: $scope.cc2,
                                city: $scope.city2,
                                office_location: $scope.off_loc2,
                                division: $scope.division2,
                                directorate: $scope.directorate2,
                                company: $scope.company2,
                                group_level: $scope.grup_lvl2,
                                isActive: false,
                              
                                create_by: localStorage.getItem('username'),
                                create_date: new Date(),
                                modified_by: localStorage.getItem('username'),
                                last_modified: new Date()

                            }
                            $scope.dataHis.push(data2);
                            console.log($scope.dataHis)
                            
                            MstService.updEmp(data, function (reply) {
                                if (reply.status === 200)
                                {
                                    console.log("Berhasil Update");
                                    loadEmployee();
                                } else {

                                }
                                
                            },function (err) {
                                console.info("error:" + JSON.stringify(err));
                                UIControlService.unloadLoading();
                            })
                           
                            MstService.insHisEmp($scope.dataHis, function (respon) {
                                if (respon.status === 200) {
                                    console.log('Berhasil')
                                    UIControlService.msg_growl("success", " Update Success");
                                    $uibModalInstance.dismiss('cancel');
                                   
                                } else {

                                }
                            },function (err) {
                                    console.info("error:" + JSON.stringify(err));
                                    UIControlService.unloadLoading();
                                })
                            //MstService.updEmp({
                            //    employeeID: param,
                            //    is_active: true,
                            //    flag: true,
                            //    ModifyBy: localStorage.getItem('username'),
                            //    LastDate:new Date()
                            //}, function (reply) {
                            //    if (reply.status === 200) {
                            //        console.log("Berhasil Insert");
                            //        $uibModalInstance.dismiss('cancel');
                            //        loadEmployee()
                            //    } else {

                            //    }
                                
                            //},function (err) {
                            //    console.info("error:" + JSON.stringify(err));
                            //    UIControlService.unloadLoading();
                            //})
                           

                        }
              
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                }
            })
        }

        vm.uploadEmp = uploadEmp;
        function uploadEmp() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadCAF.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.filecaf;
                    $scope.data = [];
                    $scope.inputane = [];
                    $scope.all = [];
                    $scope.locationsalah = [];
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
                    $scope.email = function (param) {
                        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                        return re.test(param);
                    }
                    $scope.dataYangAda = 0;
                    $scope.dataSemuaYangAda;
                    $scope.data2=[];
                    $scope.locations = [];
                    $scope.baru = [];
                    $scope.upload = function (file) {
                        ExcelReaderService.readExcel(file, function (reply) {
                            if (reply.status === 200) {
                                var excelContents = reply.data;
                                var sheet1 = excelContents[Object.keys(excelContents)[0]];
                                console.info(sheet1);
                                if (sheet1[1].Column2 != "Employee ID") {
                                    UIControlService.msg_growl("error", "Column 2 Name is " + sheet1[1].Column2);
                                    UIControlService.msg_growl("error", "Must Be Employee ID");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column3 != "Employee Name") {
                                    UIControlService.msg_growl("error", "Column 3 Name is " + sheet1[1].Column3);
                                    UIControlService.msg_growl("error", "Must Be Employee Name");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column4 != "Group Level") {
                                    UIControlService.msg_growl("error", "Column 4 Name is " + sheet1[1].Column4);
                                    UIControlService.msg_growl("error", "Must Be Group Level");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column5 != "Position") {
                                    UIControlService.msg_growl("error", "Column 5 Name is " + sheet1[1].Column5);
                                    UIControlService.msg_growl("error", "Must Be Position");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column6 != "Email") {
                                    UIControlService.msg_growl("error", "Column 6 Name is " + sheet1[1].Column6);
                                    UIControlService.msg_growl("error", "Must Be Email");
                                    $uibModalInstance.dismiss('cancel');
                                }

                                if (sheet1[1].Column7 != "Cost Center") {
                                    UIControlService.msg_growl("error", "Column 7 Name is " + sheet1[1].Column7);
                                    UIControlService.msg_growl("error", "Must Be Cost Center");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column8 != "Division") {
                                    UIControlService.msg_growl("error", "Column 8 Name is " + sheet1[1].Column8);
                                    UIControlService.msg_growl("error", "Must Be Division");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column9 != "Directorate") {
                                    UIControlService.msg_growl("error", "Column 9 Name is " + sheet1[1].Column9);
                                    UIControlService.msg_growl("error", "Must Be Directorate");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column10 != "City") {
                                    UIControlService.msg_growl("error", "Column 10 Name is " + sheet1[1].Column10);
                                    UIControlService.msg_growl("error", "Must Be City");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column11 != "Office Location") {
                                    UIControlService.msg_growl("error", "Column 11 Name is " + sheet1[1].Column11);
                                    UIControlService.msg_growl("error", "Must Be Office Location");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                if (sheet1[1].Column12 != "Company") {
                                    UIControlService.msg_growl("error", "Column 12 Name is " + sheet1[1].Column12);
                                    UIControlService.msg_growl("error", "Must Be Company");
                                    $uibModalInstance.dismiss('cancel');
                                }

                                for (var i = 2; i < sheet1.length; i++) {
                            
                                 
                                 
                                    var item = {
                                        employeeID: sheet1[i].Column2,
                                        employeeName: sheet1[i].Column3,
                                        gruplevel: sheet1[i].Column4,
                                        titleName: sheet1[i].Column5,
                                        email : sheet1[i].Column6,
                                        costCenter: sheet1[i].Column7,
                                        division: sheet1[i].Column8,
                                        directorate: sheet1[i].Column9,
                                        city: sheet1[i].Column10,
                                        office_location: sheet1[i].Column11,
                                        company:sheet1[i].Column12,
                                        createDate: new Date(),
                                        createBy: localStorage.getItem('username'),
                                        LastDate: new Date(),
                                        ModifyBy: localStorage.getItem('username'),
                                        isActive: true,
                                        flag:false
                                        //type:"upload"
                                    };
                                    console.log(item)
                                    
                                    if (!item.employeeID) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Employee ID Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.employeeName) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Employee Name Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    
                                    if (!item.titleName) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Position Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.email) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Email Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if ($scope.email(item.email) != true) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Wrong Email Format");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.costCenter) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Cost Center Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.division) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Division Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.directorate) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Directorate Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.city) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "City Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.office_location) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Office Location Must be Fill");
                                        $scope.data = [];
                                        break;
                                    }
                                    if (!item.company) {
                                        UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
                                        UIControlService.msg_growl("error", "Company Must be Fill");
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
                        //if($scope.dataYangAda > 0  ){
                        //    $scope.checks();
                        //}
                       
                       
                        //$scope.coba();
                        MstService.insEmp($scope.data, function (reply) {
                            console.log(reply)
                            if (reply.data.Count > 0) {
                                for (var i = 0; i < reply.data.Count; i++) {
                                    var words = [];
                                    words = reply.data.DataReturn.split(",");
                                    UIControlService.msg_growl("error", "Employee ID <u><i><b>" + words[i] + "</u></i></b> Already In Database");
                                }
                            } 

                                loadEmployee()
                                UIControlService.msg_growl("success", "Upload Excel Success")
                            


                        },
                   function (error) {
                       UIControlService.msg_growl("error", "NOTIFICATION.UPLOADING.ERROR")
                       UIControlService.unloadLoadingModal();
                   });
                        $uibModalInstance.dismiss('cancel');


                        
                        }
                    $scope.cekloc = function (city,off_loc) {
                        MstService.CekLocation({ city:city, address:off_loc }, function (reply) {
                            console.log(city)
                            console.log(off_loc)
                            console.log(reply)
                            if (reply.status === 200) {
                               
                                $scope.baru.push(reply.data);
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                            $scope.locationsalah.push(city);
                            $scope.locations.push(off_loc);
                            console.log("salah")
                        })
                    }
                    $scope.coba = function () {
                        console.log($scope.locations)
                        console.log($scope.data)
                       
                    }
                 
                    $scope.checks = function () {
                        console.log($scope.data2)
                        console.log($scope.data)
                        console.log($scope.dataYangAda);
                       
                        var a = [], diff = [];

                        for (var i = 0; i < $scope.data2.length; i++) {
                            a[$scope.data2[i]] = true;
                        }

                        for (var i=0;i<$scope.data.length;i++){
                            if(a[$scope.data[i]]){
                                delete a[$scope.data[i]];
                            } else {
                                a[$scope.data2[i]] = true;
                            }
                        }

                        for (var k in a) {
                            diff.push(k);
                            console.log(diff)
                            console.log(a)
                        }
                        console.log(a)
                        console.log(diff)
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
            alasql('SELECT employee_id AS `Employee Id`, employee_name AS `Employee Name`, grup_level AS `Group Level`, title_name AS `Position`,email AS `Email`, costcenter AS `Cost Center`, division AS `Division`, directorate AS `Directorate`, city AS `City`, office_location AS `Office Location`,company AS `Company`,datetime(last_modified) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-employee.xlsx",{headers:true}) FROM ?', [vm.datasEmployee]);
        }

        vm.deleteEmp=deleteEmp;

        function deleteEmp(param) {
            UIControlService.loadLoading("loading")
            MstService.deleteEmp({
                employeeID: param,
                isActive: false,
                flag:true,
                ModifyBy: localStorage.getItem('username'),
                LastDate:new Date()

            }, function (reply) {
                
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    console.log("Berhasil Hapus")
                    UIControlService.msg_growl("success", "InActive Success")
                    loadEmployee();
                    
                } else {
                    UIControlService.unloadLoading();
                    console.log("Tidak Berhasil");
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                
            })
        }
        vm.insEmp = insEmp;
        function insEmp() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insEmp.html',
                backdrop: 'static',
                controller: function ($uibModalInstance, $scope) {
                    $scope.employee_id;
                    $scope.employee_name;
                    $scope.title_name;  
                    $scope.grup_lvl;
                    $scope.position;
                    $scope.cc;
                    $scope.division;
                    $scope.directorate;
                    $scope.city;
                    $scope.off_loc;
                    $scope.company;
                    $scope.data = [];
                    $scope.Vemail;
                    $scope.location = [];
                    $scope.dataYangAda = [];
                    $scope.dataSemuaYangAda = [];
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
                    $scope.email = function (param) {
                        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                        return re.test(param);
                    }
                    $scope.check = function () {
                       
                        MstService.CekSemuaEmp({
                            EmployeeID: $scope.employee_id,
                            employeeName: $scope.employee_name,
                            titleName: $scope.title_name,
                            email: $scope.Vemail,
                            grupLevel: $scope.grup_lvl,
                            costCenter: $scope.cc,
                            division: $scope.division,
                            directorat: $scope.directorate,
                            city: $scope.city,
                            office_location: $scope.off_loc,
                            company: $scope.company

                        }, function (reply) {
                            $scope.dataSemuaYangAda = reply.data.Count;
                            MstService.CekEmpID($scope.employee_id, function (reply) {
                                $scope.dataYangAda = reply.data.Count;
                                $scope.simpan()
                                console.log(reply)
                                console.log($scope.dataYangAda)

                            })
                          

                        })
                       
                    }
                    $scope.simpan = function () {
                      console.log($scope.dataSemuaYangAda)
                        console.log($scope.dataYangAda)
                         if ($scope.employee_id == "" || $scope.employee_id == undefined) {
                            UIControlService.msg_growl("error", " Employee ID Must be Fill");
                        } else if ($scope.employee_name == "" || $scope.employee_name == undefined) {
                            UIControlService.msg_growl("error", " Employee Name Must be Fill");
                        } else if ($scope.title_name == "" || $scope.title_name == undefined) {
                            UIControlService.msg_growl("error", " Position Must be Fill");
                        } else if ($scope.email($scope.Vemail) != true) {
                            UIControlService.msg_growl("error", " Wrong Email Format");
                        } else if ($scope.grup_lvl == undefined) {
                            UIControlService.msg_growl("error", " Group Level Must be Fill");
                        } else if ($scope.cc == "" || $scope.cc == undefined) {
                            UIControlService.msg_growl("error", " Cost Center Must be Fill");
                        } else if ($scope.division == "" || $scope.division == undefined) {
                            UIControlService.msg_growl("error", " Division Must be Fill");
                        } else if ($scope.directorate == "" || $scope.directorate == undefined) {
                            UIControlService.msg_growl("error", " Directorate Must be Fill");
                        } else if ($scope.city == "" || $scope.city == undefined) {
                            UIControlService.msg_growl("error", " City Must be Fill");
                        } else if ($scope.off_loc == "" || $scope.off_loc == undefined) {
                            UIControlService.msg_growl("error", " Office Location Must be Fill");
                        } else if ($scope.company == "" || $scope.company == undefined) {
                            UIControlService.msg_growl("error", " Company Must be Fill");
                            //} else if ($scope.dataSemuaYangAda > 0) {
                            //    UIControlService.msg_growl("error", " Data  Already Used");
                            //} else if($scope.dataYangAda >0){
                            //    UIControlService.msg_growl("error", " Employee ID <u><i><b>" + $scope.employee_id + "</u></i></b> Already Used")
                            //}
                        } else {

                            var dat = {
                                employeeID: $scope.employee_id,
                                employeeName: $scope.employee_name,
                                titleName: $scope.title_name,
                                costCenter: $scope.cc,
                                city: $scope.city,
                                office_location: $scope.off_loc,
                                division: $scope.division,
                                directorate: $scope.directorate,
                                company: $scope.company,
                                gruplevel: $scope.grup_lvl,
                                email: $scope.Vemail,
                                isActive: true,
                                flag: false,
                                createBy: localStorage.getItem('username'),
                                createDate: new Date(),
                                ModifyBy: localStorage.getItem('username'),
                                LastDate: new Date()

                            }
                            $scope.data.push(dat)

                            MstService.insEmp($scope.data, function (reply) {
                                console.log(reply)
                                if (reply.status === 200) {

                                    $uibModalInstance.dismiss('cancel');
                                    loadEmployee();
                                    UIControlService.msg_growl("success", "Insert Success")

                                    console.log("Berhasil Insert");
                                } else {
                                    $.growl.error({ message: "insert Employee Failed" });
                                }
                            }, function (err) {
                                console.info("error:" + JSON.stringify(err));
                            })
                        }
                       
                    }
                
                  

                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    

                }
            });
        };

      

       
    }

})();