(function () {
    'use strict';

    angular.module('app').filter('dateJson', function () {
        return function (input) {
            var milli = input.substr(6, 18).replace(')', '').replace('/', '');
            return new Date(parseInt(milli));
        }
    });

    angular.module("app").controller("GSDashboardCtrl", ctrl);
	
    ctrl.$inject = ['$http', '$translate', '$scope', '$window', 'UIControlService', '$uibModal', '$timeout', '$interval', 'GlobalConstantService', 'DTOptionsBuilder', '$state', '$stateParams', 'CFMService', '$filter'];

    function ctrl($http, $translate, $scope, $window, UIControlService, $uibModal, $timeout, $interval, GlobalConstantService, DTOptionsBuilder, $state, $stateParams, CFMService, $filter) {
	    var vm = $scope;

	    vm.coordinator = localStorage.getItem('username');
	    vm.roles = localStorage.getItem('roles');

	    $scope.currentPage = 1;
	    vm.offset = (vm.currentPage * 10) - 10;
	    vm.totalRecords = 0;

	    vm.init = init;
	    function init() {
	        if (vm.roles == 'SYSTEM.ROLE_HR' || vm.roles == 'SYSTEM.ROLE_FLEET') {
	            vm.jLoad(1);
	        } else {
	            $state.go('home');
	        }
	    }

	    $scope.employee_name = ''; $scope.vehicle_usage = ''; $scope.police_number = ''; $scope.group_level = ''; $scope.location = ''; $scope.request_date = ''; $scope.fulfillment_date = ''; $scope.unit = ''; $scope.police_number = ''; $scope.start_date = ''; $scope.end_date = ''; $scope.remark = ''; $scope.fulfillment_time = ''; $scope.rent_time = '';

	    vm.jLoad = jLoad;
	    function jLoad(current) {

	        var offset = (current * 10) - 10;
	        var limit = 15;

	        var param = $scope.employee_name + "|" + $scope.vehicle_usage + "|" + $scope.police_number + "|" + $scope.group_level + "|" + $scope.location + "|" + $scope.request_date + "|" + $scope.fulfillment_date + "|" + $scope.unit + "|" + $scope.police_number + "|" + $scope.start_date + "|" + $scope.end_date + "|" + $scope.remark + "|" + $scope.fulfillment_time + "|" + $scope.rent_time + "|" + offset + "|" + limit;
	        //console.info(param);

	        CFMService.GetReportGSByParam(param, function succ(reply) {
	            if (reply.status == 200) {
	                console.info(reply.data.List);

	                //reply.data.List.GS_Request_Date = new Date(reply.data.List.GS_Request_Date);
	                //reply.data.List.GS_Fulfillment_Date = new Date(reply.data.List.GS_Fulfillment_Date);
	                //reply.data.List.Start_Date = new Date(reply.data.List.Start_Date);
	                //reply.data.List.End_Date = new Date(reply.data.List.End_Date);

	                $scope.ReportGS = reply.data.List;
	                $scope.totalItems = reply.data.Count;
	                $scope.itemsPerPage = limit;
	            } else {
	                $.growl.error({ message: "Gagal mengambil data Fuel Report" })
	            }
	        });
	    }

	    $scope.$watch("currentPage", function () {
	        setPagingData($scope.currentPage);
	        vm.jLoad($scope.currentPage);
	    });

	    function setPagingData(page) {
	        if (!page) {
	            var pagedData = $scope.ReportGS.slice((page - 1) * $scope.itemsPerPage, page * $scope.itemsPerPage);
	            $scope.aCandidates = pagedData;
	        }
	    }
		
		vm.toExcel = toExcel;
        function toExcel(tabel) {
            //console.info(tabel);
            //$window.alert("Export Excel");
			
            alasql('SELECT Employee_Name AS `Employee Name`, Vehicle_Usage AS `Vehicle Usage`, Police_Number AS `Police Number`, Group_Level AS `Group Level`, Location AS `Location`, GS_Request_Date AS `GS Request Date`, GS_Fulfillment_Date AS `GS Fulfillment Date`,GS_Unit_Type AS `GS Unit (Type)`, GS_Police_Number AS `GS Police Number`, Start_Date AS `Start Date`, End_Date AS `End Date`, Remark AS `Remark`, GS_FullfilmentTime AS `GS Fulfillment Time`, RentTime AS `Rent Time` INTO XLSX("ReportGS.xlsx",{headers:true}) FROM ?', [$scope.ReportGS]);
        }

        vm.dateJson = dateJson;
        function dateJson(input) {
            var milli = input.substr(6,18).replace(')','').replace('/','');
            console.info(milli);
            return new Date(parseInt(milli));
        }
		
	}

})();