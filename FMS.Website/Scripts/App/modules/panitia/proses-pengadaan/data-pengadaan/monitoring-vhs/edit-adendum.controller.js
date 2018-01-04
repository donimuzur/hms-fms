(function () {
    'use strict';

    angular.module("app").controller("EditVHSCtrl", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataVHSService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataVHSService, UIControlService, GlobalConstantService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.VHSdata;
        vm.listVHS = [];
        vm.init = init;
        var id = Number($stateParams.id);
        vm.id = Number($stateParams.id);
        vm.isCalendarOpened = [false, false, false];
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        function init() {
            $translatePartialLoader.addPart('edit-adendum');
            loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataVHSService.EditAddendum({
                Offset: vm.pageSize * (vm.currentPage - 1),
                Status: vm.id,
                Limit: vm.pageSize,
                Column: vm.column
            },
            function (reply) {
                console.info("data:" + JSON.stringify(reply.data));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listVHS = reply.data.List;
                    vm.VHSdata = data;
                    vm.Remask = vm.listVHS[0].Remask;
                    vm.AdditionalValue = vm.listVHS[0].AdditionalValue;
                    vm.AddendumCode = vm.listVHS[0].AddendumCode;
                    vm.TypeAddendum = vm.listVHS[0].TypeAddendum;
                    vm.RequestDate = vm.listVHS[0].RequestDate;
                    vm.StartDate = vm.listVHS[0].StartDate;
                    vm.EndDate = vm.listVHS[0].EndDate;
                    vm.Duration = vm.listVHS[0].Duration;
                    vm.Requestor = vm.listVHS[0].Requestor;
                    vm.Duration = vm.listVHS[0].Duration,
                    vm.RequestDate = vm.listVHS[0].RequestDate;
                    vm.DocUrl = vm.listVHS[0].DocUrl;
                    vm.DocName = vm.listVHS[0].DocName;

                    vm.totalItems = Number(data.Count);

                    vm.VendorID = vm.listVHS[0].VendorID;
                    vm.TenderStepID = vm.listVHS[0].TenderStepID;
                    console.info("data:" + JSON.stringify(vm.TenderStepID));

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
        };

        vm.UpdateAddendum = UpdateAddendum;
        function UpdateAddendum() {
            UIControlService.loadLoading(loadmsg);
            DataVHSService.UpdateAddendum({
                AddendumCode: vm.AddendumCode,
                TypeAddendum: vm.TypeAddendum,
                AdditionalValue: vm.AdditionalValue,
                RequestDate: vm.RequestDate,
                DocUrl: vm.DocUrl,
                DocName: vm.DocName,
                VHSAwardId: vm.VHSAwardId,
                StartDate: vm.StartDate,
                EndDate: vm.EndDate,
                Requestor: vm.Requestor,
                VendorID: vm.VendorID,
                TenderStepID: vm.TenderStepID,
                VHSAwardId: vm.id,
                Remask: vm.Remask,
                Duration: vm.Duration,
            },
        function (reply) {
            if (reply.status === 200) {
                UIControlService.msg_growl("success", 'MESSAGE.MESSAGE_SUCCESS', "MESSAGE.TITLE_SUCCESS");
                UIControlService.unloadLoading();
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

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
