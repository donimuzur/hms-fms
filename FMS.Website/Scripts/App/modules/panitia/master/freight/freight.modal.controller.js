(function () {
    'use strict';

    angular.module("app").controller("FormFreightCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'FreightService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance', 'ProvinsiService'];

    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, FreightService,
		RoleService, UIControlService, item, $uibModalInstance, ProvinsiService) {
        var vm = this;
        vm.isAdd = item.act;
        vm.data;
        vm.action = "";
        vm.menambah = false;
        vm.menghapus = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('master-freight');
            if (vm.isAdd === true) {
                vm.action = "Tambah ";
            }
            else {
                vm.action = "Ubah ";
                vm.data = item.item;

                vm.freightField = {
                    FreightCostID: vm.data.FreightCostID,
                    DeliveryState: vm.data.DeliveryState,
                    DeliveryTransportID: vm.data.DeliveryTransportID,
                    DeliveryTimeAir: vm.data.DeliveryTimeAir,
                    FreightCostAir: vm.data.FreightCostAir,
                    DeliveryTimeLand: vm.data.DeliveryTimeLand,
                    FreightCostLand: vm.data.FreightCostLand,
                    DeliveryTimeSea: vm.data.DeliveryTimeSea,
                    FreightCostSea: vm.data.FreightCostSea,
                    FreightCostTypeID: vm.data.FreightCostTypeID
                };
                loadTable();
            }
            getTipePengiriman();
            getTipeWilayah();
            getIncoTerms();
        }

        vm.load = [];
        vm.newID;
        vm.InsertDetail = InsertDetail;
        function InsertDetail() {
            FreightService.all(function (reply) {
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.load = data;
                    var a = vm.load.length - 1;
                    vm.newID = reply.data[a].FreightCostID;

                    for (var i = 0; i < vm.inco.length; i++) {
                        UpdateDetail(vm.inco[i].ID);
                    }

                    vm.UpdateDetail = UpdateDetail;
                    function UpdateDetail(data) {
                        FreightService.insertDetail({
                            FreightCostid: vm.newID,
                            IncoTermsid: data
                        }, function (reply) {
                            UIControlService.unloadLoading();
                            if (reply.status === 200) {
                                //UIControlService.msg_growl("success", "Berhasil Simpan Data Freight Cost Detail");
                                $uibModalInstance.close();
                            }
                            else {
                                UIControlService.msg_growl("error", "Gagal menyimpan data");
                                return;
                            }
                        }, function (err) {
                            UIControlService.msg_growl("error", "Gagal menyimpan data");
                            UIControlService.unloadLoading();
                        });
                    }
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data");
                    return;
                }
            })
        }

        vm.IncoID = {};
        vm.loadTable = loadTable;
        function loadTable() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            FreightService.selectDetail({
                Status: vm.data.FreightCostID
            }, function (reply) {
                console.info("datane:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.inco = reply.data;
                    for (var i = 0; i < vm.inco.length; i++) {
                        vm.fid = reply.data[i].FreightCostId;
                        vm.iid = reply.data[i].IncoTermsId;
                        vm.active = reply.data[i].IsActive;
                    }
                } else {
                    UIControlService.msg_growl("error", "Gagal Mendapatkan Data Freight");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "Gagal Mendapatkan Data Freight");
                UIControlService.unloadLoading();
            });
        }

        function remove(list, obj) {
            var index = list.indexOf(obj);

            if (index >= 0) {
                list.splice(index, 1);
            } else {
                UIControlService.msg_growl('error', "ERRORS.OBJECT_NOT_FOUND");
            }
            return list;
        }

        vm.getTipePengiriman = getTipePengiriman;
        vm.selectedTipeKirim;
        vm.listTipePengiriman = [];
        function getTipePengiriman() {
            FreightService.getTypeDelivery(
               function (response) {
                   if (response.status === 200) {
                       vm.listTipePengiriman = response.data;
                       if (vm.isAdd === false) {
                           for (var i = 0; i < vm.listTipePengiriman.length; i++) {
                               if (vm.freightField.DeliveryTransportID === vm.listTipePengiriman[i].RefID) {
                                   vm.selectedTipeKirim = vm.listTipePengiriman[i];
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list tipe pengiriman");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal mendapatkan list tipe pengiriman");
                   return;
               }
            );
        }

        vm.getIncoTerms = getIncoTerms;
        vm.selectedIncoTerms;
        vm.listIncoTerms = [];
        function getIncoTerms() {
            FreightService.getIncoTerms(
               function (response) {
                   if (response.status === 200) {
                       vm.listIncoTerms = response.data;
                       if (vm.isAdd === false) {
                           for (var i = 0; i < vm.listIncoTerms.length; i++) {
                               if (vm.freightField.IncoTerms === vm.listIncoTerms[i].ID) {
                                   vm.selectedIncoTerms = vm.listIncoTerms[i];
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list tipe pengiriman");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal mendapatkan list tipe pengiriman");
                   return;
               }
            );
        }

        vm.inco = [];
        vm.incoID = [];
        vm.addIncoTerms = addIncoTerms;
        vm.cekID;
        vm.flag = 0;
        function addIncoTerms() {
            var IncoTerms = { ID: vm.selectedIncoTerms.ID, Name: vm.selectedIncoTerms.Name, Code: vm.selectedIncoTerms.Code };
            for (var i = 0; i < vm.inco.length; i++) {
                if (vm.inco[i].ID === IncoTerms.ID) {
                    UIControlService.msg_growl("warning", "Inco Terms sudah dipilih!");
                    vm.flag = 1;
                }
                else if (vm.inco[i].FreightCostId === vm.freightField.FreightCostID && vm.inco[i].IncoTermsId === IncoTerms.ID && vm.active === true) {
                    UIControlService.msg_growl("warning", "Inco Terms sudah dipilih!");
                    vm.flag = 1;
                }
                
            }
            if (vm.selectedIncoTerms === undefined) {
                UIControlService.msg_growl("warning", "Inco Terms belum dipilih!");
                vm.flag = 1;
            }
            
            if (vm.flag === 0) {
                vm.inco.push(IncoTerms);
                vm.menambah = true;
            }
            vm.flag = 0;
        }

        vm.removeID = [];
        vm.removeIncoTerms = removeIncoTerms;
        function removeIncoTerms(IncoTerms) {
            if (vm.isAdd === true) {
                vm.inco = remove(vm.inco, IncoTerms);
            }
            else if (vm.isAdd === false && !IncoTerms.IsActive) {
                vm.inco = remove(vm.inco, IncoTerms);
            }
            else if (vm.isAdd === false && IncoTerms.IsActive) {
                vm.inco = remove(vm.inco, IncoTerms);
                vm.removeID.push(IncoTerms);
                vm.menghapus = true;
            }
        }

        vm.getTipeWilayah = getTipeWilayah;
        vm.selectedTipeWilayah;
        vm.listTipeWilayah = [];
        function getTipeWilayah() {
            FreightService.getTypeRegion(
               function (response) {
                   if (response.status === 200) {
                       vm.listTipeWilayah = response.data;
                       if (vm.isAdd === false) {
                           for (var i = 0; i < vm.listTipeWilayah.length; i++) {
                               if (vm.freightField.FreightCostTypeID === vm.listTipeWilayah[i].RefID) {
                                   vm.selectedTipeWilayah = vm.listTipeWilayah[i];
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list tipe wilayah");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal mendapatkan list tipe wilayah");
                   return;
               });
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

        vm.simpan = simpan;
        function simpan() {
            if (vm.freightField.DeliveryState === undefined) {
                UIControlService.msg_growl("warning", "Kota Tujuan Pengiriman belum dipilih!!");
                return;
            }
            /*if (vm.freightField.DeliveryTime === undefined) {
                UIControlService.msg_growl("warning", "Waktu pengiriman belum diisi!");
                return;
            }*/
            /*if (vm.freightField.FreightCost1 === 0) {
                UIControlService.msg_growl("warning", "Biaya pengiriman belum diisi!");
                return;
            }*/
            if (vm.selectedTipeWilayah === undefined) {
                UIControlService.msg_growl("warning", "Tipe Wilayah belum dipilih!");
                return;
            }
            /*if (vm.selectedTipeKirim === undefined) {
                UIControlService.msg_growl("warning", "Tipe Pengiriman belum dipilih!");
                return;
            }*/
            vm.selectedTipeKirim = vm.listTipePengiriman[0]; //ByPass
            if (vm.inco.length <= 0) {
                UIControlService.msg_growl("warning", "Inco Terms belum dipilih!");
                return;
            }
            vm.freightField.DeliveryTransportID = vm.selectedTipeKirim.RefID;
            vm.freightField.FreightCostTypeID = vm.selectedTipeWilayah.RefID;
            vm.prosesSimpan();
        }

        //proses simpan
        vm.prosesSimpan = prosesSimpan;
        function prosesSimpan(IncoTerms) {
            if (vm.isAdd === true) {
                FreightService.insert(vm.freightField, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Data Freight Cost");
                        $uibModalInstance.close();
                        InsertDetail();

                    }
                    else {
                        UIControlService.msg_growl("error", "Gagal menyimpan data");
                        return;
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "Gagal menyimpan data");
                    UIControlService.unloadLoading();
                });
            }
            else {
                console.info(JSON.stringify(vm.freightField));
                FreightService.update(vm.freightField, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Data Freight Cost");
                        $uibModalInstance.close();
                        if (vm.menghapus === false && vm.menambah === true) {
                            for (var i = 0; i < vm.inco.length; i++) {
                                UpdateDetail2(vm.inco[i].ID, vm.inco[i].IsActive);
                            }
                            vm.UpdateDetail2 = UpdateDetail2;
                            function UpdateDetail2(data, data2) {
                                for (var i = 0; i < data.length; i++) {
                                    if (data[i].FreightCostId === vm.fid && data[i].IncoTermsId === vm.iid) {
                                        vm.newinco = data[i].IncoTermsId === vm.iid;
                                        vm.newinco = remove(vm.newinco, data);
                                        console.info("newinco" + vm.newinco);
                                    }
                                }
                                if (!data2) {
                                    FreightService.insertDetail({
                                        FreightCostid: vm.freightField.FreightCostID,
                                        IncoTermsid: data
                                    }, function (reply) {
                                        UIControlService.unloadLoading();
                                        if (reply.status === 200) {
                                            //UIControlService.msg_growl("success", "Berhasil Simpan Data Freight Cost Detail");
                                            $uibModalInstance.close();
                                        }
                                        else {
                                            UIControlService.msg_growl("error", "Gagal menyimpan data");
                                            return;
                                        }
                                    }, function (err) {
                                        UIControlService.msg_growl("error", "Gagal menyimpan data");
                                        UIControlService.unloadLoading();
                                    });
                                }
                            }

                        }
                        else if (vm.menambah === true && vm.menghapus === true) {
                            for (var i = 0; i < vm.inco.length; i++) {
                                UpdateDetail2(vm.inco[i].ID, vm.inco[i].IsActive);
                            }
                            vm.UpdateDetail2 = UpdateDetail2;
                            function UpdateDetail2(data, data2) {
                                //console.info(data2);
                                if (!data2) {
                                    FreightService.insertDetail({
                                        FreightCostid: vm.freightField.FreightCostID,
                                        IncoTermsid: data
                                    }
                                    , function (reply) {
                                        UIControlService.unloadLoading();
                                        if (reply.status === 200) {
                                            //UIControlService.msg_growl("success", "Berhasil Simpan Data Freight Cost Detail");
                                            for (var i = 0; i < vm.removeID.length; i++) {
                                                removeData(vm.removeID[i].ID);
                                                //console.info(vm.removeID[i].ID);
                                            }
                                            vm.removeData = removeData;
                                            function removeData(data) {
                                                if (vm.isAdd === false) {
                                                    if (vm.removeID.length <= 0) {
                                                        $uibModalInstance.close();
                                                    }
                                                    else {
                                                        FreightService.removeDetail({
                                                            ID: data
                                                        }, function (reply) {
                                                            UIControlService.unloadLoading();
                                                            if (reply.status === 200) {
                                                                //UIControlService.msg_growl("success", "Berhasil Simpan Data Freight Cost Detail");
                                                                $uibModalInstance.close();
                                                            }
                                                            else {
                                                                UIControlService.msg_growl("error", "Gagal menyimpan data");
                                                                return;
                                                            }
                                                        }, function (err) {
                                                            UIControlService.msg_growl("error", "Gagal menyimpan data");
                                                            UIControlService.unloadLoading();
                                                        });
                                                    }
                                                }
                                            }
                                            $uibModalInstance.close();
                                        }
                                        else {
                                            UIControlService.msg_growl("error", "Gagal menyimpan data");
                                            return;
                                        }
                                    }, function (err) {
                                        UIControlService.msg_growl("error", "Gagal menyimpan data");
                                        UIControlService.unloadLoading();
                                    });
                                }
                            }
                        }
                        else if (vm.menghapus === true && vm.menambah === false) {
                            console.info(vm.removeID);
                            for (var i = 0; i < vm.removeID.length; i++) {
                                removeData(vm.removeID[i].ID);
                                //console.info(vm.removeID[i].ID);
                            }
                            vm.removeData = removeData;
                            function removeData(data) {
                                if (vm.isAdd === false) {
                                    if (vm.removeID.length <= 0) {
                                        $uibModalInstance.close();
                                    }
                                    else {
                                        FreightService.removeDetail({
                                            ID: data
                                        }, function (reply) {
                                            UIControlService.unloadLoading();
                                            if (reply.status === 200) {
                                                //UIControlService.msg_growl("success", "Berhasil Simpan Data Freight Cost Detail");
                                                $uibModalInstance.close();
                                            }
                                            else {
                                                UIControlService.msg_growl("error", "Gagal menyimpan data");
                                                return;
                                            }
                                        }, function (err) {
                                            UIControlService.msg_growl("error", "Gagal menyimpan data");
                                            UIControlService.unloadLoading();
                                        });
                                    }
                                }
                            }
                        }

                    }
                    else {
                        UIControlService.msg_growl("error", "Gagal menyimpan data");
                        return;
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "Gagal menyimpan data");
                    if (vm.freightField.FreightCostAir > 100 || vm.freightField.FreightCostAir === undefined) {
                        UIControlService.msg_growl("warning", "Biaya pengiriman udara diisi dalam bentuk presentase dan tidak boleh melebihi 100%");
                    }
                    if (vm.freightField.FreightCostLand > 100 || vm.freightField.FreightCostLand === undefined) {
                        UIControlService.msg_growl("warning", "Biaya pengiriman darat diisi dalam bentuk presentase dan tidak boleh melebihi 100%");
                    }
                    if (vm.freightField.FreightCostSea > 100 || vm.freightField.FreightCostSea === undefined) {
                        UIControlService.msg_growl("warning", "Biaya pengiriman laut diisi dalam bentuk presentase dan tidak boleh melebihi 100%");
                    }
                    UIControlService.unloadLoading();
                });
            }
        }
    }
})();