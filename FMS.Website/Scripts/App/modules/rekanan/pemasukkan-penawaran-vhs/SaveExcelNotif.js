(function () {
    'use strict';

    angular.module("app")
            .controller("SaveNotifExcel", ctrl);

    ctrl.$inject = ['$timeout','Excel','$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PPVHSService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($timeout,Excel, $http, $translate, $translatePartialLoader, $location, SocketService, PPVHSService, UploadFileConfigService,
        UIControlService, UploaderService, item, $uibModalInstance, GlobalConstantService) {

        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.currentPage = 1;
        vm.TenderRefID = item.TenderRefID;
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
        function init() {
            loadGenerateTemplate();
            
        }

        vm.Export = Export;
        function Export(tableId) {
            //vm.exportHref = Excel.tableToExcel(tableId, 'sheet name');
            //$timeout(function () { location.href = vm.exportHref; $uibModalInstance.close(); }, 100); // trigger download
            for (var i = 0; i < vm.vendor.length; i++) {
                var iplus = i + 1;
                JSONToCSVConvertor(vm.vendor[i], true, iplus);
            }
        }

        vm.JSONToCSVConvertor = JSONToCSVConvertor;
        function JSONToCSVConvertor(JSONData, ShowLabel, idata) {
            //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
            var arrData = JSONData;
            console.info(arrData[0]);
            var CSV = '';

            //This condition will generate the Label/Header
            if (ShowLabel) {
                var row = "";

                //This loop will extract the label from 1st index of on array
                for (var index in arrData[0]) {

                    //Now convert each value to string and comma-seprated
                    row += index + ',';
                }

                row = row.slice(0, -1);
                console.info(row);
                //append Label row with line break
                CSV += row + '\r\n';
                console.info(CSV);
            }

            //1st loop is to extract each row
            for (var i = 0; i < arrData.length; i++) {
                var row = "";

                //2nd loop will extract each column and convert it in string comma-seprated
                for (var index in arrData[i]) {
                    row += '"' + arrData[i][index] + '",';
                }

                row.slice(0, row.length - 1);

                //add a line break after each row
                CSV += row + '\r\n';
            }

            if (CSV == '') {        
                alert("Invalid data");
                return;
            }   

            //Generate a file name
            var fileName = "Dokumen Penawaran_"+idata;

            //Initialize file format you want csv or xls
            var uri = 'data:text/csv;charset=utf-8,' + escape(CSV);

            // Now the little tricky part.
            // you can use either>> window.open(uri);
            // but this will not work in some browsers
            // or you will not get the correct file extension    

            //this trick will generate a temp <a /> tag
            var link = document.createElement("a");    
            link.href = uri;

            //set the visibility hidden so it will not effect on your web-layout
            link.style = "visibility:hidden";
            link.download = fileName + ".csv";

            //this part will append the anchor tag and remove it after automatic click
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            $uibModalInstance.close();
        }


        vm.loadGenerateTemplate = loadGenerateTemplate;
        function loadGenerateTemplate() {
            PPVHSService.selectTemplate({ Status: vm.TenderRefID }, function (reply) {

                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.vendor = reply.data;
                    
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data" });
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
            $uibModalInstance.dismiss('cancel');
        };

       
    }
})();