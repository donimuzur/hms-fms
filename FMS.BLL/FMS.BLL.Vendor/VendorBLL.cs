﻿using System;
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

namespace FMS.BLL.Vendor
{

    public class VendorBLL : IVendorBLL
    {
        //private ILogger _logger;
        private IVendorService _VendorService;
        private IUnitOfWork _uow;
        public VendorBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _VendorService = new VendorService(_uow);
        }

        public List<VendorDto> GetVendor()
        {
            var data = _VendorService.GetVendor();
            var retData = Mapper.Map<List<VendorDto>>(data);
            return retData;
        }

        public VendorDto  GetExist(string VendorName)
        {
            var data = _VendorService.GetExist(VendorName);
            var retData = Mapper.Map<VendorDto>(data);

            return retData;
        }
        public void Save(VendorDto VendorDto)
        {
            var dbVendor =Mapper.Map<MST_VENDOR>(VendorDto);
            _VendorService.save(dbVendor);
        }

        public void Save(VendorDto VendorDto, Login userLogin)
        {
            var dbVendor = Mapper.Map<MST_VENDOR>(VendorDto);
            _VendorService.save(dbVendor, userLogin);
        }

        public VendorDto GetByID(int Id)
        {
            var data = _VendorService.GetVendorById (Id);
            var retData = Mapper.Map<VendorDto>(data);
            
            return retData;
        }

        public void SaveChanges()
        {
            _uow.SaveChanges();
        }
    }
}

