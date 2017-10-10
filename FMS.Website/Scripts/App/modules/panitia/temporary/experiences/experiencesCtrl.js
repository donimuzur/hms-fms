angular.module('eprocAppPanitia')
.controller( 'experiencesCtrl', function( $scope, $rootScope, $modal, $state, $cookieStore, $http ){
    $scope.content_wl = "";
    
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
    
    $scope.init = function(){
        $scope.flow = [
            {frm:"FORM 5.2.1", nama_tahapan: "Tender Letter ", status: 1, jenis_form_url:""},
            {frm:"FORM 5.2.2", nama_tahapan: "Exception to Tender Documents", status: 1,jenis_form_url:"pendaftaran-prakualifikasi"},
            {frm:"FORM 5.2.3", nama_tahapan: "Matrix Responsibility", status: 1, jenis_form_url:"kualifikasi-surat-pernyataan"},
            {frm:"FORM 5.2.4", nama_tahapan: "Checklist Item", status: 1, jenis_form_url:""},
            {frm:"FORM 5.2.5", nama_tahapan: "Experiences and Track Records", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.6", nama_tahapan: "Key Personnel and Organization Chart ", status: 1, jenis_form_url:"pengumuman-hasil-prakualvendor"},
            {frm:"FORM 5.2.7", nama_tahapan: "Manpower Plan", status: 1,  jenis_form_url:"certificate-prakualifikasi"},
            {frm:"FORM 5.2.8", nama_tahapan: "Subcontractor Plan", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.9", nama_tahapan: "Equipment Standard", status: 1,  jenis_form_url:""}
        ];
    };
    
    $scope.openmodal = function (act, data) {
        $scope.action = act === "add" ? 0 : 1;
        $scope.resolvedata = act === "add" ? {} : data;
        var opnmdl = $modal.open({
            templateUrl: "mdlAUpengalaman.html",
            controller: "mdlAUpengalaman",
            resolve: {
                action: function () {
                    return $scope.action;
                },
                datane: function () {
                    return $scope.resolvedata;
                }
            }
        });
        opnmdl.result.then(function () {
            $scope.initialize();
        });
    };
})
.controller('mdlAUpengalaman', function ($scope, $http, $modalInstance, action, datane, $cookieStore, $state, $stateParams, $rootScope) {
    
})
;
var formApprovalCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
     
    $scope.batal = function() {
        $modalInstance.dismiss('cancel');
    };    
};
;