using AutoMapper;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.DocumentType
{
    public class DocumentTypeBLL : IDocumentTypeBLL
    {
        public IDocumentTypeService _documentTypeService;
        public IUnitOfWork _uow;

        public DocumentTypeBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _documentTypeService = new DocumentTypeService(_uow);
        }

        public List<DocumentTypeDto> GetDocumentType()
        {
            var data = _documentTypeService.GetDocumentType();
            var redata = Mapper.Map<List<DocumentTypeDto>>(data);

            return redata;
        }

        public DocumentTypeDto GetDocTypeById(int MstDocType)
        {
            var data = _documentTypeService.GetDocTypeById(MstDocType);
            var redata = Mapper.Map<DocumentTypeDto>(data);

            return redata;
        }


    }
}
