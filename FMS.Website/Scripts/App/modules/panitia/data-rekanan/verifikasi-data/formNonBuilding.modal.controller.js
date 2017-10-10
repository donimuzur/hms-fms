    
(function () {
    'use strict';

    angular.module("app")
    .controller("FormNonBuildingController", ctrl);
    
    ctrl.$inject = ['$scope', '$http', '$uibModalInstance', '$cookieStore', '$state', '$stateParams', '$rootScope',
        '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'item','VerifikasiDataService',
        'ProvinsiService', 'UIControlService'];
    /* @ngInject */
    function ctrl($scope, $http, $uibModalInstance, $cookieStore, $state, $stateParams, $rootScope,
        $uibModal, $translate, $translatePartialLoader, $location, SocketService, item,VerifikasiDataService,
        ProvinsiService, UIControlService) {
        var vm = this;
        vm.dataForm = {};
        vm.isAdd = item.isForm;
        vm.isCategory = item.type;
        vm.title = "";
        vm.ID;
        vm.tipeform = item.type;
        var categoryID; //sesuai id ref
        vm.isCalendarOpened = [false, false, false, false];
        vm.tipeform = item.type;
        vm.dateconvert;

        vm.init = init;
        function init() {
            //cek vehicle atau equipment
            if(vm.isCategory === "vehicle"){
                vm.title = "Data Kendaraan";
                categoryID = 3136;
            }
            if (vm.isCategory === "equipment") {
                vm.title = "Data Peralatan";
                categoryID = 3137;
            }

            if (vm.isAdd === true) {
                vm.dataForm = new Field("", "", categoryID, 0, null, "", 0, "", "");
                vm.ID = 0;
            }
            else {
                console.info(item.data);
                vm.dataForm = new Field(item.data.Brand, item.data.Capacity, categoryID, item.data.Condition,
                    new Date(Date.parse(item.data.MfgDate)), item.data.Name, item.data.Ownership, item.data.SerialNo, item.data.Type);
                vm.ID = item.data.ID;
                vm.VendorID = item.data.VendorID;
                vm.dateconvert = UIControlService.getStrDate(vm.dataForm.MfgDate);
            }
            loadConditional();
            loadKepemilikan();
        }

        vm.listConditional = [];
        vm.selectedConditional;
        function loadConditional() {
            UIControlService.loadLoading("Loading Data Kondisi");
            VerifikasiDataService.getConditionEq(
            function (reply) {
                UIControlService.unloadLoading();
                vm.listConditional = reply.data.List;
                if (vm.isAdd === false) {
                    for (var i = 0; i < vm.listConditional.length; i++) {
                        if (item.data.Condition === vm.listConditional[i].RefID) {
                            vm.selectedConditional = vm.listConditional[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.listKepemilikan = [];
        vm.selectedKepemilikan;
        function loadKepemilikan() {
            UIControlService.loadLoading("Loading Data Kepemilikan");
            VerifikasiDataService.getOwnership(
            function (reply) {
                UIControlService.unloadLoading();
                vm.listKepemilikan = reply.data.List;
                if (vm.isAdd === false) {
                    for (var i = 0; i < vm.listKepemilikan.length; i++) {
                        if (item.data.Ownership === vm.listKepemilikan[i].RefID) {
                            vm.selectedKepemilikan = vm.listKepemilikan[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        /*open form date*/
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        vm.saveData = saveData;
        function saveData() {
            if (vm.selectedConditional === undefined) {
                UIControlService.msg_growl("warning", "MESSAGE.NO_CONDITIONAL");
                return;
            }
            if (vm.selectedKepemilikan === undefined) {
                UIControlService.msg_growl("error", "MESSAGE.NO_OWNERSHIP");
                return;
            }
            
            var senddata = {
                ID: vm.ID,
                VendorID: vm.VendorID,
                Brand : vm.dataForm.Brand,
                Capacity : vm.dataForm.Capacity,
                Category : categoryID,
                Condition : vm.selectedConditional.RefID,
                MfgDate : vm.dataForm.MfgDate,
                Name : vm.dataForm.Name,
                Ownership : vm.selectedKepemilikan.RefID,
                SerialNo : vm.dataForm.SerialNo,
                Type : vm.dataForm.Brand
            }
            //proses send data
            if (vm.isAdd === true) {
                DataPerlengkapanService.insertNonBuilding(senddata , function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "MESSAGE.SUCCES_SAVE_NONBUILDING");
                        $uibModalInstance.close();
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "MESSAGE.API");
                });
            } else {
                VerifikasiDataService.updateNonBuilding(senddata, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "MESSAGE.SUCCES_UPDATE_NONBUILDING");
                        $uibModalInstance.close();
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "MESSAGE.API");
                });
            }
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        }

        function Field(Brand, Capacity, Category, Condition, MfgDate, Name, Ownership, SerialNo, Type) {
            var self = this;
            self.Brand = Brand;
            self.Capacity = Capacity;
            self.Category = Category;
            self.Condition = Condition;
            self.MfgDate = MfgDate;
            self.Name = Name;
            self.Ownership = Ownership;
            self.SerialNo = SerialNo;
            self.Type = Type;
        }
    }
})();