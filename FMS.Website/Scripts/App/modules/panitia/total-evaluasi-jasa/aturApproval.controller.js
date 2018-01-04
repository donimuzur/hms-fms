(function () {
    'use strict';

    angular.module("app")
    .controller("aturApprovalTotalEvalCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TotalEvaluasiJasaService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, TotalEvaluasiJasaService, UIControlService, GlobalConstantService) {

        var vm = this;
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.crApps = [];
        vm.ProjectTitle = "";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');            
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            vm.crApps = [];
            vm.list = [];
            UIControlService.loadLoading(loadmsg);
            TotalEvaluasiJasaService.GetCRApprovals({
                Status: vm.contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.list = reply.data;
                    for (var i = 0; i < vm.list.length; i++) {
                        vm.crApps.push({
                            IsActive: vm.list[i].IsActive,
                            ID: vm.list[i].ID,
                            EmployeeID: vm.list[i].EmployeeID,
                            EmployeeFullName: vm.list[i].MstEmployee.FullName + ' ' + vm.list[i].MstEmployee.SurName,
                            EmployeePositionName: vm.list[i].MstEmployee.PositionName,
                            EmployeeDepartmentName: vm.list[i].MstEmployee.DepartmentName,
                            IsHighPriority: vm.list[i].IsHighPriority
                        });
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
            });
        }

        vm.add = add;
        function add() {
            var item = {
                currentData: vm.crApps
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/total-evaluasi-jasa/aturApproval.modal.html',
                controller: 'selectApproverModalTotalEvalCtrl',
                controllerAs: 'selAppModalTotalEvalCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function (selected) {
                vm.crApps.push({
                    ID: 0,
                    EmployeeID: selected.EmployeeID,
                    EmployeeFullName: selected.FullName + ' ' + selected.SurName,
                    EmployeePositionName: selected.PositionName,
                    EmployeeDepartmentName: selected.DepartmentName,
                    IsHighPriority: false,
                    IsActive: true
                });
            });
        }

        vm.delete = delet;
        function delet(data) {
            data.IsActive = false;
        }

        vm.save = save;
        function save() {
            vm.list = [];
            for (var i = 0; i < vm.crApps.length; i++) {
                var dt = {
                    ID: vm.crApps[i].ID,
                    IsActive: vm.crApps[i].IsActive,
                    EmployeeID: vm.crApps[i].EmployeeID,
                    IsHighPriority: vm.crApps[i].IsHighPriority,
                    TotalEvaluation: vm.contractRequisitionId
                }
                vm.list.push(dt);
            }
            UIControlService.loadLoading(loadmsg);
            TotalEvaluasiJasaService.SaveCRApprovals(vm.list, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_APPROVERS'));
                    init();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_APPROVERS'));
            });
        };

        vm.back = back;
        function back() {
            $state.transitionTo('verifikasi-totalEval');
        };
    }
})();