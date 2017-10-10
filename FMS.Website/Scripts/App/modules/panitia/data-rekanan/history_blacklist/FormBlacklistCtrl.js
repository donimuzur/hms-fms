(function () {
    'use strict';

    angular.module("app").controller("FormBlacklistCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'BlacklistService', 'UploadFileConfigService', 'RoleService', 'UIControlService', 'UploaderService', 'item', '$uibModalInstance', '$state', 'GlobalConstantService', '$uibModal'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, BlacklistService,UploadFileConfigService,
        RoleService, UIControlService, UploaderService,item, $uibModalInstance, $state, GlobalConstantService, $uibModal) {
        
        var vm = this;
        var page_id = 141;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.Area;
        vm.flag = item.act;
        vm.data = item.item;
        vm.BlacklistID = item.item.BlacklistID;
        vm.VendorID = item.item.VendorID;
        vm.VendorName = item.item.Vendor.VendorName;
        vm.Description = "";
        vm.action = "";
        vm.DetailPerson = "";
        vm.actionblacklst = "";
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;

        vm.DetailPersonform;
        
        vm.isCalendarOpened = [false, false, false, false];

        vm.pathFile;
        vm.fileUpload="";
        vm.size;
        vm.name;
        vm.type;
        vm.flag;
        vm.employee = [];
        
        vm.vendorstock = [];
        vm.listperson = [];
        vm.selectedMasaBlacklist ="";
        vm.blacklistdate = {};
        vm.cek1 = false;
        vm.cekWhite = false;
        vm.BlacklistType = item.type;
        function init() {
            $translatePartialLoader.addPart("blacklist-data");
            if (vm.flag == true) {
                //get tipe dan max.size file - 1
                UploadFileConfigService.getByPageName("PAGE.ADMIN.BLACKLIST", function (response) {
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
                getByID(vm.VendorID);
            }
            else {
                if (vm.BlacklistType == 'BLACKLIST_TYPE_YES') {

                    vm.DocUrl = item.item.DocUrl;
                    vm.Description = item.item.BlacklistDescription;
                    getByBlacklistCompPers(vm.BlacklistID);
                }
                else {
                    vm.cekWhite = true;
                    vm.DocUrl = item.item.DorUrlWhiteList;
                    vm.ReferenceNo = item.item.ReferenceNo;
                    getByBlacklistCompPers(vm.BlacklistID);
                }
            }
        }

        vm.getByBlacklistCompPers = getByBlacklistCompPers;
        function getByBlacklistCompPers(id) {
            BlacklistService.selectdetailCompPers({
                Status: id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.listemployee = reply.data;
                    vm.employee = vm.listemployee.CompPers;
                    vm.vendorstock = vm.listemployee.VendorStock;
                    vm.listperson = vm.listemployee.DetailPerson;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('DETAIL.MESSAGE.ERR_SELECT'));
                    UIControlService.unloadLoading();
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
                UIControlService.unloadLoading();
                return;
            });
        }

        vm.getByID = getByID;
        function getByID(id) {
            BlacklistService.SelectEmployeeVendor({
                Status: id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.employee = reply.data.List;
                    //for (var i = 0; i < vm.employee.length; i++) {
                    //    vm.employee[i]["Description"] = "";
                    //    vm.employee[i]["check"] = false;
                    //}
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('DETAIL.MESSAGE.ERR_SELECT'));
                    UIControlService.unloadLoading();
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
                UIControlService.unloadLoading();
                return;
            });

            BlacklistService.SelectVendorStock({
                Status: id
            }, function (reply) {
                //console.info("data detail:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.vendorstock = reply.data.List;
                    //for (var i = 0; i < vm.employee.length; i++) {
                    //    vm.vendorstock[i]["Description"] = "";
                    //    vm.vendorstock[i]["check"] = false;
                    //}
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('DETAIL.MESSAGE.ERR_SELECT'));
                    UIControlService.unloadLoading();
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
                UIControlService.unloadLoading();
                return;
            });



        }

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString() { // TIMEZONE (-)

            if (vm.blacklistdate.StartDate) {
                vm.blacklistdate.StartDate = UIControlService.getStrDate(vm.blacklistdate.StartDate);
            }
            if (vm.blacklistdate.EndDate) {
                vm.blacklistdate.EndDate = UIControlService.getStrDate(vm.blacklistdate.EndDate);
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate() {

            if (vm.blacklistdate.StartDate) {
                vm.blacklistdate.StartDate = new Date(Date.parse(vm.blacklistdate.StartDate));
            }
            if (vm.blacklistdate.EndDate) {
                vm.blacklist.EndDate = new Date(Date.parse(vm.blacklistdate.EndDate));
            }
        }

        //get tipe dan max.size file - 2
        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.selectUpload = selectUpload;
        //vm.fileUpload;
        function selectUpload() {
            console.info(JSON.stringify(vm.fileUpload));
        }

        /*start upload */
        vm.uploadFile = uploadFile;
        function uploadFile(data) {
            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(data, vm.fileUpload, vm.idFileSize, vm.idFileTypes, "");
            }
        }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);

            if (selectedFileType === "vnd.ms-excel") {
                selectedFileType = "xls";
            }
            else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                selectedFileType = "xlsx";
            } else if (selectedFileType === "application/msword") {
                selectedFileType = "doc";
            }
            else if (selectedFileType === "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || selectedFileType == "vnd.openxmlformats-officedocument.wordprocessingml.document") {
                selectedFileType = "docx";
            }
            else {
                selectedFileType = selectedFileType;
            }
            vm.type = selectedFileType;

            //jika excel
            if (selectedFileType === "vnd.ms-excel")
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

        vm.upload = upload;
        function upload(data, file, config, filters, callback) {

            var size = config.Size;
            var unit = config.SizeUnitName;

            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
                vm.flagSize = 0;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
                vm.flagSize = 1;
            }


            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFileBlacklist(file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;
                        if (vm.flagSize == 0) {

                            vm.size = Math.floor(s)
                        }

                        if (vm.flagSize == 1) {
                            vm.size = Math.floor(s / (1024));
                        }
                        openmodal(vm.VendorID, vm.Description, vm.pathFile, vm.listperson);

                        //msg(vm.blacklist,vm.VendorID, vm.selectedMasaBlacklist.RefID, vm.Description, vm.pathFile, vm.blacklistdate.StartDate, vm.blacklistdate.EndDate, vm.listperson);
                        

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

        vm.addCheck = addCheck;
        var cek = 0;
        var cek1 = 0;
        function addCheck(act, data, checklist) {
            console.info(act);
            if (act == 0) {
                for (var i = 0; i < data.length; i++) {
                    data[i]["check"] = checklist;
                    for (var j = 0; j < vm.vendorstock.length; j++) {
                        if (vm.vendorstock[j].OwnerID == data[i].NoID && vm.vendorstock[j].OwnerName.toLowerCase() == data[i].PersonName.toLowerCase()) {
                            vm.vendorstock[j]["check"] = checklist;
                            if (vm.vendorstock.length === 1) vm.cek2 = checklist;

                        }
                    }
                }
                //if (cek == 0) {
                //    cek = 1;
                    
                //}
                //else if (cek == 1) {
                //    cek = 0;
                //    for (var i = 0; i < data.length; i++) {
                //        data[i]["check"] = false;
                //        for (var j = 0; j< vm.vendorstock.length; j++) {
                //            if (vm.vendorstock[j].OwnerID == data[i].NoID) {
                //                vm.vendorstock[j]["check"] = false;
                //                if (vm.vendorstock.length === 1) vm.cek2 = false;
                //            }
                //        }
                //    }
                //}
            }
            else if (act == 1) {
                for (var i = 0; i < data.length; i++) {
                    data[i]["check"] = checklist;
                    for (var j = 0; j < vm.employee.length; j++) {
                        if (vm.employee[j].NoID == data[i].OwnerID && vm.employee[j].PersonName.toLowerCase() == data[i].OwnerName.toLowerCase()) {
                            vm.employee[j]["check"] = checklist;
                            if (vm.employee.length === 1) vm.cek1 = checklist;
                        }
                    }
                }
                //if (cek1 == 0) {
                //    cek1 = 1;
                //    for (var i = 0; i < data.length; i++) {
                //        data[i]["check"] = true;
                //        for (var j = 0; j< vm.employee.length; j++) {
                //            if (vm.employee[j].NoID == data[i].OwnerID) {
                //                vm.employee[j]["check"] = true;
                //                if (vm.employee.length === 1) vm.cek1 = true;
                //            }
                //        }
                //    }
                //}
                //else if (cek1 == 1) {
                //    cek1 = 0;
                //    for (var i = 0; i < data.length; i++) {
                //        data[i]["check"] = false;
                //        for (var j = 0; j< vm.employee.length; j++) {
                //            if (vm.employee[j].NoID == data[i].OwnerID) {
                //                vm.employee[j]["check"] = false;
                //                if (vm.employee.length === 1) vm.cek1 = false;
                //            }
                //        }
                //    }
                //}
            }
        }

        vm.addCheckbox = addCheckbox;
        var cek = 0;
        function addCheckbox(flag, data, dataall) {
            
            if (flag == 1) {
                for (var i = 0; i < vm.employee.length; i++) {
                    if (vm.employee[i].NoID == dataall.NoID && vm.employee[i].PersonName.toLowerCase() == dataall.PersonName.toLowerCase()) {
                        vm.employee[i]["check"] = data;
                        if (vm.employee.length === 1) vm.cek1 = data;
                    }
                }
                for (var i = 0; i < vm.vendorstock.length; i++) {
                    if (vm.vendorstock[i].OwnerID == dataall.NoID && vm.vendorstock[i].OwnerName.toLowerCase() == dataall.PersonName.toLowerCase()) {
                        vm.vendorstock[i]["check"] = data;
                        if (vm.vendorstock.length === 1) vm.cek2 = data;
                    }
                }
                if (vm.employee.length === 1) {
                    if (data == false) vm.cek1 = false;
                    else vm.cek1 = true;
                }
                else {
                    var ii =0;
                    for (var i = 0; i < vm.employee.length; i++) {
                        if (vm.employee[i]["check"] == true) ii += 1;
                    }
                    if (ii === vm.employee.length) {
                        vm.cek1 = true;
                    }
                    else {
                        vm.cek1 = false;
                    }
                }
            }
            else if (flag == 2) {
                for (var i = 0; i < vm.employee.length; i++) {
                    if (vm.employee[i].NoID == dataall.OwnerID && vm.employee[i].PersonName.toLowerCase() == dataall.OwnerName.toLowerCase()) {
                        vm.employee[i]["check"] = data;
                        if (vm.employee.length === 1) vm.cek1 = data;
                    }
                }
                for (var i = 0; i < vm.vendorstock.length; i++) {
                    if (vm.vendorstock[i].OwnerID == dataall.OwnerID && vm.vendorstock[i].OwnerName.toLowerCase() == dataall.OwnerName.toLowerCase()) {
                        vm.vendorstock[i]["check"] = data;
                        if (vm.vendorstock.length === 1) vm.cek2 = data;
                    }
                }
                if (vm.vendorstock.length === 1) {
                    if (data == false) vm.cek2 = false;
                    else vm.cek2 = true;
                }
                else {
                    var ii = 0;
                    for (var i = 0; i < vm.vendorstock.length; i++) {
                        if (vm.vendorstock[i]["check"] == true) ii += 1;
                    }
                    if (ii === vm.vendorstock.length) {
                        vm.cek2 = true;
                    }
                    else {
                        vm.cek2 = false;
                    }
                }
            }
            

        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        vm.batalkan = batalkan;
        function batalkan() {
            UIControlService.loadLoading("Silahkan Tunggu");
            BlacklistService.editBlacklist({
                BlacklistID: item.item.BlacklistID,
                BlacklistTypeID: item.item.BlacklistTypeID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    //var msg = "";
                    //if (active === false) msg = " NonAktifkan ";
                    //if (active === true) msg = "Aktifkan ";
                    UIControlService.msg_growl("success", "Data Berhasil di Blacklist");
                    $uibModalInstance.close();
                    vm.jLoad(1);
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
                    return;
                }
            }, function (err) {

                UIControlService.msg_growl("error", "Gagal Akses API ");
                UIControlService.unloadLoading();
            });

        }

        vm.msg = msg;
        function msg(data, VendorID, MasaBlacklistID, BlacklistDescription, DocUrl, StartDate, EndDate, DetailPerson) {
            bootbox.confirm('<h3 class="afta-font">Yakin akan blacklist ?</h3>', function (res) {
               if (res) {
                    BlacklistService.InsertBlacklist({
                        VendorID: VendorID,
                        MasaBlacklistID: MasaBlacklistID,
                        BlacklistDescription: BlacklistDescription,
                        DocUrl: DocUrl,
                        BlacklistTypeID: 2060,
                        StartDateBlacklist: StartDate,
                        EndDateBlacklist: EndDate
                    },
                              function (reply) {
                                  UIControlService.unloadLoadingModal();
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


                    for (var i = 0; i < data.length; i++) {
                        if (i == ((data.length) - 1) && data.length != 0 ) {
                            BlacklistService.insertDetail({
                                VendorID: VendorID,
                                DetailPerson: DetailPerson
                            },
                                function (reply) {
                                    UIControlService.unloadLoadingModal();
                                    if (reply.status === 200) {
                                        UIControlService.msg_growl("success", "Berhasil Simpan Data Blacklist !!");
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
                        for (var y = 0; y < data[i].length; y++) {
                            if (i == 0 && data[i][y].check == true) {
                                BlacklistService.insert({
                                    VendorID: VendorID,
                                    VendorStockID: null,
                                    EmployeeVendorID: data[i][y].EmployeeVendorID,
                                    Description: data[i][y].Description
                                },
                                      function (reply) {
                                          UIControlService.unloadLoadingModal();
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
                            else if (i == 1 && data[i][y].check == true) {
                                BlacklistService.insert({
                                    VendorID: VendorID,
                                    VendorStockID: data[i][y].StockID,
                                    EmployeeVendorID: null,
                                    Description: data[i][y].Description
                                },
                                      function (reply) {
                                          UIControlService.unloadLoadingModal();
                                          if (reply.status === 200) {
                                              UIControlService.msg_growl("success", "Berhasil Simpan Data Blacklist !!");
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
                        
                    }
                }
                else {
                    console.info("sorry");
                }
            });
        }

        vm.addOpsi = addOpsi;
        function addOpsi() {
            if (vm.DetailPerson == "") {
                UIControlService.msg_growl("error", "MESSAGE.API")
                return;
            }
            var data = {
                KTPNumber: vm.NoKTP,
                PersonName: vm.DetailPerson,
                BirthDate: vm.BirthDate
            }
            vm.listperson.push(data);
            vm.DetailPerson = "";
            vm.NoKTP = "";
            vm.BirthDate = "";
        }

        vm.deleteRow = deleteRow;
        function deleteRow(index) {
            var idx = index - 1;
            var _length = vm.listperson.length; // panjangSemula
            vm.listperson.splice(idx, 1);
        };


        vm.openmodal = openmodal;
        function openmodal(VendorID, BlacklistDescription, DocUrl, DetailPerson) {
            vm.listStock = [];
            vm.listCompPers = [];
            vm.listDetailPers = [];
            for (var i = 0; i < vm.vendorstock.length; i++) {
                if (vm.vendorstock[i].check !== undefined) {
                    if (vm.vendorstock[i].check === true) {
                        var data = {
                            StockID: vm.vendorstock[i].StockID,
                            Description: vm.vendorstock[i].Description
                        }
                        vm.listStock.push(data);
                    }
                }
            }
            for (var i = 0; i < vm.employee.length; i++) {
                if (vm.employee[i].check !== undefined) {
                    if (vm.employee[i].check === true) {
                        var data = {
                            ID: vm.employee[i].ID,
                            Description: vm.employee[i].Description
                        }
                        vm.listCompPers.push(data);
                    }
                }
            }
            for (var i = 0; i < DetailPerson.length; i++) {
                var pers = {
                    BirthDate:UIControlService.getStrDate(DetailPerson[i].BirthDate),
                    PersonName:DetailPerson[i].PersonName,
                    KTPNumber:DetailPerson[i].KTPNumber 
                }
                vm.listDetailPers.push(pers);
            }
            var data = {
                act: true,
                vendorstock: vm.listStock,
                employee: vm.listCompPers,
                VendorID: VendorID, 
                BlacklistDescription: vm.Description,
                DocUrl: DocUrl,
                DetailPerson: vm.listDetailPers,
                ReferenceNo: vm.ReferenceNo
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/blacklist/formcancelblacklist.html',
                controller: 'FormCancelBlacklistCtrl',
                controllerAs: 'frmCancelBlacklistCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function (flag) {
                if (flag === true) {
                    $uibModalInstance.close();
                }
                
            });
        }

        vm.addWhite = addWhite;
        function addWhite() {
            var data = {
                act: true,
                type: "BLACKLIST_TYPE_NO",
                item: vm.data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/blacklist/FormBlacklist.html',
                controller: 'FormBlacklistCtrl',
                controllerAs: 'frmBlacklistCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                
            });
        }
    }
})();