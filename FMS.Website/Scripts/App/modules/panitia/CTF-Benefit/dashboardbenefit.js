(function () {
    'use strict';

    angular.module("app").controller("ctfdashboardctrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', '$state', 'ApprovalService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, $state, ApprovalService) {

        var vm = this;
        vm.init = init;
        vm.onSearchClick = onSearchClick;
        vm.loadApproval = loadApproval;
        vm.onMenujuAppClick = onMenujuAppClick;

        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;

        vm.appSrcText = '';
        vm.appPageSize = 10;
        vm.appCurrPage = 1;
        vm.approval = [];
        vm.appCount = 0;

        function init() {
            $translatePartialLoader.addPart('dashboard-panitia');
            //loadApproval(1);
        }

        function onSearchClick(appSrcText) {
            vm.appSrcText = appSrcText;
            vm.loadApproval(1);
        }

        function loadApproval(page) {
            //$rootScope.loadLoading('Silahkan Tunggu...');
            vm.appCurrPage = page;
            ApprovalService.selectByUser({
                pegawai_id: $rootScope.userSession.session_data.pegawai_id,
                offset: (page - 1) * vm.appPageSize,
                limit: vm.appPageSize,
                search: vm.appSrcText
            }, function (reply) {
                if (reply.status === 200) {
                    vm.approval = reply.result.data.result;
                    vm.approval.forEach(function (a) {
                        a.tgl_mulai = a.tgl_mulai ? $rootScope.convertTanggalWaktu(a.tgl_mulai) : '';
                        a.tgl_selesai = a.tgl_selesai ? $rootScope.convertTanggalWaktu(a.tgl_selesai) : '';
                    });
                    vm.appCount = reply.result.data.count;
                }
                $rootScope.unloadLoading();
            }, function (error) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                $rootScope.unloadLoading();
            });
        }

        function onMenujuAppClick(app) {
            $state.transitionTo('approval-master', {
                flowpaket_id: app.flow_paket_id,
                paket_lelang_id: app.paket_id
            });
        };
    }
})();

/*
.controller('dashboardCtrl', function( $scope, $rootScope, $state, $cookieStore, $http){ // alert("Tekan Tombol Refresh (F5)");
    $scope.totalItems = 0;
    $scope.currentPage = 1;
    $scope.maxSize = 10;
    
    $scope.appSrcText = '';
    $scope.appPageSize = 10;
    $scope.appCurrPage = 1;
    $scope.approval = [];
    $scope.appCount = 0;

    $scope.init = function(){
        $rootScope.getSession().then(function(result){
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize(bacaNotif());
            $scope.loadApproval(1);
        });
        //bacaNotif(); // AWN | old: $rootScope.readNotif();
    };   
    
    $scope.onSearchClick = function(appSrcText){
        $scope.appSrcText = appSrcText;
        $scope.loadApproval(1);
    };
    
    $scope.loadApproval = function(page){
        $rootScope.loadLoading('Silahkan Tunggu...');
        $scope.appCurrPage = page;
        $rootScope.authorize(
            $http.post($rootScope.url_api + 'approval/select/byuser', {
                pegawai_id: $rootScope.userSession.session_data.pegawai_id,
                offset: (page - 1) * $scope.appPageSize,
                limit: $scope.appPageSize,
                search: $scope.appSrcText
            }).success(function(reply) {
                if (reply.status === 200) {
                    $scope.approval = reply.result.data.result;
                    $scope.approval.forEach(function(a){
                        a.tgl_mulai = a.tgl_mulai ? $rootScope.convertTanggalWaktu(a.tgl_mulai) : '';
                        a.tgl_selesai = a.tgl_selesai ? $rootScope.convertTanggalWaktu(a.tgl_selesai) : '';
                    });
                    $scope.appCount = reply.result.data.count;
                }
                $rootScope.unloadLoading();
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                $rootScope.unloadLoading();
                return;
            })
        );
    };
    
    $scope.onMenujuAppClick = function(app){
        $state.transitionTo('approval-master', {
            flowpaket_id: app.flow_paket_id,
            paket_lelang_id: app.paket_id
        });
    };
    
    function bacaNotif(){ 
        
//        eb.send( auth, {sessionID: sess}, function( authReply ){ // AWN-Auth-Step4
//            if( authReply.status === 'ok' ){
//                $rootScope.userlogged = authReply.username; // AWN-Auth-Step5
//                
//                $rootScope.readNotif(); // AWN
//                
//            } else { // AWN-Auth-Step6
//                $rootScope.isLogged = false;
//                $rootScope.userLogged = "";
//                $state.transitionTo('login');                                                
//            }
//        }); // end: AWN-Auth-Step7            
    } // end bacaNotif     
});
*/