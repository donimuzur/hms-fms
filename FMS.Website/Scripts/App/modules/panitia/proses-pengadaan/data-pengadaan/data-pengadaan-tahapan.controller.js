(function () {
    'use strict';

    angular.module("app")
    .controller("dataPengadaanTahapanController", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);

        vm.step = [];
        vm.paket;
        vm.namaLelang;
        vm.userBisaMengatur = false;
        //vm.page_id = 37;
        vm.selectedOption;
        //vm.menuhome = 0;
        vm.ditolak = false;
        vm.tahapanDitolak = {};

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-pengadaan');
            loadSteps();
        };

        function loadSteps() {

            vm.ditolak = false;
            /*cek tolak
            $http.post($rootScope.url_api + 'approval/cektolak', {
                paket_lelang_id: vm.idPaket
            }).success(function (reply) {
                console.info("tolak: " + JSON.stringify(reply));
                if (reply.status === 200) {
                    if (reply.result.data.length > 0) {
                        vm.ditolak = true;
                        vm.tahapanDitolak = reply.result.data[0];
                    }
                }
                else {
                    $.growl.error({ message: "Gagal mendapatkan status!" });
                    return;
                }
            }).error(function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                return;
            });
            */

            vm.userBisaMengatur = true;
            /* cek bisa mengatur 
            var arr2 = [];
            arr2.push(vm.idPaket);
            arr2.push($rootScope.userLogin);
            arr2.push(vm.page_id);
            //itp.paket.cekBisaMengatur
            $http.post($rootScope.url_api + 'paket/cekmengatur', {
                param: arr2,
                page_id: vm.page_id
            }).success(function (reply) {
                if (reply.status === 200) {
                    if (reply.result.data.length > 0) {
                        var data = reply.result.data[0].bisa_mengatur;
                        vm.userBisaMengatur = data;
                    } else {
                        vm.userBisaMengatur = false;
                    }
                }
                else {
                    $.growl.error({ message: "Gagal mendapatkan data hak akses!" });
                    return;
                }
            }).error(function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                return;
            });
            */

            //getStatusLelang();
            
            UIControlService.loadLoading(loadmsg);
            DataPengadaanService.GetStep({
                TenderRefID: vm.TenderRefID,
                ProcPackageType: vm.ProcPackType
            }, function (reply) {
                vm.step = reply.data;
                if (vm.step.length > 0) {
                    vm.namaLelang = vm.step[0].tender.TenderName;
                }
                vm.step.forEach(function (fl) {
                    fl.StartDateConverted = convertTanggal(fl.StartDate);
                    fl.EndDateConverted = convertTanggal(fl.EndDate);
                });
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_STEP');
            });
        };

        /*
        function getStatusLelang() {
            var arr = [];
            arr.push(vm.idPaket);
            //itp.paket.selectById
            $http.post($rootScope.url_api + 'paket/byid', {
                param: arr
            }).success(function (reply) {
                if (reply.status === 200) {
                    vm.paket = reply.result.data[0];
                    vm.namaLelang = vm.paket.nama_paket;
                    vm.selectedOption = vm.paket.status_kelangsungan;
                }
            }).error(function (err) {
                $.growl.error({ message: "Gagal Akses API > " + err });
                return;
            });
        }
        */

        function convertTanggal(input) {
            return UIControlService.convertDateTime(input);
        };

        vm.kembali = kembali;
        function kembali() {
            $state.transitionTo('data-pengadaan');
        };
                
        vm.aturTahapan = aturTahapan;
        function aturTahapan(step) {
            var stepItem = {
                ID: step.ID,
                TenderStepName: step.step.TenderStepName,
                StartDate: new Date(Date.parse(step.StartDate)),
                EndDate: new Date(Date.parse(step.EndDate)),
                Status: step.Status
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/proses-pengadaan/data-pengadaan/atur-tahapan.modal.html',
                controller: 'aturTahapanModalController',
                controllerAs: 'atModCtrl',
                resolve: {
                    item: function () {
                        return stepItem;
                    }
                }
            });
            modalInstance.result.then(function() {
                loadSteps();
            });
        };

        vm.menujuTahapan = menujuTahapan;
        function menujuTahapan(step) {
            //TenderRefID/:StepID/:ProcPackType
            $state.transitionTo(step.step.FormTypeURL, { TenderRefID: vm.TenderRefID, StepID: step.ID, ProcPackType: vm.ProcPackType });
        };
    }
})();

