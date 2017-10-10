(function () {
	'use strict';

	angular.module("app").controller("BeritaController", ctrl);

	ctrl.$inject = ['AuthService', '$translatePartialLoader', 'NewsService', '$uibModal'/*'$http', '$translate', '$location', 'SocketService' */];

	function ctrl(AuthService, $translatePartialLoader, NewsService, $uibModal/*$http, $translate, $location, SocketService */) {
		var vm = this;
		var page_id = 158;

		vm.berita = [];
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.pageSize = 10;
		//vm.offset = (current * 10) - 10;
		vm.orderBy = 1;
		vm.isAsc = true;

		//address belus diregister, jadi dibuat true semua
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;
		vm.allowControl;

		vm.kata = new Kata("");

		//tanggal
		var currentDate = new Date();
		var bln = currentDate.getMonth() + 1;
		var tgl = currentDate.getDate();

		if (bln < 10) {
			bln = "0" + bln;
		} else {
			bln = bln;
		}

		if (tgl < 10) {
			tgl = "0" + tgl;
		} else {
			tgl = tgl;
		}

		vm.tgl = currentDate.getFullYear() + "-" + bln + "-" + tgl;

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('master-berita');
			vm.jLoad(1);
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			//$rootScope.loadLoading('Silahkan Tunggu...');
			//$rootScope.authorize(
			NewsService.select({
				search: vm.kata.srcText,
				keyword: vm.kata.srcText,
				pageNumber: vm.currentPage,
				pageSize: vm.pageSize,
				orderBy: vm.orderBy,
				isAsc: vm.isAsc
			}, function (reply) {
				//console.info("reply: "+JSON.stringify(reply));
				if (reply.status === 200) {
					vm.berita = reply.data;
					vm.totalItems = reply.data.length;
					//	} else {
					//		$.growl.error({ message: "Gagal mendapatkan data berita!!" });
					//		$rootScope.unloadLoading();
					//	}
					//	$rootScope.unloadLoading();
					//}, function (err) {
					//	$.growl.error({ message: "Gagal Akses API >" + err });
					//	$rootScope.unloadLoading();
					//});
				} else {
					//$.growl.error({ message: "Gagal mendapatkan data berita!!" });
					//$rootScope.unloadLoading();
					//return;
				}
				//$rootScope.unloadLoading();
			}, function (err) {
				$.growl.error({ message: "Gagal Akses API >" + err });
				$rootScope.unloadLoading();
			});
		}

		function loadBerita() {
			NewsService.count({
				page_id: page_id,
				keyword: "%" + vm.kata.srcText + "%"
			}, function (reply) {
				if (reply.status === 200) {
					var data = reply.result.data;
					vm.totalItems = data;
				} else {
					$.growl.error({ message: "Gagal mendapatkan data hak akses!" });
				}
			}, function (err) {
				$.growl.error({ message: "Gagal Akses API >" + err });
			});
		}

		vm.cariBerita = cariBerita;
		function cariBerita() {
			vm.jLoad(1);
		}

		vm.remHtml = remHtml;
		function remHtml(text) {
			return text ? String(text).replace(/<[^>]+>/gm, '') : '';
		}

		vm.createNews = createNews;
		function createNews() {
			var post = {
				newsTitle: "",
				newsContent: "",
				newsPicture: "",
				newsDate: vm.tgl
			};

			var modalInstance = $uibModal.open({
				templateUrl: 'tambahBerita.html',
				controller: tambahBeritaCtrl,
				controllerAs: 'tambahBeritaCtrl',
				resolve: { item: function () { return post; } }
			});
			modalInstance.result.then(function () {
				//            loadBerita();
				//            vm.jLoad(1);
				vm.init();
			});
		}

		vm.ubah = ubah;
		function ubah(berita) {
			var kirim = {
				newsID: berita.NewsID
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'tambahBerita.html',
				controller: editBeritaCtrl,
				resolve: { item: function () { return kirim; } }
			});
			modalInstance.result.then(function () {
				//                    loadBerita();
				//                    vm.jLoad(1);
				vm.init();
			});
		}

		/*fungsi delete berita */
		vm.delete = remove;
		function remove(id) {
			var modalInstance = $uibModal.open({
				templateUrl: 'delModalBerita.html',
				controller: delBerita,
				resolve: {
					item: function () {
						return id;
					}
				}
			});
			modalInstance.result.then(function () {
				//                    loadBerita();
				//                    vm.jLoad(1);
				vm.init();
			});
		}
	}
})();

