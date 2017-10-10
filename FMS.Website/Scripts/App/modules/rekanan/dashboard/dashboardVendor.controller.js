(function () {
    'use strict';

    angular.module("app")
    .controller("DashboardVendorCtrl", ctrl);
    
    ctrl.$inject = ['$http', '$state', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DashboardVendorService', 'PengumumanPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($http, $state, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DashboardVendorService, PengumumanPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        var loadingCount;

        vm.paket = {
            currentPage : 1,
            pageSize : 5,
            totalItems : 0,
            keyword : "",
            list : []
        };

        vm.pengumuman = {
            currentPage: 1,
            pageSize: 10,
            totalItems: 0,
            keyword: "",
            list: []
        };

        vm.news = {
            currentPage: 1,
            pageSize: 10,
            totalItems: 0,
            keyword: "",
            list: []
        };
        vm.FilterColumn = 0;
        vm.textSearch = '';
        vm.maxSize = 10;
        vm.currentPage = 0;
        
        vm.init = init;
        function init(){
            $translatePartialLoader.addPart('dashboard-vendor');
            $translatePartialLoader.addPart("permintaan-ubah-data");
            UIControlService.loadLoading(loadmsg);
            loadingCount = 2;
            loadPaket();
            loadDataPengumuman(1);
            jLoad(1);
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * 5) - 5;
            DashboardVendorService.getDataCR({
                column: vm.FilterColumn,
                Keyword: vm.textSearch,
                Offset: (current - 1) * vm.maxSize,
                Limit: 10
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listVendors = data.List;
                    for (var i = 0; i < vm.listVendors.length; i++) {
                        if (!(vm.listVendors.ChangeRequestDate === null)) {
                            vm.listVendors[i].ChangeRequestDate = UIControlService.getStrDate(vm.listVendors[i].ChangeRequestDate);
                        }
                        if (!(vm.listVendors[i].EndChangeDate === null)) {
                            vm.listVendors[i].EndChangeDate = UIControlService.getStrDate(vm.listVendors[i].EndChangeDate);
                        }
                    }
                    vm.totalItems = data.Count;
                } else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan data Master Badan Usaha");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "Gagal akses API");
                UIControlService.unloadLoading();
            });
        }

        vm.reloadPaket = reloadPaket;
        function reloadPaket() {
            loadingCount = 1;
            UIControlService.loadLoading(loadmsg);
            loadPaket();
        }

        function loadPaket() {
            DashboardVendorService.SelectTender({
                Keyword: vm.paket.keyword,
                Offset: vm.paket.pageSize * (vm.paket.currentPage - 1),
                Limit: vm.paket.pageSize
            }, function (reply) {
                vm.paket.list = reply.data.List;
                unloadLoading();
                vm.paket.totalItems = reply.data.Count;
            }, function (error) {
                unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_TENDER');
            });
        }

        vm.detailtahapan = detailtahapan;
        function detailtahapan(tender) {
            $state.transitionTo('detail-tahapan-vendor', { TenderID: tender.TenderID });
        };

        function loadDataPengumuman(current) {
            var offset = (current * 10) - 10;
            PengumumanPengadaanService.getAllDataAnnouncementByVendor({
                Keyword: vm.pengumuman.keyword,
                Offset: vm.pengumuman.pageSize * (vm.paket.currentPage - 1),
                Limit: vm.pengumuman.pageSize
            }, function (reply) {
                //console.info("announc::"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.pengumuman.list = reply.data.List;
                    unloadLoading();
                    vm.pengumuman.totalItems = reply.data.Count;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }


        function loadPengumuman() {
            PengumumanPengadaanService.getDataAnnouncementByVendor({
                Keyword: vm.pengumuman.keyword,
                Offset: vm.pengumuman.pageSize * (vm.paket.currentPage - 1),
                Limit: vm.pengumuman.pageSize
            }, function (reply) {
                vm.pengumuman.list = reply.data.List;
                unloadLoading();
                vm.pengumuman.totalItems = reply.data.Count;
            }, function (err) {
                unloadLoading();
                UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_ANNOUNCEMENT");
            });
        }

        function unloadLoading() {
            loadingCount--;
            if (loadingCount <= 0) {
                UIControlService.unloadLoading();
            }
        }
    }
            
})();