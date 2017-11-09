using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;

namespace FMS.BLL.DocumentSequenceNumber
{
    public class DocumentSequenceNumberBLL : IDocumentSequenceNumberBLL
    {
        //private ILogger _logger;
        private IDocumentNumberService _CsfDocNumberService;
        private IUnitOfWork _uow;
        public DocumentSequenceNumberBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CsfDocNumberService = new DocumentNumberService(_uow);
        }
    }
}
