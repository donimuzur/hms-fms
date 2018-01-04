angular.module('eprocAppPanitia').controller('scopeOfWorkCtrl', function ($scope, $rootScope, $modal, $state, $cookieStore, $http) {
	$scope.init = function () {
		$scope.flow = [
            { urutan: 0, nama_tahapan: "Kuantitas", status: 1, flowpaket_id: 809, jenis_form_url: "" },
            { urutan: 1, nama_tahapan: "Kualitas", status: 1, flowpaket_id: 810, jenis_form_url: "pendaftaran-prakualifikasi" },
            { urutan: 2, nama_tahapan: "Waktu", status: 1, flowpaket_id: 858, jenis_form_url: "kualifikasi-surat-pernyataan" },
            { urutan: 3, nama_tahapan: "Biaya", status: 1, flowpaket_id: 813, jenis_form_url: "" },
            { urutan: 4, nama_tahapan: "Tenaga Kerja", status: 1, flowpaket_id: 813, jenis_form_url: "" },
            { urutan: 5, nama_tahapan: "Peralatan", status: 1, flowpaket_id: 813, jenis_form_url: "pengumuman-hasil-prakualvendor" },
            { urutan: 6, nama_tahapan: "Kemampuan Supervisi", status: 1, flowpaket_id: 813, jenis_form_url: "certificate-prakualifikasi" },
			{ urutan: 7, nama_tahapan: "Lainnya (tambahan jika diperlukan)", status: 1, flowpaket_id: 813, jenis_form_url: "" },
		];

		$scope.flow2 = [
            { urutan: 0, nama_tahapan: "Cidera pekerja dan insiden", status: 1, flowpaket_id: 809, jenis_form_url: "" },
            { urutan: 1, nama_tahapan: "Golden Rules dan Pelanggaran Safety", status: 1, flowpaket_id: 810, jenis_form_url: "pendaftaran-prakualifikasi" },
            { urutan: 2, nama_tahapan: "Health and Safety Behavioral - Violation of PTVI EHS Policies, Standards and Procedures", status: 1, flowpaket_id: 858, jenis_form_url: "kualifikasi-surat-pernyataan" },
            { urutan: 3, nama_tahapan: "EHS Meetings, Inspections, Behavioral Dialog &amp; JCC", status: 1, flowpaket_id: 813, jenis_form_url: "" },
            { urutan: 4, nama_tahapan: "Management System - Compliance", status: 1, flowpaket_id: 813, jenis_form_url: "" }
		];
	};
	$scope.detailtahapan = function (flowpaket_id, stateName) {
		$state.transitionTo(stateName, { flowpaket_id: flowpaket_id, paket_lelang_id: 47 });
	};

	$scope.prosesApprovalPR = function (dt) {
		var modalInstance = $modal.open({
			templateUrl: 'formApproval.html',
			controller: formApprovalCtrl,
			resolve: {
				item: function () {
					return dt;
				}
			}
		});
		modalInstance.result.then(function () {
			$scope.init;
		});
	};


}
);
var scopeOfWorkCtrl = function ($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {

	$scope.batal = function () {
		$modalInstance.dismiss('cancel');
	};
};
