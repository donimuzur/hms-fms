(function() {
    'use strict';

    angular.module("app")
            .controller("KonfigVarGlobalCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'KontenEmailService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, KontenEmailService) {
            
        var vm = this;
        
        vm.variabel_email_globals = [];
        vm.filter = '';
        vm.count = 0;
        vm.currentPage = 2;
        vm.pageSize = 10;

        vm.init = init;
        function init(){
            $translatePartialLoader.addPart('konfig-variabel-email');
            loadVariables();
        };

        vm.onPageChanged = onPageChanged;
        function onPageChanged(currentPage){
            vm.currentPage = currentPage;
            loadVariables();
        };

        vm.onFilterChanged = onFilterChanged;
        function onFilterChanged(currentPage){
            loadVariables();
        };

        vm.onEditClick = onEditClick;
        function onEditClick(variabel_email_global){
            var modalInstance = $modal.open({
                templateUrl: 'editVariabel.html',
                controller: editVariabelCtrl,
                resolve: {
                    variabel_email_global: function() {
                        var veg = {
                            id_variabel_global:variabel_email_global.id_variabel_global,
                            nama:variabel_email_global.nama,
                            nilai:variabel_email_global.nilai
                        };
                        return veg;
                    }
                }
            });
            modalInstance.result.then(function() {
                loadVariables();
            });  
        };

        vm.onDeleteClick = onDeleteClick;
        function onDeleteClick(variabel_email_global){
            var modalInstance = $modal.open({
                templateUrl: 'deleteVariabel.html',
                controller: deleteVariabelCtrl,
                resolve: {
                    variabel_email_global: function() {
                        var veg = {
                            id_variabel_global:variabel_email_global.id_variabel_global,
                            nama:variabel_email_global.nama
                        };
                        return veg;
                    }
                }
            });
            modalInstance.result.then(function() {
                loadVariables();
            });  
        };

        vm.onTambahClick = onTambahClick;
        function onTambahClick(){
            var modalInstance = $modal.open({
                templateUrl: 'editVariabel.html',
                controller: editVariabelCtrl,
                resolve: {
                    variabel_email_global: function() {
                        var veg = {
                            id_variabel_global:null,
                            nama:"",
                            nilai:""
                        };
                        return veg;
                    }
                }
            });
            modalInstance.result.then(function() {
                loadVariables();
            });  
        };

        function loadVariables(){
            //itp.mailconfig.getPagedMailVariables
            $rootScope.authorize(
                $http.post($rootScope.url_api + 'mailconfig/var/selectpaged',
                {
                    filter: vm.filter,
                    offset: vm.pageSize * (vm.currentPage - 1),
                    limit: vm.pageSize
                }).success(function(reply){
                    if(reply.status === 200){
                        vm.variabel_email_globals = reply.result.data.variabel_email_globals;
                        vm.count = reply.result.data.count;
                    }
                }).error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                })
            );
        }
    }
})();

//TODO
var editVariabelCtrl = function($scope, $modalInstance, $http, variabel_email_global, $cookieStore, $rootScope) {
    
    $scope.variabel_email_global = {};
    
    $scope.init = function() {
        $scope.variabel_email_global = variabel_email_global;
        $scope.variabel_email_global.namaAwal = variabel_email_global.nama;
    };
    
    $scope.onCancelClick = function() {
        $modalInstance.dismiss('cancel');
    };
    
    $scope.onSimpanClick = function() {
        
        var address;
        if ($scope.variabel_email_global.id_variabel_global){
            //itp.mailconfig.updateVariable
            address = 'mailconfig/varlok/update';
        } else {
            //itp.mailconfig.insertVariable
            address = 'mailconfig/varlok/insert';
        }
        
        $rootScope.authorize(
            $http.post($rootScope.url_api + address,
            {
                variabel_email_global:$scope.variabel_email_global
            }).success(function(reply){
                if (reply.status === 200){
                    $.growl.notice({title: "[INFO]", message: "Variabel berhasil disimpan"});
                    $modalInstance.close();
                } else {
                    var errorMessage = reply.result.data.message ? reply.result.data.message : "Variabel gagal disimpan";
                    $.growl.error({title: "[WARNING]", message: errorMessage});
                }
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            })
        );
    };
};

//TODO
var deleteVariabelCtrl = function($scope, $modalInstance, $http, variabel_email_global, $cookieStore, $rootScope) {
    $scope.variabel_email_global = {};
    $scope.init = function() {
        $scope.variabel_email_global = variabel_email_global;
    };
    
    $scope.onCancelClick = function() {
        $modalInstance.dismiss('cancel');
    };
    
    $scope.onConfirmClick = function() {
        $rootScope.authorize(
            //itp.mailconfig.deleteVariable
            $http.post($rootScope.url_api + 'mailconfig/varlok/delete',
            {
                id_variabel_global:$scope.variabel_email_global.id_variabel_global
            }).success(function(reply){
                if (reply.status === 200){
                    $.growl.notice({title: "[INFO]", message: "Variabel berhasil dihapus"});
                    $modalInstance.close();
                } else {
                    $.growl.error({title: "[WARNING]", message: "Variabel gagal dihapus"});
                }
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            })
        );
    };
};