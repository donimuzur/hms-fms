using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private IGenericRepository<MST_DOCUMENT_TYPE> _documentReposito;
        private IUnitOfWork _uow;

        public DocumentTypeService(IUnitOfWork uow)
        {
            _uow = uow;
            _documentReposito = uow.GetGenericRepository<MST_DOCUMENT_TYPE>();
        }

        public List<MST_DOCUMENT_TYPE> GetDocumentType()
        {
            return _documentReposito.Get().ToList();
        }
        
        public MST_DOCUMENT_TYPE GetDocTypeById(int MstDocType)
        {
            return _documentReposito.GetByID(MstDocType);
        }
    }
}
