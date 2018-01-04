(function () {
    'use strict';

    angular.module("app")
    .controller("UploadItemPRCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PurchaseRequisitionService', '$state',
        'UIControlService', 'UploadFileConfigService', 'ExcelReaderService', 'UploaderService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PurchReqService, $state, UIControlService,
        UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = this;
        vm.fileUpload;
        vm.selectedSearchBy = 0;
        vm.maxSize = 10;
        vm.currentPage = 1;
        vm.textSearch = "";
        vm.textDate = null;
        vm.listItemPR = [];
        vm.totalItems = 0;
        
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("purchase-requisition");
            loadTypeSizeFile();
            vm.jLoad(1);
        }

        vm.changeCombo = changeCombo;
        function changeCombo() {
            //console.info("by:" + vm.selectedSearchBy);
            if (vm.selectedSearchBy === '0') {
                vm.jLoad(1);
            }
        }

        vm.openCalendar = openCalendar;
        vm.isCalendarOpened = [false, false, false, false];
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (vm.currentPage * vm.maxSize) - vm.maxSize;
            
            PurchReqService.getDataExcel({
                Offset: offset,
                Limit: vm.maxSize,
                Keyword: vm.textSearch,
                Date1: UIControlService.getStrDate(vm.textDate),
                column: vm.selectedSearchBy
            }, function (reply) {
                //console.info("dta:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listItemPR = data.List;
                    for (var i = 0; i < vm.listItemPR.length; i++) {
                        if (vm.listItemPR[i].InputDate === null) { vm.listItemPR[i].InputDate = "-"; }
                        else { vm.listItemPR[i].InputDate = UIControlService.getStrDate(vm.listItemPR[i].InputDate); }

                        if (vm.listItemPR[i].ApprovalDate === null) { vm.listItemPR[i].ApprovalDate = "-"; }
                        else { vm.listItemPR[i].ApprovalDate = UIControlService.getStrDate(vm.listItemPR[i].ApprovalDate); }

                        if (vm.listItemPR[i].DeliveryDate === null) { vm.listItemPR[i].DeliveryDate = "-"; }
                        else { vm.listItemPR[i].DeliveryDate = UIControlService.getStrDate(vm.listItemPR[i].DeliveryDate); }
                    }
                    vm.totalItems = Number(data.Count);
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOADDATA");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoading();
            });
        }

        function loadTypeSizeFile() {
            UIControlService.loadLoading("MESSAGE.LOADING");
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
                UIControlService.msg_growl("error", "MESSAGE.API");
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

        /*start upload */
        vm.uploadFile = uploadFile;
        function uploadFile() {
            if (vm.fileUpload === undefined) {
                UIControlService.msg_growl("error", "ITEMPRUPLOAD.MSG_NOFILE");
                return;
            }
            UIControlService.loadLoading(vm.message);
            ExcelReaderService.readExcel(vm.fileUpload,
                function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        var excelContents = reply.data;
                        var Sheet1 = excelContents[Object.keys(excelContents)[0]]; /*untuk baca nama sheet*/
                        var countproperty = numAttrs(Sheet1[0]);
                        //console.info("excel:" + JSON.stringify(Sheet1[0]) + numAttrs(Sheet1[0]));
                        vm.columns = [];
                        vm.columns['ColChck'] = null; vm.columns['ColInputDate'] = null; vm.columns['ColApprovalDate'] = null; vm.columns['COlDeletionInd'] = null;
                        vm.columns['ColPurchaseReq'] = null; vm.columns['ColRequisnItem'] = null; vm.columns['ColPurchGroup'] = null; vm.columns['ColRequisitioner'] = null; vm.columns['ColName'] = null;
                        vm.columns['ColShortText'] = null; vm.columns['ColMaterial'] = null; vm.columns['ColQuantity'] = null; vm.columns['ColUnitMeasure'] = null; vm.columns['ColTrackingNumber'] = null;
                        vm.columns['ColTotalValue'] = null; vm.columns['ColUrgentNeed'] = null; vm.columns['ColPlant'] = null; vm.columns['ColTypeRequest'] = null; vm.columns['ColPurchasingOrg'] = null;
                        vm.columns['ColItemCategory'] = null; vm.columns['ColMaterialGroup'] = null; vm.columns['ColAgreement'] = null; vm.columns['ColDesiredVendor'] = null; vm.columns['ColFixedVendor'] = null;
                        vm.columns['ColAcctAssgtCat'] = null; vm.columns['ColSupplyingPlant'] = null; vm.columns['ColTime'] = null; vm.columns['ColStorLocation'] = null; vm.columns['ColProcessingStat'] = null;
                        vm.columns['ColAgreementItem'] = null; vm.columns['ColMPNMaterial'] = null; vm.columns['ColDeliveryDate'] = null; vm.columns['ColInfoRecord'] = null; vm.columns['ColFileDocument'] = null;
                        vm.columns['ColIsUsed'] = null;


                        for (var i = 1; i <= countproperty ; i++) {
                            //if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'Chck') { vm.columns['ColChck'] = i; } //???
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PREQ_DATE') { vm.columns['ColInputDate'] = i; } //???
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'REL_DATE') { vm.columns['ColApprovalDate'] = i;  } //???
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'DELETE_IND') { vm.columns['ColDeletionInd'] = i; }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PR NUMBER') { vm.columns['ColPurchaseReq'] = i; }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PREQ_ITEM') { vm.columns['ColRequisnItem'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PUR_GROUP') { vm.columns['ColPurchGroup'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PREQ_NAME') { vm.columns['ColRequisitioner'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'CREATED_BY') { vm.columns['ColName'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'SHORT_TEXT') { vm.columns['ColShortText'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'MATERIAL') { vm.columns['ColMaterial'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'QUANTITY') { vm.columns['ColQuantity'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'UNIT') { vm.columns['ColUnitMeasure'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'TRACKINGNO') { vm.columns['ColTrackingNumber'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'VALUE_ITEM') { vm.columns['ColTotalValue'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PRIO_URGENCY') { vm.columns['ColUrgentNeed'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PLANT') { vm.columns['ColPlant'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'DOC_TYPE') { vm.columns['ColTypeRequest'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PURCH_ORG') { vm.columns['ColPurchasingOrg'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'ITEM_CAT') { vm.columns['ColItemCategory'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'MATL_GROUP') { vm.columns['ColMaterialGroup'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'DES_VENDOR') { vm.columns['ColDesiredVendor'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'AGREEMENT') { vm.columns['ColAgreement'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'FIXED_VEND') { vm.columns['ColFixedVendor'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'ACCTASSCAT') { vm.columns['ColAcctAssgtCat'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'SUPPL_PLNT') { vm.columns['ColSupplyingPlant'] = i;  }
                            //if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'Time') { vm.columns['ColTime'] = i;  } //???
                            //if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'Stor. Location') { vm.columns['ColStorLocation'] = i;  } //???
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'PROC_STAT') { vm.columns['ColProcessingStat'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'AGMT_ITEM') { vm.columns['ColAgreementItem'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'MANU_MAT') { vm.columns['ColMPNMaterial'] = i; }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'DELIV_DATE') { vm.columns['ColDeliveryDate'] = i;  }
                            if (Sheet1[0]['Column' + i].trim().toUpperCase() === 'INFO_REC') { vm.columns['ColInfoRecord'] = i;  }
                            
                            if (//!(vm.columns['ColChck'] === null) &&
                                !(vm.columns['ColInputDate'] === null) &&
                                !(vm.columns['ColApprovalDate'] === null) &&
                                !(vm.columns['COlDeletionInd'] === null) &&
                                !(vm.columns['ColPurchaseReq'] === null) &&
                                !(vm.columns['ColRequisnItem'] === null) &&
                                !(vm.columns['ColPurchGroup'] === null) &&
                                !(vm.columns['ColRequisitioner'] === null) &&
                                !(vm.columns['ColName'] === null) &&
                                !(vm.columns['ColShortText'] === null) &&
                                !(vm.columns['ColMaterial'] === null) &&
                                !(vm.columns['ColQuantity'] === null) &&
                                !(vm.columns['ColUnitMeasure'] === null) &&
                                !(vm.columns['ColTrackingNumber'] === null) &&
                                !(vm.columns['ColTotalValue'] === null) &&
                                !(vm.columns['ColUrgentNeed'] === null) &&
                                !(vm.columns['ColPlant'] === null) &&
                                !(vm.columns['ColTypeRequest'] === null) &&
                                !(vm.columns['ColPurchasingOrg'] === null) &&
                                !(vm.columns['ColItemCategory'] === null) &&
                                !(vm.columns['ColMaterialGroup'] === null) &&
                                !(vm.columns['ColAgreement'] === null) &&
                                !(vm.columns['ColDesiredVendor'] === null) &&
                                !(vm.columns['ColFixedVendor'] === null) &&
                                !(vm.columns['ColAcctAssgtCat'] === null) &&
                                !(vm.columns['ColSupplyingPlant'] === null) &&
                                //!(vm.columns['ColTime'] === null) &&
                                //!(vm.columns['ColStorLocation'] === null) &&
                                !(vm.columns['ColProcessingStat'] === null) &&
                                !(vm.columns['ColAgreementItem'] === null) &&
                                !(vm.columns['ColMPNMaterial'] === null) &&
                                !(vm.columns['ColDeliveryDate'] === null) &&
                                !(vm.columns['ColInfoRecord'] === null)
                                ){
                                break;
                            }
                        }
                        //cek jika ada kolom excel tidak sesuai format
                        if (//(vm.columns['ColChck'] === null) ||
                            (vm.columns['ColInputDate'] === null) ||
                            (vm.columns['ColApprovalDate'] === null) ||
                            (vm.columns['ColDeletionInd'] === null) ||
                            (vm.columns['ColPurchaseReq'] === null) ||
                            (vm.columns['ColRequisnItem'] === null) ||
                            (vm.columns['ColPurchGroup'] === null) ||
                            (vm.columns['ColRequisitioner'] === null) ||
                            (vm.columns['ColName'] === null) ||
                            (vm.columns['ColShortText'] === null) ||
                            (vm.columns['ColMaterial'] === null) ||
                            (vm.columns['ColQuantity'] === null) ||
                            (vm.columns['ColUnitMeasure'] === null) ||
                            (vm.columns['ColTrackingNumber'] === null) ||
                            (vm.columns['ColTotalValue'] === null) ||
                            (vm.columns['ColUrgentNeed'] === null) ||
                            (vm.columns['ColPlant'] === null) ||
                            (vm.columns['ColTypeRequest'] === null) ||
                            (vm.columns['ColPurchasingOrg'] === null) ||
                            (vm.columns['ColItemCategory'] === null) ||
                            (vm.columns['ColMaterialGroup'] === null) ||
                            (vm.columns['ColAgreement'] === null) ||
                            (vm.columns['ColDesiredVendor'] === null) ||
                            (vm.columns['ColFixedVendor'] === null) ||
                            (vm.columns['ColAcctAssgtCat'] === null) ||
                            (vm.columns['ColSupplyingPlant'] === null) ||
                            //(vm.columns['ColTime'] === null) ||
                            //(vm.columns['ColStorLocation'] === null) ||
                            (vm.columns['ColProcessingStat'] === null) ||
                            (vm.columns['ColAgreementItem'] === null) ||
                            (vm.columns['ColMPNMaterial'] === null) ||
                            (vm.columns['ColDeliveryDate'] === null) ||
                            (vm.columns['ColInfoRecord'] === null)) {
                                UIControlService.msg_growl("warning", "ITEMPRUPLOAD.MSG_ERR_EXCEL");
                                return;
                        }
                        vm.newExcel = []
                        for (var a = 1; a < Sheet1.length; a++) {
                            /*
                            var isused;
                            var material = Sheet1[a]['Column' + vm.ColMaterial];
                            if (material) {
                                isused = true;
                            } else {
                                isused = false;
                            }
                            */
                            //convert date input
                            var dateinput = Sheet1[a]['Column' + vm.columns['ColInputDate']];
                            var dateapproval = Sheet1[a]['Column' + vm.columns['ColApprovalDate']];
                            var datedelivery = Sheet1[a]['Column' + vm.columns['ColDeliveryDate']];
                           
                            if (!(dateinput === null) && !(dateinput === undefined)) {
                                dateinput = UIControlService.convertDateFromExcel(dateinput);
                            }
                            else { dateinput = null; }

                            //convert approval date
                            if (!(dateapproval === null) && !(dateapproval === undefined)) {
                                dateapproval = UIControlService.convertDateFromExcel(dateapproval);
                            }
                            else { dateapproval = null; }

                            //convert  delivery date
                            if (!( datedelivery === null) && !(datedelivery === undefined)) {
                                datedelivery = UIControlService.convertDateFromExcel(datedelivery);
                            }
                            else { datedelivery = null; }
                            //console.info(Sheet1[a]['Column' + vm.ColInputDate] + UIControlService.getStrDate(Sheet1[a]['Column' + vm.ColInputDate]));
                            var objExcel = {
                                Chck: null,//Sheet1[a]['Column' + vm.columns['ColChck']],
                                InputDate: dateinput,
                                ApprovalDate: dateapproval,
                                DeletionInd: Sheet1[a]['Column' + vm.columns['ColDeletionInd']],
                                PurchaseReq: Sheet1[a]['Column' + vm.columns['ColPurchaseReq']],
                                RequisnItem: Sheet1[a]['Column' + vm.columns['ColRequisnItem']],
                                PurchGroup: Sheet1[a]['Column' + vm.columns['ColPurchGroup']],
                                Requisitioner: Sheet1[a]['Column' + vm.columns['ColRequisitioner']],
                                Name: Sheet1[a]['Column' + vm.columns['ColName']],
                                ShortText: Sheet1[a]['Column' + vm.columns['ColShortText']],
                                Material: Number(Sheet1[a]['Column' + vm.columns['ColMaterial']]),
                                Quantity: Number(Sheet1[a]['Column' + vm.columns['ColQuantity']]),
                                UnitMeasure: Sheet1[a]['Column' + vm.columns['ColUnitMeasure']],
                                TrackingNumber: Sheet1[a]['Column' + vm.columns['ColTrackingNumber']],
                                TotalValue: Sheet1[a]['Column' + vm.columns['ColTotalValue']],
                                UrgentNeed: Sheet1[a]['Column' + vm.columns['ColUrgentNeed']],
                                Plant: Sheet1[a]['Column' + vm.columns['ColPlant']],
                                TypeRequest: Sheet1[a]['Column' + vm.columns['ColTypeRequest']],
                                PurchasingOrg: Sheet1[a]['Column' + vm.columns['ColPurchasingOrg']],
                                ItemCategory: Sheet1[a]['Column' + vm.columns['ColItemCategory']],
                                MaterialGroup: Sheet1[a]['Column' + vm.columns['ColMaterialGroup']],
                                Agreement: Sheet1[a]['Column' + vm.columns['ColAgreement']],
                                DesiredVendor: Sheet1[a]['Column' + vm.columns['ColDesiredVendor']],
                                FixedVendor: Sheet1[a]['Column' + vm.columns['ColFixedVendor']],
                                AcctAssgtCat: Sheet1[a]['Column' + vm.columns['ColAcctAssgtCat']],
                                SupplyingPlant: Sheet1[a]['Column' + vm.columns['ColSupplyingPlant']],
                                Time: null,//new Date(Sheet1[a]['Column' + vm.columns['ColTime']]),
                                StorLocation: null,//Sheet1[a]['Column' + vm.columns['ColStorLocation']],
                                ProcessingStat: Sheet1[a]['Column' + vm.columns['ColProcessingStat']],
                                AgreementItem: Sheet1[a]['Column' + vm.columns['ColAgreementItem']],
                                MPNMaterial: Sheet1[a]['Column' + vm.columns['ColMPNMaterial']],
                                DeliveryDate: datedelivery,
                                InfoRecord: Sheet1[a]['Column' + vm.columns['ColInfoRecord']],
                                FileDocument: ''
                            }
                            vm.newExcel.push(objExcel);
                        }
                        //console.info("exe>"+JSON.stringify(vm.newExcel));
                        uploadSave(vm.newExcel);
                    }
                });
        }

        vm.tglSekarang = UIControlService.getDateNow("");
        function uploadSave(data) {
            UIControlService.loadLoading(vm.message);
            console.info(JSON.stringify(data));
            PurchReqService.insertUploadExcel(data, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    //console.info("hasil:" + JSON.stringify(reply));
                    UIControlService.msg_growl("success", "ITEMPRUPLOAD.MSG_SUC_SAVE");
                    //if (UIControlService.validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                    //    upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.tglSekarang);
                    //}
                    //$state.transitionTo('master-rate');
                    vm.jLoad(1);
                }
                else {
                    UIControlService.msg_growl("error", "ITEMPRUPLOAD.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.MSG_ERR_SAVE");
                UIControlService.unloadLoadingModal();
            });
        }

        /*
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

            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_ITEMPR",size, filters, dates,
                function (response) {
                    console.info("upload:" + JSON.stringify(response));
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = vm.folderFile + url;
                        UIControlService.msg_growl("success", "ITEMPRUPLOAD.MSG_SUC_UPLOADE");

                    } else {
                        UIControlService.msg_growl("error", "ITEMPRUPLOAD.MSG_ERR_UPLOAD");
                        return;
                    }
                },
                function (response) {
                    UIControlService.msg_growl("error", "MESSAGE.API")
                    UIControlService.unloadLoading();
                });

        }
        */
    }
})();