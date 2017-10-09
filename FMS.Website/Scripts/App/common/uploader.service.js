(function () {
    'use strict';

    angular.module('app').factory('UploaderService', uploadService);

    uploadService.$inject = ['$upload', 'GlobalConstantService', 'UIControlService'];

    /* @ngInject */
    function uploadService($upload, GlobalConstantService, UIControlService) {
        const REGISTRATION_UPLOAD_DIR = 'UPLOAD_DIRECTORIES_REGISTRATION';

        var storage = {
            endpoint: GlobalConstantService.getConstant('api_endpoint'),
            rootdir: 'uploads'
        };
        var service = {
            upload: upload,
            uploadVendor: uploadVendorData,
            uploadRegistration: uploadRegistration,
            uploadSingleFile: uploadSingleFile,
            uploadSingleFileContractReqSOW: uploadSingleFileContractReqSOW,
            uploadSingleFileContractReqDocument: uploadSingleFileContractReqDocument,
            uploadSingleFileLegalDocuments: uploadSingleFileLegalDocuments,
            uploadSingleFileLibrary: uploadSingleFileLibrary,
            uploadCompanyPersonID: uploadCompanyPersonID,
            uploadSingleFileRFQVHS: uploadSingleFileRFQVHS,
            uploadSingleFileSOEvaluation: uploadSingleFileSOEvaluation,
            uploadSingleFileVHSOfferEntry: uploadSingleFileVHSOfferEntry,
            uploadSingleFileGOEvaluation: uploadSingleFileGOEvaluation,
            uploadSingleFileQuestionnaireVendor: uploadSingleFileQuestionnaireVendor,
            uploadSingleFileUploadDocument: uploadSingleFileUploadDocument,
            uploadSingleFileBlacklist: uploadSingleFileBlacklist,
            uploadSingleFileAgreement: uploadSingleFileAgreement,
            uploadSingleFileBusinessConduct: uploadSingleFileBusinessConduct,
            uploadSingleFileStock: uploadSingleFileStock,
            uploadSingleFileBalance: uploadSingleFileBalance,
            uploadSingleFileCertificate: uploadSingleFileCertificate,
            uploadSingleFileBankDetail: uploadSingleFileBankDetail,
            uploadSingleFileGoodsOfferEntry: uploadSingleFileGoodsOfferEntry
        };

        function uploadVendorData(file, success, error) {
            upload(file, 'vendor', id, success, error, fields);
        }

        function uploadRegistration(file, npwp, prefix, size, types, success, error) {
            if (!file) {
                UIControlService.handleRequestError("File is required");
                UIControlService.unloadLoading();
                return;
            }

            if (!npwp) {
                UIControlService.handleRequestError('Tax Identification Number is required!');
                UIControlService.unloadLoading();
            }

            if (!prefix) {
                UIControlService.handleRequestError('Prefix is required!');
                UIControlService.unloadLoading();
                return;
            }

            if (!size) {
                UIControlService.handleRequestError('Size is required!');
                UIControlService.unloadLoading();
                return;
            }

            if (!types) {
                UIControlService.handleRequestError('List of allowed filetypes is required!');
                return;
            }

            var fields = {
                id: npwp.replace('.', '-'),
                folder: REGISTRATION_UPLOAD_DIR,
                prefix: prefix,
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFile(file, folder, size, types, dates, success, error) {
            var fields = {  
                id: dates,
                folder: folder,
                prefix: dates,
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }
        function uploadSingleFileLibrary(file, size, types, success, error) {
            var fields = {
                id: 0,
                folder: "UPLOAD_DIRECTORIES_LIBRARY",
                prefix: "LIBRARY",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }
        function uploadSingleFileContractReqSOW(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_CR_SOW",
                prefix: "CRSOW",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileContractReqDocument(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_CR_DOCS",
                prefix: "CRDOC",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileLegalDocuments(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_LEGAL_DOCS",
                prefix: "LEGAL_DOC",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileRFQVHS(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_RFQVHS",
                prefix: "RFQ_VHS",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadCompanyPersonID(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_COMPANY_PERSON",
                prefix: "COMP_PERSON",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }
        
        function uploadSingleFileSOEvaluation(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_SOEVALUATION",
                prefix: "SO_EVALUATION",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }
        function uploadSingleFileVHSOfferEntry(dates, file, size, types, success, error) {
            var fields = {
                id: dates,
                folder: "UPLOAD_DIRECTORIES_VHSOFFERENTRY",
                prefix: "VHS_OFFERENTRY",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }
        function uploadSingleFileGOEvaluation(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_GOEVALUATION",
                prefix: "GO_EVALUATION",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileUploadDocument(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_UPLOADDL",
                prefix: "UPLOAD",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileGoodsOfferEntry(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_GOODSOFFERENTRY",
                prefix: "UPLOAD",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileBankDetail(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_BANKDETAIL",
                prefix: "DETAIL",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }
        function upload(file, fields, success, error) {
            if (file) {
                var param = {
                    url: storage.endpoint + '/' + storage.rootdir,
                    file: file,
                    fields: fields
                };
                $upload.upload(param).then(success, error);
            }
        }
        function uploadSingleFileQuestionnaireVendor(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_VENDORQUESTIONNAIRE",
                prefix: "VENDORQUESTIONNAIRE",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileBlacklist(file, size, types, success, error) {
            var fields = {
                id: 0,
                folder: "UPLOAD_DIRECTORIES_BLACKLIST",
                prefix: "BLACKLIST",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileAgreement(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "DOCUMENT_TYPE_AGREEMENT",
                prefix: "AGREEMENT",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileBusinessConduct(id, file, size, types, success, error) {
            var fields = null;
            if (localStorage.getItem("currLang") === 'id') {
                fields = {
                    id: id,
                    folder: "DOCUMENT_TYPE_BUSINESSCONDUCT",
                    prefix: "BCONDUCT",
                    size: Number(size),
                    types: types
                };
            } else {
                fields = {
                    id: 0,
                    folder: "DOCUMENT_TYPE_BUSINESSCONDUCT_EN",
                    prefix: "BCONDUCT",
                    size: Number(size),
                    types: types
                };
            }
            upload(file, fields, success, error);
        }

        return service;

        function uploadSingleFileStock(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_STOCK",
                prefix: "STOCK",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }

        function uploadSingleFileBalance(id, file, size, types, success, error) {
            var fields = {
                id: id,
                folder: "UPLOAD_DIRECTORIES_BALANCE",
                prefix: "BALANCE",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }
        function uploadSingleFileCertificate(Id, file, size, types, success, error) {
            var fields = {
                id: Id,
                folder: "UPLOAD_DIRECTORIES_CERTIFICATE",
                prefix: "Tenaga Ahli",
                size: Number(size),
                types: types
            };
            upload(file, fields, success, error);
        }
    }
})();