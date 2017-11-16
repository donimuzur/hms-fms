using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
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

namespace FMS.BLL.PenaltyLogic
{
    public class PenaltyLogicBLL : IPenaltyLogicBLL
    {
        private IPenaltyLogicService _penaltyLogicService;
        private IUnitOfWork _uow;

        public PenaltyLogicBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _penaltyLogicService = new PenaltyLogicService(_uow);
        }
        public PenaltyLogicDto GetPenaltyLogicById(int MstPealtyLogicById)
        {
            var data = _penaltyLogicService.GetPenaltyLogicByID(MstPealtyLogicById);
            var redata = Mapper.Map<PenaltyLogicDto>(data);
            return redata;
        }
        public List<PenaltyLogicDto> GetPenaltyLogic()
        {
            var data = _penaltyLogicService.GetPenaltyLogic();
            var redata = Mapper.Map<List<PenaltyLogicDto>>(data);
            return redata;
        }
        public void Save (PenaltyLogicDto Dto)
        {
            var dbPenaltyLogic = Mapper.Map<MST_PENALTY_LOGIC>(Dto);
            _penaltyLogicService.Save(dbPenaltyLogic);
        }
        public void Save(PenaltyLogicDto Dto, Login userLogin)
        {
            var dbPenaltyLogic = Mapper.Map<MST_PENALTY_LOGIC>(Dto);
            _penaltyLogicService.Save(dbPenaltyLogic, userLogin);
        }

    }
}
