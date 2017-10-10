angular.module('eprocAppPanitia')
	.controller('vhsCtrl', function ($scope, $http, $rootScope, $modal, $state, $cookieStore) {
		$scope.init = function () {
			$scope.kpiList = [{ kpi_name: "On Time Services", kpi_bobot: 30 }, { kpi_name: "Safety", kpi_bobot: 10 },
				{
					kpi_name: "Quality", kpi_bobot: 20, kpi_sub: [{
						sub_name: "Kuantitas", sub_bobot: 35,
						detail_nilai: [{ skor: 1, det_skor: "Below Plan" }, { skor: 2, det_skor: "On Plan" }, { skor: 3, det_skor: "Above Plan" }]
					}, {
						sub_name: "Mutu", sub_bobot: 25,
						detail_nilai: [{ skor: 1, det_skor: "Not Suitable" }, { skor: 2, det_skor: "Suitable" }]
					}, { sub_name: 'Keaslian', sub_bobot: 30 }, { sub_name: 'Packaging', sub_bobot: 10 }]
				}, {
					kpi_name: 'After Sales Service', kpi_sub: [{
						sub_name: 'Berkunjung ke User', sub_bobot: 6
					}, {
						sub_name: 'Training / Workshop', sub_bobot: 4
					}]
				}, {
					kpi_name: 'Response Time', kpi_bobot: 10
				}, {
					kpi_name: 'Reporting and admin', kpi_bobot: 10, kpi_sub: [{
						sub_name: 'Laporan Outstanding Reservasi 2x sebulan (minggu 1 dan 3)', sub_bobot: 35
					}, {}, {}]
				}, {
					kpi_name: 'Compliance', kpi_bobot: 10
				}
			];
		};

		$scope.tambah_kpi = function (id, data) {
			var kirim = { id_data: id, data: data };
			var modalInstance = $modal.open({
				templateUrl: 'formTambahKPI.html',
				controller: formKPICtrl,
				resolve: {
					item: function () {
						return kirim;
					}
				}
			});
			modalInstance.result.then(function () {
				$scope.init();
			});
		};

		$scope.ubahkriteria = function (data) {
			var modalInstance = $modal.open({
				templateUrl: 'formUbahkriteria.html',
				controller: formKriteriaCtrl,
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				$scope.init();
			});
		};

		$scope.formnilai = function (data) {
			var modalInstance = $modal.open({
				templateUrl: 'formNilaiSub.html',
				controller: formNilaiCtrl,
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				$scope.init();
			});
		};
	});

var formNilaiCtrl = function ($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
};

var formKriteriaCtrl = function ($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
};

var formKPICtrl = function ($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
	console.info(JSON.stringify(item));

	$scope.batal = function () {
		$modalInstance.dismiss('cancel');
	};
};
;