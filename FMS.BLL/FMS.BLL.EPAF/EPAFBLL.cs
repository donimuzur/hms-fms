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
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.EPAF
{
    public class EPAFBLL : IEpafBLL
    {
        private IEpafService _epafService;
        private IArchEpafService _archEpafService;
        private IUnitOfWork _uow;
        public EPAFBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _epafService = new EpafService(_uow);
            _archEpafService = new ArchEpafService(_uow);
        }

        public List<EpafDto> GetEpaf(bool? Archived = null)
        {
            var retData = new List<EpafDto>();
            if(Archived.HasValue)
            {
                var data = _archEpafService.GetEpaf();
                retData = Mapper.Map<List<EpafDto>>(data);
            }
            else
            {
                var data = _epafService.GetEpaf();
                retData = Mapper.Map<List<EpafDto>>(data);
            }
            return retData;
        }

        public List<EpafDto> GetEpaf(EpafParamInput filter)
        {
            var retData = new List<EpafDto>();
            if(filter.Table == "2")
            {
                var data = _archEpafService.GetEpaf(filter);
                retData = Mapper.Map<List<EpafDto>>(data);
            }
            else
            {
                var data = _epafService.GetEpaf(filter);
                retData = Mapper.Map<List<EpafDto>>(data);
            }
            
            return retData;
        }
        public List<EpafDto> GetEpafByDocType(Enums.DocumentType docType)
        {
            var data = _epafService.GetEpafByDocumentType(docType);
            var retData = Mapper.Map<List<EpafDto>>(data);
            return retData;
        }
        public void DeactivateEpaf(long epafId, int Remark, string user)
        {
            _epafService.DeactivateEpaf(epafId, Remark, user);
        }


        public EpafDto GetEpafById(long? epafId, bool? ArchiveData = null)
        {
            var retData = new EpafDto();
            if(ArchiveData.HasValue)
            {
                var data = _archEpafService.GetEpafById(epafId);
                retData = Mapper.Map<EpafDto>(data);
            }
            else
            {
                var data = _epafService.GetEpafById(epafId);
                retData = Mapper.Map<EpafDto>(data);
            }
            
            return retData;
        }
    }
}
