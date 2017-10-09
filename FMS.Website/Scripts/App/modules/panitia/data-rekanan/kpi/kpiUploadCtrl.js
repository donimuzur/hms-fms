angular.module('eprocAppPanitia')
.controller( 'kpiListCtrl', function( $scope, $rootScope, $modal, $state, $cookieStore, $http ){
    $scope.formupload = function(id,act){
        $state.transitionTo('kpi-upload',{id_cpr:id, id_act:act});
    };
})
.controller( 'kpiUploadCtrl', function( $scope, $rootScope, $modal, $state, $cookieStore, $http ){
    $scope.warningletter = [];
    $scope.init = function(){
        $scope.warningletter = [
            {nama_vendor: "vendor-1", jenis:"admonition-1", tgl:"3-06-2016"},
            {nama_vendor: "vendor-2", jenis:"admonition-2", tgl:"4-06-2016"},
            {nama_vendor: "vendor-3", jenis:"notification", tgl:"5-06-2016"},
            {nama_vendor: "vendor-4", jenis:"reminder", tgl:"6-06-2016"},
            {nama_vendor: "vendor-5", jenis:"termination", tgl:"7-06-2016"}
        ];
    };
    
    $scope.inputWarning = function(data,id) {
        var kirim_data ={ id:id, data: data};
        var modalInstance = $modal.open({
            templateUrl: 'formWarningLetter.html',
            controller: formWarningLetter,
            resolve: {
                item: function() {
                    return kirim_data;
                }
            }
        });
        modalInstance.result.then(function() {
            $scope.init();
        });
    };
    
    $scope.formGenerate = function(data,id) {
        var kirim_data ={ id:id, data: data};
        var modalInstance = $modal.open({
            templateUrl: 'formGenerate.html',
            controller: formGenerate,
            resolve: {
                item: function() {
                    return kirim_data;
                }
            }
        });
        modalInstance.result.then(function() {
            $scope.init();
        });
    };
});
var formWarningLetter = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    $scope.id_data = item.id;
    $scope.data = item.data;
    $scope.action = "";
    $scope.rincian = "";
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
        if($scope.id_data === 0){ $scope.action = "Tambah "; } else { $scope.action = "Ubah "; }
    };
        
    $scope.batal = function() {
        $modalInstance.dismiss('cancel');
    };    
};

var formGenerate = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    $scope.batal = function() {
        $modalInstance.dismiss('cancel');
    };
};