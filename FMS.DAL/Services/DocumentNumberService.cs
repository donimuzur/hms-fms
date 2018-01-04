using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;

namespace FMS.DAL.Services
{
    public class DocumentNumberService : IDocumentNumberService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<TRA_DOC_NUMBER> _docNumberRepository;
        private IGenericRepository<MST_DOCUMENT_TYPE> _docTypeRepository;

        public DocumentNumberService(IUnitOfWork uow)
        {
            _uow = uow;
            _docNumberRepository = _uow.GetGenericRepository<TRA_DOC_NUMBER>();
            _docTypeRepository = _uow.GetGenericRepository<MST_DOCUMENT_TYPE>();
        }

        public string GenerateNumber(GenerateDocNumberInput input)
        {
            while (true)
            {
                TRA_DOC_NUMBER docSeqNumberToInsert;

                var docType = _docTypeRepository.Get(x => x.MST_DOCUMENT_TYPE_ID == input.DocType).FirstOrDefault().DOCUMENT_INISIAL;

                var lastSeqData = _docNumberRepository.Get(c => c.DOCUMENT_TYPE == input.DocType && c.MONTH == input.Month && c.YEAR == input.Year).Count();
                lastSeqData = lastSeqData + 1;

                var docNumber = lastSeqData.ToString("000");

                //generate number
                docNumber = docType + "/" + input.Year + "/" + input.Month.ToString("00") + "/" + docNumber;

                docSeqNumberToInsert = new TRA_DOC_NUMBER()
                {
                    MONTH = input.Month,
                    YEAR = input.Year,
                    DOCUMENT_NUMBER = docNumber,
                    DOCUMENT_TYPE = input.DocType
                };

                _docNumberRepository.Insert(docSeqNumberToInsert);

                return docNumber;
            }
        }
    }
}
