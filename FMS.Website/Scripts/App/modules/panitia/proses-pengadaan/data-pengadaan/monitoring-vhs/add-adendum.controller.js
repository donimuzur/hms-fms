(function () {
    'use strict';

    angular.module("app").controller("AddVHSCtrl", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataVHSService', 'UIControlService', 'GlobalConstantService', 'UploaderService', 'UploadFileConfigService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataVHSService, UIControlService, GlobalConstantService, UploaderService, UploadFileConfigService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.fileUpload;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.VHSdata;
        vm.listVHS = [];
        vm.init = init;
        var id = Number($stateParams.id);
        vm.id = Number($stateParams.id);
        vm.isCalendarOpened = [false, false, false];
        vm.datetoStart;
        vm.endDate;
        vm.Remask = '';
        vm.AdditionalValue = 0;
        vm.TypeAddendum = 1;
        vm.StartDate = '';
        vm.EndDate = '';
        vm.Duration = 0;
        vm.Requestor = 1;
        vm.RequestDate = '';
        vm.DocUrl = "";
        vm.DocName = "";

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        function init() {
            $translatePartialLoader.addPart('add-adendum');
            loadTypeSizeFile(1);
            loadData(1);
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataVHSService.AddAddendum({
                Offset: vm.pageSize * (vm.currentPage - 1),
                Status: vm.id,
                Limit: vm.pageSize,
                Column: vm.column
            },
            function (reply) {
                console.info("data:" + JSON.stringify(reply.data));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listVHS = reply.data.List;
                    vm.VHSdata = data;
                    vm.totalItems = Number(data.Count);
                    vm.VendorID = vm.listVHS[0].VendorID;
                    vm.datetoStart = vm.listVHS[0].StartContractDate;
                    vm.TenderStepID = vm.listVHS[0].TenderStepID;
                    console.info("data:" + JSON.stringify(vm.TenderStepID));
                }
                else {
                    $.growl.error({ message: "MESSAGE.ERR_LOAD" });
                    UIControlService.unloadLoading();
                }
            },
            function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };

        function loadTypeSizeFile() {
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.CONTRACTREQUISITION.DOCS", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
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
        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }
        vm.selectUpload = selectUpload;
        function selectUpload(fileUpload) {
            vm.fileUpload = fileUpload;
        }

        vm.save = save;
        function save() {
            if (!vm.RequestDate) {
                UIControlService.msg_growl("error", "Request Date Kosong");
                UIControlService.unloadLoading();

                return;
            }
            if (!vm.DocUrl && !vm.fileUpload) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                UIControlService.unloadLoading();

                return;
            }
            if (vm.fileUpload) {
                uploadFile();
            }
        }

        vm.cariPaket = cariPaket;
        function cariPaket(keyword) {
            vm.keyword = keyword;
            vm.currentPage = 1;
            loadData();
        };

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }


        /*proses upload file*/
        vm.uploadFile = uploadFile;
        function uploadFile() {
            var folder = "Addendum_" + vm.id + vm.DocName;
            if (vm.fileUpload === undefined) {
                UIControlService.msg_growl("error", "MESSAGE.MSG_NOFILE");
                return;
            }

            if (UIControlService.validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, folder);
            }
        }
        function upload(file, config, filters, folder, callback) {
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

            UIControlService.loadLoading("LOADING");
            UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_ADMIN", size, filters, folder,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        var nameDoc = "Addendum_" + vm.id;
                        vm.pathFile = vm.folderFile + url;
                        // console.info("sendata:" + JSON.stringify(fileName));

                        UIControlService.msg_growl("success", "MESSAGE.SUCCESS_UPLOAD");
                        saveProcess(url, nameDoc, size);

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
        /* end proses upload*/
        function saveProcess(docurl, docname, docsize) {
            var senddata = {
                AddendumCode: vm.AddendumCode,
                TypeAddendum: vm.TypeAddendum,
                AdditionalValue: vm.AdditionalValue,
                RequestDate: vm.RequestDate,
                DocUrl: docurl,
                DocName: docname,
                VHSAwardId: vm.VHSAwardId,
                StartDate: vm.StartDate,
                EndDate: vm.EndDate,
                Requestor: vm.Requestor,
                VendorID: vm.VendorID,
                TenderStepID: vm.TenderStepID,
                VHSAwardId: vm.id,
                Remask: vm.Remask,
                Duration: vm.Duration,

            }
            DataVHSService.CreateAddendum(senddata, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "MESSAGE.MESSAGE_SUCCESS");
                    $uibModalInstance.close();
                    //  loadData();
                }
                else {
                    UIControlService.msg_growl("error", "MESSAGE.MESSAGE_FAILED");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.CreateAddendum = CreateAddendum;
        function CreateAddendum() {
            UIControlService.loadLoading(loadmsg);
            DataVHSService.CreateAddendum({
                AddendumCode: vm.AddendumCode,
                TypeAddendum: vm.TypeAddendum,
                AdditionalValue: vm.AdditionalValue,
                RequestDate: vm.RequestDate,
                DocUrl: vm.DocUrl,
                DocName: vm.DocName,
                VHSAwardId: vm.VHSAwardId,
                StartDate: vm.StartDate,
                EndDate: vm.endDate,
                Requestor: vm.Requestor,
                VendorID: vm.VendorID,
                TenderStepID: vm.TenderStepID,
                VHSAwardId: vm.id,
                Remask: vm.Remask,
                Duration: vm.Duration,
            },
        function (reply) {
            if (reply.status === 200) {
                UIControlService.msg_growl("success", 'MESSAGE.MESSAGE_SUCCESS', "MESSAGE.TITLE_SUCCESS");
                UIControlService.unloadLoading();
            }
            else {
                UIControlService.msg_growl("error", 'MESSAGE.MESSAGE_FAILED', "MESSAGE.TITLE_FAILED");
                UIControlService.unloadLoading();
            }
        },
        function (error) {
            UIControlService.unloadLoading();
            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
        });
        }

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }

        vm.getMon = getMon;
        function getMon(startDate, duration) {
            var date = new Date(startDate);
            var oldDate = date.getDate();
            var oldMonth = date.getMonth();
            var oldYear = date.getFullYear();

            var newMonth = oldMonth + parseInt(duration);
            var endDate = new Date(oldYear, newMonth, oldDate);

            vm.endDate = new Date(oldYear, newMonth, oldDate);
            return endDate;
        };
    }
})();
