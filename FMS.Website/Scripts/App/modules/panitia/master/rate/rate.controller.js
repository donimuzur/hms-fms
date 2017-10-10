(function() {
    'use strict';

    angular.module("app")
            .controller("RateCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'RateService', '$state',
        'UIControlService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, RateService, $state, UIControlService) {

        var vm = this;
        vm.listRate = [];
        vm.maxSize = 10;
        vm.currentPage = 0;
        vm.dateFirst;
        vm.totalItems = 0;
        vm.firstDate = null;
        vm.endDate = null;
        vm.textSearch = "";
        vm.selectedSearchBy = 0;

        vm.init = init;
        function init(){
            $translatePartialLoader.addPart("master-rate");
            vm.jLoad(1);
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * vm.maxSize) - vm.maxSize;
            RateService.Select({
                Offset: offset,
                Limit: vm.maxSize,
                column: vm.selectedSearchBy,
                Keyword: vm.textSearch,
                Date1: UIControlService.getStrDate(vm.firstDate),
                Date2: UIControlService.getStrDate(vm.endDate)
            }, function (reply) {
                console.info("Rate:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listRate = data.List;
                    for (var i = 0; i < vm.listRate.length; i++) {
                        vm.listRate[i].ValidFrom = UIControlService.getStrDate(vm.listRate[i].ValidFrom);
                    }
                    vm.totalItems = Number(data.Count);
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOADDATA");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoading();
            });
        }

        vm.formRate = formRate;
        function formRate() {
            $state.transitionTo('form-master-rate');
        }

        vm.openCalendar = openCalendar;
        vm.isCalendarOpened = [false, false, false, false];
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
            //console.info("cal"+vm.firstDate);
        };

        vm.show = show;
        function show() {
            if (vm.firstDate === undefined) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_DATE1");
                return;
            }
            if (vm.endDate === undefined) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_DATE2");
                return;
            }
            vm.jLoad(1);
        }
    }
})();