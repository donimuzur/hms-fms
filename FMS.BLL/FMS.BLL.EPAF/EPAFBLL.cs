using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Core;
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.EPAF
{
    public class EPAFBLL : IEpafBLL
    {
        private IEpafService _epafService;
        private IUnitOfWork _uow;
        public EPAFBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _epafService = new EpafService(uow);
        }

        public List<EpafDto> GetEpaf()
        {
            var data = _epafService.GetEpaf();
            var retData = Mapper.Map<List<EpafDto>>(data);
            return retData;
        }


        public List<EpafDto> GetEpafByDocType(Enums.DocumentType docType)
        {
            var data = _epafService.GetEpafByDocumentType(docType);
            var retData = Mapper.Map<List<EpafDto>>(data);
            return retData;
        }
    }
}
