(function () {
	'use strict';

	angular.module("app").controller("AdminLoginController", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$state', '$window', 'AuthService', 'UIControlService', '$scope'];

	/* @ngInject */
	function ctrl($http, $translate, $translatePartialLoader, $state, $window, AuthService, UIControlService, $scope) {
		var vm = this;

		vm.username = "";
		vm.password = "";
		vm.passCaptcha = "";
		vm.tampung = [];
		vm.init = init;
		vm.captcha = captcha;
		vm.validCaptcha = validCaptcha;
		vm.login = login;

		function init() {
			vm.captcha();
		}

		function captcha() {
			var alpha = new Array('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
				'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
				'1', '2', '3', '4', '5', '6', '7', '8', '9', '0');

			var i;
			var tCtx = document.getElementById('textCanvas').getContext('2d');
			var imageElem = document.getElementById('image');

			for (i = 0; i < 4; i++) {
				var a = alpha[Math.floor(Math.random() * alpha.length)];
				var b = alpha[Math.floor(Math.random() * alpha.length)];
				var c = alpha[Math.floor(Math.random() * alpha.length)];
				var d = alpha[Math.floor(Math.random() * alpha.length)];
			}

			var code = a + ' ' + b + ' ' + c + ' ' + d;

			tCtx.canvas.width = tCtx.measureText(code).width;
			tCtx.fillText(code, 0, 10);
			imageElem.src = tCtx.canvas.toDataURL();
			vm.passCaptcha = $.md5(removeSpaces(code));
		}

		function validCaptcha() {
			var string1 = vm.passCaptcha;
			var string2 = $.md5(removeSpaces(document.getElementById('txtInput').value));

			if (string1 === string2) {
				return true;
			} else {
				alert('Ups, wrong captcha!');
				document.getElementById("txtInput").value = "";
				vm.captcha();
				return false;
			}
		}

		function removeSpaces(string) {
			return string.split(' ').join('');
		}

		vm.password = "admin123";
		function login() {
			if ($.trim(vm.username) == "") {
				//$.growl.warning({ message: "Masukkan username dan password!!" });
				return;
			}
		    //if (validCaptcha()) {
				AuthService.login({
					username: vm.username,
					password: vm.password,
				}, function (reply) {
					if (reply.status === 200) {
						localStorage.removeItem('eProcValeToken');
						localStorage.removeItem('eProcValeRefreshToken');
						localStorage.setItem('eProcValeToken', reply.data.access_token);
                        localStorage.setItem('eProcValeRefreshToken', reply.data.refresh_token);
					    console.log("reply",reply);
						localStorage.setItem('login', true);
						localStorage.setItem('sessEnd', new Date().setSeconds(new Date().getSeconds() + reply.data.expires_in));
						localStorage.setItem('roles', JSON.parse(reply.data.roles));
						localStorage.setItem('moduleLayer', 1);
						localStorage.setItem('username', JSON.parse(reply.data.username));
						//$scope.main.loadMenus();
					    //$state.go('csf-dashboard');
					    
						AuthService.getUserLogin(function (reply) {
							if (reply.status === 200) {
								$scope.main.getCurrUser();
								console.info(JSON.stringify(reply));
								//console.info(reply.data.CurrentUsername);
								AuthService.getRoleUserLogin({ Keyword: reply.data.CurrentUsername }, function (reply1) {
								    console.info(JSON.stringify(reply1));
								    if (reply1.status === 200 && reply1.data.List.length > 0) {
										var role = reply1.data.List[0].RoleName;
										UIControlService.msg_growl("notice", 'NOTIFICATION.LOGIN.SUCCESS.MESSAGE', "NOTIFICATION.LOGIN.SUCCESS.TITLE");
										//if (role === 'APPLICATION.ROLE_VENDOR') {
										//	$state.go('dashboard-vendor');
										//} else if (role === 'APPLICATION.ROLE_VENDOR_INTERNATIONAL') {
										//	$state.go('dashboard-vendor');
										//} else {
										//	$state.go('csf-dashboard');
									    //}
										if (role === 'HR') {
										    $state.go('csf-dashboard');
										} else if (role === 'FLEET') {
										    $state.go('CSF - WTC');
										} else {
										    $state.go('CAF-Dashboard'); 
										}
									} else {
										UIControlService.msg_growl("error", "User Tidak Berhak Login");
										//$state.go('home');
									}
								}, function (err1) {
								    //UIControlService.msg_growl("error", "MESSAGE.API");
								    $state.go('csf-dashboard');
								});
							}
						}, function (err) {
						    $state.go('CSF - WTC');
						});
						//$state.go('homeadmin');
					} else {
						UIControlService.msg_growl("error", "error");
						//alert('error');
					}
					//$rootScope.unloadLoading();
				}, function (error) {
					alert(error.data.error);
					document.getElementById("txtInput").value = "";
					vm.captcha();
					return false;
					//$.growl.error({ message: "Gagal Akses API >" + err });
					//$rootScope.unloadLoading();
				});

				//    console.log("session id: " + $cookieStore.get("sessId"));
				//    if ($cookieStore.get("sessId") != undefined && $cookieStore.get("sessId") != null) {
				//        $rootScope.getSession().then(function (result) {
				//            //console.log("get session: " + JSON.stringify(result));
				//            $rootScope.userSession = result.data.data;
				//            $rootScope.userLogin = $rootScope.userSession.username;
				//            $.growl.notice({ message: 'Login berhasil! Selamat datang, ' + $rootScope.userSession.session_data.nama_pegawai });
				//            $state.go('homeadmin');
				//            $rootScope.isLogged = true;
				//        });
				//    }
				//    $http.post($rootScope.url_api + "auth/login/admin", {
				//        username: $scope.username,
				//        password: $scope.password,
				//        api_key: $rootScope.api_key
				//    })
				//    .success(function (result) {
				//        //console.log("result: " + JSON.stringify(result));
				//        if (result.status === 200) {
				//            $rootScope.unloadLoading();
				//            var data = result.data;
				//            //console.log('data : ' + JSON.stringify(data));
				//            if (data.success) {
				//                $cookieStore.put("sessId", data.session_id);
				//                $http.post($rootScope.url_api + "auth/get_session", {
				//                    session_id: $cookieStore.get("sessId")
				//                })
				//                .success(function (result2) {
				//                    if (result2.status === 200) {
				//                        $rootScope.userSession = result2.data;
				//                        $.growl.notice({ message: 'Login berhasil! Selamat datang, ' + $rootScope.userSession.session_data.nama_pegawai });
				//                        $state.go('homeadmin');
				//                        $rootScope.isLogged = true;
				//                    }
				//                })
				//                .error(function (err) {
				//                    $.growl.error({ message: "Gagal Akses API >" + err });

				//                });
				//            } else {
				//                $.growl.error({ message: data.message });

				//            }
				//        } else {
				//            $rootScope.unloadLoading();
				//            $.growl.error({ message: result.message });
				//        }
				//    })
				//    .error(function (error) {
				//        $rootScope.unloadLoading();
				//        $.growl.error({ message: "Gagal Akses API >" + error });
				//    });
			//}  end: if $scope.validCaptcha()
		}
	}
})();