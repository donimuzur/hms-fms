using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.DAL.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;

namespace FMS.BLL.CtfExtend
{
    public class CtfExtendBLL : ICtfExtendBLL
    {
        private ICtfExtendService _CtfExtendService;
        private IUnitOfWork _uow;

        public CtfExtendBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CtfExtendService =new  CtfExtendService(uow);
        }

        public void Save(CtfExtendDto CtfExtendDto)
        {
            var db = Mapper.Map<TRA_CTF_EXTEND>(CtfExtendDto);
            _CtfExtendService.Save(db);
        }

        public List<CtfExtendDto> GetCtfExtend()
        {
            var data = _CtfExtendService.GetCtfExtend();
            var redata = Mapper.Map<List<CtfExtendDto>>(data);
            return redata;
        }
    }
}
