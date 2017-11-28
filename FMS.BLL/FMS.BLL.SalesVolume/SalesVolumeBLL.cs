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

namespace FMS.BLL.SalesVolume
{
    public class SalesVolumeBLL : ISalesVolumeBLL
    {
        private ISalesVolumeService _SalesVolumeService;
        private IUnitOfWork _uow;
        public SalesVolumeBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _SalesVolumeService = new SalesVolumeService(uow);
        }

        public List<SalesVolumeDto> GetSalesVolume()
        {
            var data = _SalesVolumeService.GetSalesVolume();
            var retData = Mapper.Map<List<SalesVolumeDto>>(data);
            return retData;
        }

        public SalesVolumeDto GetSalesVolumeById(int MstSalesVolumeId)
        {
            var data = _SalesVolumeService.GetSalesVolumeById(MstSalesVolumeId);
            var retData = Mapper.Map<SalesVolumeDto>(data);
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
