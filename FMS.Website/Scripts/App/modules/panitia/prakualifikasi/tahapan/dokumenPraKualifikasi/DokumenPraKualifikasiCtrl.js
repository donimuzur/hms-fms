angular.module('eprocAppPanitia')
.controller('DokumenKualifikasiLiveCtrl', function($scope, $http, $rootScope, $modal, $cookieStore, $stateParams, $state) {
    $scope.rekanan_id = Number($stateParams.rekanan_id);
    //$scope.flow_paket_id = Number($stateParams.flow_paket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    
    $scope.init = function(){
        console.info("iniiit");
    };
})

.controller('DokumenPraKualifikasiCtrl', function($scope, $http, $rootScope, $modal, $cookieStore, $stateParams, $state) {
    $scope.rekanan_id = Number($stateParams.rekanan_id);
    //$scope.flow_paket_id = Number($stateParams.flow_paket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    $scope.nama_paket = '';
    $scope.actionView = "rekanan";
    $scope.detailRekanans = [];
    $scope.detailIjinusaha = [];
    $scope.detailAkta = [];
    $scope.detailPajak = [];
    $scope.detailPengalaman = [];
    $scope.detailPengurus = [];
    $scope.detailPerlengkapan = [];
    $scope.detailDokumen = [];
    $scope.dokumensp = [];
    $scope.detailSaham = [];
    $scope.detailTA = [];
    $scope.detailTA_cv_pengalaman = [];
    $scope.detailTA_cv_pendidikan = [];
    $scope.detailTA_cv_sertifikat = [];
    $scope.detailTA_cv_bahasa = [];
    $scope.detailRincianAlat = [];
    $scope.page_id = 129;
    $scope.hide = false;
    $scope.id = 0;
    $scope.id_cv = 0;
    $scope.field_cv = "";
    $scope.menuhome = 0;
    $scope.files1 = [];
    $scope.files2 = [];
    $scope.files3 = [];
    $scope.opsiValid = [{label: 'Tidak Valid', value: false}, {label: 'Valid', value: true}];
    $scope.cek_adm; $scope.label_adm ="Administrasi";
    $scope.cek_ijinusaha; $scope.label_ijinusaha ="Ijin Usaha";
    $scope.cek_akta; $scope.label_akta ="Akta Pendirian";
    $scope.cek_pengurus; $scope.label_pengurus ="Pengurus Perusahaan";
    $scope.cek_pemilik_saham; $scope.label_pemilik_saham ="Kepemilikan Saham";
    $scope.cek_pajak; $scope.label_pajak ="Data Pajak";
    $scope.cek_ta; $scope.label_ta ="Data Tenaga Ahli";
    $scope.cek_perlengkapan; $scope.label_perlengkapan ="Data Perlengkapan";
    $scope.cek_pengalaman; $scope.label_pengalaman ="Data Pengalaman";
    $scope.cek_dokulain; $scope.label_dokulain ="Data Dokumen Lain-lain";

    $scope.init = function() {
        $scope.menuhome = $rootScope.menuhome;
        $rootScope.isLogged = true;
        loadProfilRekanan();
        loadAktaRekanan();
        loadIjinUsaha();
        loadPajak();
        loadPengalaman();
        loadPengurusPerusahaan();
        loadPerlengkapan();
        loadSaham();
        loadUploadDokumen();
        loadSuratPernyataan();
        loadTenagaAhli();
    };
    
    $scope.cekKelengkapan = function(type){
        //console.info("cekcekcek: "+JSON.stringify($scope.cek_adm));
        var valid; 
        if(type === "administrasi" && ($scope.cek_adm === "" || $scope.cek_adm === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "ijin_usaha" && ($scope.cek_ijinusaha === "" || $scope.cek_ijinusaha === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "akta" && ($scope.cek_akta === "" || $scope.cek_akta === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "pengurus" && ($scope.cek_pengurus === "" || $scope.cek_pengurus === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "pemilik_saham" && ($scope.cek_pemilik_saham === "" || $scope.cek_pemilik_saham === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }

        if(type === "pajak" && ($scope.cek_pajak === "" || $scope.cek_pajak === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "ta" && ($scope.cek_ta === "" || $scope.cek_ta === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "perlengkapan" && ($scope.cek_perlengkapan === "" || $scope.cek_perlengkapan === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "pengalaman" && ($scope.cek_pengalaman === "" || $scope.cek_pengalaman === undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "doku_lain" && ($scope.cek_dokulain === "" || $scope.cek_dokulain=== undefined)){
            $.growl.error({title: "[PERINGATAN]", message: "Pilih status kelengkapan terlebih dahulu"});
            return;
        }
        
        if(type === "administrasi"){ valid = $scope.cek_adm.value; }
        if(type === "ijin_usaha"){ valid = $scope.cek_ijinusaha.value; }
        if(type === "akta"){ valid = $scope.cek_akta.value; }
        if(type === "pengurus"){ valid = $scope.cek_pengurus.value; }
        if(type === "pemilik_saham"){ valid = $scope.cek_pemilik_saham.value; }
        if(type === "pajak"){ valid = $scope.cek_pajak.value; }
        if(type === "ta"){ valid = $scope.cek_ta.value; }
        if(type === "perlengkapan"){ valid = $scope.cek_perlengkapan.value; }
        if(type === "pengalaman"){ valid = $scope.cek_pengalaman.value; }
        if(type === "doku_lain"){ valid = $scope.cek_dokulain.value; }
        //console.info(valid);
        $rootScope.authorize(
            $http.post($rootScope.url_api + 'pembuktiankualifikasi/cekUpdateValid', {
                cek_valid : valid, type : type, paket_lelang_id: $scope.paket_lelang_id,
                rekanan_id: $scope.rekanan_id, username: $rootScope.userLogin
            }).success(function(reply) {
                console.info(valid+" rep: "+JSON.stringify(reply));
                $rootScope.unloadLoadingModal();
                if (reply.status === 200) {
                    $.growl.notice({title: "[INFO]", message: "Berhasil menyimpan cek kelengkapan dokumen "+type});
                    return;
                }
                else {
                    $.growl.error({title: "[PERINGATAN]", message: "Gagal menyimpan cek kelengkapan dokumen "+type});
                    return;
                }
            }).error(function(err) {
                $rootScope.unloadLoadingModal();
                $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "dokumenKualifikasiCtrl.js - paket/byid"
                  }) 
                  .then(function(response){
                      // do nothing // don't have to feedback
                  });
                return;
            })
        );
    };

    $scope.lihatsub1 = function(flowpaket_id) {
        $state.transitionTo('subkriteria-kualifikasi-lvl-1', {
            flowpaket_id: flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };

    /*fungsi detail perlengkapan */
    $scope.detailAlat = function(idAlat) {
        var modalInstance = $modal.open({
            templateUrl: 'detailAlatModal.html',
            controller: detailAlatModal,
            resolve: {
                item: function() {
                    return idAlat;
                }
            }
        });
        modalInstance.result.then(function() {
            loadPerlengkapan();
        });
    };

    function loadTenagaAhli() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/ta', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id})
                .success( function(reply) {
                    //console.info("ta:"+JSON.stringify(reply));
            if (reply.status === 200) {
                $scope.detailTA = reply.result.data;
                if($scope.detailTA.length > 0){
                    if(!($scope.detailTA[0].is_valid === null)){
                        var valid = $rootScope.strtobool($scope.detailTA[0].is_valid);
                        for(var i = 0; i < $scope.opsiValid.length; i++ ){
                            if(valid === $scope.opsiValid[i].value){
                                $scope.cek_ta = $scope.opsiValid[i]; break;
                            }
                        }
                        if($scope.cek_ta.value === false){ $scope.label_ta = "Tenaga Ahli**"; }
                    }
                } else{
                     $scope.label_ta = "Tenaga Ahli**";
                }
            }
            else {
                $.growl.error({title: "[PERINGATAN]", message: "gagal mendapatkan data dokumen kualifikasi Tenaga Ahli Rekanan"});
                return;
            }
        });
    }
    /*fungsi detail tenaga ahli */
    $scope.detailTenagaAhli = function(idTa) {
        var modalInstance = $modal.open({
            templateUrl: 'detailTenagaAhliModal.html',
            controller: detailTenagaAhliModal,
            resolve: {
                item: function() {
                    return idTa;
                }
            }
        });
        modalInstance.result.then(function() {
            loadTenagaAhli();
        });
    };

    function loadProfilRekanan() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/rekanan', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id})
                .success(function(reply) {
            if (reply.status === 200) {
                $scope.detailRekanans = reply.result.data;
                var valid = $rootScope.strtobool($scope.detailRekanans[0].is_valid);
                for(var i = 0; i < $scope.opsiValid.length; i++ ){
                    if(valid === $scope.opsiValid[i].value){
                        $scope.cek_adm = $scope.opsiValid[i]; break;
                    }
                }
                if(!($scope.detailRekanans[0].is_valid === null)){
                    if($scope.cek_adm.value === false){ $scope.label_adm = "Administrasi**"; }
                    $http.post($rootScope.url_api+'badanusaha/badanusahaname', {
                        badan_usaha_id : $scope.detailRekanans[0].jenis_perusahaan}).success(function(reply2) {
                        $scope.detailRekanans[0].jenis_perusahaan_ = reply2.result.data[0].badan_usaha_nama;
                    });
                }
            }
            else {
                $.growl.error({message:"gagal mendapatkan data dokumen kualifikasi"});
                return;
            }
        });
    }


    function loadIjinUsaha() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/ijinusaha', {sessionID: $cookieStore.get('sessId'), 
            param: arr, page_id: $scope.page_id})
                .success(function(reply) {
            //console.info("ijin:"+JSON.stringify(reply));
            if (reply.status === 200) {
                $scope.detailIjinusaha = reply.result.data;
                $scope.detailIjinusaha.forEach(function(iu){
                    if(iu.masa_berlaku){
                        iu.masa_berlaku_converted = $rootScope.convertTanggal(iu.masa_berlaku);
                    }
                });
                if(!($scope.detailIjinusaha[0].is_valid === null)){
                    var valid = $rootScope.strtobool($scope.detailIjinusaha[0].is_valid);
                    for(var i = 0; i < $scope.opsiValid.length; i++ ){
                        if(valid === $scope.opsiValid[i].value){
                            $scope.cek_ijinusaha = $scope.opsiValid[i]; break;
                        }
                    }
                    if($scope.cek_ijinusaha.value === false){ $scope.label_ijinusaha = "Ijin Usaha**"; }
                }
            }
            else {
                return;
            }
        });
    }
    /*fungsi detail ijin Usaha */
    $scope.detailIjinUsaha = function(idijin) {
        var modalInstance = $modal.open({
            templateUrl: 'detailIjinUsahaModal.html',
            controller: detailIjinUsahaModalKualifikasi,
            resolve: {
                item: function() {
                    return idijin;
                }
            }
        });
        modalInstance.result.then(function() {
            loadIjinUsaha();
        });
    };

    function loadAktaRekanan() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/akta', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id})
                .success(function(data) {
            if (data.status === 200) {
                if (data.result.data.length > 0) {
                    for (var i = 0; i < data.result.data.length; i++) {
                        if (data.result.data[i].tanggal){
                            data.result.data[i].tanggal = $rootScope.convertTanggal(data.result.data[i].tanggal);
                            data.result.data[i].tanggal_converted = $rootScope.convertTanggalWaktu(data.result.data[i].tanggal);
                        }
                        if (data.result.data[i].dokumen_type === 'pendirian') {
                            $scope.files1.push(data.result.data[i]);
                        } else if (data.result.data[i].dokumen_type === 'perubahan') {
                            $scope.files2.push(data.result.data[i]);
                        } else if (data.result.data[i].dokumen_type === 'pengesahan') {
                            $scope.files3.push(data.result.data[i]);
                        }
                        //console.info("tes :" + JSON.stringify($scope.files1));
                    }
                    //console.info("akta: "+JSON.stringify(data.result));
                    if(!(data.result.data[0].is_valid === null)){
                        var valid = $rootScope.strtobool(data.result.data[0].is_valid);
                        for(var i = 0; i < $scope.opsiValid.length; i++ ){
                            if(valid === $scope.opsiValid[i].value){
                                $scope.cek_akta = $scope.opsiValid[i]; break;
                            }
                        }
                        if($scope.cek_akta.value === false){ $scope.label_akta = "Akta Pendirian**"; }
                    }
                    //console.info("tes :" + JSON.stringify(data.result.data.length));
                }

            } else {
               $.growl.error({message:"Gagal mendapatkan data akta"});
                return;
            }
        });
    }
    function loadPengurusPerusahaan() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/pengurus', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id})
                .success(function(reply) {
            if (reply.status === 200) {
                $scope.detailPengurus = reply.result.data;
                if($scope.detailPengurus.length > 0){
                    if(!($scope.detailPengurus[0].is_valid === null)){
                        var valid = $rootScope.strtobool($scope.detailPengurus[0].is_valid);
                        for(var i = 0; i < $scope.opsiValid.length; i++ ){
                            if(valid === $scope.opsiValid[i].value){
                                $scope.cek_pengurus = $scope.opsiValid[i]; break;
                            }
                        }
                        if($scope.cek_pengurus.value === false){ $scope.label_pengurus = "Pengurus Perusahaan**"; }
                    }
                }
            }
            else {
                $.growl.error({title: "[PERINGATAN]", message: "gagal mendapatkan Data Dokumen Kualifikasi Pengurus Perusahaan Rekanan!!"});
                return;
            }
        });
    }
    /*fungsi detail Pengurus */
    $scope.detailViewPengurus = function(idPengurus) {
        var modalInstance = $modal.open({
            templateUrl: 'detailPengurusModal.html',
            controller: detailPengurusModal,
            resolve: {
                item: function() {
                    return idPengurus;
                }
            }
        });
        modalInstance.result.then(function() {
            loadPengurusPerusahaan();
        });
    };

    function loadSaham() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/saham', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id}).success(function(reply) {
            if (reply.status === 200) {
                $scope.detailSaham = reply.result.data;
                if($scope.detailSaham.length > 0){
                    if(!($scope.detailSaham[0].is_valid === null)){
                        var valid = $rootScope.strtobool($scope.detailSaham[0].is_valid);
                        for(var i = 0; i < $scope.opsiValid.length; i++ ){
                            if(valid === $scope.opsiValid[i].value){
                                $scope.cek_pemilik_saham = $scope.opsiValid[i]; break;
                            }
                        }
                        if($scope.cek_pemilik_saham.value === false){ $scope.label_pemilik_saham = "Kepemilikan Saham**"; }
                    }
                }
            }
            else {
                $.growl.error({title: "[PERINGATAN]", message: "Gagal mendapatkan Data Dokumen Kualifikasi Saham Rekanan!!"});
                return;
            }
        });
    }
    function loadPajak() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/pajak', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id}).success(function(reply) {
            if (reply.status === 200) {
                $scope.detailPajak = reply.result.data;
                for (var i = 0; i < $scope.detailPajak.length; i++) {
                    if ($scope.detailPajak[i].masa_pajak !== "") {
                        $scope.detailPajak[i].masa_pajak = $rootScope.convertTanggal($scope.detailPajak[i].masa_pajak);
                        $scope.detailPajak[i].masa_pajak_converted = $scope.detailPajak[i].masa_pajak;
                    }
                    if ($scope.detailPajak[i].tanggalbukti_pajak !== "") {
                        $scope.detailPajak[i].tanggalbukti_pajak = $rootScope.convertTanggal($scope.detailPajak[i].tanggalbukti_pajak);
                        $scope.detailPajak[i].tanggalbukti_pajak_converted = $scope.detailPajak[i].tanggalbukti_pajak;
                    }
                }
                if($scope.detailPajak.length > 0){
                    if(!($scope.detailPajak[0].is_valid === null)){
                        var valid = $rootScope.strtobool($scope.detailPajak[0].is_valid);
                        for(var i = 0; i < $scope.opsiValid.length; i++ ){
                            if(valid === $scope.opsiValid[i].value){
                                $scope.cek_pajak = $scope.opsiValid[i]; break;
                            }
                        }
                        if($scope.cek_pajak.value === false){ $scope.label_pajak = " Data Pajak **"; }
                    }
                }
            }
            else {
                //alert("gagal mendapatkan Data Dokumen Kualifikasi Pajak Rekanan!!");
                return;
            }
        });
    }
    function loadPerlengkapan() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/perlengkapan', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id}).success(function(reply) {
            if (reply.status === 200) {
                $scope.detailPerlengkapan = reply.result.data;
                if($scope.detailPerlengkapan.length > 0){
                    if(!($scope.detailPerlengkapan[0].is_valid === null)){
                        var valid = $rootScope.strtobool($scope.detailPerlengkapan[0].is_valid);
                        for(var i = 0; i < $scope.opsiValid.length; i++ ){
                            if(valid === $scope.opsiValid[i].value){
                                $scope.cek_perlengkapan = $scope.opsiValid[i]; break;
                            }
                        }
                        if($scope.cek_perlengkapan.value === false){ $scope.label_perlengkapan = "Data Perlengkapan**"; }
                    }
                }else { $scope.label_perlengkapan = "Data Perlengkapan**"; }
                
            }
            else {
                //alert("gagal mendapatkan Data Dokumen Kualifikasi Perlengkapan Alat Rekanan!!");
                return;
            }
        });
    }
    function loadPengalaman() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/pengalaman', {param: arr, page_id: $scope.page_id}).success(function(reply) {
            if (reply.status === 200) {
                $scope.detailPengalaman = reply.result.data;
                if($scope.detailPengalaman.length > 0){
                    for (var i = 0; i < $scope.detailPengalaman.length; i++) {
                        if ($scope.detailPengalaman[i].tgl_pelaksanaan !== null || $scope.detailPengalaman[i].tgl_pelaksanaan !== '') {
                            $scope.detailPengalaman[i].tgl_pelaksanaan_converted = $rootScope.convertTanggal($scope.detailPengalaman[i].tgl_pelaksanaan);
                        }
                    }
                    if(!($scope.detailPengalaman[0].is_valid === null)){
                        var valid = $rootScope.strtobool($scope.detailPengalaman[0].is_valid);
                        for(var i = 0; i < $scope.opsiValid.length; i++ ){
                            if(valid === $scope.opsiValid[i].value){
                                $scope.cek_pengalaman = $scope.opsiValid[i]; break;
                            }
                        }
                        if($scope.cek_pengalaman.value === false){ $scope.label_pengalaman = "Data Pengalaman**"; }
                    }
                }
                else{
                    $scope.label_pengalaman = "Data Pengalaman**";
                }
            }
            else {
                //alert("gagal mendapatkan Data Dokumen Kualifikasi Pengalaman Rekanan!!");                      
                return;
            }
        });
    }
    /*fungsi detail Pengurus */
    $scope.detailViewPengalaman = function(idPengalaman) {
        var modalInstance = $modal.open({
            templateUrl: 'detailPengalamanDokKualifikasiModal.html',
            controller: detailPengalamanDokKualifikasiModalCtrl,
            resolve: {
                item: function() {
                    return idPengalaman;
                }
            }
        });
        modalInstance.result.then(function() {
            loadPengalaman();
        });
    };

    function loadUploadDokumen() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/uploaddokumen', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id}).success(function(reply) {
            if (reply.status === 200) {
                $scope.detailDokumen = reply.result.data;
                if($scope.detailDokumen.length > 0){
                    if(!($scope.detailDokumen[0].is_valid === null)){
                        var valid = $rootScope.strtobool($scope.detailDokumen[0].is_valid);
                        for(var i = 0; i < $scope.opsiValid.length; i++ ){
                            if(valid === $scope.opsiValid[i].value){
                                $scope.cek_dokulain = $scope.opsiValid[i]; break;
                            }
                        }
                        if($scope.cek_dokulain.value === false){ $scope.label_dokulain = "Data Dokumen Lain-lain**"; }
                    }
                }
                else{
                    $scope.label_dokulain = "Data Dokumen Lain-lain**";
                }
            }
            else {
                return;
            }
        });
    }

    function loadSuratPernyataan() {
        var arr = [];
        arr.push($scope.rekanan_id);
        arr.push($scope.paket_lelang_id);
        $http.post($rootScope.url_api+'dokkualifikasi/suratpernyataan', {
            sessionID: $cookieStore.get('sessId'),
            param: arr,
            page_id: $scope.page_id
        }).success(function(reply) {
//                    console.info("dokumen surat pernyataan = " + JSON.stringify(reply));
            if (reply.status === 200) {
//                        console.info("dokumen surat pernyataan = " + JSON.stringify(reply));
                $scope.dokumensp = reply.result.data[0];
                //console.info("isinya file 1 $scope.dokumensp.sp_kebenaranisidok --> "+$scope.dokumensp.sp_kebenaranisidok);
                //console.info("isinya file 2 $scope.dokumensp.sp_sanggupselesaikerja --> "+$scope.dokumensp.sp_sanggupselesaikerja);
                //console.info("isinya file 3 $scope.dokumensp.sp_sangguptinjaulapangan --> "+$scope.dokumensp.sp_sangguptinjaulapangan);
                //console.info("isinya file 4 $scope.dokumensp.sp_tidakdicekal --> "+$scope.dokumensp.sp_tidakdicekal);
                //console.info("isinya file 5 $scope.dokumensp.sp_tundukpadaaturan --> "+$scope.dokumensp.sp_tundukpadaaturan);

            }
            else {
                //alert("gagal mendapatkan Data Upload Dokumen Kualifikasi Rekanan!!");

                return;
            }
        });
    }
})

        .controller('subKriteriaLevel1Ctrl', function($scope, $http, $rootScope, $modal, $cookieStore, $stateParams, $state) {
            $scope.rekanan_id = Number($stateParams.rekanan_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.actionView = "rekanan";
            $scope.page_id = 129;
            $scope.hide = false;
            $scope.id = 0;

//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function() {
                $rootScope.isLogged = true;
                loadSubKriteriaEval1();
            };

            function loadSubKriteriaEval1() {

            }

            $scope.lihatsub2 = function(flowpaket_id) {
                $state.transitionTo('subkriteria-kualifikasi-lvl-2', {
                    flowpaket_id: flowpaket_id,
                    paket_lelang_id: $scope.paket_lelang_id
                });
            };

            $scope.back = function() {
                window.history.back();
            };

        })

        .controller('subKriteriaLevel2Ctrl', function($scope,  $rootScope, $modal, $cookieStore, $stateParams, $state) {
            $scope.rekanan_id = Number($stateParams.rekanan_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.actionView = "rekanan";
            $scope.page_id = 129;
            $scope.hide = false;
            $scope.id = 0;

//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function() {
                $rootScope.isLogged = true;
                loadSubKriteriaEval2();
            };

            function loadSubKriteriaEval2() {

            }

            $scope.back = function() {
                window.history.back();
            };

        });

