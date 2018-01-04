(function () {
    'use strict';

    angular.module("app").factory("UIControlService", service);

    service.$inject = ['$state', 'growl', '$filter'];
    /* @ngInject */
    function service($state, growl, $filter) {
        // interfaces
        var service = {
            removeSpace: removeSpaces,
            generateCaptcha: captcha,
            verifyCaptcha: validCaptcha,
            loadLoading: loadLoading,
            loadLoadingModal: loadLoadingModal,
            unloadLoading: unloadLoading,
            unloadLoadingModal: unloadLoadingModal,
            getSession: getSession,
            upload: upload,
            uploadZip: uploadZip,
            uploadZipByUrl: uploadZipByUrl,
            msg_growl: msg_growl,
            handleRequestError: handleRequestError,
            handleUnauthorizedAccess: handleUnauthorizedAccess,
            log: log,
            convertDate: convertDate,
            convertDateTime: convertDateTime,
            getStrDate: getStrDate,
            getDateNow: getDateNow,
            getDateNow2: getDateNow2,
            convertDateEx: convertDateEx,
            getEndDateByWeekday: getEndDateByWeekday,
            generateFilterStrings: generateFilterStrings,
            getTimeExcel: getTimeExcel,
            convertDateFromExcel: convertDateFromExcel,
            getDateTimeExcel: getDateTimeExcel,
            validateFileType: validateFileType
        };

        return service;

        // implementation

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
            //console.info("tipefile: " + selectedFileType);
            if (selectedFileType === "vnd.ms-excel") {
                selectedFileType = "xls";
            }
            else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                selectedFileType = "xlsx";
            }
            else {
                selectedFileType = selectedFileType;
            }
            console.info("filenew:" + selectedFileType);
            //jika excel
            var allowed = false;
            for (var i = 0; i < allowedFileTypes.length; i++) {
                if (allowedFileTypes[i].Name == selectedFileType) {
                    allowed = true;
                    return allowed;
                }
            }

            if (!allowed) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_INVALID_FILETYPE");
                return false;
            }
        }

        function msg_growl(type, message, title, time) {
            //alert(JSON.stringify([type, message, title, time]));
            if (!time)
                time = 5000;

            var config = {
                ttl: time
            };

            if (title != undefined && title != null && title != "") {
                config.title = title;
            }


		    if (type == "info") {
		        growl.info(message, config);
		    }
		    else if (type == "warning") {
		        growl.warning(message, config); //orange
		    }
		    else if (type == "error") {
		        growl.error(message, config); //red
		    }
		    else if (type == "notice" || type == "success") {
		        growl.success(message, config); //green
		    }
		    else {
		        alert(message);
		    }
            
            //alert(msg);
        }

        function handleRequestError(message, title, redirectTo, growlTime) {
            if (!growlTime)
                growlTime = 5000;

            var config = {
                ttl : growlTime
            };

            if (title != undefined && title != null && title != "") {
                config.title = title;
            }

            growl.error(message, config);
            if (!redirectTo) {
                // do nothing
            } else {
                $state.go(redirectTo);
            }
        }

        function handleUnauthorizedAccess(redirectTo, growlTime) {
            handleRequestError("ERRORS.UNAUTHORIZED.MESSAGE", "ERRORS.UNAUTHORIZED.TITLE", redirectTo, growlTime);
        }

        function globalErrorRequestHandler(response) {
            log(response);
            handleRequestError("ERRORS.INTERNAL_ERROR.MESSAGE", "ERRORS.INTERNAL_ERROR.TITLE");
        }

        //get tipe dan max.size file - 2
        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        function uploadZipByUrl(url) {
            loadLoading("Uploading...");
            var deferred = $q.defer();
            $http.post($rootScope.url_api + "uploadZip", {
                url: url
            }).success(function (reply) {
                unloadLoading();
                deferred.resolve(reply);
            }).error(function (err) {
                unloadLoading();
                deferred.reject(err);
                $http.post($rootScope.url_api + "logging", {
                    message: "Tidak berhasil akses API : " + JSON.stringify(err),
                    source: "app.js - uploadZip"
                }).then(function (response) {
                    // do nothing
                    // don't have to feedback
                });
                $.growl.error({ title: "[PERINGATAN]", message: "Upload gagal" });
            });
            return deferred.promise;
        };

        function uploadZip(files, folder) {
            $rootScope.loadLoading("Uploading...");
            var deferred = $q.defer();
            var fd = new FormData();
            for (var i = 0; i < files.length; i++) {
                angular.forEach(files[i].file, function (item) {
                    fd.append("uploads[]", item);
                });
            }
            $http.post($rootScope.url_api + "uploadZip/" + folder, fd, {
                withCredentials: true,
                transformRequest: angular.identity(),
                headers: { 'Content-Type': undefined }
            }).success(function (reply) {
                $rootScope.unloadLoading();
                deferred.resolve(reply);
            }).error(function (err) {
                $rootScope.unloadLoading();
                deferred.reject(err);
                $http.post($rootScope.url_api + "logging", {
                    message: "Tidak berhasil akses API : " + JSON.stringify(err),
                    source: "app.js - uploadZip"
                }).then(function (response) {
                    // do nothing
                    // don't have to feedback
                });
                $.growl.error({ title: "[PERINGATAN]", message: "Upload gagal" });
            });
            return deferred.promise;
        }

        function upload(fileDocument, folder) {
            loadLoading("Uploading...");
            var deferred = $q.defer();
            var fd = new FormData();

            angular.forEach(fileDocument, function (item) {
                fd.append("uploads", item);
            });

            $http.post($rootScope.url_api + "upload/" + folder + "/", fd, {
                withCredentials: true,
                transformRequest: angular.identity(),
                headers: { 'Content-Type': undefined }
            }).success(function (reply) {

                deferred.resolve(reply);
                $rootScope.unloadLoading();
                console.info("reply di app " + JSON.stringify(reply));
            }).error(function (err) {

                deferred.reject(err);
                console.info("error di app " + JSON.stringify(err));
                $rootScope.unloadLoading();
                $http.post($rootScope.url_api + "logging", {
                    message: "Tidak berhasil akses API : " + JSON.stringify(err),
                    source: "app.js - upload"
                }).then(function (response) {
                    // do nothing
                    // don't have to feedback
                });
                $.growl.error({ title: "[PERINGATAN]", message: "Upload gagal app.js" });
            });
            return deferred.promise;

        }
        //function progress
        function loadLoading(msg) {
            $.blockUI({
                message: '<div style="text-align:center;opacity: 1"><img src="assets/img/loader2.gif" style="padding: 20px 20px 10px 20px; width:80px"/><br /><div style="margin-bottom: 20px">' + $filter('translate')(msg) + '</div></div>',
                css: { border: '1px solid a49db3', opacity: '0.9', filter: 'alpha(opacity=90)', 'font-size': '18px', 'font-family': 'afta_sansregular, Arial, sans-serif!important' }
            });
        }
        function loadLoadingModal(msg) {
            $('.modal-content').block({
                message: '<div style="text-align:center;opacity: 1"><img src="assets/img/loader2.gif" style="padding: 20px 20px 10px 20px; width:80px"/><br /><div style="margin-bottom: 20px">' + $filter('translate')(msg) + '</div></div>',
                css: { border: '1px solid a49db3', opacity: '0.9', filter: 'alpha(opacity=90)', 'font-size': '18px', 'font-family': 'afta_sansregular, Arial, sans-serif!important' }
            });
        }
        /*unloading */
        function unloadLoading() {
            $.unblockUI();
        }
        function unloadLoadingModal() {
            $('.modal-content').unblock();
        }

        function getSession() {
            //                    return $http.post($rootScope.url_api + "auth/get_session", {
            //                        session_id: $cookieStore.get("sessId")
            //                    });
        }
        function removeSpaces(string) {
            return string.split(' ').join('');
        }

        function captcha(id, imageId) {
            var alpha = new Array('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0');

            var i;
            var tCtx = document.getElementById(id).getContext('2d');
            var imageElem = document.getElementById(imageId);

            for (i = 0; i < 3; i++) {
                var a = alpha[Math.floor(Math.random() * alpha.length)];
                var b = alpha[Math.floor(Math.random() * alpha.length)];
                var c = alpha[Math.floor(Math.random() * alpha.length)];
                var d = alpha[Math.floor(Math.random() * alpha.length)];
            }
            var code = a + ' ' + b + ' ' + ' ' + c + ' ' + d;
            tCtx.canvas.width = tCtx.measureText(code).width;
            tCtx.fillText(code, 0, 10);
            imageElem.src = tCtx.canvas.toDataURL();
            return $.md5(removeSpaces(code));
        }

        function validCaptcha(value, id) {
            var string1 = value;
            var string2 = $.md5(removeSpaces(document.getElementById(id).value));
            if (string1 === string2) {
                return true;
            } else {
                //alert('ups, wrong captcha!!');
                msg_growl("error", "Wrong Captcha! Please re-entry", "Action Failed", 3000);
                var kosong = "";
                document.getElementById(id).value = kosong;
                return false;
            }
        }

        function log(response)
        {
            console.info(JSON.stringify(response));
        }

        //yyyy-mm-dd to dd/mm/yyyy
        function convertDate(d) {
            return d ? d.substring(8, 10) + '/' + d.substring(5, 7) + '/' + d.substring(0, 4) : d;
        }

        //yyyy-mm-ddTHH:MM:SS to dd/mm/yyyy HH:MM
        function convertDateTime(d) {
            return d ? d.substring(8, 10) + '/' + d.substring(5, 7) + '/' + d.substring(0, 4) + 
                ' ' + d.substring(11, 13) + ':' + d.substring(14, 16) : d;
        }

        //yyyymmdd to yyyy-mm-dd
        function convertDateEx(a2) {
            var a = a2.toString();
            return a ? a.substr(0, 4) + '-' + a.substr(4, 2) + '-' + a.substr(6, 2) : a;
        }

        //1899-12-31T00:00:00 to yyyy-mm-dd
        function convertDateFromExcel (a) {
            return a ? a.substr(0, 4) + '-' + a.substr(5, 2) + '-' + a.substr(8, 2) : a;
        }

        //Menghilangkan hasil konversi timezone dari datepicker sebelum dikirim ke DB
        function getStrDate(date) {
            date = new Date(Date.parse(date));
            var bln = date.getMonth() + 1;
            if (bln < 10)
                bln = "0" + bln;
            var tgl = date.getDate()
            if (tgl < 10)
                tgl = "0" + tgl;
            return date.getFullYear() + '-' + (bln) + '-' + (tgl);
        }

        //gettime
        function getTimeExcel(a) {
            return a ? a.substr(11, 8) : a;
        }

        //getDatetime
        function getDateTimeExcel(a) {
            return a ? a.substr(0, 4) + '-' + a.substr(5, 2) + '-' + a.substr(8, 2) + " " + a.substr(11, 8) : a;
        }

        function getEndDateByWeekday(startDate, duration) {
        	var date = new Date(startDate);
        	var newDate = date.getDate() + parseInt(duration);
        	var newMonth = date.getMonth();
        	var newYear = date.getFullYear();
        	var endDate = new Date(newYear, newMonth, newDate);

        	while (endDate.getDay() == 0 || endDate.getDay() == 6) {
        		newDate++;
        		endDate.setDate(newDate);
        	}

        	return endDate;
        }

        //fungsi get tanggal hari ini dd-mm-yyyy
        function getDateNow(space) {
            var now = new Date();
            var tahun = String(now.getFullYear());
            var bulan = String((now.getMonth() + 1));
            if (bulan.length === 1)
                bulan = "0" + bulan;
            var tanggal = String(now.getDate());
            if (tanggal.length === 1)
                tanggal = "0" + tanggal;
            var jam = String(now.getHours());
            if (jam.length === 1)
                jam = "0" + jam;
            var menit = String(now.getMinutes());
            if (menit.length === 1)
                menit = "0" + menit;
            var sekarang = tanggal + space + bulan + space + tahun;
            return sekarang;
        }

        //fungsi get tanggal hari ini yyyy-mm-dd
        function getDateNow2(space) {
            var now = new Date();
            var tahun = String(now.getFullYear());
            var bulan = String((now.getMonth() + 1));
            if (bulan.length === 1)
                bulan = "0" + bulan;
            var tanggal = String(now.getDate());
            if (tanggal.length === 1)
                tanggal = "0" + tanggal;
            var jam = String(now.getHours());
            if (jam.length === 1)
                jam = "0" + jam;
            var menit = String(now.getMinutes());
            if (menit.length === 1)
                menit = "0" + menit;
            var sekarang = tahun + space + bulan + space + tanggal;
            return sekarang;
        }
    }
})();