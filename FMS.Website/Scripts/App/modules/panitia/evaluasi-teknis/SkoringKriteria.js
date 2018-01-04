(function () {
    'use strict';

    angular.module("app").controller("ScoringTechnicalCtrl", ctrl);

    ctrl.$inject = ['$filter','$scope','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationTechnicalService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($filter,$scope, $http, $translate, $translatePartialLoader, $location, SocketService, EvaluationTechnicalService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        var lang = $translate.use();
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        $scope.my_tree = {};
        var page_id = 141;
        vm.evalsafety = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.kata = new Kata("");
        vm.init = init;
        vm.level = 1;
        vm.kriteria = [];
        vm.maxSize = 10;
        vm.srcText = '';
        vm.jLoad = jLoad;
        vm.datascore = {};
        vm.step = [];
        function init() {
            vm.ID = Number($stateParams.ID);
            vm.TenderRefID = Number($stateParams.TenderRefID);
            vm.ProcPackType = Number($stateParams.ProcPackType);
            UIControlService.loadLoading("Silahkan Tunggu...");
            //console.info("init");
            EvaluationTechnicalService.isEvaluator({
                TenderRefID: vm.TenderRefID
            }, function (reply) {
                if (reply.data === true) {
                    jLoad(1);
                } else {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl('error', "Anda bukan evaluator untuk pengadaan ini" );
                    vm.back();
                }
            }, function (err) {
                UIControlService.msg_growl('error', "Anda bukan evaluator untuk pengadaan ini" );
                UIControlService.unloadLoading();
            });

            EvaluationTechnicalService.getVendorInfo({
                VendorID: vm.ID
            }, function (reply) {
                vm.vendorName = reply.data.VendorName;
            }, function (err) {
                UIControlService.msg_growl('error', "Gagal mendapat data Vendor");
            });
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            var step = {
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            EvaluationTechnicalService.selectByEval(step, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                if (reply.status === 200) {
                    vm.step = reply.data;
                    console.info(JSON.stringify(reply.data));

                    vm.evaltechnical = [];
                    vm.currentPage = current;
                    var offset = (current * 10) - 10;
                    var tender = {
                        Status: vm.TenderRefID,
                        FilterType: vm.ProcPackType,
                        column: vm.ID
                    }
                    EvaluationTechnicalService.selectByVendor(tender, function (reply) {
                        //console.info("data:"+JSON.stringify(reply));
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            vm.kriteria = reply.data;

                        } else {
                            UIControlService.msg_growl('error', "Gagal mendapatkan data Evaluasi Teknis");
                            UIControlService.unloadLoading();
                        }
                    }, function (err) {
                        UIControlService.msg_growl('error', "Gagal mendapatkan data Evaluasi Teknis");
                        UIControlService.unloadLoading();
                    });

                } else {
                    UIControlService.msg_growl('error', "Gagal mendapatkan data Evaluasi Teknis");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                UIControlService.msg_growl('error', "Gagal mendapatkan data Evaluasi Teknis");
                UIControlService.unloadLoading();
            });
            
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
                cellTemplate: '<a ng-if="row.branch.children.length === 0" ng-click="cellTemplateScope.click(row.branch)" title="set" class="btn btn-flat btn-xs btn-primary"><i class="fa fa-edit"></i>&nbsp;' + (lang === 'id' ? 'Ubah Skor' : 'Set Score') + '</a>',
                cellTemplateScope: {
                    click: function (data) {
                        vm.ubahdata(data);
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
            },
            {
                field: "obj",
                displayName: "Detail  ",
                cellTemplate: '<a ng-click="cellTemplateScope.click(row.branch)" title="detail" class="btn btn-flat btn-xs btn-danger">&nbsp;' + (lang === 'id' ? 'Detail Skor' : 'Detail Score') + '</a>',
                cellTemplateScope: {
                    click: function (data) {         // this works too: $scope.someMethod;
                        vm.detail(data);
                        
                    }
                }
            }
        ];

        vm.eval = eval;
        function detail(flag) {
            if (flag === 1) {
                $state.transitionTo('detail-evaluasi-safety', { StepID: id, VendorID: vendorID });
            }
            else if (flag === 2) {
                $state.transitionTo('evaluasi-teknis-tim', { TenderRefID: vm.TenderRefID, StepID: vm.StepID, ProcPackType: vm.ProcPackType });
            }
            else if (flag === 3) {
                $state.transitionTo('detail-evaluasi-safety', { StepID: id, VendorID: vendorID });
            }
            else if (flag === 4) {
                $state.transitionTo('detail-evaluasi-safety', { StepID: id, VendorID: vendorID });
            }
        }

        vm.ubahdata = ubahdata;
        function ubahdata(data) {
            var data = {
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/evaluasi-teknis/formSkoring.html',
                controller: 'FormScoringTechnicalCtrl',
                controllerAs: 'FormScoringTechnicalCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

        vm.detail = detail;
        function detail(data) {
            var data = {
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/evaluasi-teknis/formDetailScore.html',
                controller: 'FormDetailScoreCtrl',
                controllerAs: 'FormDetailScoreCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('evaluasi-teknis-tim', { TenderRefID: vm.TenderRefID, StepID: vm.step[0].ID, ProcPackType: vm.ProcPackType });
        }

        vm.form = form;
        function form() {
        }

    }
})();
//TODO

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

