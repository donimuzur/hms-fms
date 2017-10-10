(function () {
    'use strict';

    angular.module("app")
            .controller("UploadFileVHSCtrl", ctrl);

    ctrl.$inject = ['$stateParams','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PPVHSService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'GlobalConstantService', 'ExcelReaderService', '$state'];
    function ctrl($stateParams, $http, $translate, $translatePartialLoader, $location, SocketService, PPVHSService, UploadFileConfigService,
        UIControlService, UploaderService, GlobalConstantService, ExcelReaderService, $state) {

        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.pathFile;
        vm.DocTypeID = Number($stateParams.DocTypeID);

        vm.newExcel = [];
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.tglSekarang = UIControlService.getDateNow("");
        vm.message = "Silahkan Tunggu";
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.totalItems = 0;
        vm.init = init;
        vm.vhs = [];
        function init() {
            
            loadAwal();
            GetRFQ();
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.VENDOR.VHS.OFFERENTRY", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    vm.idUploadConfigs = [];
                    vm.list = response.data;
                    for (var i = 0; i < vm.list.length; i++) {
                        if (vm.list[i].Name == "xls" || vm.list[i].Name == "xlsx") vm.idUploadConfigs.push(vm.list[i]);

                    }
                    vm.idFileTypes = UIControlService.generateFilterStrings(vm.idUploadConfigs);
                    vm.idFileSize = vm.idUploadConfigs[0];
                    
                } else {
                    UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
                return;
            });
        }

        vm.clear = clear;
        function clear() {
            vm.newExcel = [];
        }

        vm.loadAwal = loadAwal;
        function loadAwal() {
            PPVHSService.selectDetail({
                TenderDocTypeID: vm.DocTypeID,
                vhs: 
                {
                    TenderStepID: vm.StepID
                }
            }, function (reply) {
                UIControlService.unloadLoading();
                vm.newExcel = [];
                if (reply.status === 200) {
                    vm.det = reply.data;
                    console.info(reply.data);
                    for (var i = 0; i < vm.det.length; i++) {
                        
                        var objExcel = {
                            MaterialCode: vm.det[i].MaterialCode,
                            Estimate: vm.det[i].Estimate,
                            Unit: vm.det[i].Unit,
                            ItemDescrip: vm.det[i].ItemDescrip,
                            Manufacture: vm.det[i].Manufacture,
                            PartNo: vm.det[i].PartNo,
                            Currency: vm.det[i].Currency,
                            PriceIDR: vm.det[i].PriceIDR,
                            LeadTime: vm.det[i].LeadTime,
                            CountryOfOrigin: vm.det[i].CountryOfOrigin,
                            Remark: vm.det[i].Remark
                        }
                        
                        vm.newExcel.push(objExcel);
                        if (i == vm.det.length - 1) {
                            console.info(vm.newExcel);
                        }

                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evaluasi Teknis" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.GetRFQ = GetRFQ;
        function GetRFQ() {
            PPVHSService.selectRFQId({ Status: vm.TenderRefID }, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.RFQId = reply.data;
                    console.info(vm.RFQId);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data requisition" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                Status: vm.TenderRefID
            }
            PPVHSService.select(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.vhs = reply.data;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evaluasi Teknis" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


            vm.selectUpload = selectUpload;
        //vm.fileUpload;
        function selectUpload() {
            console.info((vm.fileUpload));
        }
        /*start upload */

        vm.uploadFile = uploadFile;
        function uploadFile() {
            ExcelReaderService.readExcel(vm.fileUpload,
                function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        var excelContents = reply.data;
                        var sheet1 = excelContents[Object.keys(excelContents)[0]];
                        vm.list = [];
                        for (var i = 0; i < sheet1.length; i++) {
                            if (sheet1[i].Column1 !== null && sheet1[i].Column2 !== null) {
                                vm.list.push(sheet1[i]);
                            }
                        }
                        for (var i = 1; i < vm.list.length; i++) {
                                var objExcel = {
                                    Code: vm.list[i].Column1,
                                    MaterialCode: vm.list[i].Column2,
                                    ItemDescrip: vm.list[i].Column3,
                                    Manufacture: vm.list[i].Column4,
                                    PartNo: vm.list[i].Column5,
                                    Estimate: vm.list[i].Column6,
                                    Unit: vm.list[i].Column7,
                                    Currency: vm.list[i].Column8,
                                    PriceIDR: vm.list[i].Column9,
                                    LeadTime: vm.list[i].Column10,
                                    CountryOfOrigin: vm.list[i].Column11,
                                    Remark: vm.list[i].Column12
                                };
                                if (vm.RFQId.RFQType === 1) {
                                    objExcel.LeadTime = vm.RFQId.LeadTime;

                                }
                                
                            vm.newExcel.push(objExcel);

                        }

                    }
                });
        }

        /*count of property object*/
        function numAttrs(obj) {
            var count = 0;
            for (var key in obj) {
                if (obj.hasOwnProperty(key)) {
                    ++count;
                }
            }
            return count;
        }

        vm.uploadSave = uploadSave;
        function uploadSave() {
            vm.flag = 0;
            for (var exc = 0; exc < vm.newExcel.length; exc++) {
                if (vm.newExcel[exc].CountryOfOrigin === null && vm.RFQId.RFQType == 2) {
                    vm.flag = 1;
                    UIControlService.msg_growl("error", "Maaf Country of origin tidak boleh kosong");
                    return;
                }
                if (vm.newExcel[exc].Currency === null) {
                    vm.flag = 1;
                    UIControlService.msg_growl("error", "Maaf Currency tidak boleh kosong");
                    return;
                }
                else  {
                    PPVHSService.cekCurr({
                        Keyword: vm.newExcel[exc].Currency
                    }, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            vm.flagCurr = reply.data;
                            if (vm.flagCurr == false) {
                                vm.flag = 1;
                                exc = (vm.newExcel.length - 1);
                                UIControlService.msg_growl("error", "Maaf Currency tidak sesuai");
                                return;
                            }
                        } else {
                            $.growl.error({ message: "Gagal mendapatkan data Tender" });
                            UIControlService.unloadLoading();
                        }
                    }, function (err) {
                        console.info("error:" + JSON.stringify(err));
                        //$.growl.error({ message: "Gagal Akses API >" + err });
                        UIControlService.unloadLoading();
                    });
                }
                if (exc === vm.newExcel.length - 1 && vm.flag === 0) {
                    saveAll();
                    return;
                }
             
               
                
               
            }
        }
            
            vm.saveAll = saveAll;
            function saveAll() {
                PPVHSService.selectStep({
                    ID: vm.StepID
                }, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        vm.step = reply.data;
                        PPVHSService.selectRFQ({
                            Status: reply.data.tender.TenderRefID
                        }, function (reply) {
                            UIControlService.unloadLoading();
                            if (reply.status === 200) {
                                vm.getRFQ = reply.data;
                                console.info(vm.getRFQ);
                                if (vm.getRFQ.Keyword === 'TENDER_OPTIONS_PACKAGE') {
                                    if (vm.getRFQ.Status !== vm.newExcel.length) {
                                        UIControlService.msg_growl("error", "Maaf penawaran anda tidak lengkap");
                                        return;
                                    }
                                    else {
                                        if (vm.fileUpload === undefined && vm.newExcel.length !== 0) {
                                            UIControlService.msg_growl("success", "Berhasil Simpan Data");
                                            init();
                                        }
                                        else if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                                            upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.tglSekarang);
                                        }
                                    }
                                }
                                else {
                                    if (vm.fileUpload === undefined && vm.newExcel.length !== 0) {
                                        UIControlService.msg_growl("success", "Berhasil Simpan Data");
                                        init();
                                    }
                                    else if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                                        upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.tglSekarang);
                                    }
                                }
                            } else {
                                $.growl.error({ message: "Gagal mendapatkan data" });
                                UIControlService.unloadLoading();
                            }
                        }, function (err) {
                            console.info("error:" + JSON.stringify(err));
                            //$.growl.error({ message: "Gagal Akses API >" + err });
                            UIControlService.unloadLoading();
                        });

                    } else {
                        $.growl.error({ message: "Gagal mendapatkan data Tender" });
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                    //$.growl.error({ message: "Gagal Akses API >" + err });
                    UIControlService.unloadLoading();
                });

            }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }
            return true;
        }

        vm.upload = upload;
        function upload(file, config, filters, dates, callback) {
            var size = config.Size;
            var unit = config.SizeUnitName;

            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
                vm.unit = "KB";
                vm.flag = 0;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
                vm.flag = 1;
            }
            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFileVHSOfferEntry(dates, file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;
                        if (vm.flag == 0) {
                            vm.size = Math.floor(s);
                        }
                        else if (vm.flag == 1) {
                            vm.size = Math.floor(s/(1024));
                        }
                        vm.listDetail = [];
                        vm.VHSOfferEntry = {};
                        vm.id = 0;
                        var data = {
                            DocumentUrl: vm.pathFile,
                            Filename: vm.name,
                            FileSize: vm.size,
                            IsPublish: false,
                            TenderDocTypeID: vm.DocTypeID,
                            TenderStepID: vm.StepID,
                            detail: vm.newExcel

                        }
                        UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
                        PPVHSService.Insert(data, function (reply) {
                            UIControlService.unloadLoading();
                            if (reply.status === 200) {
                                UIControlService.msg_growl("success", "Berhasil Simpan data");
                                window.location.reload();
                            }
                            else {
                                UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                                return;
                            }
                        }, function (err) {
                            UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                            UIControlService.unloadLoadingModal();
                        });
                        
                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                        return;
                    }
                },
                function (response) {
                    UIControlService.msg_growl("error", "MESSAGE.API")
                    UIControlService.unloadLoading();
                });

        }

        vm.kembali = kembali;
        function kembali() {
            PPVHSService.selectStep({
                ID: vm.StepID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.step = reply.data;
                    $state.transitionTo("pemasukan-penawaran-vhs-vendor", { StepID: vm.StepID, TenderRefID: vm.step.tender.TenderRefID, ProcPackType: vm.step.tender.ProcPackageType });
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Tender" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
            
        }
       
    }
})();