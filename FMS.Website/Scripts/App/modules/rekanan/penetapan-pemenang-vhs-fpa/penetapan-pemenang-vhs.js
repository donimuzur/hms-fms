(function () {
    'use strict';

    angular.module("app").controller("PPGVHSVCtrl", ctrl);

    ctrl.$inject = ['$timeout','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PenetapanPemenangVHSservice', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($timeout, $http, $translate, $translatePartialLoader, $location, SocketService, PenetapanPemenangVHSservice,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        var page_id = 141;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
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
        vm.exportHref;
        vm.detail = [];
        vm.jLoad = jLoad;

        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);
            loadStep();

        }
        vm.jLoad = jLoad;
        function jLoad(current) {
            var tender = {
                column: vm.StepID,
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            PenetapanPemenangVHSservice.selectVendor(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detail = reply.data;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Peneapan Pemenang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadStep = loadStep;
        function loadStep() {
            var tender = {
                column: vm.StepID,
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            PenetapanPemenangVHSservice.SelectStep(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.step = reply.data;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Peneapan Pemenang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadExcelVendor = loadExcelVendor;
        function loadExcelVendor(ID) {
            vm.vendor = [];
            var tender = {
                column: ID,
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            PenetapanPemenangVHSservice.excelvendor(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.vendor = reply.data;
                    console.info("data:" + JSON.stringify(vm.vendor));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Peneapan Pemenang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.save = save;
        function save(data) {
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
            PenetapanPemenangVHSservice.updateapproval(data, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Approval Data!!");
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                UIControlService.unloadLoadingModal();
            });
        }





        vm.edit = edit;
        function edit(dataTabel) {
            var data = {
                item: dataTabel
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/penetapan-pemenang-vhs-fpa/ubah-data.html',
                controller: 'UbahPPGVHSCtrl',
                controllerAs: 'UbahPPGVHSCtrl',
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

        vm.Export = Export;
        function Export(tableId) {
            vm.exportHref = Excel.tableToExcel(tableId, 'sheet name');
            $timeout(function () { location.href = vm.exportHref; }, 100); // trigger download
        }
    }
})();
//TODO

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

