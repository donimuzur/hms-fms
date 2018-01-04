angular.module('eprocAppPanitia')
.controller('scopeOfWorkAssesmentCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.sowa_list = [
        {id:"1", parent:"Impact To Quality",
         subparent:[
             {id_sub:"1-1", sub_name:"Organization & Focus to the Customer Requirements", childs:[
                {id_childs:"1-11", child_name:"What is the type of project organization structure?", detail:[
                   {id_det:"d01", det_name:"Foreman and/or Workers (Skill, Semi-Skill and Non-Skill)"},
                   {id_det:"d01", det_name:"Supervisor, Foreman and Workers"},
                   {id_det:"d01", det_name:"Site manager, supervisors/special functions, Foreman and Workers"}
                ]},
                {id_childs:"1-12", child_name:"How many people that involved in the project?"}
             ]},
             {id_sub:"1-2", sub_name:"Provision of Resources"}
         ]
        },
        {id:"2", parent:"Item to Enviroment, Health and Safety"}
    ];
    
    $scope.back = function(dt) {
        $state.transitionTo('data-contract-requisition');
    };
    
});