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

namespace FMS.BLL.PriceList
{

    public class PriceListBLL : IPriceListBLL
    {
        //private ILogger _logger;
        private IPriceListService _PriceListService;
        private IArchPricelistService _ArchPriceListService;
        private IUnitOfWork _uow;
        public PriceListBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _PriceListService = new PriceListService(uow);
            _ArchPriceListService = new ArchPricelistService(uow);
        }

        public List<PriceListDto> GetPriceList()
        {
            var data = _PriceListService.GetPriceList();
            var retData = Mapper.Map<List<PriceListDto>>(data);
            return retData;
        }
        public List<PriceListDto> GetPriceList(PricelistParamInput filter)
        {
            var redata = new List<PriceListDto>();
            if (filter.Table == "2")
            {
                var data = _ArchPriceListService.GetPriceList(filter);
                redata = Mapper.Map<List<PriceListDto>>(data);
            }
            else
            {
                var data = _PriceListService.GetPriceList(filter);
                redata = Mapper.Map<List<PriceListDto>>(data);
            }
            return redata;
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
        public void Save(PriceListDto PriceListDto, Login userLogin)
        {
            var dbPriceList = Mapper.Map<MST_PRICELIST>(PriceListDto);
            _PriceListService.save(dbPriceList, userLogin);
        }
        public void SaveChanges()
        {
            _uow.SaveChanges();
        }
        public PriceListDto GetByID(int Id, bool? Archive = null)
        {
            if (Archive.HasValue)
            {
                var archData = _ArchPriceListService.GetPriceListById(Id);
                return Mapper.Map<PriceListDto>(archData);
            }
            var data = _PriceListService.GetPriceListById(Id);
            var retData = Mapper.Map<PriceListDto>(data);

            return retData;
        }


    }
}

