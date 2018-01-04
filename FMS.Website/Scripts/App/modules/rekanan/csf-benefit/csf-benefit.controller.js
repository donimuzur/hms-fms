(function () {
    'use strict';

    angular.module("app").controller("CSFBCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$stateParams',
    'UploadFileConfigService'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $stateParams, CSFService,
        UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = this;
        vm.request_number = $stateParams.rq;
        vm.coordinator = localStorage.getItem('username');
        vm.reason_id;
        vm.effective_date;
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        vm.csf_date=dd + '/' + mm + '/' + yyyy;
        vm.employee_id;
        vm.vehicle_type_id;
        vm.location_id;
        vm.expected_date;
        vm.Vtype;
        vm.statusCSF
        vm.Kategori;
        vm.Usage;
        vm.purpose = "test";
        vm.vat = "Test";
        vm.admin;
        vm.selectedVehicle = [];
        vm.manufacturer;
        vm.model;
        vm.series;
        vm.color;
        vm.body_type;
        vm.vcategory
        vm.isCalendarOpened = [false, false, false];
        console.info($stateParams);
        if($stateParams.rq!="$s"){
          vm.admin=true;
        }
        else {
          vm.admin=false;
        }
        console.info(vm.admin);
        vm.init = init
        function init() {
            vm.loadCSF();
        }

        vm.getStatus = getStatus;
        function getStatus(idStatus) {
            CSFService.GetStatus(idStatus,function succ(reply) {
                if (reply.status == 200) {
                    vm.statusCSF = reply.data[0]['status1'];
                    console.info(reply.data);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Gagal mengambil data CSF" })
                }
            },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " +JSON.stringify(error));
                }
            );
        }
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }

        vm.loadCSF = loadCSF;
        function loadCSF()
        {
            console.info(vm.request_number);
            vm.dataCSF = {};
            CSFService.GetCSF(
                vm.request_number,
                function succ(reply) {
                    if (reply.status == 200) {
                        vm.dataCSF = reply.data;
                        console.info(vm.dataCSF);
                    }
                    else {
                        UIControlService.unloadLoading();
                        vm.dataCSF=null;
                        $.growl.error({ message: "Gagal mengambil data CSF" })
                    }
                },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " + JSON.stringify(error));
                    vm.dataCSF = null;
                }
            );
        }
        vm.ambil = 'no';
        vm.noCar = noCar;
        function noCar(param) {
            vm.ambil = param;
            console.info(vm.ambil);
        }
        vm.sendRequest = sendRequest;
        function sendRequest(param) {
            console.info(param);
            if (vm.ambil == 'no') {

            }
            else {
                if (vm.selectedVehicle.length == 0) {
                    UIControlService.msg_growl('warning', 'vehicle belum dipilih!');
                }
                else {
                    CSFService.UpdateCSF({
                        csfID: param,
                        vehicleType: vm.Vtype,
                        vehicleUsage: vm.Usage,
                        locationID: vm.location_id,
                        vehicleCategory: vm.selectedVehicle.car_group_level,
                        manufacturer :vm.manufacturer,
                        series:vm.series,
                        bodyType :vm.body_type,
                        color:vm.color,
                        model:vm.model,
                    }, function (reply) {
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Save Success");
                            vm.init();
                        } else {
                            UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                        }
                        UIControlService.unloadLoading();
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", "ERR_SAVE");
                    });
                }                
            }
        }
        vm.accept = accept;
        vm.decline = decline;
        vm.progress = progress;
        function accept(id) {
            CSFService.CSFApproved({
                csfID: id
            }
            , function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", "Update Success");
                    vm.init();
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
            console.info(id);
        }
        function decline(id) {
            CSFService.CSFRejected({
                csfID : id
            }
            , function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", "Update Success");
                    vm.init();
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
            console.info(id);
        }
        function progress(id) {
            CSFService.CSFInprogress({
                csfID: id
            }
            , function (reply) {
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", "Update Success");
                    vm.init();
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "ERR_SAVE");
            });
            console.info(id);
        }
        vm.save = save;
        function save() {
            var coba = {
                requestNumber: vm.request_number,
                coordinator: vm.Vtype,
                reasonid: vm.Usage,
                effectivedate: vm.alamat,
                employeeid: vm.Kategori,
                vehicletypeid: vm.selectedVehicle.vehicle_id,

            }
            console.info(coba);
            //UIControlService.loadLoading("Silahkan Tunggu");
            //CSFService.SaveCSF({
            //    requestNumber: vm.request_number,
            //    coordinator: vm.coordinator,
                //reasonID: vm.reason_id,
                //effectiveDate: vm.effective_date,
                //employeeID: vm.employee_id,
                //vehicleTypeID: vm.vehicle_type_id,
                //locationID: vm.location_id,
                //expectedDate: vm.expected_date,
                //purpose: vm.purpose,
                //vat: vm.vat,
            //}, function (reply) {
            //    if (reply.status === 200) {
            //        UIControlService.msg_growl("notice", "Save Success");
            //    } else {
            //        UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
            //    }
            //    UIControlService.unloadLoading();
            //}, function (error) {
            //    UIControlService.unloadLoading();
            //    UIControlService.msg_growl("error", "ERR_SAVE");
            //});
        }
        vm.employee;
        vm.openEmployee = openEmployee;
        function openEmployee() {
            var modalInstance = $uibModal.open({
                templateUrl: 'employee.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getEmployee = function () {
                        $scope.employees = {};
                        CSFService.GetEmployee(function (reply) {
                            if (reply.status === 200) {
                                $scope.employees = reply.data;
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data employee" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                        console.info("adjh");
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.select = function (param) {
                        vm.employee_id = param.table_emp_id;
                        vm.employee = param.employee_name;
                        $uibModalInstance.dismiss('cancel');
                    };

                }
            });
        };
        vm.uploadVehicle = uploadVehicle;
        function uploadVehicle() {
            var modalInstance = $uibModal.open({
                templateUrl: 'uploadVehicle.html',
                controller: function ($uibModalInstance, $scope) {
                    
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }
            });
        };
        vm.alamat;
        vm.costCenter = costCenter;
        function costCenter() {
            var modalInstance = $uibModal.open({
                templateUrl: 'csfSelectCity.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getCC = function () {
                        $scope.CostCenter = {};
                        CSFService.GetKota(function (reply) {
                            if (reply.status === 200) {
                                $scope.CostCenter = reply.data;
                                console.info($scope.CostCenter);
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data cc" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });  
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.Kota = function (param,param1,param2) {
                        vm.location_id = param;
                        vm.kota = param1;
                        vm.alamat = param2
                        $uibModalInstance.dismiss('cancel');
                        console.info(vm.kota);
                    };

                }
            });
        };
        vm.selectVehicle = selectVehicle;
        function selectVehicle() {
            console.info(vm.Kategori);
            var modalInstance = $uibModal.open({
                templateUrl: 'csfSelectVehicle.html',
                controller: function ($uibModalInstance, $scope) {
                    $scope.tab = 1;
                    $scope.setTab = function (newTab) {
                        $scope.tab = newTab;
                    };

                    $scope.isSet = function (tabNum) {
                        return $scope.tab === tabNum;
                    };
                    $scope.getVehicle = function () {
                        $scope.DataVehicle = {};
                        $scope.DataFleet = {};
                        CSFService.GetVehicle(vm.Kategori, function (reply) {
                            if (reply.status === 200) {
                                $scope.DataVehicle = reply.data;
                                console.info($scope.DataVehicle);
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data cc" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                        CSFService.GetFleet(function (reply) {
                            if (reply.status === 200) {
                                $scope.DataFleet = reply.data;
                                console.info($scope.DataFleet);
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data cc" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                    };
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.simpan = function (param) {
                        vm.selectedVehicle = [];
                        vm.selectedVehicle = param;
                        vm.manufacturer = param.manufacturer;
                        vm.model = param.model;
                        vm.series = param.series;
                        vm.color = param.color;
                        vm.body_type = param.body_type;
                        vm.vehicle_id = param.vehicle_specs_id;
                        vm.vcategory = param.car_group_level;
                        $uibModalInstance.dismiss('cancel');
                        console.info(vm.selectedVehicle);
                    };
                    $scope.simpanFleet = function (param) {
                        vm.selectedVehicle = [];
                        vm.selectedVehicle = param;
                        vm.manufacturer = param.manufacturer;
                        vm.model = param.model;
                        vm.series = param.series;
                        vm.color = param.color;
                        vm.body_type = param.body_type;
                        vm.vehicle_id = param.vehicle_specs_id;
                        vm.vcategory = param.car_group_level;
                        $uibModalInstance.dismiss('cancel');
                        console.info(vm.selectedVehicle);
                    };

                }
            });
        };
    }
})();
