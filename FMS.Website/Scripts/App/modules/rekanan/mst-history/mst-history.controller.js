(function () {
    'use strict';

    angular.module("app").controller("MSTHECtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'MstService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval) {
        var vm = this;
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
        vm.coordinator = localStorage.getItem('username');
        vm.dataHisEmp = [];
        vm.totalRecords = 0;
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
        vm.init = init;
        vm.roles = localStorage.getItem('roles')
        function init() {

            MstService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                console.info(vm.id_coordinator);
                console.log('coba')
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.loadEmployee();
        }

        vm.toExcel = toExcel;
        function toExcel(tabel) {
            alasql.fn.datetime = function (dateStr) {
                var date = vm.dateFormat(new Date(parseInt(dateStr.substring(6, 19))));
                return date.toLocaleString();
            };
            alasql('SELECT employee_id AS `Employee Id`, employee_name AS `Employee Name`, grup_level AS `Group Level`, title_name AS `Title Name`,email AS `Email`, costcenter AS `Cost Center`, division AS `Division`, directorate AS `Directorate`, city AS `City`, office_location AS `Office Location`,company AS `Company`,datetime(last_modified) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-employee.xlsx",{headers:true}) FROM ?', [vm.datasEmployee]);
        }

        vm.loadEmployee = loadEmployee;
        function loadEmployee() {
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

            MstService.SelectEmployeeHistory2(data, function (reply) {
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
            MstService.SelectEmployeeHistory(data, function (reply) {
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
        vm.search = search;
        function search() {
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
            MstService.SelectEmployeeHistory2(data, function (reply) {
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
            MstService.SelectEmployeeHistory(data, function (reply) {
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
            
            var email = vm.email.replace(".", "~2b");
            if (vm.format(vm.modified) == "NaN-undefined-NaN") {
                var data = vm.emp_id + "|" + vm.emp_name + "|" + vm.grup + "|" + vm.title + "|" + email + "|" + vm.cost + "|" + vm.div + "|" + vm.dir + "|" + vm.city + "|" + vm.off + "|" + vm.com + "|" + vm.modified + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            } else {
                var data = vm.emp_id + "|" + vm.emp_name + "|" + vm.grup + "|" + vm.title + "|" + email + "|" + vm.cost + "|" + vm.div + "|" + vm.dir + "|" + vm.city + "|" + vm.off + "|" + vm.com + "|" + vm.format(vm.modified) + "|" + vm.by + "|" + offset + "|" + pageSize + "|" + newss;
            }
            MstService.SelectEmployeeHistory(data, function (reply) {
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
    }
})();