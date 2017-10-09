(function () {
    'use strict';

    angular.module("app").controller("CAFCtrl", ctrl);

    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal', 'GlobalConstantService', 'CAFService', '$state', '$stateParams'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal, GlobalConstantService, CAFService, $state, $stateParams) {
        var vm = this;
        vm.request_number = $stateParams.id;
        console.info(vm.request_number);
        vm.coordinator = localStorage.getItem('username');
        vm.roles = localStorage.getItem('roles');
        vm.init = init;
        vm.caf;
        vm.cafProgress;
        function init() {
            if (vm.request_number<1) {
                $state.go('CAF-Dashboard');
            }
            CAFService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                console.info(vm.id_coordinator);
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            UIControlService.loadLoading("Loading");
            vm.jLoad(1);
        }
        vm.incidentDate;
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
        Number.prototype.padLeft = function (base, chr) {
            var len = (String(base || 10).length - String(this).length) + 1;
            return len > 0 ? new Array(len).join(chr || '0') + this : this;
        }
        vm.dateFormatLog = dateFormatLog;
        function dateFormatLog(param) {
            var params = new Date(param);
            var dformat = vm.dateFormat(param) + ' ' +
                  [params.getHours().padLeft(),
                    params.getMinutes().padLeft(),
                    params.getSeconds().padLeft()].join(':');
            return dformat;
        }
        vm.getCoord = getCoord;
        function getCoord(param) {
            CAFService.GetCoordinatorByID(param, function (reply) {
                vm.coord = reply.data;
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
        }
        vm.jLoad = jLoad;
        function jLoad(current) {
            console.info("curr " + current)
            CAFService.GetCAF(vm.request_number,function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.caf = reply.data;
                    console.info(vm.getStatusCAF(reply.data.caf_status_id));
                } else {
                    $.growl.error({ message: "Failed To Get Data CAF" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            });
            CAFService.GetCAFProgress(vm.request_number,function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.cafProgress = reply.data;
                    console.info(vm.cafProgress);
                } else {
                    $.growl.error({ message: "Failed To Get Data CAF Progress" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            });
            UIControlService.unloadLoading();
        }
        vm.back = back;
        function back() {
            $state.go('CAF-Dashboard');
        }
        vm.status;
        vm.cancel = cancel;
        function cancel(id) {
            var modalInstance = $uibModal.open({
                templateUrl: 'remark.html',
                size: 'sm',
                controller: function ($uibModalInstance, $scope) {
                    $scope.getRemark = function () {
                        CAFService.GetRemark(function (reply) {
                            if (reply.status === 200) {
                                $scope.dataRemark = reply.data;
                            } else {
                                $.growl.error({ message: "Failed To Get Remark" });
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                        });
                    };
                    $scope.simpan = function () {
                        if ($scope.remark == undefined) {
                            UIControlService.msg_growl("error", "Remark Must Be Selected");
                        }
                        else {
                            var data = {
                                cafID: id,
                                role: vm.roles,
                                remark: $scope.remark
                            }
                            console.info(data);
                            CAFService.CancelCAF(data, function (reply) {
                                if (reply.status === 200) {
                                    vm.init();
                                    $uibModalInstance.dismiss('cancel');
                                } else {
                                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                                    $uibModalInstance.dismiss('cancel');
                                }
                                UIControlService.unloadLoading();
                            }, function (error) {
                                UIControlService.unloadLoading();
                                UIControlService.msg_growl("error", "ERR_SAVE");
                            });
                        }
                    }
                    $scope.batal = function () {
                        $uibModalInstance.dismiss('cancel');
                    }
                }
            });
        }
        vm.getLog = getLog;
        function getLog(idStatus) {
            CAFService.GetLog(idStatus, function succ(reply) {
                if (reply.status == 200) {
                    console.info(reply.data);
                    vm.log = reply.data;
                    console.info(vm.log);
                }
                else {
                    UIControlService.unloadLoading();
                    $.growl.error({ message: "Failed To Get Data Log of CAF" })
                }
            },
                function err(error) {
                    UIControlService.unloadLoading();
                    console.info("Error: " + JSON.stringify(error));
                }
            );
        }

    }
})();
