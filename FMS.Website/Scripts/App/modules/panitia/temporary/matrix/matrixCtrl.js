angular.module('eprocAppPanitia')
.controller( 'matrixResponsibilityCtrl', function( $scope, $rootScope, $modal, $state, $cookieStore, $http ){
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
    
    $scope.site = [
        {item:"1.1",desc:"Offices",comment:""},
        {item:"1.2",desc:"Workshops",comment:"If required"},
        {item:"1.3",desc:"Computers",comment:""},
        {item:"1.4",desc:"Phone",comment:"Limited supply. Can only be installed at close location  to PTVI building, "},
        {item:"1.5",desc:"Internet",comment:"Restricted use and at location close to PTVI building"},
        {item:"1.6",desc:"Consumables",comment:""}
    ];
    
    $scope.serv = [
        {item:"2.1",desc:"Power - High Voltage",comment:"If required and the location is closed to a PTVI power source "},
        {item:"2.2",desc:"Power - Low Voltage",comment:"If required and the location is closed to a PTVI power source "},
        {item:"2.3",desc:"Water – Potable",comment:""},
        {item:"2.4",desc:"Phone",comment:"Water - Service water"},
        {item:"2.5",desc:"Internet",comment:"Compressed Air"}
    ];

    $scope.init = function(){
        
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