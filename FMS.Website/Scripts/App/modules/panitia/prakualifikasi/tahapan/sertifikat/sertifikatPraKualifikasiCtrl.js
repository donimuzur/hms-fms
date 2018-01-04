angular.module('eprocAppPanitia')
        .controller('sertifikatPraKualifikasiCtrl', function($state, $scope, $http, $rootScope, $cookieStore, $stateParams) {
            var flowpaket_id = Number($stateParams.flowpaket_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.nama_paket;
            $scope.nama_tahapan;
            $scope.tgl_mulai;
            $scope.tgl_selesai;
            $scope.dokumen = [];
            $scope.menuhome = 0;
            $scope.labelcurr;
            //console.info($state.current.name);

            $scope.init = function() {
                $scope.menuhome=$rootScope.menuhome;
                loadAwal();
            };

            function loadAwal() {
                $http.post($rootScope.url_api+'paket/detail/info', {
                    sessionID: $cookieStore.get('sessId'),
                    paket_lelang_id: $scope.paket_lelang_id,
                    flowpaket_id: flowpaket_id
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.nama_paket = reply.result.data.result[0].nama_paket;
                        $scope.nama_tahapan = reply.result.data.result[0].nama_tahapan;
                        $scope.tgl_mulai = convertTanggal(reply.result.data.result[0].tgl_mulai);
                        $scope.tgl_selesai = convertTanggal(reply.result.data.result[0].tgl_selesai);
                        $scope.labelcurr = reply.result.data.result[0].label;
                        $http.post($rootScope.url_api+'pemasukandokkualifikasi/select', {
                            sessionID: $cookieStore.get('sessId'),
                            paket_lelang_id: $scope.paket_lelang_id,
                            flow_paket_id: flowpaket_id
                        }).success(function(reply2){
                            if(reply2.status === 200){
                                $scope.dokumen = reply2.result.data;

                            }
                        });
                        
                    }
                });
            }
            
            $scope.lihatDokumen = function(dok){
                $state.transitionTo('lihat-profil-rekanan', {rekanan_id: dok.rekanan_id});
            };
            
            $scope.lihatProfil = function(rekanan_id) {
                //$state.transitionTo('lihat-profil-rekanan', {rekanan_id: rekanan_id});
                $state.transitionTo('detail-dokumen-prakualifikasi', {
                    rekanan_id: rekanan_id,
                    paket_lelang_id: $scope.paket_lelang_id
                });
            };
            
            $scope.printdata= function () {
                $('#printdata').jqprint();
            };
            
            function convertTanggal(input) {
                var tahun = input.substring(0, 4);
                var bulan = input.substring(5, 7);
                var z;
                if (bulan === '01')
                    z = "01";
                else if (bulan === '02')
                    z = "02";
                else if (bulan === '03')
                    z = "03";
                else if (bulan === '04')
                    z = "04";
                else if (bulan === '05')
                    z = "05";
                else if (bulan === '06')
                    z = "06";
                else if (bulan === '07')
                    z = "07";
                else if (bulan === '08')
                    z = "08";
                else if (bulan === '09')
                    z = "09";
                else if (bulan === '10')
                    z = "10";
                else if (bulan === '11')
                    z = "11";
                else if (bulan === '12')
                    z = "12";
                var tanggal = input.substring(8, 10);
                var waktu = input.substring(11, 16);
                return tanggal + "/" + z + "/" + tahun + " " + waktu;
            };
        });