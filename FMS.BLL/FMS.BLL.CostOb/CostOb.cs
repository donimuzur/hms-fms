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
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.CostOb
{

    public class CostObBLL : ICostObBLL
    {
        //private ILogger _logger;
        private ICostObService _CostObService;
        private IArchCostObService _ArchCostObService;
        private IUnitOfWork _uow;
        public CostObBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CostObService = new CostObService(uow);
            _ArchCostObService = new ArchCostObService(uow);
        }

        public List<CostObDto> GetCostOb()
        {
            var data = _CostObService.GetCostOb();
            var retData = Mapper.Map<List<CostObDto>>(data);
            return retData;
        }

        public CostObDto GetExist(string Model)
        {
            var data = _CostObService.GetExist(Model);
            var retData = Mapper.Map<CostObDto>(data);

            return retData;
        }
        public void Save(CostObDto CostObDto)
        {
            var dbCostOb = Mapper.Map<MST_COST_OB>(CostObDto);
            _CostObService.save(dbCostOb);
        }

        public void Save(CostObDto CostObDto, Login userLogin)
        {
            var dbCostOb = Mapper.Map<MST_COST_OB>(CostObDto);
            _CostObService.save(dbCostOb, userLogin);
        }
        public void SaveChanges()
        {
            _uow.SaveChanges();
        }
        public CostObDto GetByID(int Id, bool? Archived = null)
        {
            var retData = new CostObDto();
            if (Archived.HasValue)
            {
                var data = _uow.GetGenericRepository<ARCH_MST_COST_OB>().GetByID(Id);
                retData = Mapper.Map<CostObDto>(data);
            }
            else
            {
                var data = _CostObService.GetCostObById(Id);
                retData = Mapper.Map<CostObDto>(data);
            }

            return retData;
        }

        public List<CostObDto> GetByFilter(CostObParamInput filter)
        {
            var redata = new List<CostObDto>();
            if (filter.Table == "2")
            {
                var data = _ArchCostObService.GetCostObByFilter(filter);
                redata = Mapper.Map<List<CostObDto>>(data);
            }
            else
            {
                var data = _CostObService.GetCostObByFilter(filter);
                redata = Mapper.Map<List<CostObDto>>(data);
            }
            return redata;
        }
    }
}

