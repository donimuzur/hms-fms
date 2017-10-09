(function () {
    'use strict';

    angular.module("app").controller("PPGVHSCtrl", ctrl);

    ctrl.$inject = ['$filter','Excel', '$timeout', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PenetapanPemenangVHSservice', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($filter, Excel, $timeout, $http, $translate, $translatePartialLoader, $location, SocketService, PenetapanPemenangVHSservice,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        var page_id = 141;
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.evalsafety = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.kata = new Kata("");
        vm.init = init;
        vm.exportHref;
        vm.detail = [];
        vm.jLoad = jLoad;
        vm.isCalendarOpened = [false, false, false, false];

        function init() {
           
            UIControlService.loadLoading("Silahkan Tunggu...");
            getUserLogin();
            jLoad(1);
            //convertToDate();
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            var tender = {
                column: vm.StepID,
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            PenetapanPemenangVHSservice.select(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detail = reply.data.List;
                    for (var i = 0; i < vm.detail.length; i++) {
                        if (vm.detail[i].ApprovalStatusReff == null) {
                            vm.detail[i].Status = 'Draft';
                            vm.detail[i].flagTemp = 0;
                        }
                        else if (vm.detail[i].ApprovalStatusReff != null) {
                            cekEmployee(vm.detail[i].ID, vm.detail[i]);
                            vm.detail[i].Status = vm.detail[i].ApprovalStatusReff.Value;
                            vm.detail[i].flagTemp = 1;
                        }
                        vm.detail[i].StartContractDate = new Date(Date.parse(vm.detail[i].StartContractDate));
                    }
                    vm.count = reply.data.Count;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Peneapan Pemenang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.getUserLogin = getUserLogin;
        function getUserLogin() {
            PenetapanPemenangVHSservice.CekRequestor({
                TenderRefID: vm.TenderRefID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.flagSave = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.createExcel = createExcel;
        function createExcel() {
            var data = {
                StepID: vm.StepID,
            TenderRefID: vm.TenderRefID,
            ProcPackType: vm.ProcPackType
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/penetapan-pemenang-vhs-fpa/SaveExcelNotif.html',
                controller: "SaveNotifExcel",
                controllerAs: "SaveNotifExcel",
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

        vm.cekEmployee = cekEmployee;
        function cekEmployee(Id, reff) {
            PenetapanPemenangVHSservice.CekEmployee({
                ID: Id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    reff.flagEmp = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString(data) { // TIMEZONE (-)
            if (data) {
                data = UIControlService.getStrDate(data);
            }
        };


        //supaya muncul di date picker saat awal load
        function convertToDate() {
            if (vm.detail[0].StartContractDate) {
                vm.detail[0].StartContractDate = new Date(Date.parse(vm.detail[0].StartContractDate));
            }
        }

        


        vm.simpan = simpan;
        vm.List = [];
        function simpan() {
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
            for (var i = 0; i < vm.detail.length; i++) {
                convertAllDateToString(vm.detail[i].StartContractDate);

                
                var dataVHS = {
                    ID: vm.detail[i].ID,
                    durrA: vm.detail[i].durrA,
                    durrB: vm.detail[i].durrB,
                    VendorID: vm.detail[i].VendorID,
                    Duration: vm.detail[i].Duration,
                    TenderStepID: vm.detail[i].TenderStepID,
                    StartContractDate: vm.detail[i].StartContractDate,
                    SAPContractNo: vm.detail[i].SAPContractNo,
                    RFQVHSId: vm.TenderRefID,
                    RFQType: vm.detail[i].RFQType
                }
                vm.List.push(dataVHS);
                if (i == vm.detail.length - 1 && vm.List.length === vm.detail.length) {
                    PenetapanPemenangVHSservice.update(vm.List, function (reply) {
                        UIControlService.unloadLoadingModal();
                        if (reply.status === 200) {
                            vm.ListDeal = [];
                            vm.flag = false;
                            UIControlService.msg_growl("success", "Berhasil Simpan Data PO!!");
                             init();

                        }
                        else {
                            UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                            return;
                        }
                    }, function (err) {
                        UIControlService.msg_growl("error", "Gagal Akses Api!!");
                        UIControlService.unloadLoadingModal();
                    });
                }
            }

        }

        function sendMail(data) {
            var email = {
                subject: 'Notifikasi Pemenang ',
                mailContent: 'Selamat Anda adalah pemenang ',
                isHtml: false,
                addresses: data
            };

            UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
            PenetapanPemenangVHSservice.sendMail(email,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        loadNotDeal();
                        UIControlService.msg_growl("notice", "Email Telah dikirim ke vendor pemenang")
                    } else {
                        UIControlService.handleRequestError(response.data);
                    }
                },
                function (response) {
                    UIControlService.handleRequestError(response.data);
                    UIControlService.unloadLoading();
                });
        }

        vm.loadNotDeal = loadNotDeal;
        function loadNotDeal() {
            var model = {
                column: vm.StepID,
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }
            PenetapanPemenangVHSservice.selectVendorNotDeal(model, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detailNotDeal = reply.data;
                    if (vm.detailNotDeal.length != 0) {
                        vm.ListNotDeal = [];
                        for (var j = 0; j < vm.detailNotDeal.length; j++) {
                            vm.ListNotDeal.push(vm.detailNotDeal[j].Email);
                        }
                        var email = {
                            subject: 'Notifikasi Pemenang Tender' + vm.detail[0].TenderName,
                            mailContent: 'Maaf anda belum berhasil memenangkan Tender' + vm.detail[0].TenderName,
                            isHtml: false,
                            addresses: vm.ListNotDeal
                        };

                        UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
                        PenetapanPemenangVHSservice.sendMail(email,
                            function (response) {
                                UIControlService.unloadLoading();
                                if (response.status == 200) {
                                    UIControlService.msg_growl("notice", "Email Telah dikirim ke vendor kalah");
                                    window.location.reload();
                                } else {
                                    UIControlService.handleRequestError(response.data);
                                }
                            },
                            function (response) {
                                UIControlService.handleRequestError(response.data);
                                UIControlService.unloadLoading();
                            });
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Peneapan Pemenang" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.sendToApprove = sendToApprove;
        function sendToApprove(data) {
            bootbox.confirm($filter('translate')('Apakah anda ingin mengirim data approval ini untuk proses persetujuan?'), function (yes) {
                if (yes) {
                    UIControlService.loadLoading("MESSAGE.LOADING");
                    var dt = {
                        ID: data.ID,
                        TenderStepID: vm.StepID,
                        flagEmp: 1
                    };
                    PenetapanPemenangVHSservice.SendApproval(dt, function (reply) {
                        if (reply.status === 200) {
                            console.info(reply.data);
                            //for (var i = 0; i < vm.detail.length; i++) {
                            //    if (vm.detail[i].ID == 0) {
                            //        vm.flag = true;
                            //        vm.ListDeal.push(vm.detail[i].Email);
                            //    }
                            //}
                            //if (vm.flag === true) sendMail(vm.ListDeal);
                            //else
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SEND_TO_APPRV'));
                            init();
                        } else {
                            UIControlService.unloadLoading();
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
                        }
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
                    });
                }
            });
        }
        //vm.simpan = simpan;
        //vm.List = [];
        //function simpan() {
        //    UIControlService.loadLoadingModal("Silahkan Tunggu...");
            
        //    for (var i = 0; i < vm.detail.length; i++) {
        //        //if (vm.detail[i].SAPContractNo === null) {
        //        //    UIControlService.msg_growl("warning", "No Kontrak SAP belum diisi !!");
        //        //    return;
        //        //}
        //        vm.detail[i].StartContractDate = UIControlService.getStrDate(vm.detail[i].StartContractDate);

        //        //if (vm.detail[i].StartContractDate === null) {

        //        //    UIControlService.msg_growl("warning", "Tanggal Mulai Kontrak belum diisi !!");
        //        //    return;
        //        //}
        //        //if (vm.detail[i].Duration === null) {

        //        //    UIControlService.msg_growl("warning", "Durasi Kontrak belum diisi !!");
        //        //    return;
        //        //}
        //        //if (vm.detail[i].durrA === null || vm.detail[i].durrB === null) {

        //        //    UIControlService.msg_growl("warning", "Durasi Kontrak PTVI belum diisi !!");
        //        //    return;
        //        //}
        //        var dataGoods = {
        //            ID: vm.detail[0].ID,
        //            durrA: vm.detail[0].durrA,
        //            durrB: vm.detail[0].durrB,
        //            VendorID: vm.detail[0].VendorID,
        //            Duration: vm.detail[0].Duration,
        //            TenderStepID: vm.detail[0].TenderStepID,
        //            StartContractDate: vm.detail[0].StartContractDate,
        //            SAPContractNo: vm.detail[0].SAPContractNo,
        //            RFQVHSId: vm.TenderRefID,
        //            RFQType: vm.detail[0].RFQType

        //        }
        //        vm.List.push(dataGoods);
        //        if (i == vm.detail.length - 1 && vm.List.length === vm.detail.length) {

        //            PenetapanPemenangVHSservice.update(vm.List, function (reply) {
        //                // UIControlService.unloadLoadingModal();
        //                if (reply.status === 200) {
        //                    UIControlService.msg_growl("success", "Berhasil Simpan Data Pemenang VHS/FPA!!");
        //                    init();
        //                }
        //                else {
        //                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
        //                    return;
        //                }
        //            }, function (err) {
        //                UIControlService.msg_growl("error", "Gagal Akses Api!!");
        //                //UIControlService.unloadLoadingModal();
        //            });
        //        }
        //    }
        //}


        vm.edit = edit;
        function edit(dataTabel) {
            console.info(dataTabel);
            if (dataTabel.DocumentUrl !== null) {
                var data = {
                    act: true,
                    item: dataTabel,
                    reff: vm.TenderRefID,
                    StepID: vm.StepID

                }
            }
            else {

                var data = {
                    act: false,
                    item: dataTabel,
                    reff: vm.TenderRefID,
                    StepID: vm.StepID
                }
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/penetapan-pemenang-vhs-fpa/ubah-data.html',
                controller: 'UbahPPGVHSCtrl',
                controllerAs: 'UbahPPGVHSCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

        vm.Export = Export;
        function Export(tableId) {
            vm.exportHref = Excel.tableToExcel(tableId, 'sheet name');
            $timeout(function () { location.href = vm.exportHref; }, 100); // trigger download
        }

        vm.detailApproval = detailApproval;
        function detailApproval(dt, data) {
            var item = {
                ID: data.ID,
                flag: data.flagEmp,
                Status: dt
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/penetapan-pemenang-vhs-fpa/detailApproval.modal.html',
                controller: 'detailApprovalSignOffCtrl',
                controllerAs: 'detailApprovalSignOffCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function () {
                init();
            });
        };

        vm.JSONToCSVConvertor = JSONToCSVConvertor;
        function JSONToCSVConvertor(JSONData, ReportTitle, ShowLabel) {
            //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
            var arrData = JSONData;
            console.info(arrData[0]);
            var CSV = '';    
            //Set Report title in first row or line

            CSV += ReportTitle + '\r\n\n';

            //This condition will generate the Label/Header
            if (ShowLabel) {
                var row = "";

                //This loop will extract the label from 1st index of on array
                for (var index in arrData[0]) {

                    //Now convert each value to string and comma-seprated
                    row += index + ',';
                }

                //row = row.slice(0, -1);
                //console.info(row);
                ////append Label row with line break
                //CSV += row + '\r\n';
                //console.info(CSV);
            }

            ////1st loop is to extract each row
            //for (var i = 0; i < arrData.length; i++) {
            //    var row = "";

            //    //2nd loop will extract each column and convert it in string comma-seprated
            //    for (var index in arrData[i]) {
            //        row += '"' + arrData[i][index] + '",';
            //    }

            //    row.slice(0, row.length - 1);

            //    //add a line break after each row
            //    CSV += row + '\r\n';
            //}

            //if (CSV == '') {        
            //    alert("Invalid data");
            //    return;
            //}   

            ////Generate a file name
            //var fileName = "MyReport_";
            ////this will remove the blank-spaces from the title and replace it with an underscore
            //fileName += ReportTitle.replace(/ /g,"_");   

            ////Initialize file format you want csv or xls
            //var uri = 'data:text/csv;charset=utf-8,' + escape(CSV);

            //// Now the little tricky part.
            //// you can use either>> window.open(uri);
            //// but this will not work in some browsers
            //// or you will not get the correct file extension    

            ////this trick will generate a temp <a /> tag
            //var link = document.createElement("a");    
            //link.href = uri;

            ////set the visibility hidden so it will not effect on your web-layout
            //link.style = "visibility:hidden";
            //link.download = fileName + ".csv";

            ////this part will append the anchor tag and remove it after automatic click
            //document.body.appendChild(link);
            //link.click();
            //document.body.removeChild(link);
        }


    }
})();
//TODO

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

