using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.SalesVolume
{
    public class SalesVolumeBLL : ISalesVolumeBLL
    {
        private ISalesVolumeService _SalesVolumeService;
        private IArchSalesVolumeService _archSalesVolumeService;
        private IUnitOfWork _uow;
        public SalesVolumeBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _SalesVolumeService = new SalesVolumeService(uow);
            _archSalesVolumeService = new ArchSalesVolumeService(_uow);
        }

        public List<SalesVolumeDto> GetSalesVolume(SalesVolumeParamInput inputs)
        {
            var retData = new List<SalesVolumeDto>();
            if(inputs.Table =="2")
            {
                var data = _archSalesVolumeService.GetSalesVolume(inputs);
                retData = Mapper.Map<List<SalesVolumeDto>>(data);
            }
            else
            {
                var data = _SalesVolumeService.GetSalesVolume(inputs);
                retData = Mapper.Map<List<SalesVolumeDto>>(data);
            }
            return retData;
        }

        public List<SalesVolumeDto> GetAllSalesVolume(SalesVolumeParamInput input = null)
        {
            var retData = new List<SalesVolumeDto>();

            if (input != null && input.Table == "2")
            {
                var data = _archSalesVolumeService.GetAllSalesVolume();
                retData = Mapper.Map<List<SalesVolumeDto>>(data);
            }
            else
            {
                var data = _SalesVolumeService.GetAllSalesVolume();
                retData = Mapper.Map<List<SalesVolumeDto>>(data);
            }
            return retData;
        }

        public SalesVolumeDto GetSalesVolumeById(int MstSalesVolumeId, bool? Archive = null)
        {
            var retData = new SalesVolumeDto();
            if (Archive.HasValue)
            {
                var data = _archSalesVolumeService.GetSalesVolumeById(MstSalesVolumeId);
                retData = Mapper.Map<SalesVolumeDto>(data);
            }
            else
            {
                var data = _SalesVolumeService.GetSalesVolumeById(MstSalesVolumeId);
                retData = Mapper.Map<SalesVolumeDto>(data);
            }
            return retData;
        }

        public void Save(SalesVolumeDto SalesVolumeDto)
        {
            var dbSalesVolume = Mapper.Map<MST_SALES_VOLUME>(SalesVolumeDto);
            _SalesVolumeService.save(dbSalesVolume);
        }
        public void CheckSalesVolume(String Type, String Region, int Month, int Year, String User)
        {
            _SalesVolumeService.CheckSalesVolume(Type, Region, Month, Year, User);
        }
    }
}
