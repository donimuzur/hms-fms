(function () {
    'use strict';

    angular.module("app").controller("CPRFormCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'CPRService',
        '$state', 'UIControlService', 'UploadFileConfigService', 'UploaderService', 'GlobalConstantService',
        '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, CPRService,
        $state, UIControlService, UploadFileConfigService, UploaderService, GlobalConstantService,
        $uibModal, $stateParams) {
        var vm = this;
        vm.skor = "";
        vm.catatan = "";
        vm.kriteria = "";
        vm.listKuantitas = [];
        vm.tanggal = "";
        vm.noKontrak = "";
        vm.sponsor = "";
        vm.proManager = "";
        vm.conEngineer = "";
        vm.deptOwner = "";
        vm.vendor = "";
        vm.plan = "";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("cpr");
            loadData();
        }

        vm.loadData = loadData;
        function loadData() {
            vm.kpiList = [
            {
                kpi_name: "Pengelolaan Proyek", kpi_sub: [{
                    sub_name: "Kuantitas",
                    detail_nilai: [{ skor: 1, det_skor: "Below Plan" }, { skor: 2, det_skor: "On Plan" },
                    { skor: 3, det_skor: "Above Plan" }]
                },
                   {
                       sub_name: "Mutu",
                       detail_nilai: [{ skor: 1, det_skor: "Not Suitable" }, { skor: 2, det_skor: "Suitable" }]
                   }
                ]
            },
            {
                kpi_name: "Pengelolaan Sumber Daya", kpi_sub: [{
                    sub_name: "Tenaga Kerja",
                    detail_nilai: [{ skor: 1, det_skor: "Below Plan" }, { skor: 2, det_skor: "On Plan" },
                    { skor: 3, det_skor: "Above Plan" }]
                }]
            },
            { kpi_name: "Keselamatan dan Kesehatan Kerja" },
            { kpi_name: "Pengelolaan Proyek" },
            { kpi_name: "Organisasi dan Perhatian Terhadap Persyaratan" }
            ];
        }

        vm.addItem = addItem;
        function addItem() {
            var data = {
                kriteria : vm.kriteria, skor: vm.skor, catatan: vm.catatan
                };
            vm.listKuantitas.push(data);
        }
    }
})();