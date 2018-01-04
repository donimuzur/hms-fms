(function () {
    'use strict';

    angular.module("app")
    .controller("KriteriaEvaluasiParentCtrl", ctrl);
    
    ctrl.$inject = ['$scope', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'KriteriaEvaluasiService', 'UIControlService'];
    /* @ngInject */
    function ctrl($scope, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, KriteriaEvaluasiService, UIControlService) {
        var vm = this;
        var lang = $translate.use();
        var loadingMessage = '';

        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.userBisaMengatur = true; //TODO
        vm.menuhome = 0;
        $scope.my_tree = {};
        vm.page_id = 135;
        vm.level = 1;               
        vm.kriteria = [];
        vm.srcText = '';
        vm.loadKriteriaEvaluasi = loadKriteriaEvaluasi;

        function loadKriteriaEvaluasi(){
            //vm.menuhome = $rootScope.menuhome;
            $translatePartialLoader.addPart('master-kriteria-evaluasi');
            $translate.refresh().then(function () {
                loadingMessage = $filter('translate')('MESSAGE.LOADING');
            });
            loadAwal();
        }
        
        /*
        vm.pageSubKriteriaEvaluasi = pageSubKriteriaEvaluasi;
        function pageSubKriteriaEvaluasi(parent){
            $state.transitionTo('sub-kriteria-evaluasi', {level: (level + 1), parent_id: parent});
        }
        */
        
        vm.loadAwal = loadAwal;
        function loadAwal(){
            vm.loadKriteria();
        }
        
        vm.cari = cari;
        function cari(srcText) {
            vm.srcText = srcText;
            vm.currentPage = 1;
            vm.loadKriteria();
        }

        vm.loadKriteria = loadKriteria;
        function loadKriteria() {
            KriteriaEvaluasiService.countCriteria({
                keyword: vm.srcText,
                level: 1,
                parentId: 0
            }, function (reply) {
                if (reply.status === 200) {
                    vm.totalItems = reply.data;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
            });

            vm.kriteria = [];
            KriteriaEvaluasiService.selectCriteria({
                keyword: vm.srcText,
                level: 1,
                parentId: 0,
                offset: (vm.currentPage - 1) * vm.maxSize,
                limit:  vm.maxSize
            }, function (reply) {
                if (reply.status === 200) {
                    var data = reply.data;                    
                    if (data.length > 0) {
                        data.forEach(function (obj) {
                            obj.children = [];
                            obj.bisaNgatur = vm.userBisaMengatur;
                            vm.kriteria.push({
                                "CriteriaId": obj.CriteriaId,
                                "CriteriaName": obj.CriteriaName,
                                "Level": obj.Level,
                                "IsActive": obj.IsActive,
                                "ParentId": obj.ParentId,
                                "IsMandatory": obj.IsMandatory,
                                "children": obj.children,
                                "IsOptionScoreFixed": obj.IsOptionScoreFixed,
                                //"obj": obj,
                                "bisaNgatur": obj.bisaNgatur
                            });
                            vm.selectSubKriteria(obj);
                        });
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
            });
        };

        vm.selectSubKriteria = selectSubKriteria;
        function selectSubKriteria(object) {
            var level2 = object.Level + 1;
            var parent2 = object.CriteriaId;
            KriteriaEvaluasiService.selectCriteria({
                keyword: "",
                level: level2,
                parentId: parent2,
                offset: 0,
                limit: 0 //no limit
            }, function (reply) {
                //console.info(object.kriteria_id+"sel-sub: "+JSON.stringify(reply));
                if (reply.status === 200) {
                    var data = reply.data;
                    if (data.length > 0) {
                        data.forEach(function (objChild) {
                            objChild.children = [];
                            objChild.bisaNgatur = vm.userBisaMengatur;
                            vm.kriteria.forEach(function (objParent) {
                                if (objChild.ParentId === objParent.CriteriaId) {
                                    objParent.children.push({
                                        "CriteriaId": objChild.CriteriaId,
                                        "CriteriaName": objChild.CriteriaName,
                                        "Level": objChild.Level,
                                        "IsActive": objChild.IsActive,
                                        "ParentId": objChild.ParentId,
                                        "IsMandatory": objChild.IsMandatory,
                                        "children": objChild.children,
                                        "IsOptionScoreFixed": objChild.IsOptionScoreFixed,
                                        //"obj": objChild,
                                        "bisaNgatur": objChild.bisaNgatur
                                    });
                                }
                            });
                            var level3 = objChild.Level + 1;
                            var parent3 = objChild.CriteriaId;
                            KriteriaEvaluasiService.selectCriteria({
                                keyword: "",
                                level: level3,
                                parentId: parent3,
                                offset: 0,
                                limit: 0 //no limit
                            }, function (reply1) {
                                if (reply1.status === 200) {
                                    var data1 = reply1.data;
                                    if (data1.length > 0) {
                                        data1.forEach(function (objGrndChild) {
                                            objGrndChild.children = [];
                                            objGrndChild.bisaNgatur = vm.userBisaMengatur;
                                            for (var i = 0; i < vm.kriteria.length; i++) {
                                                if (vm.kriteria[i].children.length > 0) {
                                                    for (var j = 0; j < vm.kriteria[i].children.length; j++) {
                                                        if (objGrndChild.ParentId === vm.kriteria[i].children[j].CriteriaId) {
                                                            vm.kriteria[i].children[j].children.push({
                                                                "CriteriaId": objGrndChild.CriteriaId,
                                                                "CriteriaName": objGrndChild.CriteriaName,
                                                                "Level": objGrndChild.Level,
                                                                "IsActive": objGrndChild.IsActive,
                                                                "ParentId": objGrndChild.ParentId,
                                                                "IsMandatory": objGrndChild.IsMandatory,
                                                                "children": objGrndChild.children,
                                                                "IsOptionScoreFixed": objGrndChild.IsOptionScoreFixed,
                                                                //"obj": objGrndChild,
                                                                "bisaNgatur": objGrndChild.bisaNgatur
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                            });
                        });
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
            });
        };

        vm.expanding_property = {
            field: "CriteriaName",
            displayName: lang === 'id' ? 'Kriteria Evaluasi' : 'Evaluation Criteria',
            sortable: false,
            sortingType: "string",
            filterable: true
        };

        vm.col_defs = [
            {
                field: "obj",
                displayName: "  ",
                cellTemplate: '<a ng-show="row.branch.bisaNgatur && !(row.branch.IsMandatory && row.branch.ParentId != 0) && row.branch.level < 3" class="btn btn-flat btn-xs btn-primary" ng-click="cellTemplateScope.click(row.branch)" title="' + (lang === 'id' ? 'Tambah Sub Kriteria' : 'Add Sub Criteria...') + '"><i class="fa fa-plus-circle"></i>&nbsp; ' + '</a>',
                cellTemplateScope: {
                    click: function(data) {         // this works too: $scope.someMethod;
                        vm.addMasterKriteria(data.Level, data.CriteriaId);
                    }
                }
            },
            {
                field: "obj",
                displayName: "  ",
                cellTemplate: '<a ng-show="row.branch.IsMandatory === false" ng-click="cellTemplateScope.click(row.branch)" title="' + (lang === 'id' ? 'Ubah' : 'Edit') + '" class="btn btn-flat btn-xs btn-success"><i class="fa fa-edit"></i>&nbsp; ' + '</a>',
                cellTemplateScope: {
                    click: function(data) {         // this works too: $scope.someMethod;
                        vm.ubah(data);
                    }
                }
            },
            {
                field: "obj",
                displayName: "  ",
                cellTemplate: '<a ng-show="row.branch.IsMandatory === false" ng-click="cellTemplateScope.click(row.branch)" title="'+ (lang === 'id' ? 'Hapus' : 'Delete') +'" class="btn btn-flat btn-xs btn-danger"><i class="fa fa-trash-o"></i>&nbsp;' + '</a>',
                cellTemplateScope: {
                    click: function(data) {         // this works too: $scope.someMethod;
                        vm.hapus(data);
                    }
                }
            }
        ];
        
        vm.addMasterKriteria = addMasterKriteria;
        function addMasterKriteria(level, id) {
            var lempar = {level: level + 1, parentId: id};
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/kriteria-evaluasi/formKriteriaEvaluasi.html',
                controller: 'formKriteriaEvaluasiCtrl',
                controllerAs: 'formKriteriaEvaluasiCtrl',
                resolve: {
                    item: function() {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function() {
                vm.loadKriteria();
            });
        };

        vm.ubah = ubah;
        function ubah(data) {
            var lempar = {
                criteriaId: data.CriteriaId,
                criteriaName: data.CriteriaName,
                level: data.Level,
                isOptionScoreFixed: data.IsOptionScoreFixed
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/kriteria-evaluasi/formKriteriaEvaluasi.html',
                controller: 'formKriteriaEvaluasiCtrl',
                controllerAs: 'formKriteriaEvaluasiCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadKriteria();
            });
        };

        vm.hapus = hapus;
        function hapus(data) {
            var pesan = "";

            switch (lang) {
                case 'id' : pesan = 'Anda yakin untuk menghapus Kriteria/Sub-Kriteria "' + data.CriteriaName + '"?'; break;
                default : pesan = 'Are you sure want to remove "' + data.CriteriaName + '" from criteria/subcriteria list?'; break;
            }

            bootbox.confirm(pesan, function (yes) {
                if (yes) {
                    UIControlService.loadLoading(loadingMessage);
                    KriteriaEvaluasiService.deleteCriteria({
                        criteriaId: data.CriteriaId
                    }, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.SUCC_DEL'));
                            vm.loadKriteria();
                        } else {
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_DEL'));
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_DEL'));
                    });
                }
            })
        };
    }
})();
        
