angular.module('eprocAppPanitia')
.controller( 'tenderLetterCtrl', function( $scope, $rootScope, $modal, $state, $cookieStore, $http ){
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
        $scope.agreementTender = [
            {id:"d01", nama_rek:"PT. Eproc001", agree:1},
            {id:"d02", nama_rek:"PT. Eproc002", agree:0},
            {id:"d03", nama_rek:"PT. Eproc003", agree:1}
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