/* detail rincian perlengkapan atau alat */
var detailAlatModal = function($scope, $modalInstance,  item, $cookieStore, $rootScope) {
    $scope.page_id = 129;
    $scope.selectedAlat = item;

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

};
/* detail rincian detailIjinUsahaModal */
var detailIjinUsahaModalKualifikasi = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    $scope.page_id = 129;
    $scope.selectedIjinUsaha = item;
    $scope.bidangUsahaKode;
    $scope.bidangUsahaNama;

    $scope.init = function() {
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $http.post($rootScope.url_api+'dokkualifikasi/bubyijinusaha', {
            sessionID: $cookieStore.get('sessId'),
            ijinusaha_id: $scope.selectedIjinUsaha.ijinusaha_id
        }).success(function(reply) {
            if (reply.status == 200){
                if (reply.result.data.length > 0){
                    $scope.bidangUsahaKode = reply.result.data[0].string_bidang_usaha;
                    $scope.bidangUsahaNama = reply.result.data[0].nama_bidang_usaha;
                    for (var i=1; i < reply.result.data.length; i++){
                        $scope.bidangUsahaKode += (', ' + reply.result.data[i].string_bidang_usaha);
                        $scope.bidangUsahaNama += (', ' + reply.result.data[i].nama_bidang_usaha);
                    }
                } 
            } else {
                $.growl.error({title: "[WARNING]", message: "Gagal mendapat daftar BU"});
            }
            $rootScope.unloadLoadingModal();
        });
    };
    
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

};
/* detail rincian detail Pengurus */
var detailPengurusModal = function($scope, $modalInstance, item, $cookieStore, $rootScope) {
    $scope.page_id = 129;
    $scope.selectedPengurus = item;

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

};
/* detail rincian detail Pengalaman */
var detailPengalamanDokKualifikasiModalCtrl = function($scope, $modalInstance,  item, $cookieStore, $rootScope) {
    $scope.page_id = 129;
    $scope.selectedPengalaman = item;

    if ($scope.selectedPengalaman.tgl_pelaksanaan !== '' && $scope.selectedPengalaman.tgl_pelaksanaan !== null && $scope.selectedPengalaman.tgl_pelaksanaan !== undefined) {
        $scope.selectedPengalaman.tgl_pelaksanaan_view = $rootScope.convertTanggal($scope.selectedPengalaman.tgl_pelaksanaan);
        //console.info("jadinya $scope.selectedPengalaman.tgl_pelaksanaan_view --> "+$scope.selectedPengalaman.tgl_pelaksanaan_view);
    }
    else {
        $scope.selectedPengalaman.tgl_pelaksanaan_view = "";
    }

    if ($scope.selectedPengalaman.selesai_kontrak !== '' && $scope.selectedPengalaman.selesai_kontrak !== null && $scope.selectedPengalaman.selesai_kontrak !== undefined) {
        $scope.selectedPengalaman.selesai_kontrak_view = $rootScope.convertTanggal($scope.selectedPengalaman.selesai_kontrak);
        //console.info("jadinya $scope.selectedPengalaman.selesai_kontrak_view --> "+$scope.selectedPengalaman.selesai_kontrak_view);
    }
    else {
        $scope.selectedPengalaman.selesai_kontrak_view = "";
    }

    if ($scope.selectedPengalaman.tgl_serahterima !== '' && $scope.selectedPengalaman.tgl_serahterima !== null && $scope.selectedPengalaman.tgl_serahterima !== undefined) {
        $scope.selectedPengalaman.tgl_serahterima_view = $rootScope.convertTanggal($scope.selectedPengalaman.tgl_serahterima);
        //console.info("jadinya $scope.selectedPengalaman.tgl_serahterima_view --> "+$scope.selectedPengalaman.tgl_serahterima_view);
    }
    else {
        $scope.selectedPengalaman.tgl_serahterima_view = "";
    }

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

};
/* detail rincian detail tenaga ahli */
var detailTenagaAhliModal = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    $scope.page_id = 129;
    $scope.detailTA_cv_pengalaman = [];
    $scope.detailTA_cv_pendidikan = [];
    $scope.detailTA_cv_sertifikat = [];
    $scope.detailTA_cv_bahasa = [];

    $scope.selectedTA = item;
    if ($scope.selectedTA.tenaga_ahli_id == null || $scope.selectedTA.tenaga_ahli_id == '') {
        $scope.fieldCV = "baru";
        $scope.id_ta_new = $scope.selectedTA.kualifikasi_ta_id;
    }
    else {
        $scope.fieldCV = "lama";
        $scope.id_ta_new = $scope.selectedTA.tenaga_ahli_id;
    }

    var arr = [];
    arr.push($scope.id_ta_new);
    arr.push(1);
    $http.post($rootScope.url_api+'dokkualifikasi/tacv', {sessionID: $cookieStore.get('sessId'), param: arr, page_id: $scope.page_id, action: $scope.fieldCV}).success(function(reply) {
        if (reply.status === 200) {
            $scope.detailTA_cv_pengalaman = reply.result.data;           
        }
        else {          
            return;
        }
    });
    var arr2 = [];
    arr2.push($scope.id_ta_new);
    arr2.push(2);
    $http.post($rootScope.url_api+'dokkualifikasi/tacv', {sessionID: $cookieStore.get('sessId'), param: arr2, page_id: $scope.page_id, action: $scope.fieldCV}).success(function(reply) {
        if (reply.status === 200) {
            $scope.detailTA_cv_pendidikan = reply.result.data;
        }
        else {
            return;
        }
    });
    var arr3 = [];
    arr3.push($scope.id_ta_new);
    arr3.push(3);
    $http.post($rootScope.url_api+'dokkualifikasi/tacv', {sessionID: $cookieStore.get('sessId'), param: arr3, page_id: $scope.page_id, action: $scope.fieldCV}).success(function(reply) {
        if (reply.status === 200) {
            $scope.detailTA_cv_sertifikat = reply.result.data;
        }
        else {
            return;
        }
    });
    var arr4 = [];
    arr4.push($scope.id_ta_new);
    arr4.push(4);
    $http.post($rootScope.url_api+'dokkualifikasi/tacv', {sessionID: $cookieStore.get('sessId'), param: arr4, page_id: $scope.page_id, action: $scope.fieldCV}).success(function(reply) {
        if (reply.status === 200) {
            $scope.detailTA_cv_bahasa = reply.result.data;
        }
        else {
            return;
        }
    });

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

};