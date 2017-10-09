(function () {
	'use strict';

	angular.module("app").controller("MSTSpMtCtrl", ctrl);
	ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
         'GlobalConstantService', '$state', '$stateParams', 'MstService',
     'UploadFileConfigService', 'ExcelReaderService', 'UploaderService', '$timeout', '$interval', 'DTOptionsBuilder'];

	function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService, $timeout, $interval, DTOptionsBuilder) {
		var vm = $scope;
		vm.coordinator = localStorage.getItem('username');
		vm.dataSupMeth = [];
		vm.totalRecords = 0;
	
		vm.remark = "";
		vm.modified = "";
		vm.by = "";
		vm.aktive;
		vm.off = 1;
		vm.recat = "";
		vm.dataexcel = [];
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
			vm.loadSupMeth();
		}

		vm.loadSupMeth = loadSupMeth;
		function loadSupMeth() {
		    UIControlService.unloadLoading();
		    var news = vm.roles.replace("_", " ")
		    var newss = news.replace(".", "z")
		    var pageSize = 10;
		    var offset = (vm.off * 10) - 10;
		    if (vm.format(vm.modified) == "NaN-undefined-NaN") {
		        var data = vm.remark + "|" + vm.recat + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
		    } else {
		        var data = vm.remark + "|" + vm.recat + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
		    }
		    MstService.GetSupMethExcel(data, function (reply) {
		        vm.dataexcel = reply.data.List;
		    })
		    MstService.GetSupMeth(data,function (reply) {
		        if (reply.status === 200) {
		            vm.dataSupMeth = reply.data.List;
		            vm.totalRecords = reply.data.Count;
		            console.log("Sukses")
		            console.log(vm.dataSupMeth)

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
		    var format = vm.format(vm.modified);

		    if (vm.format(vm.modified) == "NaN-undefined-NaN") {
		        var data = vm.remark + "|" + vm.recat + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
		    } else {
		        var data = vm.remark + "|" + vm.recat + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
		    }
		    
		    console.log(vm.format(vm.modified))
		    MstService.GetSupMethExcel(data, function (reply) {
		        vm.dataexcel = reply.data.List;
		    })
		    MstService.GetSupMeth(data, function (reply) {
		        if (reply.status === 200) {
		            vm.dataSupMeth = reply.data.List;
		            vm.totalRecords = reply.data.Count;
		            console.log("Sukses")
		            console.log(vm.dataSupMeth)
                    console.log(reply)

		        }
		    }), function (err) {
		        console.info("error:" + JSON.stringify(err));
		        UIControlService.unloadLoading();
		    };
		    console.log("oke Gan");
		}
		vm.paging = paging
		function paging(param) {
		    var news = vm.roles.replace("_", " ")
		    var newss = news.replace(".", "z")
		    var pageSize = 10;
		    var offset = (param * 10) - 10;
            

		    if (vm.format(vm.modified) == "NaN-undefined-NaN") {
		        var data = vm.remark + "|" + vm.recat + "|" + vm.modified + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
		    } else {
		        var data = vm.remark + "|" + vm.recat + "|" + vm.format(vm.modified) + "|" + vm.by + "||" + offset + "|" + pageSize + "|" + newss;
		    }
		    MstService.GetSupMeth(data, function (reply) {
		        if (reply.status === 200) {
		            vm.dataSupMeth = reply.data.List;
		            vm.totalRecords = reply.data.Count;
		            console.log("Sukses")
		            console.log(vm.dataVehicleT)

		        }
		    }), function (err) {
		        console.info("error:" + JSON.stringify(err));
		        UIControlService.unloadLoading();
		    };
		}
		vm.insSM = insSM;
		function insSM() {
		    var modalInstance = $uibModal.open({
		        templateUrl: 'insVhcs.html',
		        backdrop: 'static',
		        controller: function ($uibModalInstance, $scope) {
		            $scope.vcty;
		            $scope.vcus;
		          
		            $scope.data = [];
		            $scope.batal = function () {
		                $uibModalInstance.dismiss('cancel');
		            }
		            $scope.insert = function () {
		                $scope.vcus = $scope.vcus + "";
		                if ($scope.vcty == "" || $scope.vcty == undefined) {
		                    UIControlService.msg_growl("error", "Supply Method Must be Fill");
		                } else if ($scope.vcus == "" || $scope.vcus == undefined) {
		                    UIControlService.msg_growl("error", "Symbol Must be Fill");
		                }  else {
		                    var dat = {
		                        supply_method: $scope.vcty,
                                symbol:$scope.vcus,
		                        created_date: new Date(),
		                        created_by: localStorage.getItem('username'),
		                        last_modified: new Date(),
		                        modified_by: localStorage.getItem('username'),
		                        is_active: true

		                    }
                            $scope.data.push(dat)
                            MstService.insSupMeth($scope.data, function (reply) {
                                console.log(reply)
                                if (reply.data.Count > 0) {
                                    for (var i = 0; i < reply.data.Count; i++) {
                                        var words = [];
                                        words = reply.data.DataReturn.split(",");
                                        UIControlService.msg_growl("error", "Supply Method <u><i><b>" + words[i] + "</u></i></b> Already In Database");
                                        $scope.data = [];
                                    }
                                } else {
                                    if (reply.status === 200) {
                                        $uibModalInstance.dismiss('cancel');
                                        init();
                                        UIControlService.msg_growl("success", "Insert Success")
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
        
		vm.updSM = updSM;
		function updSM(param) {
		    var modalInstance = $uibModal.open({
		        templateUrl: 'updVhcs.html',
		        backdrop: 'static',
		        controller: function ($uibModalInstance, $scope) {
		            $scope.vcty;
		            $scope.vcus;
		            $scope.check;
		            $scope.batal = function () {
		                $uibModalInstance.dismiss('cancel');
		            }
		            MstService.GetSuppMethByID(param, function (reply) {
		                $scope.vcty = reply.data.supply_method1;
		                $scope.vcus = reply.data.symbol;
		                $scope.check = reply.data.is_active;
		            })

		            $scope.update = function () {
		                $scope.vcus = $scope.vcus + "";
		                if ($scope.vcty == "" || $scope.vcty == undefined) {
		                    UIControlService.msg_growl("error", "Supply Method Must be Fill");
		                } else if ($scope.vcus == "" || $scope.vcus == undefined) {
		                    UIControlService.msg_growl("error", "Symbol Must be Fill");
		                } else {
		                    if ($('.tes').is(':checked')||$scope.check == 1) {
		                        var data = {
		                            supply_method_id: param,
		                            supply_method: $scope.vcty,
		                            symbol: $scope.vcus,
		                            created_date: new Date(),
		                            created_by: localStorage.getItem('username'),
		                            last_modified: new Date(),
		                            modified_by: localStorage.getItem('username'),
		                            is_active: true

		                        }
		                    } else {
		                        var data = {
		                            supply_method_id: param,
		                            supply_method: $scope.vcty,
		                            symbol: $scope.vcus,
		                            created_date: new Date(),
		                            created_by: localStorage.getItem('username'),
		                            last_modified: new Date(),
		                            modified_by: localStorage.getItem('username'),
		                            is_active: false

		                        }
		                    }
		                    
		                    MstService.updSupMeth(data, function (reply) {
		                        if (reply.status === 200) {
		                            $uibModalInstance.dismiss('cancel');
		                            init();
		                            UIControlService.msg_growl("success", "Update Success");
		                        } else {
		                            $.growl.error({ message: "Update Data Supply Method Failed" });
		                        }
		                    }, function (err) {
		                        console.info("error:" + JSON.stringify(err));
		                    })
		                }

		            }


		        }
		    })
		}

		vm.uploadSM= uploadSM;
		function uploadSM() {
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
		                        if (sheet1[1].Column2 != "Supply Method") {
		                            UIControlService.msg_growl("error", "Column 2 Name Is " + sheet1[1].Column2);
		                            UIControlService.msg_growl("error", "Must be Supply Method");
		                            $uibModalInstance.dismiss('cancel');
		                        }
		                        if (sheet1[1].Column3 != "Symbol") {
		                            UIControlService.msg_growl("error", "Column 3 Name Is " + sheet1[1].Column3);
		                            UIControlService.msg_growl("error", "Must be Symbol");
		                            $uibModalInstance.dismiss('cancel');
		                        }

		                        for (var i = 2; i < sheet1.length; i++) {
		                            var item = {
		                                supply_method: sheet1[i].Column2,
		                                symbol: sheet1[i].Column3,
		                                created_date: new Date(),
		                                created_by: localStorage.getItem('username'),
		                                type: "upload",
		                                last_modified: new Date(),
		                                modified_by: localStorage.getItem('username'),
		                                is_active: true

		                            };
		                            console.log(item)
		                            if (!item.supply_method) {
		                                UIControlService.msg_growl("error", "Item Line" + (i + 1) + " Not valid:");
		                                UIControlService.msg_growl("error", "Supply Method Must be Fill");
		                                $scope.data = [];
		                                break;
		                            }
		                            if (!item.symbol) {
		                                UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
		                                UIControlService.msg_growl("error", "Symbol Must be Fill");
		                                $scope.data = [];
		                                break;
		                            }
		                            if (item.symbol.length > 5) {
		                                UIControlService.msg_growl("error", "Item Line " + (i + 1) + " Not valid:");
		                                UIControlService.msg_growl("error", "Maximum input length symbol is 5");
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
		                MstService.insSupMeth($scope.data, function (reply) {
		                    if (reply.data.Count > 0) {
		                        for (var i = 0; i < reply.data.Count; i++) {
		                            var words = [];
		                            words = reply.data.DataReturn.split(",");
		                            UIControlService.msg_growl("error", "Supply Method <u><i><b>" + words[i] + "</b></i></u> Already In Database");
		                            $scope.data = [];
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
		    alasql('SELECT supply_method1 AS `Supply Method`,symbol AS `Symbol`, datetime(last_modified) AS `Last Modified`,modified_by AS `Modified By` INTO XLSX("mst-supmeth.xlsx",{headers:true}) FROM ?', [vm.dataexcel]);
		}

		vm.delSM = delSM;
		function delSM(param) {
		    UIControlService.loadLoading("loading");
		    var data = {
		        supply_method_id: param,
		        isActive: false,
		        last_modified: new Date(),
		        ModifyBy: localStorage.getItem('username')
		    };
		    console.log(data)
		    MstService.delSM(data, function (reply) {
		        if (reply.status === 200) {
		            UIControlService.msg_growl("success", "InActive Success");
		            UIControlService.unloadLoading();
		            init();
		        } else {
		            UIControlService.msg_growl("error", "InActive Error");
		        }
		    })
		    }


	}

})();