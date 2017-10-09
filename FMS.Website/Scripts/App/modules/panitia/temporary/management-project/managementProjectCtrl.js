angular.module('eprocAppPanitia')
.controller( 'managementProjectCtrl', function( $scope, $rootScope, $modal, $state, $cookieStore, $http ){
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
            {frm:"FORM 5.2.9", nama_tahapan: "Equipment Standard", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.10", nama_tahapan: "Management Project", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.11", nama_tahapan: "Work Schedule", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.12", nama_tahapan: "Detail Planning", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.13", nama_tahapan: "Project Control Plan", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.14", nama_tahapan: "Work Methodology", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.15", nama_tahapan: "Training & Orientation Plan", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.16", nama_tahapan: "JSA for High Risk works & activities", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.17", nama_tahapan: "EHS Plan", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.18", nama_tahapan: "Emergency Response Plan", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.19", nama_tahapan: "Work Breakdown Analysis", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.20", nama_tahapan: "Financial Reports", status: 1,  jenis_form_url:""},
            {frm:"FORM 5.2.21", nama_tahapan: "Additional Information", status: 1,  jenis_form_url:""},
        ];
    };
    
    $scope.prosesApprovalPR = function(dt) {
        var modalInstance = $modal.open({
            templateUrl: 'formApproval.html',
            controller: formApprovalCtrl,
            resolve: {
                item: function() {
                    return dt;
                }
            }
        });
        modalInstance.result.then(function() {
            $scope.init;
        });
    };
    
    
}
);
var formApprovalCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
     
    $scope.batal = function() {
        $modalInstance.dismiss('cancel');
    };    
};
;