//TODO
var tambahBeritaCtrl = function ($scope, item, GlobalConstantService, $uibModalInstance, NewsService/*$http, $rootScope*/) {
	var page_id = 158;
	$scope.berita = item;
	// $rootScope.fileuploadconfig(34);
	$scope.customTinymce = {
		theme: "modern",
		plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
		],
		toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
		toolbar2: "print preview media | forecolor backcolor",
		image_advtab: true,
		height: "200px",
		width: "auto"
	};

	$scope.file;
	$scope.filesTChanged = function (elm) {
		$scope.file = elm.files;
	};

	$scope.simpan = function () {
		if ($scope.berita.newsTitle === '' || $scope.berita.newsTitle === undefined) {
			//$.growl.error({ title: "[PERINGATAN]", message: "Judul berita belum diisi!!" });
			return;
		}
		if ($scope.berita.newsContent === '' || $scope.berita.newsContent === undefined) {
			//$.growl.error({ title: "[PERINGATAN]", message: "Isi berita belum diisi!!" });
			return;
		}
		insert(GlobalConstantService.getConstant("api_endpoint") + "/news/create");
		/*
         if ($scope.file === undefined) {
         insert("");
         }
         else {
         var fileInput = $('.upload-file');
         var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
         var maxSize = fileInput.data('max-size');
         if (fileInput.get(0).files.length) {
         var fileSize = fileInput.get(0).files[0].size;
         if (fileSize > maxSize) {
         $rootScope.unloadLoadingModal();
         $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
         return;
         } else {
         var restrictedExt = $rootScope.limitfiletype;
         if ($.inArray(extFile, restrictedExt) === -1) {
         $rootScope.unloadLoadingModal();
         $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
         return;
         } else {
         upload();
         }
         }
         }
         }
         */
	};


	/*function upload() {
     uploaderSvc.uploadFile(
     $scope.file,
     "berita"
     ).then(function (url) {
     if (!(url === 'failed')) {
     insert(url);
     } else
     $.growl.error({title: "[WARNING]", message: "Upload file penunjukkan gagal"});
     });
     }
     ;*/

	function insert(url) {
		var param = [];
		param.push($scope.berita.newsTitle);
		param.push($scope.berita.newsContent);
		param.push(url);
		//param.push($scope.berita.date_news);
		//        console.info("tgl: " + $scope.berita.date_news);
		//param.push($scope.user);

		//$http.post(url, {
		//	username: $scope.user,
		//	action: "add",
		//	param: param
		//}).success(function (reply) {
		//	if (reply.status === 200) {
		//		var msgpp = "Berhasil Simpan Berita!!";
		//		$.growl.notice({ title: "[INFO]", message: msgpp });
		//	} else {
		//		var msgpp = "Gagal Simpan Berita!!";
		//		$.growl.error({ title: "[PERINGATAN]", message: msgpp });
		//	}
		//	$scope.jLoad(1);
		//}).error(function (err) {
		//	$.growl.error({ message: "Gagal Akses API >" + err });
		//	return;
		//});

		NewsService.create({
			NewsContent: $scope.berita.newsContent,
			NewsTitle: $scope.berita.newsTitle,
			IsActive: true
		}, function (reply) {
			//console.info("reply: "+JSON.stringify(reply));
			if (reply.status === 200) {
				var data = reply.data;
				vm.berita = data;
				//select jumlah data
				//NewsService.count({
				//	//keyword: '%' + vm.kata.srcText
				//},
				//function (reply) {
				//	if (reply.status === 200) {
				//		var data = reply.result.data;
				vm.totalItems = data.length;
				//	} else {
				//		$.growl.error({ message: "Gagal mendapatkan data berita!!" });
				//		$rootScope.unloadLoading();
				//	}
				//	$rootScope.unloadLoading();
				//}, function (err) {
				//	$.growl.error({ message: "Gagal Akses API >" + err });
				//	$rootScope.unloadLoading();
				//});
			} else {
				//$.growl.error({ message: "Gagal mendapatkan data berita!!" });
				$rootScope.unloadLoading();
				return;
			}
			$rootScope.unloadLoading();
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			$rootScope.unloadLoading();
		});

		$uibModalInstance.close();
	}

	$scope.batal = function () {
		$uibModalInstance.dismiss('cancel');
	};
};

