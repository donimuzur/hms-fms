(function () {
    'use strict';

    angular.module("app").controller("BlacklistCtrl", ctrl);

    ctrl.$inject = ['$state', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'BlacklistService', 'RoleService', 'UIControlService', '$uibModal', 'GlobalConstantService'];
    function ctrl($state, $http, $translate, $translatePartialLoader, $location, SocketService, BlacklistService,
        RoleService, UIControlService, $uibModal, GlobalConstantService) {
        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        var page_id = 141;
        vm.vendorname = "";
        vm.totalItems = 0;
        vm.totalblacklis = 0;
        vm.currentPage = 1;
        vm.flag = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.selectedBlacklist = "";
        vm.selectblacklist = 2061;
        vm.init = init;

        function init() {
            $translatePartialLoader.addPart('blacklist-data');
            UIControlService.loadLoading("Silahkan Tunggu...");
            getUserLogin();
            getBlacklist();
            jLoad(1);

        }

        vm.getBlacklist = getBlacklist;
        vm.listBlacklist = [];
        function getBlacklist() {
            vm.listBlacklist = [];
            BlacklistService.GetBlacklist(
               function (response) {
                   if (response.status === 200) {
                       for(var i=0; i<response.data.length; i++){
                           var data = {
                               RefID: response.data[i].RefID,
                               Name: response.data[i].Name
                           }
                           vm.listBlacklist.push(data);
                           console.info(vm.listBlacklist);
                           if (i === 1) {
                               var data = {
                                   RefID: 0,
                                   Name: "Semua"
                               }
                               vm.listBlacklist.push(data);
                               console.info(vm.listBlacklist);
                           }

                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list data blacklist");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses API");
                   return;
               });
        }

        vm.addBlacklist = addBlacklist;
        function addBlacklist(data) {
            var data = {
                act: true,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/blacklist/FormBlacklist.html',
                controller: 'FormBlacklistCtrl',
                controllerAs: 'frmBlacklistCtrl',
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

        vm.msg = msg;
        function msg(data) {
            bootbox.confirm('<h3 class="afta-font">Yakin membatalkan blacklist ?</h3>', function (res) {
                if (res) {
                    UIControlService.loadLoading("Silahkan Tunggu");
                    BlacklistService.editBlacklist({
                        BlacklistID: data.BlacklistID,
                        BlacklistTypeID: data.BlacklistTypeID,
                        MasaBlacklistID: 0,
                        StartDateBlacklist: "",
                        EndDateBlacklist: ""
                    }, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            //var msg = "";
                            //if (active === false) msg = " NonAktifkan ";
                            //if (active === true) msg = "Aktifkan ";
                            UIControlService.msg_growl("success", "Data Berhasil di batalkan blacklist");
                            init();
                           
                        }
                        else {
                            UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
                            return;
                        }
                    }, function (err) {

                        UIControlService.msg_growl("error", "Gagal Akses API ");
                        UIControlService.unloadLoading();
                    });
                }
                else {
                    console.info("sorry");
                }
            });
        }

        vm.cancel = cancel;
        function cancel(data1) {
            var data = {
                act: false,
                item: data1
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/blacklist/FormWhitelist.html',
                controller: 'FormWhitelistCtrl',
                controllerAs: 'FormWhitelistCtrl',
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

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.blacklist = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            if (vm.selectedBlacklist === "" || vm.selectedBlacklist.RefID === 0) {
                vm.flag = 0;
                BlacklistService.selectblacklist({
                    Offset: offset,
                    Limit: vm.pageSize,
                    Keyword: vm.vendorname,
                    Status: 0
                }, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        var data = reply.data;
                        vm.blacklist = data.List;
                        vm.totalItems = Number(data.Count);
                        console.info(vm.blacklist);
                    } else {
                        $.growl.error({ message: "Gagal mendapatkan data " });
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    $.growl.error({ message: "Gagal Akses API >" + err });
                    UIControlService.unloadLoading();
                });
            }
            else if (vm.selectedBlacklist !== "") {
                if (vm.selectedBlacklist.RefID == 2060) {
                    vm.flag = 0;
                    BlacklistService.selectblacklist({
                        Offset: offset,
                        Limit: vm.pageSize,
                        Keyword: vm.vendorname,
                        Status: vm.selectedBlacklist.RefID
                    }, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            var data = reply.data;
                            vm.blacklist = data.List;
                            vm.totalItems = Number(data.Count);
                            console.info(vm.blacklist);
                        } else {
                            $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
                            UIControlService.unloadLoading();
                        }
                    }, function (err) {
                        $.growl.error({ message: "Gagal Akses API >" + err });
                        UIControlService.unloadLoading();
                    });
                }
                else if (vm.selectedBlacklist.RefID == 2061) {
                    vm.flag = 1;
                    BlacklistService.selectvendor({
                        Offset: offset,
                        Limit: vm.pageSize,
                        Keyword: vm.vendorname,
                        Status: vm.selectedBlacklist.RefID
                    }, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            var data = reply.data;
                            vm.blacklist = data.List;
                            vm.flag === 0;
                            vm.totalItems = Number(data.Count);
                            console.info(vm.blacklist);
                        } else {
                            $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
                            UIControlService.unloadLoading();
                        }
                    }, function (err) {
                        $.growl.error({ message: "Gagal Akses API >" + err });
                        UIControlService.unloadLoading();
                    });
                }
            }
        }

        vm.detailApproval = detailApproval;
        function detailApproval(dt) {
            var item = {
                data: dt
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/blacklist/detailApproval.modal.html',
                controller: 'detailApprovalBlacklistCtrl',
                controllerAs: 'detailApprovalBlacklistCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function () {
                init();
            });
        };

        vm.getUserLogin = getUserLogin;
        function getUserLogin() {
            BlacklistService.getUserLogin( function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.typelogin = reply.data;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data " });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.search = function () {
            //console.info("vendorname" + JSON.stringify(vm.vendorname));
            jLoad(1);
        }

        vm.approval = approval;
        function approval() {
            $state.transitionTo("detail-approval-blacklist");
        }
        
        vm.detail = detail;
        function detail(data, type) {
            var data = {
                act: false,
                type: type,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/blacklist/FormBlacklist.html',
                controller: 'FormBlacklistCtrl',
                controllerAs: 'frmBlacklistCtrl',
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


    }
})();
//TODO


