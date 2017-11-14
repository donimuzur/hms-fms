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

namespace FMS.BLL.CostOb
{

    public class CostObBLL : ICostObBLL
    {
        //private ILogger _logger;
        private ICostObService _CostObService;
        private IUnitOfWork _uow;
        public CostObBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CostObService = new CostObService(uow);
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

        public CostObDto GetByID(int Id)
        {
            var data = _CostObService.GetCostObById(Id);
            var retData = Mapper.Map<CostObDto>(data);

            return retData;
        }


    }
}

