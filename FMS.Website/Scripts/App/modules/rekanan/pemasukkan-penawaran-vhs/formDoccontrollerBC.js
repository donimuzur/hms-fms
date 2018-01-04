(function () {
    'use strict';

    angular.module("app")
            .controller("frmBCCtrl", ctrl);

    ctrl.$inject = ['$uibModal','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PPVHSService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($uibModal, $http, $translate, $translatePartialLoader, $location, SocketService, PPVHSService, UploadFileConfigService,
        UIControlService, UploaderService, item, $uibModalInstance, GlobalConstantService) {

        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.detail = item.item;
        vm.TenderDocType = item.act;
        vm.Limit = item.Limit;
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.keyword = '';
        vm.action = "";
        vm.pathFile;
        vm.TenderStepID = item.TenderStepID;
        vm.Description;
        vm.fileUpload;
        vm.size;
        vm.name;
        vm.type;
        vm.flag;
        vm.selectedForm;
        vm.tglSekarang = UIControlService.getDateNow("");
        vm.flag = false;
        vm.init = init;
        vm.inProcess = item.inProcess;
        function init() {
            loadVendor();
            if (vm.TenderDocType !== 18) {
                loadVendor();
                loadCompanyPerson();
                $translatePartialLoader.addPart('surat-pernyataan');
                $translatePartialLoader.addPart('pemasukkan-penawaran-vhs');
                UIControlService.loadLoading("MESSAGE.LOADING");
                //get tipe dan max.size file - 1
                UploadFileConfigService.getByPageName("PAGE.VENDOR.VHS.OFFERENTRY", function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        vm.name = response.data.name;
                        vm.idUploadConfigs = response.data;
                        console.info(JSON.stringify(vm.idUploadConfigs));
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
                if (vm.isAdd === true) {
                    vm.action = "Tambah";

                }
                else {
                    vm.action = "Ubah ";

                }


            }
            
        }

        vm.selected = selected;
        function selected() {
            console.info("respon1:" + JSON.stringify(vm.selectedDocumentType));
        }

        
        vm.loadVendor = loadVendor;
        function loadVendor() {
            PPVHSService.selectVendor(function (reply) {
                if (reply.status === 200) {
                    vm.vendor = reply.data;
                    if (vm.TenderDocType == 18) {
                        vm.flag = true;
                        loadVendorCommodity(vm.vendor[0].Vendor.VendorID);
                    }
                    else if (vm.TenderDocType == 19 || vm.TenderDocType == 17) {
                        vm.flag = true;
                        loadOffice(1);
                    }
                    else if (vm.TenderDocType == 20) {
                        vm.flag = true;
                        loadEquipmentVehicle(1);
                        loadEquipmentTools(1);
                    }

                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                    return;
                }
              },
              function (err) {
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                UIControlService.unloadLoadingModal();
              }
           );
        }

        vm.loadCompanyPerson = loadCompanyPerson;
        function loadCompanyPerson() {
            PPVHSService.selectCompanyPerson(function (reply) {
                if (reply.status === 200) {
                    vm.CompanyPerson = reply.data;
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                    return;
                }
            },
              function (err) {
                  UIControlService.msg_growl("error", "Gagal Akses Api!!");
                  UIControlService.unloadLoadingModal();
              }
           );
        }

        vm.loadVendorCommodity = loadVendorCommodity;
        function loadVendorCommodity(data) {
            PPVHSService.SelectVendorCommodity({
                VendorID: data
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.comodity = reply.data;
                    loadExperience(vm.comodity.BusinessFieldID);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data pengalaman perusahaan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.loadExperience = loadExperience;
        function loadExperience(data) {
            PPVHSService.selectVendorExperience({
                Offset: (vm.currentPage - 1) * vm.maxSize,
                Limit: vm.maxSize,
                Keyword: vm.keyword,
                FilterType: data,
                column: 1
            }, function (reply) {
                if (reply.status === 200) {
                    vm.listFinishExp = reply.data.List;
                    for (var i = 0; i < vm.listFinishExp.length; i++) {
                        vm.listFinishExp[i].StartDate = UIControlService.getStrDate(vm.listFinishExp[i].StartDate);
                    }
                    vm.totalItems = reply.data.Count;
                    UIControlService.loadLoading('LOADING.VENDOREXPERIENCE.MESSAGE');
                    PPVHSService.selectVendorExperience({
                        Offset: (vm.currentPage - 1) * vm.maxSize,
                        Limit: vm.maxSize,
                        Keyword: vm.keyword,
                        column: 2
                    }, function (reply) {
                        //console.info("current?:"+JSON.stringify(reply));
                        if (reply.status === 200) {
                            vm.listCurrentExp = reply.data.List;
                            for (var i = 0; i < vm.listCurrentExp.length; i++) {
                                vm.listCurrentExp[i].StartDate = UIControlService.getStrDate(vm.listCurrentExp[i].StartDate);
                            }
                            vm.totalItems = reply.data.Count;
                            UIControlService.unloadLoading();
                        } else {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
                    });
                    UIControlService.unloadLoading();
                } else {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
                }
            }, function (err) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
            });

            
        }

        vm.listOffice = [];
        function loadOffice(current) {
            var offset = (current * vm.maxSize) - vm.maxSize;
            if (vm.TenderDocType == 19) vm.category = "BUILDING_CATEGORY_WORKSHOP";
            else if (vm.TenderDocType == 17) vm.category = "BUILDING_CATEGORY_WAREHOUSE";
            
            PPVHSService.SelectBuildingTender({Keyword: vm.category, Ofsset: offset, Limit: vm.maxSize },
            function (reply) {
                vm.dataBuilding = reply.data.List;
                if (vm.dataBuilding.length == 0) {
                    console.info(vm.category);
                    PPVHSService.selectBuilding({ Keyword: vm.category, Ofsset: offset, Limit: vm.maxSize },
                    function (reply) {
                        vm.dataBuilding1 = reply.data.List;
                        console.info(vm.dataBuilding1);
                //    for (var i = 0; i < vm.dataBuilding1.length; i++) {
                //    if (vm.TenderDocType == 19) {
                //        if (vm.dataBuilding1[i].category.Name === "BUILDING_CATEGORY_WORKSHOP") {
                //            vm.listOffice.push(vm.dataBuilding1[i]);
                //            PPVHSService.InsertBuildingTender({
                //                BuildingAddress: vm.dataBuilding1[i].BuildingAddress,
                //                BuildingArea: vm.dataBuilding1[i].BuildingArea,
                //                Category: vm.dataBuilding1[i].Category,
                //                OwnershipStatus: vm.dataBuilding1[i].OwnershipStatus
                //            },
                //                    function (reply) {
                //                        UIControlService.unloadLoadingModal();
                //                        if (reply.status === 200) {
                //                        }
                //                        else {
                //                            UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                //                            return;
                //                        }
                //                    },
                //                    function (err) {
                //                        UIControlService.msg_growl("error", "Gagal Akses Api!!");
                //                        UIControlService.unloadLoadingModal();
                //                    }
                //                );
                //        }
                //    }
                //    else if (vm.TenderDocType == 17) {
                //        if (vm.dataBuilding1[i].category.Name === "BUILDING_CATEGORY_WAREHOUSE") {
                //            vm.listOffice.push(vm.dataBuilding1[i]);
                //            PPVHSService.InsertBuildingTender({
                //                BuildingAddress: vm.dataBuilding1[i].BuildingAddress,
                //                BuildingArea: vm.dataBuilding1[i].BuildingArea,
                //                Category: vm.dataBuilding1[i].Category,
                //                OwnershipStatus: vm.dataBuilding1[i].OwnershipStatus
                //            },
                //                    function (reply) {
                //                        UIControlService.unloadLoadingModal();
                //                        if (reply.status === 200) {
                //                        }
                //                        else {
                //                            UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                //                            return;
                //                        }
                //                    },
                //                    function (err) {
                //                        UIControlService.msg_growl("error", "Gagal Akses Api!!");
                //                        UIControlService.unloadLoadingModal();
                //                    }
                //                );
                //        }
                //    }
                //}
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
                }
                else if (vm.dataBuilding.length != 0) { vm.listOffice = reply.data.List;}


                
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.editBuilding = editBuilding;
        function editBuilding(flag) {
            var data = {
                TenderStepID: vm.StepID,
                isForm: true,
                type: 1
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/pemasukkan-penawaran-vhs/formBuilding.html',
                controller: "FormBuildingCtrl",
                controllerAs: "FormBuildingCtrl",
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                $uibModalInstance.close();
            });
        }


        vm.listVehicle = [];
        function loadEquipmentVehicle(current) {
            var offset = (current * vm.maxSize) - vm.maxSize;
            PPVHSService.selectVehicle({ Ofsset: offset, Limit: vm.maxSize },
            function (reply) {
                vm.listVehicle = reply.data.List;
                for (var i = 0; i < vm.listVehicle.length; i++) {
                    vm.listVehicle[i].MfgDate = UIControlService.getStrDate(vm.listVehicle[i].MfgDate);
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.listEquipmentTools = [];
        function loadEquipmentTools(current) {
            //console.info("mlebu");
            var offset = (current * vm.maxSize) - vm.maxSize;
            PPVHSService.selectEquipment({ Ofsset: offset, Limit: vm.maxSize },
            function (reply) {
                // console.info("Equip>>>"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                vm.listEquipmentTools = reply.data.List;
                for (var i = 0; i < vm.listEquipmentTools.length; i++) {
                    vm.listEquipmentTools[i].MfgDate = UIControlService.getStrDate(vm.listEquipmentTools[i].MfgDate);
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        //get tipe dan max.size file - 2
        function generateFilterStrings(allowedTypes) {
            console.info(allowedTypes);
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.selectUpload = selectUpload;
        //vm.fileUpload;
        function selectUpload() {
            console.info((vm.fileUpload));
        }

        /*start upload */
        vm.uploadFile = uploadFile;
        function uploadFile() {

            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, "");
            }
        }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }
            return true;
        }

        vm.upload = upload;
        function upload(file, config, filters, callback) {
            
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
            

            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFileVHSOfferEntry( vm.tglSekarang, file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    console.info("response:" + JSON.stringify(response));
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;
                        vm.size = s;
                        vm.list = [];
                        var data = {
                            ID: vm.detail.ID,
                            IsPublish: vm.detail.IsPublish,
                            TenderDocTypeID: vm.detail.TenderDocTypeID,
                            vhs:{
                                TenderStepID: vm.detail.vhs.TenderStepID
                            },
                            DocumentUrl: vm.pathFile,
                            Filename: vm.name,
                            FileSize: vm.size
                        }
                        vm.list.push(data);
                        PPVHSService.InsertOpen(vm.list,
                                function (reply) {
                                    console.info("reply" + JSON.stringify(reply))
                                    UIControlService.unloadLoadingModal();
                                    if (reply.status === 200) {
                                        UIControlService.msg_growl("success", "Berhasil Upload File !!");
                                        $uibModalInstance.close();
                                    }
                                    else {
                                        UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                        return;
                                    }
                                },
                                function (err) {
                                    UIControlService.msg_growl("error", "Gagal Akses Api!!");
                                    UIControlService.unloadLoadingModal();
                                }
                            );
                         
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

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

        vm.agree = agree;
        function agree(flag) {
            var tender = {
                IsAgree: flag, 
                TenderDocTypeID: vm.TenderDocType,
                TenderStepID: vm.TenderStepID
            }
            PPVHSService.InsertDetail(tender, function (reply) {
                if (reply.status === 200) {
                    $uibModalInstance.close();
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                    return;
                }
            },
              function (err) {
                  UIControlService.msg_growl("error", "Gagal Akses Api!!");
                  UIControlService.unloadLoadingModal();
              }
           );
        }
    }
})();