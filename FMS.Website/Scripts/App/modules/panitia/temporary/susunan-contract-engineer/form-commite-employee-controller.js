(function () {
    'use strict';

    angular.module("app").controller("FormCommitteeEmployeeCtrl", ctrl);
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ContractEngineerService', 'UIControlService', '$uibModalInstance', '$uibModal', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        ContractEngineerService, UIControlService, $uibModalInstance, $uibModal, GlobalConstantService) {

        var vm = this;


        vm.datarekanan = [];
        vm.currentPage = 1;
        vm.totalRecords = 0;
        vm.totalItems = 0;
        vm.user = '';
        vm.activator;
        vm.verificator;
        vm.menuhome = 0;
        vm.cmbStatus = 0;
        vm.rekanan_id = '';
        vm.flag = false;
        vm.fullSize = 10;
        vm.date = "";
        vm.year = "";
        vm.datemonth = "";
        vm.project = "";
        vm.waktuMulai1 = (vm.year - 1) + '-' + vm.datemonth;
        vm.waktuMulai2 = vm.date;

        vm.sStatus = -1;
        vm.thisPage = 12;
        vm.verificationPage = 130;
        vm.verifikasi = {};
        vm.isCalendarOpened = [false, false, false, false];
        //functions
        vm.init = init;
        vm.jLoad = jLoad;
        vm.detailPosition = [];
            var page_id = 141;

            vm.departemen = [];
            vm.totalItems = 0;
            vm.currentPage = 1;
            vm.pageSize = 10;
            vm.userBisaMengatur = false;
            vm.allowAdd = true;
            vm.allowEdit = true;
            vm.allowDelete = true;
            vm.kata = new Kata("");
            vm.init = init;
            vm.jLoad = jLoad;
            vm.searchText = "";

            function init() {
                UIControlService.loadLoading("Silahkan Tunggu...");
                vm.currentPage = 1;
                jLoad(vm.currentPage);
            }

            vm.onSearchSubmit = onSearchSubmit;
            function onSearchSubmit(searchText) {
                vm.searchText = searchText;
                jLoad(vm.current)
            };
            
            vm.jLoad = jLoad;
            function jLoad(current) {
                //console.info("curr "+current)
                vm.detailEmployee = [];
                vm.currentPage = current;
                var offset = (current * 10) - 10;
                ContractEngineerService.selectemployee({
                    Offset: offset,
                    Limit: vm.pageSize,
                    Keyword: vm.searchText
                }, function (reply) {
                    //console.info("data:"+JSON.stringify(reply));
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        var data = reply.data;
                        vm.detailEmployee = data.List;
                        vm.totalItems = Number(data.Count);
                    } else {
                        $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                    //$.growl.error({ message: "Gagal Akses API >" + err });
                    UIControlService.unloadLoading();
                });
            }

        vm.loadPosition = loadPosition;
        function loadPosition() {
            ContractEngineerService.selectposition(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detailPosition = reply.data;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        }

        vm.pegawai = pegawai;
        function pegawai(data) {
            $uibModalInstance.close(data);
        }

        vm.acceptactived = acceptactived;
        function acceptactived() {
            console.info("masuk form add/edit");
            var data = {
                act: true,
                item: vm.isAdd
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/ActivedRejected.html',
                controller: 'FormActivedRejectedCtrl',
                controllerAs: 'FrmActivedRejectedCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                $uibModalInstance.close();
            });
        }
    }
})();
