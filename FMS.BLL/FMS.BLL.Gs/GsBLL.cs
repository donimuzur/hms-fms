using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Gs
{
    public class GsBLL : IGsBLL
    {
        private IGsService _gsService;
        private IArchGsService _ArchGsService;
        private IUnitOfWork _uow;
        public GsBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _gsService = new GsService(uow);
            _ArchGsService = new ArchGsService(uow);
        }
        public List<GsDto> GetGs()
        {
            var data = _gsService.GetGs();
            var redata = Mapper.Map<List<GsDto>>(data);
            return redata;
        }
        public GsDto GetGsById(int MstGsId)
        {
            var data = _gsService.GetGsById(MstGsId);
            var redata = Mapper.Map<GsDto>(data);
            return redata;
        }

        public void Save(GsDto Dto)
        {
            var dbGs = Mapper.Map<MST_GS>(Dto);
            _gsService.Save(dbGs);
        }

        public void Save(GsDto Dto, Login userLogin)
        {
            var dbGs = Mapper.Map<MST_GS>(Dto);
            _gsService.Save(dbGs, userLogin);
        }

        public List<GsDto> GetGsReport(RptGsInput input)
        {
            List<MST_GS> data = _gsService.GetGsByParam(input);
            var redata = Mapper.Map<List<GsDto>>(data);
            return redata;
        }
        public List<GsDto> GetGs(GSParamInput filter)
        {
            var redata = new List<GsDto>();
            if (filter.Table == "2")
            {
                var data = _ArchGsService.GetGsByParam(filter);
                redata = Mapper.Map<List<GsDto>>(data);
            }
            else
            {
                var data = _gsService.GetGsByParam(filter);
                redata = Mapper.Map<List<GsDto>>(data);
            }
           
            return redata;
        }
        public void SaveChanges()
        {
            _uow.SaveChanges();
        }
    }
}
