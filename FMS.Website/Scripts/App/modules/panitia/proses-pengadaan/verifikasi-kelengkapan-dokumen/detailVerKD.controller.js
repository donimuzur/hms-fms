(function () {
    'use strict';

    angular.module("app").controller("detailVerKDCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'verKDService',
		'$state', 'UIControlService', 'UploadFileConfigService', 'UploaderService', 'GlobalConstantService',
		'$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, verKDService,
		$state, UIControlService, UploadFileConfigService, UploaderService, GlobalConstantService,
		$uibModal, $stateParams) {
        var vm = this;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.IDTender = Number($stateParams.TenderRefID);
        vm.VendorID = Number($stateParams.VendorID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.data = [];
        vm.init = init;
        function init() {
            console.info("aaa");
            $translatePartialLoader.addPart('verifikasi-kelengkapan-dokumen');
            loadData();
        }

        vm.CheckCriteria = CheckCriteria;
        function CheckCriteria(data) {
            if (data.Checklist === true) {
                console.info(data);
                if (data.CriteriaId !== null) {
                    verKDService.CheckOption({
                        Status: data.CriteriaId
                    }, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            var data = reply.data;
                            vm.listOption = data;
                            return vm.listOption;
                        }
                    }, function (err) {
                        UIControlService.msg_growl("error", "MESSAGE.API");
                        UIControlService.unloadLoading();
                    });

                }
            }

        }
       
        vm.CheckCriteriaOption = CheckCriteriaOption;
        function CheckCriteriaOption(index, data1, data2) {

            if (data2 !== null) {
                vm.list = [];
                vm.list = vm.data;
                vm.data = [];
                if (data1.Checklist == true) {
                    if (data1.IsFix === true) {
                        for (var i = 0; i < vm.list.length; i++) {
                            if (i == index) {
                                vm.list[i].SelectedECOptionID = data2.ID;
                                vm.list[i].Score = data2.MaxScore;
                            }
                            vm.data.push(vm.list[i]);
                        }
                    }
                    else {
                        for (var i = 0; i < vm.list.length; i++) {
                            if (i == index) {
                                vm.list[i].SelectedECOptionID = data2.ID;
                            }
                            vm.data.push(vm.list[i]);
                        }
                    }
                }
            }



        }

        function loadData() {
            verKDService.selectDetail({
                Status: vm.IDTender,
                FilterType: vm.ProcPackType,
                column: vm.VendorID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.data = data;

                    for (var i = 0; i < vm.data.length; i++) {
                        if (vm.data[i].listOption !== null) {
                            for (var j = 0; j < vm.data[i].listOption.length; j++) {
                                //console.info("selected" + vm.data[i].SelectedECOptionID);
                                //console.info("listOptionID" + vm.data[i].listOption[j].ID);
                                if (vm.data[i].listOption[j].ID === vm.data[i].SelectedECOptionID) {
                                    vm.data[i].Option = vm.data[i].listOption[j];
                                }
                            }
                        }
                    }
                    loadStep(vm.data[0].VHSOfferEntry.vhs.TenderStepID);
                    if (vm.data[0].ID !== 0) {
                        // CheckCriteria1(vm.data);
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadStep(data) {
            console.info(data);
            verKDService.Step({
                ID: data
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.step = data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.kembali = kembali;
        function kembali() {
            $state.transitionTo('verifikasi-dokumen-vhs', { TenderRefID: vm.IDTender, StepID: vm.step.ID, ProcPackType: vm.ProcPackType });

        }

        vm.Insert = Insert;
        function Insert() {
            vm.list = [];
            for (var i = 0; i < vm.data.length; i++) {
                var datalist = {
                    ID: vm.data[i].ID,
                    VHSOfferEntry: {
                        vhs: {
                            TenderStepID: vm.step.ID,
                            VendorID: vm.data[0].VHSOfferEntry.vhs.VendorID
                        }
                    },
                    VerDocVHSId: vm.data[i].VerDocVHSId,
                    Checklist: vm.data[i].Checklist,
                    VHSOEId: vm.data[i].VHSOEId,
                    Score: vm.data[i].Score,
                    Option: vm.data[i].Option,
                    Remark: vm.data[i].Remark
                }
                vm.list.push(datalist);
            }
            verKDService.Insert(vm.list,
            //verKDService.Insert(vm.data,
           function (reply) {
               if (reply.status === 200) {
                   UIControlService.msg_growl("success", "Berhasil Simpan data");
                   init();
               }
               else {
                   UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                   return;
               }
           },
           function (err) {
               UIControlService.msg_growl("error", "Gagal Akses Api!!");
           }
      );
        }

        vm.uploadDetBC = uploadDetBC;
        function uploadDetBC(vendorid, flag) {
            console.info("SS");
            var data = {
                act: flag,
                VendorID: vendorid
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/proses-pengadaan/verifikasi-kelengkapan-dokumen/FormBC.html',
                controller: "frmBCCtrl",
                controllerAs: "frmBCCtrl",
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                UIControlService.unloadLoading();
            });
        }
    }
})();