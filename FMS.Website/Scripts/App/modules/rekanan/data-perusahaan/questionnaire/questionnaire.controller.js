(function () {
    'use strict';

    angular.module("app")
    .controller("QuestionnaireController", ctrl);
    
    ctrl.$inject = ['$http', '$uibModal','$translate', '$translatePartialLoader', '$location', 'SocketService', 'QuestionnaireService'];
    /* @ngInject */
    function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, QuestionnaireService) {
        var vm = this;
    }
})();

// angular.module('eprocApp')
// .controller('questionnaireCtrl', function ($scope, $http, $cookieStore, $state, $rootScope) {
//      $scope.pertanyaan = [
//         { no : 1,
//           pertanyaan_ind: "Apakah Anda seorang Pejabat Pemerintah ataukah perusahaan anda merupakan  wewenang pemerintah?",
//           pertanyaan_eng: "Are you a Government Official or is your company a Government Authority?"
//         },
//         { no : 2,
//           pertanyaan_ind: "Dalam menyediakan barang, layanan jasa atau memasok peralatan ke Vale, apakah perusahaan Anda (atau personil atau subkontraktor perusahaan anda) memiliki hubungan kerjasama dengan Pejabat Pemerintah atau pihak berwenang pada kondisi yang tidak tercakup pada poin 2 di atas?",
//           pertanyaan_eng: "In providing services or supplying equipment or materials to Vale, will your company (or any of your personnel or subcontractors) have contact with Government Officials or a Government Authority in circumstances not covered by item 2 above?"
//         },
//         { no : 3,
//           pertanyaan_ind: "Apakah Anda menerima bayaran atau komisi untuk mencapai tujuan komersial atau aktivitas yang sah atas nama Vale?",
//           pertanyaan_eng: "Will you receive a success fee or commission for achieving commercial or legal objectives on Vale’s behalf?"
//         }
//      ];
//      $scope.init = function(){
//          $rootScope.getSession().then(function (result) {
//             $rootScope.userSession = result.data.data;
//             $rootScope.userLogin = $rootScope.userSession.session_data.username;
//             $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
//             $rootScope.nama_perusahaan = $rootScope.userSession.session_data.nama_perusahaan;
//         });
//      };
// })
// ;