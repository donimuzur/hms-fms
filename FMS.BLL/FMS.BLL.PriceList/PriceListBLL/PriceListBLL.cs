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

namespace FMS.BLL.PriceList
{

    public class PriceListBLL : IPriceListBLL
    {
        //private ILogger _logger;
        private IPriceListService _PriceListService;
        private IUnitOfWork _uow;
        public PriceListBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _PriceListService = new PriceListService(uow);
        }

        public List<PriceListDto> GetPriceList()
        {
            var data = _PriceListService.GetPriceList();
            var retData = Mapper.Map<List<PriceListDto>>(data);
            return retData;
        }

        public PriceListDto GetExist(string Model)
        {
            var data = _PriceListService.GetExist(Model);
            var retData = Mapper.Map<PriceListDto>(data);

            return retData;
        }
        public void Save(PriceListDto PriceListDto)
        {
            var dbPriceList = Mapper.Map<MST_PRICELIST>(PriceListDto);
            _PriceListService.save(dbPriceList);
        }

        public PriceListDto GetByID(int Id)
        {
            var data = _PriceListService.GetPriceListById(Id);
            var retData = Mapper.Map<PriceListDto>(data);

            return retData;
        }


    }
}

