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
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.Penalty
{
    public class PenaltyBLL : IPenaltyBLL
    {
        //private ILogger _logger;
        private IPenaltyService _penaltyService;
        private IRoleService _roleService;
        private IUnitOfWork _uow;

        public PenaltyBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _penaltyService = new PenaltyService(uow);
        }

        public List<PenaltyDto> GetPenalty()
        {
            var data = _penaltyService.GetPenalty();
            var retData = Mapper.Map<List<PenaltyDto>>(data);
            return retData;
        }

        public PenaltyDto GetByID(string Id)
        {
            var data = _penaltyService.GetPenaltyById(Id);
            var retData = Mapper.Map<PenaltyDto>(data);

            return retData;
        }

        public PenaltyDto GetExist(int MstPenaltyId)
        {
            var data = _penaltyService.GetExist(MstPenaltyId);
            var retData = Mapper.Map<PenaltyDto>(data);

            return retData;
        }
        public void Save(PenaltyDto PenaltyDto)
        {
            var dbPenalty = Mapper.Map<MST_PENALTY>(PenaltyDto);
            _uow.GetGenericRepository<MST_PENALTY>().InsertOrUpdate(dbPenalty);
            _uow.SaveChanges();
        }
    }
}
