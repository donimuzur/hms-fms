(function () {
    'use strict';

    angular.module("app")
    .controller("FormDetailScoreCtrl", ctrl);

    ctrl.$inject = ['$scope','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationTechnicalService', '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item'];
    function ctrl($scope, $http, $translate, $translatePartialLoader, $location, SocketService, EvaluationTechnicalService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item) {
        var vm = this;
        vm.detail = item.item;
        var lang = $translate.use();
        $scope.my_tree = {};
        vm.kriteria = [];
        vm.init = init;
        function init() {
            vm.kriteria.push(vm.detail);
            console.info(JSON.stringify(vm.detail));
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
            //$uibModalInstance.close();
        };

        vm.save = save;
        function save() {
            vm.data = [];
            var data = {
                ID: vm.detail.ID,
                EvalMDCriteria: vm.detail.EvalMDCriteria,
                Weight: vm.detail.Weight,
                Score: vm.detail.Score,
                EvalTechID: vm.detail.EvalTechID,
                VendorID: vm.detail.VendorID,
                TenderStepDataID: vm.detail.TenderStepDataID
            }
            vm.data.push(data);
            console.info(JSON.stringify(vm.detail));
            
          //  EvaluationTechnicalService.Insert(vm.data,
          //     function (reply) {
          //         UIControlService.unloadLoadingModal();
          //         if (reply.status === 200) {
          //             UIControlService.msg_growl("success", "Berhasil Simpan data");
          //             $uibModalInstance.close();
          //         }
          //         else {
          //             UIControlService.msg_growl("error", "Gagal menyimpan data!!");
          //             return;
          //         }
          //     },
          //     function (err) {
          //         UIControlService.msg_growl("error", "Gagal Akses Api!!");
          //         UIControlService.unloadLoadingModal();
          //     }
          //);
        }

        vm.expanding_property = {
            field: "CriteriaName",
            displayName: lang === 'id' ? 'Evaluasi Teknis' : 'Evaluation Technical',
            sortable: false,
            sortingType: "string",
            filterable: true
        };

        vm.col_defs = [
            {
                field: "obj",
                displayName: " Weight  ",
                cellTemplate: '<label ng-bind="row.branch.weight" />',
                cellTemplateScope: {
                    click: function (data) {
                    }
                }
            },
            {
                field: "obj",
                displayName: " Score  ",
                cellTemplate: '<label ng-show="!row.branch.children || row.branch.children.length === 0" ng-bind="row.branch.Score" />',
                cellTemplateScope: {
                    click: function (data) {
                        vm.ubahdata(data);
                    }
                }
            },
            {
                field: "obj",
                displayName: " Must Have  ",
                cellTemplate: '<label ng-show="!row.branch.children || row.branch.children.length === 0" ng-bind="row.branch.MustHave" />',
                cellTemplateScope: {
                    click: function (data) {
                    }
                }
            },
            {
                field: "obj",
                displayName: " Sub Total  ",
                cellTemplate: '<label ng-bind="row.branch.SubTotal.toFixed(2)" />',
                cellTemplateScope: {
                    click: function (data) {
                    }
                }
            }
        ];


    }
})();;