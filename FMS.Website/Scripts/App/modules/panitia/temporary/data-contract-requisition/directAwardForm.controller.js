angular.module('eprocAppPanitia')
.controller('directAwardFormCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.directaward_list = [
        {id:"1", ques:"The required items/services are proprietary to the Supplier/Contractor", sub:[]},
        {id:"2", ques:"A specific item is needed", sub:[
          {s_ques:"to be compatible or interchangeable with existing hardware"},
          {s_ques:"as spare or replacement hardware"}
        ]},
        {id:"3", ques:"It is not possible to obtain competition (i.e., only one source is capable of supplying the items or meeting the requirements).  In a brief explanation, provide supporting evidence for the conclusion; other sources considered should be identified and why they are not able to meet the requirements."}
    ];
    console.info(JSON.stringify($scope.directaward_list));
    $scope.formCSMSDec = function(dt) {
         $state.transitionTo('csms-decision');
    };
    
    $scope.detailContract = function(dt) {
        $state.transitionTo('detail-contract-requisition');
    };
    
});