//TODO
var editBeritaCtrl = function ($scope, $filter, item, GlobalConstantService, $uibModalInstance, $uibModal, NewsService) {
	var page_id = 158;
	var vm = this;
	$scope.berita = item;

	NewsService.getNewsByID(item.newsID, function (reply) {
		if (reply.status === 200) {
			$scope.berita.newsTitle = reply.data.NewsTitle;
			$scope.berita.newsDate = $filter('date')(reply.data.NewsDate, 'dd - MMM - yyyy');
			$scope.berita.newsContent = reply.data.NewsContent;
			$scope.berita.isActive = reply.data.IsActive;
		} else {

		}
		//$rootScope.unloadLoading();
	}, function (err) {
		$.growl.error({ message: "Gagal Akses API >" + err });
		//$rootScope.unloadLoading();
	});

	//$rootScope.fileuploadconfig(34);
	$scope.customTinymce = {
		theme: "modern",
		plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
		],
		toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
		toolbar2: "print preview media | forecolor backcolor",
		image_advtab: true,
		height: "200px",
		width: "auto"
	};

	$scope.file;
	$scope.filesTChanged = function (elm) {
		$scope.file = elm.files;
		// $scope.$apply();
	};

	$scope.simpan = function () {
		if ($scope.berita.newsTitle === '' || $scope.berita.newsTitle === undefined) {
			//$.growl.error({ title: "[PERINGATAN]", message: "Judul berita belum diisi!!" });
			//return;
		}
		if ($scope.berita.newsContent === '' || $scope.berita.newsContent === undefined) {
			//$.growl.error({ title: "[PERINGATAN]", message: "Isi berita belum diisi!!" });
			//return;
		}
		insert(GlobalConstantService.getConstant("api_endpoint") + "/news/edit");
		/*
         if ($scope.file === undefined) {
         insert("");
         }
         else {
         var fileInput = $('.upload-file');
         var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
         var maxSize = fileInput.data('max-size');
         if (fileInput.get(0).files.length) {
         var fileSize = fileInput.get(0).files[0].size;
         if (fileSize > maxSize) {
         $rootScope.unloadLoadingModal();
         $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
         return;
         } else {
         var restrictedExt = $rootScope.limitfiletype;
         if ($.inArray(extFile, restrictedExt) === -1) {
         $rootScope.unloadLoadingModal();
         $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
         return;
         } else {
         upload();
         }
         }
         }
         }
         */
	};

	function upload() {
		uploaderSvc.uploadFile($scope.file, "berita").then(function (url) {
			if (!(url === 'failed')) {
				insert(url);
			} else {
				$.growl.error({ title: "[WARNING]", message: "Upload file penunjukkan gagal" });
			}
		});
	}


	function insert(url) {
		var param = [];
		NewsService.update({
			NewsID: item.newsID,
			NewsContent: $scope.berita.newsContent,
			NewsTitle: $scope.berita.newsTitle
		}, function (reply) {
			//console.info("reply: "+JSON.stringify(reply));
			if (reply.status === 200) {
				var data = reply.data;
				vm.berita = data;
				//select jumlah data
				//NewsService.count({
				//	//keyword: '%' + vm.kata.srcText
				//},
				//function (reply) {
				//	if (reply.status === 200) {
				//		var data = reply.result.data;
				vm.totalItems = data.length;
				//	} else {
				//		$.growl.error({ message: "Gagal mendapatkan data berita!!" });
				//		$rootScope.unloadLoading();
				//	}
				//	$rootScope.unloadLoading();
				//}, function (err) {
				//	$.growl.error({ message: "Gagal Akses API >" + err });
				//	$rootScope.unloadLoading();
				//});
			} else {
				//$.growl.error({ message: "Gagal mendapatkan data berita!!" });
				$rootScope.unloadLoading();
				return;
			}
			$rootScope.unloadLoading();
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			$rootScope.unloadLoading();
		});

		$uibModalInstance.close();
	}

	$scope.batal = function () {
		$uibModalInstance.dismiss('cancel');
	};
};

//TODO
var delBerita = function ($scope, item, GlobalConstantService, $uibModalInstance, $uibModal, NewsService) {
	$scope.delBerita = function () {
		var param = [];
		param.push(item);
		NewsService.remove({
			NewsID: item
		}, function (reply) {
			//console.info("reply: "+JSON.stringify(reply));
			if (reply.status === 200) {
				var data = reply.data;
				vm.berita = data;
				//select jumlah data
				//NewsService.count({
				//	//keyword: '%' + vm.kata.srcText
				//},
				//function (reply) {
				//	if (reply.status === 200) {
				//		var data = reply.result.data;
				vm.totalItems = data.length;
				//	} else {
				//		$.growl.error({ message: "Gagal mendapatkan data berita!!" });
				//		$rootScope.unloadLoading();
				//	}
				//	$rootScope.unloadLoading();
				//}, function (err) {
				//	$.growl.error({ message: "Gagal Akses API >" + err });
				//	$rootScope.unloadLoading();
				//});
			} else {
				//$.growl.error({ message: "Gagal mendapatkan data berita!!" });
				$rootScope.unloadLoading();
				return;
			}
			$rootScope.unloadLoading();
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			$rootScope.unloadLoading();
		});

		$uibModalInstance.close();
	};
	$scope.cancel = function () {
		$uibModalInstance.dismiss('cancel');
	};
};

function Kata(srcText) {
	var self = this;
	self.srcText = srcText;
}