(function () {
    'use strict';

    angular.module("app")
    .controller("aturTahapanModalController", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataPengadaanService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, DataPengadaanService, UIControlService, GlobalConstantService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.stepItem = item;
        vm.statusOption = [];
        vm.isCalendarOpened = [false, false, false, false];

        vm.dateNow = new Date();
        vm.timezone = vm.dateNow.getTimezoneOffset();
        vm.timezoneClient = vm.timezone / 60;

        vm.init = init;
        function init() {
            //angular detectBrowser
            vm.isOpera = !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0; // Opera 8.0+ (UA detection to detect Blink/v8-powered Opera)
            vm.isFirefox = typeof InstallTrigger !== 'undefined'; // Firefox 1.0+
            vm.isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0; // At least Safari 3+: "[object HTMLElementConstructor]"
            vm.isChrome = !!window.chrome && !vm.isOpera; // Chrome 1+
            vm.isIE = /*@cc_on!@*/ false || !!document.documentMode; // At least IE6
            console.info("isChrome?" + vm.isChrome);
            console.info("timezoneClient" + vm.timezoneClient);
            
            if (vm.isChrome === true) {
                var objappVersion = navigator.appVersion; var objAgent = navigator.userAgent; var objbrowserName = navigator.appName; var objfullVersion = '' + parseFloat(navigator.appVersion); var objBrMajorVersion = parseInt(navigator.appVersion, 10); var objOffsetName, objOffsetVersion, ix;
                // In Chrome 
                if ((objOffsetVersion = objAgent.indexOf("Chrome")) != -1) { objbrowserName = "Chrome"; objfullVersion = objAgent.substring(objOffsetVersion + 7); }
                // trimming the fullVersion string at semicolon/space if present 
                if ((ix = objfullVersion.indexOf(";")) != -1) objfullVersion = objfullVersion.substring(0, ix);
                if ((ix = objfullVersion.indexOf(" ")) != -1) objfullVersion = objfullVersion.substring(0, ix); objBrMajorVersion = parseInt('' + objfullVersion, 10);
                if (isNaN(objBrMajorVersion)) { objfullVersion = '' + parseFloat(navigator.appVersion); objBrMajorVersion = parseInt(navigator.appVersion, 10); };
                //console.info("versi full:" + objfullVersion);
                //console.info("versi major:" + objBrMajorVersion);
                if (objBrMajorVersion >= 58) {
                    vm.stepItem.StartDate = new Date(vm.stepItem.StartDate.setHours(vm.stepItem.StartDate.getHours()));
                    vm.stepItem.StartDate.setSeconds(0);
                    vm.stepItem.StartDate.setMilliseconds(0);
                    vm.stepItem.EndDate = new Date(vm.stepItem.EndDate.setHours(vm.stepItem.EndDate.getHours()));
                    vm.stepItem.EndDate.setSeconds(0);
                    vm.stepItem.EndDate.setMilliseconds(0);
                }
                else if (objBrMajorVersion <= 57) {
                    vm.stepItem.StartDate = new Date(vm.stepItem.StartDate.setHours(vm.stepItem.StartDate.getHours() + vm.timezoneClient ));
                    vm.stepItem.StartDate.setSeconds(0);
                    vm.stepItem.StartDate.setMilliseconds(0);
                    vm.stepItem.EndDate = new Date(vm.stepItem.EndDate.setHours(vm.stepItem.EndDate.getHours() + vm.timezoneClient));
                    vm.stepItem.EndDate.setSeconds(0);
                    vm.stepItem.EndDate.setMilliseconds(0);
                }
            }
            UIControlService.loadLoadingModal(loadmsg);
            DataPengadaanService.GetStatusOption(function (reply) {
                vm.statusOption = reply.data;
                UIControlService.unloadLoadingModal();
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_STATUS');
            });
        };

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }

        vm.checkStartDate = checkStartDate;
        function checkStartDate(startdate) {
            var today = moment().format("YYYY-MM-DD");
            var convertedStartDate = moment(startdate).format("YYYY-MM-DD");
            if (today > convertedStartDate) {
                vm.stepItem.StartDate = new Date();
                vm.stepItem.StartDate.setHours(0);
                vm.stepItem.StartDate.setMinutes(0);
                vm.stepItem.StartDate.setSeconds(0);
                vm.stepItem.StartDate.setMilliseconds(0);
            }
        }

        vm.save = save;
        function save() {
                console.info("status:" + JSON.stringify(vm.stepItem.Status));
            if (!vm.stepItem.StartDate || !vm.stepItem.EndDate) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_NO_DATE');
                return;
            }
            else if (vm.stepItem.EndDate < vm.stepItem.StartDate) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_RANGE_DATE');
                return;
            }
           if (vm.stepItem.Status === 4175) {
                vm.stepItem.EndDate = new Date();
            }
           vm.StartDate = new Date(vm.stepItem.StartDate.setHours(vm.stepItem.StartDate.getHours() - vm.timezoneClient));
           vm.endDate = new Date(vm.stepItem.EndDate.setHours(vm.stepItem.EndDate.getHours() - vm.timezoneClient));
            UIControlService.loadLoadingModal(loadmsg);
            DataPengadaanService.AturStep({
                ID: vm.stepItem.ID,
                StartDate: vm.stepItem.StartDate,
                EndDate: vm.stepItem.EndDate,
                Status: vm.stepItem.Status
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("notice", 'MESSAGE.SUCC_SAVE_STEP');
                $uibModalInstance.close();
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE_STEP');
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();