(function () {
    'use strict';

    angular.module("app")
    .controller("pengumumanPengadaanCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PengumumanPengadaanService', '$state', 'UIControlService', '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PengumumanPengadaanService,
        $state, UIControlService,
        $uibModal, $stateParams) {
        var vm = this;
        vm.IDTender = Number($stateParams.TenderRefID);
        vm.IDStepTender = Number($stateParams.StepID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.TypeTender = "";
        vm.filteredEmails = [];
        vm.dataTenderReal = {};
        vm.dataTender = {
            TenderCode: '',
            TenderName: '',
            IsVendorEmails: false,
            IsInternational: false,
            IsLocal: false,
            IsNational: false,
            IsOpen: false,
            Vendors: [],
            Emails: '',
            CompScale: null,
            CommodityID: null,
            Description: ''
        };

        vm.listPengumuman = [];
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("pengumuman-pengadaaan-tender");
            loadDataPengumuman();
            //loadDataTender();
        }

        function loadDataTender() {
            PengumumanPengadaanService.getDataTender({
                ProcPackageType: vm.ProcPackType, TenderRefID: vm.IDTender
            }, function (reply) {                
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.dataTenderReal = data;
                    //console.info("tender::" + JSON.stringify(vm.dataTenderReal));
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadDataPengumuman() {
            PengumumanPengadaanService.selectPengumuman({
                StepID: vm.IDStepTender, tender:{ TenderRefID: vm.IDTender, ProcPackageType: vm.ProcPackType }
            }, function (reply) {
                console.info("*added: "+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    if (data.length > 0 && (data[0].TenderAnnouncement.ID != 0)) {
                        vm.listPengumuman = data;
                    }
                    else {
                        loadDataTender();
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.openForm = openForm;
        function openForm(isAdd, data) {
            //set data jika tender RFQ GOODS
            if (vm.ProcPackType === 4190) {
                vm.dataTender.TenderCode = vm.dataTenderReal.RFQCode;
                vm.dataTender.TenderName = vm.dataTenderReal.RFQName;
                vm.dataTender.IsLocal = vm.dataTenderReal.IsLocal;
                vm.dataTender.IsNational = vm.dataTenderReal.IsNational;
                vm.dataTender.IsInternational = vm.dataTenderReal.IsInternational;
                vm.dataTender.CommodityID = vm.dataTenderReal.CommodityID;
                vm.dataTender.CompScale = vm.dataTenderReal.CompScale;
                vm.dataTender.IsVendorEmails = vm.dataTenderReal.IsVendorEmails;
                vm.dataTender.Emails = vm.dataTenderReal.Emails;
                vm.dataTender.filteredEmails = vm.dataTenderReal.FilteredEmails;
                vm.dataTender.Vendors = vm.dataTenderReal.Vendors;
                vm.TypeTender = "RFQGOODS";
            }
            else if (vm.ProcPackType === 3168) {
                vm.dataTender.TenderCode = vm.dataTenderReal.RFQCode;
                vm.dataTender.TenderName = vm.dataTenderReal.RFQName;
                vm.dataTender.IsLocal = vm.dataTenderReal.IsLocal;
                vm.dataTender.IsNational = vm.dataTenderReal.IsNational;
                vm.dataTender.IsInternational = vm.dataTenderReal.IsInternational;
                vm.dataTender.CommodityID = vm.dataTenderReal.CommodityID;
                vm.dataTender.CompScale = vm.dataTenderReal.CompScale;
                vm.dataTender.IsVendorEmails = vm.dataTenderReal.IsVendorEmails;
                vm.dataTender.Emails = vm.dataTenderReal.Emails;
                vm.dataTender.filteredEmails = vm.dataTenderReal.FilteredEmails;
                vm.dataTender.Vendors = vm.dataTenderReal.Vendors;
                vm.TypeTender = "RFQVHS";
            }
            //set data jika tender CR
            else if (vm.ProcPackType === 4189) {
                vm.dataTender.TenderCode = vm.dataTenderReal.TenderCode;
                vm.dataTender.TenderName = vm.dataTenderReal.ProjectTitle;
                vm.TypeTender = "CR";
            }
            var senddata = {
                isAdd: isAdd,
                IDRefTender: vm.IDTender,
                IDStepTender: vm.IDStepTender,
                IDProcPackType: vm.ProcPackType,
                TypeTender: vm.TypeTender,
                DataTender: vm.dataTender
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/pengumuman-pengadaan/formPengumumanPengadaan.html',
                controller: 'formPPController',
                controllerAs: 'formPPCtrl',
                resolve: {
                    item: function () {
                        return senddata;
                    }
                }
            });
            modalInstance.result.then(function () {
                loadDataPengumuman();
            });
        }

        vm.editForm = editForm;
        function editForm(isAdd, data) {
            vm.dataTender.Vendor = [];
            if (vm.ProcPackType === 4190) { vm.TypeTender = "RFQGOODS"; }
            if (vm.ProcPackType === 4189) { vm.TypeTender = "CR"}
            //console.info("edit:" + data.TenderAnnouncement.Description);
            vm.dataTender.TenderCode = data.TenderAnnouncement.TenderStepData.tender.TenderCode;
            vm.dataTender.TenderName = data.TenderAnnouncement.TenderStepData.tender.TenderName;
            vm.dataTender.IsLocal = data.TenderAnnouncement.IsLokal;
            vm.dataTender.IsNational = data.TenderAnnouncement.IsNational;
            vm.dataTender.IsInternational = data.TenderAnnouncement.IsInternational;
            vm.dataTender.CommodityID = data.TenderAnnouncement.CommodityID;
            vm.dataTender.TechnicalID = data.TenderAnnouncement.TechnicalID;
            vm.dataTender.CompScale = data.TenderAnnouncement.CompanyScaleID;
            vm.dataTender.IsVendorEmails = data.TenderAnnouncement.IsVendorEmail;
            vm.dataTender.Emails = data.TenderAnnouncement.Emails;
            vm.dataTender.Description = data.TenderAnnouncement.Description;
            vm.dataTender.DocUrl = data.TenderAnnouncement.DocUrl
            //console.info(JSON.stringify(;.DetailVendor));
            var detvendor = data.DetailVendor;
            for (var i = 0; i < detvendor.length ; i++) {
                var data = {
                    VendorID: detvendor[i].VendorID,
                    Code: detvendor[i].Code,
                    VendorName: detvendor[i].Name,
                    Email: detvendor[i].Email
                }
                vm.dataTender.Vendors.push(data);
            }
            var senddata = {
                isAdd: isAdd,
                IDRefTender: vm.IDTender,
                IDStepTender: vm.IDStepTender,
                IDProcPackType: vm.ProcPackType,
                TypeTender: vm.TypeTender,
                DataTender: vm.dataTender
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/pengumuman-pengadaan/formPengumumanPengadaan.html',
                controller: 'formPPController',
                controllerAs: 'formPPCtrl',
                resolve: {
                    item: function () {
                        return senddata;
                    }
                }
            });
            modalInstance.result.then(function () {
                loadDataPengumuman();
            });
        }

        vm.detailData = detailData;
        function detailData(isAdd, data) {
            vm.dataTender.Vendor = [];
            if (vm.ProcPackType === 4190) { vm.TypeTender = "RFQGOODS"; }
            if (vm.ProcPackType === 4189) { vm.TypeTender = "CR" }
            //console.info("edit:" + data.TenderAnnouncement.Description);
            vm.dataTender.TenderCode = data.TenderAnnouncement.TenderStepData.tender.TenderCode;
            vm.dataTender.TenderName = data.TenderAnnouncement.TenderStepData.tender.TenderName;
            vm.dataTender.IsLocal = data.TenderAnnouncement.IsLokal;
            vm.dataTender.IsNational = data.TenderAnnouncement.IsNational;
            vm.dataTender.IsInternational = data.TenderAnnouncement.IsInternational;
            vm.dataTender.CommodityID = data.TenderAnnouncement.CommodityID;
            vm.dataTender.TechnicalID = data.TenderAnnouncement.TechnicalID;
            vm.dataTender.CompScale = data.TenderAnnouncement.CompanyScaleID;
            vm.dataTender.IsVendorEmails = data.TenderAnnouncement.IsVendorEmail;
            vm.dataTender.Emails = data.TenderAnnouncement.Emails;
            vm.dataTender.Description = data.TenderAnnouncement.Description;
            //console.info(JSON.stringify(data.DetailVendor));
            var detvendor = data.DetailVendor;
            for (var i = 0; i < detvendor.length ; i++) {
                var data = {
                    VendorID: detvendor[i].VendorID,
                    Code: detvendor[i].Code,
                    VendorName: detvendor[i].Name,
                    Email: detvendor[i].Email
                }
                vm.dataTender.Vendors.push(data);
            }
            var senddata = {
                isAdd: isAdd,
                IDRefTender: vm.IDTender,
                IDStepTender: vm.IDStepTender,
                IDProcPackType: vm.ProcPackType,
                TypeTender: vm.TypeTender,
                DataTender: vm.dataTender
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'detail-pengumuman-pengadaan.html',
                controller: 'formPPController',
                controllerAs: 'formPPCtrl',
                resolve: {
                    item: function () {
                        return senddata;
                    }
                }
            });
            modalInstance.result.then(function () {
                loadDataPengumuman();
            });
        }

        vm.batal = batal;
        function batal() {
            //console.info("batal");
            $uibModalInstance.dismiss('cancel');
        };

        vm.backDetailTahapan = backDetailTahapan;
        function backDetailTahapan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
        }
    }
})();