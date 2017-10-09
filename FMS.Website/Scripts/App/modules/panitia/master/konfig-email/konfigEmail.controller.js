(function() {
    'use strict';

    angular.module("app")
            .controller("KonfigEmailCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'KontenEmailService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, KontenEmailService) {

        var vm = this;
        
        vm.konten_emails = [];
        vm.filter = '';
        vm.count = 0;
        vm.currentPage = 2;
        vm.pageSize = 10;

        vm.init = init;
        function init(){
            $translatePartialLoader.addPart('konfig-email');
            loadKontenEmails();
        };

        vm.onPageChanged = onPageChanged;
        function onPageChanged(currentPage){
            vm.currentPage = currentPage;
            loadKontenEmails();
        };

        vm.onFilterChanged = onFilterChanged;
        function onFilterChanged(currentPage){
            loadKontenEmails();
        };

        vm.onEditClick = onEditClick;
        function onEditClick(konten_email){
            var modalInstance = $modal.open({
                templateUrl: 'editKonten.html',
                controller: editKontenCtrl,
                resolve: {
                    konten_email: function() {
                        var ke = {
                            id_konten_email:konten_email.id_konten_email,
                            nama_email:konten_email.nama_email,
                            konten_email:konten_email.konten_email,
                            subject_email:konten_email.subject_email
                        };
                        return ke;
                    }
                }
            });
            modalInstance.result.then(function() {
                loadKontenEmails();
            });  
        };

        vm.onConfigVariablesClick = onConfigVariablesClick;
        function onConfigVariablesClick(){
            $state.transitionTo("email-configuration-variables");
        };

        vm.onEditSignatureClick = onEditSignatureClick;
        function onEditSignatureClick(){
            var modalInstance = $modal.open({
                templateUrl: 'editSignature.html',
                controller: editSignatureCtrl
            });
            modalInstance.result.then(function() {
                loadKontenEmails();
            });  
        };

        function loadKontenEmails(){

            $rootScope.authorize(
                //itp.mailconfig.getPagedMailContents
                $http.post($rootScope.url_api + 'mailconfig/select',
                {
                    filter: vm.filter,
                    offset: vm.pageSize * (vm.currentPage - 1),
                    limit: vm.pageSize
                }).success(function(reply){
                    if(reply.status === 200){
                        vm.konten_emails = reply.result.data.konten_emails;
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
var editKontenCtrl = function($scope, $modalInstance, $http, konten_email, $cookieStore, $rootScope) {
    
    $scope.konten_email = {};
    $scope.variabel_email_globals = {};
    $scope.variabel_email_lokals = {};
    
    $scope.init = function() {
        $scope.konten_email = konten_email;
        $rootScope.authorize(loadVariables());
    };
    
    $scope.onCancelClick = function() {
        $modalInstance.dismiss('cancel');
    };
    
    $scope.onSimpanClick = function() {
        //itp.mailconfig.saveKontenEmail
        $rootScope.authorize(
            $http.post($rootScope.url_api + 'mailconfig/save',
            {
                konten_email:$scope.konten_email
            }).success(function(reply){
                if (reply.status === 200){
                    $.growl.notice({title: "[INFO]", message: "Konten Email berhasil diubah"});
                    $modalInstance.close();
                } else {
                    $.growl.error({title: "[WARNING]", message: "Konten Email gagal diubah"});
                }
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            })
        );
    };
    
    $scope.customTinymce = {
        theme: "modern",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
        ],
        toolbar1: "undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent",
        toolbar2: "forecolor backcolor",
        image_advtab: true,
        height: "200px",
        width: "auto"
    };
    
    function loadVariables(){
        //itp.mailconfig.getMailVariables
        $http.post($rootScope.url_api + 'mailconfig/var/select',
        {
            id_konten_email:konten_email.id_konten_email
        }).success(function(reply){
            if (reply.status === 200){
                $scope.variabel_email_globals = reply.result.data.variabel_email_globals;
                $scope.variabel_email_lokals = reply.result.data.variabel_email_lokals;
            }
        }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
        });
    };
};

//TODO
var editSignatureCtrl = function($scope, $modalInstance, $http, $cookieStore, $rootScope) {
    
    $scope.variabel_email_global = {};
    $scope.signature;
    
    $scope.init = function() {
        $rootScope.authorize(
            //itp.mailconfig.getVariableById
            $http.post($rootScope.url_api + 'mailconfig/var/selectbyid',
            {
                id_variabel_global: 1
            }).success(function(reply){
                if (reply.status === 200){
                    $scope.variabel_email_global = reply.result.data.variabel_email_global;
                    $scope.variabel_email_global.namaAwal = reply.result.data.variabel_email_global.nama;
                    tinymce.get('signature').setContent(reply.result.data.variabel_email_global.nilai);
                } else {
                    $.growl.error({title: "[WARNING]", message: "Data signature gagal diload"});
                }
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            })
        );
    };
    
    $scope.onCancelClick = function() {
        $modalInstance.dismiss('cancel');
    };
    
    $scope.onSimpanClick = function() {
        $rootScope.authorize(
            //itp.mailconfig.updateVariable
            $http.post($rootScope.url_api + 'mailconfig/varlok/update',
            {
                variabel_email_global:$scope.variabel_email_global
            }).success(function(reply){
                if (reply.status === 200){
                    $.growl.notice({title: "[INFO]", message: "Signature email berhasil diubah"});
                    $modalInstance.close();
                } else {
                    $.growl.error({title: "[WARNING]", message: "Signature email gagal diubah"});
                }
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            })
        );
    };
    
    $scope.customTinymce = {
        theme: "modern",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
        ],
        toolbar1: "undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent",
        toolbar2: "forecolor backcolor",
        image_advtab: true,
        height: "200px",
        width: "auto"
    };    
};