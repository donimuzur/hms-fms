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

namespace FMS.BLL.Reason
{
    public class ReasonBLL : IReasonBLL
    {
        public IReasonService _ReasonBLL;
        public IUnitOfWork _uow;
        public ReasonBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ReasonBLL = new ReasonService(_uow);
        }

        public List<ReasonDto> GetReason()
        {
            var data = _ReasonBLL.GetReason();
            var redata = Mapper.Map<List<ReasonDto>>(data);

            return redata;
        }

        public void save(ReasonDto ReasonDto)
        {
            var dbReason = Mapper.Map<MST_REASON>(ReasonDto);
            _ReasonBLL.save(dbReason);
        }

        public ReasonDto GetReasonById(int MstReasonId)
        {
            var data = _ReasonBLL.GetReasonById(MstReasonId);
            var redata = Mapper.Map<ReasonDto>(data);

            return redata;
        }
    }
}
