﻿(function () {
    'use strict';

    angular.module("app")
    .controller("detailEvaluasiPrakual", ctrl);
    
    ctrl.$inject = ['$http', '$state', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'MetodeEvaluasiPrakualService', 'KriteriaEvaluasiService', 'UIControlService'];
    /* @ngInject */
    function ctrl($http, $state, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, metodeEvaluasiPrakualService, KriteriaEvaluasiService, UIControlService) {

        var vm = this;
        var EMDid = Number($stateParams.id);
        
        vm.sudahMengaturLevel1;
        vm.userBisaMengatur = true; //TODO
        vm.kriteria = [];
        vm.namaMetode;
        vm.menuhome = 0;
        var evaluationMethodId;
        vm.evaluationMethodId;
        var page_id = 140;
        vm.page_id = 140;
        vm.obj = {};
        vm.sudahDipakai = false;

        vm.init = init;
        function init() {
            vm.initialize();
        };

        vm.initialize = initialize;
        function initialize() {
            vm.loadLevel1();
        };

        vm.expanding_property = {
            field: "kriteria_nama",
            displayName: "Nama Kriteria Evaluasi",
            sortable: true,
            sortingType: "string",
            filterable: true
        };

        vm.col_defs = [
            {
                field: "bobot",
                displayName: "Bobot (%)",
                cellTemplate: '<a style="width: 10%"> {{row.branch.bobot}}&nbsp;% </a> ',
                sortable: true,
                sortingType: "number",
                filterable: true
            },
            {
                field: "obj",
                displayName: "  ",
                cellTemplate: '<a ng-show="row.branch.bisaNgatur === true && row.branch.level < 3" class="btn btn-flat btn-xs btn-primary" ng-click="cellTemplateScope.click(row.branch)"><i class="fa fa-plus-circle"></i>&nbsp; Ubah Sub Kriteria</a>',
                cellTemplateScope: {
                    click: function (data) {         // this works too: vm.someMethod;
                        vm.tambahDetailLevel1(data);
                    }
                }
            }
        ];

        vm.loadLevel1 = loadLevel1;
        function loadLevel1() {
            vm.kriteria = [];
            metodeEvaluasiPrakualService.selectDetailById({
                EMDid: EMDid
            }, function (reply) {
                if (reply.status === 200) {
                    var metode_evaluasi = reply.data.EvaluationMethod;
                    vm.namaMetode = metode_evaluasi.EvaluationMethodName;
                    evaluationMethodId = metode_evaluasi.EvaluationMethodId;
                    vm.evaluationMethodId = metode_evaluasi.EvaluationMethodId;
                    metodeEvaluasiPrakualService.isUsed({
                        EvaluationMethodId: evaluationMethodId
                    }, function (reply) {
                        if (reply.status === 200) {
                            vm.sudahDipakai = reply.data;
                        }
                    }, function (err) {
                        UIControlService.msg_growl('error', 'Gagal Akses API');
                    });
                }
            }, function (err) {
                UIControlService.msg_growl('error','Gagal Akses API');
            });

            metodeEvaluasiPrakualService.selectDetailCriteria({
                EMDId: EMDid,
                Level: 1,
                Parent: 0
            }, function (reply) {
                if (reply.status === 200) {
                    vm.sudahMengaturLevel1 = reply.data.length > 0;
                    if (vm.sudahMengaturLevel1 === false) {
                        KriteriaEvaluasiService.selectCriteria({
                            keyword: '',
                            level: 1,
                            parentId: 0,
                            offset: 0,
                            limit: 0
                        }, function (reply2) {
                            if (reply2.status === 200) {
                                vm.kriteria = reply2.data;
                                for (var i = 0; i < vm.kriteria.length; i++) {
                                    vm.kriteria[i].checked = false;
                                    vm.kriteria[i].Weight = 0;
                                    vm.kriteria[i].EMDId = EMDid;
                                }
                            }
                        });
                    }
                    else if (vm.sudahMengaturLevel1 === true) {
                        reply.data.forEach(function (obj) {
                            obj.children = [];
                            obj.bisaNgatur = vm.userBisaMengatur;
                            vm.kriteria.push({
                                "med_kriteria_id": obj.Id,
                                "metode_evaluasi_id": obj.EvaluationMethodId,
                                "metode_evaluasi_nm": obj.EvaluationMethodName,
                                "med_id": obj.EMDId,
                                "jenis_detail": obj.DetailType,
                                "kriteria_id": obj.CriteriaId,
                                "kriteria_nama": obj.CriteriaName,
                                "is_mandatory": obj.IsMandatory,
                                "bobot": obj.Weight,
                                "level": obj.Level,
                                "is_active": obj.IsActive,
                                "parent": obj.Parent,
                                "status_parent": obj.ParentStatus,
                                "grandparent_id": obj.GrandparentId,
                                "status_grandparent": obj.GrandparentStatus,
                                "children": obj.children,
                                //"obj": obj,
                                "bisaNgatur": obj.bisaNgatur
                            });
                            vm.selectSubKriteria(obj);
                        });
                    }
                }
            }, function (err) {
                UIControlService.msg_growl('error', 'Gagal Akses API');
            });
        };

        vm.selectSubKriteria = selectSubKriteria
        function selectSubKriteria(object) {
            metodeEvaluasiPrakualService.selectDetailCriteria({
                EMDId: EMDid,
                Level: object.Level + 1,
                Parent: object.CriteriaId
            }, function (reply) {
                if (reply.status === 200) {
                    if (reply.data.length > 0) {
                        reply.data.forEach(function (objChild) {
                            objChild.children = [];
                            objChild.bisaNgatur = vm.userBisaMengatur;
                            vm.kriteria.forEach(function (objParent) {
                                if (objChild.parent === objParent.CriteriaId) {
                                    objChild.children = [];
                                    objChild.bisaNgatur = vm.userBisaMengatur;
                                    objParent.children.push({
                                        "med_kriteria_id": objChild.Id,
                                        "metode_evaluasi_id": objChild.EvaluationMethodId,
                                        "metode_evaluasi_nm": objChild.EvaluationMethodName,
                                        "med_id": objChild.EMDId,
                                        "jenis_detail": objChild.DetailType,
                                        "kriteria_id": objChild.CriteriaId,
                                        "kriteria_nama": objChild.CriteriaName,
                                        "is_mandatory": objChild.IsMandatory,
                                        "bobot": objChild.Weight,
                                        "level": objChild.Level,
                                        "is_active": objChild.IsActive,
                                        "parent": objChild.Parent,
                                        "status_parent": objChild.ParentStatus,
                                        "grandparent_id": objChild.GrandparentId,
                                        "status_grandparent": objChild.GrandparentStatus,
                                        "children": objChild.children,
                                        //"obj": obj,
                                        "bisaNgatur": objChild.bisaNgatur
                                    });
                                }
                            });
                            metodeEvaluasiPrakualService.selectDetailCriteria({
                                EMDId: EMDid,
                                Level: objChild.Level + 1,
                                Parent: objChild.CriteriaId
                            }, function (reply2) {
                                if (reply2.status === 200) {
                                    if (reply2.data.length > 0) {
                                        reply2.data.forEach(function (objGrndChild) {
                                            objGrndChild.children = [];
                                            objGrndChild.bisaNgatur = vm.userBisaMengatur;
                                            for (var i = 0; i < vm.kriteria.length; i++) {
                                                if (vm.kriteria[i].children.length > 0) {
                                                    for (var j = 0; j < vm.kriteria[i].children.length; j++) {
                                                        if (objGrndChild.Parent === vm.kriteria[i].children[j].kriteria_id) {
                                                            vm.kriteria[i].children[j].children.push({
                                                                "med_kriteria_id": objGrndChild.Id,
                                                                "metode_evaluasi_id": objGrndChild.EvaluationMethodId,
                                                                "metode_evaluasi_nm": objGrndChild.EvaluationMethodName,
                                                                "med_id": objGrndChild.EMDId,
                                                                "jenis_detail": objGrndChild.DetailType,
                                                                "kriteria_id": objGrndChild.CriteriaId,
                                                                "kriteria_nama": objGrndChild.CriteriaName,
                                                                "is_mandatory": objGrndChild.IsMandatory,
                                                                "bobot": objGrndChild.Weight,
                                                                "level": objGrndChild.Level,
                                                                "is_active": objGrndChild.IsActive,
                                                                "parent": objGrndChild.Parent,
                                                                "status_parent": objGrndChild.ParentStatus,
                                                                "grandparent_id": objGrndChild.GrandparentId,
                                                                "status_grandparent": objGrndChild.GrandparentStatus,
                                                                "children": objGrndChild.children,
                                                                //"obj": obj,
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
                }
            });
        };

        vm.simpan = simpan
        function simpan() {
            if (vm.kriteria.length === 0) {
                UIControlService.msg_growl('error',"Tidak ada kriteria yang bisa dimasukkan");
                return;
            }
            for (var i = 0; i < vm.kriteria.length; i++) {
                if (vm.kriteria[i].checked && !(vm.kriteria[i].Weight > 0)) {
                    UIControlService.msg_growl('error', "Bobot harus diisi antara 0 - 100");
                    return;
                }
            }
            var totalPersentase = 0;
            for (var i = 0; i < vm.kriteria.length; i++) {
                if (vm.kriteria[i].checked) {
                    totalPersentase = totalPersentase + Number(vm.kriteria[i].Weight);
                }
            }
            if (totalPersentase !== 100) {
                UIControlService.msg_growl('error', "Total persentase tidak 100%");
                return;
            }
            var detail = [];
            for (var i = 0; i < vm.kriteria.length; i++) {
                vm.kriteria[i].Parent = vm.kriteria[i].ParentId; //Beda Nama Kolom di DB
                vm.kriteria[i].IsActive = vm.kriteria[i].checked;
                vm.kriteria[i].Weight = vm.kriteria[i].checked ? vm.kriteria[i].Weight : 0;
                detail.push(vm.kriteria[i]);
            }
            UIControlService.loadLoading("Silahkan Tunggu...");
            metodeEvaluasiPrakualService.saveDetailCriteria(detail,
                function (reply) {
                    if (reply.status === 200) {
                        vm.kriteria = [];
                        vm.loadLevel1();
                        UIControlService.msg_growl('notice', "Berhasil mengatur level 1");
                        UIControlService.unloadLoading();

                    }
                    else {
                        UIControlService.msg_growl('error', "Gagal mengatur level 1");
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    UIControlService.msg_growl('error','Gagal Akses API');
                }
            );
        };

        vm.back = back
        function back() {
            $state.transitionTo('tambah-metode-prakual', { id: evaluationMethodId });
        };

        vm.ubahDetailLevel1 = ubahDetailLevel1
        function ubahDetailLevel1() {
            var lempar = {
                data: vm.kriteria,
                med_id: EMDid,
                page_id: page_id,
                level: 1,
                parent: 0,
                nama: '',
                sudahDipakai: vm.sudahDipakai
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/prakualifikasi/metode-evaluasi/metodeEvaluasiLevel1.modal.html',
                controller: 'detailEvaluasiPrakualModal',
                controllerAs: 'depmCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadLevel1();
            });
        };

        vm.tambahDetailLevel1 = tambahDetailLevel1;
        function tambahDetailLevel1(obj) {
            var data = [];
            if (obj.level == 0) {
                data = vm.kriteria;
            } else if (obj.level == 1) {
                for (var i = 0; i < vm.kriteria.length; i++) {
                    if (vm.kriteria[i].kriteria_id == obj.kriteria_id) {
                        data = vm.kriteria[i].children;
                    }
                }
            } else {
                for (var i = 0; i < vm.kriteria.length; i++) {
                    for (var j = 0; j < vm.kriteria[i].children.length; j++) {
                        if (vm.kriteria[i].children[j].kriteria_id == obj.kriteria_id) {
                            data = vm.kriteria[i].children[j].children;
                        }
                    }
                }
            }
            var lempar = {
                data: data,
                med_id: EMDid,
                page_id: page_id,
                level: Number(obj.level) + 1,
                parent: obj.kriteria_id,
                nama: obj.kriteria_nama,
                sudahDipakai: vm.sudahDipakai
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/prakualifikasi/metode-evaluasi/metodeEvaluasiLevel1.modal.html',
                controller: 'detailEvaluasiPrakualModal',
                controllerAs: 'depmCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadLevel1();
            });
        };
    }
})();