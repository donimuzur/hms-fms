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
using System.Data.Entity.Core.EntityClient;

namespace FMS.BLL.CarComplaintForm
{
    public class CarComplaintFormBLL : ICarComplaintFormBLL
    {
        private ICarComplaintFormService _ccf;
        private IUnitOfWork _uow;

        public CarComplaintFormBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ccf = new CarComplaintFormService(uow);
        }

        public List<CarComplaintFormDto> GetCCF()
        {
            var data = _ccf.GetCCF();
            var retData = Mapper.Map<List<CarComplaintFormDto>>(data);
            return retData;
        }

        public CarComplaintFormDto GetCCFByID(int Id)
        {
            var data = _ccf.GetCCFById(Id);
            var retData = Mapper.Map<CarComplaintFormDto>(data);

            return retData;
        }

        public void Save(CarComplaintFormDto CCFDto)
        {
            var dbCCF = Mapper.Map<TRA_CCF>(CCFDto);
            _ccf.save(dbCCF);
        }

        public List<CarComplaintFormDto> GetFleetByEmployee(string EmployeeId)
        {
            FMSEntities context = new FMSEntities();
            var query = (from a in context.MST_FLEET
                         where a.EMPLOYEE_ID == EmployeeId

                         select new CarComplaintFormDto()
                         {
                             Manufacturer = a.MANUFACTURER,
                             Models = a.MODEL,
                             Series = a.SERIES,
                             Vendor = a.VENDOR_NAME,
                             StartPeriod = a.START_CONTRACT,
                             EndPeriod = a.END_CONTRACT
                         });
            var dbResult = query.ToList();
            return dbResult;
        }

        public CarComplaintFormDto GetFleetByPoliceNumber(string Id)
        {
            FMSEntities context = new FMSEntities();
            var query = (from a in context.MST_FLEET
                         where a.POLICE_NUMBER == Id

                         select new CarComplaintFormDto()
                         {
                             VehicleType = a.VEHICLE_TYPE,
                             VehicleUsage = a.VEHICLE_USAGE
                         });
            var dbResult = query.ToList();
            var dbresult2 = Mapper.Map<CarComplaintFormDto>(dbResult);
            return dbresult2;
        }
    }
}
