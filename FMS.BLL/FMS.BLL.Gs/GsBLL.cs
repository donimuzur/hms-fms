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

namespace FMS.BLL.Gs
{
    public class GsBLL : IGsBLL
    {
        private IGsService _gsService;
        private IUnitOfWork _uow;
        public GsBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _gsService = new GsService(uow);
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
    }
}
