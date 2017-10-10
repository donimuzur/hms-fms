/*
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

        //count of property object
        function numAttrs(obj) {
            var count = 0;
            for (var key in obj) {
                if (obj.hasOwnProperty(key)) {
                    ++count;
                }
            }
            return count;
        }

        //start upload
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
                        var Sheet1 = excelContents[Object.keys(excelContents)[0]]; //untuk baca nama sheet
                        var countproperty = numAttrs(Sheet1[0]);
                        //console.info("excel:" + JSON.stringify(Sheet1[0]) + numAttrs(Sheet1[0]));

                        vm.ColChck; vm.ColInputDate; vm.ColApprovalDate; vm.COlDeletionInd;
                        vm.ColPurchaseReq; vm.ColRequisnItem; vm.ColPurchGroup; vm.ColRequisitioner; vm.ColName;
                        vm.ColShortText; vm.ColMaterial; vm.ColQuantity; vm.ColUnitMeasure; vm.ColTrackingNumber;
                        vm.ColTotalValue; vm.ColUrgentNeed; vm.ColPlant; vm.ColTypeRequest; vm.ColPurchasingOrg;
                        vm.ColItemCategory; vm.ColMaterialGroup; vm.ColAgreement; vm.ColDesiredVendor; vm.ColFixedVendor;
                        vm.ColAcctAssgtCat; vm.ColSupplyingPlant; vm.ColTime; vm.ColStorLocation; vm.ColProcessingStat;
                        vm.ColAgreementItem; vm.ColMPNMaterial; vm.ColDeliveryDate; vm.ColInfoRecord; vm.ColFileDocument;
                        vm.ColIsUsed;


                        for (var i = 1; i <= countproperty ; i++) {
                            if (Sheet1[0]['Column' + i] === 'Chck') { vm.ColChck = i;    }
                            if (Sheet1[0]['Column' + i] === 'Date') { vm.ColInputDate = i;  }
                            if (Sheet1[0]['Column' + i] === 'Date of approval of the request') { vm.ColApprovalDate = i;  }
                            if (Sheet1[0]['Column' + i] === 'Deletion Ind.') { vm.ColDeletionInd = i;  }
                            if (Sheet1[0]['Column' + i] === 'Purchase Req.') { vm.ColPurchaseReq = i;  }
                            if (Sheet1[0]['Column' + i] === 'Requisn Item') { vm.ColRequisnItem = i;  }
                            if (Sheet1[0]['Column' + i] === 'Purch. Group') { vm.ColPurchGroup = i;  }
                            if (Sheet1[0]['Column' + i] === 'Requisitioner') { vm.ColRequisitioner = i;  }
                            if (Sheet1[0]['Column' + i] === 'Name') { vm.ColName = i;  }
                            if (Sheet1[0]['Column' + i] === 'Short Text') { vm.ColShortText = i;  }
                            if (Sheet1[0]['Column' + i] === 'Material') { vm.ColMaterial = i;  }
                            if (Sheet1[0]['Column' + i] === 'Quantity') { vm.ColQuantity = i;  }
                            if (Sheet1[0]['Column' + i] === 'Unit of Measure') { vm.ColUnitMeasure = i;  }
                            if (Sheet1[0]['Column' + i] === 'Tracking Number') { vm.ColTrackingNumber = i;  }
                            if (Sheet1[0]['Column' + i] === 'Total Value of Request') { vm.ColTotalValue = i;  }
                            if (Sheet1[0]['Column' + i] === 'Urgent Need') { vm.ColUrgentNeed = i;  }
                            if (Sheet1[0]['Column' + i] === 'Plant') { vm.ColPlant = i;  }
                            if (Sheet1[0]['Column' + i] === 'Type of Request') { vm.ColTypeRequest = i;  }
                            if (Sheet1[0]['Column' + i] === 'Purchasing Org.') { vm.ColPurchasingOrg = i;  }
                            if (Sheet1[0]['Column' + i] === 'Item Category') { vm.ColItemCategory = i;  }
                            if (Sheet1[0]['Column' + i] === 'Material Group') { vm.ColMaterialGroup = i;  }
                            if (Sheet1[0]['Column' + i] === 'Desired Vendor') { vm.ColDesiredVendor = i;  }
                            if (Sheet1[0]['Column' + i] === 'Agreement') { vm.ColAgreement = i;  }
                            if (Sheet1[0]['Column' + i] === 'Fixed Vendor') { vm.ColFixedVendor = i;  }
                            if (Sheet1[0]['Column' + i] === 'Acct Assgt Cat.') { vm.ColAcctAssgtCat = i;  }
                            if (Sheet1[0]['Column' + i] === 'Supplying Plant') { vm.ColSupplyingPlant = i;  }
                            if (Sheet1[0]['Column' + i] === 'Time') { vm.ColTime = i;  }
                            if (Sheet1[0]['Column' + i] === 'Stor. Location') { vm.ColStorLocation = i;  }
                            if (Sheet1[0]['Column' + i] === 'Processing stat') { vm.ColProcessingStat = i;  }
                            if (Sheet1[0]['Column' + i] === 'Agreement Item') { vm.ColAgreementItem = i;  }
                            if (Sheet1[0]['Column' + i] === 'MPN Material') { vm.ColMPNMaterial = i;  }
                            if (Sheet1[0]['Column' + i] === 'Delivery Date') { vm.ColDeliveryDate = i;  }
                            if (Sheet1[0]['Column' + i] === 'Info Record') { vm.ColInfoRecord = i;  }
                            
                            if (!(vm.ColChck === undefined) &&
                                !(vm.ColInputDate === undefined) &&
                                !(vm.ColApprovalDate === undefined) &&
                                !(vm.COlDeletionInd === undefined) &&
                                !(vm.ColPurchaseReq === undefined) &&
                                !(vm.ColRequisnItem === undefined) &&
                                !(vm.ColPurchGroup === undefined) &&
                                !(vm.ColRequisitioner === undefined) &&
                                !(vm.ColName === undefined) &&
                                !(vm.ColShortText === undefined) &&
                                !(vm.ColMaterial === undefined) &&
                                !(vm.ColQuantity === undefined) &&
                                !(vm.ColUnitMeasure === undefined) &&
                                !(vm.ColTrackingNumber === undefined) &&
                                !(vm.ColTotalValue === undefined) &&
                                !(vm.ColUrgentNeed === undefined) &&
                                !(vm.ColPlant === undefined) &&
                                !(vm.ColTypeRequest === undefined) &&
                                !(vm.ColPurchasingOrg === undefined) &&
                                !(vm.ColItemCategory === undefined) &&
                                !(vm.ColMaterialGroup === undefined) &&
                                !(vm.ColAgreement === undefined) &&
                                !(vm.ColDesiredVendor === undefined) &&
                                !(vm.ColFixedVendor === undefined) &&
                                !(vm.ColAcctAssgtCat === undefined) &&
                                !(vm.ColSupplyingPlant === undefined) &&
                                !(vm.ColTime === undefined) &&
                                !(vm.ColStorLocation === undefined) &&
                                !(vm.ColProcessingStat === undefined) &&
                                !(vm.ColAgreementItem === undefined) &&
                                !(vm.ColMPNMaterial === undefined) &&
                                !(vm.ColDeliveryDate === undefined) &&
                                !(vm.ColInfoRecord === undefined)
                                ){
                                break;
                            }
                        }
                        //cek jika ada kolom excel tidak sesuai format
                        if ((vm.ColChck === undefined) || (vm.ColInputDate === undefined) ||
                               (vm.ColApprovalDate === undefined) || (vm.ColDeletionInd === undefined) ||
                               (vm.ColPurchaseReq === undefined) || (vm.ColRequisnItem === undefined) ||
                               (vm.ColPurchGroup === undefined) || (vm.ColRequisitioner === undefined) ||
                               (vm.ColName === undefined) || (vm.ColShortText === undefined) || (vm.ColMaterial === undefined) ||
                               (vm.ColQuantity === undefined) || (vm.ColUnitMeasure === undefined) ||
                               (vm.ColTrackingNumber === undefined) || (vm.ColTotalValue === undefined) ||
                               (vm.ColUrgentNeed === undefined) || (vm.ColPlant === undefined) || (vm.ColTypeRequest === undefined) ||
                               (vm.ColPurchasingOrg === undefined) || (vm.ColItemCategory === undefined) || (vm.ColMaterialGroup === undefined) ||
                               (vm.ColAgreement === undefined) || (vm.ColDesiredVendor === undefined) || (vm.ColFixedVendor === undefined) ||
                               (vm.ColAcctAssgtCat === undefined) || (vm.ColSupplyingPlant === undefined) || (vm.ColTime === undefined) ||
                               (vm.ColStorLocation === undefined) || (vm.ColProcessingStat === undefined) || (vm.ColAgreementItem === undefined) ||
                               (vm.ColMPNMaterial === undefined) || (vm.ColDeliveryDate === undefined) || (vm.ColInfoRecord === undefined)
                            ) {
                                UIControlService.msg_growl("warning", "ITEMPRUPLOAD.MSG_ERR_EXCEL");
                                return;
                        }
                        vm.newExcel = []
                        for (var a = 1; a < Sheet1.length; a++) {
                            var isused;
                            var material = Sheet1[a]['Column' + vm.ColMaterial];
                            //console.info(a+"::"+Sheet1[a]['Column' + vm.ColMaterial]);
                            if (!(material) === undefined && !(material === null)) {
                                material = material;
                                isused = true;
                            } else { isused = false; material = 0; }
                            //convert date input
                            console.info(new Date(Sheet1[a]['Column' + vm.ColTime]));
                            var dateinput = Sheet1[a]['Column' + vm.ColInputDate];
                            var dateapproval = Sheet1[a]['Column' + vm.ColApprovalDate];
                            var datedelivery = Sheet1[a]['Column' + vm.ColDeliveryDate];
                           
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
                                Chck: Sheet1[a]['Column' + vm.ColChck],
                                InputDate: dateinput,
                                ApprovalDate: dateapproval,
                                DeletionInd: Sheet1[a]['Column' + vm.ColDeletionInd],
                                PurchaseReq: Sheet1[a]['Column' + vm.ColPurchaseReq],
                                RequisnItem: Sheet1[a]['Column' + vm.ColRequisnItem],
                                PurchGroup: Sheet1[a]['Column' + vm.ColPurchGroup],
                                Requisitioner: Sheet1[a]['Column' + vm.ColRequisitioner],
                                Name: Sheet1[a]['Column' + vm.ColName],
                                ShortText: Sheet1[a]['Column' + vm.ColShortText],
                                Material: material,
                                Quantity: Sheet1[a]['Column' + vm.ColQuantity],
                                UnitMeasure: Sheet1[a]['Column' + vm.ColUnitMeasure],
                                TrackingNumber: Sheet1[a]['Column' + vm.ColTrackingNumber],
                                TotalValue: Sheet1[a]['Column' + vm.ColTotalValue],
                                UrgentNeed: Sheet1[a]['Column' + vm.ColUrgentNeed],
                                Plant: Sheet1[a]['Column' + vm.ColPlant],
                                TypeRequest: Sheet1[a]['Column' + vm.ColTypeRequest],
                                PurchasingOrg: Sheet1[a]['Column' + vm.ColPurchasingOrg],
                                ItemCategory: Sheet1[a]['Column' + vm.ColItemCategory],
                                MaterialGroup: Sheet1[a]['Column' + vm.ColMaterialGroup],
                                Agreement: Sheet1[a]['Column' + vm.ColAgreement],
                                DesiredVendor: Sheet1[a]['Column' + vm.ColDesiredVendor],
                                FixedVendor: Sheet1[a]['Column' + vm.ColFixedVendor],
                                AcctAssgtCat: Sheet1[a]['Column' + vm.ColAcctAssgtCat],
                                SupplyingPlant: Sheet1[a]['Column' + vm.ColSupplyingPlant],
                                Time: new Date(Sheet1[a]['Column' + vm.ColTime]),
                                StorLocation: Sheet1[a]['Column' + vm.ColStorLocation],
                                ProcessingStat: Sheet1[a]['Column' + vm.ColProcessingStat],
                                AgreementItem: Sheet1[a]['Column' + vm.ColAgreementItem],
                                MPNMaterial: Sheet1[a]['Column' + vm.ColMPNMaterial],
                                DeliveryDate: datedelivery,
                                InfoRecord: Sheet1[a]['Column' + vm.ColInfoRecord],
                                FileDocument: '',
                                IsUsed: isused
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
            console.info(JSON.stringify(data));
            PurchReqService.insertUploadExcel(data, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    console.info("hasil:" + JSON.stringify(reply));
                    UIControlService.msg_growl("success", "ITEMPRUPLOAD.MSG_SUC_SAVE");
                    if (UIControlService.validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                        upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.tglSekarang);
                    }
                    //$state.transitionTo('master-rate');
                }
                else {
                    UIControlService.msg_growl("error", "ITEMPRUPLOAD.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
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
    }
})();*/