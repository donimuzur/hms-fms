(function () {
    'use strict';

    angular.module("app").controller("EditDistribusiCtrl", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataKontrakService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataKontrakService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.list = [];
        vm.init = init;
        vm.contractSignOff;
        var id = Number($stateParams.id);
        vm.id = Number($stateParams.id);
        var param = Number($stateParams.param);
        vm.param = Number($stateParams.param);
        vm.isCalendarOpened = [false, false, false];
        vm.years;
        vm.Tahun;
        vm.buyer_remask = '';
        vm.budget = 0;
        vm.freight_method = '';
        vm.delivery_point = '';


        function init() {
            $translatePartialLoader.addPart('edit-distribusi');
            loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {

            UIControlService.loadLoading(loadmsg);
            DataKontrakService.EditDistribusi({
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Status: id,
                Parameter: param

            },

            function (reply) {
                console.info("data:" + JSON.stringify(reply.data));
                UIControlService.unloadLoading();

                if (reply.status === 200) {
                    var data = reply.data;
                    vm.contractSignOff = data;
                    vm.list = reply.data.List;
                    vm.ContractRequisitionId = vm.contractSignOff.ContractRequisitionId;
                    vm.vendorId = vm.contractSignOff.VendorID;
                    vm.negoId = vm.contractSignOff.NegoId;
                    vm.TenderStepId = vm.contractSignOff.TenderStepID;
                    vm.DistributionBudgetId = vm.contractSignOff.DisBudgetId;
                    vm.TanggalPO = vm.contractSignOff.DatePurchase;
                    vm.keterangan = vm.contractSignOff.keterangan;
                    vm.totalprice = vm.contractSignOff.totalprice;
                    vm.NoPO = vm.contractSignOff.NoPurchase;
                    vm.TahunDistribusi = vm.contractSignOff.TahunDistribusi;
                    var year = new Date().getFullYear();

                    var range = [];
                    for (var i = 0; i < 10; i++) {
                        range.push({
                            label: year + i,
                            value: year + i
                        });
                    }

                    vm.years = range;
                    console.info("vm.years:" + JSON.stringify(vm.years));

                }
                else {
                    $.growl.error({ message: "MESSAGE.ERR_LOAD" });
                    UIControlService.unloadLoading();
                }
            },
            function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        }
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }

        vm.ubahtahun = ubahtahun;
        function ubahtahun(date) {
            date = new Date(Date.parse(date));
            return date.getFullYear();
        }

        vm.getStrDate = getStrDate;
        function getStrDate(date) {
            return UIControlService.getStrDate(date);
        }

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }

        vm.EditDistribusi = EditDistribusi;
        function EditDistribusi() {

            console.info("vm.TahunDistribusi:" + JSON.stringify(vm.TahunDistribusi));
            if (vm.TahunDistribusi == null && vm.TanggalPO == '') {
                UIControlService.msg_growl("error", 'Pastikan Form Terisi Semua', "Failed");
                UIControlService.unloadLoading();
            } else {
                UIControlService.loadLoading(loadmsg);
                 DataKontrakService.Update({
                    DistributionBudgetId: vm.DistributionBudgetId,
                    NegoId: vm.negoId,
                    buyer_remark: vm.buyer_remark,
                    keterangan: vm.keterangan,
                    TahunDistribusi: vm.TahunDistribusi,
                    TenderStepId: vm.TenderStepId,
                    totalprice: vm.totalprice,
                    VendorId: vm.vendorId,
                    ContractRequisitionId: vm.ContractRequisitionId,
                    DatePurchase: vm.TanggalPO,
                    NoPurchase: vm.NoPO
                },
            function (reply) {
                if (reply.status === 200) {

                    UIControlService.msg_growl("success", 'MESSAGE.MESSAGE_SUCCESS', "MESSAGE.TITLE_SUCCESS");
                    UIControlService.unloadLoading();
                 //   $state.transitionTo('monitoring-kontrak');
                }
                else {
                    UIControlService.msg_growl("error", 'MESSAGE.MESSAGE_FAILED', "MESSAGE.TITLE_FAILED");
                    UIControlService.unloadLoading();
                }
            },
            function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
            }
        }
    }
})();
