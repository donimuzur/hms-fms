(function () {
    'use strict';

    angular.module("app")
    .controller("loginRekananCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PengumumanPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PengumumanPengadaanService, UIControlService) {
        /* jshint validthis: true */
        var vm = this;
        vm.panels = [];
        vm.passCaptcha = "";
        
        // functions
        vm.init = init;
        vm.Captcha = Captcha;

        // function declarations
        function init() {
            // Load partial traslastion
            $translatePartialLoader.addPart('login-rekanan');
            Captcha();
        }
        
        function Captcha() {
            vm.passCaptcha = UIControlService.generateCaptcha('textCanvas', 'image');
        }
    }
})();
/*
angular.module('eprocApp')
    .controller('loginRekananCtrl', function($scope, $cookieStore, $state, $rootScope, $modal, $http) {
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.maxSize = 1;
        $scope.srcText = "";
        $scope.pemenang = [];
        $scope.failedMsg = "";
        
        $scope.init = function(){
            $scope.Captcha();
        };
        //start chaptcha
        $scope.Captcha = function(){
            var alpha = new Array('A','B','C','D','E','F','G','H','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                                  'a','b','c','d','e','f','g','h','i','j','k','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                                  '1','2','3','4','5','6','7','8','9','0');

            var tCtx = document.getElementById('textCanvas').getContext('2d');
            var imageElem = document.getElementById('image');

            for( var i = 0; i < 3; i++ ){
                var a = alpha[ Math.floor( Math.random() * alpha.length ) ];
                var b = alpha[ Math.floor( Math.random() * alpha.length ) ];
                var c = alpha[ Math.floor( Math.random() * alpha.length ) ];
                var d = alpha[ Math.floor( Math.random() * alpha.length ) ];
            }

            var code = a +' '+ b +' '+ c +' '+ d;

            tCtx.canvas.width = tCtx.measureText( code ).width;
            tCtx.fillText( code, 0, 10 );
            imageElem.src = tCtx.canvas.toDataURL();
            $scope.passCaptcha = $.md5( removeSpaces(code) );
        };

    function removeSpaces( string ){
        return string.split(' ').join('');
    }

        $scope.validCaptcha = function() {
            var string1 = $scope.passCaptcha;
            var string2 = $.md5( removeSpaces( document.getElementById('txtInput').value ) );

            if( string1 === string2 ) return true;
            else {
                $.growl.error({title: "[PERINGATAN]", message: "Ups, Captcha yang anda masukan salah!!"});
                $rootScope.unloadLoading();

                document.getElementById("txtInput").value = "";
                $scope.Captcha();
                return false;
            }
        };
        //end chaptcha
        
        /* start login processs */
//        $scope.login = function(){
//            if( $.trim($scope.username) === "" || $.trim($scope.password) === "" ){
//            $.growl.warning({ message: "Masukkan username dan password!!" });
//            return;
//            }
//            //ngProgress.start();
//            $rootScope.loadLoading( "Verifying credentials..." );
//
//            if( $scope.validCaptcha() )
//            {
//                $http.post($rootScope.url_api+"auth/login/vendor", {
//                username: $scope.username,
//                password: $scope.password,
//                api_key: $rootScope.api_key
//            })
//            .success(function(result) {
//                console.log("result: " + JSON.stringify(result));
//                if(result.status === 200) {
//                    var data = result.data;
//                    //console.log('data : ' + JSON.stringify(data));
//                    $rootScope.unloadLoading();
//                    if(data.success) {
//                        $cookieStore.put("vendor_sessId", data.session_id);
//                        $http.post($rootScope.url_api+"auth/get_session", {
//                        session_id: $cookieStore.get("vendor_sessId")
//                        })
//                        .success(function(result2) {
//                            if(result2.status === 200) {
//                                $rootScope.userSession = result2.data;
//                                $.growl.notice({ message: 'Login berhasil! Selamat datang, ' + $rootScope.userSession.session_data.nama_perusahaan });
//                                $state.go('dashboard-vendor');
//                                $rootScope.isLogged = true;
//                            }
//                        })
//                        .error(function(err){
//                            $.growl.error({ message: "Gagal Akses API >"+ err });
//                    
//                        });
//                    } else {
//                        $.growl.error({ message: data.message });
//
//                    }
//                } else {
//                    $rootScope.unloadLoading();
//                     $.growl.error({ message: result.message });
//                }
//            })
//            .error(function(error){
//                $rootScope.unloadLoading();
//                $.growl.error({ message: "Gagal Akses API >"+ error });
//            });
//            //$state.transitionTo('pengingat-aktivitas');
//            }
//        };
//        /* end login process*/
//        
//    });
//;
