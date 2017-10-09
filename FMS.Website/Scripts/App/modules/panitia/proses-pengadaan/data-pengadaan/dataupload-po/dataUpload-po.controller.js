(function () {
    'use strict';

    angular.module("app").controller("dataUpload", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'dataUploadService', 'UIControlService', 'UploadFileConfigService', 'ExcelReaderService', 'GlobalConstantService', 'UploaderService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $translate, $translatePartialLoader, $location, SocketService, dataUploadService, UIControlService, UploadFileConfigService, ExcelReaderService, GlobalConstantService, UploaderService) {

        var vm = this;
        var loadmsg = "LOADING";
        vm.currentPage = 1;
        vm.fileUpload;
        vm.maxSize = 10;
        vm.DocName = "";
        vm.currentPage = 1;
        vm.listItemPO = [];
        vm.totalItems = 0;
        vm.keyword = "";
        vm.ID = Number($stateParams.id);
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('dataupload-po');
            loadTypeSizeFile();
            loadData();
        }
        function loadData(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("LOADING");
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            dataUploadService.Select({
                Offset: offset,
                Limit: vm.maxSize
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listItemPO = data.List;

                    for (var i = 0; i < vm.listItemPO.length; i++) {
                        if (vm.listItemPO[i].PODate === null) { vm.listItemPO[i].PODate = "-"; }
                        else { vm.listItemPO[i].PODate = UIControlService.getStrDate(vm.listItemPO[i].PODate); }
                        if (vm.listItemPO[i].PODueDate === null) { vm.listItemPO[i].PODueDate = "-"; }
                        else { vm.listItemPO[i].PODueDate = UIControlService.getStrDate(vm.listItemPO[i].PODueDate); }
                    }
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "MESSAGE.ERR_LOAD" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        function loadTypeSizeFile() {
            UIControlService.loadLoading("LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.MASTER.ITEMPR", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = UIControlService.generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];

                } else {
                    UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoading();
                return;
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
        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }

        vm.toDetail = toDetail;
        function toDetail(id) {
            $state.transitionTo('detail-dataupload-po', { id: id });
        }
        vm.toCompare = toCompare;
        function toCompare(id) {
            $state.transitionTo('compare-dataupload-po', { id: id });
        }

        //start upload
        vm.uploadFile = uploadFile;
        function uploadFile() {
            if (vm.fileUpload === undefined) {
                UIControlService.msg_growl("error", "MSG_NOFILE");
                return;
            }

            if (UIControlService.validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.tglSekarang);
            }            
        }

        function upload(file, config, filters, dates, callback) {
            //console.info(file);
            var size = config.Size;

            var unit = config.SizeUnitName;
            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
            }

            UIControlService.loadLoading("LOADING");
            UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_ADMIN", size, filters, dates,
                function (response) {
                    //console.info("upload:" + JSON.stringify(response.data));
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        var fileName = response.data.FileName;
                        vm.pathFile = vm.folderFile + url;
                        UIControlService.msg_growl("success", "MSG_SUC_UPLOAD");
                        saveExcelContent(fileName, url);
                    } else {
                        UIControlService.msg_growl("error", "MSG_ERR_UPLOAD");
                        return;
                    }
                },
                function (response) {
                    UIControlService.msg_growl("error", "MESSAGE.API")
                    UIControlService.unloadLoading();
                });

        }

        function saveExcelContent(fileName,url) {
            UIControlService.loadLoading(vm.message);
            ExcelReaderService.readExcel(vm.fileUpload,
                function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        var excelContents = reply.data;
                        var Sheet1 = excelContents[Object.keys(excelContents)[0]]; //untuk baca nama sheet
                        var countproperty = numAttrs(Sheet1[0]);
                        //console.info("excel:" + JSON.stringify(Sheet1[0]) + numAttrs(Sheet1[0]));

                        vm.ColVendorCode;
                        vm.ColPODate;
                        vm.ColPONumber;
                        vm.ColPOCurr;
                        vm.ColPOItem;
                        vm.ColMaterialNumber;
                        vm.ColQty;
                        vm.ColUM;
                        vm.ColDesc;
                        vm.ColPODue;
                        vm.ColNettPrice;
                        vm.ColTotalPrice;
                     //   vm.ColPOType;

                        for (var i = 1; i <= countproperty ; i++) {
                            if (Sheet1[0]['Column' + i] === 'Vendor Code') { vm.ColVendorCode = i; }
                            if (Sheet1[0]['Column' + i] === 'Po Date') { vm.ColPODate = i; }
                            if (Sheet1[0]['Column' + i] === 'PO Number') { vm.ColPONumber = i; }
                            if (Sheet1[0]['Column' + i] === 'PO Item') { vm.ColPOItem = i; }
                            if (Sheet1[0]['Column' + i] === 'Material Number') { vm.ColMaterialNumber = i; }
                            if (Sheet1[0]['Column' + i] === 'QTY') { vm.ColQty = i; }
                            if (Sheet1[0]['Column' + i] === 'Unit Measure') { vm.ColUM = i; }
                            if (Sheet1[0]['Column' + i] === 'Description') { vm.ColDesc = i; }
                            if (Sheet1[0]['Column' + i] === 'PO Due Date') { vm.ColPODue = i; }
                            if (Sheet1[0]['Column' + i] === 'PO Curr') { vm.ColPOCurr = i; }
                            if (Sheet1[0]['Column' + i] === 'Netto') { vm.ColNettPrice = i; }
                            if (Sheet1[0]['Column' + i] === 'Total') { vm.ColTotalPrice = i; }
                     //       if (Sheet1[0]['Column' + i] === 'Po Type') { vm.ColPOType = i; }

                            if (!(vm.ColVendorCode === undefined) &&
                                !(vm.ColPODate === undefined) &&
                                !(vm.ColPONumber === undefined) &&
                                !(vm.ColPOCurr === undefined) &&
                                !(vm.ColPOItem === undefined) &&
                                !(vm.ColMaterialNumber === undefined) &&
                                !(vm.ColQty === undefined) &&
                                !(vm.ColUM === undefined) &&
                                !(vm.ColDesc === undefined) &&
                                !(vm.ColPODue === undefined) &&
                                !(vm.ColNettPrice === undefined) &&
                                !(vm.ColTotalPrice === undefined) 
                         //       !(vm.ColPOType === undefined)
                                ) {
                                break;
                            }
                        }
                        //cek jika ada kolom excel tidak sesuai format
                        if ((vm.ColVendorCode === undefined) || (vm.ColPODate === undefined) ||
                                (vm.ColPODate === undefined) || (vm.ColPOCurr === undefined) || (vm.ColPOItem === undefined) ||
                               (vm.ColMaterialNumber === undefined) || (vm.ColQty === undefined) ||
                               (vm.ColUM === undefined) || (vm.ColDesc === undefined) ||
                               (vm.ColPODue === undefined) || (vm.ColNettPrice === undefined) ||
                               (vm.ColTotalPrice === undefined) 
                            ) {
                            UIControlService.msg_growl("warning", "ITEMPRUPLOAD.MSG_ERR_EXCEL");
                            return;
                        }
                        vm.newExcel = []
                        for (var a = 1; a < Sheet1.length; a++) {

                            //  console.info(new Date(Sheet1[a]['Column' + vm.ColPODate]));
                            var podate = Sheet1[a]['Column' + vm.ColPODate];
                            var poduedate = Sheet1[a]['Column' + vm.ColPODue];

                            //convert po date
                            if (!(podate === null) && !(podate === undefined)) {
                                podate = UIControlService.convertDateFromExcel(podate);
                            }
                            else { podate = null; }

                            //convert po due date
                            if (!(poduedate === null) && !(poduedate === undefined)) {
                                poduedate = UIControlService.convertDateFromExcel(poduedate);
                            }
                            else { poduedate = null; }

                            //console.info(Sheet1[a]['Column' + vm.ColInputDate] + UIControlService.getStrDate(Sheet1[a]['Column' + vm.ColInputDate]));
                            var objExcel = {

                                VendorCodeSAP: Sheet1[a]['Column' + vm.ColVendorCode],
                                PODate: podate,
                                PONumber: Sheet1[a]['Column' + vm.ColPONumber],
                                POItem: Sheet1[a]['Column' + vm.ColPOItem],
                                MaterialNumber: Sheet1[a]['Column' + vm.ColMaterialNumber],
                                Qty: Sheet1[a]['Column' + vm.ColQty],
                                UnitMeasure: Sheet1[a]['Column' + vm.ColUM],
                                Description: Sheet1[a]['Column' + vm.ColDesc],
                                PODueDate: poduedate,
                                POCurr: Sheet1[a]['Column' + vm.ColPOCurr],
                                NetPricePO: Sheet1[a]['Column' + vm.ColNettPrice],
                                TotalPricePO: Sheet1[a]['Column' + vm.ColTotalPrice]

                            };

                            vm.newExcel.push(objExcel);
                        }
                        uploadSave(fileName, url, vm.newExcel);
                    }
                });
        }

        vm.tglSekarang = UIControlService.getDateNow("");
        function uploadSave(filename,url,data) {
            console.info("dataUplode:" + JSON.stringify(data));
            dataUploadService.Insert({
                FileName: filename,
                DocUrl: url,
                UploadDataPOes: data
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    //console.info("hasil:" + JSON.stringify(reply));
                    UIControlService.msg_growl("success", "MSG_SUC_SAVE");
                    
                    //$state.transitionTo('master-rate');
                }
                else {
                    UIControlService.msg_growl("error", "MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

        

    }
})();
