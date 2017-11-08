using AutoMapper;
using FMS.BusinessObject;
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

namespace FMS.BLL.Ctf
{
    public class CtfBLL : ITraCtfBLL
    {
        private IUnitOfWork _uow;
        private ICtfService _ctfService;

        public CtfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ctfService = new CtfService(uow);
        }

        public List<TraCtfDto> GetCtf()
        {
            var data = _ctfService.GetCtf();
            var redata = Mapper.Map<List<TraCtfDto>>(data);
            return redata;
        }

        public void Save(TraCtfDto Dto)
        {
            var db = Mapper.Map<TRA_CTF>(Dto);
            _ctfService.Save(db);
        }
    }
}
