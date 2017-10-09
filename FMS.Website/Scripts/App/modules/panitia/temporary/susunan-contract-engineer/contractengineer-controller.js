(function () {
    'use strict';

    angular.module("app").controller("ContractEngineerCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ContractEngineerService', 'RoleService', 'UIControlService', '$uibModal', '$state'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, ContractEngineerService,
        RoleService, UIControlService, $uibModal, $state) {

        var vm = this;
        var page_id = 141;
        vm.data = "";
        vm.list = [];
        vm.contract = {};
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.fullSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.kata = new Kata("");
        vm.init = init;
        vm.selectedRFQ = "";
        vm.jLoad = jLoad;
        vm.tambah = tambah;
        vm.isCalendarOpened = [false, false, false, false];
        vm.flag = false;
        function init() {
            //UIControlService.loadLoading("Silahkan Tunggu...");
            convertToDate();
            jLoad(1,null,"");

        }

        vm.jLoad = jLoad;
        function jLoad(current, colom, input, type, key) {
            key = UIControlService.getStrDate(key);
            console.info(key);
            if ((colom === '0' || colom === '1' || colom === '2' || colom === '7') && input === undefined) {
                UIControlService.msg_growl("warning", "Keyword Belum diisi"); return;
               
            }
            else if (colom === '4' && key === undefined) {
                UIControlService.msg_growl("warning", "Tanggal Belum diisi"); return;
            }
            else if ((colom === '3' || colom === '5') && type === undefined) {
                UIControlService.msg_growl("warning", "Keyword Belum diisi"); return;
            }
            vm.list = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            if (colom === '4') {
                vm.contract = {
                    Offset: offset,
                    Limit: vm.pageSize,
                    Date1: key,
                    column: colom
                }
            }
            else if (colom === '3' || colom === '5') {
                vm.contract = {
                    Offset: offset,
                    Limit: vm.pageSize,
                    Status: type,
                    column: colom
                }
            }
            else {
                if (colom === 6) colom = 0;
                vm.contract = {
                    Offset: offset,
                    Limit: vm.pageSize,
                    Keyword: input,
                    column: colom
                }
            }
            ContractEngineerService.select(vm.contract, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.list = data.List;
                    console.info(vm.list);
                    vm.totalItems = data.Count;
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Susunan Contract Engineer" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.add = add;
        function add(data) {
            vm.input = undefined;
            vm.selectedType = undefined;
            vm.SetDate = undefined;
            if (data === '6') {
                vm.flag = false;
            }
            else vm.flag = true;
        }


        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString() { // TIMEZONE (-)
            if (vm.SetDate) {
                vm.SetDate = UIControlService.getStrDate(vm.SetDate);
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate() {
            if (vm.SetDate) {
                vm.SetDate = new Date(Date.parse(vm.SetDate));
            }
        }
        
        vm.Dokumen = Dokumen;
        function Dokumen(data) {
            $state.transitionTo('contract-requisition-contract', { contractRequisitionId: data.ContractRequisitionID });
        }

        vm.tambah = tambah;
        function tambah(data) {
            console.info("masuk form add/edit");
            var data = {
                act: 1,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/departemen/formDepartemen.html',
                controller: 'departemenModalCtrl',
                controllerAs: 'formDepartCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
        }

        vm.commite = commite;
        function commite(data) {
            console.info(data);
            var data = {
                act: 0,
                item: data,
                dataTemp : data.flag
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/temporary/susunan-contract-engineer/commite-modal.html',
                controller: 'CommitteeModalCtrl',
                controllerAs: 'CommitteeModalCtrl',
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

        vm.detailContract = detailContract;
        function detailContract(dt) {
            console.info("ss");
            $state.transitionTo('detail-contract-requisition-contract', { contractRequisitionId: dt.ContractRequisitionID });
        };
    }
})();
//TODO

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

