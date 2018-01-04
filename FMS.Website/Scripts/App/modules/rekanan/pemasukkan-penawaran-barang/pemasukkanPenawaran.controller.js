(function () {
    'use strict';

    angular.module("app").controller("PemasukkanPenawaranBarangVendorCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'GoodOfferEntryService',
        '$state', 'UIControlService', 'UploaderService', '$uibModal', 'GlobalConstantService', '$stateParams', 'PurchaseRequisitionService', 'VerifiedSendService', 'UploadFileConfigService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, GoodOfferEntryService, $state,
        UIControlService, UploaderService, $uibModal, GlobalConstantService, $stateParams, PurchReqService, VerifiedSendService, UploadFileConfigService) {
        var vm = this;
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.StepID = Number($stateParams.StepID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.IDDoc = Number($stateParams.DocID);
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.habisTanggal = false;
        vm.showterendah = true;
        vm.selectedCurrencies = {};
        vm.goods = [];
        vm.selectedFreightCostTime = {};
        vm.init = init;
        vm.Date;
        vm.status = 2;
        vm.listUpload = [];
        vm.listyyy = [];
        vm.listOtherDoc = [];
        vm.listOther = [];
        vm.listDelete = [];
        function init() {
            $translatePartialLoader.addPart('pemasukkan-penawaran-barang');
            UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);
            loadVerifiedVendor();
            UploadFileConfigService.getByPageName("PAGE.VENDOR.GOODSOFFERENTRY", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    vm.name = response.data.name;
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = generateFilterStrings(response.data);
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

        vm.selectedPaymentTerm;
        vm.listPaymentTerm = [];
        function loadPaymentTerm(data) {
            GoodOfferEntryService.getPaymentTerm(function (reply) {
                UIControlService.unloadLoading();
                vm.listPaymentTerm = reply.data;
                for (var i = 0; i < vm.listPaymentTerm.length; i++) {
                    if (data === vm.listPaymentTerm[i].Id) {
                        vm.selectedPaymentTerm = vm.listPaymentTerm[i];
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadReference = loadReference;
        function loadReference(data) {
            GoodOfferEntryService.DeliveryTerm({}, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.DeliveryTerm = reply.data.List;
                    for (var i = 0; i < vm.DeliveryTerm.length; i++) {
                        if (data == vm.DeliveryTerm[i].RefID) vm.selectedDeliveryTerms = vm.DeliveryTerm[i];
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data penawaran" });
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
            var tender = {
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            GoodOfferEntryService.select(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.goods = reply.data;
                    for (var i = 0; i < vm.goods.length; i++) {
                        if (i == vm.goods.length - 1) {
                                
                            vm.offer = vm.goods[i].goods.OfferTotalCost;
                            vm.Date = vm.goods[i].goods.CurrencyDate;
                        }
                    }
                    if (vm.goods[0].ID !== 0) {
                        loadOtherDoc( vm.goods[0].GoodsOEId);
                        vm.QLTime = vm.goods[0].goods.SupplierQLTime;
                        loadFreightCost2(vm.goods[0].goods.FreighCostId);
                    }
                    else {
                        loadFreightCost2();
                    }
                            
                    //loadIncoTerms();
                    //loadStateDelivery();
                    loadCurrencies();
                    loadDeliveryTerms();
                    loadFreightCost();
                    loadRFQItem();
                    loadRFQGoods();
                    //console.info("data:" + JSON.stringify(vm.goods));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Penawaran Barang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadOtherDoc = loadOtherDoc;
        function loadOtherDoc(data) {
            vm.listOtherDoc = [];
            GoodOfferEntryService.getOtherDoc({Status: data},function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.data = reply.data;
                    for (var i = 0; i < vm.data.length; i++) {
                        vm.listOtherDoc.push({
                            ID: vm.data[i].ID,
                            fileUpload: [{
                                name: vm.data[i].Filename
                            }],
                            Remark: vm.data[i].Remark,
                            DocUrl: vm.data[i].DocUrl
                        });
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Perusahaan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }
        vm.batal = batal;
        function batal() {
            $state.transitionTo('detail-tahapan-vendor', { TenderID: vm.goods[0].goods.TenderID });
        }

        vm.changePrice = changePrice;
        function changePrice(data) {
            data.TotalPrice = data.UnitPrice * data.item.Quantity;
        }
        vm.selectedIncoTerms;
        vm.listIncoTerms = [];
        function loadIncoTerms() {
            PurchReqService.getIncoTerms(function (reply) {
                UIControlService.unloadLoading();
                vm.listIncoTerms = reply.data;
                for (var i = 0; i < vm.listIncoTerms.length; i++) {
                    if (vm.goods[0].goods.IncoId === vm.listIncoTerms[i].ID) {
                        vm.selectedIncoTerms = vm.listIncoTerms[i];
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.selectedState;
        vm.listState = [];
        function loadStateDelivery() {
            PurchReqService.getStateDelivery(function (reply) {
                UIControlService.unloadLoading();
                vm.listState = reply.data;
                for (var i = 0; i < vm.listState.length; i++) {
                    if (vm.goods[0].goods.StateID === vm.listState[i].StateID) {
                        vm.selectedState = vm.listState[i];
                        loadCityDelivery(vm.selectedState);
                        //changeState();
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
            
        vm.selectedFreightCost2;
        vm.listFreightCost2 = [];
        function loadFreightCost2(data) {
            GoodOfferEntryService.GetFreightCost(function (reply) {
                UIControlService.unloadLoading();
                vm.listFreightCost2 = reply.data;
                if (data !== undefined) {
                    for (var i = 0; i < vm.listFreightCost2.length; i++) {
                        if (vm.listFreightCost2[i].FreightCostID == data) {
                            vm.selectedFreightCost2 = vm.listFreightCost2[i];
                        }
                    }
                }
                    
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.changeState = changeState;
        function changeState(data) {
            loadCityDelivery(data.StateID);
        }

        //list City
        vm.selectedCity;
        vm.listCity = [];
        function loadCityDelivery(data) {
            PurchReqService.getCityDelivery({ StateID: data.StateID }, function (reply) {
                UIControlService.unloadLoading();
                //console.info("city:" + JSON.stringify(reply));
                vm.listCity = reply.data;
                for (var i = 0; i < vm.listCity.length; i++) {
                    if (vm.goods[0].goods.CityID === vm.listCity[i].CityID) {
                        vm.selectedCity = vm.listCity[i];
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadCurrencies = loadCurrencies;
        vm.selectedCurrencies;
        vm.listCurrencies = [];
        var CurrencyID;
        function loadCurrencies() {
            GoodOfferEntryService.getCurrencies(
            function (response) {
                if (response.status === 200) {
                    vm.listCurrencies = response.data;
                    for (var i = 0; i < vm.listCurrencies.length; i++) {
                        if (vm.goods[0].goods.RateID === vm.listCurrencies[i].CurrencyID) {
                            vm.selectedCurrencies = vm.listCurrencies[i];
                            changeCurrent();
                            break;
                        }
                    }
                }
                else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan list Currency");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                return;
            });
        }

        vm.selectedDeliveryTerms;
        vm.listDeliveryTerms = [];
        function loadDeliveryTerms() {
            PurchReqService.getDeliveryTerms(function (reply) {
                UIControlService.unloadLoading();
                vm.listDeliveryTerms = reply.data.List;
                for (var i = 0; i < vm.listDeliveryTerms.length; i++) {
                    if (vm.goods[0].goods.DeliveryTerms === vm.listDeliveryTerms[i].RefID) {
                        vm.selectedDeliveryTerms = vm.listDeliveryTerms[i];
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
        
        vm.loadRFQItem = loadRFQItem;
        function loadRFQItem() {
            var status = {
                Status: vm.TenderRefID
            }
            GoodOfferEntryService.GetRFQ(status, function (reply) {
                UIControlService.unloadLoading();
                vm.RfqID = reply.data;
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadRFQGoods = loadRFQGoods;
        function loadRFQGoods() {
            var status = {
                Status: vm.TenderRefID
            }
            GoodOfferEntryService.GetRFQGoods(status, function (reply) {
                UIControlService.unloadLoading();
                vm.RfqGoods = reply.data;
                vm.TenderType = reply.data[0].TenderType;
                loadReference(reply.data[0].DeliveryTerms);
                loadPaymentTerm(reply.data[0].PaymentTerm);
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.selectedFreightCost;
        vm.listFreightCost = [];
        function loadFreightCost() {
            GoodOfferEntryService.getTypeFreightCost(function (reply) {
                UIControlService.unloadLoading();
                vm.listFreightCost = reply.data.List;
                for (var i = 0; i < vm.listFreightCost.length; i++) {
                    if (vm.goods[0].goods.FreightCostType === vm.listFreightCost[i].RefID) {
                        vm.selectedFreightCost = vm.listFreightCost[i];
                        changeFreight(vm.selectedFreightCost.RefID);
                        break;
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
            
        vm.changeFreight = changeFreight;
        vm.freight = [];
        vm.selectedFreightCostTime;
        function changeFreight(data) { 
            GoodOfferEntryService.GetFreightTime({Status: data}, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.freight = reply.data;
                    for (var i = 0; i < vm.freight.length; i++) {
                        if (vm.goods[0].goods.FreightLeadTime === vm.freight[i].DeliveryTime) {
                            vm.selectedFreightCostTime = vm.freight[i];
                        }
                    }
                        
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Penawaran Barang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.changeCurrent = changeCurrent;
        function changeCurrent() {
            var data = {
                Keyword:vm.selectedCurrencies.Symbol
            }
            GoodOfferEntryService.GetExchangeRate(data, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.rate = reply.data;
                    vm.ExchangeRate = vm.rate.ExchangeRate;
                    vm.Date = vm.rate.ValidFrom;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Penawaran Barang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.deleteRow = deleteRow;
        function deleteRow(index, data) {
            var idx = index - 1;
            var _length = vm.listOtherDoc.length; // panjangSemula
            vm.listOtherDoc.splice(idx, 1);
            if (data.ID != undefined) {
                vm.listDelete.push(data);
            }
            };

            vm.uploadFile = uploadFile;
            function uploadFile(flag, iplus, data) {
               
                if (flag === 0) {
                    if (validateFileType(data, vm.idUploadConfigs)) {
                     upload(iplus, data, vm.idFileSize, vm.idFileTypes, "");
                    }
                }
                else {
                    if (validateFileType(data, vm.idUploadConfigs)) {
                        upload1(iplus, data, vm.idFileSize, vm.idFileTypes, "");
                    }
                }
            }

            function validateFileType(data, allowedFileTypes) {
                var allowed = false;
                if (!data.fileUpload || data.fileUpload.length == 0) {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                    return false;
                }
                return true;
            }

            vm.upload1 = upload1;
            function upload1(iplus, data, config, filters, callback) {
                vm.flagS = 0;
                var size = config.Size;
                var unit = config.SizeUnitName;

                if (unit == 'SIZE_UNIT_KB') {
                    size *= 1024;
                    vm.flag = 0;
                }
                if (unit == 'SIZE_UNIT_MB') {
                    size *= (1024 * 1024);
                    vm.flag = 1;
                }
                if(iplus == 0)
                UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
                var id = data.fileUpload[0].name + '_' + data.Remark;
                data.FileName = data.fileUpload[0].name;
                UploaderService.uploadSingleFileGoodsOfferEntry(id, data.fileUpload, size, filters,
                    function (response) {
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            vm.flagS = 1;
                            var url = response.data.Url;
                            vm.pathFile = url;
                            data.DocUrl = vm.pathFile;
                            vm.name = response.data.FileName;
                            vm.listOther.push(data);
                            var s = response.data.FileLength;
                            GoodOfferEntryService.UploadOther(vm.listOther,
                          function (reply) {
                              if (reply.status === 200) {
                                  if (iplus == (vm.listOtherDoc.length - 1)) {
                                      if (vm.listUpload.length != 0) {
                                          for (var x = 0; x <= vm.listUpload.length; x++) {
                                              if (x != vm.listUpload.length) {
                                                  uploadFile(0, x, vm.listUpload[x]);
                                              }
                                          }
                                      }
                                      else {
                                          UIControlService.msg_growl("success", "Berhasil Simpan data");
                                          window.location.reload();
                                      } 
                                  }
                                  //if (vm.listUpload.length != 0) {
                                  //    for (x = 0; x <= vm.listUpload.length; x++) {
                                  //        if (x != vm.listUpload.length) {
                                  //            uploadFile(0, x, vm.listUpload[x]);
                                  //        }
                                  //    }
                                  //}
                              }
                              else {
                                  UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                  return;
                              }
                          },
                          function (err) {
                              UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                          }
                          );
                        } else {
                            UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                            return;
                        }
                    },
                    function (response) {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD")
                        UIControlService.unloadLoading();
                    });


            }

            vm.upload = upload;
            function upload(iplus, data, config, filters, callback) {
                var size = config.Size;
                var unit = config.SizeUnitName;

                if (unit == 'SIZE_UNIT_KB') {
                    size *= 1024;
                    vm.flag = 0;
                }
                if (unit == 'SIZE_UNIT_MB') {
                    size *= (1024 * 1024);
                    vm.flag = 1;
                }
                if (iplus == 0)
                    UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
                var id = data.ShortText + '_' + data.VendorName;
                console.info(data);
                UploaderService.uploadSingleFileGoodsOfferEntry(id, data.fileUpload, size, filters,
                    function (response) {
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            var url = response.data.Url;
                            vm.pathFile = url;
                            data.DocUrl = vm.pathFile;
                            vm.name = response.data.FileName;
                            vm.listyyy.push(data);
                            var s = response.data.FileLength;
                            GoodOfferEntryService.Upload(vm.listyyy,
                            function (reply) {
                                if (reply.status === 200) {
                                    vm.listyyy = [];
                                    UIControlService.msg_growl("success", "Berhasil Simpan data");
                                    if (iplus === vm.listUpload.length - 1) window.location.reload();
                                }
                                else {
                                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                    return;
                                }
                            },
                            function (err) {
                                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                            }
                            );
                            if (vm.flag == 0) {
                                vm.size = Math.floor(s);

                            }
                            if (vm.flag == 1) {
                                vm.size = Math.floor(s / (1024));
                            }
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

            vm.selectUpload = selectUpload;
            function selectUpload(data) {
                vm.listUpload.push({
                    ItemPRId: data.ItemPRId,
                    ShortText: data.item.ShortText,
                    VendorName: data.goods.VendorName,
                    fileUpload: data.fileUpload,
                    TenderStepID: vm.StepID
                });
                console.info(vm.listUpload);
            }

            vm.Simpan = Simpan;
            function Simpan() {
                vm.list = [];
                vm.up = 0;
                vm.flagList = false;
                GoodOfferEntryService.UploadOther(vm.listDelete,
                         function (reply) {
                             if (reply.status === 200) {
                             }
                             else {
                                 UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                 return;
                             }
                         },
                     function (err) {
                         UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                     }
                );
                for (var i = 0; i < vm.goods.length; i++) {
                    var data = {
                        ID: vm.goods[i].ID,
                        GoodsOEId: vm.goods[i].GoodsOEId,
                        ItemPRId: vm.goods[i].ItemPRId,
                        UnitPrice: vm.goods[i].UnitPrice,
                        LeadTime: vm.goods[i].LeadTime,
                        DocUrl: vm.goods[i].DocUrl,
                        Alternative: vm.goods[i].Alternative,
                        Remark: vm.goods[i].Remark,
                        goods: {
                            FreighCostId: vm.selectedFreightCost2.FreightCostID,
                            TenderStepID:vm.goods[i].goods.TenderStepID ,
                            VendorID:vm.goods[i].goods.VendorID,
                            RateID: vm.selectedCurrencies.CurrencyID,
                                CurrencyDate: UIControlService.getStrDate(vm.Date),
                            DeliveryTerms: 3088, //Belum di null
                            FreightCostType: vm.listFreightCost2.FreightCostType,
                            FreightLeadTime: 1, //Belum di null
                            SupplierQLTime: vm.QLTime,
                            StartDate: vm.goods[i].goods.StartDate,
                            QuotationNo: vm.goods[0].goods.QuotationNo
                        },
                        item: {
                            Quantity: vm.goods[i].item.Quantity
                        }
                    }
                    vm.list.push(data);
                      }

                GoodOfferEntryService.InsertDetail(vm.list,
                   function (reply) {
                       var x = 0, k = 0;
                       if (reply.status === 200) {
                           vm.ada = 0;
                           for (vm.up = 0; vm.up < vm.listOtherDoc.length; vm.up++) {
                               if (vm.listOtherDoc[vm.up].ID == undefined) vm.flagList = true;
                           }
                           console.info(vm.up);
                           if (vm.up == (vm.listOtherDoc.length)) {
                               if (vm.flagList == true) {
                                   SimpanUpload();
                               }
                               else {
                                   for (var x = 0; x < vm.listUpload.length; x++) {
                                       if (x != vm.listUpload.length) {
                                           uploadFile(0, x, vm.listUpload[x]);
                                       }
                                   }
                                   if (vm.listUpload.length == 0) window.location.reload();
                               }
                           }
                           if (vm.listOtherDoc.length == 0) {
                               for (var x = 0; x < vm.listUpload.length; x++) {
                                   if (x != vm.listUpload.length) {
                                       uploadFile(0, x, vm.listUpload[x]);
                                   }
                               }
                               if (vm.listUpload.length == 0) window.location.reload();
                           }
                           if (vm.listOtherDoc.length == 0 && vm.listUpload.length == 0) {
                               UIControlService.msg_growl("success", "Berhasil Simpan data");
                               window.location.reload();
                           }
                       }
                       else {
                           UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                           return;
                       }
                   },
               function (err) {
                   UIControlService.msg_growl("error", "Gagal menyimpan data!!");
               }
          );
            }

            vm.SimpanUpload = SimpanUpload;
            function SimpanUpload() {
                for (var i = 0; i < vm.listOtherDoc.length; i++) { //1
                    vm.listOtherDoc[i].TenderStepId = vm.StepID;
                    if (vm.listOtherDoc[i].ID == undefined) {
                        uploadFile(1, i, vm.listOtherDoc[i]);
                    }
                }
            }



        //ambil VendorID
            vm.loadVerifiedVendor = loadVerifiedVendor;
            function loadVerifiedVendor() {
                VerifiedSendService.selectVerifikasi(function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        vm.verified = reply.data;
                        vm.cekTemporary = vm.verified.IsTemporary;
                        vm.VendorID = vm.verified.VendorID;
                    } else {
                        $.growl.error({ message: "Gagal mendapatkan data Perusahaan" });
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    console.info("error:" + JSON.stringify(err));
                    //$.growl.error({ message: "Gagal Akses API >" + err });
                    UIControlService.unloadLoading();
                });
            }

        //get tipe dan max.size file - 2
            function generateFilterStrings(allowedTypes) {
                var filetypes = "";
                for (var i = 0; i < allowedTypes.length; i++) {
                    filetypes += "." + allowedTypes[i].Name + ",";
                }
                return filetypes.substring(0, filetypes.length - 1);
            }

            vm.selectUpload1 = selectUpload1;
            function selectUpload1(data) {
                console.info(data);
            }

            vm.addOtherDoc = addOtherDoc;
            function addOtherDoc() {
                vm.listOtherDoc.push({
                    fileUpload: vm.fileUpload1,
                    Remark: vm.Remark
                });
                vm.fileUpload1 = [];
                vm.Remark = undefined;
            }

    }
})();
