(function () {
    'use strict';

    angular.module("app").controller("ComplaintCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ComplaintService', 'RoleService', 'UIControlService', '$uibModal'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, ComplaintService,
        RoleService, UIControlService, $uibModal) {
        var vm = this;
        vm.search = "";
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;

        function init() {
            $translatePartialLoader.addPart('master-complaint');
            jLoad(1);

        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            console.info("curr ")
            vm.complaint = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            ComplaintService.select({
                Offset: offset,
                Limit: vm.pageSize,
                Keyword: vm.search
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.complaint = data.List;
                    console.info(JSON.stringify(vm.complaint));
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data pengaduan Departemen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }
        vm.cariReporter = cariReporter;
        function cariReporter() {
            init();
        }

        vm.tambah = tambah;
        function tambah(data, act) {
            console.info("masuk form add/edit");
            var data = {
                act: act,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/FormWarningLetter.html',
                controller: 'FormWarningLetterCtrl',
                controllerAs: 'frmWarningLetterCtrl',
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

        vm.ubah_aktif = ubah_aktif;
        function ubah_aktif(data, active) {
            UIControlService.loadLoading("Silahkan Tunggu");
            //console.info("ada:"+JSON.stringify(data))
            ComplaintService.editActive({
                ComplaintID: data.ComplaintID,
                IsActive: active
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var msg = "";
                    if (active === false) msg = " NonAktifkan ";
                    if (active === true) msg = "Aktifkan ";
                    UIControlService.msg_growl("success", "Data Berhasil di " + msg);
                    jLoad(1);
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

        vm.lihatDetail = lihatDetail;
        function lihatDetail(data) {
            console.info("masuk form add/edit");
            var data = {
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/pengaduan/DetailComplaint.html',
                controller: 'DetailPengaduanCtrl',
                controllerAs: 'DetailPengaduanCtrl',
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

        

        









        //vm.ubah_aktif = ubah_aktif;
        //function ubah_aktif(data, active) {
        //    UIControlService.loadLoading("Silahkan Tunggu");
        //    //console.info("ada:"+JSON.stringify(data))
        //    DepartemenService.editActive({
        //        DepartmentID: data.DepartmentID, DepartmentCode: data.DepartmentCode, DepartmentName: data.DepartmentName, IsActive: active
        //    }, function (reply) {
        //        UIControlService.unloadLoading();
        //        if (reply.status === 200) {
        //            var msg = "";
        //            if (active === false) msg = " NonAktifkan ";
        //            if (active === true) msg = "Aktifkan ";
        //            UIControlService.msg_growl("success", "Data Berhasil di " + msg);
        //            jLoad(1);
        //        }
        //        else {
        //            UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
        //            return;
        //        }
        //    }, function (err) {

        //        UIControlService.msg_growl("error", "Gagal Akses API ");
        //        UIControlService.unloadLoading();
        //    });

        //}

        //vm.tambah = tambah;
        //function tambah(data, act) {
        //    console.info("masuk form add/edit");
        //    var data = {
        //        act: act,
        //        item: data
        //    }
        //    var modalInstance = $uibModal.open({
        //        templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/FormWarningLetter.html',
        //        controller: 'FormWarningLetterCtrl',
        //        controllerAs: 'frmWarningLetterCtrl',
        //        resolve: {
        //            item: function () {
        //                return data;
        //            }
        //        }
        //    });
        //    modalInstance.result.then(function () {
        //        vm.jLoad(1);
        //    });
        //}

        //vm.edit = edit;
        //function edit(data, act) {
        //    console.info("masuk form add/edit");
        //    var data = {
        //        act: act,
        //        item: data
        //    }
        //    var modalInstance = $uibModal.open({
        //        templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/FormWarningLetter.html',
        //        controller: 'FormWarningLetterCtrl',
        //        controllerAs: 'frmWarningLetterCtrl',
        //        resolve: {
        //            item: function () {
        //                return data;
        //            }
        //        }
        //    });
        //    modalInstance.result.then(function () {
        //        vm.jLoad(1);
        //    });
        //}

        //vm.ubah_aktif = ubah_aktif;
        //function ubah_aktif(data, active) {
        //    UIControlService.loadLoading("Silahkan Tunggu");
        //    WarningLetterService.editActive({
        //        LetterID: data.LetterID,
        //        IsActive: active
        //    }, function (reply) {
        //        UIControlService.unloadLoading();
        //        if (reply.status === 200) {
        //            var msg = "";
        //            if (active === false) msg = " NonAktifkan ";
        //            if (active === true) msg = "Aktifkan ";
        //            UIControlService.msg_growl("success", "Data Berhasil di " + msg);
        //            jLoad(1);
        //        }
        //        else {
        //            UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
        //            return;
        //        }
        //    }, function (err) {

        //        UIControlService.msg_growl("error", "Gagal Akses API ");
        //        UIControlService.unloadLoading();
        //    });

        //}

        //vm.lihatDetail = lihatDetail;
        //function lihatDetail(data) {
        //    console.info("masuk form add/edit");
        //    var data = {
        //        item: data
        //    }
        //    var modalInstance = $uibModal.open({
        //        templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/DetailWarningLetter.html',
        //        controller: 'DetailWarningLetterCtrl',
        //        controllerAs: 'DetailwarningLetterCtrl',
        //        resolve: {
        //            item: function () {
        //                return data;
        //            }
        //        }
        //    });
        //    modalInstance.result.then(function () {
        //        vm.jLoad(1);
        //    });
        //}


    }
})();
//TODO



















//angular.module('eprocAppPanitia')

//.controller( 'pengaduanCtrl', function( $scope, $rootScope, $modal, $state, $cookieStore, $http ){
//    $scope.warningletter = [];
//    $scope.init = function(){
//        $scope.pengaduanlist = [
//            {id: 1, nm_pengadu:"Michael", department_pengadu:"DHS", tgl:"3-06-2016", vendor:"CV. Maju Karya",ket:"pelanggaran lalin"},
//            {id: 2, nm_pengadu:"Steven", department_pengadu:"Finance", tgl:"3-06-2016", vendor:"PT. Abadi Sentosa", ket:"pemalsuan bukti pembayaran"}
//        ];
//    };
    
//    $scope.formpengaduan = function(data,id) {
//        var kirim_data ={ id:id, data: data};
//        var modalInstance = $modal.open({
//            templateUrl: 'formPengaduan.html',
//            controller: formPengaduan,
//            resolve: {
//                item: function() {
//                    return kirim_data;
//                }
//            }
//        });
//        modalInstance.result.then(function() {
//            $scope.init();
//        });
//    };
    
//});
//var formPengaduan = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
//    $scope.id_data = item.id;
//    $scope.data = item.data;
//    $scope.action = "";
//    $scope.rincian = "";
//    $scope.customTinymce = {
//        theme: "modern",
//        plugins: [
//            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
//            "searchreplace wordcount visualblocks visualchars fullscreen",
//            "insertdatetime media nonbreaking save table contextmenu directionality",
//            "emoticons template paste textcolor"
//        ],
//        toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
//        toolbar2: "print preview media | forecolor backcolor",
//        image_advtab: true,
//        height: "200px",
//        width: "auto"
//    };
    
//    $scope.init = function(){
//        if($scope.id_data === 0){ $scope.action = "Tambah "; } else { $scope.action = "Ubah "; }
//    };
        
//    $scope.batal = function() {
//        $modalInstance.dismiss('cancel');
//    };    
//};

//var formGenerate = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
//    $scope.batal = function() {
//        $modalInstance.dismiss('cancel');
//    };
//};