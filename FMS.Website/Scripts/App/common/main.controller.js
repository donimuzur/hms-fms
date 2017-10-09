(function () {
	'use strict';

	angular.module("app").controller("MainController", mainController);

	mainController.$inject = ['$window', '$translate', '$location', 'SocketService', 'PageComponentService', 'AuthService', 'GlobalConstantService', 'UIControlService', 'NotificationService'];
	/* @ngInject */
	function mainController($window, $translate, $location, SocketService, PageComponentService, AuthService, GlobalConstantService, UIControlService, NotificationService) {

		/* jshint validthis: true */
		var vm = this;
		vm.initialize = initialize;
		vm.isActive = isActiveState;
		vm.setActive = setActiveState;
		vm.isLoggedIn = isLoggedIn;
		vm.moduleLayer = getModuleLayer;
		vm.logout = logout;
		vm.currUser = '';
		vm.currTime = '';
		vm.getCurrUser = getCurrUser;
		vm.getCurrLang = getCurrLang;
		vm.getLangList = getLangList;
		vm.setLang = setLang;
		vm.isAdmin = isAdmin;
		vm.redirect = redirect
		vm.requireHideSideMenu = hideSideMenu;
		vm.api_endpoint = GlobalConstantService.getConstant('api');
		vm.loadMenus = loadMenus;
		vm.jumlahRequestAktivasi = 0;
		vm.jumlahRequestVerifikasi = 0;
		vm.jumlahRequestUbahData = 0;
		vm.roles = localStorage.getItem('roles');
		function checkTime(i) {
			if (i < 10) {
				i = "0" + i;
			}  // add zero in front of numbers < 10

			return i;
		}

		var pageActive = 0;
		var isChildrenActive = false;
		var hideSideBar = ['/', '/panitia', '/home', '/vendor', '/pre-daftar', '/daftar', '/daftar_kuesioner', '/pengumuman-pengadaan-client', '/login-rekanan', '/login'];

		const MODULE_LAYER_ADMIN = 1;
		const MODULE_LAYER_VENDOR = 2;

		// function declarations
		function initialize() {
			vm.currUser = getCurrUser();
			//redirect();
			SocketService.emit("daftarRekanan");
			SocketService.emit("PermintaanUbahData");
			var root = document.location.protocol + "://" + document.location.hostname + document.location.pathname;
			PageComponentService.config("Application/PageComponent").then(function (response) {
				vm.pageComponent = response.data;
				//if (vm.isLoggedIn() === 'true' && vm.moduleLayer() === MODULE_LAYER_ADMIN)
				//	loadMenus();
			}, function (err) {
				console.info(JSON.stringify(err));
				UIControlService.handleRequestError(err.data, err.status);
			});
		}

		SocketService.on('feed', function (reply) {
			vm.currTime = reply.timestamp
		});

		SocketService.on('actionDaftarRekanan', function (reply) {
			if (isLoggedIn() === 'true' && (vm.currRoles.indexOf('APPLICATION.ROLE_ADMINPROC') !== -1 || vm.currRoles.indexOf('APPLICATION.ROLE_MGRPROC') !== -1)) {
				NotificationService.activationReqCount(function (reply) {
					if (reply.status === 200) {
						vm.jumlahRequestAktivasi = reply.data[0];
						vm.jumlahRequestVerifikasi = reply.data[1];
					}
				}, function (err) {
					//nothing to do here.
				});
			}
		});

		SocketService.on('actionPermintaanUbahData', function (reply) {
			if (isLoggedIn() === 'true' && (vm.currRoles.indexOf('APPLICATION.ROLE_ADMINPROC') !== -1 || vm.currRoles.indexOf('APPLICATION.ROLE_MGRPROC') !== -1)) {
				NotificationService.dataChgReqCount(function (reply) {
					if (reply.status === 200) {
						vm.jumlahRequestUbahData = reply.data;
					}
				}, function (err) {
					//nothing to do here.
				});
			}
		});

		SocketService.on('actionContractRequisitionApproval', function (reply) {
			if (isLoggedIn() === 'true') {
				NotificationService.dataCntrctReqApprv(function (reply) {
					if (reply.status === 200) {
						vm.jumlahRequestUbahData = reply.data;
					}
				}, function (err) {
					//nothing to do here.
				});
			}
		});

		function redirect() {
			if (isLoggedIn() === 'true') {
				if (isAdmin()) {
					$location.path('/homeadmin');
				}
			} else {
				return false;
			}
		}

		function loadMenus() {
			SocketService.emit("daftarRekanan");
			SocketService.emit("PermintaanUbahData");
			getCurrLang();
			console.log("Start loading menus . . .");
			AuthService.getMenus(function (response) {
			    vm.menus = mapMenu(response.data);
			    console.info(vm.menus);
			}, function (response) {
				if (!hideSideMenu()) {
					UIControlService.handleRequestError(response.Message);
					$location.path('/login');
				}
			});
		}

		function mapMenu(menus) {
			var result = [];
			for (var i = 0; i < menus.length; i++) {
				var submenus = [];
				if (menus[i].Children.length > 0) {
					for (var j = 0; j < menus[i].Children.length; j++) {
						var subs = [];
						var current = menus[i].Children[j];
						if (current.Children.length > 0) {
							for (var k = 0; k < current.Children.length; k++) {
								subs.push({
									title: current.Children[k].Label,
									state: current.Children[k].StateName,
									iconClass: current.Children[k].Icon,
									IsChecked: current.Children[k].IsChecked,
									submenu: []
								});
							}
						}
						submenus.push({
							title: current.Label,
							state: current.StateName,
							iconClass: current.Icon,
							IsChecked: current.IsChecked,
							submenu: subs
						});
					}
				}
				result.push({
					title: menus[i].Label,
					state: menus[i].StateName,
					iconClass: menus[i].Icon,
					IsChecked: menus[i].IsChecked,
					submenu: submenus
				});
			}
			vm.menuLoaded = true;
			return result;
		}

		vm.menujuAktivasi = menujuAktivasi;
		function menujuAktivasi() {
			$location.path('/data-rekanan/verifikasi-data');
		}

		vm.toChangeRequest = toChangeRequest;
		function toChangeRequest() {
			$location.path('/data-rekanan/cr-openlock');
		}

		function isActiveState(path) {
			//return path === $location.path();
			return pageActive === path;
		}

		function logout() {
			$location.path('/logout');
			//localStorage.removeItem('eProcValeToken');
			//localStorage.removeItem('eProcValeRefreshToken');
			//localStorage.removeItem('roles');
			//localStorage.removeItem('sessEnd');
			//localStorage.removeItem('username');
			//localStorage.removeItem('login');
			//localStorage.removeItem('moduleLayer');
			//$window.location.reload();
		}

		function setActiveState(state) {
			pageActive = state;
			//pageActive = state;
			//isChildrenActive = childrenActive;
		}

		function setLang(lang) {
			localStorage.setItem('currLang', lang);
			$translate.preferredLanguage(getCurrLang()); //untuk ganti bahasa
			$window.location.reload();
		}

		function getCurrUser() {
			if (localStorage.getItem('username') === null || localStorage.getItem("username") === '') return '';
			if (localStorage.getItem('roles') === null || localStorage.getItem('roles') === '') return '';
			var usr = localStorage.getItem("username").toString();
			var roles = localStorage.getItem('roles').toString().split(',');
			//alert(usr);
			vm.currUser = usr;
			vm.currRoles = roles;
			return usr;
		}

		function getCurrLang() {
			if (localStorage.getItem("currLang") === null || localStorage.getItem("currLang") === '')
				return '';

			return localStorage.getItem("currLang");
		}

		function isLoggedIn() {
		    //console.log("Is Logged In: " + GlobalConstantService.getLoginState());
			return GlobalConstantService.getLoginState();// === "true" || GlobalConstantService.getLoginState() === 1;
			//return GlobalConstantService.getLoginState() === "true" || GlobalConstantService.getLoginState() === 1;
		}

		function getModuleLayer() {
			return Number(GlobalConstantService.getModuleLayer());
		}

		var langList = new Array();
		langList = [{
			id: 'id',
			bahasa: 'Bahasa Indonesia'
		}, {
			id: 'en',
			bahasa: 'English'
		}];
		function getLangList() {
			//var langList = new Array();
			//langList = ['id', 'en'];

			return langList;
		}

		function isAdmin() {
			var result = false;

			if (localStorage.getItem("roles") === null || localStorage.getItem("roles") === '') return false;
			var roles = localStorage.getItem("roles").toString().split(',');

			for (var i = 0; i < roles.length; i++)
				if (roles[i] === "SYSTEM.ROLE_ADMIN")
					result = true;

			return result;

			//return localStorage.getItem("moduleLayer");
			//return getModuleLayer() === MODULE_LAYER_ADMIN;
		}

		function hideSideMenu() {
			var path = $location.path();
			//console.info(path);
			return hideSideBar.indexOf(path) >= 0;
		}

		// services and events
		SocketService.on('feed', function (data) {
			vm.serverTime = data.timestamp;
		});
	}
})();